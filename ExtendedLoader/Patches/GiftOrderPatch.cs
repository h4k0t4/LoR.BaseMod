using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace ExtendedLoader
{
	[HarmonyPatch]
	internal class GiftOrderPatch
	{
		[HarmonyPatch(typeof(GiftAppearance), nameof(GiftAppearance.RefreshAppearance))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> GiftAppearance_RefreshAppearance_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilgen)
		{
			var codes = instructions.ToList();
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
		}
	}
}
