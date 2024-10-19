using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BattleCharacterProfile;
using HarmonyLib;
using UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace ExtendedLoader
{
	[HarmonyPatch]
	class UnitLimitPatch
	{
		#region UnitLimitUtil

		internal static int minEmotionSlots = 9;
		internal static float yShift = 64f;

		[HarmonyPatch(typeof(StageWaveModel), nameof(StageWaveModel.GetUnitBattleDataListByFormation))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> StageWaveModel_GetUnitBattleDataListByFormation_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			foreach (var instruction in instructions)
			{
				if (instruction.opcode == OpCodes.Ldc_I4_5)
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StageWaveModel), nameof(StageWaveModel._formation)));
					yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(FormationModel), nameof(FormationModel.PostionList)));
					yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(List<FormationPosition>), nameof(List<FormationPosition>.Count)));
				}
				else
				{
					yield return instruction;
				}
			}
		}

		[HarmonyPatch(typeof(BattleUnitInfoManagerUI), nameof(BattleUnitInfoManagerUI.Initialize))]
		[HarmonyPrefix]
		[HarmonyPriority(Priority.High + 1)]
		static void BattleUnitInfoManagerUI_Initialize_Prefix(BattleUnitInfoManagerUI __instance)
		{
			try
			{
				var lastAlly = __instance.allyProfileArray.Length - 1;
				if (lastAlly < minEmotionSlots - 1)
				{
					var allyProfileArray2 = new List<BattleCharacterProfileUI>(__instance.allyProfileArray);
					for (int i = lastAlly + 1; i < minEmotionSlots; i++)
					{
						allyProfileArray2.Add(UnityEngine.Object.Instantiate(allyProfileArray2[lastAlly], allyProfileArray2[lastAlly].transform.parent));
						allyProfileArray2[i].gameObject.SetActive(false);
						allyProfileArray2[i].transform.localPosition = allyProfileArray2[lastAlly].transform.localPosition + new Vector3(0f, (i - lastAlly) * yShift, 0f);
					}
					__instance.allyProfileArray = allyProfileArray2.ToArray();
				}
				var lastEnemy = __instance.enemyProfileArray.Length - 1;
				if (lastEnemy < minEmotionSlots - 1)
				{
					var enemyProfileArray2 = new List<BattleCharacterProfileUI>(__instance.enemyProfileArray);
					for (int i = lastEnemy + 1; i < minEmotionSlots; i++)
					{
						enemyProfileArray2.Add(UnityEngine.Object.Instantiate(enemyProfileArray2[lastEnemy], enemyProfileArray2[lastEnemy].transform.parent));
						enemyProfileArray2[i].gameObject.SetActive(false);
						enemyProfileArray2[i].transform.localPosition = enemyProfileArray2[lastEnemy].transform.localPosition + new Vector3(0f, (i - lastEnemy) * yShift, 0f);
					}
					__instance.enemyProfileArray = enemyProfileArray2.ToArray();
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
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
				Debug.LogException(ex);
			}
		}

		[HarmonyPatch(typeof(BattleUnitInfoManagerUI), nameof(BattleUnitInfoManagerUI.UpdateCharacterProfile))]
		[HarmonyPostfix]
		static void BattleUnitInfoManagerUI_UpdateCharacterProfile_Postfix(BattleUnitModel unit, float hp, int bp, BattleBufUIDataList bufDataList = null)
		{
			try
			{
				unit.view.unitBottomStatUI.UpdateStatUI(hp, bp, bufDataList);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		[HarmonyPatch(typeof(BattleEmotionCoinUI), nameof(BattleEmotionCoinUI.Init))]
		[HarmonyPrefix]
		static void BattleEmotionCoinUI_Init_Prefix(BattleEmotionCoinUI __instance)
		{
			try
			{
				var unitList = BattleManagerUI.Instance.ui_unitListInfoSummary;
				var lastLibrarian = __instance.librarian.Length - 1;
				if (lastLibrarian < minEmotionSlots - 1)
				{
					var librarian = new List<BattleEmotionCoinUI.BattleEmotionCoinData>(__instance.librarian);
					for (int i = lastLibrarian + 1; i < minEmotionSlots; i++)
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
				if (lastEnermy < minEmotionSlots - 1)
				{
					var enermy = new List<BattleEmotionCoinUI.BattleEmotionCoinData>(__instance.enermy);
					for (int i = lastEnermy + 1; i < minEmotionSlots; i++)
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
				Debug.LogException(ex);
			}
		}

		[HarmonyPatch(typeof(BattleEmotionCoinUI), nameof(BattleEmotionCoinUI.Init))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> BattleEmotionCoinUI_Init_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var method = AccessTools.Method(typeof(BattleObjectManager), nameof(BattleObjectManager.GetAliveList), new Type[] { typeof(bool) });
			foreach (var instruction in instructions)
			{
				yield return instruction;
				if (instruction.Is(OpCodes.Callvirt, method))
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UnitLimitPatch), nameof(BattleEmotionCoinUI_Init_FilterUnitCount)));
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

		[HarmonyPatch(typeof(BattleEmotionCoinUI), nameof(BattleEmotionCoinUI.Acquisition))]
		[HarmonyPrefix]
		static bool BattleEmotionCoinUI_Acquisition_Prefix(BattleUnitModel unit)
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
				Debug.LogException(ex);
			}
			return true;
		}

		[HarmonyPatch(typeof(UICharacterRenderer), nameof(UICharacterRenderer.SetCharacter))]
		[HarmonyPrefix]
		[HarmonyPriority(Priority.High)]
		static void UICharacterRenderer_SetCharacter_Prefix(UICharacterRenderer __instance, int index)
		{
			try
			{
				var fixedIndex = UICRIndexHelpers.GetIndexWithSkip(index);
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
				Debug.LogException(ex);
			}
		}

		[HarmonyPatch(typeof(UICharacterRenderer), nameof(UICharacterRenderer.GetRenderTextureByIndexAndSize))]
		[HarmonyPatch(typeof(UICharacterRenderer), nameof(UICharacterRenderer.GetRenderTextureByIndex))]
		[HarmonyPatch(typeof(UICharacterRenderer), nameof(UICharacterRenderer.GetUICharacterByIndex))]
		[HarmonyPatch(typeof(UICharacterRenderer), nameof(UICharacterRenderer.Redraw))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> UICharacterRenderer_GeneralLimitRaising_Transpiler(IEnumerable<CodeInstruction> instructions)
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
		static IEnumerable<CodeInstruction> UICharacterRenderer_SetCharacter_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			bool passedTextureIndexInit = false;
			bool storedFixedIndex = false;
			var customBookProperty = AccessTools.PropertyGetter(typeof(UnitDataModel), nameof(UnitDataModel.CustomBookItem));
			var textureIndexField = AccessTools.Field(typeof(UnitDataModel), nameof(UnitDataModel.textureIndex));
			var getMaxForCheck = AccessTools.Method(typeof(UICRIndexHelpers), nameof(UICRIndexHelpers.GetMaxWithoutSkip));
			var getIndex = AccessTools.Method(typeof(UICRIndexHelpers), nameof(UICRIndexHelpers.GetIndexWithSkip));
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
		[HarmonyPatch(typeof(BattleEmotionRewardInfoUI), nameof(BattleEmotionRewardInfoUI.SetData))]
		[HarmonyPrefix]
		static void BattleEmotionRewardInfoUI_SetData_Prefix(BattleEmotionRewardInfoUI __instance, List<UnitBattleDataModel> units)
		{
			try
			{
				while (units.Count > __instance.slots.Count)
				{
					BattleEmotionRewardSlotUI newUI = UnityEngine.Object.Instantiate(__instance.slots[0]);
					__instance.slots.Add(newUI);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		#endregion

		#region former StageTurnPageButton

		[HarmonyPatch(typeof(UIBattleSettingPanel), nameof(UIBattleSettingPanel.SetToggles))]
		[HarmonyPrefix]
		static void UIBattleSettingPanel_SetToggles_Prefix(UIBattleSettingPanel __instance, ref List<bool> __state)
		{
			try
			{
				if (UnitUIUtils.updatingLibrarianForWave)
				{
					__state = __instance.currentAvailbleUnitslots.Select(slot => slot._unitBattleData.IsAddedBattle).ToList();
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		[HarmonyPatch(typeof(UIBattleSettingPanel), nameof(UIBattleSettingPanel.SetToggles))]
		[HarmonyPostfix]
		static void UIBattleSettingPanel_SetToggles_Postfix(UIBattleSettingPanel __instance, List<bool> __state)
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
				Debug.LogException(ex);
			}
		}

		[HarmonyPatch(typeof(UICharacterListPanel), nameof(UICharacterListPanel.SetCharacterRenderer), new Type[] { typeof(List<UnitDataModel>), typeof(bool) })]
		[HarmonyPatch(typeof(UICharacterListPanel), nameof(UICharacterListPanel.SetCharacterRenderer), new Type[] { typeof(List<UnitBattleDataModel>), typeof(bool) })]
		[HarmonyPrefix]
		static void UICharacterListPanel_SetCharacterRenderer_Prefix(UICharacterListPanel __instance, System.Collections.IList unitList)
		{
			UnitUIUtils.SetSlotCount(__instance.CharacterList, unitList != null ? unitList.Count : 0);
		}

		[HarmonyPatch(typeof(UICharacterList), nameof(UICharacterList.InitNotClearEnemyList))]
		[HarmonyPrefix]
		static void UICharacterList_InitNotClearEnemyList_Prefix(UICharacterList __instance)
		{
			UnitUIUtils.SetSlotCount(__instance, 0);
		}

		[HarmonyPatch(typeof(UICharacterListPanel), nameof(UICharacterListPanel.SetCharacterRenderer), new Type[] { typeof(List<UnitDataModel>), typeof(bool) })]
		[HarmonyPatch(typeof(UICharacterListPanel), nameof(UICharacterListPanel.SetCharacterRenderer), new Type[] { typeof(List<UnitBattleDataModel>), typeof(bool) })]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> UICharacterListPanel_SetCharacterRenderer_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var skipFix = AccessTools.Method(typeof(UnitLimitPatch), nameof(UnitLimitPatch.SkipCustomizationIndex));
			var startFix = AccessTools.Method(typeof(UnitLimitPatch), nameof(UnitLimitPatch.FixRightStartIndex));
			CodeInstruction fixLeaveFor = null;
			foreach (var instruction in instructions)
			{
				if (fixLeaveFor != null && instruction.opcode != OpCodes.Nop)
				{
					yield return new CodeInstruction(OpCodes.Nop).MoveBlocksFrom(fixLeaveFor);
					fixLeaveFor = null;
				}
				if (instruction.opcode == OpCodes.Stloc_0 || instruction.opcode == OpCodes.Stloc_3)
				{
					yield return new CodeInstruction(OpCodes.Call, skipFix);
				}
				yield return instruction;
				if (instruction.LoadsConstant(5L))
				{
					yield return new CodeInstruction(OpCodes.Call, startFix);
				}
				else if (instruction.opcode == OpCodes.Leave || instruction.opcode == OpCodes.Leave_S)
				{
					fixLeaveFor = instruction;
				}
			}
			if (fixLeaveFor != null)
			{
				yield return new CodeInstruction(OpCodes.Nop).MoveBlocksFrom(fixLeaveFor);
			}
		}
		static int SkipCustomizationIndex(int index)
		{
			return index >= 10 ? index + 1 : index;
		}
		static int FixRightStartIndex(int index)
		{
			var enemyPanel = UnitUIUtils.GetEnemyCharacterListPanel();
			return enemyPanel ? enemyPanel.CharacterList.slotList.Count : index;
		}

		#endregion

		#region WaveExtension

		[HarmonyPatch(typeof(UIBattleSettingWaveList), nameof(UIBattleSettingWaveList.SetData))]
		[HarmonyPrefix]
		static void UIBattleSettingWaveList_SetData_Prefix(UIBattleSettingWaveList __instance, StageModel stage)
		{
			try
			{
				var scrollRect = __instance.transform.parent.GetComponent<ScrollRect>();
				if (!scrollRect)
				{
					var target = __instance.gameObject.transform as RectTransform;
					var scrollView = new GameObject("[Rect]WaveListView");
					var scrollTransform = scrollView.AddComponent<RectTransform>();
					scrollTransform.SetParent(target.parent);
					scrollTransform.localPosition = new Vector3(0, -35, 0);
					scrollTransform.localEulerAngles = Vector3.zero;
					scrollTransform.localScale = Vector3.one;
					scrollTransform.sizeDelta = Vector2.one * 800;
					scrollView.AddComponent<RectMask2D>();
					target.SetParent(scrollTransform, true);
					scrollRect = scrollView.AddComponent<ScrollRect>();
					scrollRect.content = target;
				}
				scrollRect.scrollSensitivity = 15f;
				scrollRect.horizontal = false;
				scrollRect.vertical = true;
				scrollRect.movementType = ScrollRect.MovementType.Elastic;
				scrollRect.elasticity = 0.1f;
				if (stage.waveList.Count > __instance.waveSlots.Count)
				{
					var newList = new List<UIBattleSettingWaveSlot>(stage.waveList.Count - __instance.waveSlots.Count);
					for (int i = __instance.waveSlots.Count; i < stage.waveList.Count; i++)
					{
						UIBattleSettingWaveSlot uibattleSettingWaveSlot = UnityEngine.Object.Instantiate(__instance.waveSlots[0], __instance.waveSlots[0].transform.parent);
						uibattleSettingWaveSlot.name = $"[Rect]WaveSlot ({i})";
						newList.Add(uibattleSettingWaveSlot);
					}
					newList.Reverse();
					__instance.waveSlots.InsertRange(0, newList);
				}
				for (int i = 0; i < __instance.waveSlots.Count; i++)
				{
					__instance.waveSlots[i].gameObject.transform.localScale = Vector3.one;
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		[HarmonyPatch(typeof(UIEnemyCharacterListPanel), nameof(UIEnemyCharacterListPanel.SetEnemyInfo))]
		[HarmonyPrefix]
		static void UIEnemyCharacterListPanel_SetEnemyInfo_Prefix(UIEnemyCharacterListPanel __instance, StageClassInfo data)
		{
			if (data != null && UIPanel.Controller.CurrentUIPhase == UIPhase.Invitation)
			{
				int targetNum = Math.Min(data.waveList.Count, 16);
				if (targetNum > __instance.StageNumList.Count)
				{
					for (int i = __instance.StageNumList.Count; i < targetNum; i++)
					{
						var clone = UnityEngine.Object.Instantiate(__instance.StageNumList[0], __instance.StageNumList[0].transform.parent);
						clone.name = $"[Button]Num{i + 1}";
						__instance.StageNumList.Add(clone);
						var waveText = clone.GetComponentsInChildren<TextMeshProUGUI>().FirstOrDefault();
						waveText.text = (i + 1).ToString();
						var submitEvent = clone.GetComponent<UICustomSelectable>().SubmitEvent;
						submitEvent.RemoveAllListeners();
						submitEvent.m_PersistentCalls.Clear();
						submitEvent.DirtyPersistentCalls();
						int wave = i;
						submitEvent.AddCall(new InvokableCall(() => __instance.ChangeEnemyWave(wave)));
					}
				}
			}
		}

		#endregion
	}
}
