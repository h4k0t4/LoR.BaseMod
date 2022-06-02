using GTMDProjectMoon;
using HarmonyLib;
using LOR_DiceSystem;
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
        /// <summary>
        /// 生成LorId
        /// </summary>
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
        public static T GetScript<T>(this BattleDiceCardModel cardModel) where T : DiceCardSelfAbilityBase
        {
            if (cardModel._script != null && cardModel._script is T)
            {
                return cardModel._script as T;
            }
            return null;
        }
        public static DiceCardSelfAbilityBase SetScript<T>(this BattleDiceCardModel cardModel, DiceCardSelfAbilityBase selfAbilityBase)
        {
            cardModel._script = selfAbilityBase;
            return selfAbilityBase;
        }
        public static bool ContainsCategory(this BookModel book, string category)
        {
            return book._classInfo.ContainsCategory(category);
        }
        public static bool ContainsCategory(this BookXmlInfo book, string category)
        {
            return book.categoryList.Contains(OrcTools.GetBookCategory(category));
        }
        public static bool ContainsCategory(this BattleDiceCardModel card, string category)
        {
            return card._xmlData.ContainsCategory(category);
        }
        public static bool ContainsCategory(this DiceCardXmlInfo card, string category)
        {
            if (card is DiceCardXmlInfo_New)
            {
                (card as DiceCardXmlInfo_New).categoryList.Contains(OrcTools.GetBookCategory(category));
            }
            return card.category == OrcTools.GetBookCategory(category);
        }
        public static void SetAbilityData(this BattleCardBehaviourResult battleCardBehaviourResult, EffectTypoData effectTypoData)
        {
            if (Harmony_Patch.CustomEffectTypoData == null)
            {
                Harmony_Patch.CustomEffectTypoData = new Dictionary<BattleCardBehaviourResult, List<EffectTypoData>>();
            }
            if (battleCardBehaviourResult == null || effectTypoData == null)
            {
                return;
            }
            if (!Harmony_Patch.CustomEffectTypoData.ContainsKey(battleCardBehaviourResult))
            {
                Harmony_Patch.CustomEffectTypoData[battleCardBehaviourResult] = new List<EffectTypoData>();
            }
            Harmony_Patch.CustomEffectTypoData[battleCardBehaviourResult].Add(effectTypoData);
        }
        public static void SetAbilityData(this BattleCardBehaviourResult battleCardBehaviourResult, EffectTypoData_New effectTypoData_New)
        {
            if (Harmony_Patch.CustomEffectTypoData == null)
            {
                Harmony_Patch.CustomEffectTypoData = new Dictionary<BattleCardBehaviourResult, List<EffectTypoData>>();
            }
            if (battleCardBehaviourResult == null || effectTypoData_New == null)
            {
                return;
            }
            if (!Harmony_Patch.CustomEffectTypoData.ContainsKey(battleCardBehaviourResult))
            {
                Harmony_Patch.CustomEffectTypoData[battleCardBehaviourResult] = new List<EffectTypoData>();
            }
            Harmony_Patch.CustomEffectTypoData[battleCardBehaviourResult].Add(effectTypoData_New);
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
            if (!File.Exists(path))
            {
                return null;
            }
            string Name = Path.GetFileNameWithoutExtension(path);
            return GetAudio(path, Name);
        }
        public static AudioClip GetAudio(string path, string Name = "")
        {
            if (Harmony_Patch.AudioClips == null)
            {
                Harmony_Patch.AudioClips = new Dictionary<string, AudioClip>();
            }
            if (!string.IsNullOrWhiteSpace(Name) && Harmony_Patch.AudioClips.ContainsKey(Name))
            {
                return Harmony_Patch.AudioClips[Name];
            }
            if (!File.Exists(path))
            {
                return null;
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
            if (audioClip.isHttpError || audioClip.isNetworkError)
            {
                return null;
            }
            AudioClip content = DownloadHandlerAudioClip.GetContent(audioClip);
            if (path.EndsWith(".mp3"))
            {
                File.Delete(fullname);
            }
            if (!string.IsNullOrWhiteSpace(Name))
            {
                content.name = Name;
                Harmony_Patch.AudioClips[Name] = content;
            }
            return content;
        }
        public static void Save<T>(this T value, string key)
        {
            if (string.IsNullOrWhiteSpace(GetModId(Assembly.GetCallingAssembly())))
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
            if (string.IsNullOrWhiteSpace(GetModId(Assembly.GetCallingAssembly())))
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
    public class EffectTypoData_New : EffectTypoData
    {
        public string type;

        public BattleUIPassiveSet battleUIPassiveSet = null;

        public class BattleUIPassiveSet
        {
            public Sprite frame;

            public Sprite Icon;

            public Sprite IconGlow;

            public Color textColor;

            public Color IconColor;

            public Color IconGlowColor;
        }
    }
}

namespace BaseMod
{
    public static class PassiveAbilityExtension
    {
        public static T FindPassive<T>(this BattleUnitPassiveDetail passiveDetail) where T : PassiveAbilityBase
        {
            return (T)passiveDetail.PassiveList.Find(x => (x as T) != null);
        }
        public static List<T> FindPassives<T>(this BattleUnitPassiveDetail passiveDetail) where T : PassiveAbilityBase
        {
            List<T> list = new List<T>();
            List<PassiveAbilityBase> sourcer = passiveDetail.PassiveList;
            for (int i = 0; i < sourcer.Count; i++)
            {
                if (list[i] is T x)
                {
                    list.Add(x);
                }
            }
            return list;
        }
    }
}

namespace BaseMod
{
    public static class BuffExtension
    {
        public static BattleUnitBuf AddBufByCard(this BattleUnitBufListDetail unitBufListDetail, BattleUnitBuf buf, int stack, BattleUnitModel actor = null, BufReadyType readyType = BufReadyType.ThisRound)
        {
            if (buf == null)
            {
                return buf;
            }
            if (actor == null)
            {
                actor = unitBufListDetail._self;
            }
            buf._owner = unitBufListDetail._self;
            buf.stack = 0;
            BattleUnitBuf battleUnitBuf = buf.FindMatch(readyType);
            if (battleUnitBuf == null)
            {
                return battleUnitBuf;
            }
            battleUnitBuf.Modify(stack, actor, true);
            return battleUnitBuf;
        }
        public static T AddBufByCard<T>(this BattleUnitBufListDetail unitBufListDetail, int stack, BattleUnitModel actor = null, BufReadyType readyType = BufReadyType.ThisRound) where T : BattleUnitBuf
        {
            if (actor == null)
            {
                actor = unitBufListDetail._self;
            }
            T buf = Activator.CreateInstance<T>();
            buf._owner = unitBufListDetail._self;
            buf.stack = 0;
            BattleUnitBuf battleUnitBuf = buf.FindMatch(readyType);
            if (battleUnitBuf == null)
            {
                return (T)battleUnitBuf;
            }
            battleUnitBuf.Modify(stack, actor, true);
            return (T)battleUnitBuf;
        }
        public static BattleUnitBuf AddBufByEtc(this BattleUnitBufListDetail unitBufListDetail, BattleUnitBuf buf, int stack, BattleUnitModel actor = null, BufReadyType readyType = BufReadyType.ThisRound)
        {
            if (buf == null)
            {
                return buf;
            }
            if (actor == null)
            {
                actor = unitBufListDetail._self;
            }
            buf._owner = unitBufListDetail._self;
            buf.stack = 0;
            BattleUnitBuf battleUnitBuf = buf.FindMatch(readyType);
            if (battleUnitBuf == null)
            {
                return battleUnitBuf;
            }
            battleUnitBuf.Modify(stack, actor, false);
            return battleUnitBuf;
        }
        public static T AddBufByEtc<T>(this BattleUnitBufListDetail unitBufListDetail, int stack, BattleUnitModel actor = null, BufReadyType readyType = BufReadyType.ThisRound) where T : BattleUnitBuf
        {
            if (actor == null)
            {
                actor = unitBufListDetail._self;
            }
            T buf = Activator.CreateInstance<T>();
            buf._owner = unitBufListDetail._self;
            buf.stack = 0;
            BattleUnitBuf battleUnitBuf = buf.FindMatch(readyType);
            if (battleUnitBuf == null)
            {
                return (T)battleUnitBuf;
            }
            battleUnitBuf.Modify(stack, actor, false);
            return (T)battleUnitBuf;
        }
        public static TResult FindBuf<TResult>(this BattleUnitBufListDetail unitBufListDetail, BufReadyType readyType = BufReadyType.ThisRound) where TResult : BattleUnitBuf
        {
            List<BattleUnitBuf> list = unitBufListDetail.FindList(readyType);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] is TResult x && !list[i].IsDestroyed())
                {
                    return x;
                }
            }
            return null;
        }
        public static List<TResult> FindAllBuf<TResult>(this BattleUnitBufListDetail unitBufListDetail, BufReadyType readyType = BufReadyType.ThisRound) where TResult : BattleUnitBuf
        {
            List<TResult> list = new List<TResult>();
            List<BattleUnitBuf> sourcer = unitBufListDetail.FindList(readyType);
            for (int i = 0; i < sourcer.Count; i++)
            {
                if (list[i] is TResult x && !list[i].IsDestroyed())
                {
                    list.Add(x);
                }
            }
            return list;
        }
        public static int AddBufStack(this BattleUnitBuf buf, int stack, BattleUnitModel actor = null, bool byCard = true)
        {
            buf.Modify(stack, actor, byCard);
            return buf.stack;
        }
        public static int SetBufStack<TResult>(this BattleUnitBufListDetail unitBufListDetail, int stack, BufReadyType readyType = BufReadyType.ThisRound) where TResult : BattleUnitBuf
        {
            BattleUnitBuf buf = unitBufListDetail.FindBuf<TResult>(readyType);
            if (buf != null)
            {
                buf.stack = stack;
                buf.OnAddBuf(stack);
                return buf.stack;
            }
            return 0;
        }
        public static BattleUnitBuf FindMatch(this BattleUnitBuf buf, BufReadyType readyType)
        {
            if (buf._owner == null || !buf._owner.bufListDetail.CanAddBuf(buf))
            {
                return null;
            }
            List<BattleUnitBuf> bufList = buf._owner.bufListDetail.FindList(readyType);
            BattleUnitBuf battleUnitBuf = bufList.Find((BattleUnitBuf targetBuf) => targetBuf.GetType() == buf.GetType() && !targetBuf.IsDestroyed());
            if (battleUnitBuf == null || battleUnitBuf.independentBufIcon)
            {
                buf.Init(buf._owner);
                battleUnitBuf = buf;
                bufList.Add(battleUnitBuf);
            }
            return battleUnitBuf;
        }
        private static BattleUnitBuf Modify(this BattleUnitBuf buf, int stack, BattleUnitModel actor, bool byCard = true)
        {
            if (byCard)
            {
                int num = 0;
                num += actor.OnGiveKeywordBufByCard(buf, stack, buf._owner);
                num += buf._owner.OnAddKeywordBufByCard(buf, stack);
                stack += num;
                stack *= actor.GetMultiplierOnGiveKeywordBufByCard(buf, buf._owner);
            }
            buf._owner.bufListDetail.ModifyStack(buf, stack);
            int stack2 = buf.stack;
            buf.stack += stack;
            buf.OnAddBuf(stack);
            if (byCard)
            {
                buf._owner.OnAddKeywordBufByCardForEvent(buf.bufType, stack, BufReadyType.NextRound);
            }
            if (buf.bufType == KeywordBuf.WarpCharge && buf.stack > stack2)
            {
                buf._owner.OnGainChargeStack();
            }
            buf._owner.bufListDetail.CheckGift(buf.bufType, stack, actor);
            return buf;
        }
        public static List<BattleUnitBuf> FindList(this BattleUnitBufListDetail unitBufListDetail, BufReadyType readyType = BufReadyType.ThisRound)
        {
            List<BattleUnitBuf> result = unitBufListDetail.GetActivatedBufList();
            switch (readyType)
            {
                case BufReadyType.NextRound:
                    result = unitBufListDetail.GetReadyBufList();
                    break;
                case BufReadyType.NextNextRound:
                    result = unitBufListDetail.GetReadyReadyBufList();
                    break;
            }
            return result;
        }
    }
}

namespace BaseMod
{
    public static class CardBuffExtension
    {

    }
}