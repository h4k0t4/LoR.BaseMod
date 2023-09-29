using BaseMod;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ModSettingTool
{
	public class InitGameObject : MonoBehaviour
	{
		public static void SetAlarmText(string alarmtype, List<string> Options, OptionEvent confirmFunc = null, params object[] args)
		{
			if (UIAlarmPopup.instance.IsOpened())
			{
				UIAlarmPopup.instance.Close();
			}
			if (OptionDropdown == null)
			{
				GameObject gameObject6 = Instantiate(UtilTools.GetTransform("[Canvas][Script]PopupCanvas/[Script]PopupManager/[Prefab]UIOptionWindow/OptionRootPanel/[Layout]PanelLayout/RightPanel/[Prefab]OptionDropdown (1)/").gameObject);
				OptionDropdown = gameObject6.GetComponent<TMP_Dropdown>();
				gameObject6.transform.SetParent(UIAlarmPopup.instance.transform, false);
				gameObject6.transform.localPosition = new Vector3(-121.3409f, 72.2201f, 0f);
				gameObject6.transform.localScale = new Vector3(1.1f, 1.15f, 1f);
				GameObject gameObject7 = Instantiate(UtilTools.GetTransform("[Canvas][Script]PopupCanvas/[Prefab]PopupAlarm/[Rect]Normal/[Button]OK/").gameObject);
				gameObject7.transform.SetParent(UIAlarmPopup.instance.transform, false);
				gameObject7.transform.localPosition = new Vector3(140.7599f, 69.4f, 0f);
				GetEvent = gameObject7.GetComponent<EventTrigger>();
				GetEvent.triggers[0].callback.RemoveAllListeners();
				GetEvent.triggers[0].callback.AddListener(new UnityAction<BaseEventData>(OnClick));
				GetEvent.triggers[2].callback.RemoveAllListeners();
				GetEvent.triggers[2].callback.AddListener(new UnityAction<BaseEventData>(OnClick));
			}
			OptionDropdown.gameObject.SetActive(true);
			GetEvent.gameObject.SetActive(true);
			OptionDropdown.ClearOptions();
			OptionDropdown.AddOptions(Options);
			UIAlarmPopup.instance.currentAnimState = UIAlarmPopup.UIAlarmAnimState.Normal;
			GameObject ob_blue = UIAlarmPopup.instance.ob_blue;
			GameObject ob_normal = UIAlarmPopup.instance.ob_normal;
			GameObject ob_Reward = UIAlarmPopup.instance.ob_Reward;
			GameObject ob_BlackBg = UIAlarmPopup.instance.ob_BlackBg;
			List<GameObject> ButtonRoots = UIAlarmPopup.instance.ButtonRoots;
			ob_blue.SetActive(false);
			ob_normal.SetActive(true);
			ob_Reward.SetActive(false);
			ob_BlackBg.SetActive(false);
			foreach (GameObject gameObject8 in ButtonRoots)
			{
				gameObject8.SetActive(false);
			}
			UIAlarmPopup.instance.currentAlarmType = UIAlarmType.Default;
			UIAlarmPopup.instance.buttonNumberType = UIAlarmButtonType.YesNo;
			UIAlarmPopup.instance.currentmode = AnimatorUpdateMode.Normal;
			UIAlarmPopup.instance.anim.updateMode = AnimatorUpdateMode.Normal;
			TextMeshProUGUI txt_alarm = UIAlarmPopup.instance.txt_alarm;
			txt_alarm.text = TextDataModel.GetText(alarmtype, args);
			if (string.IsNullOrWhiteSpace(txt_alarm.text))
			{
				txt_alarm.text = string.Format(alarmtype, args);
			}
			GetOptionEvent = confirmFunc;
			UIAlarmPopup.instance.Open();
		}
		static void OnClick(BaseEventData data)
		{
			UISoundManager.instance.PlayEffectSound(UISoundType.Ui_Click);
			OptionDropdown.gameObject.SetActive(false);
			GetEvent.gameObject.SetActive(false);
			UIAlarmPopup.instance.Close();
			if (GetOptionEvent != null)
			{
				GetOptionEvent(OptionDropdown.value);
				GetOptionEvent = null;
			}
		}
		static TMP_Dropdown OptionDropdown;

		static OptionEvent GetOptionEvent;

		static EventTrigger GetEvent;
	}
	public delegate void OptionEvent(int selection);
}
