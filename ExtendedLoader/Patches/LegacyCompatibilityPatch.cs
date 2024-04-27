using HarmonyLib;
using Mod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UI;
using UnityEngine;

namespace ExtendedLoader
{
	internal class LegacyCompatibilityPatch
	{
		public static void PrepareLegacy(Harmony h)
		{
			harmony = h;
			Assembly basemod = null;
			try
			{
				basemod = Assembly.Load("BaseMod");
			}
			catch {}
			if (basemod != null)
			{
				try
				{
					if (basemod.GetType("ExtendedLoader.LorName") != null)
					{
						harmony.Patch(basemod.GetType("BaseMod.BaseModInitialize").GetMethod("OnInitializeMod"), finalizer: new HarmonyMethod(typeof(LegacyCompatibilityPatch), nameof(UnpatchLegacy)));
						harmony.Patch(basemod.GetType("BaseMod.Harmony_Patch").GetMethod("LoadBookSkins", AccessTools.all), prefix: new HarmonyMethod(typeof(LegacyCompatibilityPatch), nameof(LoadBookSkinsForLegacy)));
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}
		}

		static Exception UnpatchLegacy(Exception __exception)
		{
			Harmony.UnpatchID(LegacyLoaderId);
			foreach (var target in legacyPrefixTargets)
			{
				harmony.Unpatch(target, HarmonyPatchType.Prefix, BasemodId);
			}
			foreach (var target in legacyPostfixTargets)
			{
				harmony.Unpatch(target, HarmonyPatchType.Postfix, BasemodId);
			}
			return null;
		}

		static readonly MethodInfo[] legacyPrefixTargets = new MethodInfo[]
		{
			AccessTools.Method(typeof(SdCharacterUtil), nameof(SdCharacterUtil.CreateSkin)),
			AccessTools.Method(typeof(BattleUnitView), nameof(BattleUnitView.ChangeSkin)),
			AccessTools.Method(typeof(BattleUnitView), nameof(BattleUnitView.ChangeEgoSkin)),
			AccessTools.Method(typeof(BattleUnitView), nameof(BattleUnitView.ChangeCreatureSkin)),
			AccessTools.Method(typeof(CharacterSound), nameof(CharacterSound.LoadAudioCoroutine)),
			AccessTools.Method(typeof(UIEquipPageCustomizePanel), nameof(UIEquipPageCustomizePanel.ApplyFilterAll)),
			AccessTools.Method(typeof(UIEquipPageCustomizePanel), nameof(UIEquipPageCustomizePanel.UpdateEquipPageSlotList)),
		};

		static readonly MethodInfo[] legacyPostfixTargets = new MethodInfo[]
		{
			AccessTools.Method(typeof(FaceEditor), nameof(FaceEditor.Init)),
			AccessTools.Method(typeof(UnitDataModel), nameof(UnitDataModel.LoadFromSaveData)),
		};

		static bool LoadBookSkinsForLegacy()
		{
			var customizingBookSkinLoader = CustomizingBookSkinLoader.Instance;
			Dictionary<string, List<Workshop.WorkshopSkinData>> _bookSkinData = customizingBookSkinLoader._bookSkinData;
			foreach (ModContent modContent in ModContentManager.Instance._loadedContents)
			{
				string modDirectory = modContent._dirInfo.FullName;
				string modName = modContent._itemUniqueId;
				string modId = "";
				if (!modName.ToLower().EndsWith("@origin"))
				{
					modId = modName;
				}
				int oldSkins = 0;
				if (customizingBookSkinLoader._bookSkinData.TryGetValue(modName, out List<Workshop.WorkshopSkinData> skinlist))
				{
					oldSkins = skinlist.Count;
				}
				string charDirectory = Path.Combine(modDirectory, "Char");
				List<Workshop.WorkshopSkinData> list = new List<Workshop.WorkshopSkinData>();
				if (Directory.Exists(charDirectory))
				{
					string[] directories = Directory.GetDirectories(charDirectory);
					for (int i = 0; i < directories.Length; i++)
					{
						try
						{
							Workshop.WorkshopAppearanceInfo workshopAppearanceInfo = Workshop.WorkshopAppearanceItemLoader.LoadCustomAppearance(directories[i]);
							if (workshopAppearanceInfo != null)
							{
								string[] array = directories[i].Split(new char[]
								{
								'\\'
								});
								string bookName = array[array.Length - 1];
								workshopAppearanceInfo.path = directories[i];
								workshopAppearanceInfo.uniqueId = modId;
								workshopAppearanceInfo.bookName = "Custom_" + bookName;
								if (workshopAppearanceInfo.isClothCustom)
								{
									list.Add(new Workshop.WorkshopSkinData
									{
										dic = workshopAppearanceInfo.clothCustomInfo,
										dataName = workshopAppearanceInfo.bookName,
										contentFolderIdx = workshopAppearanceInfo.uniqueId,
										id = oldSkins + i
									});
								}
								if (workshopAppearanceInfo.faceCustomInfo != null && workshopAppearanceInfo.faceCustomInfo.Count > 0 && FaceData.GetExtraData(workshopAppearanceInfo.faceCustomInfo) == null)
								{
									XLRoot.LoadFaceCustom(workshopAppearanceInfo.faceCustomInfo, $"mod_{modName}:{bookName}");
								}
							}
						}
						catch (Exception ex)
						{
							Debug.LogError("BaseMod: error loading skin at " + directories[i]);
							Debug.LogError(ex);
						}
					}
					if (_bookSkinData.TryGetValue(modId, out var modSkinList) && modSkinList != null)
					{
						modSkinList.AddRange(list);
					}
					else
					{
						_bookSkinData[modId] = list;
					}
				}
			}
			return false;
		}

		static Harmony harmony = null;
		private const string LegacyLoaderId = "LOR.BaseMod.Cyaminthe.ExtendedLoader";
		const string BasemodId = "LOR.BaseMod";
	}
}
