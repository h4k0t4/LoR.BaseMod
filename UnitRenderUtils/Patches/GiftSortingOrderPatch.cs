using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace ExtendedLoader
{
	[HarmonyPatch]
	class GiftSortingOrderPatch
	{
		[HarmonyPatch(typeof(GiftAppearance), nameof(GiftAppearance.RefreshAppearance))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> GiftAppearance_RefreshAppearance_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = instructions.ToList();
			for (int i = 1; i < codes.Count; i++)
			{
				if (codes[i - 1].IsLdloc(3) && codes[i].LoadsConstant(100L))
				{
					codes.InsertRange(i - 1, new CodeInstruction[]
					{
						new CodeInstruction(OpCodes.Ldarg_2),
						new CodeInstruction(OpCodes.Ldloca, 1),
						new CodeInstruction(OpCodes.Ldloca, 2),
						new CodeInstruction(OpCodes.Ldloca, 4),
						new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GiftSortingOrderPatch), nameof(GiftSortingOrderPatch.GiftAppearance_RefreshAppearance_FixLayers)))
					});
					break;
				}
			}
			return codes;
			/*
			var indexes = new object[4];
			bool found = false;
			for (var i = 0; i < codes.Count; i++)
			{
				if (!found && i < codes.Count - 8 && codes[i].LoadsConstant(1000L) && codes[i + 1].IsStloc() 
					&& codes[i + 2].LoadsConstant(1000L) && codes[i + 3].IsStloc()
					&& codes[i + 4].LoadsConstant(1000L) && codes[i + 5].IsStloc() 
					&& codes[i + 6].LoadsConstant(1000L) && codes[i + 7].IsStloc())
				{
					found = true;
					for (var j = 0; j < 4; j++)
					{
						indexes[j] = GetIndex(codes[i + 1 + 2 * j]);
					}
				}
				if (found && i < codes.Count - 1 && codes[i].IsLdloc() && codes[i + 1].LoadsConstant(100L))
				{
					var label = ilgen.DefineLabel();
					var label2 = ilgen.DefineLabel();
					yield return new CodeInstruction(OpCodes.Ldloc, indexes[0]).MoveLabelsFrom(codes[i]);
					yield return new CodeInstruction(OpCodes.Ldc_I4, 1000);
					yield return new CodeInstruction(OpCodes.Blt, label);
					yield return new CodeInstruction(OpCodes.Ldloc, indexes[3]);
					yield return new CodeInstruction(OpCodes.Ldc_I4_S, 10);
					yield return new CodeInstruction(OpCodes.Add);
					yield return new CodeInstruction(OpCodes.Stloc, indexes[0]);
					yield return new CodeInstruction(OpCodes.Ldloc, indexes[1]).WithLabels(label);
					yield return new CodeInstruction(OpCodes.Ldc_I4, 1000);
					yield return new CodeInstruction(OpCodes.Blt, label2);
					yield return new CodeInstruction(OpCodes.Ldloc, indexes[0]);
					yield return new CodeInstruction(OpCodes.Ldc_I4_S, 10);
					yield return new CodeInstruction(OpCodes.Add);
					yield return new CodeInstruction(OpCodes.Stloc, indexes[1]);
					codes[i].WithLabels(label2);
				}
				yield return codes[i];
			}

			object GetIndex(CodeInstruction instruction)
			{
				if (instruction.operand != null)
					return instruction.operand;
				if (instruction.opcode == OpCodes.Stloc_0)
					return 0;
				if (instruction.opcode == OpCodes.Stloc_1)
					return 1;
				if (instruction.opcode == OpCodes.Stloc_2)
					return 2;
				if (instruction.opcode == OpCodes.Stloc_3)
					return 3;
				return null;
			}
			*/
		}

		static void GiftAppearance_RefreshAppearance_FixLayers(CharacterMotion motion, ref int faceOrder, ref int frontHairOrder, ref int headOrder)
		{
			if (!motion)
			{
				return;
			}
			int headOrderFixed = int.MinValue;
			int faceOrderFixed = int.MinValue;
			int frontHairOrderFixed = int.MinValue;
			foreach (var spr in motion.motionSpriteSet)
			{
				switch (spr.sprType)
				{
					case CharacterAppearanceType.Head:
						headOrderFixed = Math.Max(headOrderFixed, spr.sprRenderer.sortingOrder);
						break;
					case CharacterAppearanceType.Face:
						faceOrderFixed = Math.Max(faceOrderFixed, spr.sprRenderer.sortingOrder);
						break;
					case CharacterAppearanceType.FrontHair:
						frontHairOrderFixed = Math.Max(frontHairOrderFixed, spr.sprRenderer.sortingOrder);
						break;
				}
			}
			if (faceOrderFixed == int.MinValue)
			{
				faceOrderFixed = headOrderFixed + 10;
			}
			if (frontHairOrderFixed == int.MinValue)
			{
				frontHairOrderFixed = faceOrderFixed + 10;
			}
			headOrder = headOrderFixed;
			faceOrder = faceOrderFixed;
			frontHairOrder = frontHairOrderFixed;
		}
	}
}
