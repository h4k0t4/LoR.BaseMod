using HarmonyLib;
using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SummonLiberation
{
    public class ButtonColor : EventTrigger
    {
        private void Update()
        {
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            Image.color = OnEnterColor;
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            Image.color = DefaultColor;
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            Image.color = DefaultColor;
        }

        public Color DefaultColor = new Color(1f, 1f, 1f);

        public static Color OnEnterColor;

        public Image Image;
    }
}


namespace SummonLiberation
{
    public static class StageButtonTool
    {
        static StageButtonTool()
        {
            Sprite sprite = BaseMod.UIPanelTool.GetEnemyCharacterListPanel().transform.GetChild(0).GetChild(3).GetChild(1).GetChild(0).GetComponent<Image>().sprite;
            ButtonColor.OnEnterColor = new Color(0.13333334f, 1f, 0.89411765f);
            EnemyUP = BaseMod.UtilTools.CreateButton(BaseMod.UIPanelTool.GetEnemyCharacterListPanel().transform.GetChild(0).transform, sprite, new Vector2(0.07f, 0.07f), new Vector2(550f, -105f));
            EnemyUP.name = "[Button]up";
            EnemyUP.image.color = new Color(1f, 1f, 1f);
            EnemyUP.transform.rotation = new Quaternion(0f, 0f, 180f, 0f);
            ButtonColor buttonColor = EnemyUP.gameObject.AddComponent<ButtonColor>();
            buttonColor.Image = EnemyUP.image;
            LibrarianUP = BaseMod.UtilTools.CreateButton(BaseMod.UIPanelTool.GetLibrarianCharacterListPanel().transform.GetChild(0).transform, sprite, new Vector2(0.07f, 0.07f), new Vector2(-550f, -95f));
            LibrarianUP.name = "[Button]up";
            LibrarianUP.image.color = new Color(1f, 1f, 1f);
            LibrarianUP.transform.rotation = new Quaternion(0f, 0f, 180f, 0f);
            ButtonColor buttonColor2 = LibrarianUP.gameObject.AddComponent<ButtonColor>();
            buttonColor2.Image = LibrarianUP.image;
            EnemyDown = BaseMod.UtilTools.CreateButton(BaseMod.UIPanelTool.GetEnemyCharacterListPanel().transform.GetChild(0).transform, sprite, new Vector2(0.07f, 0.07f), new Vector2(550f, -270f));
            EnemyDown.name = "[Button]down";
            EnemyDown.image.color = new Color(1f, 1f, 1f);
            ButtonColor buttonColor3 = EnemyDown.gameObject.AddComponent<ButtonColor>();
            buttonColor3.Image = EnemyDown.image;
            LibrarianDown = BaseMod.UtilTools.CreateButton(BaseMod.UIPanelTool.GetLibrarianCharacterListPanel().transform.GetChild(0).transform, sprite, new Vector2(0.07f, 0.07f), new Vector2(-550f, -260f));
            LibrarianDown.name = "[Button]down";
            LibrarianDown.image.color = new Color(1f, 1f, 1f);
            ButtonColor buttonColor4 = LibrarianDown.gameObject.AddComponent<ButtonColor>();
            buttonColor4.Image = LibrarianDown.image;
            Init();
            EnemyUP.gameObject.SetActive(false);
            EnemyDown.gameObject.SetActive(false);
            LibrarianUP.gameObject.SetActive(false);
            LibrarianDown.gameObject.SetActive(false);
        }
        public static void Init()
        {
            EnemyUP.onClick.AddListener(new UnityAction(OnClickEnemyUP));
            EnemyDown.onClick.AddListener(new UnityAction(OnClickEnemyDown));
            LibrarianUP.onClick.AddListener(new UnityAction(OnClickLibrarianUP));
            LibrarianDown.onClick.AddListener(new UnityAction(OnClickLibrarianDown));
        }
        public static void RefreshEnemy()
        {
            if (enemyCharacterList == null)
            {
                enemyCharacterList = (UICharacterList)typeof(UICharacterListPanel).GetField("CharacterList", AccessTools.all).GetValue(BaseMod.UIPanelTool.GetEnemyCharacterListPanel());
            }
            /*currentEnemyStageinfo = (StageClassInfo)BaseMod.UIPanelTool.GetEnemyCharacterListPanel().GetType().GetField("currentEnemyStageinfo", AccessTools.all).GetValue(BaseMod.UIPanelTool.GetEnemyCharacterListPanel());
            currentWave = (int)BaseMod.UIPanelTool.GetEnemyCharacterListPanel().GetType().GetField("currentWave", AccessTools.all).GetValue(BaseMod.UIPanelTool.GetEnemyCharacterListPanel());*/
            currentEnemyUnitIndex = 0;
            Color color = UIColorManager.Manager.EnemyUIColor;
            if (Enum.IsDefined(typeof(UIStoryLine), Singleton<StageController>.Instance.GetStageModel().ClassInfo.storyType))
            {
                UIStoryLine story = (UIStoryLine)Enum.Parse(typeof(UIStoryLine), Singleton<StageController>.Instance.GetStageModel().ClassInfo.storyType);
                switch (story)
                {
                    case UIStoryLine.TheBlueReverberationPrimary:
                        color = UIColorManager.Manager.BlueEffectContentColor;
                        break;
                    case UIStoryLine.BlackSilence:
                        color = UIColorManager.Manager.BlackSilenceEffectColor[0];
                        color.a = 0.5f;
                        break;
                    case UIStoryLine.TwistedBlue:
                        color = UIColorManager.Manager.BlueEffectContentColor;
                        break;
                    case UIStoryLine.Final:
                        color = UIColorManager.Manager.AnotherEtcEffectColor[0];
                        break;
                    default:
                        break;

                }
            }
            EnemyUP.gameObject.GetComponent<ButtonColor>().Image.color = color;
            EnemyDown.gameObject.GetComponent<ButtonColor>().Image.color = color;
            EnemyUP.gameObject.GetComponent<ButtonColor>().DefaultColor = color;
            EnemyDown.gameObject.GetComponent<ButtonColor>().DefaultColor = color;
            EnemyUP.gameObject.SetActive(false);
            EnemyDown.gameObject.SetActive(false);
            if (UIPanel.Controller.CurrentUIPhase == UIPhase.BattleSetting)
            {
                if (Singleton<StageController>.Instance.GetStageModel() != null && Singleton<StageController>.Instance.GetCurrentWaveModel().UnitList.Count > 5)
                {
                    EnemyDown.gameObject.SetActive(true);
                }
                else
                {
                    EnemyDown.gameObject.SetActive(false);
                }
            }
        }
        public static void RefreshLibrarian()
        {
            if (librarianCharacterList == null)
            {
                librarianCharacterList = (UICharacterList)typeof(UICharacterListPanel).GetField("CharacterList", AccessTools.all).GetValue(BaseMod.UIPanelTool.GetLibrarianCharacterListPanel());
            }
            Color color = UIColorManager.Manager.GetSephirahColor(UIPanel.Controller.CurrentSephirah);
            LibrarianUP.gameObject.GetComponent<ButtonColor>().Image.color = color;
            LibrarianDown.gameObject.GetComponent<ButtonColor>().Image.color = color;
            LibrarianUP.gameObject.GetComponent<ButtonColor>().DefaultColor = color;
            LibrarianDown.gameObject.GetComponent<ButtonColor>().DefaultColor = color;
            currentLibrarianUnitIndex = 0;
            LibrarianUP.gameObject.SetActive(false);
            LibrarianDown.gameObject.SetActive(false);
            if (UIPanel.Controller.CurrentUIPhase == UIPhase.BattleSetting)
            {
                if (Singleton<StageController>.Instance.GetCurrentStageFloorModel() != null && Singleton<StageController>.Instance.GetCurrentStageFloorModel().GetUnitBattleDataList().Count > 5)
                {
                    LibrarianDown.gameObject.SetActive(true);
                }
                else
                {
                    LibrarianDown.gameObject.SetActive(false);
                }
            }
            IsTurningPage = false;
        }
        public static void OnClickEnemyUP()
        {
            if (currentEnemyUnitIndex < 5)
            {
                EnemyUP.gameObject.SetActive(false);
                return;
            }
            try
            {
                currentEnemyUnitIndex -= 5;
                if (currentEnemyUnitIndex <= 0)
                {
                    currentEnemyUnitIndex = 0;
                    EnemyUP.gameObject.SetActive(false);
                }
                UpdateEnemyCharacterList();
                if (Singleton<StageController>.Instance.GetStageModel() != null && Singleton<StageController>.Instance.GetCurrentWaveModel().UnitList.Count - currentEnemyUnitIndex > 5)
                {
                    EnemyDown.gameObject.SetActive(true);
                }
                else
                {
                    EnemyDown.gameObject.SetActive(false);
                }
            }
            catch { }
        }
        public static void OnClickEnemyDown()
        {
            if (Singleton<StageController>.Instance.GetStageModel() != null && Singleton<StageController>.Instance.GetCurrentWaveModel().UnitList.Count - currentEnemyUnitIndex <= 5)
            {
                EnemyDown.gameObject.SetActive(false);
                return;
            }
            try
            {
                currentEnemyUnitIndex += 5;
                EnemyUP.gameObject.SetActive(true);
                UpdateEnemyCharacterList();
                if (Singleton<StageController>.Instance.GetStageModel() != null && Singleton<StageController>.Instance.GetCurrentWaveModel().UnitList.Count - currentEnemyUnitIndex <= 5)
                {
                    EnemyDown.gameObject.SetActive(false);
                }
                else
                {
                    EnemyDown.gameObject.SetActive(true);
                }
            }
            catch { }
        }
        public static void OnClickLibrarianUP()
        {
            if (currentLibrarianUnitIndex == 0)
            {
                LibrarianUP.gameObject.SetActive(false);
                return;
            }
            try
            {
                currentLibrarianUnitIndex -= 5;
                if (currentLibrarianUnitIndex <= 0)
                {
                    currentLibrarianUnitIndex = 0;
                    LibrarianUP.gameObject.SetActive(false);
                }
                UpdateLibrarianCharacterList();
                if (Singleton<StageController>.Instance.GetCurrentStageFloorModel() != null && Singleton<StageController>.Instance.GetCurrentStageFloorModel().GetUnitBattleDataList().Count - currentLibrarianUnitIndex > 5)
                {
                    LibrarianDown.gameObject.SetActive(true);
                }
                else
                {
                    LibrarianDown.gameObject.SetActive(false);
                }
            }
            catch { }
        }

