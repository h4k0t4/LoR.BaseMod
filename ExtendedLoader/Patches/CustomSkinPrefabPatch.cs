using HarmonyLib;
using System.Collections.Generic;
using UI;
using static HarmonyLib.AccessTools;
using static System.Reflection.Emit.OpCodes;

namespace ExtendedLoader
{
	[HarmonyPatch]
	internal class CustomSkinPrefabPatch
	{
		[HarmonyPatch(typeof(UICharacterRenderer), nameof(UICharacterRenderer.SetCharacter))]
		[HarmonyPatch(typeof(AssetBundleManagerRemake), nameof(AssetBundleManagerRemake.LoadCharacterPrefab_DefaultMotion))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> LoadUIAppearance_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var customPrefabGetter = PropertyGetter(typeof(XLRoot), nameof(XLRoot.UICustomAppearancePrefab));
			var codes = new List<CodeInstruction>(instructions);
			for (var i = 0; i < codes.Count; i++)
			{
				if (codes[i].Is(Ldstr, "Prefabs/Characters/[Prefab]Appearance_Custom"))
				{
					yield return new CodeInstruction(Call, customPrefabGetter);
					i++;
				}
				else
				{
					yield return codes[i];
				}
			}
		}

		[HarmonyPatch(typeof(SdCharacterUtil), nameof(SdCharacterUtil.LoadAppearance))]
		[HarmonyPatch(typeof(AssetBundleManagerRemake), nameof(AssetBundleManagerRemake.LoadCharacterPrefab))]
		[HarmonyPatch(typeof(AssetBundleManagerRemake), nameof(AssetBundleManagerRemake.LoadSdPrefab))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> LoadAppearance_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var customPrefabGetter = PropertyGetter(typeof(XLRoot), nameof(XLRoot.CustomAppearancePrefab));
			var codes = new List<CodeInstruction>(instructions);
			for (var i = 0; i < codes.Count; i++)
			{
				if (codes[i].Is(Ldstr, "Prefabs/Characters/[Prefab]Appearance_Custom"))
				{
					yield return new CodeInstruction(Call, customPrefabGetter);
					i++;
				}
				else
				{
					yield return codes[i];
				}
			}
		}
	}
}
