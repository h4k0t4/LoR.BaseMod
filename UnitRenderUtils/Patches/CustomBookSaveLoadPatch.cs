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
				LorId bookClassInfoId = __instance._CustomBookItem != null ? __instance._CustomBookItem.GetBookClassInfoId() : LorId.None;
				saveDict[UnitDataModel.save_customcorebookInstanceId] = bookClassInfoId.GetSaveData();
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
				SaveData customcorebook = data.GetData(UnitDataModel.save_customcorebookInstanceId);
				if (customcorebook == null)
				{
					return;
				}
				string pid = null;
				if (customcorebook.GetData("_pid") is SaveData pidStr)
				{
					pid = pidStr.GetStringSelf();
				}
				else if (customcorebook.GetData("pid") is SaveData pidInt)
				{
					pid = SaveManager.Instance.ConvertIntToPackageId(pidInt.GetIntSelf());
				}
				if (pid == null)
				{
					return;
				}
				LorId id = new LorId(pid, customcorebook.GetInt("_id"));
				BookXmlInfo bookXml = BookXmlList.Instance.GetData(id);
				if (bookXml == null || bookXml.isError)
				{
					return;
				}
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
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
	}
}
