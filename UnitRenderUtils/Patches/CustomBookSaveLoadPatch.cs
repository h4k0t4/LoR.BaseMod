using System;
using GameSave;
using HarmonyLib;
using UnityEngine;

namespace ExtendedLoader
{
	[HarmonyPatch]
	class CustomBookSaveLoadPatch
	{
		[HarmonyPatch(typeof(UnitDataModel), nameof(UnitDataModel.GetSaveData))]
		[HarmonyPostfix]
		static void UnitDataModel_GetSaveData_Postfix(UnitDataModel __instance, SaveData __result)
		{
			try
			{
				var saveDict = __result.GetDictionarySelf();
				if (__instance._CustomBookItem != null)
				{
					LorId bookClassInfoId = __instance._CustomBookItem.GetBookClassInfoId();
					SaveData customcorebook = new SaveData(SaveDataType.Dictionary);
					customcorebook.AddData("_pid", new SaveData(bookClassInfoId.packageId));
					customcorebook.AddData("_id", new SaveData(bookClassInfoId.id));
					saveDict[customBookKey] = customcorebook;
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		[HarmonyPatch(typeof(UnitDataModel), nameof(UnitDataModel.LoadFromSaveData))]
		[HarmonyPostfix]
		static void UnitDataModel_LoadFromSaveData_Postfix(UnitDataModel __instance, SaveData data)
		{
			try
			{
				SaveData customcorebook = data.GetData("customcorebookInstanceId");
				if (customcorebook != null)
				{
					if (customcorebook.GetData("_pid") != null)
					{
						LorId id = new LorId(customcorebook.GetString("_pid"), customcorebook.GetInt("_id"));
						BookXmlInfo bookXml = BookXmlList.Instance.GetData(id);
						if (bookXml != null && !bookXml.isError)
						{
							BookModel bookModel = new BookModel(bookXml);
							if (SaveManager.Instance.iver <= 13 && bookModel.GetBookClassInfoId() == __instance.bookItem.GetBookClassInfoId())
							{
								__instance.EquipCustomCoreBook(null);
							}
							else
							{
								__instance.EquipCustomCoreBook(bookModel);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		private const string customBookKey = "customcorebookInstanceId";
	}
}