        public static void OnClickLibrarianDown()
        {
            try
            {
                if (Singleton<StageController>.Instance.GetCurrentStageFloorModel() != null && Singleton<StageController>.Instance.GetCurrentStageFloorModel().GetUnitBattleDataList().Count - currentLibrarianUnitIndex <= 5)
                {
                    LibrarianDown.gameObject.SetActive(false);
                    return;
                }
                try
                {
                    currentLibrarianUnitIndex += 5;
                    LibrarianUP.gameObject.SetActive(true);
                    UpdateLibrarianCharacterList();
                    if (Singleton<StageController>.Instance.GetCurrentStageFloorModel() != null && Singleton<StageController>.Instance.GetCurrentStageFloorModel().GetUnitBattleDataList().Count - currentLibrarianUnitIndex <= 5)
                    {
                        LibrarianDown.gameObject.SetActive(false);
                    }
                    else
                    {
                        LibrarianDown.gameObject.SetActive(true);
                    }
                }
                catch { }
            }
            catch { }
        }
        public static void UpdateEnemyCharacterList()
        {
            if (UIPanel.Controller.CurrentUIPhase == UIPhase.BattleSetting)
            {
                List<UnitBattleDataModel> list = new List<UnitBattleDataModel>();
                for (int i = 0; i < 5; i++)
                {
                    if (Singleton<StageController>.Instance.GetCurrentWaveModel().UnitList.Count <= (currentEnemyUnitIndex + i))
                    {
                        break;
                    }
                    list.Add(Singleton<StageController>.Instance.GetCurrentWaveModel().UnitList[currentEnemyUnitIndex + i]);
                }
                if (list != null)
                {
                    BaseMod.UIPanelTool.GetEnemyCharacterListPanel().SetCharacterRenderer(list, true);
                    enemyCharacterList.InitBattleEnemyList(list);
                    BaseMod.UIPanelTool.GetEnemyCharacterListPanel().GetType().GetMethod("UpdateFrame", AccessTools.all).Invoke(BaseMod.UIPanelTool.GetEnemyCharacterListPanel(), new object[] { UIStoryLine.None });
                }
            }
        }
        public static void UpdateLibrarianCharacterList()
        {
            StageLibraryFloorModel currentStageFloorModel = Singleton<StageController>.Instance.GetCurrentStageFloorModel();
            if (currentStageFloorModel == null)
            {
                return;
            }
            if (UIPanel.Controller.CurrentUIPhase == UIPhase.BattleSetting)
            {

                List<UnitBattleDataModel> list = new List<UnitBattleDataModel>();
                for (int i = 0; i < 5; i++)
                {
                    if (currentStageFloorModel.GetUnitBattleDataList().Count <= currentLibrarianUnitIndex + i)
                    {
                        break;
                    }
                    list.Add(currentStageFloorModel.GetUnitBattleDataList()[currentLibrarianUnitIndex + i]);
                }
                if (list != null)
                {
                    BaseMod.UIPanelTool.GetLibrarianCharacterListPanel().SetCharacterRenderer(list, false);
                    IsTurningPage = true;
                    librarianCharacterList.InitUnitListFromBattleData(list);
                    BaseMod.UIPanelTool.GetLibrarianCharacterListPanel().GetType().GetMethod("UpdateFrameToSephirahInBattle", AccessTools.all).Invoke(BaseMod.UIPanelTool.GetLibrarianCharacterListPanel(), new object[] { currentStageFloorModel.Sephirah });
                }
            }
        }

        public static bool IsTurningPage;

        public static UICharacterList enemyCharacterList;

        public static UICharacterList librarianCharacterList;

        public static int currentEnemyUnitIndex = 0;

        public static int currentLibrarianUnitIndex = 0;

        public static Button EnemyUP;

        public static Button EnemyDown;

        public static Button LibrarianUP;

        public static Button LibrarianDown;
    }
}
