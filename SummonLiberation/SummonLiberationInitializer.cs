using BaseMod;
using BattleCharacterProfile;
using ExtendedLoader;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Workshop;

namespace SummonLiberation
{
    public class Harmony_Patch
    {
        public static void Init()
        {
            //初始化侧边栏信息槽
            enemyProfileArray2 = new List<BattleCharacterProfileUI>();
            allyProfileArray2 = new List<BattleCharacterProfileUI>();
            enermy2 = new List<BattleEmotionCoinUI.BattleEmotionCoinData>();
            librarian2 = new List<BattleEmotionCoinUI.BattleEmotionCoinData>();
            EnemyListCache = new Dictionary<LorId, List<List<UnitDataModel>>>();
        }
        //Enlarge Librarian's formation
        [HarmonyPatch(typeof(LibraryFloorModel), "Init")]
        [HarmonyPostfix]
        private static void LibraryFloorModel_Init_Post(ref List<int> ____formationIndex, ref FormationModel ____defaultFormation, ref FormationModel ____formation)
        {
            try
            {
                ____formationIndex = new List<int>();
                for (int i = 0; i < 99; i++)
                {
                    ____formationIndex.Add(i);
                }
                ____formationIndex[0] = 1;
                ____formationIndex[1] = 0;
                for (int j = 3; j < 99; j++)
                {
                    ____formationIndex[j] = j;
                }
                ____defaultFormation = new FormationModel(Singleton<FormationXmlList>.Instance.GetData(1));
                AddFormationPosition(____defaultFormation);
                ____formation = ____defaultFormation;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/LFIerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //Enlarge Enemy's formation
        [HarmonyPatch(typeof(StageWaveModel), "Init")]
        [HarmonyPostfix]
        private static void StageWaveModel_Init_Post(ref FormationModel ____formation, ref List<int> ____formationIndex)
        {
            try
            {
                for (int i = 5; i < 100; i++)
                {
                    ____formationIndex.Add(i);
                }
                AddFormationPositionForEnemy(____formation);
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/SWMIerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //Enlarge Enemy's formation
        [HarmonyPatch(typeof(StageWaveModel), "GetUnitBattleDataListByFormation")]
        [HarmonyPrefix]
        private static bool StageWaveModel_GetUnitBattleDataListByFormation_Pre(StageWaveModel __instance, ref List<UnitBattleDataModel> __result, List<UnitBattleDataModel> ____unitList)
        {
            try
            {
                List<UnitBattleDataModel> list = new List<UnitBattleDataModel>();
                int num = Math.Max(____unitList.Count, 5);
                for (int i = 0; i < num; i++)
                {
                    int formationIndex = __instance.GetFormationIndex(i);
                    if (formationIndex < ____unitList.Count)
                    {
                        list.Add(____unitList[formationIndex]);
                    }
                }
                __result = list;
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/SWMGUBDLBFerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //BattleUnitProfileArray Up to 9
        [HarmonyPatch(typeof(BattleUnitInfoManagerUI), "Initialize")]
        [HarmonyPrefix]
        private static bool BattleUnitInfoManagerUI_Initialize_Pre(BattleUnitInfoManagerUI __instance, IList<BattleUnitModel> unitList, ref Direction ___allyDirection)
        {
            try
            {
                if (allyProfileArray2.Count < 9)
                {
                    allyProfileArray2.Clear();
                    for (int i = 0; i < 9; i++)
                    {
                        if (__instance.allyProfileArray.Length > i)
                        {
                            allyProfileArray2.Add(__instance.allyProfileArray[i]);
                        }
                        else
                        {
                            allyProfileArray2.Add(UnityEngine.Object.Instantiate(allyProfileArray2[4], allyProfileArray2[4].transform.parent));
                            allyProfileArray2[i].gameObject.transform.localPosition += new Vector3(0f, (i - 4) * 64f, 0f);
                        }
                    }
                }
                if (enemyProfileArray2.Count < 9)
                {
                    enemyProfileArray2.Clear();
                    for (int i = 0; i < 9; i++)
                    {
                        if (__instance.enemyProfileArray.Length > i)
                        {
                            enemyProfileArray2.Add(__instance.enemyProfileArray[i]);
                        }
                        else
                        {
                            enemyProfileArray2.Add(UnityEngine.Object.Instantiate(enemyProfileArray2[4], enemyProfileArray2[4].transform.parent));
                            enemyProfileArray2[i].gameObject.transform.localPosition += new Vector3(0f, (i - 4) * 64f, 0f);
                        }
                    }
                }

                ___allyDirection = Singleton<StageController>.Instance.AllyFormationDirection;

                for (int i = 0; i < __instance.allyProfileArray.Length; i++)
                {
                    __instance.allyProfileArray[i].gameObject.SetActive(false);
                }
                for (int j = 0; j < __instance.enemyProfileArray.Length; j++)
                {
                    __instance.enemyProfileArray[j].gameObject.SetActive(false);
                }

                for (int i = 0; i < allyProfileArray2.Count; i++)
                {
                    allyProfileArray2[i].gameObject.SetActive(false);
                }
                for (int j = 0; j < enemyProfileArray2.Count; j++)
                {
                    enemyProfileArray2[j].gameObject.SetActive(false);
                }

                __instance.enemyarray = ((___allyDirection == Direction.RIGHT) ? enemyProfileArray2.ToArray() : allyProfileArray2.ToArray());
                __instance.allyarray = ((___allyDirection == Direction.RIGHT) ? allyProfileArray2.ToArray() : enemyProfileArray2.ToArray());

                for (int k = 0; k < unitList.Count; k++)
                {
                    BattleUnitModel battleUnitModel = unitList[k];
                    int index = battleUnitModel.index;
                    if (index >= 9)
                    {
                        continue;
                    }
                    if (battleUnitModel.faction == Faction.Enemy)
                    {
                        __instance.enemyarray[index].gameObject.SetActive(true);
                        __instance.enemyarray[index].Initialize();
                        __instance.enemyarray[index].SetUnitModel(battleUnitModel);
                    }
                    else
                    {
                        __instance.allyarray[index].gameObject.SetActive(true);
                        __instance.allyarray[index].Initialize();
                        __instance.allyarray[index].SetUnitModel(battleUnitModel);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/BUIFMIerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //CharacterProfile under character
        [HarmonyPatch(typeof(BattleUnitInfoManagerUI), "UpdateCharacterProfile")]
        [HarmonyPostfix]
        private static void BattleUnitInfoManagerUI_UpdateCharacterProfile_Post(BattleUnitModel unit, float hp, int bp, BattleBufUIDataList bufDataList = null)
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
        [HarmonyPatch(typeof(BattleEmotionCoinUI), "Init")]
        [HarmonyPrefix]
        private static bool BattleEmotionCoinUI_Init_Pre(BattleEmotionCoinUI __instance, ref Dictionary<int, BattleEmotionCoinUI.BattleEmotionCoinData> ____librarian_lib, ref Dictionary<int, BattleEmotionCoinUI.BattleEmotionCoinData> ____enemy_lib, ref Dictionary<int, Queue<EmotionCoinType>> ____lib_queue, ref Dictionary<int, Queue<EmotionCoinType>> ____ene_queue, ref bool ____init)
        {
            try
            {
                ____librarian_lib.Clear();
                ____enemy_lib.Clear();
                ____lib_queue.Clear();
                ____ene_queue.Clear();
                List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(false);
                int num = 0;
                int num2 = 0;
                Direction allyFormationDirection = Singleton<StageController>.Instance.AllyFormationDirection;

                if (librarian2.Count < 9)
                {
                    librarian2.Clear();
                    for (int i = 0; i < 9; i++)
                    {
                        if (__instance.librarian.Length > i)
                        {
                            librarian2.Add(__instance.librarian[i]);
                        }
                        else
                        {
                            librarian2.Add(new BattleEmotionCoinUI.BattleEmotionCoinData()
                            {
                                cosFactor = 1f,
                                sinFactor = 1f,
                                target = UnityEngine.Object.Instantiate(__instance.librarian[4].target, __instance.librarian[4].target)
                            });
                            librarian2[i].target.localPosition += new Vector3(0f, (i - 4) * 64f, 0f);
                        }
                    }
                }
                if (enermy2.Count < 9)
                {
                    enermy2.Clear();
                    for (int i = 0; i < 9; i++)
                    {
                        if (__instance.enermy.Length > i)
                        {
                            enermy2.Add(__instance.enermy[i]);
                        }
                        else
                        {
                            enermy2.Add(new BattleEmotionCoinUI.BattleEmotionCoinData()
                            {
                                cosFactor = 1f,
                                sinFactor = 1f,
                                target = UnityEngine.Object.Instantiate(__instance.enermy[4].target, __instance.enermy[4].target)
                            });
                            enermy2[i].target.localPosition += new Vector3(0f, (i - 4) * 64f, 0f);
                        }
                    }
                }
                __instance.librarian = librarian2.ToArray();
                __instance.enermy = enermy2.ToArray();

                BattleEmotionCoinUI.BattleEmotionCoinData[] array = (allyFormationDirection == Direction.RIGHT) ? __instance.librarian : __instance.enermy;
                BattleEmotionCoinUI.BattleEmotionCoinData[] array2 = (allyFormationDirection == Direction.RIGHT) ? __instance.enermy : __instance.librarian;

                foreach (BattleUnitModel battleUnitModel in aliveList)
                {
                    if (battleUnitModel.faction == Faction.Enemy)
                    {
                        if (num2 <= 8)
                        {
                            ____enemy_lib.Add(battleUnitModel.id, array2[num2++]);
                        }
                    }
                    else if (num <= 8)
                    {
                        ____librarian_lib.Add(battleUnitModel.id, array[num++]);
                    }
                }
                ____init = true;
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/BECU_Initerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //BattleEmotionCoinUI
        [HarmonyPatch(typeof(BattleEmotionCoinUI), "Acquisition")]
        [HarmonyPrefix]
        private static bool BattleEmotionCoinUI_Acquisition_Pre(BattleUnitModel unit)
        {
            try
            {
                if (SingletonBehavior<BattleManagerUI>.Instance.ui_unitListInfoSummary.GetProfileUI(unit) == null)
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
        [HarmonyPatch(typeof(EmotionBattleTeamModel), "UpdateUnitList")]
        [HarmonyPrefix]
        private static bool EmotionBattleTeamModel_UpdateUnitList_Pre(EmotionBattleTeamModel __instance, ref List<UnitBattleDataModel> ____unitlist)
        {/*
            try
            {
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/TeamEmotion_UpdateListerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }*/
            return true;
        }
        //ProfileUI Unit Preview
        [HarmonyPatch(typeof(UICharacterRenderer), "SetCharacter")]
        [HarmonyPrefix]
        private static bool UICharacterRenderer_SetCharacter_Pre(UICharacterRenderer __instance, UnitDataModel unit, int index, bool forcelyReload = false)
        {
            if (__instance.characterList.Capacity < 199)
            {
                List<UICharacter> uICharacters = new List<UICharacter>(199);
                uICharacters.AddRange(__instance.characterList);
                while (uICharacters.Count < 199)
                {
                    uICharacters.Add(new UICharacter(null, null));
                }
                __instance.characterList = uICharacters;
            }
            while (__instance.cameraList.Count <= index + 1)
            {
                Camera camera = UnityEngine.Object.Instantiate(__instance.cameraList[0], __instance.cameraList[0].transform.parent);
                camera.name = "[Camera]" + (index + 1).ToString();
                camera.targetTexture = UnityEngine.Object.Instantiate(__instance.cameraList[0].targetTexture);
                camera.targetTexture.name = "RT_Character_" + (index + 1).ToString();
                camera.transform.position += new Vector3(10f * index, 0f, 0f);
                __instance.cameraList.Add(camera);
            }
            try
            {
                unit.textureIndex = index;
                if (index >= 10 && Singleton<StageController>.Instance.State == StageController.StageState.Battle && GameSceneManager.Instance.battleScene.gameObject.activeSelf)
                {
                    unit.textureIndex++;
                }
                UICharacter uicharacter = __instance.characterList[index];
                if (forcelyReload || (uicharacter.unitAppearance != null && uicharacter.unitModel != unit))
                {
                    if (uicharacter.unitAppearance != null)
                    {
                        Singleton<AssetBundleManagerRemake>.Instance.ReleaseSdObject(uicharacter.unitAppearance.resourceName);
                        if (uicharacter.unitAppearance.gameObject != null)
                        {
                            try
                            {
                                UnityEngine.Object.Destroy(uicharacter.unitAppearance.gameObject);
                            }
                            catch { }
                        }
                    }
                    uicharacter.unitAppearance = null;
                    uicharacter.unitModel = null;
                    if (uicharacter.resName != "")
                    {
                        Singleton<AssetBundleManagerRemake>.Instance.ReleaseSdObject(uicharacter.resName);
                    }
                    uicharacter.resName = "";
                }
                if (uicharacter.unitAppearance == null)
                {
                    string resName = string.Empty;
                    int num = 10 * index;
                    BookModel customBookItem = unit.CustomBookItem;
                    string characterName = customBookItem.GetCharacterName();
                    try
                    {
                        string s = string.Empty;
                        switch (unit.appearanceType)
                        {
                            case Gender.F:
                                s = "_F";
                                break;
                            case Gender.M:
                                s = "_M";
                                break;
                            case Gender.N:
                                s = "_N";
                                break;
                        }
                        bool isCustom = false;
                        if (!string.IsNullOrWhiteSpace(unit.workshopSkin))
                        {
                            WorkshopSkinData workshopSkinData = Singleton<CustomizingResourceLoader>.Instance.GetWorkshopSkinData(unit.workshopSkin);
                            GameObject original = XLRoot.UICustomAppearancePrefab;
                            uicharacter.unitModel = unit;
                            uicharacter.unitAppearance = UnityEngine.Object.Instantiate(original, __instance.characterRoot).GetComponent<CharacterAppearance>();
                            uicharacter.unitAppearance.transform.localPosition = new Vector3(num, -2f, 10f);
                            if (workshopSkinData != null)
                            {
                                uicharacter.unitAppearance.GetComponent<WorkshopSkinDataSetter>().SetData(workshopSkinData);
                                uicharacter.resName = workshopSkinData.dataName;
                                resName = workshopSkinData.dataName;
                                isCustom = true;
                            }
                            else
							{
                                Debug.Log("Workshop skin not found: " + unit.workshopSkin);
							}
                        }
                        else
                        {
                            if ((unit.CustomBookItem.IsWorkshop || Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData("") != null) && customBookItem.ClassInfo.skinType == "Custom")
                            {
                                string characterSkin = unit.CustomBookItem.ClassInfo.GetCharacterSkin();
                                WorkshopSkinData workshopBookSkinData = Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData(unit.CustomBookItem.BookId.packageId, characterSkin, s) ??
                                    Singleton<CustomizingResourceLoader>.Instance.GetWorkshopSkinData(characterSkin);
                                GameObject original = XLRoot.UICustomAppearancePrefab;
                                uicharacter.unitModel = unit;
                                uicharacter.unitAppearance = UnityEngine.Object.Instantiate(original, __instance.characterRoot).GetComponent<CharacterAppearance>();
                                uicharacter.unitAppearance.transform.localPosition = new Vector3(num, -2f, 10f);
                                WorkshopSkinDataSetter component = uicharacter.unitAppearance.GetComponent<WorkshopSkinDataSetter>();
                                if (workshopBookSkinData != null)
                                {
                                    component.SetData(workshopBookSkinData);
                                    uicharacter.resName = workshopBookSkinData.dataName;
                                    resName = workshopBookSkinData.dataName;
                                    isCustom = true;
                                }
                                else
								{
                                    Debug.Log("Book skin not found: " + new LorName(unit.CustomBookItem.BookId.packageId, characterSkin));
                                }
                            }
                            else
                            {
                                GameObject gameObject;
                                if (unit.gender == Gender.Creature)
                                {
                                    resName = characterName;
                                    gameObject = Singleton<AssetBundleManagerRemake>.Instance.LoadSdPrefab(resName);
                                }
                                else
                                {
                                    if (unit.gender == Gender.EGO)
                                    {
                                        resName = "[Prefab]" + characterName;
                                        gameObject = Singleton<AssetBundleManagerRemake>.Instance.LoadSdPrefab(resName);
                                    }
                                    else
                                    {
                                        gameObject = Singleton<AssetBundleManagerRemake>.Instance.LoadCharacterPrefab_DefaultMotion(characterName, s, out resName);
                                    }
                                }
                                if (gameObject != null)
                                {
                                    uicharacter.unitModel = unit;
                                    uicharacter.unitAppearance = UnityEngine.Object.Instantiate(gameObject, __instance.characterRoot).GetComponent<CharacterAppearance>();
                                    uicharacter.unitAppearance.transform.localPosition = new Vector3(num, -2f, 10f);
                                    uicharacter.resName = resName;
                                }
                                else
								{
                                    Debug.Log("Core skin not found: " + (string.IsNullOrWhiteSpace(resName) ? characterName : resName));
								}
                            }
                        }
                        if (uicharacter != null)
                        {
                            CharacterAppearance unitAppearance = uicharacter.unitAppearance;
                            if (unitAppearance != null)
                            {
                                unitAppearance.Initialize("");
                                unitAppearance.InitCustomData(unit.customizeData, unit.defaultBook.GetBookClassInfoId());
                                unitAppearance.InitGiftDataAll(unit.giftInventory.GetEquippedList());
                                unitAppearance.ChangeMotion(ActionDetail.Standing);
                                unitAppearance.ChangeLayer("CharacterAppearance_UI");
                            }
                        }
                        if (isCustom)
                        {
                            try
                            {
                                uicharacter.unitAppearance.GetComponent<WorkshopSkinDataSetter>().LateInit();
                            }
                            catch
                            {
                            }
                        }
                        if (unit != null && unit.EnemyUnitId != -1)
                        {
                            Transform transform = uicharacter?.unitAppearance.cameraPivot;
                            if (transform != null)
                            {
                                CharacterMotion characterMotion = uicharacter?.unitAppearance.GetCharacterMotion(ActionDetail.Standing);
                                if (characterMotion != null)
                                {
                                    Vector3 b = characterMotion.transform.position - transform.position;
                                    b.z = 0f;
                                    characterMotion.transform.localPosition += b;
                                }
                                else
                                {
                                    Debug.Log("Null charactermotion:     " + uicharacter.resName);
                                }
                            }
                        }
                        __instance.StartCoroutine(UtilTools.RenderCam_2(unit.textureIndex, __instance));
                    }
                    catch (Exception message)
                    {
                        Debug.LogError(message);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/UICR_SCerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //ProfileUI Unit Preview
        [HarmonyPatch(typeof(UICharacterRenderer), "GetRenderTextureByIndex")]
        [HarmonyPrefix]
        private static bool UICharacterRenderer_GetRenderTextureByIndex_Pre(UICharacterRenderer __instance, ref Texture __result, int index)
        {
            try
            {
                if (index < __instance.characterList.Count)
                {
                    UICharacter uicharacter = __instance.characterList[index];
                    if (uicharacter.unitAppearance != null)
                    {
                        uicharacter.unitAppearance.transform.localScale = Vector2.one;
                    }
                    __result = __instance.cameraList[index].targetTexture;
                    return false;
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/UICR_GTBIerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //ProfileUI Unit Preview
        [HarmonyPatch(typeof(UICharacterRenderer), "GetRenderTextureByIndexAndSize")]
        [HarmonyPrefix]
        private static bool UICharacterRenderer_GetRenderTextureByIndexAndSize_Pre(UICharacterRenderer __instance, ref Texture __result, int index)
        {
            try
            {
                Vector2 v = Vector2.one;
                UICharacter uicharacter = __instance.characterList[index];
                if (uicharacter.unitModel != null)
                {
                    float d = uicharacter.unitModel.customizeData.height;
                    if (uicharacter.unitAppearance != null)
                    {
                        v = Vector2.one * d * 0.005f;
                        uicharacter.unitAppearance.transform.localScale = v;
                    }
                    __result = __instance.cameraList[index].targetTexture;
                    return false;
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/UICR_GTBIASerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }/*
        //StageTurnPageButton
        [HarmonyPatch(typeof(GameOpeningController), "StopOpening")]
        [HarmonyPostfix]
        private static void GameOpeningController_StopOpening_Post()
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
        [HarmonyPatch(typeof(UIBattleSettingPanel), "OnOpen")]
        [HarmonyPrefix]
        private static void UIBattleSettingPanel_OnOpen_Pre()
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
        [HarmonyPatch(typeof(UIEnemyCharacterListPanel), "Activate")]
        [HarmonyPrefix]
        private static void UIEnemyCharacterListPanel_Activate_Pre(bool isTrue)
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
        [HarmonyPatch(typeof(UIEnemyCharacterListPanel), "SetEnemyWave")]
        [HarmonyPrefix]
        private static bool UIEnemyCharacterListPanel_SetEnemyWave_Pre(UIEnemyCharacterListPanel __instance, int targetWave, ref StageClassInfo ___currentEnemyStageinfo, UICharacterList ___CharacterList, ref int ___currentWave, GameObject ___ob_blueeffect)
        {
            try
            {
                if (___currentEnemyStageinfo == null)
                {
                    Debug.LogError("스테이지 인포 null");
                    ___CharacterList.InitEnemyList(null, false, UIStoryLine.None);
                    return false;
                }
                ___currentWave = targetWave;
                if (___ob_blueeffect.activeSelf)
                {
                    ___ob_blueeffect.gameObject.SetActive(false);
                }
                if (___currentEnemyStageinfo.invitationInfo.combine == StageCombineType.BookRecipe)
                {
                    if (___currentEnemyStageinfo.storyType == UIStoryLine.TheBlueReverberationPrimary.ToString() || ___currentEnemyStageinfo.storyType == UIStoryLine.BlackSilence.ToString() || ___currentEnemyStageinfo.storyType == UIStoryLine.TwistedBlue.ToString() || ___currentEnemyStageinfo.storyType == UIStoryLine.Final.ToString())
                    {
                        ___CharacterList.SetBlockRaycast(false);

                        List<UnitDataModel> list = GetEnemyUnitDataList(___currentEnemyStageinfo, ___currentWave);
                        if (list != null)
                        {
                            __instance.SetCharacterRenderer(list, true);
                            UIStoryLine story = (UIStoryLine)Enum.Parse(typeof(UIStoryLine), ___currentEnemyStageinfo.storyType);
                            ___CharacterList.InitEnemyList(list, false, story);
                            __instance.GetType().GetMethod("UpdateFrame", AccessTools.all).Invoke(__instance, new object[] { story });
                        }
                        if (___currentEnemyStageinfo.storyType == UIStoryLine.TheBlueReverberationPrimary.ToString() && !___ob_blueeffect.activeSelf)
                        {
                            ___ob_blueeffect.gameObject.SetActive(true);
                        }
                    }
                    else if (___currentEnemyStageinfo.currentState == StoryState.Clear)
                    {
                        ___CharacterList.SetBlockRaycast(false);
                        List<UnitDataModel> list2 = GetEnemyUnitDataList(___currentEnemyStageinfo, ___currentWave);
                        if (list2 != null)
                        {
                            __instance.SetCharacterRenderer(list2, true);
                            ___CharacterList.InitEnemyList(list2, false, UIStoryLine.None);
                            __instance.GetType().GetMethod("UpdateFrame", AccessTools.all).Invoke(__instance, new object[] { UIStoryLine.None });
                        }
                    }
                    else
                    {
                        ___CharacterList.SetBlockRaycast(true);
                        ___CharacterList.InitNotClearEnemyList();
                        __instance.GetType().GetMethod("UpdateFrame", AccessTools.all).Invoke(__instance, new object[] { UIStoryLine.None });
                    }
                }
                else if (LibraryModel.Instance.GetChapter() >= 2)
                {
                    if (___currentEnemyStageinfo.currentState == StoryState.Clear)
                    {
                        ___CharacterList.SetBlockRaycast(false);
                        List<UnitDataModel> list3 = GetEnemyUnitDataList(___currentEnemyStageinfo, ___currentWave);
                        if (list3 != null)
                        {
                            __instance.SetCharacterRenderer(list3, true);
                            ___CharacterList.InitEnemyList(list3, UIPanel.Controller.CurrentUIPhase == UIPhase.BattleSetting, UIStoryLine.None);
                            __instance.GetType().GetMethod("UpdateFrame", AccessTools.all).Invoke(__instance, new object[] { UIStoryLine.None });
                        }
                    }
                    else
                    {
                        ___CharacterList.SetBlockRaycast(true);
                        ___CharacterList.InitNotClearEnemyList();
                        __instance.GetType().GetMethod("UpdateFrame", AccessTools.all).Invoke(__instance, new object[] { UIStoryLine.None });
                    }
                }
                else
                {
                    ___CharacterList.InitNotClearEnemyList();
                    __instance.GetType().GetMethod("UpdateFrame", AccessTools.all).Invoke(__instance, new object[] { UIStoryLine.None });
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
        private static List<UnitDataModel> GetEnemyUnitDataList(StageClassInfo ___currentEnemyStageinfo, int ___currentWave)
        {
            if (EnemyListCache == null)
            {
                EnemyListCache = new Dictionary<LorId, List<List<UnitDataModel>>>();
            }
            if (!EnemyListCache.ContainsKey(___currentEnemyStageinfo.id))
            {
                EnemyListCache[___currentEnemyStageinfo.id] = new List<List<UnitDataModel>>();
            }
            while (EnemyListCache[___currentEnemyStageinfo.id].Count <= ___currentWave)
            {
                EnemyListCache[___currentEnemyStageinfo.id].Add(new List<UnitDataModel>());
            }
            if (EnemyListCache[___currentEnemyStageinfo.id][___currentWave].Count <= 0)
            {
                foreach (LorId id in ___currentEnemyStageinfo.waveList[___currentWave].enemyUnitIdList)
                {
                    EnemyUnitClassInfo data = Singleton<EnemyUnitClassInfoList>.Instance.GetData(id);
                    int id2 = data.bookId[RandomUtil.SystemRange(data.bookId.Count)];
                    UnitDataModel unitDataModel = new UnitDataModel(new LorId(data.workshopID, id2), SephirahType.None, false);
                    unitDataModel.SetByEnemyUnitClassInfo(data);
                    EnemyListCache[___currentEnemyStageinfo.id][___currentWave].Add(unitDataModel);
                }
            }
            return EnemyListCache[___currentEnemyStageinfo.id][___currentWave];
        }
        //StageTurnPageButton
        [HarmonyPatch(typeof(UILibrarianCharacterListPanel), "OnSetSephirah")]
        [HarmonyPrefix]
        private static void UILibrarianCharacterListPanel_OnSetSephirah_Pre(SephirahType targetSephirah)
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
        //StageTurnPageButton
        [HarmonyPatch(typeof(UICharacterList), "InitEnemyList")]
        [HarmonyPrefix]
        private static void UICharacterList_InitEnemyList_Pre(ref List<UnitDataModel> unitList)
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
                File.WriteAllText(Application.dataPath + "/Mods/StageTurnPage5.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //StageTurnPageButton
        [HarmonyPatch(typeof(UICharacterList), "InitLibrarianList")]
        [HarmonyPrefix]
        private static bool UICharacterList_InitLibrarianList_Pre(UICharacterList __instance, List<UnitDataModel> unitList, SephirahType sephirah, bool Selectable, Image ___highlightFrame)
        {
            if (unitList.Count <= 5)
            {
                return true;
            }
            try
            {
                __instance.isSelectableList = Selectable;
                ___highlightFrame.enabled = __instance.isSelectableList;
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
                }*/
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/StageTurnPage6.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }

        //StageTurnPageButton
        [HarmonyPatch(typeof(UICharacterList), "InitUnitListFromBattleData")]
        [HarmonyPrefix]
        private static bool UICharacterList_InitUnitListFromBattleData_Pre(UICharacterList __instance, List<UnitBattleDataModel> dataList, Image ___highlightFrame)
        {
            if (dataList.Count <= 5)
            {
                return true;
            }
            try
            {
                List<UnitBattleDataModel> newdatalist = new List<UnitBattleDataModel>()
                {
                    dataList[0],
                    dataList[1],
                    dataList[2],
                    dataList[3],
                    dataList[4],
                };
                UIBattleSettingPanel uibattleSettingPanel = UI.UIController.Instance.GetUIPanel(UIPanelType.BattleSetting) as UIBattleSettingPanel;
                uibattleSettingPanel.currentAvailbleUnitslots.Clear();
                __instance.isSelectableList = true;
                ___highlightFrame.enabled = __instance.isSelectableList;
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
                }*/
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/StageTurnPage7.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //StageTurnPageButton
        [HarmonyPatch(typeof(UICharacterList), "InitBattleEnemyList")]
        [HarmonyPrefix]
        private static void UICharacterList_InitBattleEnemyList_Pre(ref List<UnitBattleDataModel> unitList)
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
        }
        //StageTurnPageButton
        [HarmonyPatch(typeof(UIBattleSettingPanel), "SetToggles")]
        [HarmonyPrefix]
        private static bool UIBattleSettingPanel_SetToggles_Pre(UIBattleSettingPanel __instance, TextMeshProUGUI ___txt_AvailableUnitNumberText, Animator ___anim_availableText)
        {
            List<UnitBattleDataModel> battleDataModels = Singleton<StageController>.Instance.GetCurrentStageFloorModel().GetUnitBattleDataList();
            if (battleDataModels.Count <= 5 || battleDataModels.Count <= Singleton<StageController>.Instance.GetCurrentWaveModel().AvailableUnitNumber)
            {
                return true;
            }
            try
            {
                if (!StageButtonTool.IsTurningPage)
                {
                    foreach (UnitBattleDataModel unitBattleData in battleDataModels)
                    {
                        unitBattleData.IsAddedBattle = false;
                    }
                    for (int i = 0; i < Singleton<StageController>.Instance.GetCurrentWaveModel().AvailableUnitNumber; i++)
                    {
                        battleDataModels[i].IsAddedBattle = true;
                    }
                }
                StageButtonTool.IsTurningPage = false;
                foreach (UICharacterSlot uicharacterSlot in __instance.currentAvailbleUnitslots)
                {
                    if (uicharacterSlot._unitData.IsLockUnit())
                    {
                        uicharacterSlot.SetToggle(false);
                    }
                    else
                    {
                        uicharacterSlot.SetToggle(true);
                        if (uicharacterSlot.unitBattleData.IsAddedBattle)
                        {
                            uicharacterSlot.SetYesToggleState();
                        }
                        else
                        {
                            uicharacterSlot.SetNoToggleState();
                        }
                    }
                }
                if (Singleton<StageController>.Instance.GetStageModel() != null && Singleton<StageController>.Instance.GetStageModel().ClassInfo.stageType == StageType.Creature)
                {
                    foreach (UICharacterSlot uicharacterSlot2 in __instance.currentAvailbleUnitslots)
                    {
                        uicharacterSlot2.SetToggle(false);
                    }
                }
                SetAvailibleText(battleDataModels, ___txt_AvailableUnitNumberText, ___anim_availableText);
                return false;
            }
            catch (Exception ex)
            {
                foreach (UnitBattleDataModel unitBattleData in battleDataModels)
                {
                    unitBattleData.IsAddedBattle = true;
                }
                File.WriteAllText(Application.dataPath + "/Mods/StageTurnPage10.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //StageTurnPageButton
        [HarmonyPatch(typeof(UIBattleSettingPanel), "SelectedToggles")]
        [HarmonyPrefix]
        private static bool UIBattleSettingPanel_SelectedToggles_Pre(UIBattleSettingPanel __instance, UICharacterSlot slot, TextMeshProUGUI ___txt_AvailableUnitNumberText, Animator ___anim_availableText)
        {
            List<UnitBattleDataModel> battleDataModels = Singleton<StageController>.Instance.GetCurrentStageFloorModel().GetUnitBattleDataList();
            if (battleDataModels.Count <= 5)
            {
                return true;
            }
            try
            {
                if (Singleton<StageController>.Instance.GetCurrentWaveModel().AvailableUnitNumber == 1)
                {
                    foreach (UICharacterSlot uicharacterSlot in __instance.currentAvailbleUnitslots)
                    {
                        uicharacterSlot.SetNoToggleState();
                    }
                    foreach (UnitBattleDataModel unitBattleData in battleDataModels)
                    {
                        unitBattleData.IsAddedBattle = false;
                    }
                    slot.SetYesToggleState();
                    UISoundManager.instance.PlayEffectSound(UISoundType.Ui_Click);
                }
                else if (slot.IsToggleSelected)
                {
                    if (GetAvailableOne(battleDataModels))
                    {
                        slot.StartVibe();
                        UISoundManager.instance.PlayEffectSound(UISoundType.Card_Lock);
                        return false;
                    }
                    slot.SetNoToggleState();
                }
                else
                {
                    if (GetAvailableMaxState(battleDataModels))
                    {
                        slot.StartVibe();
                        UISoundManager.instance.PlayEffectSound(UISoundType.Card_Lock);
                        return false;
                    }
                    slot.SetYesToggleState();
                    UISoundManager.instance.PlayEffectSound(UISoundType.Ui_Click);
                }
                SetAvailibleText(battleDataModels, ___txt_AvailableUnitNumberText, ___anim_availableText);
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/StageTurnPage11.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        private static bool GetAvailableOne(List<UnitBattleDataModel> battleDataModels)
        {
            return battleDataModels.FindAll((UnitBattleDataModel x) => x.IsAddedBattle).Count == 1;
        }
        private static bool GetAvailableMaxState(List<UnitBattleDataModel> battleDataModels)
        {
            return battleDataModels.FindAll((UnitBattleDataModel x) => x.IsAddedBattle).Count >= Singleton<StageController>.Instance.GetCurrentWaveModel().AvailableUnitNumber;
        }
        private static void SetAvailibleText(List<UnitBattleDataModel> battleDataModels, TextMeshProUGUI ___txt_AvailableUnitNumberText, Animator ___anim_availableText)
        {
            string text = TextDataModel.GetText("ui_battlesetting_selectedunitnumber", Array.Empty<object>());
            ___txt_AvailableUnitNumberText.text = string.Concat(new object[]
            {
                text,
                " ",
                battleDataModels.FindAll((UnitBattleDataModel x) => x.IsAddedBattle).Count,
                "/",
                Singleton<StageController>.Instance.GetCurrentWaveModel().AvailableUnitNumber
            });
            ___anim_availableText.SetTrigger("Reveal");
        }
        //StageTurnPageButton
        [HarmonyPatch(typeof(BattleEmotionRewardInfoUI), "SetData")]
        [HarmonyPrefix]
        private static bool BattleEmotionRewardInfoUI_SetData_Pre(List<UnitBattleDataModel> units, Faction faction, List<BattleEmotionRewardSlotUI> ___slots)
        {
            try
            {
                while (units.Count > ___slots.Count && ___slots.Count < 9)
                {
                    BattleEmotionRewardSlotUI newUI = UnityEngine.Object.Instantiate(___slots[0]);
                    ___slots.Add(newUI);
                }
                foreach (BattleEmotionRewardSlotUI battleEmotionRewardSlotUI in ___slots)
                {
                    battleEmotionRewardSlotUI.gameObject.SetActive(false);
                }
                for (int i = 0; i < units.Count; i++)
                {
                    if (i > 8)
                    {
                        break;
                    }
                    ___slots[i].gameObject.SetActive(true);
                    ___slots[i].SetData(units[i], faction);
                }
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/EmotionRewardInfoUI_SetData.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        private static void AddFormationPosition(FormationModel Formation)
        {
            List<FormationPosition> _postionList = (List<FormationPosition>)Formation.GetType().GetField("_postionList", AccessTools.all).GetValue(Formation);
            for (int i = _postionList.Count; i < 99; i++)
            {
                FormationPositionXmlData data = new FormationPositionXmlData()
                {
                    name = "E" + i.ToString(),
                    vector = new XmlVector2()
                    {
                        x = GetVector2X(i - 4),
                        y = GetVector2Y(i - 4),
                    },
                    eventList = null
                };
                FormationPosition formationPosition = new FormationPosition(data)
                {
                    eventList = new List<FormationPositionEvent>(),
                    index = i
                };
                _postionList.Add(formationPosition);
            }
        }
        private static int GetVector2X(int i)
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
        private static int GetVector2Y(int i)
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
        private static void AddFormationPositionForEnemy(FormationModel Formation)
        {
            List<FormationPosition> _postionList = (List<FormationPosition>)Formation.GetType().GetField("_postionList", AccessTools.all).GetValue(Formation);
            int x = -23;
            int y = 18;
            for (int i = _postionList.Count; i < 99; i++)
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
                    StageModel _stageModel = (StageModel)typeof(StageController).GetField("_stageModel", AccessTools.all).GetValue(Singleton<StageController>.Instance);
                    BattleTeamModel _enemyTeam = (BattleTeamModel)typeof(StageController).GetField("_enemyTeam", AccessTools.all).GetValue(Singleton<StageController>.Instance);

                    UnitBattleDataModel EnemyUnitBattleDataModel = UnitBattleDataModel.CreateUnitBattleDataByEnemyUnitId(_stageModel, EnemyUnitID);
                    BattleObjectManager.instance.UnregisterUnitByIndex(Faction, Index);

                    StageWaveModel currentWaveModel = Singleton<StageController>.Instance.GetCurrentWaveModel();
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

                    StageLibraryFloorModel currentStageFloorModel = Singleton<StageController>.Instance.GetCurrentStageFloorModel();
                    BattleTeamModel _librarianTeam = (BattleTeamModel)typeof(StageController).GetField("_librarianTeam", AccessTools.all).GetValue(Singleton<StageController>.Instance);

                    UnitDataModel unitDataModel = new UnitDataModel(BookID, currentStageFloorModel.Sephirah, false);
                    UnitBattleDataModel unitBattleDataModel = new UnitBattleDataModel(Singleton<StageController>.Instance.GetStageModel(), unitDataModel);
                    BattleObjectManager.instance.UnregisterUnitByIndex(Faction, Index);

                    if (Singleton<EnemyUnitClassInfoList>.Instance.GetData(EnemyUnitID) != null)
                    {
                        unitDataModel.SetByEnemyUnitClassInfo(Singleton<EnemyUnitClassInfoList>.Instance.GetData(EnemyUnitID));
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
                    unitDataModel.GetType().GetField("_enemyUnitId", AccessTools.all).SetValue(unitDataModel, EnemyUnitID);
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
                if (Singleton<StageController>.Instance.Phase <= (StageController.StagePhase)5)
                {
                    battleUnitModel.OnRoundStartOnlyUI();
                    battleUnitModel.RollSpeedDice();
                }
                SingletonBehavior<BattleManagerUI>.Instance.ui_unitListInfoSummary.UpdateCharacterProfileAll();
                int num2 = 0;
                foreach (BattleUnitModel battleUnitModel2 in BattleObjectManager.instance.GetList())
                {
                    SingletonBehavior<UICharacterRenderer>.Instance.SetCharacter(battleUnitModel2.UnitData.unitData, num2++, true, false);
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
                    StageModel _stageModel = (StageModel)typeof(StageController).GetField("_stageModel", AccessTools.all).GetValue(Singleton<StageController>.Instance);
                    BattleTeamModel _enemyTeam = (BattleTeamModel)typeof(StageController).GetField("_enemyTeam", AccessTools.all).GetValue(Singleton<StageController>.Instance);


                    BattleObjectManager.instance.UnregisterUnitByIndex(Faction, Index);
                    StageWaveModel currentWaveModel = Singleton<StageController>.Instance.GetCurrentWaveModel();
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

                    StageLibraryFloorModel currentStageFloorModel = Singleton<StageController>.Instance.GetCurrentStageFloorModel();
                    BattleTeamModel _librarianTeam = (BattleTeamModel)typeof(StageController).GetField("_librarianTeam", AccessTools.all).GetValue(Singleton<StageController>.Instance);

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
                    unitBattleData.unitData.GetType().GetField("_enemyUnitId", AccessTools.all).SetValue(unitBattleData.unitData, EnemyUnitID);
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
                if (Singleton<StageController>.Instance.Phase <= (StageController.StagePhase)5)
                {
                    battleUnitModel.OnRoundStartOnlyUI();
                    battleUnitModel.RollSpeedDice();
                }
                SingletonBehavior<BattleManagerUI>.Instance.ui_unitListInfoSummary.UpdateCharacterProfileAll();
                int num2 = 0;
                foreach (BattleUnitModel battleUnitModel2 in BattleObjectManager.instance.GetList())
                {
                    SingletonBehavior<UICharacterRenderer>.Instance.SetCharacter(battleUnitModel2.UnitData.unitData, num2++, true, false);
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
                    StageModel _stageModel = (StageModel)typeof(StageController).GetField("_stageModel", AccessTools.all).GetValue(Singleton<StageController>.Instance);
                    BattleTeamModel _enemyTeam = (BattleTeamModel)typeof(StageController).GetField("_enemyTeam", AccessTools.all).GetValue(Singleton<StageController>.Instance);

                    UnitBattleDataModel EnemyUnitBattleDataModel = new UnitBattleDataModel(_stageModel, unitData);
                    EnemyUnitBattleDataModel.Init();

                    BattleObjectManager.instance.UnregisterUnitByIndex(Faction, Index);

                    StageWaveModel currentWaveModel = Singleton<StageController>.Instance.GetCurrentWaveModel();

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
                    StageLibraryFloorModel currentStageFloorModel = Singleton<StageController>.Instance.GetCurrentStageFloorModel();
                    BattleTeamModel _librarianTeam = (BattleTeamModel)typeof(StageController).GetField("_librarianTeam", AccessTools.all).GetValue(Singleton<StageController>.Instance);

                    UnitBattleDataModel unitBattleDataModel = new UnitBattleDataModel(Singleton<StageController>.Instance.GetStageModel(), unitData);
                    BattleObjectManager.instance.UnregisterUnitByIndex(Faction, Index);

                    if (Singleton<EnemyUnitClassInfoList>.Instance.GetData(EnemyUnitID) != null)
                    {
                        unitData.SetByEnemyUnitClassInfo(Singleton<EnemyUnitClassInfoList>.Instance.GetData(EnemyUnitID));
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
                    unitData.GetType().GetField("_enemyUnitId", AccessTools.all).SetValue(unitData, EnemyUnitID);
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
                if (Singleton<StageController>.Instance.Phase <= (StageController.StagePhase)5)
                {
                    battleUnitModel.OnRoundStartOnlyUI();
                    battleUnitModel.RollSpeedDice();
                }
                SingletonBehavior<BattleManagerUI>.Instance.ui_unitListInfoSummary.UpdateCharacterProfileAll();
                int num2 = 0;
                foreach (BattleUnitModel battleUnitModel2 in BattleObjectManager.instance.GetList())
                {
                    SingletonBehavior<UICharacterRenderer>.Instance.SetCharacter(battleUnitModel2.UnitData.unitData, num2++, true, false);
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

        private static List<BattleCharacterProfileUI> enemyProfileArray2;

        private static List<BattleCharacterProfileUI> allyProfileArray2;

        private static List<BattleEmotionCoinUI.BattleEmotionCoinData> enermy2;

        private static List<BattleEmotionCoinUI.BattleEmotionCoinData> librarian2;

        private static Dictionary<LorId, List<List<UnitDataModel>>> EnemyListCache;

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