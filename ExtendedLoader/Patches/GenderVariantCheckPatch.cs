using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UI;

namespace ExtendedLoader
{
	[HarmonyPatch]
	internal class GenderVariantCheckPatch
	{
		[HarmonyPatch(typeof(BookModel), nameof(BookModel.GetThumbSprite))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> BookModel_GetThumbSprite_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilgen)
		{
			return GenericGenderChecker_Transpiler(instructions, ilgen, new CodeInstruction[] {
				new CodeInstruction(OpCodes.Ldarg_0),
				new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(BookModel), nameof(BookModel.ClassInfo))),
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(BookXmlInfo), nameof(BookXmlInfo.gender)))
			});
		}

		[HarmonyPatch(typeof(BookXmlInfo), nameof(BookXmlInfo.GetThumbSprite))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> BookXmlInfo_GetThumbSprite_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilgen)
		{
			return GenericGenderChecker_Transpiler(instructions, ilgen, new CodeInstruction[] {
				new CodeInstruction(OpCodes.Ldarg_0),
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(BookXmlInfo), nameof(BookXmlInfo.gender)))
			});
		}

		[HarmonyPatch(typeof(SdCharacterUtil), nameof(SdCharacterUtil.CreateSkin))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> SdCharacterUtil_CreateSkin_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilgen)
		{
			return GenericGenderChecker_Transpiler(instructions, ilgen, new CodeInstruction[] {
				new CodeInstruction(OpCodes.Ldarg_0),
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(UnitDataModel), nameof(UnitDataModel.appearanceType)))
			});
		}

		[HarmonyPatch(typeof(UICharacterRenderer), nameof(UICharacterRenderer.SetCharacter))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> UICharacterRenderer_SetCharacter_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilgen)
		{
			return GenericGenderChecker_Transpiler(instructions, ilgen, new CodeInstruction[] {
				new CodeInstruction(OpCodes.Ldarg_1),
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(UnitDataModel), nameof(UnitDataModel.appearanceType)))
			});
		}

		static IEnumerable<CodeInstruction> GenericGenderChecker_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilgen, CodeInstruction[] genderGetterSequence)
		{
			var codes = new List<CodeInstruction>(instructions);
			for (int i = 0; i < codes.Count; i++)
			{
				if (codes[i].Is(OpCodes.Callvirt, baseMethod))
				{
					var idLocal = ilgen.DeclareLocal(typeof(string));
					var nameLocal = ilgen.DeclareLocal(typeof(string));
					yield return new CodeInstruction(OpCodes.Stloc, nameLocal);
					yield return new CodeInstruction(OpCodes.Stloc, idLocal);
					yield return new CodeInstruction(OpCodes.Ldloc, idLocal);
					yield return new CodeInstruction(OpCodes.Ldloc, nameLocal);
					yield return codes[i];
					yield return new CodeInstruction(OpCodes.Ldloc, idLocal);
					yield return new CodeInstruction(OpCodes.Ldloc, nameLocal);
					for (int j = 0; j < genderGetterSequence.Length; j++)
					{
						yield return genderGetterSequence[j];
					}
					yield return new CodeInstruction(OpCodes.Call, upgradeMethod);
				}
				else
				{
					yield return codes[i];
				}
			}
		}

		static Workshop.WorkshopSkinData TryReplaceGenderData(Workshop.WorkshopSkinData baseData, string id, string name, Gender gender)
		{
			var upgradeData = SkinTools.GetWorkshopBookSkinData(id, name, "_" + gender);
			return upgradeData ?? baseData;
		}

		static readonly MethodInfo baseMethod = AccessTools.Method(typeof(CustomizingBookSkinLoader), nameof(CustomizingBookSkinLoader.GetWorkshopBookSkinData), new Type[] { typeof(string), typeof(string) });
		static readonly MethodInfo upgradeMethod = AccessTools.Method(typeof(GenderVariantCheckPatch), nameof(TryReplaceGenderData));
	}
}
