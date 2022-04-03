using GTMDProjectMoon;
using LOR_XML;
using Mod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace BaseMod
{
    public class LocalizedTextLoader_New
    {
        private static string GetModdingPath(DirectoryInfo dir, string type)
        {
            return string.Concat(new string[]
            {
                dir.FullName,
                "/Localize/",
                TextDataModel.CurrentLanguage,
                "/",
                type
            });
        }
        public static void LocalizeTextExport(string path, string outpath, string outname)
        {
            string text = Resources.Load<TextAsset>(path).text;
            Directory.CreateDirectory(outpath);
            File.WriteAllText(outpath + "/" + outname + ".txt", text);
        }
        public static void LocalizeTextExport_str(string str, string outpath, string outname)
        {
            Directory.CreateDirectory(outpath);
            File.WriteAllText(outpath + "/" + outname + ".txt", str);
        }
        public static void ExportOriginalFiles()
        {
            try
            {
                string _currentLanguage = TextDataModel.CurrentLanguage ?? "cn";
                TextAsset textAsset = Resources.Load<TextAsset>(Harmony_Patch.BuildPath(new string[]
                {
                    "LocalizeList"
                }));
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(textAsset.text);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("localize_list");
                XmlNodeList xmlNodeList = xmlNode.SelectNodes("language");
                List<string> list = new List<string>();
                foreach (object obj in xmlNodeList)
                {
                    string innerText = ((XmlNode)obj).InnerText;
                    list.Add(innerText);
                }
                XmlNodeList xmlNodeList2 = xmlNode.SelectNodes("localize");
                List<string> list2 = new List<string>();
                foreach (object obj2 in xmlNodeList2)
                {
                    string innerText2 = ((XmlNode)obj2).InnerText;
                    list2.Add(innerText2);
                }
                if (!list.Contains(_currentLanguage))
                {
                    Debug.LogError(string.Format("Not supported language {0}", _currentLanguage));
                    return;
                }
                foreach (string text in list2)
                {
                    ExportOringalLocalizeFile(Harmony_Patch.BuildPath(new string[]
                    {
                        "Localize/",
                        _currentLanguage,
                        "/",
                        _currentLanguage,
                        "_",
                        text
                    }));
                }
                ExportOthers(_currentLanguage);
            }
            catch (Exception ex)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                File.WriteAllText(Application.dataPath + "/Mods/ELerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        public static void ExportOringalLocalizeFile(string path)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(path);
            LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/etc", textAsset.name);
        }
        public static void ExportOthers(string language)
        {
            ExportBattleDialogues(language);
            ExportBattleDialoguesRelations();
            ExportCharactersName(language);
            ExportLibrariansName(language);
            ExportStageName(language);
            ExportPassiveDesc(language);
            ExportGiftDesc(language);
            ExportBattleCardDescriptions(language);
            ExportBattleCardAbilityDescriptions(language);
            ExportBattleEffectTexts(language);
            ExportAbnormalityCardDescriptions(language);
            ExportAbnormalityAbilityDescription(language);
            ExportBookDescriptions(language);
            ExportOpeningLyrics(language);
            ExportEndingLyrics(language);
            ExportBossBirdText(language);
            ExportWhiteNightText(language);
        }
        private static void ExportBattleDialogues(string language)
        {
            TextAsset[] array = Resources.LoadAll<TextAsset>("Xml/BattleDialogues/" + language + "/");
            for (int i = 0; i < array.Length; i++)
            {
                LocalizeTextExport_str(array[i].text, Harmony_Patch.LocalizePath + "/BattleDialogues", array[i].name);
            }
        }
        private static void ExportBattleDialoguesRelations()
        {
            LocalizeTextExport("Xml/BattleDialogues/Book_BattleDlg_Relations", Harmony_Patch.LocalizePath + "/Book_BattleDlg_Relations", "Book_BattleDlg_Relations");
        }
        private static void ExportCharactersName(string language)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_CharactersName", language));
            LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/CharactersName", "CharactersName");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_CreatureName", language), Harmony_Patch.LocalizePath + "/CharactersName", "CreatureName");
        }
        private static void ExportLibrariansName(string language)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_NormalLibrariansNamePreset", language));
            LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/NormalLibrariansNamePreset", "NormalLibrariansNamePreset");
        }
        private static void ExportStageName(string language)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_StageName", language));
            LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/StageName", "StageName");
        }
        private static void ExportPassiveDesc(string language)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_PassiveDesc", language));
            LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/PassiveDesc", "PassiveDesc");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_CreaturePassive", language), Harmony_Patch.LocalizePath + "/PassiveDesc", "CreaturePassive");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_CreaturePassive_Final", language), Harmony_Patch.LocalizePath + "/PassiveDesc", "CreaturePassive_Final");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_PassiveDesc_Ch7_Eileen", language), Harmony_Patch.LocalizePath + "/PassiveDesc", "PassiveDesc_Ch7_Eileen");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_PassiveDesc_Ch7_Jaeheon", language), Harmony_Patch.LocalizePath + "/PassiveDesc", "PassiveDesc_Ch7_Jaeheon");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_PassiveDesc_Ch7_Oswald", language), Harmony_Patch.LocalizePath + "/PassiveDesc", "PassiveDesc_Ch7_Oswald");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_PassiveDesc_Ch7_Elena", language), Harmony_Patch.LocalizePath + "/PassiveDesc", "PassiveDesc_Ch7_Elena");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_PassiveDesc_Ch7_Argalia", language), Harmony_Patch.LocalizePath + "/PassiveDesc", "PassiveDesc_Ch7_Argalia");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_PassiveDesc_Ch7_Tanya", language), Harmony_Patch.LocalizePath + "/PassiveDesc", "PassiveDesc_Ch7_Tanya");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_PassiveDesc_Ch7_Pluto", language), Harmony_Patch.LocalizePath + "/PassiveDesc", "PassiveDesc_Ch7_Pluto");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_PassiveDesc_Ch7_Greta", language), Harmony_Patch.LocalizePath + "/PassiveDesc", "PassiveDesc_Ch7_Greta");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_PassiveDesc_Ch7_Philip", language), Harmony_Patch.LocalizePath + "/PassiveDesc", "PassiveDesc_Ch7_Philip");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_PassiveDesc_Ch7_Bremen", language), Harmony_Patch.LocalizePath + "/PassiveDesc", "PassiveDesc_Ch7_Bremen");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_PassiveDesc_Ch7_BandFinal", language), Harmony_Patch.LocalizePath + "/PassiveDesc", "PassiveDesc_Ch7_BandFinal");
        }
        private static void ExportGiftDesc(string language)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_GiftTexts", language));
            LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/GiftTexts", "GiftTexts");
        }
        private static void ExportBattleCardDescriptions(string language)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{1}_BattleCards", language, language));
            LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/BattlesCards", "BattleCards");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCards_Creature", language, language), Harmony_Patch.LocalizePath + "/BattlesCards", "BattleCards_Creature");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCards_Creature_Final", language, language), Harmony_Patch.LocalizePath + "/BattlesCards", "BattleCards_Creature_Final");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCards_Ch7_Eileen", language, language), Harmony_Patch.LocalizePath + "/BattlesCards", "BattleCards_Ch7_Eileen");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCards_Ch7_Jaeheon", language, language), Harmony_Patch.LocalizePath + "/BattlesCards", "BattleCards_Ch7_Jaeheon");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCards_Ch7_Elena", language, language), Harmony_Patch.LocalizePath + "/BattlesCards", "BattleCards_Ch7_Elena");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCards_Ch7_Argalia", language, language), Harmony_Patch.LocalizePath + "/BattlesCards", "BattleCards_Ch7_Argalia");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCards_Ch7_Tanya", language, language), Harmony_Patch.LocalizePath + "/BattlesCards", "BattleCards_Ch7_Tanya");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCards_Ch7_Oswald", language, language), Harmony_Patch.LocalizePath + "/BattlesCards", "BattleCards_Ch7_Oswald");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCards_Ch7_Pluto", language, language), Harmony_Patch.LocalizePath + "/BattlesCards", "BattleCards_Ch7_Pluto");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCards_Ch7_Greta", language, language), Harmony_Patch.LocalizePath + "/BattlesCards", "BattleCards_Ch7_Greta");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCards_Ch7_Philip", language, language), Harmony_Patch.LocalizePath + "/BattlesCards", "BattleCards_Ch7_Philip");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCards_Ch7_Bremen", language, language), Harmony_Patch.LocalizePath + "/BattlesCards", "BattleCards_Ch7_Bremen");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCards_Ch7_BandFinal", language, language), Harmony_Patch.LocalizePath + "/BattlesCards", "BattleCards_Ch7_BandFinal");
        }
        private static void ExportBattleCardAbilityDescriptions(string language)
        {
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCardAbilities", language, language), Harmony_Patch.LocalizePath + "/BattleCardAbilities", "BattleCardAbilities");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCardAbilities_elena", language, language), Harmony_Patch.LocalizePath + "/BattleCardAbilities", "BattleCardAbilities_elena");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCardAbilities_Eileen", language, language), Harmony_Patch.LocalizePath + "/BattleCardAbilities", "BattleCardAbilities_Eileen");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCardAbilities_Argalia", language, language), Harmony_Patch.LocalizePath + "/BattleCardAbilities", "BattleCardAbilities_Argalia");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCardAbilities_Tanya", language, language), Harmony_Patch.LocalizePath + "/BattleCardAbilities", "BattleCardAbilities_Tanya");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCardAbilities_BandFinal", language, language), Harmony_Patch.LocalizePath + "/BattleCardAbilities", "BattleCardAbilities_BandFinal");
        }
        private static void ExportBattleEffectTexts(string language)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_EffectTexts", language));
            LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/EffectTexts", "EffectTexts");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_EffectTexts_Ch7_Jaeheon", language, language), Harmony_Patch.LocalizePath + "/EffectTexts", "EffectTexts_Ch7_Jaeheon");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_EffectTexts_Ch7_Oswald", language, language), Harmony_Patch.LocalizePath + "/EffectTexts", "EffectTexts_Ch7_Oswald");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_EffectTexts_Ch7_Pluto", language, language), Harmony_Patch.LocalizePath + "/EffectTexts", "EffectTexts_Ch7_Pluto");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_EffectTexts_Ch7_Greta", language, language), Harmony_Patch.LocalizePath + "/EffectTexts", "EffectTexts_Ch7_Greta");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_EffectTexts_Ch7_Philip", language, language), Harmony_Patch.LocalizePath + "/EffectTexts", "EffectTexts_Ch7_Philip");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_EffectTexts_Ch7_Bremen", language, language), Harmony_Patch.LocalizePath + "/EffectTexts", "EffectTexts_Ch7_Bremen");
            LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_EffectTexts_Ch7_BandFinal", language, language), Harmony_Patch.LocalizePath + "/EffectTexts", "EffectTexts_Ch7_BandFinal");
        }
        private static void ExportAbnormalityCardDescriptions(string language)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_AbnormalityCards", language));
            LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/AbnormalityCards", "AbnormalityCards");
        }
        private static void ExportAbnormalityAbilityDescription(string language)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_AbnormalityAbilities", language));
            LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/AbnormalityAbilities", "AbnormalityAbilities");
        }
        private static void ExportBookDescriptions(string language)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_Books", language));
            LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/Books", "Books");
        }
        private static void ExportOpeningLyrics(string language)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_OpeningLyrics", language));
            LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/OpeningLyrics", "_OpeningLyrics");
        }
        private static void ExportEndingLyrics(string language)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_EndingLyrics", language));
            LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/EndingLyrics", "EndingLyrics");
        }
        private static void ExportBossBirdText(string language)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_Bossbird", language));
            LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/BossBirdText", "BossBirdText");
        }
        private static void ExportWhiteNightText(string language)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_WhiteNight", language));
            LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/WhiteNightText", "WhiteNightText");
        }
        public static void LoadModFiles(List<ModContent> loadedContents)
        {
            Dictionary<string, LOR_XML.BattleDialogRoot> BattleDialogues = Singleton<BattleDialogXmlList>.Instance._dictionary;
            List<BattleDialogRelationWithBookID> BattleDialoguesRelations = Singleton<BattleDialogXmlList>.Instance._relationList;
            Dictionary<int, string> CharactersName = Singleton<CharactersNameXmlList>.Instance._dictionary;
            Dictionary<int, string> LibrariansName = Singleton<LibrariansNameXmlList>.Instance._dictionary;
            Dictionary<int, string> StageName = Singleton<StageNameXmlList>.Instance._dictionary;
            Dictionary<LorId, PassiveDesc> PassiveDesc = Singleton<PassiveDescXmlList>.Instance._dictionary;
            Dictionary<string, GiftText> GiftDesc = Singleton<GiftDescXmlList>.Instance._dictionary;
            Dictionary<LorId, BattleCardDesc> BattleCardDesc = Singleton<BattleCardDescXmlList>.Instance._dictionary;
            Dictionary<string, BattleCardAbilityDesc> BattleCardAbilityDesc = Singleton<BattleCardAbilityDescXmlList>.Instance._dictionary;
            Dictionary<string, BattleEffectText> BattleEffectTexts = Singleton<BattleEffectTextsXmlList>.Instance._dictionary;
            Dictionary<string, AbnormalityCard> AbnormalityCardDesc = Singleton<AbnormalityCardDescXmlList>.Instance._dictionary;
            Dictionary<string, AbnormalityAbilityText> AbnormalityAbilityText = Singleton<AbnormalityAbilityTextXmlList>.Instance._dictionary;
            Dictionary<string, List<BookDesc>> BookDesc = Singleton<BookDescXmlList>.Instance._dictionaryWorkshop;
            Dictionary<int, BookDesc> BookDescOrigin = Singleton<BookDescXmlList>.Instance._dictionaryOrigin;

            foreach (ModContent modcontent in loadedContents)
            {
                DirectoryInfo _dirInfo = modcontent._dirInfo;
                ModContentInfo _modInfo = modcontent._modInfo;
                string workshopId = _modInfo.invInfo.workshopInfo.uniqueId;
                if (workshopId.ToLower().EndsWith("@origin"))
                {
                    workshopId = "";
                }
                try
                {
                    LoadLocalizeFile_MOD(_dirInfo);
                    LoadBattleDialogues_Relations_MOD(_dirInfo, BattleDialoguesRelations, BattleDialogues, workshopId);
                    LoadBattleDialogues_MOD(_dirInfo, BattleDialogues, workshopId);
                    LoadCharactersName_MOD(_dirInfo, CharactersName, workshopId);
                    LoadLibrariansName_MOD(_dirInfo, LibrariansName);
                    LoadStageName_MOD(_dirInfo, StageName, workshopId);
                    LoadPassiveDesc_MOD(_dirInfo, PassiveDesc, workshopId);
                    LoadGiftDesc_MOD(_dirInfo, GiftDesc);
                    LoadBattleCardDescriptions_MOD(_dirInfo, BattleCardDesc, workshopId);
                    LoadBattleCardAbilityDescriptions_MOD(_dirInfo, BattleCardAbilityDesc);
                    LoadBattleEffectTexts_MOD(_dirInfo, BattleEffectTexts);
                    LoadAbnormalityCardDescriptions_MOD(_dirInfo, AbnormalityCardDesc);
                    LoadAbnormalityAbilityDescription_MOD(_dirInfo, AbnormalityAbilityText);
                    LoadBookDescriptions_MOD(_dirInfo, BookDesc, BookDescOrigin, workshopId);
                }
                catch (Exception ex)
                {
                    Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                    File.WriteAllText(Application.dataPath + "/Mods/" + _dirInfo.Name + "_LTLerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }
        }
        public static void LoadLocalizeFile_MOD(DirectoryInfo dir)
        {
            string moddingPath = GetModdingPath(dir, "etc");
            if (Directory.Exists(moddingPath))
            {
                LoadLocalizeFile_MOD_Checking(new DirectoryInfo(moddingPath), TextDataModel.textDic);
            }
        }
        public static void LoadLocalizeFile_MOD_Checking(DirectoryInfo dir, Dictionary<string, string> dic)
        {
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(File.ReadAllText(fileInfo.FullName));
                foreach (object obj in xmlDocument.SelectNodes("localize/text"))
                {
                    XmlNode xmlNode = (XmlNode)obj;
                    string text = string.Empty;
                    if (xmlNode.Attributes.GetNamedItem("id") != null)
                    {
                        text = xmlNode.Attributes.GetNamedItem("id").InnerText;
                    }
                    string key = text;
                    string innerText = xmlNode.InnerText;
                    dic[key] = innerText;
                }
            }
        }
        private static void LoadBattleDialogues_MOD(DirectoryInfo dir, Dictionary<string, LOR_XML.BattleDialogRoot> dic, string uniqueId)
        {
            string moddingPath = GetModdingPath(dir, "BattleDialogues");
            if (Directory.Exists(moddingPath))
            {
                LoadBattleDialogues_MOD_Checking(new DirectoryInfo(moddingPath), dic, uniqueId);
            }
        }
        private static void LoadBattleDialogues_MOD_Checking(DirectoryInfo dir, Dictionary<string, LOR_XML.BattleDialogRoot> dic, string uniqueId)
        {
            Dictionary<string, LOR_XML.BattleDialogRoot> dictionary = new Dictionary<string, LOR_XML.BattleDialogRoot>();
            List<BattleDialogCharacter> list = new List<BattleDialogCharacter>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
                {
                    GTMDProjectMoon.BattleDialogRoot battleDialogRoot = (GTMDProjectMoon.BattleDialogRoot)new XmlSerializer(typeof(GTMDProjectMoon.BattleDialogRoot)).Deserialize(stringReader);
                    dictionary.Add(battleDialogRoot.groupName, new LOR_XML.BattleDialogRoot().CopyBattleDialogRoot(battleDialogRoot, uniqueId));
                    list.AddRange(dictionary[battleDialogRoot.groupName].characterList);
                }
            }
            foreach (KeyValuePair<string, LOR_XML.BattleDialogRoot> keyValuePair in dictionary)
            {
                dic[keyValuePair.Key] = keyValuePair.Value;
            }
            Singleton<BattleDialogXmlList>.Instance.AddDialogByMod(list);
        }
        private static void LoadBattleDialogues_Relations_MOD(DirectoryInfo dir, List<BattleDialogRelationWithBookID> _relationList, Dictionary<string, LOR_XML.BattleDialogRoot> _dictionary, string uniqueId)
        {
            string moddingPath = GetModdingPath(dir, "Book_BattleDlg_Relations");
            if (Directory.Exists(moddingPath))
            {
                LoadBattleDialogues_Relations_MOD_Checking(new DirectoryInfo(moddingPath), _relationList, _dictionary, uniqueId);
            }
        }
        private static void LoadBattleDialogues_Relations_MOD_Checking(DirectoryInfo dir, List<BattleDialogRelationWithBookID> _relationList, Dictionary<string, LOR_XML.BattleDialogRoot> _dictionary, string uniqueId)
        {
            List<BattleDialogRelationWithBookID> list = new List<BattleDialogRelationWithBookID>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
                {
                    List<BattleDialogRelationWithBookID_New> list2 = ((GTMDProjectMoon.BattleDialogRelationRoot)new XmlSerializer(typeof(GTMDProjectMoon.BattleDialogRelationRoot)).Deserialize(stringReader)).list;
                    list.AddRange(new List<BattleDialogRelationWithBookID>().CopyBattleDialogRelation(list2, uniqueId));
                }
            }
            foreach (BattleDialogRelationWithBookID battleDialogRelationWithBookID in list)
            {
                bool flag = false;
                BattleDialogRelationWithBookID item = null;
                foreach (BattleDialogRelationWithBookID battleDialogRelationWithBookID2 in _relationList)
                {
                    if (battleDialogRelationWithBookID2.groupName == battleDialogRelationWithBookID.groupName && battleDialogRelationWithBookID2.bookID == battleDialogRelationWithBookID.bookID)
                    {
                        flag = true;
                        item = battleDialogRelationWithBookID2;
                        break;
                    }
                }
                if (flag)
                {
                    _relationList.Remove(item);
                }
                _relationList.Add(battleDialogRelationWithBookID);
            }
        }
        private static void LoadCharactersName_MOD(DirectoryInfo dir, Dictionary<int, string> dic, string uniqueId)
        {
            string moddingPath = GetModdingPath(dir, "CharactersName");
            if (Directory.Exists(moddingPath))
            {
                LoadCharactersName_MOD_Checking(new DirectoryInfo(moddingPath), dic, uniqueId);
            }
        }
        private static void LoadCharactersName_MOD_Checking(DirectoryInfo dir, Dictionary<int, string> dic, string uniqueId)
        {
            List<CharacterName> list = new List<CharacterName>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
                {
                    CharactersNameRoot charactersNameRoot = (CharactersNameRoot)new XmlSerializer(typeof(CharactersNameRoot)).Deserialize(stringReader);
                    list.AddRange(charactersNameRoot.nameList);
                }
            }
            foreach (CharacterName characterName in list)
            {
                if (uniqueId == "")
                {
                    dic[characterName.ID] = characterName.name;
                }
                else
                {
                    OrcTools.CharacterNameDic[new LorId(uniqueId, characterName.ID)] = characterName.name;
                }
            }
        }
        private static void LoadLibrariansName_MOD(DirectoryInfo dir, Dictionary<int, string> dic)
        {
            string moddingPath = GetModdingPath(dir, "NormalLibrariansNamePreset");
            if (Directory.Exists(moddingPath))
            {
                LoadLibrariansName_MOD_Checking(new DirectoryInfo(moddingPath), dic);
            }
        }
        private static void LoadLibrariansName_MOD_Checking(DirectoryInfo dir, Dictionary<int, string> dic)
        {
            List<CharacterName> list = new List<CharacterName>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
                {
                    CharactersNameRoot charactersNameRoot = (CharactersNameRoot)new XmlSerializer(typeof(CharactersNameRoot)).Deserialize(stringReader);
                    list.AddRange(charactersNameRoot.nameList);
                }
            }
            foreach (CharacterName characterName in list)
            {
                dic[characterName.ID] = characterName.name;
            }
        }
        private static void LoadStageName_MOD(DirectoryInfo dir, Dictionary<int, string> dic, string uniqueId)
        {
            string moddingPath = GetModdingPath(dir, "StageName");
            if (Directory.Exists(moddingPath))
            {
                LoadStageName_MOD_Checking(new DirectoryInfo(moddingPath), dic, uniqueId);
            }
        }
        private static void LoadStageName_MOD_Checking(DirectoryInfo dir, Dictionary<int, string> dic, string uniqueId)
        {
            List<CharacterName> list = new List<CharacterName>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
                {
                    CharactersNameRoot charactersNameRoot = (CharactersNameRoot)new XmlSerializer(typeof(CharactersNameRoot)).Deserialize(stringReader);
                    list.AddRange(charactersNameRoot.nameList);
                }
            }
            foreach (CharacterName characterName in list)
            {
                if (uniqueId == "")
                {
                    dic[characterName.ID] = characterName.name;
                    OrcTools.StageNameDic[new LorId(uniqueId, characterName.ID)] = characterName.name;
                }
                else
                {
                    OrcTools.StageNameDic[new LorId(uniqueId, characterName.ID)] = characterName.name;
                }
            }
        }
        private static void LoadPassiveDesc_MOD(DirectoryInfo dir, Dictionary<LorId, PassiveDesc> dic, string uniqueId)
        {
            string moddingPath = GetModdingPath(dir, "PassiveDesc");
            if (Directory.Exists(moddingPath))
            {
                LoadPassiveDesc_MOD_Checking(new DirectoryInfo(moddingPath), dic, uniqueId);
            }
        }
        private static void LoadPassiveDesc_MOD_Checking(DirectoryInfo dir, Dictionary<LorId, PassiveDesc> dic, string uniqueId)
        {
            List<PassiveDesc> list = new List<PassiveDesc>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
                {
                    PassiveDescRoot passiveDescRoot = (PassiveDescRoot)new XmlSerializer(typeof(PassiveDescRoot)).Deserialize(stringReader);
                    list.AddRange(passiveDescRoot.descList);
                }
            }
            foreach (PassiveDesc passiveDesc in list)
            {
                dic[new LorId(uniqueId, passiveDesc._id)] = passiveDesc;
            }
        }
        private static void LoadGiftDesc_MOD(DirectoryInfo dir, Dictionary<string, GiftText> dic)
        {
            string moddingPath = GetModdingPath(dir, "GiftTexts");
            if (Directory.Exists(moddingPath))
            {
                LoadGiftDesc_MOD_Checking(new DirectoryInfo(moddingPath), dic);
            }
        }
        private static void LoadGiftDesc_MOD_Checking(DirectoryInfo dir, Dictionary<string, GiftText> dic)
        {
            List<GiftText> list = new List<GiftText>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
                {
                    GiftTextRoot giftTextRoot = (GiftTextRoot)new XmlSerializer(typeof(GiftTextRoot)).Deserialize(stringReader);
                    list.AddRange(giftTextRoot.giftList);
                }
            }
            foreach (GiftText giftText in list)
            {
                dic[giftText.id] = giftText;
            }
        }
        private static void LoadBattleCardDescriptions_MOD(DirectoryInfo dir, Dictionary<LorId, BattleCardDesc> dic, string uniqueId)
        {
            string moddingPath = GetModdingPath(dir, "BattlesCards");
            if (Directory.Exists(moddingPath))
            {
                LoadBattleCardDescriptions_MOD_Checking(new DirectoryInfo(moddingPath), dic, uniqueId);
            }
        }
        private static void LoadBattleCardDescriptions_MOD_Checking(DirectoryInfo dir, Dictionary<LorId, BattleCardDesc> dic, string uniqueId)
        {
            Dictionary<int, BattleCardDesc> dictionary = new Dictionary<int, BattleCardDesc>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
                {
                    BattleCardDescRoot battleCardDescRoot = (BattleCardDescRoot)new XmlSerializer(typeof(BattleCardDescRoot)).Deserialize(stringReader);
                    for (int j = 0; j < battleCardDescRoot.cardDescList.Count; j++)
                    {
                        BattleCardDesc battleCardDesc = battleCardDescRoot.cardDescList[j];
                        dictionary.Add(battleCardDesc.cardID, battleCardDesc);
                    }
                }
            }
            foreach (KeyValuePair<int, BattleCardDesc> keyValuePair in dictionary)
            {
                dic[new LorId(uniqueId, keyValuePair.Key)] = keyValuePair.Value;
            }
        }
        private static void LoadBattleCardAbilityDescriptions_MOD(DirectoryInfo dir, Dictionary<string, BattleCardAbilityDesc> root)
        {
            string moddingPath = GetModdingPath(dir, "BattleCardAbilities");
            if (Directory.Exists(moddingPath))
            {
                LoadBattleCardAbilityDescriptions_MOD_Checking(new DirectoryInfo(moddingPath), root);
            }
        }
        private static void LoadBattleCardAbilityDescriptions_MOD_Checking(DirectoryInfo dir, Dictionary<string, BattleCardAbilityDesc> root)
        {
            Dictionary<string, BattleCardAbilityDesc> dictionary = new Dictionary<string, BattleCardAbilityDesc>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
                {
                    BattleCardAbilityDescRoot battleCardAbilityDescRoot = (BattleCardAbilityDescRoot)new XmlSerializer(typeof(BattleCardAbilityDescRoot)).Deserialize(stringReader);
                    for (int j = 0; j < battleCardAbilityDescRoot.cardDescList.Count; j++)
                    {
                        BattleCardAbilityDesc battleCardAbilityDesc = battleCardAbilityDescRoot.cardDescList[j];
                        dictionary.Add(battleCardAbilityDesc.id, battleCardAbilityDesc);
                    }
                }
            }
            foreach (KeyValuePair<string, BattleCardAbilityDesc> keyValuePair in dictionary)
            {
                root[keyValuePair.Key] = keyValuePair.Value;
            }
        }
        private static void LoadBattleEffectTexts_MOD(DirectoryInfo dir, Dictionary<string, BattleEffectText> root)
        {
            string moddingPath = GetModdingPath(dir, "EffectTexts");
            if (Directory.Exists(moddingPath))
            {
                LoadBattleEffectTexts_MOD_Checking(new DirectoryInfo(moddingPath), root);
            }
        }
        private static void LoadBattleEffectTexts_MOD_Checking(DirectoryInfo dir, Dictionary<string, BattleEffectText> root)
        {
            Dictionary<string, BattleEffectText> dictionary = new Dictionary<string, BattleEffectText>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
                {
                    BattleEffectTextRoot battleEffectTextRoot = (BattleEffectTextRoot)new XmlSerializer(typeof(BattleEffectTextRoot)).Deserialize(stringReader);
                    for (int j = 0; j < battleEffectTextRoot.effectTextList.Count; j++)
                    {
                        BattleEffectText battleEffectText = battleEffectTextRoot.effectTextList[j];
                        dictionary.Add(battleEffectText.ID, battleEffectText);
                    }
                }
            }
            foreach (KeyValuePair<string, BattleEffectText> keyValuePair in dictionary)
            {
                root[keyValuePair.Key] = keyValuePair.Value;
            }
        }
        private static void LoadAbnormalityCardDescriptions_MOD(DirectoryInfo dir, Dictionary<string, AbnormalityCard> root)
        {
            string moddingPath = GetModdingPath(dir, "AbnormalityCards");
            if (Directory.Exists(moddingPath))
            {
                LoadAbnormalityCardDescriptions_MOD_Checking(new DirectoryInfo(moddingPath), root);
            }
        }
        private static void LoadAbnormalityCardDescriptions_MOD_Checking(DirectoryInfo dir, Dictionary<string, AbnormalityCard> root)
        {
            Dictionary<string, AbnormalityCard> dictionary = new Dictionary<string, AbnormalityCard>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
                {
                    foreach (Sephirah sephirah in ((AbnormalityCardsRoot)new XmlSerializer(typeof(AbnormalityCardsRoot)).Deserialize(stringReader)).sephirahList)
                    {
                        foreach (AbnormalityCard abnormalityCard in sephirah.list)
                        {
                            dictionary.Add(abnormalityCard.id, abnormalityCard);
                        }
                    }
                }
            }
            foreach (KeyValuePair<string, AbnormalityCard> keyValuePair in dictionary)
            {
                root[keyValuePair.Key] = keyValuePair.Value;
            }
        }
        private static void LoadAbnormalityAbilityDescription_MOD(DirectoryInfo dir, Dictionary<string, AbnormalityAbilityText> root)
        {
            string moddingPath = GetModdingPath(dir, "AbnormalityAbilities");
            if (Directory.Exists(moddingPath))
            {
                LoadAbnormalityAbilityDescription_MOD_Checking(new DirectoryInfo(moddingPath), root);
            }
        }
        private static void LoadAbnormalityAbilityDescription_MOD_Checking(DirectoryInfo dir, Dictionary<string, AbnormalityAbilityText> root)
        {
            Dictionary<string, AbnormalityAbilityText> dictionary = new Dictionary<string, AbnormalityAbilityText>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
                {
                    foreach (AbnormalityAbilityText abnormalityAbilityText in ((AbnormalityAbilityRoot)new XmlSerializer(typeof(AbnormalityAbilityRoot)).Deserialize(stringReader)).abnormalityList)
                    {
                        dictionary.Add(abnormalityAbilityText.id, abnormalityAbilityText);
                    }
                }
            }
            foreach (KeyValuePair<string, AbnormalityAbilityText> keyValuePair in dictionary)
            {
                root[keyValuePair.Key] = keyValuePair.Value;
            }
        }
        private static void LoadBookDescriptions_MOD(DirectoryInfo dir, Dictionary<string, List<BookDesc>> _dic, Dictionary<int, BookDesc> BookDescOrigin, string uniqueId)
        {
            string moddingPath = GetModdingPath(dir, "Books");
            if (Directory.Exists(moddingPath))
            {
                LoadBookDescriptions_MOD_Checking(new DirectoryInfo(moddingPath), _dic, BookDescOrigin, uniqueId);
            }
        }
        private static void LoadBookDescriptions_MOD_Checking(DirectoryInfo dir, Dictionary<string, List<BookDesc>> _dic, Dictionary<int, BookDesc> BookDescOrigin, string uniqueId)
        {
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
                {
                    BookDescRoot bookDescRoot = (BookDescRoot)new XmlSerializer(typeof(BookDescRoot)).Deserialize(stringReader);
                    if (uniqueId == "")
                    {
                        foreach (BookDesc bookDesc in bookDescRoot.bookDescList)
                        {
                            BookDescOrigin[bookDesc.bookID] = bookDesc;
                        }
                    }
                    else if (_dic.ContainsKey(uniqueId))
                    {
                        foreach (BookDesc bookDesc in bookDescRoot.bookDescList)
                        {
                            _dic[uniqueId].Remove(_dic[uniqueId].Find((BookDesc x) => x.bookID == bookDesc.bookID));
                            _dic[uniqueId].Add(bookDesc);
                        }
                    }
                    else
                    {
                        _dic[uniqueId] = bookDescRoot.bookDescList;
                    }
                }
            }
        }
    }
}
