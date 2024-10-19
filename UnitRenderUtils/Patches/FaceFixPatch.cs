using System.Collections.Generic;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using static HarmonyLib.AccessTools;
using static System.Reflection.Emit.OpCodes;
using System;
using System.Runtime.CompilerServices;

namespace ExtendedLoader
{
	[HarmonyPatch]
	internal class FaceFixPatch
	{
		[HarmonyPatch(typeof(CustomizedAppearance), nameof(CustomizedAppearance.RefreshAppearanceByMotion))]
		[HarmonyTranspiler]
		[HarmonyPriority(Priority.LowerThanNormal)]
		static IEnumerable<CodeInstruction> CustomizedAppearance_RefreshAppearanceByMotion_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var checkFix = Method(typeof(FaceFixPatch), nameof(CheckFix));
			int count = 0;
			foreach (var instruction in instructions)
			{
				yield return instruction;
				if (instruction.Is(Ldfld, actionField))
				{
					count++;
					if (count == 2)
					{
						yield return new CodeInstruction(Call, checkFix);
					}
				}
			}
		}

		[HarmonyPatch(typeof(SpecialCustomizedAppearance), nameof(SpecialCustomizedAppearance.RefreshAppearanceByMotion))]
		[HarmonyTranspiler]
		[HarmonyPriority(Priority.LowerThanNormal)]
		static IEnumerable<CodeInstruction> SpecialCustomizedAppearance_RefreshAppearanceByMotion_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilgen)
		{
			var checkFix = Method(typeof(FaceFixPatch), nameof(CheckFixSpecial));
			var codes = instructions.ToList();
			int jumpFrom = -1;
			int jumpTo = -1;
			FieldInfo generatedField = null;
			for (int i = 0; i < codes.Count; i++)
			{
				if (codes[i].LoadsField(actionField))
				{
					codes.InsertRange(i + 1, new CodeInstruction[]
					{
						new CodeInstruction(Ldarg_0),
						new CodeInstruction(Call, checkFix)
					});
					break;
				}
			}
			for (int i = 0; i < codes.Count; i++)
			{
				if (codes[i].opcode == Stfld && codes[i].operand is FieldInfo field)
				{
					if (generatedField == null)
					{
						if (field.FieldType == typeof(ActionDetail) && field.DeclaringType is Type type && type.GetCustomAttribute<CompilerGeneratedAttribute>() != null)
						{
							generatedField = field;
							jumpFrom = i + 1;
						}
					}
					else
					{
						if (field == generatedField)
						{
							jumpTo = i + 1;
						}
					}
				}
			}
			if (jumpFrom >= 0 && jumpTo >= 0)
			{
				var label = ilgen.DefineLabel();
				codes[jumpTo].labels.Add(label);
				codes.Insert(jumpFrom, new CodeInstruction(Br, label));
			}
			return codes;
		}

		static ActionDetail CheckFix(ActionDetail originalAction)
		{
			if (originalAction == ActionDetail.Penetrate2)
			{
				return ActionDetail.Penetrate;
			}
			if (originalAction == ActionDetail.Hit2) 
			{
				return ActionDetail.Hit;
			}
			if (originalAction - ActionDetail.Fire <= 8 && originalAction - ActionDetail.Fire >= 0)
			{
				return ActionDetail.Slash;
			}
			return originalAction;
		}

		static ActionDetail CheckFixSpecial(ActionDetail originalAction, SpecialCustomizedAppearance appearance)
		{
			if (appearance.list.Exists(x => x.detail == originalAction))
			{
				return originalAction;
			}
			if (originalAction == ActionDetail.Penetrate2)
			{
				return ActionDetail.Penetrate;
			}
			if (originalAction == ActionDetail.Hit2)
			{
				return ActionDetail.Hit;
			}
			if (originalAction - ActionDetail.Fire <= 8 && originalAction - ActionDetail.Fire >= 0)
			{
				return ActionDetail.Slash;
			}
			return originalAction;
		}

		static readonly FieldInfo actionField = Field(typeof(CharacterMotion), nameof(CharacterMotion.actionDetail));
	}
}
