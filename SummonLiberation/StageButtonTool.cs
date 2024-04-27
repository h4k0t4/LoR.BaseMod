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
		void Update()
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
			currentEnemyUnitIndex = 0;
			currentLibrarianUnitIndex = 0;
			var enemyPanelRoot = BaseMod.UIPanelTool.GetEnemyCharacterListPanel().transform.Find("PanelActiveController");
			var librarianPanelRoot = BaseMod.UIPanelTool.GetLibrarianCharacterListPanel().transform.Find("PanelActiveController");
			Sprite sprite = enemyPanelRoot.Find("[Rect]StageEnemyList/[Button]Left/Image").GetComponent<Image>().sprite;
			ButtonColor.OnEnterColor = new Color(0.13333334f, 1f, 0.89411765f);
			EnemyUP = BaseMod.UtilTools.CreateButton(enemyPanelRoot, sprite, new Vector2(0.07f, 0.07f), new Vector2(550f, -105f));
			EnemyUP.name = "[Button]up";
			EnemyUP.image.color = new Color(1f, 1f, 1f);
			EnemyUP.transform.rotation = new Quaternion(0f, 0f, 180f, 0f);
			EnemyUP.gameObject.AddComponent<ButtonColor>().Image = EnemyUP.image;
			LibrarianUP = BaseMod.UtilTools.CreateButton(librarianPanelRoot, sprite, new Vector2(0.07f, 0.07f), new Vector2(-550f, -95f));
			LibrarianUP.name = "[Button]up";
			LibrarianUP.image.color = new Color(1f, 1f, 1f);
			LibrarianUP.transform.rotation = new Quaternion(0f, 0f, 180f, 0f);
			LibrarianUP.gameObject.AddComponent<ButtonColor>().Image = LibrarianUP.image;
			EnemyDown = BaseMod.UtilTools.CreateButton(enemyPanelRoot, sprite, new Vector2(0.07f, 0.07f), new Vector2(550f, -270f));
			EnemyDown.name = "[Button]down";
			EnemyDown.image.color = new Color(1f, 1f, 1f);
			EnemyDown.gameObject.AddComponent<ButtonColor>().Image = EnemyDown.image;
			LibrarianDown = BaseMod.UtilTools.CreateButton(librarianPanelRoot, sprite, new Vector2(0.07f, 0.07f), new Vector2(-550f, -260f));
			LibrarianDown.name = "[Button]down";
			LibrarianDown.image.color = new Color(1f, 1f, 1f);
			LibrarianDown.gameObject.AddComponent<ButtonColor>().Image = LibrarianDown.image;
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
				enemyCharacterList = BaseMod.UIPanelTool.GetEnemyCharacterListPanel().CharacterList;
			}
			/*currentEnemyStageinfo = BaseMod.UIPanelTool.GetEnemyCharacterListPanel().currentEnemyStageinfo;
			currentWave = BaseMod.UIPanelTool.GetEnemyCharacterListPanel().currentWave;*/
			currentEnemyUnitIndex = 0;
			Debug.Log(enemyCharacterList.slotList.Count);
			Color color = enemyCharacterList.slotList[0].originalColor;
			EnemyUP.gameObject.GetComponent<ButtonColor>().Image.color = color;
			EnemyDown.gameObject.GetComponent<ButtonColor>().Image.color = color;
			EnemyUP.gameObject.GetComponent<ButtonColor>().DefaultColor = color;
			EnemyDown.gameObject.GetComponent<ButtonColor>().DefaultColor = color;
			EnemyUP.gameObject.SetActive(false);
			if (UIPanel.Controller.CurrentUIPhase == UIPhase.BattleSetting)
			{
				if (StageController.Instance.GetStageModel() != null && StageController.Instance.GetCurrentWaveModel().UnitList.Count > 5)
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
				librarianCharacterList = BaseMod.UIPanelTool.GetLibrarianCharacterListPanel().CharacterList;
			}
			currentLibrarianUnitIndex = 0;
			Color color = librarianCharacterList.slotList[0].originalColor;
			LibrarianUP.gameObject.GetComponent<ButtonColor>().Image.color = color;
			LibrarianDown.gameObject.GetComponent<ButtonColor>().Image.color = color;
			LibrarianUP.gameObject.GetComponent<ButtonColor>().DefaultColor = color;
			LibrarianDown.gameObject.GetComponent<ButtonColor>().DefaultColor = color;
			LibrarianUP.gameObject.SetActive(false);
			if (UIPanel.Controller.CurrentUIPhase == UIPhase.BattleSetting)
			{
				if (StageController.Instance.GetCurrentStageFloorModel() != null && StageController.Instance.GetCurrentStageFloorModel().GetUnitBattleDataList().Count > 5)
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
			try
			{
				if (currentEnemyUnitIndex <= 0)
				{
					EnemyUP.gameObject.SetActive(false);
					return;
				}
				currentEnemyUnitIndex -= 5;
				if (currentEnemyUnitIndex <= 0)
				{
					currentEnemyUnitIndex = 0;
					EnemyUP.gameObject.SetActive(false);
				}
				UpdateEnemyCharacterList();
				if (StageController.Instance.GetStageModel() != null && StageController.Instance.GetCurrentWaveModel().UnitList.Count - currentEnemyUnitIndex > 5)
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
			try
			{
				if (StageController.Instance.GetStageModel() != null && StageController.Instance.GetCurrentWaveModel().UnitList.Count - currentEnemyUnitIndex <= 5)
				{
					EnemyDown.gameObject.SetActive(false);
					return;
				}
				currentEnemyUnitIndex += 5;
				EnemyUP.gameObject.SetActive(true);
				UpdateEnemyCharacterList();
				if (StageController.Instance.GetStageModel() != null && StageController.Instance.GetCurrentWaveModel().UnitList.Count - currentEnemyUnitIndex <= 5)
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
			try
			{
				if (currentLibrarianUnitIndex <= 0)
				{
					LibrarianUP.gameObject.SetActive(false);
					return;
				}
				currentLibrarianUnitIndex -= 5;
				if (currentLibrarianUnitIndex <= 0)
				{
					currentLibrarianUnitIndex = 0;
					LibrarianUP.gameObject.SetActive(false);
				}
				UpdateLibrarianCharacterList();
				if (StageController.Instance.GetCurrentStageFloorModel() != null && StageController.Instance.GetCurrentStageFloorModel().GetUnitBattleDataList().Count - currentLibrarianUnitIndex > 5)
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
				if (StageController.Instance.GetCurrentStageFloorModel() != null && StageController.Instance.GetCurrentStageFloorModel().GetUnitBattleDataList().Count - currentLibrarianUnitIndex <= 5)
				{
					LibrarianDown.gameObject.SetActive(false);
					return;
				}
				currentLibrarianUnitIndex += 5;
				LibrarianUP.gameObject.SetActive(true);
				UpdateLibrarianCharacterList();
				if (StageController.Instance.GetCurrentStageFloorModel() != null && StageController.Instance.GetCurrentStageFloorModel().GetUnitBattleDataList().Count - currentLibrarianUnitIndex <= 5)
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
		public static void UpdateEnemyCharacterList()
		{
			StageWaveModel currentStageWaveModel = StageController.Instance.GetCurrentWaveModel();
			if (currentStageWaveModel == null)
			{
				return;
			}
			if (UIPanel.Controller.CurrentUIPhase == UIPhase.BattleSetting)
			{
				int count = currentStageWaveModel.GetUnitBattleDataList().Count - currentEnemyUnitIndex;
				if (count > 0)
				{
					List<UnitBattleDataModel> list = currentStageWaveModel.GetUnitBattleDataList().GetRange(currentEnemyUnitIndex, Math.Min(5, count));
					BaseMod.UIPanelTool.GetEnemyCharacterListPanel().SetCharacterRenderer(list, false);
					enemyCharacterList.InitBattleEnemyList(list);
				}
			}
		}
		public static void UpdateLibrarianCharacterList()
		{
			StageLibraryFloorModel currentStageFloorModel = StageController.Instance.GetCurrentStageFloorModel();
			if (currentStageFloorModel == null)
			{
				return;
			}
			if (UIPanel.Controller.CurrentUIPhase == UIPhase.BattleSetting)
			{
				int count = currentStageFloorModel.GetUnitBattleDataList().Count - currentLibrarianUnitIndex;
				if (count > 0)
				{
					List<UnitBattleDataModel> list = currentStageFloorModel.GetUnitBattleDataList().GetRange(currentLibrarianUnitIndex, Math.Min(5, count));
					BaseMod.UIPanelTool.GetLibrarianCharacterListPanel().SetCharacterRenderer(list, false);
					IsTurningPage = true;
					librarianCharacterList.InitUnitListFromBattleData(list);
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
