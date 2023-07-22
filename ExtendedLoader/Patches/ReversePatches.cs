using HarmonyLib;
using UI;
using UnityEngine;

namespace ExtendedLoader
{
	internal class ReversePatches
	{
		[HarmonyReversePatch(HarmonyReversePatchType.Snapshot)]
		[HarmonyPatch(typeof(UICharacterRenderer), nameof(UICharacterRenderer.SetCharacter))]
		public static void UICharacterRenderer_SetCharacter_Snapshot(UICharacterRenderer __instance, UnitDataModel unit, int index, bool forcelyReload, bool renderRealtime)
		{

		}

		[HarmonyReversePatch(HarmonyReversePatchType.Snapshot)]
		[HarmonyPatch(typeof(SdCharacterUtil), nameof(SdCharacterUtil.CreateSkin))]
		public static CharacterAppearance SdCharacterUtil_CreateSkin_Snapshot(UnitDataModel unit, Faction faction, Transform characterRoot)
		{
			return null;
		}

		[HarmonyReversePatch(HarmonyReversePatchType.Snapshot)]
		[HarmonyPatch(typeof(BattleUnitView), nameof(BattleUnitView.ChangeSkin))]
		public static void BattleUnitView_ChangeSkin_Snapshot(BattleUnitView __instance, string charName)
		{

		}

		[HarmonyReversePatch(HarmonyReversePatchType.Snapshot)]
		[HarmonyPatch(typeof(BattleUnitView), nameof(BattleUnitView.ChangeEgoSkin))]
		public static void BattleUnitView_ChangeEgoSkin_Snapshot(BattleUnitView __instance, string egoName, bool bookNameChange)
		{

		}

		[HarmonyReversePatch(HarmonyReversePatchType.Snapshot)]
		[HarmonyPatch(typeof(BattleUnitView), nameof(BattleUnitView.ChangeCreatureSkin))]
		public static void BattleUnitView_ChangeCreatureSkin_Snapshot(BattleUnitView __instance, string creatureName)
		{

		}
	}
}
