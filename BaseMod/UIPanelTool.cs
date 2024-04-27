using System.Collections.Generic;
using System.Linq;
using System.Text;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace BaseMod
{
	public static class UIPanelTool
	{
		public static T GetUIPanel<T>(UIPanelType type) where T : UIPanel
		{
			return UI.UIController.Instance.GetUIPanel(type) as T;
		}
		public static UIMainPanel GetMainPanel()
		{
			这个是注译 = "主界面中间楼层和云朵UI";
			UIMainPanel uipanel = GetUIPanel<UIMainPanel>(UIPanelType.Main);
			Debug(uipanel.gameObject);
			return uipanel;
		}
		public static UIBattleResultPanel GetBattleResultPanel()
		{
			这个是注译 = "接待后奖励界面";
			UIBattleResultPanel uipanel = GetUIPanel<UIBattleResultPanel>(UIPanelType.BattleResult);
			Debug(uipanel.gameObject);
			return uipanel;
		}
		public static UIBattleSettingPanel GetBattleSettingPanel()
		{
			这个是注译 = "接待前";
			UIBattleSettingPanel uipanel = GetUIPanel<UIBattleSettingPanel>(UIPanelType.BattleSetting);
			Debug(uipanel.gameObject);
			return uipanel;
		}
		public static UIControlButtonPanel GetControlButtonPanel()
		{
			这个是注译 = "左侧栏按钮(不是敌人)";
			UIControlButtonPanel uipanel = GetUIPanel<UIControlButtonPanel>(UIPanelType.ControlButton);
			Debug(uipanel.gameObject);
			return uipanel;
		}
		public static UICurtainPanel GetCurtainPanel()
		{
			这个是注译 = "没有使用";
			UICurtainPanel uipanel = GetUIPanel<UICurtainPanel>(UIPanelType.Curtain);
			Debug(uipanel.gameObject);
			return uipanel;
		}
		public static UIPanel GetDecorationsPanel()
		{
			这个是注译 = "UI05";
			UIPanel uipanel = GetUIPanel<UIPanel>(UIPanelType.Decorations);
			Debug(uipanel.gameObject);
			return uipanel;
		}
		public static UIPanel GetDUMMYPanel()
		{
			这个是注译 = "UI06";
			UIPanel uipanel = GetUIPanel<UIPanel>(UIPanelType.DUMMY);
			Debug(uipanel.gameObject);
			return uipanel;
		}
		public static UIFilterPanel GetFilterPanel()
		{
			这个是注译 = "点击司书出现的界面最上方几个按钮(非接待前)";
			UIFilterPanel uipanel = GetUIPanel<UIFilterPanel>(UIPanelType.Filter);
			Debug(uipanel.gameObject);
			return uipanel;
		}
		public static UIFloorPanel GetFloorPanel()
		{
			这个是注译 = "主界面楼层信息";
			UIFloorPanel uipanel = GetUIPanel<UIFloorPanel>(UIPanelType.FloorInfo);
			Debug(uipanel.gameObject);
			return uipanel;
		}
		public static UIInvitationPanel GetInvitationPanel()
		{
			这个是注译 = "主线界面";
			UIInvitationPanel uipanel = GetUIPanel<UIInvitationPanel>(UIPanelType.Invitation);
			Debug(uipanel.gameObject);
			return uipanel;
		}
		public static UILibrarianInfoPanel GetLibrarianInfoPanel()
		{
			这个是注译 = "司书信息";
			UILibrarianInfoPanel uipanel = GetUIPanel<UILibrarianInfoPanel>(UIPanelType.LibrarianInfo);
			Debug(uipanel.gameObject);
			return uipanel;
		}
		public static UIStoryArchivesPanel GetStoryArchivesPanel()
		{
			这个是注译 = "书库";
			UIStoryArchivesPanel uipanel = GetUIPanel<UIStoryArchivesPanel>(UIPanelType.Story);
			Debug(uipanel.gameObject);
			return uipanel;
		}
		public static UITitlePanel GetTitlePanel()
		{
			这个是注译 = "左上角标题";
			UITitlePanel uipanel = GetUIPanel<UITitlePanel>(UIPanelType.Title);
			Debug(uipanel.gameObject);
			return uipanel;
		}
		public static UIBookPanel GetBookPanel()
		{
			这个是注译 = "烧书界面";
			UIBookPanel uipanel = GetUIPanel<UIBookPanel>(UIPanelType.Book);
			Debug(uipanel.gameObject);
			return uipanel;
		}
		public static UICardPanel GetCardPanel()
		{
			这个是注译 = "战斗外战斗书页列表";
			UICardPanel uipanel = GetUIPanel<UICardPanel>(UIPanelType.Page);
			Debug(uipanel.gameObject);
			return uipanel;
		}
		public static UIEquipPageInventoryPanel GetEquipPageInventoryPanel()
		{
			这个是注译 = "战斗外核心书页列表";
			UIEquipPageInventoryPanel uipanel = GetUIPanel<UIEquipPageInventoryPanel>(UIPanelType.ItemList);
			Debug(uipanel.gameObject);
			return uipanel;
		}
		public static UILibrarianCharacterListPanel GetLibrarianCharacterListPanel()
		{
			这个是注译 = "右侧玩家UI";
			UILibrarianCharacterListPanel uipanel = GetUIPanel<UILibrarianCharacterListPanel>(UIPanelType.CharacterList_Right);
			Debug(uipanel.gameObject);
			return uipanel;
		}
		public static UIEnemyCharacterListPanel GetEnemyCharacterListPanel()
		{
			这个是注译 = "左侧敌人UI";
			UIEnemyCharacterListPanel uipanel = GetUIPanel<UIEnemyCharacterListPanel>(UIPanelType.CharacterList);
			Debug(uipanel.gameObject);
			return uipanel;
		}
		public static UISephirahButton GetSephirahButton(SephirahType sephirahType)
		{
			这个是注译 = "战斗主界面楼层按钮";
			UISephirahButton uisephirahButton = GetBattleSettingPanel().FindSephirahButton(sephirahType);
			Debug(uisephirahButton.gameObject);
			return uisephirahButton;
		}
		public static UISettingEquipPageInvenPanel GetEquipInvenPanel()
		{
			这个是注译 = "UI16";
			UISettingEquipPageInvenPanel equipPagePanel = GetUIPanel<UIBattleSettingPanel>(UIPanelType.BattleSetting).EditPanel.EquipPagePanel;
			Debug(equipPagePanel.gameObject);
			return equipPagePanel;
		}
		public static UIBattleStoryPanel GetBattleStoryPanel()
		{
			这个是注译 = "UI17";
			UIBattleStoryPanel battleStoryPanel = GetUIPanel<UIStoryArchivesPanel>(UIPanelType.Story).battleStoryPanel;
			Debug(battleStoryPanel.gameObject);
			return battleStoryPanel;
		}
		public static UILibrarianEquipDeckPanel GetCardDeckPanel()
		{
			这个是注译 = "玩家核心书页";
			if (UI.UIController.Instance.CurrentUIPhase != UIPhase.BattleSetting)
			{
				UILibrarianEquipDeckPanel equipInfoDeckPanel = GetUIPanel<UICardPanel>(UIPanelType.Page).EquipInfoDeckPanel;
				Debug(equipInfoDeckPanel.gameObject);
				return equipInfoDeckPanel;
			}
			UILibrarianEquipDeckPanel equipInfoDeckPanel2 = GetUIPanel<UIBattleSettingPanel>(UIPanelType.BattleSetting).EditPanel.BattleCardPanel.EquipInfoDeckPanel;
			Debug(equipInfoDeckPanel2.gameObject);
			return equipInfoDeckPanel2;
		}
		public static UICardEquipInfoPanel GetCardEquipInfoPanel()
		{
			这个是注译 = "战斗书页";
			if (UI.UIController.Instance.CurrentUIPhase != UIPhase.BattleSetting)
			{
				UICardEquipInfoPanel cardEquipInfoPanel = GetUIPanel<UICardPanel>(UIPanelType.Page).CardEquipInfoPanel;
				Debug(cardEquipInfoPanel.gameObject);
				return cardEquipInfoPanel;
			}
			UICardEquipInfoPanel cardEquipInfoPanel2 = GetUIPanel<UIBattleSettingPanel>(UIPanelType.BattleSetting).EditPanel.BattleCardPanel.CardEquipInfoPanel;
			Debug(cardEquipInfoPanel2.gameObject);
			return cardEquipInfoPanel2;
		}
		static void Debug(GameObject UIPanel)
		{
			if (IsDebug)
			{
				try
				{
					Text[] componentsInChildren = UIPanel.GetComponentsInChildren<Text>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].text = 这个是注译 + "_" + i.ToString();
						componentsInChildren[i].color = Color.red;
					}
					Image[] componentsInChildren2 = UIPanel.GetComponentsInChildren<Image>();
					for (int j = 0; j < componentsInChildren2.Length; j++)
					{
						componentsInChildren2[j].color = Color.red;
					}
				}
				catch
				{
				}
			}
		}
		public static void DoSetParent(Transform Target, Transform transform)
		{
			if (Target.GetComponent<RectTransform>())
			{
				Target.transform.SetParent(transform, false);
				return;
			}
			Target.transform.parent = transform;
		}
		public static void SetParent(Transform Target, string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				DoSetParent(Target, null);
				return;
			}
			Transform transform = GetTransform(input);
			if (transform)
			{
				DoSetParent(Target, transform);
			}
		}
		static string GetTransformPath(Transform transform, bool includeSelf = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (includeSelf)
			{
				stringBuilder.Append(transform.name);
			}
			while (transform.parent)
			{
				transform = transform.parent;
				stringBuilder.Insert(0, '/');
				stringBuilder.Insert(0, transform.name);
			}
			return stringBuilder.ToString();
		}
		public static Transform GetTransform(string input)
		{
			Transform result = null;
			if (input.EndsWith("/"))
			{
				input = input.Remove(input.Length - 1);
			}
			GameObject gameObject = GameObject.Find(input);
			if (gameObject != null)
			{
				result = gameObject.transform;
			}
			else
			{
				string b = input.Split(new char[]
				{
					'/'
				}).Last();
				Object[] array = Resources.FindObjectsOfTypeAll(typeof(GameObject));
				List<GameObject> list = new List<GameObject>();
				foreach (Object @object in array)
				{
					if (@object.name == b)
					{
						list.Add((GameObject)@object);
					}
				}
				foreach (GameObject gameObject2 in list)
				{
					string text = GetTransformPath(gameObject2.transform, true);
					if (text.EndsWith("/"))
					{
						text = text.Remove(text.Length - 1);
					}
					if (text == input)
					{
						result = gameObject2.transform;
						break;
					}
				}
			}
			return result;
		}

		public static string 这个是注译 = string.Empty;

		public static bool IsDebug;
	}
}