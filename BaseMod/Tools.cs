using HarmonyLib;
using MyJsonTool;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Networking;

namespace BaseMod
{
    public static class Tools
    {
        public static LorId MakeLorId(int id)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            if (!Harmony_Patch.ModWorkShopId.TryGetValue(callingAssembly, out string WorkShopId))
            {
                string path = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(callingAssembly.CodeBase).Path));
                DirectoryInfo dir = new DirectoryInfo(path);
                if (File.Exists(path + "/StageModInfo.xml"))
                {
                    using (StringReader stringReader = new StringReader(File.ReadAllText(path + "/StageModInfo.xml")))
                    {
                        WorkShopId = ((Workshop.NormalInvitation)new XmlSerializer(typeof(Workshop.NormalInvitation)).Deserialize(stringReader)).workshopInfo.uniqueId;
                    }
                }
                else if (File.Exists(dir.Parent.FullName + "/StageModInfo.xml"))
                {
                    using (StringReader stringReader = new StringReader(File.ReadAllText(dir.Parent.FullName + "/StageModInfo.xml")))
                    {
                        WorkShopId = ((Workshop.NormalInvitation)new XmlSerializer(typeof(Workshop.NormalInvitation)).Deserialize(stringReader)).workshopInfo.uniqueId;
                    }
                }
                else
                {
                    WorkShopId = "";
                }
                if (WorkShopId.ToLower().EndsWith("@origin"))
                {
                    WorkShopId = "";
                }
                Harmony_Patch.ModWorkShopId[callingAssembly] = WorkShopId;
            }
            return new LorId(Harmony_Patch.ModWorkShopId[callingAssembly], id);
        }
        public static void ExhaustCardAnyWhere(this BattleAllyCardDetail cardDetail, BattleDiceCardModel card)
        {
            cardDetail._cardInReserved.Remove(card);
            cardDetail._cardInUse.Remove(card);
            cardDetail._cardInHand.Remove(card);
            cardDetail._cardInDiscarded.Remove(card);
            cardDetail._cardInDeck.Remove(card);
        }
        public static BattleDiceCardModel DrawCardSpecified(this BattleAllyCardDetail cardDetail, Predicate<BattleDiceCardModel> match)
        {
            BattleDiceCardModel result;
            if (cardDetail.GetHand().Count >= cardDetail._maxDrawHand)
            {
                result = null;
            }
            else
            {
                try
                {
                    List<BattleDiceCardModel> list = cardDetail._cardInDeck;
                    List<BattleDiceCardModel> list2 = cardDetail._cardInDiscarded;
                    list.AddRange(list2);
                    list2.Clear();
                    BattleDiceCardModel battleDiceCardModel = list.Find(match);
                    if (battleDiceCardModel != null)
                    {
                        cardDetail.AddCardToHand(battleDiceCardModel, false);
                        list.Remove(battleDiceCardModel);
                        return battleDiceCardModel;
                    }
                }
                catch (Exception ex)
                {
                    File.WriteAllText(Application.dataPath + "/Mods/DrawCardSpecifiederror.log", ex.Message + Environment.NewLine + ex.StackTrace);
                }
                result = null;
            }
            return result;
        }
        public static void AddCustomIcon(this BattleDiceCardModel cardModel, string resName, int priority = 0)
        {
            cardModel._iconAdder = resName;
            Sprite spr = Harmony_Patch.ArtWorks[resName];
            List<BattleDiceCardModel.CardIcon> list = cardModel._addedIcons;
            bool flag = list.Find((BattleDiceCardModel.CardIcon x) => x.Icon == spr) == null;
            if (flag)
            {
                BattleDiceCardModel.CardIcon item = new BattleDiceCardModel.CardIcon(spr, priority);
                list.Add(item);
                list.Sort((BattleDiceCardModel.CardIcon x, BattleDiceCardModel.CardIcon y) => y.Priority - x.Priority);
            }
        }
        public static void SetAlarmText(string alarmtype, UIAlarmButtonType btnType = UIAlarmButtonType.Default, ConfirmEvent confirmFunc = null, params object[] args)
        {
            if (UIAlarmPopup.instance.IsOpened())
            {
                UIAlarmPopup.instance.Close();
            }
            UIAlarmPopup.instance.currentAnimState = UIAlarmPopup.UIAlarmAnimState.Normal;
            GameObject ob_blue = UIAlarmPopup.instance.ob_blue;
            GameObject ob_normal = UIAlarmPopup.instance.ob_normal;
            GameObject ob_Reward = UIAlarmPopup.instance.ob_Reward;
            GameObject ob_BlackBg = UIAlarmPopup.instance.ob_BlackBg;
            List<GameObject> ButtonRoots = UIAlarmPopup.instance.ButtonRoots;
            if (ob_blue.activeSelf)
            {
                ob_blue.gameObject.SetActive(false);
            }
            if (!ob_normal.activeSelf)
            {
                ob_normal.gameObject.SetActive(true);
            }
            if (ob_Reward.activeSelf)
            {
                ob_Reward.SetActive(false);
            }
            if (ob_BlackBg.activeSelf)
            {
                ob_BlackBg.SetActive(false);
            }
            foreach (GameObject gameObject in ButtonRoots)
            {
                gameObject.gameObject.SetActive(false);
            }
            UIAlarmPopup.instance.currentAlarmType = UIAlarmType.Default;
            UIAlarmPopup.instance.buttonNumberType = btnType;
            UIAlarmPopup.instance.currentmode = AnimatorUpdateMode.Normal;
            UIAlarmPopup.instance.anim.updateMode = AnimatorUpdateMode.Normal;
            TextMeshProUGUI txt_alarm = UIAlarmPopup.instance.txt_alarm;
            if (args == null)
            {
                txt_alarm.text = TextDataModel.GetText(alarmtype, Array.Empty<object>());
            }
            else
            {
                txt_alarm.text = TextDataModel.GetText(alarmtype, args);
            }
            typeof(UIAlarmPopup).GetField("_confirmEvent", AccessTools.all).SetValue(UIAlarmPopup.instance, confirmFunc);
            ButtonRoots[(int)btnType].gameObject.SetActive(true);
            UIAlarmPopup.instance.Open();
            if (btnType == UIAlarmButtonType.Default)
            {
                UIControlManager.Instance.SelectSelectableForcely(UIAlarmPopup.instance.OkButton, false);
                return;
            }
            if (btnType == UIAlarmButtonType.YesNo)
            {
                UIControlManager.Instance.SelectSelectableForcely(UIAlarmPopup.instance.yesButton, false);
            }
        }
        public static AudioClip GetAudio(string path)
        {
            string Name = Path.GetFileNameWithoutExtension(path);
            return GetAudio(path, Name);
        }
        public static AudioClip GetAudio(string path, string Name = "")
        {
            if (Harmony_Patch.AudioClips == null)
            {
                Harmony_Patch.AudioClips = new Dictionary<string, AudioClip>();
            }
            if (!string.IsNullOrEmpty(Name) && Harmony_Patch.AudioClips.ContainsKey(Name))
            {
                return Harmony_Patch.AudioClips[Name];
            }
            AudioType audioType;
            string fullname;
            if (path.EndsWith(".wav"))
            {
                fullname = path;
                audioType = AudioType.WAV;
            }
            else if (path.EndsWith(".ogg"))
            {
                fullname = path;
                audioType = AudioType.OGGVORBIS;
            }
            else if (path.EndsWith(".mp3"))
            {
                fullname = path.Replace(".mp3", ".wav");
                Mp3FileReader mp3FileReader = new Mp3FileReader(path);
                WaveFileWriter.CreateWaveFile(fullname, mp3FileReader);
                audioType = AudioType.WAV;
            }
            else
            {
                return null;
            }
            UnityWebRequest audioClip = UnityWebRequestMultimedia.GetAudioClip("file://" + fullname, audioType);
            audioClip.SendWebRequest();
            while (!audioClip.isDone)
            {
            }
            AudioClip content = DownloadHandlerAudioClip.GetContent(audioClip);
            if (path.EndsWith(".mp3"))
            {
                File.Delete(fullname);
            }
            if (!string.IsNullOrEmpty(Name))
            {
                content.name = Name;
                Harmony_Patch.AudioClips[Name] = content;
            }
            return content;
        }
        public static void Save<T>(this T value, string key)
        {
            if (string.IsNullOrEmpty(GetModId(Assembly.GetCallingAssembly())))
            {
                return;
            }
            string text = string.Concat(new string[]
            {
                Application.dataPath,
                "/ModSaves/",
                GetModId(Assembly.GetCallingAssembly())+"/",
                key,
                ".json"
            });
            Directory.CreateDirectory(Application.dataPath + "/ModSaves/" + GetModId(Assembly.GetCallingAssembly()));
            File.WriteAllText(text, new Test<T>
            {
                value = value
            }.ToJson());
        }
        public static T Load<T>(string key)
        {
            if (string.IsNullOrEmpty(GetModId(Assembly.GetCallingAssembly())))
            {
                return default;
            }
            string text = string.Concat(new string[]
            {
                Application.dataPath,
                "/ModSaves/",
                GetModId(Assembly.GetCallingAssembly())+"/",
                key,
                ".json"
            });
            if (!File.Exists(text))
            {
                return default;
            }
            return File.ReadAllText(text).ToObject<Test<T>>().value;
        }
        public static string GetModId(Assembly callingAssembly)
        {
            if (!Harmony_Patch.ModWorkShopId.TryGetValue(callingAssembly, out string WorkShopId))
            {
                string path = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(callingAssembly.CodeBase).Path));
                DirectoryInfo dir = new DirectoryInfo(path);
                if (File.Exists(path + "/StageModInfo.xml"))
                {
                    using (StringReader stringReader = new StringReader(File.ReadAllText(path + "/StageModInfo.xml")))
                    {
                        WorkShopId = ((Workshop.NormalInvitation)new XmlSerializer(typeof(Workshop.NormalInvitation)).Deserialize(stringReader)).workshopInfo.uniqueId;
                    }
                }
                else if (File.Exists(dir.Parent.FullName + "/StageModInfo.xml"))
                {
                    using (StringReader stringReader = new StringReader(File.ReadAllText(dir.Parent.FullName + "/StageModInfo.xml")))
                    {
                        WorkShopId = ((Workshop.NormalInvitation)new XmlSerializer(typeof(Workshop.NormalInvitation)).Deserialize(stringReader)).workshopInfo.uniqueId;
                    }
                }
                else
                {
                    WorkShopId = "";
                }
                Harmony_Patch.ModWorkShopId[callingAssembly] = WorkShopId;
            }
            return Harmony_Patch.ModWorkShopId[callingAssembly];
        }
        public class Test<T>
        {
            public T value;
        }
    }
}
