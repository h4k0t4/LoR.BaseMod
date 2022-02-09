using BaseMod;
using GameSave;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ModSettingTool
{
    public static class ModSaveTool
    {
        //存档拉屎内容
        public static string ModSavePath
        {
            get
            {
                return SaveManager.savePath + "/" + "ModSaveFiles" + ".dat";
            }
        }
        public static void LoadFromSaveData()
        {
            try
            {
                if (!File.Exists(ModSavePath))
                {
                    using (FileStream fileStream = File.Create(ModSavePath))
                    {
                        new BinaryFormatter().Serialize(fileStream, ModSaveData.GetSerializedData());
                    }
                    return;
                }
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream saveFileStream;
                FileStream serializationStream = saveFileStream = File.Open(ModSavePath, FileMode.Open);
                object obj;
                try
                {
                    obj = binaryFormatter.Deserialize(serializationStream);
                }
                finally
                {
                    if (saveFileStream != null)
                    {
                        ((IDisposable)saveFileStream).Dispose();
                    }
                }
                if (obj == null)
                {
                    throw new Exception();
                }
                ModSaveData.LoadFromSerializedData(obj);
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/LoadModSaveerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        public static void SaveModSaveData()
        {
            try
            {
                using (FileStream fileStream = File.Create(ModSavePath))
                {
                    new BinaryFormatter().Serialize(fileStream, ModSaveData.GetSerializedData());
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/ModSaveerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        public static void RemoveUnknownSaves()
        {
            List<string> removeSaves = new List<string>();
            foreach (KeyValuePair<string, SaveData> keyValuePair in ModSaveData.GetDictionarySelf())
            {
                if (!LoadedModsWorkshopId.Contains(keyValuePair.Key))
                {
                    removeSaves.Add(keyValuePair.Key);
                }
            }
            if (removeSaves.Count > 0)
            {
                Tools.SetAlarmText("BaseMod_ReMoveUnloadSave", UIAlarmButtonType.YesNo, delegate (bool flag)
                {
                    if (flag)
                    {
                        foreach (string id in removeSaves)
                        {
                            ModSaveData.GetDictionarySelf().Remove(id);
                        }
                    }
                }, null);
            }
        }
        public static SaveData GetModSaveData(string WorkshopId = "")
        {
            if (string.IsNullOrEmpty(WorkshopId))
            {
                WorkshopId = Tools.GetModId(Assembly.GetCallingAssembly());
            }
            if (string.IsNullOrEmpty(WorkshopId))
            {
                return null;
            }
            if (ModSaveData.GetData(WorkshopId) == null)
            {
                ModSaveData.AddData(WorkshopId, new SaveData(SaveDataType.Dictionary));
            }
            return ModSaveData.GetData(WorkshopId);
        }
        public static void SaveString(string name, string value, string WorkshopId = "")
        {
            if (string.IsNullOrEmpty(WorkshopId))
            {
                WorkshopId = Tools.GetModId(Assembly.GetCallingAssembly());
            }
            if (string.IsNullOrEmpty(WorkshopId))
            {
                return;
            }
            GetModSaveData(WorkshopId).GetDictionarySelf()[name] = new SaveData(value);
        }
        public static void Saveint(string name, int value, string WorkshopId = "")
        {
            if (string.IsNullOrEmpty(WorkshopId))
            {
                WorkshopId = Tools.GetModId(Assembly.GetCallingAssembly());
            }
            if (string.IsNullOrEmpty(WorkshopId))
            {
                return;
            }
            GetModSaveData(WorkshopId).GetDictionarySelf()[name] = new SaveData(value);
        }
        public static void Saveulong(string name, ulong value, string WorkshopId = "")
        {
            if (string.IsNullOrEmpty(WorkshopId))
            {
                WorkshopId = Tools.GetModId(Assembly.GetCallingAssembly());
            }
            if (string.IsNullOrEmpty(WorkshopId))
            {
                return;
            }
            GetModSaveData(WorkshopId).GetDictionarySelf()[name] = new SaveData(value);
        }
        public static HashSet<string> LoadedModsWorkshopId = new HashSet<string>();

        public static SaveData ModSaveData = new SaveData(SaveDataType.Dictionary);
    }
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
            try
            {
                typeof(UIAlarmPopup).GetField("currentAnimState", AccessTools.all).SetValue(UIAlarmPopup.instance, 0);
            }
            catch
            {
                Debug.LogError("set currentAnimState error");
            }
            GameObject gameObject2 = (GameObject)typeof(UIAlarmPopup).GetField("ob_blue", AccessTools.all).GetValue(UIAlarmPopup.instance);
            GameObject gameObject3 = (GameObject)typeof(UIAlarmPopup).GetField("ob_normal", AccessTools.all).GetValue(UIAlarmPopup.instance);
            GameObject gameObject4 = (GameObject)typeof(UIAlarmPopup).GetField("ob_Reward", AccessTools.all).GetValue(UIAlarmPopup.instance);
            GameObject gameObject5 = (GameObject)typeof(UIAlarmPopup).GetField("ob_BlackBg", AccessTools.all).GetValue(UIAlarmPopup.instance);
            List<GameObject> list = (List<GameObject>)typeof(UIAlarmPopup).GetField("ButtonRoots", AccessTools.all).GetValue(UIAlarmPopup.instance);
            gameObject2.SetActive(false);
            gameObject3.SetActive(true);
            gameObject4.SetActive(false);
            gameObject5.SetActive(false);
            foreach (GameObject gameObject8 in list)
            {
                gameObject8.SetActive(false);
            }
            typeof(UIAlarmPopup).GetField("currentAlarmType", AccessTools.all).SetValue(UIAlarmPopup.instance, UIAlarmType.Default);
            typeof(UIAlarmPopup).GetField("buttonNumberType", AccessTools.all).SetValue(UIAlarmPopup.instance, UIAlarmButtonType.YesNo);
            typeof(UIAlarmPopup).GetField("currentmode", AccessTools.all).SetValue(UIAlarmPopup.instance, AnimatorUpdateMode.Normal);
            ((Animator)typeof(UIAlarmPopup).GetField("anim", AccessTools.all).GetValue(UIAlarmPopup.instance)).updateMode = AnimatorUpdateMode.Normal;
            TextMeshProUGUI textMeshProUGUI = (TextMeshProUGUI)typeof(UIAlarmPopup).GetField("txt_alarm", AccessTools.all).GetValue(UIAlarmPopup.instance);
            textMeshProUGUI.text = TextDataModel.GetText(alarmtype, args);
            if (string.IsNullOrEmpty(textMeshProUGUI.text))
            {
                textMeshProUGUI.text = string.Format(alarmtype, args);
            }
            GetOptionEvent = confirmFunc;
            UIAlarmPopup.instance.Open();
        }
        private static void OnClick(BaseEventData data)
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
        private static TMP_Dropdown OptionDropdown;

        private static OptionEvent GetOptionEvent;

        private static EventTrigger GetEvent;
    }
    public delegate void OptionEvent(int selection);
    public static class UtilTools
    {
        public static string GetTransformPath(Transform transform, bool includeSelf = false)
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
                }).Last<string>();
                UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(GameObject));
                List<GameObject> list = new List<GameObject>();
                foreach (UnityEngine.Object @object in array)
                {
                    if (@object.name == b)
                    {
                        list.Add((GameObject)@object);
                    }
                }
                foreach (GameObject gameObject2 in list)
                {
                    string text = UtilTools.GetTransformPath(gameObject2.transform, true);
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
    }
}
