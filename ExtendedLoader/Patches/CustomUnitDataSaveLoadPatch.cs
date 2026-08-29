using System;
using GameSave;
using HarmonyLib;
using UnityEngine;

namespace ExtendedLoader
{
	[HarmonyPatch]
	class CustomUnitDataSaveLoadPatch
	{
		[HarmonyPatch(typeof(UnitCustomizingData), nameof(UnitCustomizingData.GetSaveData))]
		[HarmonyPostfix]
		static void UnitCustomizingData_GetSaveData_Postfix(UnitCustomizingData __instance, SaveData __result)
		{
			try
			{
				SaveCustomizeLocation(__result, CustomizingLookType.FrontHair, __instance.frontHairID, frontHairSaveKey);
				SaveCustomizeLocation(__result, CustomizingLookType.BackHair, __instance.backHairID, backHairSaveKey);
				SaveCustomizeLocation(__result, CustomizingLookType.Eye, __instance.eyeID, eyeSaveKey);
				SaveCustomizeLocation(__result, CustomizingLookType.Brow, __instance.browID, browSaveKey);
				SaveCustomizeLocation(__result, CustomizingLookType.Mouth, __instance.mouthID, mouthSaveKey);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		static void SaveCustomizeLocation(SaveData saveData, CustomizingLookType type, int index, string saveKey)
		{
			if (XLRoot.indexesToLocations[type].TryGetValue(index, out var location))
			{
				saveData.AddData(saveKey, new SaveData(location));
			}
		}

		[HarmonyPatch(typeof(UnitCustomizingData), nameof(UnitCustomizingData.LoadFromSaveData))]
		[HarmonyPostfix]
		static void UnitCustomizingData_LoadFromSaveData_Postfix(UnitCustomizingData __instance, SaveData data)
		{
			if (data != null)
			{
				try
				{
					LoadCustomizeLocation(data, CustomizingLookType.FrontHair, ref __instance.frontHairID, frontHairSaveKey);
					LoadCustomizeLocation(data, CustomizingLookType.BackHair, ref __instance.backHairID, backHairSaveKey);
					LoadCustomizeLocation(data, CustomizingLookType.Eye, ref __instance.eyeID, eyeSaveKey);
					LoadCustomizeLocation(data, CustomizingLookType.Brow,	ref __instance.browID, browSaveKey);
					LoadCustomizeLocation(data, CustomizingLookType.Mouth, ref __instance.mouthID, mouthSaveKey);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}
		}

		static void LoadCustomizeLocation(SaveData saveData, CustomizingLookType type, ref int index, string saveKey)
		{
			var save = saveData.GetData(saveKey);
			if (save != null)
			{
				var location = save.GetStringSelf();
				if (XLRoot.locationsToIndexes[type].TryGetValue(location, out var savedIndex))
				{
					index = savedIndex;
				}
			}
		}

		const string frontHairSaveKey = "frontHairLocationXL";
		const string backHairSaveKey = "backHairLocationXL";
		const string eyeSaveKey = "eyeLocationXL";
		const string browSaveKey = "browLocationXL";
		const string mouthSaveKey = "mouthLocationXL";
	}
}