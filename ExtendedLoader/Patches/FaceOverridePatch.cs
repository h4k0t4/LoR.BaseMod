using System.Collections.Generic;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;

namespace ExtendedLoader
{
	[HarmonyPatch]
	internal class FaceOverridePatch
	{
		[HarmonyPatch(typeof(CustomizingResourceLoader), nameof(CustomizingResourceLoader.CreateCustomizedAppearance))]
		[HarmonyPostfix]
		[HarmonyPriority(Priority.Low)]
		static void CustomizingResourceLoader_CreateCustomizedAppearance_Postfix(CustomizedAppearance __result)
		{
			if (__result is SpecialCustomizedAppearance special && special.list != null)
			{
				var index = special.list.FindIndex(head => head.detail == ActionDetail.Default);
				if (index > 0)
				{
					var x = special.list[index];
					special.list.RemoveAt(index);
					special.list.Insert(0, x);
				}
				switch (special.name)
				{
					case "Customized_Gebura(Clone)":
						FixHeadRedirect(special.list, ActionDetail.Penetrate, ActionDetail.Slash);
						return;
					case "Customized_Binah(Clone)":
						FixHeadRedirect(special.list, ActionDetail.Slash, ActionDetail.Hit);
						FixHeadRedirect(special.list, ActionDetail.Penetrate, ActionDetail.Hit);
						FixHeadRedirect(special.list, ActionDetail.Default, ActionDetail.Guard);
						FixHeadRedirect(special.list, ActionDetail.Evade, ActionDetail.Guard);
						return;
					case "Customized_Angela(Clone)":
						FixHeadRedirect(special.list, ActionDetail.Slash, ActionDetail.Hit);
						return;
				}
			}
		}

		static void FixHeadRedirect(List<SpecialCustomHead> headList, ActionDetail source, ActionDetail target)
		{
			var sourceHead = headList.Find(x => x.detail == source);
			if (sourceHead == null)
			{
				return;
			}
			if (sourceHead.replaceHead && headList.Exists(x => x.rootObject.name == sourceHead.replaceHead.name))
			{
				return;
			}
			var targetHead = headList.Find(x => x.detail == target);
			if (targetHead != null)
			{
				sourceHead.replaceHead = targetHead.rootObject;
			}
		}

		[HarmonyPatch(typeof(CustomizedAppearance), nameof(CustomizedAppearance.RefreshAppearanceByMotion))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> CustomizedAppearance_RefreshAppearanceByMotion_Transpiler(IEnumerable<CodeInstruction> instructions)
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
		static IEnumerable<CodeInstruction> SpecialCustomizedAppearance_RefreshAppearanceByMotion_Transpiler(IEnumerable<CodeInstruction> instructions)
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
						waiting = false;
					}
				}
			}
		}

		static ActionDetail CheckOverride(ActionDetail originalAction, CharacterMotion motion)
		{
			if (motion.GetComponent<ExtendedCharacterMotion>() is ExtendedCharacterMotion extendedMotion && extendedMotion.faceOverride != FaceOverride.None)
			{
				return (ActionDetail)extendedMotion.faceOverride;
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

		static readonly FieldInfo actionField = AccessTools.Field(typeof(CharacterMotion), nameof(CharacterMotion.actionDetail));
		static readonly MethodInfo checkOverride = AccessTools.Method(typeof(FaceOverridePatch), nameof(CheckOverride));
	}
}
