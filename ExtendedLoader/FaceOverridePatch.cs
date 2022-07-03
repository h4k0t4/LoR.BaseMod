using System;
using System.Collections.Generic;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;

namespace ExtendedLoader
{

	static class FaceOverridePatch
	{
		[HarmonyPatch(typeof(CustomizedAppearance), nameof(CustomizedAppearance.RefreshAppearanceByMotion))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> CustomizedAppearanceRefreshTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			int count = 0;
			foreach (var instruction in instructions)
			{
				yield return instruction;
				if (instruction.Is(OpCodes.Ldfld, actionField))
				{
					count++;
					if (count == 2)
					{
						yield return new CodeInstruction(OpCodes.Ldarg_1);
						yield return new CodeInstruction(OpCodes.Call, checkOverride);
					}
				}
			}
		}

		[HarmonyPatch(typeof(SpecialCustomizedAppearance), nameof(SpecialCustomizedAppearance.RefreshAppearanceByMotion))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> SpecialCustomizedAppearanceRefreshTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			bool waiting = true;
			foreach (var instruction in instructions)
			{
				yield return instruction;
				if (instruction.Is(OpCodes.Ldfld, actionField))
				{
					if (waiting)
					{
						yield return new CodeInstruction(OpCodes.Ldarg_1);
						yield return new CodeInstruction(OpCodes.Call, checkOverride);
					}
					waiting = false;
				}
			}
		}

		static ActionDetail CheckOverride(ActionDetail originalAction, CharacterMotion motion)
		{
			if (motion is ExtendedCharacterMotion extendedMotion && extendedMotion.faceOverride != FaceOverride.None)
			{
				return (ActionDetail)extendedMotion.faceOverride;
			}
			if (originalAction - ActionDetail.Fire <= 10 && originalAction - ActionDetail.Fire >= 0)
			{
				return ActionDetail.Slash;
			}
			return originalAction;
		}

		static readonly FieldInfo actionField = AccessTools.Field(typeof(CharacterMotion), nameof(CharacterMotion.actionDetail));
		static readonly MethodInfo checkOverride = AccessTools.Method(typeof(FaceOverridePatch), nameof(FaceOverridePatch.CheckOverride));
	}
}
