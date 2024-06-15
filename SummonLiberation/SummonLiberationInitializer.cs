using BattleCharacterProfile;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using UI;
using UnityEngine;
using System.Linq;
using System.Reflection;

namespace SummonLiberation
{
	public class Harmony_Patch
	{
		//Enlarge Librarian's formation
		[HarmonyPatch(typeof(LibraryFloorModel), nameof(LibraryFloorModel.Init))]
		[HarmonyPostfix]
		static void LibraryFloorModel_Init_Post(LibraryFloorModel __instance)
		{
			try
			{
				AddIndexes(__instance._formationIndex, 99);
				AddFormationPosition(__instance._defaultFormation, 99);
				if (__instance._formation == null)
				{
					__instance._formation = __instance._defaultFormation;
				}
				else
				{
					AddFormationPosition(__instance._formation, 99);
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/LFIerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//remove error logs about saved formations being too big
		[HarmonyPatch(typeof(LibraryFloorModel), nameof(LibraryFloorModel.LoadFromSaveData))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> LibraryFloorModel_LoadFromSaveData_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var method = AccessTools.Method(typeof(Debug), nameof(Debug.LogError), new Type[] {typeof(object)});
			var codes = instructions.ToList();
			for (int i = 0; i < codes.Count - 2; i++)
			{
				if (codes[i].Is(OpCodes.Ldstr, "formation index length is too high") && codes[i + 1].Is(OpCodes.Call, method))
				{
					codes.RemoveRange(i, 2);
					break;
				}
			}
			return codes;
		}
		//Enlarge Enemy's formation
		[HarmonyPatch(typeof(StageWaveModel), nameof(StageWaveModel.Init))]
		[HarmonyPostfix]
		static void StageWaveModel_Init_Post(StageWaveModel __instance)
		{
			try
			{
				AddIndexes(__instance._formationIndex, 100);
				AddFormationPositionForEnemy(__instance._formation, 100);
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/SWMIerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		static void AddIndexes(List<int> indexes, int targetCount)
		{
			List<int> sortedIndexes = new List<int>(indexes);
			sortedIndexes.Sort();
			int i = 0;
			for (int j = 0; indexes.Count < targetCount; j++)
			{
				if (i < sortedIndexes.Count && j == sortedIndexes[i])
				{
					i++;
				}
				else
				{
					indexes.Add(j);
				}
			}
		}
		//Enlarge Enemy's formation
		[HarmonyPatch(typeof(StageWaveModel), nameof(StageWaveModel.GetUnitBattleDataListByFormation))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> StageWaveModel_GetUnitBattleDataListByFormation_In(IEnumerable<CodeInstruction> instructions)
		{
			foreach (var instruction in instructions)
			{
				yield return instruction;
				if (instruction.opcode == OpCodes.Ldc_I4_5)
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StageWaveModel), nameof(StageWaveModel._unitList)));
					yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(List<UnitBattleDataModel>), nameof(List<UnitBattleDataModel>.Count)));
					yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Math), nameof(Math.Max), new Type[] { typeof(int), typeof(int) }));
				}
			}
		}
		//BattleUnitProfileArray Up to 9
		[HarmonyPatch(typeof(BattleUnitInfoManagerUI), nameof(BattleUnitInfoManagerUI.Initialize))]
		[HarmonyPrefix]
		[HarmonyPriority(Priority.High)]
		static void BattleUnitInfoManagerUI_Initialize_Pre(BattleUnitInfoManagerUI __instance)
		{
			try
			{
				var lastAlly = __instance.allyProfileArray.Length - 1;
				if (lastAlly < MIN_EMOTION_SLOTS - 1)
				{
					var allyProfileArray2 = new List<BattleCharacterProfileUI>(__instance.allyProfileArray);
					for (int i = lastAlly + 1; i < MIN_EMOTION_SLOTS; i++)
					{
						allyProfileArray2.Add(UnityEngine.Object.Instantiate(allyProfileArray2[lastAlly], allyProfileArray2[lastAlly].transform.parent));
						allyProfileArray2[i].gameObject.SetActive(false);
						allyProfileArray2[i].transform.localPosition = allyProfileArray2[lastAlly].transform.localPosition + new Vector3(0f, (i - lastAlly) * Y_SHIFT, 0f);
					}
					__instance.allyProfileArray = allyProfileArray2.ToArray();
				}
				var lastEnemy = __instance.enemyProfileArray.Length - 1;
				if (lastEnemy < MIN_EMOTION_SLOTS - 1)
				{
					var enemyProfileArray2 = new List<BattleCharacterProfileUI>(__instance.enemyProfileArray);
					for (int i = lastEnemy + 1; i < MIN_EMOTION_SLOTS; i++)
					{
						enemyProfileArray2.Add(UnityEngine.Object.Instantiate(enemyProfileArray2[lastEnemy], enemyProfileArray2[lastEnemy].transform.parent));
						enemyProfileArray2[i].gameObject.SetActive(false);
						enemyProfileArray2[i].transform.localPosition = enemyProfileArray2[lastEnemy].transform.localPosition + new Vector3(0f, (i - lastEnemy) * Y_SHIFT, 0f);
					}
					__instance.enemyProfileArray = enemyProfileArray2.ToArray();
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/BUIFMIerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		[HarmonyPatch(typeof(BattleUnitInfoManagerUI), nameof(BattleUnitInfoManagerUI.Initialize))]
		[HarmonyPrefix]
		[HarmonyPriority(Priority.Last)]
		static void BattleUnitInfoManagerUI_Initialize_Pre2(BattleUnitInfoManagerUI __instance, ref IList<BattleUnitModel> unitList)
		{
			try
			{
				var allyDirection = StageController.Instance.AllyFormationDirection;
				var enemyProfilesCount = (allyDirection == Direction.RIGHT) ? __instance.enemyProfileArray.Length : __instance.allyProfileArray.Length;
				var allyProfilesCount = (allyDirection == Direction.RIGHT) ? __instance.allyProfileArray.Length : __instance.enemyProfileArray.Length;
				unitList = unitList.Where(model => model.faction == Faction.Enemy ? (model.index < enemyProfilesCount) : (model.index < allyProfilesCount)).ToList();
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/BUIFMI2error.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//CharacterProfile under character
		[HarmonyPatch(typeof(BattleUnitInfoManagerUI), nameof(BattleUnitInfoManagerUI.UpdateCharacterProfile))]
		[HarmonyPostfix]
		static void BattleUnitInfoManagerUI_UpdateCharacterProfile_Post(BattleUnitModel unit, float hp, int bp, BattleBufUIDataList bufDataList = null)
		{
			try
			{
				unit.view.unitBottomStatUI.UpdateStatUI(hp, bp, bufDataList);
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/BUIFMUCPerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//BattleEmotionCoinUI
		[HarmonyPatch(typeof(BattleEmotionCoinUI), nameof(BattleEmotionCoinUI.Init))]
		[HarmonyPrefix]
		static void BattleEmotionCoinUI_Init_Pre(BattleEmotionCoinUI __instance)
		{
			try
			{
				var unitList = BattleManagerUI.Instance.ui_unitListInfoSummary;
				var lastLibrarian = __instance.librarian.Length - 1;
				if (lastLibrarian < MIN_EMOTION_SLOTS - 1)
				{
					var librarian = new List<BattleEmotionCoinUI.BattleEmotionCoinData>(__instance.librarian);
					for (int i = lastLibrarian + 1; i < MIN_EMOTION_SLOTS; i++)
					{
						librarian.Add(new BattleEmotionCoinUI.BattleEmotionCoinData()
						{
							cosFactor = 1f,
							sinFactor = 1f,
							target = UnityEngine.Object.Instantiate(librarian[lastLibrarian].target, librarian[lastLibrarian].target.parent)
						});
						librarian[i].target.localPosition = librarian[lastLibrarian].target.localPosition + unitList.allyProfileArray[i].transform.localPosition - unitList.allyProfileArray[lastLibrarian].transform.localPosition;
					}
					__instance.librarian = librarian.ToArray();
				}
				var lastEnermy = __instance.enermy.Length - 1;
				if (lastEnermy < MIN_EMOTION_SLOTS - 1)
				{
					var enermy = new List<BattleEmotionCoinUI.BattleEmotionCoinData>(__instance.enermy);
					for (int i = lastEnermy + 1; i < MIN_EMOTION_SLOTS; i++)
					{
						enermy.Add(new BattleEmotionCoinUI.BattleEmotionCoinData()
						{
							cosFactor = 1f,
							sinFactor = 1f,
							target = UnityEngine.Object.Instantiate(enermy[lastEnermy].target, enermy[lastEnermy].target.parent)
						});
						enermy[i].target.localPosition = enermy[lastEnermy].target.localPosition + unitList.enemyProfileArray[i].transform.localPosition - unitList.enemyProfileArray[lastEnermy].transform.localPosition;
					}
					__instance.enermy = enermy.ToArray();
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/BECU_Initerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		public static int MIN_EMOTION_SLOTS = 9;
		public static float Y_SHIFT = 64f;

		[HarmonyPatch(typeof(BattleEmotionCoinUI), nameof(BattleEmotionCoinUI.Init))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> BattleEmotionCoinUI_Init_In(IEnumerable<CodeInstruction> instructions)
		{
			var method = AccessTools.Method(typeof(BattleObjectManager), nameof(BattleObjectManager.GetAliveList), new Type[] { typeof(bool) });
			foreach (var instruction in instructions)
			{
				yield return instruction;
				if (instruction.Is(OpCodes.Callvirt, method))
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Harmony_Patch), nameof(BattleEmotionCoinUI_Init_FilterUnitCount)));
				}
			}
		}
		static List<BattleUnitModel> BattleEmotionCoinUI_Init_FilterUnitCount(List<BattleUnitModel> unitList, BattleEmotionCoinUI __instance)
		{
			var allyDirection = StageController.Instance.AllyFormationDirection;
			var enemyDataCount = (allyDirection == Direction.RIGHT) ? __instance.enermy.Length : __instance.librarian.Length;
			var allyDataCount = (allyDirection == Direction.RIGHT) ? __instance.librarian.Length : __instance.enermy.Length;
			var allyUnits = 0;
			var enemyUnits = 0;
			var filteredList = unitList.Where(model => model.faction == Faction.Enemy ? (enemyUnits++ < enemyDataCount) : (allyUnits++ < allyDataCount)).ToList();
			unitList.Clear();
			unitList.AddRange(filteredList);
			return unitList;
		}

		//BattleEmotionCoinUI
		[HarmonyPatch(typeof(BattleEmotionCoinUI), nameof(BattleEmotionCoinUI.Acquisition))]
		[HarmonyPrefix]
		static bool BattleEmotionCoinUI_Acquisition_Pre(BattleUnitModel unit)
		{
			try
			{
				if (BattleManagerUI.Instance.ui_unitListInfoSummary.GetProfileUI(unit) == null)
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/CoinUI_Acquisitionerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return true;
		}
		//EmotionBattleTeamModel
		/*
		[HarmonyPatch(typeof(EmotionBattleTeamModel), nameof(EmotionBattleTeamModel.UpdateUnitList))]
		[HarmonyPrefix]
		static bool EmotionBattleTeamModel_UpdateUnitList_Pre(EmotionBattleTeamModel __instance)
		{
			try
			{
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/TeamEmotion_UpdateListerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return true;
		}
		*/
		//ProfileUI Unit Preview
		[HarmonyPatch(typeof(UICharacterRenderer), nameof(UICharacterRenderer.SetCharacter))]
		[HarmonyPrefix]
		[HarmonyPriority(Priority.High)]
		static void UICharacterRenderer_SetCharacter_Pre(UICharacterRenderer __instance, int index)
		{
			try
			{
				var fixedIndex = UICharacterRenderer_SetCharacter_GetIndexWithSkip(index);
				var minCharCount = Math.Max(199, fixedIndex + 1);
				while (__instance.characterList.Count < minCharCount)
				{
					__instance.characterList.Add(new UICharacter(null, null));
				}
				while (__instance.cameraList.Count <= fixedIndex)
				{
					Camera camera = UnityEngine.Object.Instantiate(__instance.cameraList[0], __instance.cameraList[0].transform.parent);
					camera.name = "[Camera]" + __instance.cameraList.Count;
					camera.targetTexture = UnityEngine.Object.Instantiate(__instance.cameraList[0].targetTexture);
					camera.targetTexture.name = "RT_Character_" + __instance.cameraList.Count;
					camera.transform.position += new Vector3(10f * __instance.cameraList.Count, 0f, 0f);
					__instance.cameraList.Add(camera);
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/UICR_ECerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}

		[HarmonyPatch(typeof(UICharacterRenderer), nameof(UICharacterRenderer.GetRenderTextureByIndexAndSize))]
		[HarmonyPatch(typeof(UICharacterRenderer), nameof(UICharacterRenderer.GetRenderTextureByIndex))]
		[HarmonyPatch(typeof(UICharacterRenderer), nameof(UICharacterRenderer.GetUICharacterByIndex))]
		[HarmonyPatch(typeof(UICharacterRenderer), nameof(UICharacterRenderer.Redraw))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> UICharacterRenderer_GeneralLimitRaising_In(IEnumerable<CodeInstruction> instructions)
		{
			foreach (var instruction in instructions)
			{
				if (instruction.LoadsConstant(11L))
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(UICharacterRenderer), nameof(UICharacterRenderer.characterList)));
					yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(List<UICharacter>), nameof(List<UICharacter>.Count)));
				}
				else
				{
					yield return instruction;
				}
			}
		}

		[HarmonyPatch(typeof(UICharacterRenderer), nameof(UICharacterRenderer.SetCharacter))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> UICharacterRenderer_SetCharacter_In(IEnumerable<CodeInstruction> instructions)
		{
			bool passedTextureIndexInit = false;
			bool storedFixedIndex = false;
			var customBookProperty = AccessTools.PropertyGetter(typeof(UnitDataModel), nameof(UnitDataModel.CustomBookItem));
			var textureIndexField = AccessTools.Field(typeof(UnitDataModel), nameof(UnitDataModel.textureIndex));
			var getMaxForCheck = AccessTools.Method(typeof(Harmony_Patch), nameof(UICharacterRenderer_SetCharacter_GetMaxWithoutSkip));
			var getIndex = AccessTools.Method(typeof(Harmony_Patch), nameof(UICharacterRenderer_SetCharacter_GetIndexWithSkip));
			foreach (var instruction in instructions)
			{
				if (instruction.LoadsConstant(11L))
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, getMaxForCheck);
				}
				else
				{
					if (instruction.operand as MethodInfo == customBookProperty)
					{
						passedTextureIndexInit = true;
						yield return instruction;
					}
					else
					{
						if (passedTextureIndexInit)
						{
							if (instruction.opcode == OpCodes.Ldarg_2)
							{
								yield return new CodeInstruction(OpCodes.Ldarg_1);
								yield return new CodeInstruction(OpCodes.Ldfld, textureIndexField);
							}
							else
							{
								yield return instruction;
							}
						}
						else
						{
							yield return instruction;
							if (instruction.Is(OpCodes.Stfld, textureIndexField) && !storedFixedIndex)
							{
								yield return new CodeInstruction(OpCodes.Ldarg_1);
								yield return new CodeInstruction(OpCodes.Ldarg_2);
								yield return new CodeInstruction(OpCodes.Call, getIndex);
								yield return new CodeInstruction(OpCodes.Stfld, textureIndexField);
								storedFixedIndex = true;
							}
						}
					}
				}
			}
		}
		static int UICharacterRenderer_SetCharacter_GetMaxWithoutSkip(UICharacterRenderer renderer)
		{
			int count = renderer.characterList.Count;
			if (count >= 11 && SkipCustomizationIndex())
			{
				count--;
			}
			return count;
		}
		static int UICharacterRenderer_SetCharacter_GetIndexWithSkip(int index)
		{
			if (index >= 10 && SkipCustomizationIndex())
			{
				index++;
			}
			return index;
		}
		static bool SkipCustomizationIndex()
		{
			return StageController.Instance.State == StageController.StageState.Battle && GameSceneManager.Instance.battleScene.gameObject.activeSelf;
		}
		/*
		//StageTurnPageButton
		[HarmonyPatch(typeof(GameOpeningController), nameof(GameOpeningController.StopOpening))]
		[HarmonyPostfix]
		static void GameOpeningController_StopOpening_Post()
		{
			try
			{
				StageButtonTool.currentEnemyUnitIndex = 0;
				StageButtonTool.currentLibrarianUnitIndex = 0;
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/StageTurnPage.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}*/
		//StageTurnPageButton
		[HarmonyPatch(typeof(UIBattleSettingPanel), nameof(UIBattleSettingPanel.OnOpen))]
		[HarmonyPrefix]
		static void UIBattleSettingPanel_OnOpen_Pre()
		{
			try
			{
				StageButtonTool.RefreshEnemy();
				StageButtonTool.RefreshLibrarian();
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/StageTurnPage1.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//StageTurnPageButton
		[HarmonyPatch(typeof(UIEnemyCharacterListPanel), nameof(UIEnemyCharacterListPanel.Activate))]
		[HarmonyPrefix]
		static void UIEnemyCharacterListPanel_Activate_Pre()
		{
			try
			{
				if (UIPanel.Controller.CurrentUIPhase == UIPhase.BattleSetting)
				{
					StageButtonTool.RefreshEnemy();
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/StageTurnPage2.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//StageTurnPageButton
		/*
		[HarmonyPatch(typeof(UIEnemyCharacterListPanel), nameof(UIEnemyCharacterListPanel.SetEnemyWave))]
		[HarmonyPrefix]
		static bool UIEnemyCharacterListPanel_SetEnemyWave_Pre(UIEnemyCharacterListPanel __instance, int targetWave)
		{
			try
			{
				if (__instance.currentEnemyStageinfo == null)
				{
					Debug.LogError("스테이지 인포 null");
					__instance.CharacterList.InitEnemyList(null, false, UIStoryLine.None);
					return false;
				}
				__instance.currentWave = targetWave;
				if (__instance.ob_blueeffect.activeSelf)
				{
					__instance.ob_blueeffect.gameObject.SetActive(false);
				}
				if (__instance.currentEnemyStageinfo.invitationInfo.combine == StageCombineType.BookRecipe)
				{
					if (__instance.currentEnemyStageinfo.storyType == UIStoryLine.TheBlueReverberationPrimary.ToString() || __instance.currentEnemyStageinfo.storyType == UIStoryLine.BlackSilence.ToString() || __instance.currentEnemyStageinfo.storyType == UIStoryLine.TwistedBlue.ToString() || __instance.currentEnemyStageinfo.storyType == UIStoryLine.Final.ToString())
					{
						__instance.CharacterList.SetBlockRaycast(false);

						List<UnitDataModel> list = GetEnemyUnitDataList(__instance.currentEnemyStageinfo, __instance.currentWave);
						if (list != null)
						{
							__instance.SetCharacterRenderer(list, true);
							UIStoryLine story = (UIStoryLine)Enum.Parse(typeof(UIStoryLine), __instance.currentEnemyStageinfo.storyType);
							__instance.CharacterList.InitEnemyList(list, false, story);
							__instance.UpdateFrame(story);
						}
						if (__instance.currentEnemyStageinfo.storyType == UIStoryLine.TheBlueReverberationPrimary.ToString() && !__instance.ob_blueeffect.activeSelf)
						{
							__instance.ob_blueeffect.gameObject.SetActive(true);
						}
					}
					else if (__instance.currentEnemyStageinfo.currentState == StoryState.Clear)
					{
						__instance.CharacterList.SetBlockRaycast(false);
						List<UnitDataModel> list2 = GetEnemyUnitDataList(__instance.currentEnemyStageinfo, __instance.currentWave);
						if (list2 != null)
						{
							__instance.SetCharacterRenderer(list2, true);
							__instance.CharacterList.InitEnemyList(list2, false, UIStoryLine.None);
							__instance.UpdateFrame(UIStoryLine.None);
						}
					}
					else
					{
						__instance.CharacterList.SetBlockRaycast(true);
						__instance.CharacterList.InitNotClearEnemyList();
						__instance.UpdateFrame(UIStoryLine.None);
					}
				}
				else if (LibraryModel.Instance.GetChapter() >= 2)
				{
					if (__instance.currentEnemyStageinfo.currentState == StoryState.Clear)
					{
						__instance.CharacterList.SetBlockRaycast(false);
						List<UnitDataModel> list3 = GetEnemyUnitDataList(__instance.currentEnemyStageinfo, __instance.currentWave);
						if (list3 != null)
						{
							__instance.SetCharacterRenderer(list3, true);
							__instance.CharacterList.InitEnemyList(list3, UIPanel.Controller.CurrentUIPhase == UIPhase.BattleSetting, UIStoryLine.None);
							__instance.UpdateFrame(UIStoryLine.None);
						}
					}
					else
					{
						__instance.CharacterList.SetBlockRaycast(true);
						__instance.CharacterList.InitNotClearEnemyList();
						__instance.UpdateFrame(UIStoryLine.None);
					}
				}
				else
				{
					__instance.CharacterList.InitNotClearEnemyList();
					__instance.UpdateFrame(UIStoryLine.None);
				}
				__instance.ReleaseCurrentSlot();
				return false;
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/StageTurnPage3.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return true;
		}
		*/
		/*
		static List<UnitDataModel> GetEnemyUnitDataList(StageClassInfo currentEnemyStageinfo, int currentWave)
		{
			if (EnemyListCache == null)
			{
				EnemyListCache = new Dictionary<LorId, List<List<UnitDataModel>>>();
			}
			if (!EnemyListCache.ContainsKey(currentEnemyStageinfo.id))
			{
				EnemyListCache[currentEnemyStageinfo.id] = new List<List<UnitDataModel>>();
			}
			while (EnemyListCache[currentEnemyStageinfo.id].Count <= currentWave)
			{
				EnemyListCache[currentEnemyStageinfo.id].Add(new List<UnitDataModel>());
			}
			if (EnemyListCache[currentEnemyStageinfo.id][currentWave].Count <= 0)
			{
				foreach (LorId id in currentEnemyStageinfo.waveList[currentWave].enemyUnitIdList)
				{
					EnemyUnitClassInfo data = EnemyUnitClassInfoList.Instance.GetData(id);
					int id2 = data.bookId[RandomUtil.SystemRange(data.bookId.Count)];
					UnitDataModel unitDataModel = new UnitDataModel(new LorId(data.workshopID, id2), SephirahType.None, false);
					unitDataModel.SetByEnemyUnitClassInfo(data);
					EnemyListCache[currentEnemyStageinfo.id][currentWave].Add(unitDataModel);
				}
			}
			return EnemyListCache[currentEnemyStageinfo.id][currentWave];
		}
		*/
		//StageTurnPageButton
		[HarmonyPatch(typeof(UILibrarianCharacterListPanel), nameof(UILibrarianCharacterListPanel.OnSetSephirah))]
		[HarmonyPrefix]
		static void UILibrarianCharacterListPanel_OnSetSephirah_Pre()
		{
			try
			{
				StageButtonTool.RefreshLibrarian();
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/StageTurnPage4.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		/*//StageTurnPageButton
		[HarmonyPatch(typeof(UICharacterList), nameof(UICharacterList.InitEnemyList))]
		[HarmonyPrefix]
		static void UICharacterList_InitEnemyList_Pre(ref List<UnitDataModel> unitList)
		{
			try
			{
				unitList = unitList.GetRange(0, Math.Min(5, unitList.Count));
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/StageTurnPage5.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//StageTurnPageButton
		[HarmonyPatch(typeof(UICharacterList), nameof(UICharacterList.InitLibrarianList))]
		[HarmonyPrefix]
		static bool UICharacterList_InitLibrarianList_Pre(UICharacterList __instance, List<UnitDataModel> unitList, SephirahType sephirah, bool Selectable)
		{
			if (unitList.Count <= 5)
			{
				return true;
			}
			try
			{
				__instance.isSelectableList = Selectable;
				__instance.highlightFrame.enabled = __instance.isSelectableList;
				for (int i = 0; i < __instance.slotList.Count; i++)
				{
					Color color = UIColorManager.Manager.GetSephirahColor(sephirah);
					if (sephirah == SephirahType.Binah)
					{
						color = UIColorManager.Manager.GetSephirahGlowColor(sephirah);
					}
					if (i < unitList.Count)
					{
						__instance.slotList[i].SetSlot(unitList[i], UIColorManager.Manager.GetSephirahColor(sephirah), false);
						__instance.slotList[i].SetOriginalFrameColor(color);
						__instance.slotList[i].SetOverallColor(color);
					}
					else
					{
						__instance.slotList[i].SetDisabledSlot();
						__instance.slotList[i].SetOriginalFrameColor(color);
						__instance.slotList[i].SetOverallColor(color);
					}
				}
				if (sephirah == SephirahType.Keter)
				{
					if (LibraryModel.Instance.PlayHistory.Start_TheBlueReverberationPrimaryBattle > 0 && LibraryModel.Instance.PlayHistory.first_TheBluePrimary_keterXmark == 0)
					{
						for (int j = 0; j < __instance.slotList.Count; j++)
						{
							if (j < unitList.Count && !unitList[j].isSephirah)
							{
								__instance.slotList[j].StartKeterXmarkForBlueAnim();
							}
						}
						LibraryModel.Instance.PlayHistory.first_TheBluePrimary_keterXmark = 1;
					}
					if (LibraryModel.Instance.PlayHistory.Start_EndContents == 1)
					{
						switch (LibraryModel.Instance.GetEndContentState())
						{
							case UIEndContentsState.None:
							case UIEndContentsState.BluePrimary:
							case UIEndContentsState.KeterCompleteOpen:
							case UIEndContentsState.ANOTHERETC:
								break;
							case UIEndContentsState.BlackSilence:
								for (int k = 0; k < __instance.slotList.Count; k++)
								{
									if (k < unitList.Count && unitList[k].isSephirah)
									{
										__instance.slotList[k].ActiveDeathMark();
									}
								}
								return false;
							case UIEndContentsState.TwistBlue:
								for (int l = 0; l < __instance.slotList.Count; l++)
								{
									if (l < unitList.Count && unitList[l].isSephirah)
									{
										__instance.slotList[l].ActiveDeathMark();
									}
								}
								break;
							default:
								return false;
						}
					}
				}
				return false;
				/*
				while (unitList.Count > 5)
				{
					unitList.RemoveAt(5);
				}/
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/StageTurnPage6.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return true;
		}

		//StageTurnPageButton
		[HarmonyPatch(typeof(UICharacterList), nameof(UICharacterList.InitUnitListFromBattleData))]
		[HarmonyPrefix]
		static bool UICharacterList_InitUnitListFromBattleData_Pre(UICharacterList __instance, List<UnitBattleDataModel> dataList)
		{
			if (dataList.Count <= 5)
			{
				return true;
			}
			try
			{
				List<UnitBattleDataModel> newdatalist = dataList.GetRange(0, 5);
				UIBattleSettingPanel uibattleSettingPanel = UI.UIController.Instance.GetUIPanel(UIPanelType.BattleSetting) as UIBattleSettingPanel;
				uibattleSettingPanel.currentAvailbleUnitslots.Clear();
				__instance.isSelectableList = true;
				__instance.highlightFrame.enabled = __instance.isSelectableList;
				for (int i = 0; i < __instance.slotList.Count; i++)
				{
					SephirahType currentSephirah = UI.UIController.Instance.CurrentSephirah;
					Color color = UIColorManager.Manager.GetSephirahColor(currentSephirah);
					if (currentSephirah == SephirahType.Binah)
					{
						color = UIColorManager.Manager.GetSephirahGlowColor(currentSephirah);
					}
					if (i < dataList.Count)
					{
						__instance.slotList[i].SetBattleCharacter(dataList[i]);
						__instance.slotList[i].SetOriginalFrameColor(color);
						__instance.slotList[i].SetOverallColor(color);
						if (dataList[i].unitData.IsLockUnit())
						{
							__instance.slotList[i].SetToggle(false);
							__instance.slotList[i].SetNoToggleState();
						}
						if (!dataList[i].isDead && !dataList[i].unitData.IsLockUnit())
						{
							uibattleSettingPanel.currentAvailbleUnitslots.Add(__instance.slotList[i]);
						}
					}
					else
					{
						__instance.slotList[i].SetDisabledSlot();
						__instance.slotList[i].SetOriginalFrameColor(color);
						__instance.slotList[i].SetOverallColor(color);
					}
				}
				uibattleSettingPanel.SetToggles();
				return false;
				/*
				while (dataList.Count > 5)
				{
					dataList.RemoveAt(5);
				}*
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/StageTurnPage7.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return true;
		}
		//StageTurnPageButton
		[HarmonyPatch(typeof(UICharacterList), nameof(UICharacterList.InitBattleEnemyList))]
		[HarmonyPrefix]
		static void UICharacterList_InitBattleEnemyList_Pre(ref List<UnitBattleDataModel> unitList)
		{
			try
			{
				while (unitList.Count > 5)
				{
					unitList.RemoveAt(5);
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/StageTurnPage8.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}*/
		//StageTurnPageButton
		[HarmonyPatch(typeof(UIBattleSettingPanel), nameof(UIBattleSettingPanel.SetToggles))]
		[HarmonyPrefix]
		static void UIBattleSettingPanel_SetToggles_Pre(UIBattleSettingPanel __instance, ref List<bool> __state)
		{
			List<UnitBattleDataModel> battleDataModels = StageController.Instance.GetCurrentStageFloorModel().GetUnitBattleDataList();
			if (battleDataModels.Count <= 5 || battleDataModels.Count <= StageController.Instance.GetCurrentWaveModel().AvailableUnitNumber)
			{
				return;
			}
			try
			{
				if (StageButtonTool.IsTurningPage)
				{
					__state = __instance.currentAvailbleUnitslots.Select(slot => slot._unitBattleData.IsAddedBattle).ToList();
					StageButtonTool.IsTurningPage = false;
				}
				else
				{
					foreach (UnitBattleDataModel unitBattleData in battleDataModels)
					{
						unitBattleData.IsAddedBattle = false;
					}
					int num = 0;
					for (int i = 0; num < StageController.Instance.GetCurrentWaveModel().AvailableUnitNumber; i++)
					{
						if (i >= battleDataModels.Count)
						{
							break;
						}
						if (battleDataModels[i].unitData.IsLockUnit() || battleDataModels[i].isDead)
						{
							continue;
						}
						battleDataModels[i].IsAddedBattle = true;
						num++;
					}
				}
			}
			catch (Exception ex)
			{
				foreach (UnitBattleDataModel unitBattleData in battleDataModels)
				{
					unitBattleData.IsAddedBattle = true;
				}
				__state = null;
				File.WriteAllText(Application.dataPath + "/Mods/StageTurnPage10.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		[HarmonyPatch(typeof(UIBattleSettingPanel), nameof(UIBattleSettingPanel.SetToggles))]
		[HarmonyPostfix]
		static void UIBattleSettingPanel_SetToggles_Post(UIBattleSettingPanel __instance, List<bool> __state)
		{
			if (__state == null)
			{
				return;
			}
			try
			{
				for (int i = 0; i < __state.Count; i++)
				{
					UICharacterSlot uicharacterSlot = __instance.currentAvailbleUnitslots[i];
					if (__state[i])
					{
						uicharacterSlot.SetYesToggleState();
					}
					else
					{
						uicharacterSlot.SetNoToggleState();
					}
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/StageTurnPage10b.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//StageTurnPageButton
		[HarmonyPatch(typeof(UIBattleSettingPanel), nameof(UIBattleSettingPanel.SelectedToggles))]
		[HarmonyPrefix]
		static void UIBattleSettingPanel_SelectedToggles_Pre()
		{
			try
			{
				List<UnitBattleDataModel> battleDataModels = StageController.Instance.GetCurrentStageFloorModel().GetUnitBattleDataList();
				if (battleDataModels.Count <= 5)
				{
					return;
				}
				if (StageController.Instance.GetCurrentWaveModel().AvailableUnitNumber == 1)
				{
					foreach (UnitBattleDataModel unitBattleData in battleDataModels)
					{
						unitBattleData.IsAddedBattle = false;
					}
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/StageTurnPage11.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		[HarmonyPatch(typeof(UIBattleSettingPanel), nameof(UIBattleSettingPanel.GetAvailableOne))]
		[HarmonyPrefix]
		static bool UIBattleSettingPanel_GetAvailableOne_Post(ref bool __result)
		{
			__result = StageController.Instance.GetCurrentStageFloorModel().GetUnitBattleDataList().Count(unit => unit.IsAddedBattle) == 1;
			return false;
		}
		[HarmonyPatch(typeof(UIBattleSettingPanel), nameof(UIBattleSettingPanel.GetAvailableMaxState))]
		[HarmonyPrefix]
		static bool UIBattleSettingPanel_GetAvailableMaxState_Post(ref bool __result)
		{
			__result = StageController.Instance.GetCurrentStageFloorModel().GetUnitBattleDataList().Count(unit => unit.IsAddedBattle) >= StageController.Instance.GetCurrentWaveModel().AvailableUnitNumber;
			return false;
		}
		[HarmonyPatch(typeof(UIBattleSettingPanel), nameof(UIBattleSettingPanel.GetAddedBattleUnitValue))]
		[HarmonyPrefix]
		static bool UIBattleSettingPanel_GetAddedBattleUnitValue_Post(ref int __result)
		{
			__result = StageController.Instance.GetCurrentStageFloorModel().GetUnitBattleDataList().Count(unit => unit.IsAddedBattle);
			return false;
		}
		//StageTurnPageButton
		[HarmonyPatch(typeof(BattleEmotionRewardInfoUI), nameof(BattleEmotionRewardInfoUI.SetData))]
		[HarmonyPrefix]
		static void BattleEmotionRewardInfoUI_SetData_Pre(BattleEmotionRewardInfoUI __instance, List<UnitBattleDataModel> units)
		{
			try
			{
				while (units.Count > __instance.slots.Count && __instance.slots.Count < MIN_EMOTION_SLOTS)
				{
					BattleEmotionRewardSlotUI newUI = UnityEngine.Object.Instantiate(__instance.slots[0]);
					__instance.slots.Add(newUI);
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/EmotionRewardInfoUI_SetData.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		[HarmonyPatch(typeof(BattleEmotionRewardInfoUI), nameof(BattleEmotionRewardInfoUI.SetData))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> BattleEmotionRewardInfoUI_SetData_In(IEnumerable<CodeInstruction> instructions)
		{
			var method = AccessTools.PropertyGetter(typeof(List<UnitBattleDataModel>), nameof(List<UnitBattleDataModel>.Count));
			foreach (var instruction in instructions)
			{
				yield return instruction;
				if (instruction.Is(OpCodes.Callvirt, method))
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(BattleEmotionRewardInfoUI), nameof(BattleEmotionRewardInfoUI.slots)));
					yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(List<BattleEmotionRewardSlotUI>), nameof(List<BattleEmotionRewardSlotUI>.Count)));
					yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Math), nameof(Math.Min), new Type[] { typeof(int), typeof(int) }));
				}
			}
		}
		static void AddFormationPosition(FormationModel Formation, int targetCount)
		{
			List<FormationPosition> _postionList = Formation._postionList;
			for (int i = _postionList.Count; i < targetCount; i++)
			{
				FormationPositionXmlData data = new FormationPositionXmlData()
				{
					name = "E" + i.ToString(),
					vector = new XmlVector2()
					{
						x = GetVector2X(i - 4),
						y = GetVector2Y(i - 4),
					},
					eventList = new List<FormationPositionEventXmlData>()
				};
				FormationPosition formationPosition = new FormationPosition(data)
				{
					eventList = new List<FormationPositionEvent>(),
					index = i
				};
				_postionList.Add(formationPosition);
			}
		}
		static int GetVector2X(int i)
		{
			switch (i)
			{
				case 1:
					return 12;
				case 2:
					return 12;
				case 3:
					return 9;
				case 4:
					return 9;
				case 5:
					return 8;
				case 6:
					return 8;
				case 7:
					return 21;
				case 8:
					return 21;
				case 9:
					return 20;
				case 10:
					return 20;
				case 11:
					return 2;
				case 12:
					return 2;
				case 13:
					return 22;
				case 14:
					return 22;
				case 15:
					return 22;
				default:
					return 12;
			}
		}
		static int GetVector2Y(int i)
		{
			switch (i)
			{
				case 1:
					return 7;
				case 2:
					return -9;
				case 3:
					return -5;
				case 4:
					return -15;
				case 5:
					return 19;
				case 6:
					return 9;
				case 7:
					return 19;
				case 8:
					return 9;
				case 9:
					return -5;
				case 10:
					return -15;
				case 11:
					return -14;
				case 12:
					return 14;
				case 13:
					return -16;
				case 14:
					return 0;
				case 15:
					return 16;
				default:
					return 0;
			}
		}
		static void AddFormationPositionForEnemy(FormationModel Formation, int targetCount)
		{
			List<FormationPosition> _postionList = Formation._postionList;
			int x = -23;
			int y = 18;
			for (int i = _postionList.Count; i < targetCount; i++)
			{
				FormationPositionXmlData data = new FormationPositionXmlData()
				{
					name = "E" + i.ToString(),
					vector = new XmlVector2()
					{
						x = x,
						y = y
					},
					eventList = null
				};
				x += 5;
				if (x > -3)
				{
					y -= 7;
					x = -23;
				}
				if (y < -17)
				{
					x = -12;
					y = 0;
				}
				FormationPosition formationPosition = new FormationPosition(data)
				{
					eventList = new List<FormationPositionEvent>(),
					index = i
				};
				_postionList.Add(formationPosition);
			}
		}
		public static BattleUnitModel SummonUnit(Faction Faction, LorId EnemyUnitID, LorId BookID, int Index = -1, string PlayerUnitName = "Null", bool ForcedlySummon = false)
		{
			try
			{
				if (EnemyUnitID == null)
				{
					EnemyUnitID = LorId.None;
				}
				BattleUnitModel battleUnitModel = null;
				if (BattleObjectManager.instance.ExistsUnit((BattleUnitModel x) => x.faction == Faction && x.index == Index) && !ForcedlySummon)
				{
					return battleUnitModel;
				}
				if (Faction == Faction.Enemy)
				{
					StageModel _stageModel = StageController.Instance._stageModel;
					BattleTeamModel _enemyTeam = StageController.Instance._enemyTeam;

					UnitBattleDataModel EnemyUnitBattleDataModel = UnitBattleDataModel.CreateUnitBattleDataByEnemyUnitId(_stageModel, EnemyUnitID);
					BattleObjectManager.instance.UnregisterUnitByIndex(Faction, Index);

					StageWaveModel currentWaveModel = StageController.Instance.GetCurrentWaveModel();
					UnitDataModel unitData = EnemyUnitBattleDataModel.unitData;
					battleUnitModel = BattleObjectManager.CreateDefaultUnit(Faction.Enemy);

					if (Index == -1)
					{
						battleUnitModel.index = -1;
						foreach (BattleUnitModel unit in BattleObjectManager.instance.GetAliveList(Faction))
						{
							if (unit.index >= battleUnitModel.index)
							{
								battleUnitModel.index = unit.index + 1;
							}
						}
					}
					else
					{
						battleUnitModel.index = Index;
					}

					battleUnitModel.formation = currentWaveModel.GetFormationPosition(battleUnitModel.index);
					if (EnemyUnitBattleDataModel.isDead)
					{
						return battleUnitModel;
					}
					battleUnitModel.grade = unitData.grade;
					battleUnitModel.SetUnitData(EnemyUnitBattleDataModel);
					battleUnitModel.OnCreated();
					_enemyTeam.AddUnit(battleUnitModel);
				}
				else
				{

					StageLibraryFloorModel currentStageFloorModel = StageController.Instance.GetCurrentStageFloorModel();
					BattleTeamModel _librarianTeam = StageController.Instance._librarianTeam;

					UnitDataModel unitDataModel = new UnitDataModel(BookID, currentStageFloorModel.Sephirah, false);
					UnitBattleDataModel unitBattleDataModel = new UnitBattleDataModel(StageController.Instance.GetStageModel(), unitDataModel);
					BattleObjectManager.instance.UnregisterUnitByIndex(Faction, Index);

					if (EnemyUnitClassInfoList.Instance.GetData(EnemyUnitID) != null)
					{
						unitDataModel.SetByEnemyUnitClassInfo(EnemyUnitClassInfoList.Instance.GetData(EnemyUnitID));
					}
					else
					{
						unitDataModel.SetTemporaryPlayerUnitByBook(BookID);
						unitDataModel.isSephirah = false;
						unitDataModel.customizeData.height = 175;
						unitDataModel.gender = Gender.N;
						unitDataModel.appearanceType = unitDataModel.gender;
						unitDataModel.SetCustomName(PlayerUnitName);
						unitDataModel.forceItemChangeLock = true;
					}
					if (PlayerUnitName != "Null")
					{
						unitDataModel.SetTempName(PlayerUnitName);
					}
					unitBattleDataModel.Init();

					battleUnitModel = BattleObjectManager.CreateDefaultUnit(Faction.Player);

					if (Index == -1)
					{
						battleUnitModel.index = -1;
						foreach (BattleUnitModel unit in BattleObjectManager.instance.GetAliveList(Faction))
						{
							if (unit.index >= battleUnitModel.index)
							{
								battleUnitModel.index = unit.index + 1;
							}
						}
					}
					else
					{
						battleUnitModel.index = Index;
					}

					battleUnitModel.grade = unitDataModel.grade;
					battleUnitModel.formation = currentStageFloorModel.GetFormationPosition(battleUnitModel.index);
					battleUnitModel.SetUnitData(unitBattleDataModel);
					unitDataModel._enemyUnitId = EnemyUnitID;
					battleUnitModel.OnCreated();
					_librarianTeam.AddUnit(battleUnitModel);
				}
				BattleObjectManager.instance.RegisterUnit(battleUnitModel);
				battleUnitModel.passiveDetail.OnUnitCreated();
				if (battleUnitModel.allyCardDetail.GetAllDeck().Count > 0)
				{
					battleUnitModel.allyCardDetail.ReturnAllToDeck();
					battleUnitModel.allyCardDetail.DrawCards(battleUnitModel.UnitData.unitData.GetStartDraw());
				}
				if (StageController.Instance.Phase <= StageController.StagePhase.ApplyLibrarianCardPhase)
				{
					battleUnitModel.OnRoundStartOnlyUI();
					battleUnitModel.RollSpeedDice();
				}
				BattleManagerUI.Instance.ui_unitListInfoSummary.UpdateCharacterProfileAll();
				int num2 = 0;
				foreach (BattleUnitModel battleUnitModel2 in BattleObjectManager.instance.GetList())
				{
					UICharacterRenderer.Instance.SetCharacter(battleUnitModel2.UnitData.unitData, num2++, true, false);
				}
				BattleObjectManager.instance.InitUI();
				return battleUnitModel;
			}
			catch (Exception ex)
			{
				Debug.LogError("召唤错误" + ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return null;
		}
		public static BattleUnitModel SummonUnitByUnitBattleData(Faction Faction, UnitBattleDataModel unitBattleData, LorId EnemyUnitID, int Index = -1, string PlayerUnitName = "Null", bool ForcedlySummon = false)
		{
			try
			{
				if (EnemyUnitID == null)
				{
					EnemyUnitID = LorId.None;
				}
				BattleUnitModel battleUnitModel = null;
				if (BattleObjectManager.instance.ExistsUnit((BattleUnitModel x) => x.faction == Faction && x.index == Index) && !ForcedlySummon)
				{
					return battleUnitModel;
				}
				if (Faction == Faction.Enemy)
				{
					StageModel _stageModel = StageController.Instance._stageModel;
					BattleTeamModel _enemyTeam = StageController.Instance._enemyTeam;


					BattleObjectManager.instance.UnregisterUnitByIndex(Faction, Index);
					StageWaveModel currentWaveModel = StageController.Instance.GetCurrentWaveModel();
					UnitDataModel unitData = unitBattleData.unitData;
					battleUnitModel = BattleObjectManager.CreateDefaultUnit(Faction.Enemy);

					if (Index == -1)
					{
						battleUnitModel.index = -1;
						foreach (BattleUnitModel unit in BattleObjectManager.instance.GetAliveList(Faction))
						{
							if (unit.index >= battleUnitModel.index)
							{
								battleUnitModel.index = unit.index + 1;
							}
						}
					}
					else
					{
						battleUnitModel.index = Index;
					}

					battleUnitModel.formation = currentWaveModel.GetFormationPosition(battleUnitModel.index);
					if (unitBattleData.isDead)
					{
						return battleUnitModel;
					}
					battleUnitModel.grade = unitData.grade;
					battleUnitModel.SetUnitData(unitBattleData);
					battleUnitModel.OnCreated();
					_enemyTeam.AddUnit(battleUnitModel);
				}
				else
				{

					StageLibraryFloorModel currentStageFloorModel = StageController.Instance.GetCurrentStageFloorModel();
					BattleTeamModel _librarianTeam = StageController.Instance._librarianTeam;

					BattleObjectManager.instance.UnregisterUnitByIndex(Faction, Index);


					if (PlayerUnitName != "Null")
					{
						unitBattleData.unitData.SetTempName(PlayerUnitName);
					}
					unitBattleData.Init();
					battleUnitModel = BattleObjectManager.CreateDefaultUnit(Faction.Player);

					if (Index == -1)
					{
						battleUnitModel.index = -1;
						foreach (BattleUnitModel unit in BattleObjectManager.instance.GetAliveList(Faction))
						{
							if (unit.index >= battleUnitModel.index)
							{
								battleUnitModel.index = unit.index + 1;
							}
						}
					}
					else
					{
						battleUnitModel.index = Index;
					}

					battleUnitModel.grade = unitBattleData.unitData.grade;
					battleUnitModel.formation = currentStageFloorModel.GetFormationPosition(battleUnitModel.index);
					battleUnitModel.SetUnitData(unitBattleData);
					unitBattleData.unitData._enemyUnitId = EnemyUnitID;
					battleUnitModel.OnCreated();
					_librarianTeam.AddUnit(battleUnitModel);
				}
				BattleObjectManager.instance.RegisterUnit(battleUnitModel);
				battleUnitModel.passiveDetail.OnUnitCreated();
				if (battleUnitModel.allyCardDetail.GetAllDeck().Count > 0)
				{
					battleUnitModel.allyCardDetail.ReturnAllToDeck();
					battleUnitModel.allyCardDetail.DrawCards(battleUnitModel.UnitData.unitData.GetStartDraw());
				}
				if (StageController.Instance.Phase <= StageController.StagePhase.ApplyLibrarianCardPhase)
				{
					battleUnitModel.OnRoundStartOnlyUI();
					battleUnitModel.RollSpeedDice();
				}
				BattleManagerUI.Instance.ui_unitListInfoSummary.UpdateCharacterProfileAll();
				int num2 = 0;
				foreach (BattleUnitModel battleUnitModel2 in BattleObjectManager.instance.GetList())
				{
					UICharacterRenderer.Instance.SetCharacter(battleUnitModel2.UnitData.unitData, num2++, true, false);
				}
				BattleObjectManager.instance.InitUI();
				return battleUnitModel;
			}
			catch (Exception ex)
			{
				Debug.LogError("召唤错误" + ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return null;
		}
		public static BattleUnitModel SummonUnitByUnitData(Faction Faction, UnitDataModel unitData, LorId EnemyUnitID, LorId BookID, int Index = -1, string PlayerUnitName = "Null", bool ForcedlySummon = false)
		{
			try
			{
				if (EnemyUnitID == null)
				{
					EnemyUnitID = LorId.None;
				}
				BattleUnitModel battleUnitModel = null;
				if (BattleObjectManager.instance.ExistsUnit((BattleUnitModel x) => x.faction == Faction && x.index == Index) && !ForcedlySummon)
				{
					return battleUnitModel;
				}
				if (Faction == Faction.Enemy)
				{
					StageModel _stageModel = StageController.Instance._stageModel;
					BattleTeamModel _enemyTeam = StageController.Instance._enemyTeam;

					UnitBattleDataModel EnemyUnitBattleDataModel = new UnitBattleDataModel(_stageModel, unitData);
					EnemyUnitBattleDataModel.Init();

					BattleObjectManager.instance.UnregisterUnitByIndex(Faction, Index);

					StageWaveModel currentWaveModel = StageController.Instance.GetCurrentWaveModel();

					battleUnitModel = BattleObjectManager.CreateDefaultUnit(Faction.Enemy);

					if (Index == -1)
					{
						battleUnitModel.index = -1;
						foreach (BattleUnitModel unit in BattleObjectManager.instance.GetAliveList(Faction))
						{
							if (unit.index >= battleUnitModel.index)
							{
								battleUnitModel.index = unit.index + 1;
							}
						}
					}
					else
					{
						battleUnitModel.index = Index;
					}

					battleUnitModel.formation = currentWaveModel.GetFormationPosition(battleUnitModel.index);
					if (EnemyUnitBattleDataModel.isDead)
					{
						return battleUnitModel;
					}
					battleUnitModel.grade = unitData.grade;
					battleUnitModel.SetUnitData(EnemyUnitBattleDataModel);
					battleUnitModel.OnCreated();
					_enemyTeam.AddUnit(battleUnitModel);
				}
				else
				{
					StageLibraryFloorModel currentStageFloorModel = StageController.Instance.GetCurrentStageFloorModel();
					BattleTeamModel _librarianTeam = StageController.Instance._librarianTeam;

					UnitBattleDataModel unitBattleDataModel = new UnitBattleDataModel(StageController.Instance.GetStageModel(), unitData);
					BattleObjectManager.instance.UnregisterUnitByIndex(Faction, Index);

					if (EnemyUnitClassInfoList.Instance.GetData(EnemyUnitID) != null)
					{
						unitData.SetByEnemyUnitClassInfo(EnemyUnitClassInfoList.Instance.GetData(EnemyUnitID));
					}

					if (PlayerUnitName != "Null")
					{
						unitData.SetTempName(PlayerUnitName);
					}

					unitBattleDataModel.Init();
					battleUnitModel = BattleObjectManager.CreateDefaultUnit(Faction.Player);

					if (Index == -1)
					{
						battleUnitModel.index = -1;
						foreach (BattleUnitModel unit in BattleObjectManager.instance.GetAliveList(Faction))
						{
							if (unit.index >= battleUnitModel.index)
							{
								battleUnitModel.index = unit.index + 1;
							}
						}
					}
					else
					{
						battleUnitModel.index = Index;
					}

					battleUnitModel.grade = unitData.grade;
					battleUnitModel.formation = currentStageFloorModel.GetFormationPosition(battleUnitModel.index);
					battleUnitModel.SetUnitData(unitBattleDataModel);
					unitData._enemyUnitId = EnemyUnitID;
					battleUnitModel.OnCreated();
					_librarianTeam.AddUnit(battleUnitModel);
				}
				BattleObjectManager.instance.RegisterUnit(battleUnitModel);
				battleUnitModel.passiveDetail.OnUnitCreated();
				if (battleUnitModel.allyCardDetail.GetAllDeck().Count > 0)
				{
					battleUnitModel.allyCardDetail.ReturnAllToDeck();
					battleUnitModel.allyCardDetail.DrawCards(battleUnitModel.UnitData.unitData.GetStartDraw());
				}
				if (StageController.Instance.Phase <= StageController.StagePhase.ApplyLibrarianCardPhase)
				{
					battleUnitModel.OnRoundStartOnlyUI();
					battleUnitModel.RollSpeedDice();
				}
				BattleManagerUI.Instance.ui_unitListInfoSummary.UpdateCharacterProfileAll();
				int num2 = 0;
				foreach (BattleUnitModel battleUnitModel2 in BattleObjectManager.instance.GetList())
				{
					UICharacterRenderer.Instance.SetCharacter(battleUnitModel2.UnitData.unitData, num2++, true, false);
				}
				BattleObjectManager.instance.InitUI();
				return battleUnitModel;
			}
			catch (Exception ex)
			{
				Debug.LogError("召唤错误" + ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return null;
		}
	}
}
/*
namespace SummonLiberation
{
	public class StageAndWave : IEquatable<StageAndWave>
	{
		public StageAndWave(LorId stageid, int wave)
		{
			_stageid = stageid;
			_wave = wave;
		}
		public override bool Equals(object obj)
		{
			LorId other;
			return (other = (obj as LorId)) != null && Equals(other);
		}
		public bool Equals(StageAndWave other)
		{
			return _stageid == other._stageid && _wave == other._wave;
		}
		public override int GetHashCode()
		{
			if (_stageid == null)
			{
				Debug.LogError("error");
			}
			return _stageid.GetHashCode() + _wave.GetHashCode();
		}
		public static bool operator ==(StageAndWave lhs, StageAndWave rhs)
		{
			if (lhs == null)
			{
				lhs = None;
			}
			if (rhs == null)
			{
				rhs = None;
			}
			return lhs.Equals(rhs);
		}
		public static bool operator !=(StageAndWave lhs, StageAndWave rhs)
		{
			return !(lhs == rhs);
		}

		public LorId _stageid = new LorId(-1);

		public int _wave = -1;

		public static readonly StageAndWave None = new StageAndWave(new LorId(-1), -1);
	}
}*/