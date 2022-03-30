using Battle.DiceAttackEffect;
using ExtendedLoader;
using GameSave;
using GTMDProjectMoon;
using HarmonyLib;
using LOR_DiceSystem;
using LOR_XML;
using Mod;
using ModSettingTool;
using Opening;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace BaseMod
{
    public static class Harmony_Patch
    {
        static Harmony_Patch()
        {
        }
        public static string StaticPath
        {
            get
            {
                Staticpath = Directory.CreateDirectory(Application.dataPath + "/Managed/BaseMod/StaticInfo").FullName;
                return Staticpath;
            }
        }
        public static string LocalizePath
        {
            get
            {
                Localizepath = Directory.CreateDirectory(Application.dataPath + "/Managed/BaseMod/Localize/" + TextDataModel.CurrentLanguage).FullName;
                return Localizepath;
            }
        }
        public static string StoryPath_Static
        {
            get
            {
                StoryStaticpath = Directory.CreateDirectory(Application.dataPath + "/Managed/BaseMod/Story/EffectInfo").FullName;
                return StoryStaticpath;
            }
        }
        public static string StoryPath_Localize
        {
            get
            {
                Storylocalizepath = Directory.CreateDirectory(Application.dataPath + "/Managed/BaseMod/Story/Localize/" + TextDataModel.CurrentLanguage).FullName;
                return Storylocalizepath;
            }
        }
        public static List<ModContent> LoadedModContents
        {
            get
            {
                return Singleton<ModContentManager>.Instance._loadedContents;
            }
        }
        private static bool VoidPre()
        {
            try
            {
                return false;
            }
            catch (Exception ex)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                File.WriteAllText(Application.dataPath + "/Mods/error.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        public static void Init()
        {
            try
            {
                path = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
                IsEditing = false;
                AssemList = new List<Assembly>();
                LoadedAssembly = new List<string>();
                ArtWorks = null;
                BookThumb = null;
                AudioClips = null;
                CustomEffects = new Dictionary<string, Type>();
                CustomMapManager = new Dictionary<string, Type>();
                CustomBattleDialogModel = new Dictionary<string, Type>();
                CustomGiftPassive = new Dictionary<string, Type>();
                CustomEmotionCardAbility = new Dictionary<string, Type>();
                ModStoryCG = new Dictionary<LorId, ModStroyCG>();
                ModWorkShopId = new Dictionary<Assembly, string>();
                IsModStorySelected = false;
                //CreateShortcuts();
                ExportDocuments();
            }
            catch (Exception ex)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                File.WriteAllText(Application.dataPath + "/Mods/error.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }/*
        private static void CreateShortcuts()
        {
            string baseModPath = Singleton<ModContentManager>.Instance.GetModPath("BaseMod");
            UtilTools.CreateShortcut(Application.dataPath + "/Managed/BaseMod/", "BaseMod for Workshop", baseModPath, "Way to BaseMod Files");
            UtilTools.CreateShortcut(Application.dataPath + "/Managed/BaseMod/", "SaveFiles", SaveManager.savePath, "Way to BaseMod Files");
        }*/
        private static void ExportDocuments()
        {
            string baseModPath = Singleton<ModContentManager>.Instance.GetModPath("BaseMod");
            if (Directory.Exists(baseModPath + "/Documents/"))
            {
                UtilTools.CopyDir(baseModPath + "/Documents", Application.dataPath + "/Managed/BaseMod/Documents");
            }
        }
        public static void LoadAssemblyFiles()
        {
            ModSaveTool.LoadedModsWorkshopId.Add("BaseMod");
            foreach (ModContent modContent in LoadedModContents)
            {
                DirectoryInfo _dirInfo = modContent._dirInfo;
                Assembly currentAssembly;
                foreach (FileInfo fileInfo in _dirInfo.GetFiles())
                {
                    string errorname = "unknown";
                    try
                    {
                        if (fileInfo.Name.Contains(".dll") && !LoadedAssembly.Contains(fileInfo.FullName))
                        {
                            LoadedAssembly.Add(fileInfo.Directory.FullName);
                            errorname = "LoasAssembly";
                            currentAssembly = Assembly.LoadFile(fileInfo.FullName);
                            errorname = "GetAssemblyTypes";
                            foreach (Type type in currentAssembly.GetTypes())
                            {
                                errorname = "LoadCustomTypes";
                                string name = type.Name;
                                if (type.IsSubclassOf(typeof(DiceAttackEffect)) && name.StartsWith("DiceAttackEffect_"))
                                {
                                    CustomEffects[name.Substring("DiceAttackEffect_".Length).Trim()] = type;
                                }
                                if (type.IsSubclassOf(typeof(CustomMapManager)) && name.EndsWith("MapManager"))
                                {
                                    CustomMapManager[name.Trim()] = type;
                                }
                                if (type.IsSubclassOf(typeof(BattleDialogueModel)) && name.StartsWith("BattleDialogueModel_"))
                                {
                                    CustomBattleDialogModel[name.Substring("BattleDialogueModel_".Length).Trim()] = type;
                                }
                                if (type.IsSubclassOf(typeof(PassiveAbilityBase)) && name.StartsWith("GiftPassiveAbility_"))
                                {
                                    CustomGiftPassive[name.Substring("GiftPassiveAbility_".Length).Trim()] = type;
                                }
                                if (type.IsSubclassOf(typeof(EmotionCardAbilityBase)) && name.StartsWith("EmotionCardAbility_"))
                                {
                                    CustomEmotionCardAbility[name.Substring("EmotionCardAbility_".Length).Trim()] = type;
                                }
                                errorname = "LoadHarmonyPatch";
                                if ((type != null && type.BaseType != null && type.BaseType.Name == "Harmony_Patch") || name == "Harmony_Patch")
                                {
                                    Activator.CreateInstance(type);
                                }
                            }
                            errorname = "LoadOtherTypes";
                            LoadTypesFromAssembly(currentAssembly);
                            ModSaveTool.LoadedModsWorkshopId.Add(Tools.GetModId(currentAssembly));
                            AssemList.Add(currentAssembly);
                        }
                    }
                    catch (Exception ex)
                    {
                        Singleton<ModContentManager>.Instance.AddErrorLog("Load_" + fileInfo.Name + "_" + errorname + "_Error");
                        File.WriteAllText(Application.dataPath + "/Mods/Load_" + fileInfo.Name + "_" + errorname + "_Error" + ".log", ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                }
            }
        }
        private static void LoadTypesFromAssembly(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                string name = type.Name;
                if (type.IsSubclassOf(typeof(DiceCardSelfAbilityBase)) && name.StartsWith("DiceCardSelfAbility_"))
                {
                    Singleton<AssemblyManager>.Instance._diceCardSelfAbilityDict.Add(name.Substring("DiceCardSelfAbility_".Length), type);
                }
                else if (type.IsSubclassOf(typeof(DiceCardAbilityBase)) && name.StartsWith("DiceCardAbility_"))
                {
                    Singleton<AssemblyManager>.Instance._diceCardAbilityDict.Add(name.Substring("DiceCardAbility_".Length), type);
                }
                else if (type.IsSubclassOf(typeof(BehaviourActionBase)) && name.StartsWith("BehaviourAction_"))
                {
                    Singleton<AssemblyManager>.Instance._behaviourActionDict.Add(name.Substring("BehaviourAction_".Length), type);
                }
                else if (type.IsSubclassOf(typeof(PassiveAbilityBase)) && name.StartsWith("PassiveAbility_"))
                {
                    Singleton<AssemblyManager>.Instance._passiveAbilityDict.Add(name.Substring("PassiveAbility_".Length), type);
                }
                else if (type.IsSubclassOf(typeof(DiceCardPriorityBase)) && name.StartsWith("DiceCardPriority_"))
                {
                    Singleton<AssemblyManager>.Instance._diceCardPriorityDict.Add(name.Substring("DiceCardPriority_".Length), type);
                }
                else if (type.IsSubclassOf(typeof(EnemyUnitAggroSetter)) && name.StartsWith("EnemyUnitAggroSetter_"))
                {
                    Singleton<AssemblyManager>.Instance._enemyUnitAggroSetterDict.Add(name.Substring("EnemyUnitAggroSetter_".Length), type);
                }
                else if (type.IsSubclassOf(typeof(EnemyTeamStageManager)) && name.StartsWith("EnemyTeamStageManager_"))
                {
                    Singleton<AssemblyManager>.Instance._enemyTeamStageManagerDict.Add(name.Substring("EnemyTeamStageManager_".Length), type);
                }
                else if (type.IsSubclassOf(typeof(EnemyUnitTargetSetter)) && name.StartsWith("EnemyUnitTargetSetter_"))
                {
                    Singleton<AssemblyManager>.Instance._enemyUnitTargetSetterDict.Add(name.Substring("EnemyUnitTargetSetter_".Length), type);
                }
                else if (type.IsSubclassOf(typeof(ModInitializer)))
                {
                    ModInitializer modInitializer = Activator.CreateInstance(type) as ModInitializer;
                    modInitializer.OnInitializeMod();
                }
            }
        }
        public static void LoadModFiles()
        {
            Dictionary<string, List<Workshop.WorkshopSkinData>> _bookSkinData = Singleton<CustomizingBookSkinLoader>.Instance._bookSkinData;
            StaticDataLoader_New.ExportOriginalFiles();
            StaticDataLoader_New.LoadModFiles(LoadedModContents);
            LocalizedTextLoader_New.ExportOriginalFiles();
            LocalizedTextLoader_New.LoadModFiles(LoadedModContents);
            StorySerializer_new.ExportStory();
            LoadBookSkins(_bookSkinData);
        }
        private static void ReloadModFiles()
        {
            GlobalGameManager.Instance.LoadStaticData();
            Singleton<StageClassInfoList>.Instance.GetAllWorkshopData().Clear();
            Singleton<EnemyUnitClassInfoList>.Instance.GetAllWorkshopData().Clear();
            Singleton<BookXmlList>.Instance.GetAllWorkshopData().Clear();
            Singleton<CardDropTableXmlList>.Instance.GetAllWorkshopData().Clear();
            Singleton<DropBookXmlList>.Instance.GetAllWorkshopData().Clear();
            ItemXmlDataList.instance.GetAllWorkshopData().Clear();
            Singleton<BookDescXmlList>.Instance._dictionaryWorkshop.Clear();
            Singleton<CustomizingCardArtworkLoader>.Instance._artworkData.Clear();
            Singleton<CustomizingBookSkinLoader>.Instance._bookSkinData.Clear();
            ArtWorks = null;
            CustomEffects.Clear();
            CustomMapManager.Clear();
            CustomBattleDialogModel.Clear();
            CustomGiftPassive.Clear();
            CustomEmotionCardAbility.Clear();
            LoadedAssembly.Clear();
            ArtWorks.Clear();
            BookThumb.Clear();
            AudioClips.Clear();
            ModPid.Clear();
            ModEpMatch.Clear();
            ModStoryCG = null;
            ModWorkShopId.Clear();
            ModWorkshopBookIndex.Clear();
            OrcTools.OnlyCardDic.Clear();
            OrcTools.SoulCardDic.Clear();
            OrcTools.EpisodeDic.Clear();
            OrcTools.StageNameDic.Clear();
            OrcTools.StageConditionDic.Clear();
            OrcTools.CharacterNameDic.Clear();
            OrcTools.EgoDic.Clear();
            OrcTools.DropItemDic.Clear();
            OrcTools.DialogDic.Clear();
        }
        private static void LoadBookSkins(Dictionary<string, List<Workshop.WorkshopSkinData>> _bookSkinData)
        {
            foreach (ModContent modContent in LoadedModContents)
            {
                string modDirectory = modContent._dirInfo.FullName;
                string modName = modContent._itemUniqueId;
                string modId = "";
                if (!modName.ToLower().EndsWith("@origin"))
                {
                    modId = modName;
                }
                int oldSkins = 0;
                if (Singleton<CustomizingBookSkinLoader>.Instance._bookSkinData.TryGetValue(modName, out List<Workshop.WorkshopSkinData> skinlist))
                {
                    oldSkins = skinlist.Count;
                    string defaultCharDirectory = Path.Combine(modDirectory, "Resource\\CharacterSkin");
                    if (Directory.Exists(defaultCharDirectory))
                    {
                        string[] directories = Directory.GetDirectories(defaultCharDirectory);
                        for (int i = 0; i < directories.Length; i++)
                        {
                            Workshop.WorkshopAppearanceInfo workshopAppearanceInfo = LoadCustomAppearance(directories[i]);
                            if (workshopAppearanceInfo != null && workshopAppearanceInfo is ExtendedWorkshopAppearanceInfo extendedAppearanceInfo)
                            {
                                string[] array = directories[i].Split(new char[]
                                {
                                    '\\'
                                });
                                string bookName = array[array.Length - 1];
                                extendedAppearanceInfo.path = directories[i];
                                extendedAppearanceInfo.uniqueId = modId;
                                extendedAppearanceInfo.bookName = bookName;
                                if (extendedAppearanceInfo.isClothCustom)
                                {
                                    var extendedData = new ExtendedWorkshopSkinData
                                    {
                                        dic = extendedAppearanceInfo.clothCustomInfo,
                                        dataName = extendedAppearanceInfo.bookName,
                                        contentFolderIdx = extendedAppearanceInfo.uniqueId,
                                        atkEffectPivotDic = extendedAppearanceInfo.atkEffectPivotDic,
                                        specialMotionPivotDic = extendedAppearanceInfo.specialMotionPivotDic,
                                        motionSoundList = extendedAppearanceInfo.motionSoundList,
                                        id = i
                                    };
                                    var oldData = skinlist.Find((Workshop.WorkshopSkinData data) => data.id == i);
                                    if (oldData != null)
                                    {
                                        skinlist.Remove(oldData);
                                    }
                                    skinlist.Add(extendedData);
                                }
                            }
                        }
                    }
                }
                string charDirectory = Path.Combine(modDirectory, "Char");
                List<Workshop.WorkshopSkinData> list = new List<Workshop.WorkshopSkinData>();
                if (Directory.Exists(charDirectory))
                {
                    string[] directories = Directory.GetDirectories(charDirectory);
                    for (int i = 0; i < directories.Length; i++)
                    {
                        Workshop.WorkshopAppearanceInfo workshopAppearanceInfo = LoadCustomAppearance(directories[i]);
                        if (workshopAppearanceInfo != null)
                        {
                            string[] array = directories[i].Split(new char[]
                            {
                                '\\'
                            });
                            string str = array[array.Length - 1];
                            workshopAppearanceInfo.path = directories[i];
                            workshopAppearanceInfo.uniqueId = modId;
                            workshopAppearanceInfo.bookName = "Custom_" + str;
                            bool isClothCustom = workshopAppearanceInfo.isClothCustom;
                            if (isClothCustom)
                            {
                                if (workshopAppearanceInfo is ExtendedWorkshopAppearanceInfo extendedAppearanceInfo)
                                {
                                    list.Add(new ExtendedWorkshopSkinData
                                    {
                                        dic = extendedAppearanceInfo.clothCustomInfo,
                                        dataName = extendedAppearanceInfo.bookName,
                                        contentFolderIdx = extendedAppearanceInfo.uniqueId,
                                        atkEffectPivotDic = extendedAppearanceInfo.atkEffectPivotDic,
                                        specialMotionPivotDic = extendedAppearanceInfo.specialMotionPivotDic,
                                        motionSoundList = extendedAppearanceInfo.motionSoundList,
                                        id = oldSkins + i
                                    });
                                }
                                else
                                {
                                    list.Add(new Workshop.WorkshopSkinData
                                    {
                                        dic = workshopAppearanceInfo.clothCustomInfo,
                                        dataName = workshopAppearanceInfo.bookName,
                                        contentFolderIdx = workshopAppearanceInfo.uniqueId,
                                        id = oldSkins + i
                                    });
                                }
                            }
                        }
                    }
                    if (_bookSkinData.ContainsKey(modId))
                    {
                        _bookSkinData[modId].AddRange(list);
                    }
                    else
                    {
                        _bookSkinData[modId] = list;
                    }
                }
            }
            var loader = Singleton<CustomizingResourceLoader>.Instance;
            var list2 = LoadWorkshopExtendedCustomAppearance();
            foreach (Workshop.WorkshopAppearanceInfo workshopAppearanceInfo in list2)
            {
                if (workshopAppearanceInfo.isClothCustom)
                {
                    Workshop.WorkshopSkinData workshopSkinData;
                    if (workshopAppearanceInfo is ExtendedWorkshopAppearanceInfo extendedAppearanceInfo)
                    {
                        workshopSkinData = new ExtendedWorkshopSkinData
                        {
                            dic = extendedAppearanceInfo.clothCustomInfo,
                            dataName = extendedAppearanceInfo.bookName,
                            contentFolderIdx = extendedAppearanceInfo.uniqueId,
                            atkEffectPivotDic = extendedAppearanceInfo.atkEffectPivotDic,
                            specialMotionPivotDic = extendedAppearanceInfo.specialMotionPivotDic,
                            motionSoundList = extendedAppearanceInfo.motionSoundList
                        };
                    }
                    else
                    {
                        workshopSkinData = new Workshop.WorkshopSkinData
                        {
                            dic = workshopAppearanceInfo.clothCustomInfo,
                            dataName = workshopAppearanceInfo.bookName,
                            contentFolderIdx = workshopAppearanceInfo.uniqueId
                        };
                    }
                    int num = loader._skinData.Count;
                    if (loader._skinData.TryGetValue(workshopAppearanceInfo.uniqueId, out Workshop.WorkshopSkinData workshopSkinData1))
                    {
                        num = workshopSkinData1.id;
                        loader._skinData.Remove(workshopAppearanceInfo.uniqueId);
                    }
                    workshopSkinData.id = num;
                    loader._skinData.Add(workshopAppearanceInfo.uniqueId, workshopSkinData);
                }
            }
        }
        public static Workshop.WorkshopAppearanceInfo LoadCustomAppearance(string path)
        {
            string text = path + "/ModInfo.xml";
            if (File.Exists(text))
            {
                return LoadCustomAppearanceInfo(path, text);
            }
            return null;
        }
        private static List<Workshop.WorkshopAppearanceInfo> LoadWorkshopExtendedCustomAppearance()
        {
            List<Workshop.WorkshopAppearanceInfo> list = new List<Workshop.WorkshopAppearanceInfo>();
            string workshopDirPath = PlatformManager.Instance.GetWorkshopDirPath();
            if (Directory.Exists(workshopDirPath))
            {
                foreach (string text in Directory.GetDirectories(workshopDirPath))
                {
                    Workshop.WorkshopAppearanceInfo workshopAppearanceInfo = LoadCustomAppearance(text);
                    if (workshopAppearanceInfo != null && workshopAppearanceInfo is ExtendedWorkshopAppearanceInfo)
                    {
                        list.Add(workshopAppearanceInfo);
                        string[] array = text.Split(new char[]
                        {
                            '\\'
                        });
                        string text2 = array[array.Length - 1];
                        workshopAppearanceInfo.path = text;
                        workshopAppearanceInfo.uniqueId = text2;
                        if (string.IsNullOrEmpty(workshopAppearanceInfo.bookName))
                        {
                            workshopAppearanceInfo.bookName = text2;
                        }
                    }
                }
            }
            string localDirPath = Path.Combine(Application.dataPath, "Mods");
            if (Directory.Exists(localDirPath))
            {
                foreach (string text in Directory.GetDirectories(localDirPath))
                {
                    Workshop.WorkshopAppearanceInfo workshopAppearanceInfo = LoadCustomAppearance(text);
                    if (workshopAppearanceInfo != null)
                    {
                        list.Add(workshopAppearanceInfo);
                        string[] array = text.Split(new char[]
                        {
                            '\\'
                        });
                        string text2 = array[array.Length - 1];
                        workshopAppearanceInfo.path = text;
                        workshopAppearanceInfo.uniqueId = text2;
                        if (string.IsNullOrEmpty(workshopAppearanceInfo.bookName))
                        {
                            workshopAppearanceInfo.bookName = text2;
                        }
                    }
                }
            }
            return list;
        }
        private static Workshop.WorkshopAppearanceInfo LoadCustomAppearanceInfo(string rootPath, string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }
            Workshop.WorkshopAppearanceInfo workshopAppearanceInfo = null;
            StreamReader streamReader = new StreamReader(xml);
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.LoadXml(streamReader.ReadToEnd());
                XmlNode xmlNode = xmlDocument.SelectSingleNode("ModInfo");
                bool isExtended = false;
                XmlNode extendedXml = xmlNode.Attributes.GetNamedItem("extended");
                if (extendedXml == null)
                {
                    extendedXml = xmlNode.Attributes.GetNamedItem("Extended");
                }
                if (extendedXml != null)
                {
                    bool.TryParse(extendedXml.InnerText, out isExtended);
                }
                if (isExtended)
                {
                    workshopAppearanceInfo = new ExtendedWorkshopAppearanceInfo();
                }
                else
                {
                    workshopAppearanceInfo = new Workshop.WorkshopAppearanceInfo();
                }
                XmlNode xmlNode2 = xmlNode.SelectSingleNode("FaceInfo");
                XmlNode xmlNode3 = xmlNode.SelectSingleNode("ClothInfo");
                if (xmlNode2 != null)
                {
                    workshopAppearanceInfo.isFaceCustom = true;
                    for (int i = 0; i < 17; i++)
                    {
                        Workshop.FaceCustomType key = (Workshop.FaceCustomType)i;
                        string xpath = key.ToString();
                        XmlNode xmlNode4 = xmlNode2.SelectSingleNode(xpath);
                        if (xmlNode4 != null)
                        {
                            string innerText = xmlNode4.InnerText;
                            Sprite sprite = SpriteUtil.LoadSprite(rootPath + "/FaceCustom/" + innerText + ".png", new Vector2(0.5f, 0.5f));
                            if (sprite != null)
                            {
                                workshopAppearanceInfo.faceCustomInfo.Add(key, sprite);
                            }
                        }
                    }
                    LoadFaceCustom(workshopAppearanceInfo.faceCustomInfo);
                }
                if (xmlNode3 != null)
                {
                    workshopAppearanceInfo.isClothCustom = true;
                    string innerText2 = xmlNode3.SelectSingleNode("Name").InnerText;
                    if (!string.IsNullOrEmpty(innerText2))
                    {
                        workshopAppearanceInfo.bookName = innerText2;
                    }
                    for (int j = 0; j < 31; j++)
                    {
                        ActionDetail actionDetail = (ActionDetail)j;
                        if (actionDetail == ActionDetail.Standing)
                        {
                            continue;
                        }
                        string actionName = actionDetail.ToString();
                        try
                        {
                            XmlNode actionNode = xmlNode3.SelectSingleNode(actionName);
                            if (actionNode == null)
                            {
                                continue;
                            }

                            Vector2Int size = new Vector2Int(512, 512);
                            float res = 50f;
                            bool isCustomSize = false;
                            if (isExtended)
                            {
                                XmlNode sizeX = actionNode.Attributes.GetNamedItem("size_x");
                                if (sizeX != null)
                                {
                                    size.x = int.Parse(sizeX.InnerText);
                                }
                                XmlNode sizeY = actionNode.Attributes.GetNamedItem("size_y");
                                if (sizeY != null)
                                {
                                    size.y = int.Parse(sizeY.InnerText);
                                }
                                XmlNode resXml = actionNode.Attributes.GetNamedItem("quality");
                                if (resXml != null)
                                {
                                    res = float.Parse(resXml.InnerText, CultureInfo.InvariantCulture);
                                }
                                isCustomSize = (size.x != 512 || size.y != 512 || res != 50f);
                            }

                            XmlNode spritePivotNode = actionNode.SelectSingleNode("Pivot");
                            XmlNode headNode = actionNode.SelectSingleNode("Head");
                            XmlNode spritePivotXXml = null;
                            XmlNode spritePivotYXml = null;
                            XmlNode headXXml = null;
                            XmlNode headYXml = null;
                            XmlNode headRXml = null;
                            if (isExtended)
                            {
                                spritePivotXXml = spritePivotNode.Attributes.GetNamedItem("pivot_x_custom");
                                spritePivotYXml = spritePivotNode.Attributes.GetNamedItem("pivot_y_custom");
                                headXXml = headNode.Attributes.GetNamedItem("head_x_custom");
                                headYXml = headNode.Attributes.GetNamedItem("head_y_custom");
                                headRXml = headNode.Attributes.GetNamedItem("rotation_custom");
                            }
                            if (spritePivotXXml == null)
                            {
                                spritePivotXXml = spritePivotNode.Attributes.GetNamedItem("pivot_x");
                            }
                            if (spritePivotYXml == null)
                            {
                                spritePivotYXml = spritePivotNode.Attributes.GetNamedItem("pivot_y");
                            }
                            float spritePivotX = float.Parse(spritePivotXXml.InnerText, CultureInfo.InvariantCulture);
                            float spritePivotY = float.Parse(spritePivotYXml.InnerText, CultureInfo.InvariantCulture);
                            Vector2 pivotPos = new Vector2(spritePivotX / (2 * size.x) + 0.5f, spritePivotY / (2 * size.y) + 0.5f);

                            if (headXXml == null)
                            {
                                headXXml = headNode.Attributes.GetNamedItem("head_x");
                            }
                            if (headYXml == null)
                            {
                                headYXml = headNode.Attributes.GetNamedItem("head_y");
                            }
                            float headX = float.Parse(headXXml.InnerText, CultureInfo.InvariantCulture);
                            float headY = float.Parse(headYXml.InnerText, CultureInfo.InvariantCulture);
                            Vector2 headPos = new Vector2(headX / 100f, headY / 100f);
                            if (headRXml == null)
                            {
                                headRXml = headNode.Attributes.GetNamedItem("rotation");
                            }
                            float headRotation = float.Parse(headRXml.InnerText, CultureInfo.InvariantCulture);
                            XmlNode headEnabledXml = headNode.Attributes.GetNamedItem("head_enable");
                            bool headEnabled = true;
                            if (headEnabledXml != null)
                            {
                                bool.TryParse(headEnabledXml.InnerText, out headEnabled);
                            }

                            XmlNode directionNode = actionNode.SelectSingleNode("Direction");
                            CharacterMotion.MotionDirection direction = CharacterMotion.MotionDirection.FrontView;
                            if (directionNode.InnerText == "Side")
                            {
                                direction = CharacterMotion.MotionDirection.SideView;
                            }

                            List<Vector3> additionalPivotCoords = null;
                            if (isExtended)
                            {
                                XmlNodeList additionalPivotNodes = actionNode.SelectNodes("AdditionalPivot");
                                if (additionalPivotNodes != null)
                                {
                                    additionalPivotCoords = new List<Vector3>();
                                    foreach (XmlNode additionalPivot in additionalPivotNodes)
                                    {
                                        XmlNode addPivotX = additionalPivot.Attributes.GetNamedItem("pivot_x");
                                        XmlNode addPivotY = additionalPivot.Attributes.GetNamedItem("pivot_y");
                                        XmlNode addPivotR = additionalPivot.Attributes.GetNamedItem("rotation");
                                        additionalPivotCoords.Add(new Vector3(float.Parse(addPivotX.InnerText, CultureInfo.InvariantCulture) / 100f,
                                            float.Parse(addPivotY.InnerText, CultureInfo.InvariantCulture) / 100f,
                                            float.Parse(addPivotR?.InnerText ?? "0", CultureInfo.InvariantCulture)));
                                    }
                                }
                            }

                            bool hasSpriteFile = false;
                            string spritePath = rootPath + "/ClothCustom/" + actionName + "_mid.png";
                            bool hasSkinSprite = false;
                            string skinSpritePath = null;
                            bool hasBackSprite = false;
                            string backSpritePath = rootPath + "/ClothCustom/" + actionName + "_back.png";
                            bool hasBackSkinSprite = false;
                            string backSkinSpritePath = rootPath + "/ClothCustom/" + actionName + "_back_skin.png";
                            if (isExtended && File.Exists(spritePath))
                            {
                                hasSpriteFile = true;
                                skinSpritePath = rootPath + "/ClothCustom/" + actionName + "_mid_skin.png";
                                if (File.Exists(skinSpritePath))
                                {
                                    hasSkinSprite = true;
                                }
                                if (!File.Exists(backSpritePath))
                                {
                                    backSpritePath = rootPath + "/ClothCustom/" + actionName + ".png";
                                    backSkinSpritePath = rootPath + "/ClothCustom/" + actionName + "_skin.png";
                                }
                            }
                            else
                            {
                                spritePath = rootPath + "/ClothCustom/" + actionName + ".png";
                                skinSpritePath = rootPath + "/ClothCustom/" + actionName + "_skin.png";
                                if (File.Exists(spritePath))
                                {
                                    hasSpriteFile = true;
                                    if (isExtended)
                                    {
                                        if (File.Exists(skinSpritePath))
                                        {
                                            hasSkinSprite = true;
                                        }
                                        var largeSpritePath = rootPath + "/ClothCustom/" + actionName + "_large.png";
                                        if (File.Exists(largeSpritePath))
                                        {
                                            spritePath = largeSpritePath;
                                        }
                                    }
                                }
                            }

                            if (isExtended && File.Exists(backSpritePath))
                            {
                                hasBackSprite = true;
                                if (File.Exists(backSkinSpritePath))
                                {
                                    hasBackSkinSprite = true;
                                }
                            }

                            bool hasFrontSprite = false;
                            bool hasFrontSpriteFile = false;
                            string frontSpritePath = rootPath + "/ClothCustom/" + actionName + "_front.png";
                            bool hasFrontSkinSprite = false;
                            string frontSkinSpritePath = frontSkinSpritePath = rootPath + "/ClothCustom/" + actionName + "_front_skin.png";
                            if (File.Exists(frontSpritePath))
                            {
                                hasFrontSprite = true;
                                hasFrontSpriteFile = true;
                                if (isExtended)
                                {
                                    var largeSpritePath = rootPath + "/ClothCustom/" + actionName + "_large.png";
                                    if (File.Exists(largeSpritePath))
                                    {
                                        frontSpritePath = largeSpritePath;
                                    }
                                    if (File.Exists(frontSkinSpritePath))
                                    {
                                        hasFrontSkinSprite = true;
                                    }
                                }
                            }

                            Workshop.ClothCustomizeData value;
                            if (isExtended)
                            {
                                value = new ExtendedClothCustomizeData
                                {
                                    hasSpriteFile = hasSpriteFile,
                                    spritePath = spritePath,
                                    hasFrontSprite = hasFrontSprite,
                                    hasFrontSpriteFile = hasFrontSpriteFile,
                                    frontSpritePath = frontSpritePath,
                                    pivotPos = pivotPos,
                                    headPos = headPos,
                                    headRotation = headRotation,
                                    direction = direction,
                                    headEnabled = headEnabled,
                                    isCustomSize = isCustomSize,
                                    size = size,
                                    resolution = res,
                                    hasSkinSprite = hasSkinSprite,
                                    skinSpritePath = skinSpritePath,
                                    hasFrontSkinSprite = hasFrontSkinSprite,
                                    frontSkinSpritePath = frontSkinSpritePath,
                                    hasBackSprite = hasBackSprite,
                                    backSpritePath = backSpritePath,
                                    hasBackSkinSprite = hasBackSkinSprite,
                                    backSkinSpritePath = backSkinSpritePath,
                                    additionalPivots = additionalPivotCoords
                                };
                            }
                            else
                            {
                                value = new Workshop.ClothCustomizeData
                                {
                                    hasSpriteFile = hasSpriteFile,
                                    spritePath = spritePath,
                                    hasFrontSprite = hasFrontSprite,
                                    hasFrontSpriteFile = hasFrontSpriteFile,
                                    frontSpritePath = frontSpritePath,
                                    pivotPos = pivotPos,
                                    headPos = headPos,
                                    headRotation = headRotation,
                                    direction = direction,
                                    headEnabled = headEnabled
                                };
                            }
                            if (spritePath != null)
                            {
                                workshopAppearanceInfo.clothCustomInfo.Add(actionDetail, value);
                            }
                        }
                        catch (Exception message)
                        {
                            Debug.LogError(message);
                        }
                    }
                    if (isExtended)
                    {
                        ExtendedWorkshopAppearanceInfo extendedInfo = (ExtendedWorkshopAppearanceInfo)workshopAppearanceInfo;
                        XmlNode soundNode = xmlNode3.SelectSingleNode("SoundList");
                        if (soundNode != null)
                        {
                            List<CustomInvitation.BookSoundInfo> motionSoundList = new List<CustomInvitation.BookSoundInfo>();
                            XmlNodeList motionSoundsXml = soundNode.SelectNodes("SoundInfo");
                            if (motionSoundsXml != null)
                            {
                                foreach (XmlNode motionSound in motionSoundsXml)
                                {
                                    XmlAttributeCollection attr = motionSound.Attributes;
                                    XmlNode motion = attr.GetNamedItem("Motion");
                                    XmlNode winExternal = attr.GetNamedItem("WinExternal");
                                    XmlNode winPath = attr.GetNamedItem("Win");
                                    XmlNode loseExternal = attr.GetNamedItem("LoseExternal");
                                    XmlNode losePath = attr.GetNamedItem("Lose");
                                    if (Enum.TryParse(motion.InnerText, true, out MotionDetail motionDetail))
                                    {
                                        motionSoundList.Add(new CustomInvitation.BookSoundInfo()
                                        {
                                            motion = motionDetail,
                                            isWinExternal = bool.Parse(winExternal.InnerText),
                                            winSound = winPath.InnerText,
                                            isLoseExternal = bool.Parse(loseExternal.InnerText),
                                            loseSound = losePath.InnerText
                                        });
                                    }
                                }
                            }
                            extendedInfo.motionSoundList = motionSoundList;
                        }
                        XmlNode effectNode = xmlNode3.SelectSingleNode("AtkEffectPivotInfo");
                        if (effectNode != null)
                        {
                            Dictionary<string, EffectPivot> atkEffectPivotDic = new Dictionary<string, EffectPivot>();
                            AddPivot(atkEffectPivotDic, effectNode, "atkEffectRoot");
                            AddPivot(atkEffectPivotDic, effectNode, "atkEffectPivot_H");
                            AddPivot(atkEffectPivotDic, effectNode, "atkEffectPivot_J");
                            AddPivot(atkEffectPivotDic, effectNode, "atkEffectPivot_Z");
                            AddPivot(atkEffectPivotDic, effectNode, "atkEffectPivot_G");
                            AddPivot(atkEffectPivotDic, effectNode, "atkEffectPivot_E");
                            AddPivot(atkEffectPivotDic, effectNode, "atkEffectPivot_S");
                            AddPivot(atkEffectPivotDic, effectNode, "atkEffectPivot_F");
                            extendedInfo.atkEffectPivotDic = atkEffectPivotDic;
                        }
                        XmlNode specialNode = xmlNode3.SelectSingleNode("SpecialMotionPivotInfo");
                        if (specialNode != null)
                        {
                            Dictionary<ActionDetail, EffectPivot> specialMotionPivotDic = new Dictionary<ActionDetail, EffectPivot>();
                            for (int j = 0; j < 31; j++)
                            {
                                ActionDetail actionDetail = (ActionDetail)j;
                                string text = actionDetail.ToString();
                                XmlNode effectPivotNode = specialNode.SelectSingleNode(text);
                                if (effectPivotNode != null)
                                {
                                    specialMotionPivotDic[actionDetail] = GetEffectPivot(effectPivotNode);
                                }
                            }
                            extendedInfo.specialMotionPivotDic = specialMotionPivotDic;
                        }
                    }
                }
            }
            catch (Exception message2)
            {
                Debug.LogError(message2);
            }
            return workshopAppearanceInfo;
        }
        private static void AddPivot(Dictionary<string, EffectPivot> atkEffectPivotDic, XmlNode effectNode, string pivotNode)
        {
            XmlNode effectPivotNode = effectNode.SelectSingleNode(pivotNode);
            if (effectPivotNode != null)
            {
                atkEffectPivotDic[pivotNode] = GetEffectPivot(effectPivotNode);
            }
        }
        private static EffectPivot GetEffectPivot(XmlNode effectPivotNode)
        {
            EffectPivot atkEffectRoot = new EffectPivot();
            if (effectPivotNode != null)
            {
                XmlNode positionNode = effectPivotNode.SelectSingleNode("localPosition");
                if (positionNode != null)
                {
                    XmlNode x = positionNode.Attributes.GetNamedItem("x");
                    if (x != null)
                    {
                        atkEffectRoot.localPosition.x = float.Parse(x.InnerText, CultureInfo.InvariantCulture);
                    }
                    XmlNode y = positionNode.Attributes.GetNamedItem("y");
                    if (y != null)
                    {
                        atkEffectRoot.localPosition.y = float.Parse(y.InnerText, CultureInfo.InvariantCulture);
                    }
                    XmlNode z = positionNode.Attributes.GetNamedItem("z");
                    if (z != null)
                    {
                        atkEffectRoot.localPosition.z = float.Parse(z.InnerText, CultureInfo.InvariantCulture);
                    }
                }
                XmlNode scaleNode = effectPivotNode.SelectSingleNode("localScale");
                if (scaleNode != null)
                {
                    XmlNode x = scaleNode.Attributes.GetNamedItem("x");
                    if (x != null)
                    {
                        atkEffectRoot.localScale.x = float.Parse(x.InnerText, CultureInfo.InvariantCulture);
                    }
                    XmlNode y = scaleNode.Attributes.GetNamedItem("y");
                    if (y != null)
                    {
                        atkEffectRoot.localScale.y = float.Parse(y.InnerText, CultureInfo.InvariantCulture);
                    }
                    XmlNode z = scaleNode.Attributes.GetNamedItem("z");
                    if (z != null)
                    {
                        atkEffectRoot.localScale.z = float.Parse(z.InnerText, CultureInfo.InvariantCulture);
                    }
                }
                XmlNode anglesNode = effectPivotNode.SelectSingleNode("localEulerAngles");
                if (anglesNode != null)
                {
                    XmlNode x = anglesNode.Attributes.GetNamedItem("x");
                    if (x != null)
                    {
                        atkEffectRoot.localEulerAngles.x = float.Parse(x.InnerText, CultureInfo.InvariantCulture);
                    }
                    XmlNode y = anglesNode.Attributes.GetNamedItem("y");
                    if (y != null)
                    {
                        atkEffectRoot.localEulerAngles.y = float.Parse(y.InnerText, CultureInfo.InvariantCulture);
                    }
                    XmlNode z = anglesNode.Attributes.GetNamedItem("z");
                    if (z != null)
                    {
                        atkEffectRoot.localEulerAngles.z = float.Parse(z.InnerText, CultureInfo.InvariantCulture);
                    }
                }
            }
            return atkEffectRoot;
        }
        public class EffectPivot
        {
            public Vector3 localPosition = Vector3.zero;
            public Vector3 localScale = new Vector3(1, 1, 1);
            public Vector3 localEulerAngles = Vector3.zero;
        }
        private static void LoadFaceCustom(Dictionary<Workshop.FaceCustomType, Sprite> faceCustomInfo)
        {
            FaceResourceSet faceResourceSet = new FaceResourceSet();
            FaceResourceSet faceResourceSet2 = new FaceResourceSet();
            FaceResourceSet faceResourceSet3 = new FaceResourceSet();
            HairResourceSet hairResourceSet = new HairResourceSet();
            HairResourceSet hairResourceSet2 = new HairResourceSet();
            foreach (KeyValuePair<Workshop.FaceCustomType, Sprite> keyValuePair in faceCustomInfo)
            {
                Workshop.FaceCustomType key = keyValuePair.Key;
                Sprite value = keyValuePair.Value;
                switch (key)
                {
                    case Workshop.FaceCustomType.Front_RearHair:
                        hairResourceSet2.Default = value;
                        break;
                    case Workshop.FaceCustomType.Front_FrontHair:
                        hairResourceSet.Default = value;
                        break;
                    case Workshop.FaceCustomType.Front_Eye:
                        faceResourceSet.normal = value;
                        break;
                    case Workshop.FaceCustomType.Front_Brow_Normal:
                        faceResourceSet2.normal = value;
                        break;
                    case Workshop.FaceCustomType.Front_Brow_Attack:
                        faceResourceSet2.atk = value;
                        break;
                    case Workshop.FaceCustomType.Front_Brow_Hit:
                        faceResourceSet2.hit = value;
                        break;
                    case Workshop.FaceCustomType.Front_Mouth_Normal:
                        faceResourceSet3.normal = value;
                        break;
                    case Workshop.FaceCustomType.Front_Mouth_Attack:
                        faceResourceSet3.atk = value;
                        break;
                    case Workshop.FaceCustomType.Front_Mouth_Hit:
                        faceResourceSet3.hit = value;
                        break;
                    case Workshop.FaceCustomType.Side_RearHair_Rear:
                        hairResourceSet2.Side_Back = value;
                        break;
                    case Workshop.FaceCustomType.Side_FrontHair:
                        hairResourceSet.Side_Front = value;
                        break;
                    case Workshop.FaceCustomType.Side_RearHair_Front:
                        hairResourceSet2.Side_Front = value;
                        break;
                    case Workshop.FaceCustomType.Side_Mouth:
                        faceResourceSet3.atk_side = value;
                        break;
                    case Workshop.FaceCustomType.Side_Brow:
                        faceResourceSet2.atk_side = value;
                        break;
                    case Workshop.FaceCustomType.Side_Eye:
                        faceResourceSet.atk_side = value;
                        break;
                }
            }
            faceResourceSet.FillSprite();
            faceResourceSet2.FillSprite();
            faceResourceSet3.FillSprite();
            if (faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_Eye) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Side_Eye))
            {
                Singleton<CustomizingResourceLoader>.Instance._eyeResources.Add(faceResourceSet);
            }
            if (faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_Brow_Attack) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_Brow_Hit) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_Brow_Normal) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Side_Brow))
            {
                Singleton<CustomizingResourceLoader>.Instance._browResources.Add(faceResourceSet2);
            }
            if (faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_Mouth_Attack) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_Mouth_Hit) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_Mouth_Normal) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Side_Mouth))
            {
                Singleton<CustomizingResourceLoader>.Instance._mouthResources.Add(faceResourceSet3);
            }
            if (faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_FrontHair) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Side_FrontHair))
            {
                Singleton<CustomizingResourceLoader>.Instance._frontHairResources.Add(hairResourceSet);
            }
            if (faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_RearHair) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Side_RearHair_Front) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Side_RearHair_Rear))
            {
                Singleton<CustomizingResourceLoader>.Instance._rearHairResources.Add(hairResourceSet2);
            }
        }
        private static void LoadCoreThumbs()
        {
            var dic = new Dictionary<string, int>();
            var list = Singleton<BookXmlList>.Instance._list;
            foreach (BookXmlInfo info in list)
            {
                if (info.id.IsWorkshop())
                {
                    continue;
                }
                foreach (string skinName in info.CharacterSkin)
                {
                    if (!dic.ContainsKey(skinName))
                    {
                        dic.Add(skinName, info._id);
                    }
                }
            }
            CoreThumbDic = dic;
        }
        //Apperance
        //CreateSkin
        [HarmonyPatch(typeof(SdCharacterUtil), "CreateSkin")]
        [HarmonyPrefix]
        private static bool SdCharacterUtil_CreateSkin_Pre(UnitDataModel unit, Faction faction, Transform characterRoot, ref CharacterAppearance __result)
        {
            try
            {
                if (!string.IsNullOrEmpty(unit.workshopSkin))
                {
                    return true;
                }
                Workshop.WorkshopSkinData workshopBookSkinData = Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData(unit.CustomBookItem.BookId.packageId, unit.CustomBookItem.GetOriginalCharcterName(), "_" + unit.appearanceType);
                if (workshopBookSkinData != null && unit.CustomBookItem.ClassInfo.skinType == "Custom")
                {
                    UnitCustomizingData customizeData = unit.customizeData;
                    GiftInventory giftInventory = unit.giftInventory;
                    GameObject gameObject = CreateCustomCharacter_new(workshopBookSkinData, out string resourceName, characterRoot);
                    CharacterAppearance characterAppearance = null;
                    Workshop.WorkshopSkinDataSetter component = gameObject.GetComponent<Workshop.WorkshopSkinDataSetter>();
                    component.SetData(workshopBookSkinData);
                    List<CustomInvitation.BookSoundInfo> motionSoundList = unit.CustomBookItem.ClassInfo.motionSoundList;
                    if (motionSoundList != null && motionSoundList.Count > 0)
                    {
                        string motionSoundPath = ModUtil.GetMotionSoundPath(Singleton<ModContentManager>.Instance.GetModPath(unit.CustomBookItem.ClassInfo.id.packageId));
                        CharacterSound component2 = gameObject.GetComponent<CharacterSound>();
                        if (component2 != null)
                        {
                            component2.SetMotionSounds(motionSoundList, motionSoundPath);
                        }
                    }
                    characterAppearance = gameObject.GetComponent<CharacterAppearance>();
                    characterAppearance._initialized = false;
                    characterAppearance.Initialize(resourceName);
                    characterAppearance.InitCustomData(customizeData, unit.defaultBook.GetBookClassInfoId());
                    characterAppearance.InitGiftDataAll(giftInventory.GetEquippedList());
                    characterAppearance.ChangeMotion(ActionDetail.Standing);
                    characterAppearance.ChangeLayer("Character");
                    characterAppearance.SetLibrarianOnlySprites(faction);
                    try
                    {
                        gameObject.GetComponent<Workshop.WorkshopSkinDataSetter>().LateInit();
                    }
                    catch { }
                    __result = characterAppearance;
                    return false;
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/CreateSkinerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //ChangeEgoSkin
        [HarmonyPatch(typeof(BattleUnitView), "ChangeEgoSkin")]
        [HarmonyPrefix]
        private static bool BattleUnitView_ChangeEgoSkin_Pre(BattleUnitView __instance, ref BattleUnitView.SkinInfo ____skinInfo, string egoName, bool bookNameChange = true)
        {
            try
            {
                if (LorName.IsCompressed(egoName))
                {
                    __instance.ChangeEgoSkin(new LorName(egoName), bookNameChange);
                    return false;
                }
                Workshop.WorkshopSkinData workshopBookSkinData = Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData(__instance.model.UnitData.unitData.bookItem.ClassInfo.workshopID, egoName);
                if (workshopBookSkinData == null)
                {
                    return true;
                }
                __instance._skinInfo.state = BattleUnitView.SkinState.EGO;
                __instance._skinInfo.skinName = egoName;
                UnitCustomizingData customizeData = __instance.model.UnitData.unitData.customizeData;
                GiftInventory giftInventory = __instance.model.UnitData.unitData.giftInventory;
                ActionDetail currentMotionDetail = __instance.charAppearance.GetCurrentMotionDetail();
                __instance.DestroySkin();
                GameObject gameObject = CreateCustomCharacter_new(workshopBookSkinData, out string resourceName, __instance.characterRotationCenter);
                List<CustomInvitation.BookSoundInfo> motionSoundList = __instance.model.UnitData.unitData.bookItem.ClassInfo.motionSoundList;
                if (motionSoundList != null && motionSoundList.Count > 0)
                {
                    string motionSoundPath = ModUtil.GetMotionSoundPath(Singleton<ModContentManager>.Instance.GetModPath(__instance.model.UnitData.unitData.bookItem.ClassInfo.id.packageId));
                    gameObject.GetComponent<CharacterSound>()?.SetMotionSounds(motionSoundList, motionSoundPath);
                }
                gameObject.GetComponent<Workshop.WorkshopSkinDataSetter>().SetData(workshopBookSkinData);
                __instance.charAppearance = gameObject.GetComponent<CharacterAppearance>();
                __instance.charAppearance._initialized = false;
                __instance.charAppearance.Initialize(resourceName);
                __instance.charAppearance.InitCustomData(customizeData, __instance.model.UnitData.unitData.defaultBook.GetBookClassInfoId());
                __instance.charAppearance.InitGiftDataAll(giftInventory.GetEquippedList());
                __instance.charAppearance.ChangeMotion(currentMotionDetail);
                __instance.charAppearance.ChangeLayer("Character");
                __instance.charAppearance.SetLibrarianOnlySprites(__instance.model.faction);
                if (bookNameChange)
                {
                    __instance.model.Book.SetCharacterName(egoName);
                }
                if (customizeData != null)
                {
                    __instance.ChangeHeight(customizeData.height);
                }
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/ChangeEgoSkinerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //ChangeSkin
        [HarmonyPatch(typeof(BattleUnitView), "ChangeSkin")]
        [HarmonyPrefix]
        private static bool BattleUnitView_ChangeSkin_Pre(BattleUnitView __instance, ref BattleUnitView.SkinInfo ____skinInfo, string charName)
        {
            try
            {
                if (LorName.IsCompressed(charName))
                {
                    __instance.ChangeSkin(new LorName(charName));
                    return false;
                }
                Workshop.WorkshopSkinData workshopBookSkinData = Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData(__instance.model.UnitData.unitData.bookItem.ClassInfo.workshopID, charName);
                if (workshopBookSkinData == null)
                {
                    return true;
                }
                __instance._skinInfo.state = BattleUnitView.SkinState.Changed;
                __instance._skinInfo.skinName = charName;
                UnitCustomizingData customizeData = __instance.model.UnitData.unitData.customizeData;
                GiftInventory giftInventory = __instance.model.UnitData.unitData.giftInventory;
                ActionDetail currentMotionDetail = __instance.charAppearance.GetCurrentMotionDetail();
                __instance.DestroySkin();
                GameObject gameObject = CreateCustomCharacter_new(workshopBookSkinData, out string resourceName, __instance.characterRotationCenter);
                List<CustomInvitation.BookSoundInfo> motionSoundList = __instance.model.UnitData.unitData.bookItem.ClassInfo.motionSoundList;
                if (motionSoundList != null && motionSoundList.Count > 0)
                {
                    string motionSoundPath = ModUtil.GetMotionSoundPath(Singleton<ModContentManager>.Instance.GetModPath(__instance.model.UnitData.unitData.bookItem.ClassInfo.id.packageId));
                    gameObject.GetComponent<CharacterSound>()?.SetMotionSounds(motionSoundList, motionSoundPath);
                }
                gameObject.GetComponent<Workshop.WorkshopSkinDataSetter>().SetData(workshopBookSkinData);
                __instance.charAppearance = gameObject.GetComponent<CharacterAppearance>();
                __instance.charAppearance._initialized = false;
                __instance.charAppearance.Initialize(resourceName);
                __instance.charAppearance.InitCustomData(customizeData, __instance.model.UnitData.unitData.defaultBook.GetBookClassInfoId());
                __instance.charAppearance.InitGiftDataAll(giftInventory.GetEquippedList());
                __instance.charAppearance.ChangeMotion(currentMotionDetail);
                __instance.charAppearance.ChangeLayer("Character");
                __instance.charAppearance.SetLibrarianOnlySprites(__instance.model.faction);
                if (customizeData != null)
                {
                    __instance.ChangeHeight(customizeData.height);
                }
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/ChangeSkinerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }/*
        //Clone special motion characterMotion
        [HarmonyPatch(typeof(Workshop.WorkshopSkinDataSetter), "SetMotionData")]
        [HarmonyPrefix]
        private static void WorkshopSkinDataSetter_SetMotionData_Pre(Workshop.WorkshopSkinDataSetter __instance, ActionDetail motion)
        {
            try
            {
                if (__instance.Appearance.GetCharacterMotion(motion) == null)
                {
                    CharacterMotion NewMotion = CopyCharacterMotion(__instance.Appearance, motion);
                    __instance.Appearance._motionList.Add(NewMotion);
                    if (__instance.Appearance._motionList.Count > 0)
                    {
                        foreach (CharacterMotion characterMotion in __instance.Appearance._motionList)
                        {
                            if (!__instance.Appearance.CharacterMotions.ContainsKey(characterMotion.actionDetail))
                            {
                                __instance.Appearance.CharacterMotions.Add(characterMotion.actionDetail, characterMotion);
                            }
                            characterMotion.gameObject.SetActive(false);
                        }
                    }
                }
                else if (motion >= ActionDetail.Special)
                {
                    __instance.Appearance.GetCharacterMotion(motion).transform.position = __instance.Appearance._motionList[0].transform.position;
                    __instance.Appearance.GetCharacterMotion(motion).transform.localPosition = __instance.Appearance._motionList[0].transform.localPosition;
                    __instance.Appearance.GetCharacterMotion(motion).transform.localScale = __instance.Appearance._motionList[0].transform.localScale;
                    __instance.Appearance.GetCharacterMotion(motion).transform.name = "Custom_" + motion.ToString();
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/SetMotionDataerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }*/
        public static GameObject CreateCustomCharacter_new(Workshop.WorkshopSkinData workshopSkinData, out string resourceName, Transform characterRotationCenter = null)
        {
            GameObject result = null;
            resourceName = "";
            try
            {
                if (workshopSkinData != null)
                {
                    GameObject original = XLRoot.CustomAppearancePrefab;
                    if (characterRotationCenter != null)
                    {
                        result = UnityEngine.Object.Instantiate(original, characterRotationCenter);
                    }
                    else
                    {
                        result = UnityEngine.Object.Instantiate(original);
                    }
                    resourceName = workshopSkinData.dataName;
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/CCCnewerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
                result = null;
            }
            return result;
        }
        public static CharacterMotion CopyCharacterMotion(CharacterAppearance apprearance, ActionDetail detail)
        {
            CharacterMotion characterMotion = UnityEngine.Object.Instantiate(apprearance._motionList[0]);
            characterMotion.transform.parent = apprearance._motionList[0].transform.parent;
            characterMotion.transform.position = apprearance._motionList[0].transform.position;
            characterMotion.transform.localPosition = apprearance._motionList[0].transform.localPosition;
            characterMotion.transform.localScale = apprearance._motionList[0].transform.localScale;
            characterMotion.transform.name = "Custom_" + detail.ToString();
            characterMotion.actionDetail = detail;
            characterMotion.motionSpriteSet.Clear();
            characterMotion.motionSpriteSet.Add(new SpriteSet(characterMotion.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>(), CharacterAppearanceType.Body));
            characterMotion.motionSpriteSet.Add(new SpriteSet(characterMotion.transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>(), CharacterAppearanceType.Head));
            characterMotion.motionSpriteSet.Add(new SpriteSet(characterMotion.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>(), CharacterAppearanceType.Body));
            return characterMotion;
        }
        //CharacterSound
        [HarmonyPatch(typeof(CharacterSound), "LoadAudioCoroutine")]
        [HarmonyPrefix]
        private static bool CharacterSound_LoadAudioCoroutine_Pre(string path, List<CharacterSound.ExternalSound> externalSoundList, List<CharacterSound.Sound> ____motionSounds, ref IEnumerator __result)
        {
            try
            {
                __result = LoadAudioCoroutine(path, externalSoundList, ____motionSounds);
            }
            catch (Exception ex)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                File.WriteAllText(Application.dataPath + "/Mods/LoadAudioCoroutineerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return false;
        }
        private static IEnumerator LoadAudioCoroutine(string path, List<CharacterSound.ExternalSound> externalSoundList, List<CharacterSound.Sound> ____motionSounds)
        {
            foreach (CharacterSound.ExternalSound externalSound1 in externalSoundList)
            {
                CharacterSound.ExternalSound externalSound = externalSound1;
                string soundName = externalSound.soundName;
                bool win = externalSound.isWin;
                MotionDetail motion = externalSound.motion;
                string path1 = Path.Combine(path, soundName);
                AudioType audioType = AudioType.OGGVORBIS;
                if (path1.EndsWith(".wav"))
                    audioType = AudioType.WAV;
                if (File.Exists(path1))
                {
                    using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path1, audioType))
                    {
                        yield return www.SendWebRequest();
                        if (www.isNetworkError)
                        {
                            Debug.Log(www.error);
                        }
                        else
                        {
                            DownloadHandlerAudioClip downloadHandler = www.downloadHandler as DownloadHandlerAudioClip;
                            if (downloadHandler != null && downloadHandler.isDone)
                            {
                                AudioClip audioClip = downloadHandler.audioClip;
                                audioClip.name = soundName;
                                CharacterSound.Sound sound1 = ____motionSounds.Find(x => x.motion == motion);
                                if (sound1 != null)
                                {
                                    if (win)
                                    {
                                        sound1.winSound = audioClip;
                                    }
                                    else
                                    {
                                        sound1.loseSound = audioClip;
                                    }
                                }
                                else
                                {
                                    CharacterSound.Sound sound2 = new CharacterSound.Sound
                                    {
                                        motion = motion
                                    };
                                    if (win)
                                    {
                                        sound2.winSound = audioClip;
                                    }
                                    else
                                    {
                                        sound2.loseSound = audioClip;
                                    }
                                    ____motionSounds.Add(sound2);
                                    sound2 = null;
                                }
                                audioClip = null;
                                sound1 = null;
                            }
                            downloadHandler = null;
                        }
                    }
                }
                soundName = null;
                soundName = null;
                path1 = null;
                externalSound = null;
            }
        }
        [HarmonyPatch(typeof(FaceEditor), "Init")]
        [HarmonyPostfix]
        private static void FaceEditor_Init_Post(FaceEditor __instance)
        {
            try
            {
                __instance.face.gameObject.GetComponent<RectTransform>().sizeDelta = __instance.face.sprite.pivot;
                __instance.frontHair.gameObject.GetComponent<RectTransform>().sizeDelta = __instance.frontHair.sprite.pivot;
                __instance.backHair.gameObject.GetComponent<RectTransform>().sizeDelta = __instance.backHair.sprite.pivot;
                __instance.mouth.gameObject.GetComponent<RectTransform>().sizeDelta = __instance.mouth.sprite.pivot;
                __instance.brow.gameObject.GetComponent<RectTransform>().sizeDelta = __instance.brow.sprite.pivot;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/FaceEditorIniterror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //CustomProjection
        //Tab
        [HarmonyPatch(typeof(UIEquipPageCustomizePanel), "ApplyFilterAll")]
        [HarmonyPrefix]
        private static bool UIEquipPageCustomizePanel_ApplyFilterAll_Pre(UIEquipPageCustomizePanel __instance, List<int> ___filteredBookIdList, List<int> ___originBookIdList, UICustomScrollBar ___scrollBar, UIStoryGradeFilter ___gradeFilter, List<UIEquipPageCustomizeSlot> ___bookSlotList, float ___slotWidth, float ___slotHeight, int ___row, int ___column)
        {
            try
            {
                if (!InitWorkshopSkinChangeButton)
                {
                    uiEquipPageCustomizePanel = __instance;
                    __instance.gameObject.AddComponent<WorkshopSkinChangeButton>();
                    InitWorkshopSkinChangeButton = true;
                }
                if (__instance.isWorkshop)
                {
                    ___filteredBookIdList.Clear();
                    ___filteredBookIdList.AddRange(___originBookIdList);
                }
                else
                {
                    List<Grade> storyGradeFilter = ___gradeFilter.GetStoryGradeFilter();
                    ___filteredBookIdList.Clear();
                    bool flag2 = storyGradeFilter.Count == 0;
                    if (flag2)
                    {
                        ___filteredBookIdList.AddRange(___originBookIdList);
                    }
                    foreach (Grade g2 in storyGradeFilter)
                    {
                        Grade g = g2;
                        ___filteredBookIdList.AddRange(___originBookIdList.FindAll((int x) => GetModWorkshopBookData(x).Chapter == (int)g));
                    }
                }
                ___filteredBookIdList.Sort((int a, int b) => b.CompareTo(a));
                Predicate<int> match = (int x) => Singleton<BookXmlList>.Instance.GetData(x).RangeType != __instance.panel.Parent.SelectedUnit.bookItem.ClassInfo.RangeType;
                switch (__instance.panel.Parent.SelectedUnit.bookItem.ClassInfo.RangeType)
                {
                    case EquipRangeType.Melee:
                        match = ((int x) => GetModWorkshopBookData(x).RangeType == EquipRangeType.Range);
                        break;
                    case EquipRangeType.Range:
                        match = ((int x) => GetModWorkshopBookData(x).RangeType == EquipRangeType.Melee);
                        break;
                    case EquipRangeType.Hybrid:
                        match = ((int x) => GetModWorkshopBookData(x).RangeType != EquipRangeType.Hybrid);
                        break;
                }
                List<int> collection = ___filteredBookIdList.FindAll(match);
                ___filteredBookIdList.RemoveAll(match);
                ___filteredBookIdList.AddRange(collection);
                int num = __instance.GetMaxRow();
                ___scrollBar.SetScrollRectSize(___column * ___slotWidth, (num + ___row - 1) * ___slotHeight);
                ___scrollBar.SetWindowPosition(0f, 0f);
                __instance.ParentSelectable.ChildSelectable = ___bookSlotList[0].selectable;
                __instance.UpdateBookListPage(false);
                return false;
            }
            catch (Exception ex)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                File.WriteAllText(Application.dataPath + "/Mods/UIEquipPageCustomizePanel_ApplyFilterAll.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //Tab
        [HarmonyPatch(typeof(UIEquipPageCustomizePanel), "UpdateEquipPageSlotList")]
        [HarmonyPrefix]
        private static bool UIEquipPageCustomizePanel_UpdateEquipPageSlotList_Pre(UIEquipPageCustomizePanel __instance, List<int> ___currentScreenBookIdList, List<UIEquipPageCustomizeSlot> ___bookSlotList)
        {
            try
            {
                for (int i = 0; i < ___bookSlotList.Count; i++)
                {
                    if (i < ___currentScreenBookIdList.Count)
                    {
                        BookModel data = new BookModel(GetModWorkshopBookData(___currentScreenBookIdList[i]));
                        ___bookSlotList[i].SetData(data);
                        ___bookSlotList[i].SetActiveSlot(true);
                        bool flag2 = __instance.panel.PreviewData.customBookInfo != null && ___bookSlotList[i].BookDataModel.ClassInfo.id == __instance.panel.PreviewData.customBookInfo.id;
                        if (flag2)
                        {
                            ___bookSlotList[i].SetHighlighted(true, false, false);
                        }
                    }
                    else
                    {
                        ___bookSlotList[i].SetActiveSlot(false);
                    }
                }
                __instance.UpdatePageButtons();
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/UIEquipPageCustomizePanel_UpdateEquipPageSlotList.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //Remove Garbage Projection
        [HarmonyPatch(typeof(CustomCoreBookInventoryModel), "GetBookIdList_CustomCoreBook")]
        [HarmonyPostfix]
        private static List<int> CustomCoreBookInventoryModel_GetBookIdList_CustomCoreBook_Post(List<int> idList)
        {
            try
            {
                idList.RemoveAll(x => Singleton<BookInventoryModel>.Instance.GetBookCount(x) < 1);
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/RemoveGarbageProjectionerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return idList;
        }
        public static BookXmlInfo GetModWorkshopBookData(int i)
        {
            BookXmlInfo data;
            if (isModWorkshopSkin && ModWorkshopBookIndex != null && ModWorkshopBookIndex.ContainsKey(i))
            {
                data = Singleton<BookXmlList>.Instance.GetData(ModWorkshopBookIndex[i], true);
            }
            else
            {
                data = Singleton<BookXmlList>.Instance.GetData(i);
            }
            return data;
        }
        //savecustombook
        [HarmonyPatch(typeof(UnitDataModel), "GetSaveData")]
        [HarmonyPostfix]
        private static void UnitDataModel_GetSaveData_Post(UnitDataModel __instance, SaveData __result, BookModel ____CustomBookItem)
        {
            try
            {
                if (____CustomBookItem != null)
                {
                    LorId bookClassInfoId = ____CustomBookItem.GetBookClassInfoId();
                    SaveData customcorebook = new SaveData(SaveDataType.Dictionary);
                    customcorebook.AddData("_pid", new SaveData(bookClassInfoId.packageId));
                    customcorebook.AddData("_id", new SaveData(bookClassInfoId.id));
                    __result.GetDictionarySelf()["customcorebookInstanceId"] = customcorebook;
                }

            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/SaveCustomBookerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //loadcustombook
        [HarmonyPatch(typeof(UnitDataModel), "LoadFromSaveData")]
        [HarmonyPostfix]
        private static void UnitDataModel_LoadFromSaveData_Post(UnitDataModel __instance, SaveData data)
        {
            try
            {
                SaveData customcorebook = data.GetData("customcorebookInstanceId");
                if (customcorebook == null)
                {
                    return;
                }
                if (customcorebook.GetData("_pid") == null)
                {
                    return;
                }
                LorId id = new LorId(customcorebook.GetString("_pid"), customcorebook.GetInt("_id"));
                if (!Singleton<BookXmlList>.Instance.GetData(id).isError)
                {
                    BookModel bookModel = new BookModel(Singleton<BookXmlList>.Instance.GetData(id));
                    if (bookModel != null)
                    {
                        if (bookModel.ClassInfo.optionList.Contains(BookOption.Basic))
                        {
                            LorId bookClassInfoId = bookModel.GetBookClassInfoId();
                            if (bookClassInfoId.id > 10)
                            {
                                __instance.EquipCustomCoreBook(bookModel);
                            }
                            else if (__instance.OwnerSephirah != (SephirahType)bookClassInfoId.id)
                            {
                                __instance.EquipCustomCoreBook(null);
                            }
                            else if (__instance.isSephirah)
                            {
                                __instance.EquipCustomCoreBook(bookModel);
                            }
                            else
                            {
                                __instance.EquipCustomCoreBook(null);
                            }
                        }
                        else
                        {
                            __instance.EquipCustomCoreBook(bookModel);
                        }
                        if (Singleton<SaveManager>.Instance.iver <= 13 && bookModel.GetBookClassInfoId() == __instance.bookItem.GetBookClassInfoId())
                        {
                            __instance.EquipCustomCoreBook(null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/LoadCustomBookerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        //Book
        //BookIcon
        [HarmonyPatch(typeof(BookModel), "bookIcon", MethodType.Getter)]
        [HarmonyPrefix]
        private static bool BookModel_get_bookIcon_Pre(BookModel __instance, ref Sprite __result)
        {
            try
            {
                if (ArtWorks == null)
                {
                    GetArtWorks();
                }
                if (ArtWorks.TryGetValue(__instance.ClassInfo.BookIcon, out Sprite sprite))
                {
                    __result = sprite;
                    return false;
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/BookIconerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //BookName
        [HarmonyPatch(typeof(BookXmlInfo), "Name", MethodType.Getter)]
        [HarmonyPrefix]
        private static bool BookXmlInfo_get_Name_Pre(BookXmlInfo __instance, ref string __result)
        {
            try
            {
                string bookName = Singleton<BookDescXmlList>.Instance.GetBookName(new LorId(__instance.workshopID, __instance.TextId));
                if (!string.IsNullOrEmpty(bookName))
                {
                    __result = bookName;
                    return false;
                }
            }
            catch { }
            return true;
        }
        //BookStory
        [HarmonyPatch(typeof(BookXmlInfo), "Desc", MethodType.Getter)]
        [HarmonyPrefix]
        private static bool BookXmlInfo_get_Desc_Pre(BookXmlInfo __instance, ref List<string> __result)
        {
            try
            {
                List<string> bookText = Singleton<BookDescXmlList>.Instance.GetBookText(new LorId(__instance.workshopID, __instance.TextId));
                if (bookText.Count > 0)
                {
                    __result = bookText;
                    return false;
                }
            }
            catch { }
            return true;
        }
        //BookModelThumb
        [HarmonyPatch(typeof(BookModel), "GetThumbSprite")]
        [HarmonyPrefix]
        private static bool BookModel_GetThumbSprite_Pre(BookModel __instance, ref Sprite __result)
        {
            try
            {
                Workshop.WorkshopSkinData workshopBookSkinData = Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData(__instance.BookId.packageId, __instance.ClassInfo.GetCharacterSkin(), "_" + __instance.ClassInfo.gender);
                if (workshopBookSkinData != null)
                {
                    string path = workshopBookSkinData.dic[ActionDetail.Default].spritePath;
                    DirectoryInfo directoryInfo = new DirectoryInfo(path);
                    path = directoryInfo.Parent.Parent.FullName + "/Thumb.png";
                    Sprite bookThumb = GetBookThumb(__instance.BookId, path);
                    if (bookThumb != null)
                    {
                        __result = bookThumb;
                        return false;
                    }
                    bookThumb = XLRoot.MakeThumbnail(workshopBookSkinData.dic[ActionDetail.Default]);
                    if (bookThumb != null)
                    {
                        BookThumb.Add(__instance.BookId, bookThumb);
                        __result = bookThumb;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/Book_Thumberror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            if (__instance.ClassInfo.skinType != "Custom")
            {
                CoreThumbDic.TryGetValue(__instance.ClassInfo.GetCharacterSkin(), out int index);
                if (index == 0)
                {
                    index++;
                }
                __result = Resources.Load<Sprite>("Sprites/Books/Thumb/" + index);
                return false;
            }
            return true;
        }
        //BookXmlInfoThumb
        [HarmonyPatch(typeof(BookXmlInfo), "GetThumbSprite")]
        [HarmonyPrefix]
        private static bool BookXmlInfo_GetThumbSprite_Pre(BookXmlInfo __instance, ref Sprite __result)
        {
            try
            {
                Workshop.WorkshopSkinData workshopBookSkinData = Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData(__instance.id.packageId, __instance.GetCharacterSkin(), "_" + __instance.gender);
                if (workshopBookSkinData != null)
                {
                    string path = workshopBookSkinData.dic[ActionDetail.Default].spritePath;
                    DirectoryInfo dir = new DirectoryInfo(path);
                    path = dir.Parent.Parent.FullName + "/Thumb.png";
                    Sprite bookThumb = GetBookThumb(__instance.id, path);
                    if (bookThumb != null)
                    {
                        __result = bookThumb;
                        return false;
                    }
                    bookThumb = XLRoot.MakeThumbnail(workshopBookSkinData.dic[ActionDetail.Default]);
                    if (bookThumb != null)
                    {
                        BookThumb.Add(__instance.id, bookThumb);
                        __result = bookThumb;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/BookXml_Thumberror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            if (__instance.skinType != "Custom")
            {
                CoreThumbDic.TryGetValue(__instance.GetCharacterSkin(), out int index);
                if (index == 0)
                {
                    index++;
                }
                __result = Resources.Load<Sprite>("Sprites/Books/Thumb/" + index);
                return false;
            }
            return true;
        }
        private static Sprite GetBookThumb(LorId BookId, string Path)
        {
            if (BookThumb == null)
            {
                BookThumb = new Dictionary<LorId, Sprite>();
            }
            if (BookThumb.TryGetValue(BookId, out Sprite result))
            {
                return result;
            }
            else if (File.Exists(Path))
            {
                Texture2D texture2D = new Texture2D(2, 2);
                texture2D.LoadImage(File.ReadAllBytes(Path));
                Sprite value = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
                BookThumb[BookId] = value;
                return value;
            }
            return null;
        }
        //OnlyCards
        [HarmonyPatch(typeof(BookModel), "SetXmlInfo")]
        [HarmonyPostfix]
        private static void BookModel_SetXmlInfo_Post(BookModel __instance, BookXmlInfo classInfo, ref List<DiceCardXmlInfo> ____onlyCards)
        {
            try
            {
                if (!OrcTools.OnlyCardDic.ContainsKey(classInfo.id))
                {
                    return;
                }
                ____onlyCards = new List<DiceCardXmlInfo>();
                foreach (LorId id in OrcTools.OnlyCardDic[classInfo.id])
                {
                    DiceCardXmlInfo cardItem = ItemXmlDataList.instance.GetCardItem(id);
                    if (cardItem == null)
                    {
                        Debug.LogError("soulcard not found");
                    }
                    else
                    {
                        ____onlyCards.Add(cardItem);
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/SetOnlyCardserror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //SoulCard
        [HarmonyPatch(typeof(BookModel), "CreateSoulCard")]
        [HarmonyPrefix]
        private static bool BookModel_CreateSoulCard_Pre(BookModel __instance, int emotionLevel, BookXmlInfo ____classInfo, ref BattleDiceCardModel __result)
        {
            try
            {
                if (____classInfo == null)
                {
                    Debug.LogError("BookXmlInfo is null");
                    return false;
                }
                if (!OrcTools.SoulCardDic.ContainsKey(____classInfo.id))
                {
                    return true;
                }
                BookSoulCardInfo_New bookSoulCardInfo_New = OrcTools.SoulCardDic[____classInfo.id].Find((BookSoulCardInfo_New x) => x.emotionLevel == emotionLevel);
                if (bookSoulCardInfo_New == null)
                {
                    __result = null;
                    return false;
                }
                DiceCardXmlInfo cardItem = ItemXmlDataList.instance.GetCardItem(new LorId(bookSoulCardInfo_New.WorkshopId, bookSoulCardInfo_New.cardId));
                if (cardItem == null)
                {
                    __result = null;
                    return false;
                }
                __result = BattleDiceCardModel.CreatePlayingCard(cardItem);
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/CreateSoulCarderror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }

        //Stage
        //StageName
        [HarmonyPatch(typeof(StageNameXmlList), "GetName", new Type[] { typeof(StageClassInfo) })]
        [HarmonyPrefix]
        private static bool StageNameXmlList_GetName_Pre(StageClassInfo stageInfo, ref string __result)
        {
            try
            {
                if (stageInfo == null || stageInfo.id == null)
                {
                    __result = "Not Found";
                    return false;
                }
                if (OrcTools.StageNameDic.TryGetValue(stageInfo.id, out string stageName))
                {
                    __result = stageName;
                    return false;
                }
            }
            catch { }
            return true;
        }

        //BattleUnitModel
        //CharacterName
        [HarmonyPatch(typeof(CharactersNameXmlList), "GetName", new Type[] { typeof(LorId) })]
        [HarmonyPrefix]
        private static bool CharactersNameXmlList_GetName_Pre(LorId id, ref string __result)
        {
            try
            {
                if (OrcTools.CharacterNameDic.TryGetValue((id), out string characterName))
                {
                    __result = characterName;
                    return false;
                }
            }
            catch { }
            return true;
        }
        //DropBookName
        [HarmonyPatch(typeof(DropBookXmlInfo), "Name", MethodType.Getter)]
        [HarmonyPrefix]
        private static bool DropBookXmlInfo_get_Name_Pre(DropBookXmlInfo __instance, ref string __result)
        {
            try
            {
                string dropBookName = TextDataModel.GetText(__instance._targetText, Array.Empty<object>());
                if (!string.IsNullOrEmpty(dropBookName))
                {
                    __result = dropBookName;
                    return false;
                }
            }
            catch { }
            return true;
        }
        //EnemyDrop
        [HarmonyPatch(typeof(UnitDataModel), "SetByEnemyUnitClassInfo")]
        [HarmonyPostfix]
        private static void UnitDataModel_SetByEnemyUnitClassInfo_Post(EnemyUnitClassInfo classInfo, ref string ____name)
        {
            try
            {
                if (OrcTools.CharacterNameDic.TryGetValue(new LorId(classInfo.workshopID, classInfo.nameId), out string characterName))
                {
                    ____name = characterName;
                }
            }
            catch { }
        }
        //Drop
        [HarmonyPatch(typeof(UnitDataModel), "SetEnemyDropTable")]
        [HarmonyPrefix]
        private static bool UnitDataModel_SetEnemyDropTable_Pre(EnemyUnitClassInfo classInfo, Dictionary<int, DropTable> ____dropTable, ref float ____dropBonus, ref int ____expDrop)
        {
            try
            {
                if (classInfo != null && !OrcTools.DropItemDic.ContainsKey(classInfo.id))
                {
                    return true;
                }
                foreach (EnemyDropItemTable_New dropItemTable_New in OrcTools.DropItemDic[classInfo.id])
                {
                    DropTable dropTable = new DropTable();
                    foreach (EnemyDropItem_ReNew dropItem_ReNew in dropItemTable_New.dropList)
                    {
                        dropTable.Add(dropItem_ReNew.prob, dropItem_ReNew.bookId);
                    }
                    ____dropTable.Add(dropItemTable_New.emotionLevel, dropTable);
                }
                ____dropBonus = classInfo.dropBonus;
                ____expDrop = classInfo.exp;
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/SetEnemyDropTableerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //DropUI
        [HarmonyPatch(typeof(UIInvitationStageInfoPanel), "SetData")]
        [HarmonyPrefix]
        private static void UIInvitationStageInfoPanel_SetData_Post(StageClassInfo stage, UIRewardDropBookList ___rewardBookList, UIStoryLine story = UIStoryLine.None)
        {
            CanvasGroup cg = ___rewardBookList.cg;
            if (story == UIStoryLine.None && cg.interactable)
            {

                List<LorId> list = new List<LorId>();
                if (stage.id == 40008)
                {
                    list.Add(new LorId(240023));
                }
                foreach (StageWaveInfo stageWaveInfo in stage.waveList)
                {
                    foreach (LorId id in stageWaveInfo.enemyUnitIdList)
                    {
                        EnemyUnitClassInfo data = Singleton<EnemyUnitClassInfoList>.Instance.GetData(id);
                        if (OrcTools.DropItemDic.ContainsKey(data.id))
                        {
                            foreach (EnemyDropItemTable_New dropItemTable_New in OrcTools.DropItemDic[data.id])
                            {
                                foreach (EnemyDropItem_ReNew dropItem_ReNew in dropItemTable_New.dropList)
                                {
                                    if (!list.Contains(dropItem_ReNew.bookId))
                                    {
                                        list.Add(dropItem_ReNew.bookId);
                                    }
                                }
                            }
                        }
                    }
                }
                ___rewardBookList.SetData(list);
            }
        }
        //EnemyBattleDialog
        [HarmonyPatch(typeof(UnitDataModel), "InitBattleDialogByDefaultBook")]
        [HarmonyPrefix]
        private static bool UnitDataModel_InitBattleDialogByDefaultBook_Pre(UnitDataModel __instance, LorId lorId)
        {
            try
            {
                if (lorId.id <= 0)
                {
                    Debug.LogError("Invalid book ID : " + lorId.id);
                    return false;
                }
                else
                {
                    if (OrcTools.DialogDic.TryGetValue(lorId, out string characterName))
                    {
                        BattleDialogCharacter characterData = null;
                        if (!string.IsNullOrEmpty(characterName) && OrcTools.DialogRelationDic.TryGetValue(lorId, out string groupID))
                        {
                            characterData = Singleton<BattleDialogXmlList>.Instance.GetCharacterData(groupID, lorId.id.ToString());
                        }
                        if (characterData == null)
                        {
                            characterData = Singleton<BattleDialogXmlList>.Instance.GetCharacterData_Mod(lorId.packageId, lorId.id);
                        }
                        Type type = FindBattleDialogueModel(characterName, characterData.characterID);
                        if (type == null)
                        {
                            __instance.battleDialogModel = new BattleDialogueModel(characterData);
                            return false;
                        }
                        else
                        {
                            __instance.battleDialogModel = Activator.CreateInstance(type, new object[]
                            {
                                characterData
                            }) as BattleDialogueModel;
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/InitBattleDialogByDefaultBookerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        public static Type FindBattleDialogueModel(string name, string id)
        {
            if (!string.IsNullOrEmpty(name) && CustomBattleDialogModel.TryGetValue(name, out Type type))
            {
                return type;
            }
            if (!string.IsNullOrEmpty(id) && CustomBattleDialogModel.TryGetValue(id, out Type type2))
            {
                return type2;
            }
            foreach (Type type3 in Assembly.LoadFile(Application.dataPath + "/Managed/Assembly-CSharp.dll").GetTypes())
            {
                if (type3.Name == "BattleDialogueModel_" + id.Trim())
                {
                    return type3;
                }
            }
            return null;
        }
        //防止被动过多炸UI，存在字体问题待解决
        [HarmonyPatch(typeof(BattleUnitInformationUI_PassiveList), "SetData")]
        [HarmonyPrefix]
        private static bool BattleUnitInformationUI_PassiveList_SetData_Pre(BattleUnitInformationUI_PassiveList __instance, List<PassiveAbilityBase> passivelist, List<BattleUnitInformationUI_PassiveList.BattleUnitInformationPassiveSlot> ___passiveSlotList)
        {
            try
            {
                for (int i = ___passiveSlotList.Count; i < passivelist.Count; i++)
                {
                    BattleUnitInformationUI_PassiveList.BattleUnitInformationPassiveSlot battleUnitInformationPassiveSlot = new BattleUnitInformationUI_PassiveList.BattleUnitInformationPassiveSlot();
                    RectTransform rectTransform = UnityEngine.Object.Instantiate(___passiveSlotList[0].Rect, ___passiveSlotList[0].Rect.parent);
                    battleUnitInformationPassiveSlot.Rect = rectTransform;
                    for (int j = 0; j < battleUnitInformationPassiveSlot.Rect.childCount; j++)
                    {
                        if (battleUnitInformationPassiveSlot.Rect.GetChild(j).gameObject.name.Contains("Glow"))
                        {
                            battleUnitInformationPassiveSlot.img_IconGlow = battleUnitInformationPassiveSlot.Rect.GetChild(j).gameObject.GetComponent<Image>();
                        }
                        else if (battleUnitInformationPassiveSlot.Rect.GetChild(j).gameObject.name.Contains("Desc"))
                        {
                            battleUnitInformationPassiveSlot.txt_PassiveDesc = battleUnitInformationPassiveSlot.Rect.GetChild(j).gameObject.GetComponent<TextMeshProUGUI>();
                        }
                        else
                        {
                            battleUnitInformationPassiveSlot.img_Icon = rectTransform.GetChild(j).gameObject.GetComponent<Image>();
                        }
                    }
                    rectTransform.gameObject.SetActive(false);
                    ___passiveSlotList.Add(battleUnitInformationPassiveSlot);
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/BUIUIPSerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }

        //Card
        //CardName
        [HarmonyPatch(typeof(BattleDiceCardModel), "GetName")]
        [HarmonyPrefix]
        private static bool BattleDiceCardModel_GetName_Pre(BattleDiceCardModel __instance, ref string __result)
        {
            try
            {
                string cardName = "";
                if (__instance.XmlData._textId <= 0)
                {
                    cardName = GetCardName(__instance.GetID());
                }
                else
                {
                    cardName = GetCardName(new LorId(__instance.XmlData.workshopID, __instance.XmlData._textId));
                }
                if (cardName != "Not Found")
                {
                    __result = cardName;
                    return false;
                }
            }
            catch { }
            return true;
        }
        //CardItemName
        [HarmonyPatch(typeof(DiceCardItemModel), "GetName")]
        [HarmonyPrefix]
        public static bool DiceCardItemModel_GetName_Pre(DiceCardItemModel __instance, ref string __result)
        {
            try
            {
                string cardName = "";
                if (__instance.ClassInfo._textId <= 0)
                {
                    cardName = GetCardName(__instance.GetID());
                }
                else
                {
                    cardName = GetCardName(new LorId(__instance.ClassInfo.workshopID, __instance.ClassInfo._textId));
                }
                if (cardName != "Not Found")
                {
                    __result = cardName;
                    return false;
                }
            }
            catch { }
            return true;
        }
        //CardXmlName
        [HarmonyPatch(typeof(DiceCardXmlInfo), "Name", MethodType.Getter)]
        [HarmonyPrefix]
        private static bool DiceCardXmlInfo_get_Name_Pre(DiceCardXmlInfo __instance, ref string __result)
        {
            try
            {
                string cardName = "";
                if (__instance._textId <= 0)
                {
                    cardName = GetCardName(__instance.id);
                }
                else
                {
                    cardName = GetCardName(new LorId(__instance.workshopID, __instance._textId));
                }
                if (cardName != "Not Found")
                {
                    __result = cardName;
                    return false;
                }
            }
            catch { }
            return true;
        }
        //DiceCardUIName
        [HarmonyPatch(typeof(BattleDiceCardUI), "SetCard")]
        [HarmonyPostfix]
        private static void BattleDiceCardUI_SetCard_Post(BattleDiceCardModel cardModel, TextMeshProUGUI ___txt_cardName)
        {
            try
            {
                string cardName = "";
                if (cardModel.XmlData._textId <= 0)
                {
                    cardName = GetCardName(cardModel.GetID());
                }
                else
                {
                    cardName = GetCardName(new LorId(cardModel.XmlData.workshopID, cardModel.XmlData._textId));
                }
                if (cardName != "Not Found")
                {
                    ___txt_cardName.text = cardName;
                }
            }
            catch { }
        }
        //Get CardSprite
        [HarmonyPatch(typeof(AssetBundleManagerRemake), "LoadCardSprite")]
        [HarmonyPrefix]
        private static bool AssetBundleManagerRemake_LoadCardSprite_Pre(string name, ref Sprite __result)
        {
            try
            {
                if (name == null)
                {
                    return true;
                }
                else
                {
                    if (ArtWorks == null)
                    {
                        GetArtWorks();
                    }
                    if (ArtWorks.TryGetValue(name, out Sprite sprite))
                    {
                        __result = sprite;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/LoadCardSpriteerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //Get CardSprite
        [HarmonyPatch(typeof(CustomizingCardArtworkLoader), "GetSpecificArtworkSprite")]
        [HarmonyPostfix]
        private static Sprite CustomizingCardArtworkLoader_GetSpecificArtworkSprite_Post(Sprite cardArtwork, string name)
        {
            try
            {
                if (cardArtwork == null)
                {
                    cardArtwork = Singleton<AssetBundleManagerRemake>.Instance.LoadCardSprite(name);
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/LoadCardSprite2error.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return cardArtwork;
        }
        //If has script and is not creating a new playingcard,return _script
        [HarmonyPatch(typeof(BattleDiceCardModel), "CreateDiceCardSelfAbilityScript")]
        [HarmonyPrefix]
        private static bool BattleDiceCardModel_CreateDiceCardSelfAbilityScript_Pre(ref DiceCardSelfAbilityBase __result, DiceCardSelfAbilityBase ____script)
        {
            try
            {
                if (____script != null)
                {
                    System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
                    string name = stackTrace.GetFrame(2).GetMethod().Name;
                    if (!name.Contains("AddCard") && !name.Contains("AllowTargetChanging"))
                    {
                        __result = ____script;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/ScriptFixPreerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //If has script,create a playingcard for cardModel in hand
        [HarmonyPatch(typeof(BattleDiceCardModel), "CreateDiceCardSelfAbilityScript")]
        [HarmonyPostfix]
        private static void BattleDiceCardModel_CreateDiceCardSelfAbilityScript_Post(BattleDiceCardModel __instance, DiceCardSelfAbilityBase __result)
        {
            try
            {
                if (__result == null)
                {
                    return;
                }
                if (__result.card == null)
                {
                    BattlePlayingCardDataInUnitModel card = new BattlePlayingCardDataInUnitModel
                    {
                        owner = __instance.owner,
                        card = __instance,
                        cardAbility = __result
                    };
                    __result.card = card;
                    return;
                }
                if (__result.card.owner == null)
                {
                    __result.card.owner = __instance.owner;
                    return;
                }
                if (__result.card.card == null)
                {
                    __result.card.card = __instance;
                    return;
                }
                if (__result.card.card.owner == null)
                {
                    __result.card.card.owner = __instance.owner;
                    return;
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/ScriptFixPosterror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //Apply owner for personal card
        [HarmonyPatch(typeof(BattlePersonalEgoCardDetail), "AddCard", new Type[] { typeof(LorId) })]
        [HarmonyPrefix]
        private static bool BattlePersonalEgoCardDetail_AddCard_Pre(LorId cardId, List<BattleDiceCardModel> ____cardsAll, List<BattleDiceCardModel> ____cardInHand, BattleUnitModel ____self)
        {
            try
            {
                DiceCardXmlInfo cardItem = ItemXmlDataList.instance.GetCardItem(cardId, false);
                if (cardItem == null)
                {
                    return false;
                }
                if (!cardItem.IsPersonal())
                {
                    return false;
                }
                if (____cardsAll.FindAll((BattleDiceCardModel x) => x.GetID() == cardId).Count == 0)
                {
                    BattleDiceCardModel battleDiceCardModel = BattleDiceCardModel.CreatePlayingCard(cardItem);
                    battleDiceCardModel.owner = ____self;
                    ____cardsAll.Add(battleDiceCardModel);
                    ____cardInHand.Add(battleDiceCardModel);
                    if (battleDiceCardModel.XmlData.IsEgo())
                    {
                        battleDiceCardModel.ResetCoolTime();
                        battleDiceCardModel.SetMaxCooltime();
                        battleDiceCardModel.SetCurrentCostMax();
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/PersonalEGOAddCarderror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //CardSelfAbilityKeywords
        [HarmonyPatch(typeof(BattleCardAbilityDescXmlList), "GetAbilityKeywords_byScript")]
        [HarmonyPrefix]
        private static void BattleCardAbilityDescXmlList_GetAbilityKeywords_byScript_Pre(string scriptName, ref Dictionary<string, List<string>> ____dictionaryKeywordCache)
        {
            try
            {
                if (string.IsNullOrEmpty(scriptName))
                {
                    return;
                }
                if (!____dictionaryKeywordCache.ContainsKey(scriptName))
                {
                    DiceCardAbilityBase diceCardAbilityBase = Singleton<AssemblyManager>.Instance.CreateInstance_DiceCardAbility(scriptName.Trim());
                    if (diceCardAbilityBase == null)
                    {
                        DiceCardSelfAbilityBase diceCardSelfAbilityBase = Singleton<AssemblyManager>.Instance.CreateInstance_DiceCardSelfAbility(scriptName.Trim());
                        if (diceCardSelfAbilityBase != null)
                        {
                            ____dictionaryKeywordCache[scriptName] = new List<string>();
                            ____dictionaryKeywordCache[scriptName].AddRange(diceCardSelfAbilityBase.Keywords);
                            return;
                        }
                    }
                    else
                    {
                        ____dictionaryKeywordCache[scriptName] = new List<string>();
                        ____dictionaryKeywordCache[scriptName].AddRange(diceCardAbilityBase.Keywords);
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/KeywordsbyScripterror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //RangeSpecial
        [HarmonyPatch(typeof(UISpriteDataManager), "GetRangeIconSprite")]
        [HarmonyPrefix]
        private static bool UISpriteDataManager_GetRangeIconSprite_Pre(ref Sprite __result, CardRange range)
        {
            try
            {
                if (ArtWorks == null)
                {
                    GetArtWorks();
                }
                if (range == CardRange.Special)
                {
                    __result = ArtWorks["CommonPage_RightTop_Type_SpecialAttack"];
                    return false;
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/SpecialRangeIconerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //CardBufIcon
        [HarmonyPatch(typeof(BattleDiceCardBuf), "GetBufIcon")]
        [HarmonyPrefix]
        private static bool BattleDiceCardBuf_GetBufIcon_Pre(BattleDiceCardBuf __instance, ref Sprite __result, ref bool ____iconInit, ref Sprite ____bufIcon)
        {
            if (ArtWorks == null)
            {
                GetArtWorks();
            }
            if (!____iconInit)
            {
                try
                {
                    string keywordIconId = __instance.keywordIconId;
                    if (!string.IsNullOrEmpty(keywordIconId) && ArtWorks.TryGetValue("CardBuf_" + keywordIconId, out Sprite sprite) && sprite != null)
                    {
                        ____iconInit = true;
                        ____bufIcon = sprite;
                        __result = sprite;
                        return false;
                    }
                }
                catch { }
            }
            return true;
        }
        //costtozero real
        [HarmonyPatch(typeof(BattleDiceCardModel), "GetCost")]
        [HarmonyPrefix]
        private static bool BattleDiceCardModel_GetCost_Pre(ref int __result, BattleDiceCardModel __instance, DiceCardSelfAbilityBase ____script, DiceCardXmlInfo ____xmlData, List<BattleDiceCardBuf> ____bufList, int ____curCost, int ____costAdder, bool ____costZero)
        {
            try
            {
                if (____script != null && ____script.IsFixedCost())
                {
                    __result = ____xmlData.Spec.Cost;
                    return false;
                }
                int baseCost = ____curCost;
                foreach (BattleDiceCardBuf battleDiceCardBuf in ____bufList)
                {
                    baseCost = battleDiceCardBuf.GetCost(baseCost);
                }
                int abilityCostAdder = 0;
                if (__instance.owner != null && !__instance.XmlData.IsPersonal())
                {
                    abilityCostAdder += __instance.owner.emotionDetail.GetCardCostAdder(__instance);
                    abilityCostAdder += __instance.owner.bufListDetail.GetCardCostAdder(__instance);
                    if (____script != null)
                    {
                        abilityCostAdder += ____script.GetCostAdder(__instance.owner, __instance);
                    }
                }
                int finalCost = baseCost + ____costAdder + abilityCostAdder;
                if (____costZero)
                {
                    finalCost = 0;
                }
                if (__instance.owner != null && ____script != null && ____script != null)
                {
                    finalCost = ____script.GetCostLast(__instance.owner, __instance, finalCost);
                }
                __result = Mathf.Max(0, finalCost);
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/GetCostRemakeerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //BattleDiceBehavior ignorepower lead to dicevalue zero
        [HarmonyPatch(typeof(BattleDiceBehavior), "UpdateDiceFinalValue")]
        [HarmonyPrefix]
        private static void BattleDiceBehavior_UpdateDiceFinalValue_Pre(ref int ____diceFinalResultValue, int ____diceResultValue)
        {
            try
            {
                ____diceFinalResultValue = ____diceResultValue;
            }
            catch { }
        }

        //Passive
        //PassiveName
        [HarmonyPatch(typeof(BookPassiveInfo), "name", MethodType.Getter)]
        [HarmonyPrefix]
        private static bool BookPassiveInfo_get_name_Pre(BookPassiveInfo __instance, ref string __result)
        {
            try
            {
                string passiveName = Singleton<PassiveDescXmlList>.Instance.GetName(__instance.passive.id);
                if (!string.IsNullOrEmpty(passiveName))
                {
                    __result = passiveName;
                    return false;
                }
            }
            catch { }
            return true;
        }
        //PassiveDesc
        [HarmonyPatch(typeof(BookPassiveInfo), "desc", MethodType.Getter)]
        [HarmonyPrefix]
        private static bool BookPassiveInfo_get_desc_Pre(BookPassiveInfo __instance, ref string __result)
        {
            try
            {
                string passiveDesc = Singleton<PassiveDescXmlList>.Instance.GetDesc(__instance.passive.id);
                if (!string.IsNullOrEmpty(passiveDesc))
                {
                    __result = passiveDesc;
                    return false;
                }
            }
            catch { }
            return true;
        }

        //BattleUnitBuff
        //ReadyBuf
        [HarmonyPatch(typeof(BattleUnitBufListDetail), "OnRoundStart")]
        [HarmonyPrefix]
        private static bool BattleUnitBufListDetail_OnRoundStart_Pre(BattleUnitBufListDetail __instance, BattleUnitModel ____self, List<BattleUnitBuf> ____bufList, List<BattleUnitBuf> ____readyBufList, List<BattleUnitBuf> ____readyReadyBufList)
        {
            try
            {
                foreach (BattleUnitBuf ReadyBuf in ____readyBufList)
                {
                    if (!ReadyBuf.IsDestroyed())
                    {
                        BattleUnitBuf buf = ____bufList.Find((BattleUnitBuf x) => x.GetType() == ReadyBuf.GetType() && !x.IsDestroyed());
                        if (buf != null && !ReadyBuf.independentBufIcon && buf.GetBufIcon() != null)
                        {
                            buf.stack += ReadyBuf.stack;
                            buf.OnAddBuf(ReadyBuf.stack);
                        }
                        else
                        {
                            __instance.AddBuf(ReadyBuf);
                            ReadyBuf.OnAddBuf(ReadyBuf.stack);
                        }
                    }
                }
                ____readyBufList.Clear();
                foreach (BattleUnitBuf ReadyReadyBuf in ____readyReadyBufList)
                {
                    if (!ReadyReadyBuf.IsDestroyed())
                    {
                        BattleUnitBuf rbuf = ____readyBufList.Find((BattleUnitBuf x) => x.GetType() == ReadyReadyBuf.GetType() && !x.IsDestroyed());
                        if (rbuf != null && !ReadyReadyBuf.independentBufIcon && rbuf.GetBufIcon() != null)
                        {
                            rbuf.stack += ReadyReadyBuf.stack;
                            rbuf.OnAddBuf(ReadyReadyBuf.stack);
                        }
                        else
                        {
                            ____readyBufList.Add(ReadyReadyBuf);
                            ReadyReadyBuf.OnAddBuf(ReadyReadyBuf.stack);
                        }
                    }
                }
                ____readyReadyBufList.Clear();
                if (____self.faction == Faction.Player && Singleton<StageController>.Instance.GetStageModel().ClassInfo.chapter == 3)
                {
                    int kewordBufStack = __instance.GetKewordBufStack(KeywordBuf.Endurance);
                    ____self.UnitData.historyInStage.maxEndurance = Mathf.Max(____self.UnitData.historyInStage.maxEndurance, kewordBufStack);
                }
                foreach (BattleUnitBuf battleUnitBuf3 in ____bufList.ToArray())
                {
                    try
                    {
                        if (!battleUnitBuf3.IsDestroyed())
                        {
                            battleUnitBuf3.OnRoundStart();
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                    }
                }
                __instance.CheckDestroyedBuf();
                __instance.CheckAchievements();
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/ReadyBufFixerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //CanAddBuf Apply owner
        [HarmonyPatch(typeof(BattleUnitBufListDetail), "CanAddBuf")]
        [HarmonyPrefix]
        private static void BattleUnitBufListDetail_CanAddBuf_Pre(BattleUnitBufListDetail __instance, BattleUnitModel ____self, BattleUnitBuf buf)
        {
            try
            {
                if (buf == null || ____self == null)
                {
                    return;
                }
                buf._owner = ____self;
            }
            catch { }
        }
        //GetBufIcon
        [HarmonyPatch(typeof(BattleUnitBuf), "GetBufIcon")]
        [HarmonyPrefix]
        private static void BattleUnitBuf_GetBufIcon_Pre(BattleUnitBuf __instance)
        {
            if (ArtWorks == null)
            {
                GetArtWorks();
            }
            if (BattleUnitBuf._bufIconDictionary == null)
            {
                BattleUnitBuf._bufIconDictionary = new Dictionary<string, Sprite>();
            }
            if (BattleUnitBuf._bufIconDictionary.Count == 0)
            {
                Sprite[] array = Resources.LoadAll<Sprite>("Sprites/BufIconSheet/");
                if (array != null && array.Length != 0)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        BattleUnitBuf._bufIconDictionary.Add(array[i].name, array[i]);
                    }
                }
            }
            string keywordIconId = __instance.keywordIconId;
            if (!string.IsNullOrEmpty(keywordIconId) && !BattleUnitBuf._bufIconDictionary.ContainsKey(keywordIconId) && ArtWorks.TryGetValue(keywordIconId, out Sprite sprite))
            {
                BattleUnitBuf._bufIconDictionary[keywordIconId] = sprite;
            }
        }

        //EmotionCard
        //EmotionCardAbilityApply
        [HarmonyPatch(typeof(BattleEmotionCardModel), MethodType.Constructor, new Type[] { typeof(EmotionCardXmlInfo), typeof(BattleUnitModel) })]
        [HarmonyPostfix]
        private static void BattleEmotionCardModel_ctor_Post(BattleEmotionCardModel __instance, EmotionCardXmlInfo xmlInfo, BattleUnitModel owner, ref EmotionCardXmlInfo ____xmlInfo, ref BattleUnitModel ____owner, ref List<EmotionCardAbilityBase> ____abilityList)
        {
            try
            {
                if (____xmlInfo == null)
                {
                    ____xmlInfo = xmlInfo;
                }
                if (____owner == null)
                {
                    ____owner = owner;
                }
                if (____abilityList == null)
                {
                    ____abilityList = new List<EmotionCardAbilityBase>();
                }
                foreach (string text in xmlInfo.Script)
                {
                    EmotionCardAbilityBase emotionCardAbilityBase = FindEmotionCardAbility(text.Trim());
                    if (emotionCardAbilityBase != null)
                    {
                        emotionCardAbilityBase.SetEmotionCard(__instance);
                        ____abilityList.RemoveAll(x => x.GetType().Name.Substring("EmotionCardAbility_".Length).Trim() == text);
                        ____abilityList.Add(emotionCardAbilityBase);
                    }
                }/*
                List<string> list = new List<string>();
                list.AddRange(xmlInfo.Script);
                foreach (EmotionCardAbilityBase emotionCardAbility in ____abilityList)
                {
                    list.Remove(emotionCardAbility.GetType().Name.Substring("EmotionCardAbility_".Length).Trim());
                }
                foreach (string text in list)
                {
                    EmotionCardAbilityBase emotionCardAbilityBase = FindEmotionCardAbility(text.Trim());
                    if (emotionCardAbilityBase != null)
                    {
                        emotionCardAbilityBase.SetEmotionCard(__instance);
                        ____abilityList.Add(emotionCardAbilityBase);
                    }
                }*/
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/SetEmotionAbilityerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        public static EmotionCardAbilityBase FindEmotionCardAbility(string name)
        {
            if (!string.IsNullOrEmpty(name) && CustomEmotionCardAbility.TryGetValue(name, out Type type))
            {
                return Activator.CreateInstance(type) as EmotionCardAbilityBase;
            }
            foreach (Type type2 in Assembly.LoadFile(Application.dataPath + "/Managed/Assembly-CSharp.dll").GetTypes())
            {
                if (type2.Name == "EmotionCardAbility_" + name.Trim())
                {
                    return Activator.CreateInstance(type2) as EmotionCardAbilityBase;
                }
            }
            return null;
        }
        //EmotionCard Preview Artwork
        [HarmonyPatch(typeof(UIAbnormalityCardPreviewSlot), "Init")]
        [HarmonyPostfix]
        private static void UIAbnormalityCardPreviewSlot_Init_Post(UIAbnormalityCardPreviewSlot __instance, EmotionCardXmlInfo card)
        {
            try
            {
                if (ArtWorks == null)
                {
                    GetArtWorks();
                }
                if (ArtWorks.ContainsKey(card.Artwork))
                {
                    __instance.artwork.sprite = ArtWorks[card.Artwork];
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/EmotionCardPreviewArtworkerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //EmotionCard DiceAction Artwork
        [HarmonyPatch(typeof(BattleUnitDiceActionUI_EmotionCard), "Init")]
        [HarmonyPostfix]
        private static void BattleUnitDiceActionUI_EmotionCard_Init_Post(BattleUnitDiceActionUI_EmotionCard __instance, BattleEmotionCardModel card)
        {
            try
            {
                if (ArtWorks == null)
                {
                    GetArtWorks();
                }
                string artwork = card.XmlInfo.Artwork;
                if (ArtWorks.ContainsKey(artwork))
                {
                    __instance.artwork.sprite = ArtWorks[artwork];
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/EmotionCardDiceActionArtworkerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //EmotionCardInven Passive Artwork
        [HarmonyPatch(typeof(UIEmotionPassiveCardInven), "SetSprites")]
        [HarmonyPostfix]
        private static void UIEmotionPassiveCardInven_SetSprites_Post(UIEmotionPassiveCardInven __instance)
        {
            try
            {
                if (ArtWorks == null)
                {
                    GetArtWorks();
                }
                string artwork = __instance._card.Artwork;
                if (ArtWorks.ContainsKey(artwork))
                {
                    __instance._artwork.sprite = ArtWorks[artwork];
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/EmotionCardInvenPassiveArtworkerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //EmotionCard Passive Artwork
        [HarmonyPatch(typeof(EmotionPassiveCardUI), "SetSprites")]
        [HarmonyPostfix]
        private static void EmotionPassiveCardUI_SetSprites_Post(EmotionPassiveCardUI __instance)
        {
            try
            {
                if (ArtWorks == null)
                {
                    GetArtWorks();
                }
                string artwork = __instance._card.Artwork;
                if (ArtWorks.ContainsKey(artwork))
                {
                    __instance._artwork.sprite = ArtWorks[artwork];
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/EmotionCardPassiveArtworkerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        //EGO
        //EGOCardName
        [HarmonyPatch(typeof(BattleDiceCardUI), "SetEgoCardForPopup")]
        [HarmonyPostfix]
        private static void BattleDiceCardUI_SetEgoCardForPopup_Post(EmotionEgoXmlInfo egoxmlinfo, TextMeshProUGUI ___txt_cardName)
        {
            try
            {
                DiceCardXmlInfo cardXmlInfo = ItemXmlDataList.instance.GetCardItem(egoxmlinfo.CardId);
                string cardName = "";
                if (cardXmlInfo._textId <= 0)
                {
                    cardName = GetCardName(cardXmlInfo.id);
                }
                else
                {
                    cardName = GetCardName(new LorId(cardXmlInfo.workshopID, cardXmlInfo._textId));
                }
                if (cardName != "Not Found")
                {
                    ___txt_cardName.text = cardName;
                }
            }
            catch { }
        }
        //EGOId
        [HarmonyPatch(typeof(EmotionEgoXmlInfo), "CardId", MethodType.Getter)]
        [HarmonyPrefix]
        private static bool EmotionEgoXmlInfo_get_CardId_Pre(EmotionEgoXmlInfo __instance, ref LorId __result)
        {
            try
            {
                if (OrcTools.EgoDic.ContainsKey(__instance))
                {
                    __result = OrcTools.EgoDic[__instance];
                    return false;
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/EgoCardIderror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //EGOData
        [HarmonyPatch(typeof(EmotionEgoXmlList), "GetData", new Type[] { typeof(LorId) })]
        [HarmonyPrefix]
        private static bool EmotionEgoXmlList_GetData_Pre(LorId id, ref EmotionEgoXmlInfo __result)
        {
            try
            {
                foreach (KeyValuePair<EmotionEgoXmlInfo, LorId> keyValuePair in OrcTools.EgoDic)
                {
                    if (keyValuePair.Value == id)
                    {
                        __result = keyValuePair.Key;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                File.WriteAllText(Application.dataPath + "/Mods/EgoCardIderror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //EGOName
        [HarmonyPatch(typeof(EmotionEgoCardUI), "Init")]
        [HarmonyPrefix]
        private static bool EmotionEgoCardUI_Init_Pre(EmotionEgoCardUI __instance, EmotionEgoXmlInfo card, ref EmotionEgoXmlInfo ____card, TextMeshProUGUI ____cardName)
        {
            try
            {
                if (OrcTools.EgoDic.ContainsKey(card))
                {
                    ____card = card;
                    DiceCardXmlInfo cardItem = ItemXmlDataList.instance.GetCardItem(OrcTools.EgoDic[card], false);
                    ____cardName.text = cardItem.Name;
                    __instance.gameObject.SetActive(true);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                File.WriteAllText(Application.dataPath + "/Mods/EgoCardUIIniterror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }

        //Story
        //StoryIcon
        [HarmonyPatch(typeof(UISpriteDataManager), "GetStoryIcon")]
        [HarmonyPrefix]
        private static bool UISpriteDataManager_GetStoryIcon_Pre(string story, ref UIIconManager.IconSet __result)
        {
            try
            {
                if (story == null)
                {
                    return true;
                }
                else
                {
                    if (ArtWorks == null)
                    {
                        GetArtWorks();
                    }
                    if (ArtWorks.TryGetValue(story, out Sprite sprite))
                    {
                        __result = new UIIconManager.IconSet
                        {
                            type = story,
                            icon = ArtWorks[story],
                            iconGlow = ArtWorks[story]
                        };
                        if (ArtWorks.ContainsKey(story + "_Glow"))
                        {
                            __result.iconGlow = ArtWorks[story + "_Glow"];
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/StroyIconerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //书库
        [HarmonyPatch(typeof(UIStoryArchivesPanel), "InitData")]
        [HarmonyPrefix]
        private static bool UIStoryArchivesPanel_InitData_Pre(UIStoryArchivesPanel __instance, ref List<BookXmlInfo> ___episodeBooksData, ref List<BookXmlInfo> ___chapterBooksData)
        {
            try
            {
                ___episodeBooksData = new List<BookXmlInfo>();
                ___chapterBooksData = new List<BookXmlInfo>();
                List<BookModel> bookModels = Singleton<BookInventoryModel>.Instance.GetBookListAll();
                List<BookXmlInfo> bookXmlInfoList = new List<BookXmlInfo>();
                for (int i = 0; i < bookModels.Count; i++)
                {
                    if (!bookXmlInfoList.Exists(x => x.id == bookModels[i].ClassInfo.id) && !bookModels[i].ClassInfo.optionList.Contains(BookOption.Basic))
                        bookXmlInfoList.Add(bookModels[i].ClassInfo);
                }
                foreach (BookXmlInfo bookXmlInfo in bookXmlInfoList)
                {
                    if (!bookXmlInfo.id.IsWorkshop())
                    {
                        if (bookXmlInfo.episode > 1)
                        {
                            ___episodeBooksData.Add(bookXmlInfo);
                        }
                        else
                        {
                            ___chapterBooksData.Add(bookXmlInfo);
                        }
                    }
                    else if (OrcTools.EpisodeDic.ContainsKey(bookXmlInfo.id))
                    {
                        ___episodeBooksData.Add(bookXmlInfo);
                    }
                    else
                    {
                        ___chapterBooksData.Add(bookXmlInfo);
                    }
                }
                ___episodeBooksData.Sort((x, y) => x.id.id.CompareTo(y.id.id));
                ___chapterBooksData.Sort((x, y) => x.id.id.CompareTo(y.id.id));
                __instance.sephirahStoryPanel.SetData();
                __instance.battleStoryPanel.SetData();
                __instance.creatureRebattlePanel.SetData();
                if (___episodeBooksData.Count >= 1)
                {
                    __instance.bookStoryPanel.SetData();
                }
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/StoryArchivesIniterror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //书库剧情槽位
        [HarmonyPatch(typeof(UIBookStoryChapterSlot), "SetEpisodeSlots")]
        [HarmonyPrefix]
        private static bool UIBookStoryChapterSlot_SetEpisodeSlots_Pre(UIBookStoryChapterSlot __instance, ref int[] ___episodeIdx, List<UIBookStoryEpisodeSlot> ___EpisodeSlots, UIBookStoryPanel ___panel, UIBookStoryEpisodeSlot ___EpisodeSlotModel, RectTransform ___EpisodeSlotsListRect)
        {
            try
            {
                int i = ___episodeIdx[__instance.chapter - 1] + 1;
                int j = 0;
                while (i < ___episodeIdx[__instance.chapter] + 1)
                {
                    if (i != 0 && ___panel.panel.GetEpisodeBooksData((UIStoryLine)i).Count > 0)
                    {
                        if (___EpisodeSlots.Count <= j)
                        {
                            __instance.InstatiateAdditionalSlot();
                        }
                        ___EpisodeSlots[j].Init(___panel.panel.GetEpisodeBooksData((UIStoryLine)i), __instance);
                        j++;
                    }
                    i++;
                }
                Dictionary<LorId, List<BookXmlInfo>> dictionary = new Dictionary<LorId, List<BookXmlInfo>>();
                foreach (BookXmlInfo bookXmlInfo in ___panel.panel.GetEpisodeBooksDataAll())
                {
                    if (OrcTools.EpisodeDic.ContainsKey(bookXmlInfo.id) && Singleton<StageClassInfoList>.Instance.GetData(OrcTools.EpisodeDic[bookXmlInfo.id]).chapter == __instance.chapter && !Enum.TryParse(bookXmlInfo.BookIcon, out UIStoryLine uistoryLine))
                    {
                        if (!dictionary.ContainsKey(OrcTools.EpisodeDic[bookXmlInfo.id]))
                        {
                            dictionary[OrcTools.EpisodeDic[bookXmlInfo.id]] = new List<BookXmlInfo>();
                        }
                        dictionary[OrcTools.EpisodeDic[bookXmlInfo.id]].Add(bookXmlInfo);
                    }
                }
                if (dictionary.Count > 0)
                {
                    foreach (KeyValuePair<LorId, List<BookXmlInfo>> keyValuePair in dictionary)
                    {
                        if (___EpisodeSlots.Count <= j)
                        {
                            __instance.InstatiateAdditionalSlot();
                        }
                        ___EpisodeSlots[j].Init(keyValuePair.Value, __instance);
                        if (OrcTools.StageNameDic.TryGetValue(keyValuePair.Key, out string stagename))
                        {
                            ___EpisodeSlots[j].episodeText.text = stagename;
                        }
                        j++;
                    }
                }
                if (___panel.panel.GetChapterBooksData(__instance.chapter).Count > 0)
                {
                    if (___EpisodeSlots.Count <= j)
                    {
                        UIBookStoryEpisodeSlot item = UnityEngine.Object.Instantiate(___EpisodeSlotModel, ___EpisodeSlotsListRect);
                        ___EpisodeSlots.Add(item);
                    }
                    ___EpisodeSlots[j].Init(__instance.chapter, ___panel.panel.GetChapterBooksData(__instance.chapter), __instance);
                    j++;
                }
                while (j < ___EpisodeSlots.Count)
                {
                    ___EpisodeSlots[j].Deactive();
                    j++;
                }
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/UBSCSSESerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //EpisodeSlotName
        [HarmonyPatch(typeof(UIBookStoryPanel), "OnSelectEpisodeSlot")]
        [HarmonyPrefix]
        private static void UIBookStoryPanel_OnSelectEpisodeSlot_Post(UIBookStoryEpisodeSlot slot, TextMeshProUGUI ___selectedEpisodeText)
        {
            try
            {
                if (slot != null && slot.books.Count > 0 && OrcTools.EpisodeDic.ContainsKey(slot.books[0].id) && OrcTools.StageNameDic.TryGetValue(OrcTools.EpisodeDic[slot.books[0].id], out string stagename))
                {
                    ___selectedEpisodeText.text = stagename;
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/OnSelectEpisodeSloterror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //MoreLanguageStoryIdentify
        [HarmonyPatch(typeof(StorySerializer), "HasEffectFile")]
        [HarmonyPrefix]
        private static bool StorySerializer_HasEffectFile_Pre(StageStoryInfo stageStoryInfo, ref bool __result)
        {
            try
            {
                if (stageStoryInfo == null)
                {
                    return false;
                }
                if (stageStoryInfo.IsMod)
                {
                    string modPath = Singleton<ModContentManager>.Instance.GetModPath(stageStoryInfo.packageId);
                    string storyFilePath = Path.Combine(modPath, "Data", "StoryText");
                    string[] array = stageStoryInfo.story.Split(new char[]
                    {
                         '.'
                    });
                    if (Directory.Exists(storyFilePath))
                    {
                        foreach (FileInfo fileInfo in new DirectoryInfo(storyFilePath).GetFiles())
                        {
                            if (Path.GetFileNameWithoutExtension(fileInfo.FullName) == array[0])
                            {
                                __result = true;
                            }
                        }
                    }
                    storyFilePath = Path.Combine(modPath, "Data", "StoryText", TextDataModel.CurrentLanguage);
                    if (Directory.Exists(storyFilePath))
                    {
                        foreach (FileInfo fileInfo in new DirectoryInfo(storyFilePath).GetFiles())
                        {
                            if (Path.GetFileNameWithoutExtension(fileInfo.FullName) == array[0])
                            {
                                __result = true;
                            }
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/HasEffectFileerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //LoadStageStory
        [HarmonyPatch(typeof(StorySerializer), "LoadStageStory")]
        [HarmonyPrefix]
        private static bool StorySerializer_LoadStageStory_Pre(StageStoryInfo stageStoryInfo, ref bool __result)
        {
            try
            {
                if (stageStoryInfo == null)
                {
                    return false;
                }
                if (stageStoryInfo.IsMod)
                {
                    string modPath = Singleton<ModContentManager>.Instance.GetModPath(stageStoryInfo.packageId);
                    string storyFilePath = Path.Combine(modPath, "Data", "StoryText");
                    string effectFilePath = Path.Combine(modPath, "Data", "StoryEffect");
                    string storyPath = Path.Combine(modPath, "Data", "StoryText", stageStoryInfo.story);
                    string effectPath = Path.Combine(modPath, "Data", "StoryEffect", stageStoryInfo.story);
                    string[] array = stageStoryInfo.story.Split(new char[]
                    {
                         '.'
                    });
                    if (Directory.Exists(effectFilePath))
                    {
                        foreach (FileInfo fileInfo in new DirectoryInfo(effectFilePath).GetFiles())
                        {
                            if (Path.GetFileNameWithoutExtension(fileInfo.FullName) == array[0])
                            {
                                effectPath = fileInfo.FullName;
                            }
                        }
                    }
                    if (Directory.Exists(storyFilePath))
                    {
                        foreach (FileInfo fileInfo in new DirectoryInfo(storyFilePath).GetFiles())
                        {
                            if (Path.GetFileNameWithoutExtension(fileInfo.FullName) == array[0])
                            {
                                storyPath = fileInfo.FullName;
                            }
                        }
                    }
                    storyFilePath = Path.Combine(modPath, "Data", "StoryText", TextDataModel.CurrentLanguage);
                    if (Directory.Exists(storyFilePath))
                    {
                        foreach (FileInfo fileInfo in new DirectoryInfo(storyFilePath).GetFiles())
                        {
                            if (Path.GetFileNameWithoutExtension(fileInfo.FullName) == array[0])
                            {
                                storyPath = fileInfo.FullName;
                            }
                        }
                    }
                    __result = StorySerializer.LoadStoryFile(storyPath, effectPath, modPath);
                    return false;
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/LoadStageStoryerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //BattleStoryCG
        [HarmonyPatch(typeof(UIBattleStoryInfoPanel), "SetData")]
        [HarmonyPostfix]
        private static void UIBattleStoryInfoPanel_SetData_Post(UIBattleStoryInfoPanel __instance, StageClassInfo stage, ref Image ___CG)
        {
            try
            {
                if (stage != null && stage.id.IsWorkshop())
                {
                    string path = Path.Combine(Singleton<ModContentManager>.Instance.GetModPath(stage.workshopID), "Resource", "StoryBgSprite", StorySerializer.effectDefinition.cg.src);
                    Sprite result = GetModStoryCG(stage.id, path);
                    if (result != null)
                    {
                        ___CG.sprite = result;
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/UIStoryInfo_SDerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //StorySceneCG
        [HarmonyPatch(typeof(StoryScene.StoryManager), "LoadBackgroundSprite")]
        [HarmonyPrefix]
        private static bool StoryManager_LoadBackgroundSprite_Pre(string src, ref Dictionary<string, Sprite> ____loadedCustomSprites, ref Sprite __result)
        {
            try
            {
                Sprite sprite = Resources.Load<Sprite>("StoryResource/BgSprites/" + src);
                if (sprite == null && !____loadedCustomSprites.TryGetValue("bgsprite:" + src, out sprite))
                {
                    if (!File.Exists(Path.Combine(ModUtil.GetModBgSpritePath(StorySerializer.curModPath), src)))
                    {
                        src += ".png";
                    }
                    sprite = SpriteUtil.LoadSprite(Path.Combine(ModUtil.GetModBgSpritePath(StorySerializer.curModPath), src), new Vector2(0.5f, 0.5f));
                    if (sprite != null)
                    {
                        ____loadedCustomSprites.Add("bgsprite:" + src, sprite);
                    }
                }
                __result = sprite;
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/LoadBackgroundSpriteerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        public static Sprite GetModStoryCG(LorId StageId, string Path)
        {
            if (ModStoryCG.TryGetValue(StageId, out ModStroyCG result))
            {
                return result.sprite;
            }
            else if (File.Exists(Path))
            {
                Texture2D texture2D = new Texture2D(2, 2);
                texture2D.LoadImage(File.ReadAllBytes(Path));
                Sprite value = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
                ModStoryCG[StageId] = new ModStroyCG
                {
                    path = Path,
                    sprite = value,
                };
                return value;
            }
            else if (File.Exists(Path + ".png"))
            {
                Texture2D texture2D = new Texture2D(2, 2);
                texture2D.LoadImage(File.ReadAllBytes(Path + ".png"));
                Sprite value = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
                ModStoryCG[StageId] = new ModStroyCG
                {
                    path = Path,
                    sprite = value,
                };
                return value;
            }
            return null;
        }
        //EntryCG
        [HarmonyPatch(typeof(StageController), "GameOver")]
        [HarmonyPostfix]
        private static void StageController_GameOver_Post(StageModel ____stageModel)
        {
            try
            {
                if (ModStoryCG.ContainsKey(____stageModel.ClassInfo.id))
                {
                    ModSaveTool.SaveString("ModLastStroyCG", ModStoryCG[____stageModel.ClassInfo.id].path, "BaseMod");
                }
                else if (TryAddModStoryCG(____stageModel.ClassInfo))
                {
                    ModSaveTool.SaveString("ModLastStroyCG", ModStoryCG[____stageModel.ClassInfo.id].path, "BaseMod");
                }
                else
                {
                    ModSaveTool.SaveString("ModLastStroyCG", "", "BaseMod");
                }

            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/SaveModCGerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        private static bool TryAddModStoryCG(StageClassInfo stageClassInfo)
        {
            if (stageClassInfo.GetStartStory() == null)
            {
                return false;
            }
            if (!stageClassInfo.id.IsWorkshop())
            {
                return false;
            }
            string modPath = Singleton<ModContentManager>.Instance.GetModPath(stageClassInfo.GetStartStory().packageId);
            if (string.IsNullOrEmpty(modPath) || string.IsNullOrEmpty(stageClassInfo.GetStartStory().story))
            {
                return false;
            }
            string effectPath = Path.Combine(modPath, "Data", "StoryEffect", stageClassInfo.GetStartStory().story);
            string[] array = stageClassInfo.GetStartStory().story.Split(new char[]
            {
                        '.'
            });
            string cg = string.Empty;
            if (File.Exists(Path.Combine(modPath, "Data", "StoryEffect", array[0] + ".xml")))
            {
                effectPath = Path.Combine(modPath, "Data", "StoryEffect", array[0] + ".xml");
            }
            if (File.Exists(Path.Combine(modPath, "Data", "StoryEffect", array[0] + ".txt")))
            {
                effectPath = Path.Combine(modPath, "Data", "StoryEffect", array[0] + ".txt");
            }
            if (File.Exists(effectPath))
            {
                using (StreamReader streamReader2 = new StreamReader(effectPath))
                {
                    cg = ((SceneEffect)new XmlSerializer(typeof(SceneEffect)).Deserialize(streamReader2)).cg.src;
                }
            }
            if (string.IsNullOrEmpty(cg))
            {
                return false;
            }
            string path = Path.Combine(modPath, "Resource", "StoryBgSprite", cg);
            return GetModStoryCG(stageClassInfo.id, path) != null;
        }
        //EntryCG
        [HarmonyPatch(typeof(EntryScene), "SetCG")]
        [HarmonyPostfix]
        private static void EntryScene_SetCG_Post(EntryScene __instance, ref LatestDataModel ____latestData)
        {
            try
            {
                string stroycg = ModSaveTool.GetModSaveData("BaseMod").GetString("ModLastStroyCG");
                if (!string.IsNullOrEmpty(stroycg) && File.Exists(stroycg))
                {
                    Texture2D texture2D = new Texture2D(1, 1);
                    texture2D.LoadImage(File.ReadAllBytes(stroycg));
                    Sprite value = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
                    __instance.CGImage.sprite = value;
                }
            }
            catch (Exception ex)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                File.WriteAllText(Application.dataPath + "/Mods/SetEntrySceneCGerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //Condition
        [HarmonyPatch(typeof(StageExtraCondition), "IsUnlocked")]
        [HarmonyPrefix]
        private static bool StageExtraCondition_IsUnlocked_Pre(StageExtraCondition __instance, ref bool __result)
        {
            try
            {
                if (LibraryModel.Instance.GetLibraryLevel() < __instance.needLevel)
                {
                    __result = false;
                    return false;
                }
                if (!OrcTools.StageConditionDic.ContainsKey(__instance))
                {
                    return true;
                }
                foreach (LorId stageId in OrcTools.StageConditionDic[__instance])
                {
                    if (LibraryModel.Instance.ClearInfo.GetClearCount(stageId) <= 0)
                    {
                        __result = false;
                        return false;
                    }
                }
                __result = true;
                return false;
            }
            catch (Exception ex)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                File.WriteAllText(Application.dataPath + "/Mods/StageConditionerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //Condition
        [HarmonyPatch(typeof(UIInvitationRightMainPanel), "SendInvitation")]
        [HarmonyPrefix]
        private static bool UIInvitationRightMainPanel_SendInvitation_Pre(UIInvitationRightMainPanel __instance, GameObject ___ob_tutorialSendButtonhighlight, GameObject ___ob_tutorialConfirmButtonhighlight)
        {
            try
            {
                StageClassInfo bookRecipe = __instance.GetBookRecipe();
                if (bookRecipe == null)
                {
                    return true;
                }
                if (bookRecipe.extraCondition == null)
                {
                    return true;
                }
                if (!OrcTools.StageConditionDic.ContainsKey(bookRecipe.extraCondition))
                {
                    return true;
                }
                if (___ob_tutorialSendButtonhighlight.activeSelf)
                {
                    ___ob_tutorialSendButtonhighlight.SetActive(false);
                    SingletonBehavior<UIMainAutoTooltipManager>.Instance.AllCloseTooltip();
                }
                if (___ob_tutorialConfirmButtonhighlight.activeSelf)
                {
                    ___ob_tutorialConfirmButtonhighlight.SetActive(false);
                }
                if (bookRecipe != null)
                {
                    if (LibraryModel.Instance.GetChapter() < bookRecipe.chapter)
                    {
                        UIAlarmPopup.instance.SetAlarmText(UIAlarmType.ClosedStage, UIAlarmButtonType.Default, null, "", "");
                    }
                    else
                    {
                        bool flag = true;
                        foreach (LorId stageId in OrcTools.StageConditionDic[bookRecipe.extraCondition])
                        {
                            if (LibraryModel.Instance.ClearInfo.GetClearCount(stageId) == 0)
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            UIAlarmPopup.instance.SetAlarmText(UIAlarmType.ClosedStage, UIAlarmButtonType.Default, null, "", "");
                        }
                        else
                        {
                            __instance.confirmAreaRoot.SetActive(true);
                            if (bookRecipe.id == 2 && !LibraryModel.Instance.IsClearRats())
                            {
                                ___ob_tutorialConfirmButtonhighlight.gameObject.SetActive(true);
                                SingletonBehavior<UIMainAutoTooltipManager>.Instance.OpenTooltip(UIAutoTooltipType.Invitation_Click_ConfirmButton, ___ob_tutorialConfirmButtonhighlight.transform as RectTransform, false, null, UIToolTipPanelType.Normal, UITooltipPanelPositionType.None, false, null);
                            }
                        }
                    }
                }
                else
                {
                    UIAlarmPopup.instance.SetAlarmText(UIAlarmType.ClosedStage, UIAlarmButtonType.Default, null, "", "");
                }
                UIControlManager.Instance.SelectSelectableForcely(__instance.confirmButtonSelectable, false);
                return false;
            }
            catch (Exception ex)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                File.WriteAllText(Application.dataPath + "/Mods/SendInvitationerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }

        //EquipPageInvenPanel
        //MoreEuipPageUI
        [HarmonyPatch(typeof(UIOriginEquipPageList), "UpdateEquipPageList")]
        [HarmonyPrefix]
        private static bool UIOriginEquipPageList_UpdateEquipPageList_Pre(UIOriginEquipPageList __instance)
        {
            try
            {
                List<UIOriginEquipPageSlot> list = __instance.equipPageSlotList;
                int i;
                for (i = 0; i < __instance.currentScreenBookModelList.Count; i++)
                {
                    if (i == list.Count)
                    {
                        UIOriginEquipPageSlot uioriginEquipPageSlot = UnityEngine.Object.Instantiate(list[0], list[0].transform.parent);
                        /*UIOriginEquipPageSlot uioriginEquipPageSlot = UtilTools.DuplicateEquipPageSlot(list[0], __instance);*/
                        uioriginEquipPageSlot.Initialized();
                        list.Add(uioriginEquipPageSlot);
                    }
                    list[i].SetActiveSlot(true);
                    list[i].SetData(__instance.currentScreenBookModelList[i]);
                }
                if (i < list.Count)
                {
                    for (int j = i; j < list.Count; j++)
                    {
                        list[j].SetActiveSlot(false);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/UIOEPLUEPLerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //UIEquipPageScrollList
        [HarmonyPatch(typeof(UIEquipPageScrollList), "SetData")]
        [HarmonyPrefix]
        private static bool UIEquipPageScrollList_SetData_Pre(UIEquipPageScrollList __instance, List<BookModel> books, UnitDataModel unit, ref int ___currentslotcount, ref UnitDataModel ____selectedUnit, ref List<BookModel> ____originBookModelList, ref List<BookModel> ___currentBookModelList, ref bool ___isActiveScrollBar, ref List<UIStoryKeyData> ___totalkeysdata, ref List<UISlotHeightData> ___heightdatalist, ref Dictionary<UIStoryKeyData, List<BookModel>> ___currentStoryBooksDic, ref bool ___isClickedUpArrow, ref bool ___isClickedDownArrow, ref RectTransform ___rect_slotListRoot, ref List<UISettingInvenEquipPageListSlot> ____equipPagesPanelSlotList, bool init = false)
        {
            try
            {
                ModEpMatch = new Dictionary<LorId, int>();
                if (init)
                {
                    __instance.CurrentSelectedBook = null;
                    ___currentslotcount = 0;
                }
                ____selectedUnit = unit;
                ____originBookModelList.Clear();
                ___currentBookModelList.Clear();
                ___isActiveScrollBar = false;
                ____originBookModelList.AddRange(books);
                ___currentBookModelList.AddRange(books);
                ___currentBookModelList = __instance.FilterBookModels(___currentBookModelList);
                /*___currentBookModelList = (List<BookModel>)__instance.GetType().GetMethod("FilterBookModels", AccessTools.all).Invoke(__instance, new object[]
                                {
                                    ___currentBookModelList
                                });*/
                ___totalkeysdata.Clear();
                __instance.CurrentSelectedBook = null;
                ___heightdatalist.Clear();
                ___currentStoryBooksDic.Clear();
                int num = 200;
                /*int num = Enum.GetValues(typeof(UIStoryLine)).Length - 1;*/
                foreach (BookModel bookModel in ___currentBookModelList)
                {
                    string bookIcon = bookModel.ClassInfo.BookIcon;
                    UIStoryKeyData uistoryKeyData;
                    if (bookModel.IsWorkshop || !Enum.IsDefined(typeof(UIStoryLine), bookIcon))
                    {
                        if (OrcTools.EpisodeDic.ContainsKey(bookModel.BookId))
                        {
                            if (!ModEpMatch.ContainsKey(OrcTools.EpisodeDic[bookModel.BookId]))
                            {
                                num++;
                                ModEpMatch.Add(OrcTools.EpisodeDic[bookModel.BookId], num);
                                uistoryKeyData = new UIStoryKeyData(bookModel.ClassInfo.Chapter, bookModel.ClassInfo.id.packageId)
                                {
                                    StoryLine = (UIStoryLine)num
                                };
                                ___totalkeysdata.Add(uistoryKeyData);
                            }
                            else
                            {
                                uistoryKeyData = ___totalkeysdata.Find((UIStoryKeyData x) => x.workshopId == bookModel.ClassInfo.id.packageId && x.chapter == bookModel.ClassInfo.Chapter && x.StoryLine == (UIStoryLine)ModEpMatch[OrcTools.EpisodeDic[bookModel.BookId]]);
                            }
                        }
                        else
                        {
                            uistoryKeyData = ___totalkeysdata.Find((UIStoryKeyData x) => x.chapter == bookModel.ClassInfo.Chapter && x.workshopId == bookModel.ClassInfo.workshopID);
                            if (uistoryKeyData == null)
                            {
                                uistoryKeyData = new UIStoryKeyData(bookModel.ClassInfo.Chapter, bookModel.ClassInfo.id.packageId);
                                ___totalkeysdata.Add(uistoryKeyData);
                            }
                        }
                    }
                    else
                    {
                        if (!Enum.IsDefined(typeof(UIStoryLine), bookIcon))
                        {
                            Debug.LogError(bookIcon + "스토리 string enum 변환 오류");
                            continue;
                        }
                        UIStoryLine storyLine = (UIStoryLine)Enum.Parse(typeof(UIStoryLine), bookIcon);
                        uistoryKeyData = ___totalkeysdata.Find((UIStoryKeyData x) => x.chapter == bookModel.ClassInfo.Chapter && x.StoryLine == storyLine);
                        if (uistoryKeyData == null)
                        {
                            uistoryKeyData = new UIStoryKeyData(bookModel.ClassInfo.Chapter, storyLine);
                            ___totalkeysdata.Add(uistoryKeyData);
                        }
                    }
                    if (!___currentStoryBooksDic.ContainsKey(uistoryKeyData))
                    {
                        List<BookModel> list = new List<BookModel>
                            {
                                bookModel
                            };
                        ___currentStoryBooksDic.Add(uistoryKeyData, list);
                    }
                    else
                    {
                        ___currentStoryBooksDic[uistoryKeyData].Add(bookModel);
                    }
                }
                ___totalkeysdata.Sort(delegate (UIStoryKeyData x, UIStoryKeyData y)
                {
                    if (x.chapter == -1 && y.chapter == -1)
                    {
                        return 0;
                    }
                    if (x.chapter < y.chapter)
                    {
                        return -1;
                    }
                    if (x.chapter > y.chapter)
                    {
                        return 1;
                    }
                    int comparenum = x.workshopId.CompareTo(y.workshopId);
                    if (comparenum > 0)
                    {
                        return -1;
                    }
                    if (comparenum < 0)
                    {
                        return 1;
                    }
                    if (x.StoryLine < y.StoryLine)
                    {
                        return -1;
                    }
                    if (x.StoryLine > y.StoryLine)
                    {
                        return 1;
                    }
                    return 0;
                });
                ___totalkeysdata.Reverse();
                __instance.CalculateSlotsHeight();
                __instance.UpdateSlotList();
                __instance.SetScrollBar();
                ___isClickedUpArrow = false;
                ___isClickedDownArrow = false;
                LayoutRebuilder.ForceRebuildLayoutImmediate(___rect_slotListRoot);
                UIOriginEquipPageSlot saveFirstChild = ____equipPagesPanelSlotList[0].EquipPageSlotList[0];
                __instance.SetSaveFirstChild(saveFirstChild);
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/EPSLSDerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //UISettingEquipPageScrollList
        [HarmonyPatch(typeof(UISettingEquipPageScrollList), "SetData")]
        [HarmonyPrefix]
        private static bool UISettingEquipPageScrollList_SetData_Pre(UISettingEquipPageScrollList __instance, List<BookModel> books, UnitDataModel unit, ref int ___currentslotcount, ref UnitDataModel ____selectedUnit, ref List<BookModel> ____originBookModelList, ref List<BookModel> ___currentBookModelList, ref bool ___isActiveScrollBar, ref List<UIStoryKeyData> ___totalkeysdata, ref List<UISlotHeightData> ___heightdatalist, ref Dictionary<UIStoryKeyData, List<BookModel>> ___currentStoryBooksDic, ref bool ___isClickedUpArrow, ref bool ___isClickedDownArrow, ref RectTransform ___rect_slotListRoot, ref List<UISettingInvenEquipPageListSlot> ____equipPagesPanelSlotList, bool init = false)
        {
            try
            {
                ModEpMatch = new Dictionary<LorId, int>();
                if (init)
                {
                    __instance.CurrentSelectedBook = null;
                    ___currentslotcount = 0;
                }
                ____selectedUnit = unit;
                ____originBookModelList.Clear();
                ___currentBookModelList.Clear();
                ___isActiveScrollBar = false;
                ____originBookModelList.AddRange(books);
                ___currentBookModelList.AddRange(books);
                ___currentBookModelList = __instance.FilterBookModels(___currentBookModelList);
                /*___currentBookModelList = (List<BookModel>)__instance.GetType().GetMethod("FilterBookModels", AccessTools.all).Invoke(__instance, new object[]
                {
                    ___currentBookModelList
                });*/
                ___totalkeysdata.Clear();
                __instance.CurrentSelectedBook = null;
                ___heightdatalist.Clear();
                ___currentStoryBooksDic.Clear();
                int num = 200;
                /*int num = Enum.GetValues(typeof(UIStoryLine)).Length - 1;*/
                foreach (BookModel bookModel in ___currentBookModelList)
                {
                    string bookIcon = bookModel.ClassInfo.BookIcon;
                    UIStoryKeyData uistoryKeyData;
                    if (bookModel.IsWorkshop || !Enum.IsDefined(typeof(UIStoryLine), bookIcon))
                    {
                        if (OrcTools.EpisodeDic.ContainsKey(bookModel.BookId))
                        {
                            if (!ModEpMatch.ContainsKey(OrcTools.EpisodeDic[bookModel.BookId]))
                            {
                                num++;
                                ModEpMatch.Add(OrcTools.EpisodeDic[bookModel.BookId], num);
                                uistoryKeyData = new UIStoryKeyData(bookModel.ClassInfo.Chapter, bookModel.ClassInfo.id.packageId)
                                {
                                    StoryLine = (UIStoryLine)num
                                };
                                ___totalkeysdata.Add(uistoryKeyData);
                            }
                            else
                            {
                                uistoryKeyData = ___totalkeysdata.Find((UIStoryKeyData x) => x.workshopId == bookModel.ClassInfo.id.packageId && x.chapter == bookModel.ClassInfo.Chapter && x.StoryLine == (UIStoryLine)ModEpMatch[OrcTools.EpisodeDic[bookModel.BookId]]);
                            }
                        }
                        else
                        {
                            uistoryKeyData = ___totalkeysdata.Find((UIStoryKeyData x) => x.chapter == bookModel.ClassInfo.Chapter && x.workshopId == bookModel.ClassInfo.workshopID);
                            if (uistoryKeyData == null)
                            {
                                uistoryKeyData = new UIStoryKeyData(bookModel.ClassInfo.Chapter, bookModel.ClassInfo.id.packageId);
                                ___totalkeysdata.Add(uistoryKeyData);
                            }
                        }
                    }
                    else
                    {
                        if (!Enum.IsDefined(typeof(UIStoryLine), bookIcon))
                        {
                            Debug.LogError(bookIcon + "스토리 string enum 변환 오류");
                            continue;
                        }
                        UIStoryLine storyLine = (UIStoryLine)Enum.Parse(typeof(UIStoryLine), bookIcon);
                        uistoryKeyData = ___totalkeysdata.Find((UIStoryKeyData x) => x.chapter == bookModel.ClassInfo.Chapter && x.StoryLine == storyLine);
                        if (uistoryKeyData == null)
                        {
                            uistoryKeyData = new UIStoryKeyData(bookModel.ClassInfo.Chapter, storyLine);
                            ___totalkeysdata.Add(uistoryKeyData);
                        }
                    }
                    if (!___currentStoryBooksDic.ContainsKey(uistoryKeyData))
                    {
                        List<BookModel> list = new List<BookModel>
                            {
                                bookModel
                            };
                        ___currentStoryBooksDic.Add(uistoryKeyData, list);
                    }
                    else
                    {
                        ___currentStoryBooksDic[uistoryKeyData].Add(bookModel);
                    }
                }
                ___totalkeysdata.Sort(delegate (UIStoryKeyData x, UIStoryKeyData y)
                {
                    if (x.chapter == -1 && y.chapter == -1)
                    {
                        return 0;
                    }
                    if (x.chapter < y.chapter)
                    {
                        return -1;
                    }
                    if (x.chapter > y.chapter)
                    {
                        return 1;
                    }
                    int comparenum = x.workshopId.CompareTo(y.workshopId);
                    if (comparenum > 0)
                    {
                        return -1;
                    }
                    if (comparenum < 0)
                    {
                        return 1;
                    }
                    if (x.StoryLine < y.StoryLine)
                    {
                        return -1;
                    }
                    if (x.StoryLine > y.StoryLine)
                    {
                        return 1;
                    }
                    return 0;
                });
                ___totalkeysdata.Reverse();
                __instance.CalculateSlotsHeight();
                __instance.UpdateSlotList();
                __instance.SetScrollBar();
                ___isClickedUpArrow = false;
                ___isClickedDownArrow = false;
                LayoutRebuilder.ForceRebuildLayoutImmediate(___rect_slotListRoot);
                UIOriginEquipPageSlot saveFirstChild = ____equipPagesPanelSlotList[0].EquipPageSlotList[0];
                __instance.SetSaveFirstChild(saveFirstChild);
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/SEPSLSDerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //UISettingInvenEquipPageListSlot
        [HarmonyPatch(typeof(UISettingInvenEquipPageListSlot), "SetBooksData")]
        [HarmonyPrefix]
        private static bool UISettingInvenEquipPageListSlot_SetBooksData_Pre(UISettingInvenEquipPageListSlot __instance, List<BookModel> books, UIStoryKeyData storyKey, ref Image ___img_IconGlow, ref Image ___img_Icon, ref TextMeshProUGUI ___txt_StoryName, UISettingEquipPageScrollList ___listRoot, ref List<UIOriginEquipPageSlot> ___equipPageSlotList)
        {
            try
            {
                if (storyKey.StoryLine < (UIStoryLine)Enum.GetValues(typeof(UIStoryLine)).Length)
                {
                    return true;
                }
                if (books.Count < 0)
                {
                    return true;
                }
                if (books.Count >= 0)
                {
                    if (!OrcTools.EpisodeDic.ContainsKey(books[0].BookId))
                    {
                        return true;
                    }
                    StageClassInfo data = Singleton<StageClassInfoList>.Instance.GetData(OrcTools.EpisodeDic[books[0].BookId]);
                    if (data == null)
                    {
                        return true;
                    }
                    UIIconManager.IconSet storyIcon = UISpriteDataManager.instance.GetStoryIcon(data.storyType);
                    if (storyIcon != null)
                    {
                        ___img_IconGlow.enabled = true;
                        ___img_Icon.enabled = true;
                        ___img_Icon.sprite = storyIcon.icon;
                        ___img_IconGlow.sprite = storyIcon.iconGlow;
                    }
                    if (OrcTools.StageNameDic.TryGetValue(data.id, out string stageName))
                    {
                        ___txt_StoryName.text = "workshop " + stageName;
                    }
                }
                __instance.SetFrameColor(UIColorManager.Manager.GetUIColor(UIColor.Default));
                List<BookModel> list = __instance.ApplyFilterBooksInStory(books);
                /*List<BookModel> list = new List<BookModel>((List<BookModel>)typeof(UISettingInvenEquipPageListSlot).GetMethod("ApplyFilterBooksInStory", AccessTools.all).Invoke(__instance, new object[]
                    {
                        books
                    }));*/
                __instance.SetEquipPagesData(list);
                BookModel bookModel = list.Find((BookModel x) => x == UI.UIController.Instance.CurrentUnit.bookItem);
                if (___listRoot.CurrentSelectedBook == null && bookModel != null)
                {
                    ___listRoot.CurrentSelectedBook = bookModel;
                }
                if (___listRoot.CurrentSelectedBook != null)
                {
                    UIOriginEquipPageSlot uioriginEquipPageSlot = ___equipPageSlotList.Find((UIOriginEquipPageSlot x) => x.BookDataModel == ___listRoot.CurrentSelectedBook);
                    if (uioriginEquipPageSlot != null)
                    {
                        uioriginEquipPageSlot.SetHighlighted(true, true, false);
                    }
                }
                __instance.SetSlotSize();
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/SIEPLSSBDerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //UIInvenEquipPageListSlot
        [HarmonyPatch(typeof(UIInvenEquipPageListSlot), "SetBooksData")]
        [HarmonyPrefix]
        private static bool UIInvenEquipPageListSlot_SetBooksData_Pre(UIInvenEquipPageListSlot __instance, List<BookModel> books, UIStoryKeyData storyKey, ref Image ___img_IconGlow, ref Image ___img_Icon, ref TextMeshProUGUI ___txt_StoryName, UISettingEquipPageScrollList ___listRoot, ref List<UIOriginEquipPageSlot> ___equipPageSlotList)
        {
            try
            {
                if (storyKey.StoryLine < (UIStoryLine)Enum.GetValues(typeof(UIStoryLine)).Length)
                {
                    return true;
                }
                if (books.Count < 0)
                {
                    return true;
                }
                if (books.Count >= 0)
                {/*
                    LorId key = new LorId(-1);
                    foreach (KeyValuePair<LorId, int> keyValuePair in ModEpMatch)
                    {
                        {
                            if (keyValuePair.Value == (int)storyKey.StoryLine)
                            {
                                key = keyValuePair.Key;
                                break;
                            }
                        }
                    }
                    if (key.id <= 0)
                    {
                        return true;
                    }
                    StageClassInfo data = Singleton<StageClassInfoList>.Instance.GetData(key);*/
                    if (!OrcTools.EpisodeDic.ContainsKey(books[0].BookId))
                    {
                        return true;
                    }
                    StageClassInfo data = Singleton<StageClassInfoList>.Instance.GetData(OrcTools.EpisodeDic[books[0].BookId]);
                    if (data == null)
                    {
                        return true;
                    }
                    UIIconManager.IconSet storyIcon = UISpriteDataManager.instance.GetStoryIcon(data.storyType);
                    if (storyIcon != null)
                    {
                        ___img_IconGlow.enabled = true;
                        ___img_Icon.enabled = true;
                        ___img_Icon.sprite = storyIcon.icon;
                        ___img_IconGlow.sprite = storyIcon.iconGlow;
                    }
                    if (OrcTools.StageNameDic.TryGetValue(data.id, out string stageName))
                    {
                        ___txt_StoryName.text = "workshop " + stageName;
                    }
                }
                __instance.SetFrameColor(UIColorManager.Manager.GetUIColor(UIColor.Default));
                List<BookModel> list = __instance.ApplyFilterBooksInStory(books);
                /*List<BookModel> list = new List<BookModel>((List<BookModel>)typeof(UIInvenEquipPageListSlot).GetMethod("ApplyFilterBooksInStory", AccessTools.all).Invoke(__instance, new object[]
                    {
                        books
                    }));*/
                __instance.SetEquipPagesData(list);
                BookModel bookModel = list.Find((BookModel x) => x == UI.UIController.Instance.CurrentUnit.bookItem);
                if (___listRoot.CurrentSelectedBook == null && bookModel != null)
                {
                    ___listRoot.CurrentSelectedBook = bookModel;
                }
                if (___listRoot.CurrentSelectedBook != null)
                {
                    UIOriginEquipPageSlot uioriginEquipPageSlot = ___equipPageSlotList.Find((UIOriginEquipPageSlot x) => x.BookDataModel == ___listRoot.CurrentSelectedBook);
                    if (uioriginEquipPageSlot != null)
                    {
                        uioriginEquipPageSlot.SetHighlighted(true, true, false);
                    }
                }
                __instance.SetSlotSize();
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/IEPLSSBDerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }

        //CustomMapManager
        //InitCustomMap
        [HarmonyPatch(typeof(StageController), "InitializeMap")]
        [HarmonyPrefix]
        private static bool StageController_InitializeMap_Pre(StageController __instance)
        {
            try
            {
                if (__instance.stageType != StageType.Invitation)
                {
                    return true;
                }
                else
                {
                    SingletonBehavior<BattleSceneRoot>.Instance.HideAllFloorMap();
                    List<string> mapInfo = __instance.GetStageModel().ClassInfo.mapInfo;
                    if (mapInfo != null && mapInfo.Count > 0)
                    {
                        try
                        {
                            foreach (string text in mapInfo)
                            {
                                if (string.IsNullOrEmpty(text))
                                {
                                    continue;
                                }
                                string[] array = text.Split(new char[]
                                {
                                    '_'
                                });
                                if (array[0].ToLower() == "custom")
                                {
                                    string mapName = text.Substring("custom_".Length).Trim();
                                    string resourcePath = Singleton<ModContentManager>.Instance.GetModPath(__instance.GetStageModel().ClassInfo.workshopID) + "/CustomMap_" + mapName;
                                    Debug.LogError("resourcePath:" + resourcePath);
                                    if (CustomMapManager.TryGetValue(mapName, out Type mapmanager))
                                    {
                                        Debug.LogError("Find MapManager:" + mapName);
                                        if (mapmanager == null)
                                        {
                                            return true;
                                        }
                                        GameObject gameObject = Util.LoadPrefab("LibraryMaps/KETHER_Map", SingletonBehavior<BattleSceneRoot>.Instance.transform);
                                        GameObject borderFrame = gameObject.GetComponent<MapManager>().borderFrame;
                                        GameObject backgroundRoot = gameObject.GetComponent<MapManager>().backgroundRoot;
                                        UnityEngine.Object.Destroy(gameObject.GetComponent<MapManager>());
                                        gameObject.name = "InvitationMap_" + text;
                                        MapManager mapManager = (MapManager)gameObject.AddComponent(mapmanager);
                                        mapManager.borderFrame = borderFrame;
                                        mapManager.backgroundRoot = backgroundRoot;
                                        if (mapManager is CustomMapManager)
                                        {
                                            (mapManager as CustomMapManager).CustomInit();
                                        }
                                        SingletonBehavior<BattleSceneRoot>.Instance.InitInvitationMap(mapManager);
                                    }
                                    else if (Directory.Exists(resourcePath))
                                    {
                                        GameObject gameObject = Util.LoadPrefab("LibraryMaps/KETHER_Map", SingletonBehavior<BattleSceneRoot>.Instance.transform);
                                        GameObject borderFrame = gameObject.GetComponent<MapManager>().borderFrame;
                                        GameObject backgroundRoot = gameObject.GetComponent<MapManager>().backgroundRoot;
                                        UnityEngine.Object.Destroy(gameObject.GetComponent<MapManager>());
                                        gameObject.name = "InvitationMap_" + text;
                                        SimpleMapManager simpleMapManager = (SimpleMapManager)gameObject.AddComponent(typeof(SimpleMapManager));
                                        simpleMapManager.borderFrame = borderFrame;
                                        simpleMapManager.backgroundRoot = backgroundRoot;
                                        simpleMapManager.SimpleInit(resourcePath, mapName);
                                        simpleMapManager.CustomInit();
                                        SingletonBehavior<BattleSceneRoot>.Instance.InitInvitationMap(simpleMapManager);
                                    }
                                }
                                else
                                {
                                    GameObject gameObject2 = Util.LoadPrefab("InvitationMaps/InvitationMap_" + text, SingletonBehavior<BattleSceneRoot>.Instance.transform);
                                    gameObject2.name = "InvitationMap_" + text;
                                    SingletonBehavior<BattleSceneRoot>.Instance.InitInvitationMap(gameObject2.GetComponent<MapManager>());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            File.WriteAllText(Application.dataPath + "/Mods/stageerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
                            SingletonBehavior<BattleSceneRoot>.Instance.InitFloorMap(__instance.CurrentFloor);
                        }
                    }
                }
                SingletonBehavior<BattleSceneRoot>.Instance.InitFloorMap(__instance.CurrentFloor);
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/InitializeMaperror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //EgoMap
        [HarmonyPatch(typeof(BattleSceneRoot), "ChangeToEgoMap")]
        [HarmonyPrefix]
        private static bool BattleSceneRoot_ChangeToEgoMap_Pre(string mapName)
        {
            try
            {
                MapChangeFilter mapChangeFilter = SingletonBehavior<BattleSceneRoot>.Instance._mapChangeFilter;
                GameObject gameObject = null;
                string[] array = mapName.Split(new char[]
                {
            '_'
                });
                if (array[0].ToLower() == "custom" && CustomMapManager.TryGetValue(mapName.Substring("custom_".Length).Trim() + "MapManager", out Type mapmanager))
                {
                    if (mapmanager == null)
                    {
                        return true;
                    }
                    gameObject = Util.LoadPrefab("LibraryMaps/KETHER_Map", SingletonBehavior<BattleSceneRoot>.Instance.transform);
                    GameObject borderFrame = gameObject.GetComponent<MapManager>().borderFrame;
                    GameObject backgroundRoot = gameObject.GetComponent<MapManager>().backgroundRoot;
                    try
                    {
                        UnityEngine.Object.Destroy(gameObject.GetComponent<MapManager>());
                    }
                    catch { }
                    gameObject.name = "EGO_CardMap_" + mapName;
                    MapManager mapManager = (MapManager)gameObject.AddComponent(mapmanager);
                    mapManager.borderFrame = borderFrame;
                    mapManager.backgroundRoot = backgroundRoot;
                    if (mapManager is CustomMapManager)
                    {
                        (mapManager as CustomMapManager).CustomInit();
                    }
                    if (gameObject != null)
                    {
                        mapChangeFilter.StartMapChangingEffect(Direction.RIGHT, true);
                        MapManager component = gameObject.GetComponent<MapManager>();
                        gameObject.name = "CreatureMap_" + mapName;
                        component.isBossPhase = false;
                        component.isEgo = true;
                        if (SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject.isCreature)
                        {
                            UnityEngine.Object.Destroy(SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject.gameObject);
                        }
                        else
                        {
                            SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject.EnableMap(false);
                        }
                        if (component != null)
                        {
                            if (SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject != null && SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject.isCreature)
                            {
                                UnityEngine.Object.Destroy(SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject.gameObject);
                                SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject = null;
                            }
                            SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject = component;
                            SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject.ActiveMap(true);
                            SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject.InitializeMap();
                            return false;
                        }
                        else
                        {
                            Debug.LogError("Ego map not found");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/ChangeToEgoMaperror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //CanChangeMap
        [HarmonyPatch(typeof(StageController), "CanChangeMap")]
        [HarmonyPostfix]
        private static void StageController_CanChangeMap_Post(ref bool __result)
        {
            if (BattleSceneRoot.Instance.currentMapObject is CustomMapManager customMap)
            {
                __result = customMap.IsMapChangable();
            }
        }
        //Not be used now
        public static GameObject FindBaseMap(string name)
        {
            GameObject gameObject = null;
            if (name == "malkuth")
            {
                gameObject = Util.LoadPrefab("LibraryMaps/MALKUTH_Map", SingletonBehavior<BattleSceneRoot>.Instance.transform);
            }
            if (name == "yesod")
            {
                gameObject = Util.LoadPrefab("LibraryMaps/YESOD_Map", SingletonBehavior<BattleSceneRoot>.Instance.transform);
            }
            if (name == "hod")
            {
                gameObject = Util.LoadPrefab("LibraryMaps/HOD_Map", SingletonBehavior<BattleSceneRoot>.Instance.transform);
            }
            if (name == "netzach")
            {
                gameObject = Util.LoadPrefab("LibraryMaps/NETZACH_Map", SingletonBehavior<BattleSceneRoot>.Instance.transform);
            }
            if (name == "tiphereth")
            {
                gameObject = Util.LoadPrefab("LibraryMaps/TIPHERETH_Map", SingletonBehavior<BattleSceneRoot>.Instance.transform);
            }
            if (name == "gebura")
            {
                gameObject = Util.LoadPrefab("LibraryMaps/GEBURAH_Map", SingletonBehavior<BattleSceneRoot>.Instance.transform);
            }
            if (name == "chesed")
            {
                gameObject = Util.LoadPrefab("LibraryMaps/CHESED_Map", SingletonBehavior<BattleSceneRoot>.Instance.transform);
            }
            if (name == "keter")
            {
                gameObject = Util.LoadPrefab("LibraryMaps/KETHER_Map", SingletonBehavior<BattleSceneRoot>.Instance.transform);
            }
            if (name == "hokma")
            {
                gameObject = Util.LoadPrefab("LibraryMaps/HOKMA_Map", SingletonBehavior<BattleSceneRoot>.Instance.transform);
            }
            if (name == "binah")
            {
                gameObject = Util.LoadPrefab("LibraryMaps/BINAH_Map", SingletonBehavior<BattleSceneRoot>.Instance.transform);
            }
            if (gameObject == null)
            {
                try
                {
                    gameObject = Util.LoadPrefab("InvitationMaps/InvitationMap_" + name, SingletonBehavior<BattleSceneRoot>.Instance.transform);
                }
                catch (Exception)
                {
                    gameObject = null;
                }
            }
            if (gameObject == null)
            {
                try
                {
                    gameObject = Util.LoadPrefab("CreatureMaps/CreatureMap_" + name, SingletonBehavior<BattleSceneRoot>.Instance.transform);
                }
                catch (Exception)
                {
                    gameObject = null;
                }
            }
            GameObject result;
            if (gameObject != null)
            {
                result = gameObject;
            }
            else
            {
                result = gameObject;
            }
            return result;
        }

        //Others
        //AutoMod1
        [HarmonyPatch(typeof(UIStoryProgressPanel), "SelectedSlot")]
        [HarmonyPrefix]
        private static void UIStoryProgressPanel_SelectedSlot_Pre(UIStoryProgressIconSlot slot, bool isSelected)
        {
            try
            {
                if (slot == null || slot._storyData == null)
                {
                    return;
                }
                if (slot._storyData[0].workshopID != "")
                {
                    IsModStorySelected = true;
                }
                else
                {
                    IsModStorySelected = false;
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/CheckSelectedSloterror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //AutoMod2
        [HarmonyPatch(typeof(UIInvitationRightMainPanel), "SetCustomInvToggle")]
        [HarmonyPostfix]
        private static void UIInvitationRightMainPanel_SetCustomInvToggle_Post(ref Toggle ____workshopInvitationToggle)
        {
            if (IsModStorySelected)
            {
                ____workshopInvitationToggle.isOn = true;
            }
        }
        //ErrorNull for delete card from deck
        [HarmonyPatch(typeof(ItemXmlDataList), "GetCardItem", new Type[] { typeof(LorId), typeof(bool) })]
        [HarmonyPrefix]
        private static bool ItemXmlDataList_GetCardItem_Pre(ref DiceCardXmlInfo __result, LorId id, Dictionary<LorId, DiceCardXmlInfo> ____cardInfoTable, bool errNull = false)
        {
            try
            {
                if (____cardInfoTable.TryGetValue(id, out DiceCardXmlInfo result))
                {
                    __result = result;
                    return false;
                }
                if (!errNull)
                {
                    if (errNullCard == null)
                    {
                        errNullCard = new DiceCardXmlInfo(id)
                        {
                            Artwork = "Basic1",
                            Rarity = Rarity.Common,
                            DiceBehaviourList = new List<DiceBehaviour>
                            {
                                new DiceBehaviour
                                {
                                 Min = 1,
                                 Dice = 4,
                                 Type = BehaviourType.Def,
                                 Detail = BehaviourDetail.Evasion
                                }
                            },
                            Chapter = 1,
                            Priority = 0,
                            isError = true
                        };
                    }
                    __result = errNullCard;
                    return false;
                }
                __result = null;
                return false;
            }
            catch (Exception ex)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                File.WriteAllText(Application.dataPath + "/Mods/GetCardItemerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //0book to "∞"
        [HarmonyPatch(typeof(UIInvitationDropBookSlot), "SetData_DropBook")]
        [HarmonyPostfix]
        private static void UIInvitationDropBookSlot_SetData_DropBook_Post(LorId bookId, TextMeshProUGUI ___txt_bookNum)
        {
            try
            {
                if (Singleton<DropBookInventoryModel>.Instance.GetBookCount(bookId) == 0)
                {
                    ___txt_bookNum.text = "∞";
                }
            }
            catch { }
        }
        //ChangeLanguage
        [HarmonyPatch(typeof(TextDataModel), "InitTextData")]
        [HarmonyPostfix]
        private static void TextDataModel_InitTextData_Post(string currentLanguage)
        {
            try
            {
                LocalizedTextLoader_New.ExportOriginalFiles();
                LocalizedTextLoader_New.LoadModFiles(Singleton<ModContentManager>.Instance._loadedContents);
            }
            catch (Exception ex)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                File.WriteAllText(Application.dataPath + "/Mods/InitTextDataerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //LoadStory
        [HarmonyPatch(typeof(StorySerializer), "LoadStory", new Type[] { typeof(bool) })]
        [HarmonyPostfix]
        private static void StorySerializer_LoadStory_Post()
        {
            try
            {
                StorySerializer_new.ExportStory();
            }
            catch (Exception ex)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                File.WriteAllText(Application.dataPath + "/Mods/LoadStoryerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //DiceAttackEffect
        [HarmonyPatch(typeof(DiceEffectManager), "CreateBehaviourEffect")]
        [HarmonyPrefix]
        private static bool DiceEffectManager_CreateBehaviourEffect_Pre(DiceEffectManager __instance, ref DiceAttackEffect __result, string resource, float scaleFactor, BattleUnitView self, BattleUnitView target, float time = 1f)
        {
            try
            {
                if (resource == null || string.IsNullOrEmpty(resource))
                {
                    __result = null;
                    return false;
                }
                else
                {
                    if (CustomEffects.ContainsKey(resource))
                    {
                        Type componentType = CustomEffects[resource];
                        DiceAttackEffect diceAttackEffect3 = new GameObject(resource).AddComponent(componentType) as DiceAttackEffect;
                        diceAttackEffect3.Initialize(self, target, time);
                        diceAttackEffect3.SetScale(scaleFactor);
                        __result = diceAttackEffect3;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/CreateBehaviourEffecterror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //More Waves UI
        [HarmonyPatch(typeof(UIBattleSettingWaveList), "SetData")]
        [HarmonyPrefix]
        private static bool UIBattleSettingWaveList_SetData_Pre(UIBattleSettingWaveList __instance, StageModel stage)
        {
            try
            {
                if (__instance.gameObject.GetComponent<ScrollRect>() == null)
                {
                    if (ArtWorks == null)
                    {
                        GetArtWorks();
                    }
                    Image image = __instance.gameObject.AddComponent<Image>();
                    image.sprite = ArtWorks["UIBattleSettingWaveList_Mask"];
                    image.color = new Color(0f, 0f, 0f, 0f);
                    (__instance.transform as RectTransform).sizeDelta = new Vector2(400f, stage.waveList.Count * 250f / 3f);
                    ScrollRect scrollRect = __instance.gameObject.AddComponent<ScrollRect>();
                    scrollRect.scrollSensitivity = 15f;
                    scrollRect.content = (__instance.transform as RectTransform);
                    scrollRect.horizontal = false;
                    scrollRect.movementType = ScrollRect.MovementType.Elastic;
                    scrollRect.elasticity = 0.1f;
                }
                if (stage.waveList.Count > __instance.waveSlots.Count)
                {
                    int num = stage.waveList.Count - __instance.waveSlots.Count;
                    for (int i = 0; i < num; i++)
                    {
                        UIBattleSettingWaveSlot uibattleSettingWaveSlot = UnityEngine.Object.Instantiate(__instance.waveSlots[0], __instance.waveSlots[0].transform.parent);
                        InitUIBattleSettingWaveSlot(uibattleSettingWaveSlot, __instance);
                        List<UIBattleSettingWaveSlot> list = new List<UIBattleSettingWaveSlot>
                        {
                            uibattleSettingWaveSlot
                        };
                        list.AddRange(__instance.waveSlots);
                        __instance.waveSlots = list;
                    }
                }
                if (stage.waveList.Count < __instance.waveSlots.Count)
                {
                    int num2 = __instance.waveSlots.Count - stage.waveList.Count;
                    int num3 = 0;
                    while (num3 < num2 && __instance.waveSlots.Count != 5)
                    {
                        UIBattleSettingWaveSlot uibattleSettingWaveSlot2 = __instance.waveSlots[__instance.waveSlots.Count - 1];
                        __instance.waveSlots.Remove(uibattleSettingWaveSlot2);
                        UnityEngine.Object.DestroyImmediate(uibattleSettingWaveSlot2);
                        num3++;
                    }
                }
                InitUIBattleSettingWaveSlots(__instance.waveSlots, __instance);
                foreach (UIBattleSettingWaveSlot uibattleSettingWaveSlot3 in __instance.waveSlots)
                {
                    uibattleSettingWaveSlot3.gameObject.SetActive(false);
                }
                for (int j = 0; j < stage.waveList.Count; j++)
                {
                    UIBattleSettingWaveSlot uibattleSettingWaveSlot4 = __instance.waveSlots[j];
                    uibattleSettingWaveSlot4.SetData(stage.waveList[j]);
                    uibattleSettingWaveSlot4.gameObject.SetActive(true);
                    if (stage.waveList[j].IsUnavailable())
                    {
                        uibattleSettingWaveSlot4.SetDefeat();
                    }
                    if (j == stage.waveList.Count - 1)
                    {
                        uibattleSettingWaveSlot4.ActivateArrow(false);
                    }
                }
                int num4 = Singleton<StageController>.Instance.CurrentWave - 1;
                if (num4 < 0 || __instance.waveSlots.Count <= num4)
                {
                    Debug.LogError("Index Error");
                }
                else
                {
                    __instance.waveSlots[num4].SetHighlighted();
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/UIBSWLerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
           (__instance.transform as RectTransform).sizeDelta = new Vector2(400f, stage.waveList.Count * 250f / 3f);
            return false;
        }
        private static void InitUIBattleSettingWaveSlot(UIBattleSettingWaveSlot slot, UIBattleSettingWaveList list)
        {
            FieldInfo field = slot.GetType().GetField("panel", AccessTools.all);
            FieldInfo field2 = slot.GetType().GetField("rect", AccessTools.all);
            FieldInfo field3 = slot.GetType().GetField("img_circle", AccessTools.all);
            FieldInfo field4 = slot.GetType().GetField("img_circleglow", AccessTools.all);
            FieldInfo field5 = slot.GetType().GetField("img_Icon", AccessTools.all);
            FieldInfo field6 = slot.GetType().GetField("img_IconGlow", AccessTools.all);
            FieldInfo field7 = slot.GetType().GetField("hsv_Icon", AccessTools.all);
            FieldInfo field8 = slot.GetType().GetField("hsv_IconGlow", AccessTools.all);
            FieldInfo field9 = slot.GetType().GetField("hsv_Circle", AccessTools.all);
            FieldInfo field10 = slot.GetType().GetField("hsv_CircleGlow", AccessTools.all);
            FieldInfo field11 = slot.GetType().GetField("txt_Alarm", AccessTools.all);
            FieldInfo field12 = slot.GetType().GetField("materialsetter_txtAlarm", AccessTools.all);
            FieldInfo field13 = slot.GetType().GetField("arrow", AccessTools.all);
            FieldInfo field14 = slot.GetType().GetField("defeatColor", AccessTools.all);
            FieldInfo field15 = slot.GetType().GetField("anim", AccessTools.all);
            FieldInfo field16 = slot.GetType().GetField("cg", AccessTools.all);
            field.SetValue(slot, list);
            RectTransform value = slot.transform as RectTransform;
            field2.SetValue(slot, value);
            field3.SetValue(slot, slot.gameObject.transform.GetChild(1).GetChild(1).gameObject.GetComponent<Image>());
            field4.SetValue(slot, slot.gameObject.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>());
            field5.SetValue(slot, slot.gameObject.transform.GetChild(1).GetChild(3).gameObject.GetComponent<Image>());
            field6.SetValue(slot, slot.gameObject.transform.GetChild(1).GetChild(2).gameObject.GetComponent<Image>());
            field7.SetValue(slot, slot.gameObject.transform.GetChild(1).GetChild(3).gameObject.GetComponent<_2dxFX_HSV>());
            field8.SetValue(slot, slot.gameObject.transform.GetChild(1).GetChild(2).gameObject.GetComponent<_2dxFX_HSV>());
            field9.SetValue(slot, slot.gameObject.transform.GetChild(1).GetChild(1).gameObject.GetComponent<_2dxFX_HSV>());
            field10.SetValue(slot, slot.gameObject.transform.GetChild(1).GetChild(0).gameObject.GetComponent<_2dxFX_HSV>());
            field11.SetValue(slot, slot.gameObject.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>());
            field12.SetValue(slot, slot.gameObject.transform.GetChild(2).gameObject.GetComponent<TextMeshProMaterialSetter>());
            field13.SetValue(slot, slot.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>());
            Color color = new Color(0.454902f, 0.1098039f, 0f, 1f);
            field14.SetValue(slot, color);
            field15.SetValue(slot, slot.gameObject.GetComponent<Animator>());
            field16.SetValue(slot, slot.gameObject.GetComponent<CanvasGroup>());
            slot.transform.localPosition = new Vector2(120f, 0f);
            slot.gameObject.SetActive(false);
        }
        private static void InitUIBattleSettingWaveSlots(List<UIBattleSettingWaveSlot> slots, UIBattleSettingWaveList __instance)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].gameObject.transform.localScale = new Vector3(1f, 1f);
            }
        }
        //over 999 dicevalue
        [HarmonyPatch(typeof(BattleSimpleActionUI_Dice), "SetDiceValue")]
        [HarmonyPrefix]
        private static bool BattleSimpleActionUI_Dice_SetDiceValue_Pre(BattleSimpleActionUI_Dice __instance, bool enable, int diceValue)
        {
            try
            {
                int num = 0;
                int num2 = diceValue;
                List<GameObject> list = new List<GameObject>();
                List<GameObject> list2 = new List<GameObject>();
                for (int i = 0; i < __instance.layout_numbers.childCount; i++)
                {
                    list.Add(__instance.layout_numbers.GetChild(i).gameObject);
                    list2.Add(__instance.layout_numberbgs.GetChild(i).gameObject);
                    __instance.layout_numbers.GetChild(i).gameObject.SetActive(false);
                    __instance.layout_numberbgs.GetChild(i).gameObject.SetActive(false);
                }
                bool flag;
                do
                {
                    num++;
                    num2 /= 10;
                    flag = (num2 == 0);
                }
                while (!flag);
                int num3 = num - __instance.layout_numbers.childCount;
                for (int j = 0; j < num3; j++)
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate(__instance.layout_numbers.GetChild(0).gameObject, __instance.layout_numbers);
                    list.Add(gameObject);
                    gameObject.gameObject.SetActive(false);
                    GameObject gameObject2 = UnityEngine.Object.Instantiate(__instance.layout_numberbgs.GetChild(0).gameObject, __instance.layout_numberbgs);
                    list2.Add(gameObject2);
                    gameObject2.gameObject.SetActive(false);
                }
                if (enable)
                {
                    List<Sprite> battleDice_NumberAutoSlice = UISpriteDataManager.instance.BattleDice_NumberAutoSlice;
                    List<Sprite> battleDice_numberAutoSliceBg = UISpriteDataManager.instance.BattleDice_numberAutoSliceBg;
                    for (int k = 0; k < num; k++)
                    {
                        int index = diceValue % 10;
                        Sprite sprite = battleDice_NumberAutoSlice[index];
                        Image component = list[list.Count - k - 1].GetComponent<Image>();
                        component.sprite = sprite;
                        component.SetNativeSize();
                        component.gameObject.SetActive(true);
                        Sprite sprite2 = battleDice_numberAutoSliceBg[index];
                        Image component2 = list2[list.Count - k - 1].GetComponent<Image>();
                        component2.sprite = sprite2;
                        component2.SetNativeSize();
                        component2.gameObject.SetActive(true);
                        component2.rectTransform.anchoredPosition = component.rectTransform.anchoredPosition;
                        diceValue /= 10;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/SetDiceValueerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //VersionViewer
        [HarmonyPatch(typeof(VersionViewer), "Start")]
        [HarmonyPrefix]
        private static bool VersionViewer_Start_Pre(VersionViewer __instance)
        {
            __instance.GetComponent<Text>().text = GlobalGameManager.Instance.ver;
            __instance.GetComponent<Text>().fontSize = 30;
            __instance.gameObject.transform.localPosition = new Vector3(-830f, -460f);
            return false;
        }
        /*
        private static void DiceCardXmlInfo_Copy_Post(DiceCardXmlInfo __instance, DiceCardXmlInfo __result)
        {
            __result.Keywords = __instance.Keywords;
        }*/
        /*
        //Mod_Update
        //Using For Reload
        [HarmonyPatch(typeof(DebugConsoleScript), "Update")]
        [HarmonyPrefix]
        private static void Mod_Update()
        {
            try
            {
                if (!IsEditing && entryScene != null && Input.GetKeyDown(KeyCode.R))
                {
                    File.WriteAllText(Application.dataPath + "/Mods/PressSuccess.log", "success");
                    entryScene.GetType().GetMethod("OnCompleteInitializePlatform_xboxlive", AccessTools.all).Invoke(entryScene,
                    new object[]
                    {
                    true
                    });
                }
                IsEditing = true;
            }
            catch (Exception ex)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                File.WriteAllText(Application.dataPath + "/Mods/Updateerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //Using For Reload
        [HarmonyPatch(typeof(ModContentManager), "SetActiveContents")]
        [HarmonyPrefix]
        private static void ModContentManager_SetActiveContents_Pre(List<ModContent> ____loadedContents)
        {
            ____loadedContents.Clear();
        }
        //Using For Reload
        [HarmonyPatch(typeof(UIModPopup), "Close")]
        [HarmonyPrefix]
        private static void UIModPopup_Close_Post()
        {
            try
            {
                if (IsEditing)
                {
                    ReloadModFiles();
                    LoadAssemblyFiles();
                    LoadModFiles();
                }
            }
            catch (Exception ex)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                File.WriteAllText(Application.dataPath + "/Mods/ModSettingerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            IsEditing = false;
        }*/

        //ModItemSort
        //BookSort
        [HarmonyPatch(typeof(UIInvitationDropBookList), "ApplyFilterAll")]
        [HarmonyPostfix]
        private static void UIInvitationDropBookList_ApplyFilterAll_Post(UIInvitationDropBookList __instance, List<LorId> ____currentBookIdList)
        {
            try
            {
                if (ModPid == null)
                {
                    ModPid = new List<string>();
                    foreach (ModContentInfo modContentInfo in Singleton<ModContentManager>.Instance.GetAllMods())
                    {
                        if (!modContentInfo.invInfo.workshopInfo.uniqueId.ToLower().EndsWith("@origin") && !ModPid.Contains(modContentInfo.invInfo.workshopInfo.uniqueId) && modContentInfo.activated)
                        {
                            ModPid.Add(modContentInfo.invInfo.workshopInfo.uniqueId);
                        }
                    }
                    ModPid.Sort();
                }
                List<LorId> list = new List<LorId>();
                list.AddRange(____currentBookIdList);
                ____currentBookIdList.Clear();
                foreach (string id in ModPid)
                {
                    ____currentBookIdList.AddRange(list.FindAll((LorId x) => x.packageId == id));
                }
                ____currentBookIdList.AddRange(list.FindAll((LorId x) => x.packageId == ""));
                __instance.SelectablePanel.ChildSelectable = __instance.BookSlotList[0].selectable;
                __instance.UpdateBookListPage(false);
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/ModBookSort_Invi.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //BookSort
        [HarmonyPatch(typeof(UIInvenFeedBookList), "ApplyFilterAll")]
        [HarmonyPostfix]
        private static void UIInvenFeedBookList_ApplyFilterAll_Post(UIInvenFeedBookList __instance, List<LorId> ____currentBookIdList)
        {
            try
            {
                if (ModPid == null)
                {
                    ModPid = new List<string>();
                    foreach (ModContentInfo modContentInfo in Singleton<ModContentManager>.Instance.GetAllMods())
                    {
                        if (!modContentInfo.invInfo.workshopInfo.uniqueId.ToLower().EndsWith("@origin") && !ModPid.Contains(modContentInfo.invInfo.workshopInfo.uniqueId))
                        {
                            ModPid.Add(modContentInfo.invInfo.workshopInfo.uniqueId);
                        }
                    }
                    ModPid.Sort();
                }
                List<LorId> list = new List<LorId>();
                list.AddRange(____currentBookIdList);
                ____currentBookIdList.Clear();
                foreach (string id in ModPid)
                {
                    ____currentBookIdList.AddRange(list.FindAll((LorId x) => x.packageId == id));
                }
                ____currentBookIdList.AddRange(list.FindAll((LorId x) => x.packageId == ""));
                __instance.UpdateBookListPage(false);
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/ModBookSort_Feed.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //CardSort
        [HarmonyPatch(typeof(UIInvenCardListScroll), "ApplyFilterAll")]
        [HarmonyPrefix]
        private static bool UIInvenCardListScroll_ApplyFilterAll_Pre(UIInvenCardListScroll __instance, List<DiceCardItemModel> ____currentCardListForFilter, List<DiceCardItemModel> ____originCardList, UnitDataModel ____unitdata, int ___column, int ___row, float ___slotWidth, float ___slotHeight, UICustomScrollBar ___scrollBar, UICustomSelectablePanel ___selectablePanel, List<UIOriginCardSlot> ___slotList)
        {
            try
            {
                ____currentCardListForFilter.Clear();
                List<DiceCardItemModel> cardsByDetailFilterUI = __instance.GetCardsByDetailFilterUI(__instance.GetCardBySearchFilterUI(__instance.GetCardsByCostFilterUI(__instance.GetCardsByGradeFilterUI(____originCardList))));
                cardsByDetailFilterUI.Sort(new Comparison<DiceCardItemModel>(ModCardItemSort));
                if (____unitdata != null)
                {
                    Predicate<DiceCardItemModel> cond1 = (DiceCardItemModel x) => true;
                    switch (____unitdata.bookItem.ClassInfo.RangeType)
                    {
                        case EquipRangeType.Melee:
                            cond1 = ((DiceCardItemModel x) => x.GetSpec().Ranged != CardRange.Far);
                            break;
                        case EquipRangeType.Range:
                            cond1 = ((DiceCardItemModel x) => x.GetSpec().Ranged > CardRange.Near);
                            break;
                        case EquipRangeType.Hybrid:
                            cond1 = ((DiceCardItemModel x) => true);
                            break;
                    }
                    List<DiceCardXmlInfo> onlyCards = ____unitdata.bookItem.GetOnlyCards();
                    Predicate<DiceCardItemModel> cond2 = ((DiceCardItemModel x) => onlyCards.Exists((DiceCardXmlInfo y) => y.id == x.GetID()));
                    foreach (DiceCardItemModel item in cardsByDetailFilterUI.FindAll((DiceCardItemModel x) => x.ClassInfo.optionList.Contains(CardOption.OnlyPage) && !cond2(x)))
                    {
                        cardsByDetailFilterUI.Remove(item);
                    }
                    ____currentCardListForFilter.AddRange(cardsByDetailFilterUI.FindAll((DiceCardItemModel x) => (!x.ClassInfo.optionList.Contains(CardOption.OnlyPage)) ? cond1(x) : cond2(x)));
                    ____currentCardListForFilter.AddRange(cardsByDetailFilterUI.FindAll((DiceCardItemModel x) => (x.ClassInfo.optionList.Contains(CardOption.OnlyPage) ? (cond2(x) ? 1 : 0) : (cond1(x) ? 1 : 0)) == 0));
                }
                else
                {
                    ____currentCardListForFilter.AddRange(cardsByDetailFilterUI);
                }
                int num = __instance.GetMaxRow();
                ___scrollBar.SetScrollRectSize(___column * ___slotWidth, (num + (float)___row - 1f) * ___slotHeight);
                ___scrollBar.SetWindowPosition(0f, 0f);
                ___selectablePanel.ChildSelectable = ___slotList[0].selectable;
                __instance.SetCardsData(__instance.GetCurrentPageList());
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/ModCardSort.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        private static int ModCardItemSort(DiceCardItemModel a, DiceCardItemModel b)
        {
            int num = b.ClassInfo.optionList.Contains(CardOption.OnlyPage) ? 1 : 0;
            int num2 = a.ClassInfo.optionList.Contains(CardOption.OnlyPage) ? 1 : 0;
            int num3 = (b.ClassInfo.isError ? -1 : num) - (a.ClassInfo.isError ? -1 : num2);
            int result;
            if (num3 != 0)
            {
                result = num3;
            }
            else
            {
                num3 = a.GetSpec().Cost - b.GetSpec().Cost;
                if (num3 != 0)
                {
                    result = num3;
                }
                else
                {
                    num3 = a.ClassInfo.workshopID.CompareTo(b.ClassInfo.workshopID);
                    result = ((num3 != 0) ? num3 : (a.GetID().id - b.GetID().id));
                }
            }
            return result;
        }

        //ModSetting
        //SaveModSetting
        [HarmonyPatch(typeof(SaveManager), "SavePlayData")]
        [HarmonyPrefix]
        private static void SaveManager_SavePlayData_Pre()
        {
            try
            {
                ModSaveTool.SaveModSaveData();
                if (File.Exists(SaveManager.savePath + "/Player.log"))
                {
                    if (File.Exists(Application.dataPath + "/Mods/Player.log"))
                    {
                        File.Delete(Application.dataPath + "/Mods/Player.log");
                    }
                    File.Copy(SaveManager.savePath + "/Player.log", Application.dataPath + "/Mods/Player.log");
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/SaveFailed.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //RemoveUnknownSaves
        [HarmonyPatch(typeof(GameOpeningController), "StopOpening")]
        [HarmonyPostfix]
        private static void GameOpeningController_StopOpening_Post()
        {
            try
            {
                LoadCoreThumbs();
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/LoadFromModSaveDataerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        //CustomGift
        //CreateGiftData
        [HarmonyPatch(typeof(CharacterAppearance), "CreateGiftData")]
        [HarmonyPrefix]
        private static bool CharacterAppearance_CreateGiftData_Pre(CharacterAppearance __instance, ref GiftAppearance __result, GiftModel gift, string resPath)
        {
            try
            {
                if (__instance.CustomAppearance == null)
                {
                    __result = null;
                    return false;
                }
                else
                {
                    string[] array = resPath.Split(new char[]
                    {
                        '/'
                    });
                    string[] array2 = array[array.Length - 1].Split(new char[]
                    {
                         '_'
                    });
                    if (array2[1].ToLower() != "custom")
                    {
                        return true;
                    }
                    else
                    {
                        bool flag = false;
                        GiftAppearance giftAppearance = null;
                        Dictionary<GiftPosition, GiftAppearance> dictionary = __instance._giftAppearanceDic;
                        if (dictionary.ContainsKey(gift.ClassInfo.Position))
                        {
                            giftAppearance = dictionary[gift.ClassInfo.Position];
                            if (giftAppearance.ResourceName != resPath)
                            {
                                dictionary.Remove(gift.ClassInfo.Position);
                                UnityEngine.Object.Destroy(giftAppearance.gameObject);
                                flag = true;
                            }
                        }
                        else
                        {
                            flag = true;
                        }
                        if (flag)
                        {
                            giftAppearance = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Gifts/Gifts_NeedRename/Gift_Challenger"), __instance.transform).GetComponent<GiftAppearance>();
                            SpriteRenderer spriteRenderer = giftAppearance._frontSpriteRenderer;
                            SpriteRenderer spriteRenderer2 = giftAppearance._sideSpriteRenderer;
                            SpriteRenderer spriteRenderer3 = giftAppearance._frontBackSpriteRenderer;
                            SpriteRenderer spriteRenderer4 = giftAppearance._sideBackSpriteRenderer;
                            spriteRenderer.gameObject.transform.localScale = new Vector2(1f, 1f);
                            if (CustomGiftAppearance.GiftArtWork.ContainsKey(array2[2] + "_front"))
                            {
                                spriteRenderer.sprite = CustomGiftAppearance.GiftArtWork[array2[2] + "_front"];
                            }
                            else
                            {
                                spriteRenderer.gameObject.SetActive(false);
                                giftAppearance._frontSpriteRenderer = null;
                            }
                            spriteRenderer2.gameObject.transform.localScale = new Vector2(1f, 1f);
                            if (CustomGiftAppearance.GiftArtWork.ContainsKey(array2[2] + "_side"))
                            {
                                spriteRenderer2.sprite = CustomGiftAppearance.GiftArtWork[array2[2] + "_side"];
                            }
                            else
                            {
                                spriteRenderer2.gameObject.SetActive(false);
                                giftAppearance._sideSpriteRenderer = null;
                            }
                            spriteRenderer3.gameObject.transform.localScale = new Vector2(1f, 1f);
                            if (CustomGiftAppearance.GiftArtWork.ContainsKey(array2[2] + "_frontBack"))
                            {
                                spriteRenderer3.sprite = CustomGiftAppearance.GiftArtWork[array2[2] + "_frontBack"];
                            }
                            else
                            {
                                spriteRenderer3.gameObject.SetActive(false);
                                giftAppearance._frontBackSpriteRenderer = null;
                            }
                            spriteRenderer4.gameObject.transform.localScale = new Vector2(1f, 1f);
                            if (CustomGiftAppearance.GiftArtWork.ContainsKey(array2[2] + "_sideBack"))
                            {
                                spriteRenderer4.sprite = CustomGiftAppearance.GiftArtWork[array2[2] + "_sideBack"];
                            }
                            else
                            {
                                spriteRenderer4.gameObject.SetActive(false);
                                giftAppearance._sideBackSpriteRenderer = null;
                            }
                            dictionary.Add(gift.ClassInfo.Position, giftAppearance);
                        }
                        if (giftAppearance != null)
                        {
                            string layer = __instance._layerName;
                            CharacterMotion motion = __instance._currentMotion;
                            giftAppearance.Init(gift, layer);
                            giftAppearance.RefreshAppearance(__instance.CustomAppearance, motion);
                        }
                        __result = giftAppearance;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/CACGDerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //GiftPassive
        [HarmonyPatch(typeof(GiftModel), "CreateScripts")]
        [HarmonyPrefix]
        private static bool GiftModel_CreateScripts_Pre(GiftModel __instance, ref List<PassiveAbilityBase> __result)
        {
            try
            {
                List<PassiveAbilityBase> list = new List<PassiveAbilityBase>();
                foreach (int num in __instance.ClassInfo.ScriptList)
                {
                    PassiveAbilityBase passiveAbilityBase = FindGiftPassiveAbility(num.ToString().Trim());
                    if (passiveAbilityBase != null)
                    {
                        passiveAbilityBase.name = __instance.GetName();
                        passiveAbilityBase.desc = __instance.GiftDesc;
                        list.Add(passiveAbilityBase);
                    }
                }
                __result = list;
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/GMCSerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        public static PassiveAbilityBase FindGiftPassiveAbility(string name)
        {
            if (!string.IsNullOrEmpty(name) && CustomGiftPassive.TryGetValue(name, out Type type))
            {
                return Activator.CreateInstance(type) as PassiveAbilityBase;
            }
            foreach (Type type2 in Assembly.LoadFile(Application.dataPath + "/Managed/Assembly-CSharp.dll").GetTypes())
            {
                if (type2.Name == "PassiveAbilityBase_" + name)
                {
                    return Activator.CreateInstance(type2) as PassiveAbilityBase;
                }
            }
            return null;
        }
        //GiftDataSlot
        [HarmonyPatch(typeof(UIGiftDataSlot), "SetData")]
        [HarmonyPrefix]
        private static bool UIGiftDataSlot_SetData_Pre(UIGiftDataSlot __instance, GiftModel data, ref Image ___img_giftImage, ref Image ___img_xmark, ref Image ___img_giftMask, ref TextMeshProUGUI ___txt_giftName, ref TextMeshProUGUI ___txt_giftNameDetail, ref TextMeshProUGUI ___txt_giftPartName)
        {
            try
            {
                if (data == null)
                {
                    __instance.gameObject.SetActive(false);
                    return false;
                }
                else
                {
                    ___img_giftImage.enabled = true;
                    ___img_xmark.enabled = false;
                    ___img_giftMask.enabled = true;
                    __instance.OpenInit();
                    string[] array = data.GetResourcePath().Split(new char[]
                    {
                         '/'
                    });
                    string[] array2 = array[array.Length - 1].Split(new char[]
                    {
                         '_'
                    });
                    GiftAppearance giftAppearance;
                    if (array2[1].ToLower() != "custom")
                    {
                        return true;
                    }
                    giftAppearance = CustomGiftAppearance.CreateCustomGift(array2);
                    giftAppearance.gameObject.SetActive(false);
                    if (giftAppearance != null)
                    {
                        if (giftAppearance.GetType() == typeof(GiftAppearance_Aura))
                        {
                            ___img_giftImage.enabled = true;
                            ___img_giftImage.sprite = UISpriteDataManager.instance.GiftAuraIcon;
                            ___img_giftImage.rectTransform.localScale = new Vector2(0.8f, 0.8f);
                        }
                        else
                        {
                            ___img_giftImage.sprite = giftAppearance.GetGiftPreview();
                            ___img_giftImage.rectTransform.localScale = Vector2.one;
                        }
                        if (___img_giftImage.sprite == null)
                        {
                            ___img_giftImage.enabled = false;
                        }
                    }
                    else
                    {
                        ___img_giftImage.enabled = false;
                        ___img_giftMask.enabled = false;
                    }
                    ___txt_giftName.text = data.GetName();
                    ___txt_giftNameDetail.text = data.GiftDesc;
                    ___img_giftImage.gameObject.SetActive(true);
                    string id = "";
                    switch (data.ClassInfo.Position)
                    {
                        case GiftPosition.Eye:
                            id = "ui_gift_eye";
                            break;
                        case GiftPosition.Nose:
                            id = "ui_gift_nose";
                            break;
                        case GiftPosition.Cheek:
                            id = "ui_gift_cheek";
                            break;
                        case GiftPosition.Mouth:
                            id = "ui_gift_mouth";
                            break;
                        case GiftPosition.Ear:
                            id = "ui_gift_ear";
                            break;
                        case GiftPosition.HairAccessory:
                            id = "ui_gift_headdress1";
                            break;
                        case GiftPosition.Hood:
                            id = "ui_gift_headdress2";
                            break;
                        case GiftPosition.Mask:
                            id = "ui_gift_headdress3";
                            break;
                        case GiftPosition.Helmet:
                            id = "ui_gift_headdress4";
                            break;
                    }
                    ___txt_giftPartName.text = TextDataModel.GetText(id, Array.Empty<object>());
                    return false;
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/GiftSetDataerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //UIGiftInvenSlot
        [HarmonyPatch(typeof(UIGiftInvenSlot), "SetData")]
        [HarmonyPrefix]
        private static bool UIGiftInvenSlot_SetData_Pre(UIGiftInvenSlot __instance, GiftModel gift, UIGiftInventory inven, ref UIGiftInventory ___panel, ref Image ___img_Gift, ref TextMeshProUGUI ___txt_Part, ref TextMeshProUGUI ___txt_Name, ref TextMeshProUGUI ___txt_desc, ref TextMeshProUGUI ___txt_getcondition, ref GameObject ___conditionTextGameObject)
        {
            try
            {
                __instance.gameObject.SetActive(true);
                __instance.giftData = gift;
                ___panel = inven;
                if (gift == null)
                {
                    return false;
                }
                string id = "";
                switch (gift.ClassInfo.Position)
                {
                    case GiftPosition.Eye:
                        id = "ui_gift_eye";
                        break;
                    case GiftPosition.Nose:
                        id = "ui_gift_nose";
                        break;
                    case GiftPosition.Cheek:
                        id = "ui_gift_cheek";
                        break;
                    case GiftPosition.Mouth:
                        id = "ui_gift_mouth";
                        break;
                    case GiftPosition.Ear:
                        id = "ui_gift_ear";
                        break;
                    case GiftPosition.HairAccessory:
                        id = "ui_gift_headdress1";
                        break;
                    case GiftPosition.Hood:
                        id = "ui_gift_headdress2";
                        break;
                    case GiftPosition.Mask:
                        id = "ui_gift_headdress3";
                        break;
                    case GiftPosition.Helmet:
                        id = "ui_gift_headdress4";
                        break;
                }
                string[] array = gift.GetResourcePath().Split(new char[]
                {
                '/'
                });
                string[] array2 = array[array.Length - 1].Split(new char[]
                {
                '_'
                });
                GiftAppearance giftAppearance;
                if (array2[1].ToLower() != "custom")
                {
                    return true;
                }
                else
                {
                    giftAppearance = CustomGiftAppearance.CreateCustomGift(array2);
                    giftAppearance.gameObject.SetActive(false);
                }
                ___img_Gift.enabled = true;
                if (giftAppearance != null)
                {
                    if (giftAppearance.GetType() == typeof(GiftAppearance_Aura))
                    {
                        ___img_Gift.sprite = UISpriteDataManager.instance.GiftAuraIcon;
                        ___img_Gift.rectTransform.localScale = new Vector2(0.8f, 0.8f);
                    }
                    else
                    {
                        ___img_Gift.sprite = giftAppearance.GetGiftPreview();
                        ___img_Gift.rectTransform.localScale = new Vector2(1f, 1f);
                    }
                }
                if (___img_Gift.sprite == null)
                {
                    ___img_Gift.enabled = false;
                }
                ___txt_Part.text = TextDataModel.GetText(id, Array.Empty<object>());
                ___txt_Name.text = gift.GetName();
                ___txt_desc.text = gift.GiftDesc;
                ___txt_getcondition.text = gift.GiftAcquireCondition;
                ___conditionTextGameObject.SetActive(true);
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/GiftInvSetDataerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //UIGiftPreviewSlot
        [HarmonyPatch(typeof(UIGiftPreviewSlot), "UpdateSlot")]
        [HarmonyPrefix]
        private static bool UIGiftPreviewSlot_UpdateSlot_Pre(UIGiftPreviewSlot __instance, ref GiftModel ___Gift, ref TextMeshProUGUI ___txt_GiftName, ref TextMeshProUGUI ___txt_GiftDesc, ref Image ___img_Gift, ref GameObject ___detailcRect, bool ___isEyeOpen, ref UILibrarianAppearanceInfoPanel ___panel)
        {
            try
            {
                if (___Gift != null)
                {
                    ___txt_GiftName.gameObject.SetActive(true);
                    ___txt_GiftName.text = ___Gift.GetName();
                    ___txt_GiftDesc.text = ___Gift.GiftDesc;
                    ___img_Gift.gameObject.SetActive(true);
                    ___img_Gift.enabled = true;
                    string[] array = ___Gift.GetResourcePath().Split(new char[]
                    {
                        '/'
                    });
                    string[] array2 = array[array.Length - 1].Split(new char[]
                    {
                        '_'
                    });
                    GiftAppearance giftAppearance;
                    if (array2[1].ToLower() != "custom")
                    {
                        return true;
                    }
                    else
                    {
                        giftAppearance = CustomGiftAppearance.CreateCustomGift(array2);
                        giftAppearance.gameObject.SetActive(false);
                    }
                    if (giftAppearance != null)
                    {
                        if (giftAppearance.GetType() == typeof(GiftAppearance_Aura))
                        {
                            ___img_Gift.sprite = UISpriteDataManager.instance.GiftAuraIcon;
                            ___img_Gift.rectTransform.localScale = new Vector2(0.8f, 0.8f);
                        }
                        else
                        {
                            ___img_Gift.sprite = giftAppearance.GetGiftPreview();
                            ___img_Gift.rectTransform.localScale = new Vector2(1f, 1f);
                        }
                    }
                    if (___img_Gift.sprite == null)
                    {
                        ___img_Gift.enabled = false;
                    }
                    ___detailcRect.SetActive(___panel.giftDetailToggle.isOn);
                }
                else
                {
                    ___txt_GiftName.gameObject.SetActive(false);
                    ___img_Gift.gameObject.SetActive(false);
                    ___detailcRect.SetActive(false);
                }
                __instance.SetEyeButton(___isEyeOpen);
                __instance.SetHighlight(false);
                __instance.SetEyeHighlight(false);
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/GiftUpdateSloterror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //LoadGift
        [HarmonyPatch(typeof(GiftInventory), "LoadFromSaveData")]
        [HarmonyPrefix]
        private static bool GiftInventory_LoadFromSaveData_Pre(GiftInventory __instance, SaveData data)
        {
            try
            {
                SaveData data2 = data.GetData("equipList");
                SaveData data3 = data.GetData("unequipList");
                SaveData data4 = data.GetData("offList");
                foreach (SaveData saveData in data2)
                {
                    if (!(Singleton<GiftXmlList>.Instance.GetData(saveData.GetIntSelf()) == null))
                    {
                        GiftModel giftModel = null;
                        try
                        {
                            giftModel = new GiftModel(Singleton<GiftXmlList>.Instance.GetData(saveData.GetIntSelf()));
                        }
                        catch
                        {
                            giftModel = null;
                        }
                        if (giftModel != null)
                        {
                            __instance.AddGift(giftModel);
                            __instance.Equip(giftModel);
                        }
                    }
                }
                foreach (SaveData saveData2 in data3)
                {
                    if (!(Singleton<GiftXmlList>.Instance.GetData(saveData2.GetIntSelf()) == null))
                    {
                        GiftModel giftModel2 = null;
                        try
                        {
                            giftModel2 = new GiftModel(Singleton<GiftXmlList>.Instance.GetData(saveData2.GetIntSelf()));
                        }
                        catch
                        {
                            giftModel2 = null;
                        }
                        if (giftModel2 != null)
                        {
                            __instance.AddGift(giftModel2);
                        }
                    }
                }
                if (data4 != null)
                {
                    foreach (SaveData idData in data4)
                    {
                        if (__instance.GetEquippedList().Find((GiftModel x) => x.ClassInfo.id == idData.GetIntSelf()) != null)
                        {
                            __instance.GetEquippedList().Find((GiftModel x) => x.ClassInfo.id == idData.GetIntSelf()).isShowEquipGift = false;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/LoadGiftSaveerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        //BehaviorAbilityData
        [HarmonyPatch(typeof(BattleCardBehaviourResult), "GetAbilityDataAfterRoll")]
        [HarmonyPostfix]
        private static List<EffectTypoData> BattleCardBehaviourResult_GetAbilityDataAfterRoll_Post(List<EffectTypoData> list, BattleCardBehaviourResult __instance)
        {
            try
            {
                if (CustomEffectTypoData.TryGetValue(__instance, out List<EffectTypoData> addedlist))
                {
                    list.AddRange(addedlist);
                    CustomEffectTypoData.Remove(__instance);
                }
            }
            catch { }
            return list;
        }

        //Skeleton
        //[HarmonyPatch(typeof(SkeletonJson), "ReadSkeletonData", new Type[] { typeof(TextReader) })]
        //[HarmonyPrefix]
        private static bool SkeletonJson_ReadSkeletonData_Pre(ref SkeletonData __result, TextReader reader, SkeletonJson __instance)
        {
            bool result;
            try
            {
                if (reader == null)
                {
                    throw new ArgumentNullException("reader", "reader cannot be null.");
                }
                float scale = __instance.Scale;
                SkeletonData skeletonData = new SkeletonData();
                if (!(Json.Deserialize(reader) is Dictionary<string, object> dictionary))
                {
                    throw new Exception("Invalid JSON.");
                }
                if (dictionary.ContainsKey("skeleton"))
                {
                    Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["skeleton"];
                    skeletonData.Hash = (string)dictionary2["hash"];
                    skeletonData.Version = (string)dictionary2["spine"];
                    skeletonData.X = SkeletonJSON_new.GetFloat(dictionary2, "x", 0f);
                    skeletonData.Y = SkeletonJSON_new.GetFloat(dictionary2, "y", 0f);
                    skeletonData.Width = SkeletonJSON_new.GetFloat(dictionary2, "width", 0f);
                    skeletonData.Height = SkeletonJSON_new.GetFloat(dictionary2, "height", 0f);
                    skeletonData.Fps = SkeletonJSON_new.GetFloat(dictionary2, "fps", 30f);
                    skeletonData.ImagesPath = SkeletonJSON_new.GetString(dictionary2, "images", null);
                    skeletonData.AudioPath = SkeletonJSON_new.GetString(dictionary2, "audio", null);
                }
                if (dictionary.ContainsKey("bones"))
                {
                    foreach (object obj in ((List<object>)dictionary["bones"]))
                    {
                        Dictionary<string, object> dictionary3 = (Dictionary<string, object>)obj;
                        BoneData boneData = null;
                        if (dictionary3.ContainsKey("parent"))
                        {
                            boneData = skeletonData.FindBone((string)dictionary3["parent"]);
                            if (boneData == null)
                            {
                                string str = "Parent bone not found: ";
                                object obj2 = dictionary3["parent"];
                                throw new Exception(str + (obj2?.ToString()));
                            }
                        }
                        BoneData boneData2 = new BoneData(skeletonData.Bones.Count, (string)dictionary3["name"], boneData)
                        {
                            Length = SkeletonJSON_new.GetFloat(dictionary3, "length", 0f) * scale,
                            X = SkeletonJSON_new.GetFloat(dictionary3, "x", 0f) * scale,
                            Y = SkeletonJSON_new.GetFloat(dictionary3, "y", 0f) * scale,
                            Rotation = SkeletonJSON_new.GetFloat(dictionary3, "rotation", 0f),
                            ScaleX = SkeletonJSON_new.GetFloat(dictionary3, "scaleX", 1f),
                            ScaleY = SkeletonJSON_new.GetFloat(dictionary3, "scaleY", 1f),
                            ShearX = SkeletonJSON_new.GetFloat(dictionary3, "shearX", 0f),
                            ShearY = SkeletonJSON_new.GetFloat(dictionary3, "shearY", 0f)
                        };
                        string @string = SkeletonJSON_new.GetString(dictionary3, "transform", TransformMode.Normal.ToString());
                        boneData2.TransformMode = (TransformMode)Enum.Parse(typeof(TransformMode), @string, true);
                        boneData2.SkinRequired = SkeletonJSON_new.GetBoolean(dictionary3, "skin", false);
                        skeletonData.Bones.Add(boneData2);
                    }
                }
                if (dictionary.ContainsKey("slots"))
                {
                    foreach (object obj3 in ((List<object>)dictionary["slots"]))
                    {
                        Dictionary<string, object> dictionary4 = (Dictionary<string, object>)obj3;
                        string name = (string)dictionary4["name"];
                        string text = (string)dictionary4["bone"];
                        BoneData boneData3 = skeletonData.FindBone(text);
                        if (boneData3 == null)
                        {
                            throw new Exception("Slot bone not found: " + text);
                        }
                        SlotData slotData = new SlotData(skeletonData.Slots.Count, name, boneData3);
                        if (dictionary4.ContainsKey("color"))
                        {
                            string hexString = (string)dictionary4["color"];
                            slotData.R = SkeletonJSON_new.ToColor(hexString, 0, 8);
                            slotData.G = SkeletonJSON_new.ToColor(hexString, 1, 8);
                            slotData.B = SkeletonJSON_new.ToColor(hexString, 2, 8);
                            slotData.A = SkeletonJSON_new.ToColor(hexString, 3, 8);
                        }
                        if (dictionary4.ContainsKey("dark"))
                        {
                            string hexString2 = (string)dictionary4["dark"];
                            slotData.R2 = SkeletonJSON_new.ToColor(hexString2, 0, 6);
                            slotData.G2 = SkeletonJSON_new.ToColor(hexString2, 1, 6);
                            slotData.B2 = SkeletonJSON_new.ToColor(hexString2, 2, 6);
                            slotData.HasSecondColor = true;
                        }
                        slotData.AttachmentName = SkeletonJSON_new.GetString(dictionary4, "attachment", null);
                        if (dictionary4.ContainsKey("blend"))
                        {
                            slotData.BlendMode = (BlendMode)Enum.Parse(typeof(BlendMode), (string)dictionary4["blend"], true);
                        }
                        else
                        {
                            slotData.BlendMode = BlendMode.Normal;
                        }
                        skeletonData.Slots.Add(slotData);
                    }
                }
                if (dictionary.ContainsKey("ik"))
                {
                    foreach (object obj4 in ((List<object>)dictionary["ik"]))
                    {
                        Dictionary<string, object> dictionary5 = (Dictionary<string, object>)obj4;
                        IkConstraintData ikConstraintData = new IkConstraintData((string)dictionary5["name"])
                        {
                            Order = SkeletonJSON_new.GetInt(dictionary5, "order", 0),
                            SkinRequired = SkeletonJSON_new.GetBoolean(dictionary5, "skin", false)
                        };
                        if (dictionary5.ContainsKey("bones"))
                        {
                            foreach (object obj5 in ((List<object>)dictionary5["bones"]))
                            {
                                string text2 = (string)obj5;
                                BoneData boneData4 = skeletonData.FindBone(text2);
                                if (boneData4 == null)
                                {
                                    throw new Exception("IK bone not found: " + text2);
                                }
                                ikConstraintData.Bones.Add(boneData4);
                            }
                        }
                        string text3 = (string)dictionary5["target"];
                        ikConstraintData.Target = skeletonData.FindBone(text3);
                        if (ikConstraintData.Target == null)
                        {
                            throw new Exception("IK target bone not found: " + text3);
                        }
                        ikConstraintData.Mix = SkeletonJSON_new.GetFloat(dictionary5, "mix", 1f);
                        ikConstraintData.Softness = SkeletonJSON_new.GetFloat(dictionary5, "softness", 0f) * scale;
                        ikConstraintData.BendDirection = (SkeletonJSON_new.GetBoolean(dictionary5, "bendPositive", true) ? 1 : -1);
                        ikConstraintData.Compress = SkeletonJSON_new.GetBoolean(dictionary5, "compress", false);
                        ikConstraintData.Stretch = SkeletonJSON_new.GetBoolean(dictionary5, "stretch", false);
                        ikConstraintData.Uniform = SkeletonJSON_new.GetBoolean(dictionary5, "uniform", false);
                        skeletonData.IkConstraints.Add(ikConstraintData);
                    }
                }
                if (dictionary.ContainsKey("transform"))
                {
                    foreach (object obj6 in ((List<object>)dictionary["transform"]))
                    {
                        Dictionary<string, object> dictionary6 = (Dictionary<string, object>)obj6;
                        TransformConstraintData transformConstraintData = new TransformConstraintData((string)dictionary6["name"])
                        {
                            Order = SkeletonJSON_new.GetInt(dictionary6, "order", 0),
                            SkinRequired = SkeletonJSON_new.GetBoolean(dictionary6, "skin", false)
                        };
                        if (dictionary6.ContainsKey("bones"))
                        {
                            foreach (object obj7 in ((List<object>)dictionary6["bones"]))
                            {
                                string text4 = (string)obj7;
                                BoneData boneData5 = skeletonData.FindBone(text4);
                                if (boneData5 == null)
                                {
                                    throw new Exception("Transform constraint bone not found: " + text4);
                                }
                                transformConstraintData.Bones.Add(boneData5);
                            }
                        }
                        string text5 = (string)dictionary6["target"];
                        transformConstraintData.Target = skeletonData.FindBone(text5);
                        if (transformConstraintData.Target == null)
                        {
                            throw new Exception("Transform constraint target bone not found: " + text5);
                        }
                        transformConstraintData.Local = SkeletonJSON_new.GetBoolean(dictionary6, "local", false);
                        transformConstraintData.Relative = SkeletonJSON_new.GetBoolean(dictionary6, "relative", false);
                        transformConstraintData.OffsetRotation = SkeletonJSON_new.GetFloat(dictionary6, "rotation", 0f);
                        transformConstraintData.OffsetX = SkeletonJSON_new.GetFloat(dictionary6, "x", 0f) * scale;
                        transformConstraintData.OffsetY = SkeletonJSON_new.GetFloat(dictionary6, "y", 0f) * scale;
                        transformConstraintData.OffsetScaleX = SkeletonJSON_new.GetFloat(dictionary6, "scaleX", 0f);
                        transformConstraintData.OffsetScaleY = SkeletonJSON_new.GetFloat(dictionary6, "scaleY", 0f);
                        transformConstraintData.OffsetShearY = SkeletonJSON_new.GetFloat(dictionary6, "shearY", 0f);
                        transformConstraintData.RotateMix = SkeletonJSON_new.GetFloat(dictionary6, "rotateMix", 1f);
                        transformConstraintData.TranslateMix = SkeletonJSON_new.GetFloat(dictionary6, "translateMix", 1f);
                        transformConstraintData.ScaleMix = SkeletonJSON_new.GetFloat(dictionary6, "scaleMix", 1f);
                        transformConstraintData.ShearMix = SkeletonJSON_new.GetFloat(dictionary6, "shearMix", 1f);
                        skeletonData.TransformConstraints.Add(transformConstraintData);
                    }
                }
                if (dictionary.ContainsKey("path"))
                {
                    foreach (object obj8 in ((List<object>)dictionary["path"]))
                    {
                        Dictionary<string, object> dictionary7 = (Dictionary<string, object>)obj8;
                        PathConstraintData pathConstraintData = new PathConstraintData((string)dictionary7["name"])
                        {
                            Order = SkeletonJSON_new.GetInt(dictionary7, "order", 0),
                            SkinRequired = SkeletonJSON_new.GetBoolean(dictionary7, "skin", false)
                        };
                        if (dictionary7.ContainsKey("bones"))
                        {
                            foreach (object obj9 in ((List<object>)dictionary7["bones"]))
                            {
                                string text6 = (string)obj9;
                                BoneData boneData6 = skeletonData.FindBone(text6);
                                if (boneData6 == null)
                                {
                                    throw new Exception("Path bone not found: " + text6);
                                }
                                pathConstraintData.Bones.Add(boneData6);
                            }
                        }
                        string text7 = (string)dictionary7["target"];
                        pathConstraintData.Target = skeletonData.FindSlot(text7);
                        if (pathConstraintData.Target == null)
                        {
                            throw new Exception("Path target slot not found: " + text7);
                        }
                        pathConstraintData.PositionMode = (PositionMode)Enum.Parse(typeof(PositionMode), SkeletonJSON_new.GetString(dictionary7, "positionMode", "percent"), true);
                        pathConstraintData.SpacingMode = (SpacingMode)Enum.Parse(typeof(SpacingMode), SkeletonJSON_new.GetString(dictionary7, "spacingMode", "length"), true);
                        pathConstraintData.RotateMode = (RotateMode)Enum.Parse(typeof(RotateMode), SkeletonJSON_new.GetString(dictionary7, "rotateMode", "tangent"), true);
                        pathConstraintData.OffsetRotation = SkeletonJSON_new.GetFloat(dictionary7, "rotation", 0f);
                        pathConstraintData.Position = SkeletonJSON_new.GetFloat(dictionary7, "position", 0f);
                        if (pathConstraintData.PositionMode == PositionMode.Fixed)
                        {
                            pathConstraintData.Position *= scale;
                        }
                        pathConstraintData.Spacing = SkeletonJSON_new.GetFloat(dictionary7, "spacing", 0f);
                        if (pathConstraintData.SpacingMode == SpacingMode.Length || pathConstraintData.SpacingMode == SpacingMode.Fixed)
                        {
                            pathConstraintData.Spacing *= scale;
                        }
                        pathConstraintData.RotateMix = SkeletonJSON_new.GetFloat(dictionary7, "rotateMix", 1f);
                        pathConstraintData.TranslateMix = SkeletonJSON_new.GetFloat(dictionary7, "translateMix", 1f);
                        skeletonData.PathConstraints.Add(pathConstraintData);
                    }
                }
                if (dictionary.ContainsKey("skins"))
                {
                    try
                    {
                        foreach (KeyValuePair<string, object> keyValuePair in ((Dictionary<string, object>)dictionary["skins"]))
                        {
                            Skin skin = new Skin(keyValuePair.Key);
                            foreach (KeyValuePair<string, object> keyValuePair2 in ((Dictionary<string, object>)keyValuePair.Value))
                            {
                                int slotIndex = skeletonData.FindSlotIndex(keyValuePair2.Key);
                                foreach (KeyValuePair<string, object> keyValuePair3 in ((Dictionary<string, object>)keyValuePair2.Value))
                                {
                                    try
                                    {
                                        Attachment attachment = SkeletonJSON_new.ReadAttachment(__instance, (Dictionary<string, object>)keyValuePair3.Value, skin, slotIndex, keyValuePair3.Key, skeletonData);
                                        if (attachment != null)
                                        {
                                            skin.SetAttachment(slotIndex, keyValuePair3.Key, attachment);
                                        }
                                    }
                                    catch (Exception innerException)
                                    {
                                        throw new Exception(string.Concat(new object[]
                                        {
                                    "Error reading attachment: ",
                                    keyValuePair3.Key,
                                    ", skin: ",
                                    skin
                                        }), innerException);
                                    }
                                }
                            }
                            skeletonData.Skins.Add(skin);
                            if (skin.Name == "default")
                            {
                                skeletonData.DefaultSkin = skin;
                            }
                        }
                    }
                    catch
                    {
                        foreach (object obj10 in ((List<object>)dictionary["skins"]))
                        {
                            Dictionary<string, object> dictionary8 = (Dictionary<string, object>)obj10;
                            Skin skin2 = new Skin((string)dictionary8["name"]);
                            if (dictionary8.ContainsKey("attachments"))
                            {
                                foreach (KeyValuePair<string, object> keyValuePair4 in ((Dictionary<string, object>)dictionary8["attachments"]))
                                {
                                    int slotIndex2 = skeletonData.FindSlotIndex(keyValuePair4.Key);
                                    foreach (KeyValuePair<string, object> keyValuePair5 in ((Dictionary<string, object>)keyValuePair4.Value))
                                    {
                                        try
                                        {
                                            Attachment attachment2 = SkeletonJSON_new.ReadAttachment(__instance, (Dictionary<string, object>)keyValuePair5.Value, skin2, slotIndex2, keyValuePair5.Key, skeletonData);
                                            if (attachment2 != null)
                                            {
                                                skin2.SetAttachment(slotIndex2, keyValuePair5.Key, attachment2);
                                            }
                                        }
                                        catch (Exception innerException2)
                                        {
                                            throw new Exception(string.Concat(new object[]
                                            {
                                        "Error reading attachment: ",
                                        keyValuePair5.Key,
                                        ", skin: ",
                                        skin2
                                            }), innerException2);
                                        }
                                    }
                                }
                            }
                            skeletonData.Skins.Add(skin2);
                            if (skin2.Name == "default")
                            {
                                skeletonData.DefaultSkin = skin2;
                            }
                        }
                    }
                }
                int i = 0;
                int count = SkeletonJSON_new.linkedMeshes.Count;
                while (i < count)
                {
                    SkeletonJSON_new.LinkedMesh linkedMesh = SkeletonJSON_new.linkedMeshes[i];
                    Skin skin3 = (linkedMesh.skin == null) ? skeletonData.DefaultSkin : skeletonData.FindSkin(linkedMesh.skin);
                    if (skin3 == null)
                    {
                        throw new Exception("Slot not found: " + linkedMesh.skin);
                    }
                    Attachment attachment3 = skin3.GetAttachment(linkedMesh.slotIndex, linkedMesh.parent);
                    if (attachment3 == null)
                    {
                        throw new Exception("Parent mesh not found: " + linkedMesh.parent);
                    }
                    linkedMesh.mesh.ParentMesh = (MeshAttachment)attachment3;
                    linkedMesh.mesh.UpdateUVs();
                    i++;
                }
                SkeletonJSON_new.linkedMeshes.Clear();
                if (dictionary.ContainsKey("events"))
                {
                    foreach (KeyValuePair<string, object> keyValuePair6 in ((Dictionary<string, object>)dictionary["events"]))
                    {
                        Dictionary<string, object> map = (Dictionary<string, object>)keyValuePair6.Value;
                        EventData eventData = new EventData(keyValuePair6.Key)
                        {
                            Int = SkeletonJSON_new.GetInt(map, "int", 0),
                            Float = SkeletonJSON_new.GetFloat(map, "float", 0f),
                            String = SkeletonJSON_new.GetString(map, "string", string.Empty),
                            AudioPath = SkeletonJSON_new.GetString(map, "audio", null)
                        };
                        if (eventData.AudioPath != null)
                        {
                            eventData.Volume = SkeletonJSON_new.GetFloat(map, "volume", 1f);
                            eventData.Balance = SkeletonJSON_new.GetFloat(map, "balance", 0f);
                        }
                        skeletonData.Events.Add(eventData);
                    }
                }
                if (dictionary.ContainsKey("animations"))
                {
                    foreach (KeyValuePair<string, object> keyValuePair7 in ((Dictionary<string, object>)dictionary["animations"]))
                    {
                        try
                        {
                            SkeletonJSON_new.ReadAnimation(__instance, (Dictionary<string, object>)keyValuePair7.Value, keyValuePair7.Key, skeletonData);
                        }
                        catch (Exception)
                        {
                            try
                            {
                                SkeletonJSON_new.ReadAnimation_new(__instance, (Dictionary<string, object>)keyValuePair7.Value, keyValuePair7.Key, skeletonData);
                            }
                            catch (Exception innerException3)
                            {
                                throw new Exception("Error reading animation: " + keyValuePair7.Key, innerException3);
                            }
                        }
                    }
                }
                skeletonData.Bones.TrimExcess();
                skeletonData.Slots.TrimExcess();
                skeletonData.Skins.TrimExcess();
                skeletonData.Events.TrimExcess();
                skeletonData.Animations.TrimExcess();
                skeletonData.IkConstraints.TrimExcess();
                __result = skeletonData;
                result = false;
            }
            catch (Exception)
            {
                try
                {
                    __result = ReadSkeletonData(__instance, reader);
                    result = false;
                }
                catch (Exception)
                {
                    reader.GetType().GetField("_pos", AccessTools.all).SetValue(reader, 0);
                    result = true;
                }
            }
            return result;
        }
        private static SkeletonData ReadSkeletonData(SkeletonJson __instance, TextReader reader)
        {
            reader.GetType().GetField("_pos", AccessTools.all).SetValue(reader, 0);
            if (reader == null)
            {
                throw new ArgumentNullException("reader", "reader cannot be null.");
            }
            float scale = __instance.Scale;
            SkeletonData skeletonData = new SkeletonData();
            if (!(Json.Deserialize(reader) is Dictionary<string, object> dictionary))
            {
                throw new Exception("Invalid JSON.");
            }
            if (dictionary.ContainsKey("skeleton"))
            {
                Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["skeleton"];
                skeletonData.Hash = (string)dictionary2["hash"];
                if (dictionary2.ContainsKey("spine"))
                {
                    skeletonData.Version = (string)dictionary2["spine"];
                }
                else
                {
                    skeletonData.Version = "3.6.50";
                }
                skeletonData.Width = SkeletonJSON_new.GetFloat(dictionary2, "width", 0f);
                skeletonData.Height = SkeletonJSON_new.GetFloat(dictionary2, "height", 0f);
                skeletonData.Fps = SkeletonJSON_new.GetFloat(dictionary2, "fps", 30f);
                skeletonData.ImagesPath = SkeletonJSON_new.GetString(dictionary2, "images", null);
            }
            if (dictionary.ContainsKey("bones"))
            {
                foreach (object obj in ((List<object>)dictionary["bones"]))
                {
                    Dictionary<string, object> dictionary3 = (Dictionary<string, object>)obj;
                    BoneData boneData = null;
                    if (dictionary3.ContainsKey("parent"))
                    {
                        boneData = skeletonData.FindBone((string)dictionary3["parent"]);
                        if (boneData == null)
                        {
                            string str = "Parent bone not found: ";
                            object obj2 = dictionary3["parent"];
                            throw new Exception(str + (obj2?.ToString()));
                        }
                    }
                    BoneData boneData2 = new BoneData(skeletonData.Bones.Count, (string)dictionary3["name"], boneData)
                    {
                        Length = SkeletonJSON_new.GetFloat(dictionary3, "length", 0f) * scale,
                        X = SkeletonJSON_new.GetFloat(dictionary3, "x", 0f) * scale,
                        Y = SkeletonJSON_new.GetFloat(dictionary3, "y", 0f) * scale,
                        Rotation = SkeletonJSON_new.GetFloat(dictionary3, "rotation", 0f),
                        ScaleX = SkeletonJSON_new.GetFloat(dictionary3, "scaleX", 1f),
                        ScaleY = SkeletonJSON_new.GetFloat(dictionary3, "scaleY", 1f),
                        ShearX = SkeletonJSON_new.GetFloat(dictionary3, "shearX", 0f),
                        ShearY = SkeletonJSON_new.GetFloat(dictionary3, "shearY", 0f)
                    };
                    string @string = SkeletonJSON_new.GetString(dictionary3, "transform", TransformMode.Normal.ToString());
                    boneData2.TransformMode = (TransformMode)Enum.Parse(typeof(TransformMode), @string, true);
                    skeletonData.Bones.Add(boneData2);
                }
            }
            if (dictionary.ContainsKey("slots"))
            {
                foreach (object obj3 in ((List<object>)dictionary["slots"]))
                {
                    Dictionary<string, object> dictionary4 = (Dictionary<string, object>)obj3;
                    string name = (string)dictionary4["name"];
                    string text = (string)dictionary4["bone"];
                    BoneData boneData3 = skeletonData.FindBone(text);
                    if (boneData3 == null)
                    {
                        throw new Exception("Slot bone not found: " + text);
                    }
                    SlotData slotData = new SlotData(skeletonData.Slots.Count, name, boneData3);
                    if (dictionary4.ContainsKey("color"))
                    {
                        string hexString = (string)dictionary4["color"];
                        slotData.R = SkeletonJSON_new.ToColor(hexString, 0, 8);
                        slotData.G = SkeletonJSON_new.ToColor(hexString, 1, 8);
                        slotData.B = SkeletonJSON_new.ToColor(hexString, 2, 8);
                        slotData.A = SkeletonJSON_new.ToColor(hexString, 3, 8);
                    }
                    if (dictionary4.ContainsKey("dark"))
                    {
                        string hexString2 = (string)dictionary4["dark"];
                        slotData.R2 = SkeletonJSON_new.ToColor(hexString2, 0, 6);
                        slotData.G2 = SkeletonJSON_new.ToColor(hexString2, 1, 6);
                        slotData.B2 = SkeletonJSON_new.ToColor(hexString2, 2, 6);
                        slotData.HasSecondColor = true;
                    }
                    slotData.AttachmentName = SkeletonJSON_new.GetString(dictionary4, "attachment", null);
                    if (dictionary4.ContainsKey("blend"))
                    {
                        slotData.BlendMode = (BlendMode)Enum.Parse(typeof(BlendMode), (string)dictionary4["blend"], true);
                    }
                    else
                    {
                        slotData.BlendMode = BlendMode.Normal;
                    }
                    skeletonData.Slots.Add(slotData);
                }
            }
            if (dictionary.ContainsKey("ik"))
            {
                foreach (object obj4 in ((List<object>)dictionary["ik"]))
                {
                    Dictionary<string, object> dictionary5 = (Dictionary<string, object>)obj4;
                    IkConstraintData ikConstraintData = new IkConstraintData((string)dictionary5["name"])
                    {
                        Order = SkeletonJSON_new.GetInt(dictionary5, "order", 0)
                    };
                    if (dictionary5.ContainsKey("bones"))
                    {
                        foreach (object obj5 in ((List<object>)dictionary5["bones"]))
                        {
                            string text2 = (string)obj5;
                            BoneData boneData4 = skeletonData.FindBone(text2);
                            if (boneData4 == null)
                            {
                                throw new Exception("IK bone not found: " + text2);
                            }
                            ikConstraintData.Bones.Add(boneData4);
                        }
                    }
                    string text3 = (string)dictionary5["target"];
                    ikConstraintData.Target = skeletonData.FindBone(text3);
                    if (ikConstraintData.Target == null)
                    {
                        throw new Exception("IK target bone not found: " + text3);
                    }
                    ikConstraintData.Mix = SkeletonJSON_new.GetFloat(dictionary5, "mix", 1f);
                    ikConstraintData.BendDirection = (SkeletonJSON_new.GetBoolean(dictionary5, "bendPositive", true) ? 1 : -1);
                    skeletonData.IkConstraints.Add(ikConstraintData);
                }
            }
            if (dictionary.ContainsKey("transform"))
            {
                foreach (object obj6 in ((List<object>)dictionary["transform"]))
                {
                    Dictionary<string, object> dictionary6 = (Dictionary<string, object>)obj6;
                    TransformConstraintData transformConstraintData = new TransformConstraintData((string)dictionary6["name"])
                    {
                        Order = SkeletonJSON_new.GetInt(dictionary6, "order", 0)
                    };
                    if (dictionary6.ContainsKey("bones"))
                    {
                        foreach (object obj7 in ((List<object>)dictionary6["bones"]))
                        {
                            string text4 = (string)obj7;
                            BoneData boneData5 = skeletonData.FindBone(text4);
                            if (boneData5 == null)
                            {
                                throw new Exception("Transform constraint bone not found: " + text4);
                            }
                            transformConstraintData.Bones.Add(boneData5);
                        }
                    }
                    string text5 = (string)dictionary6["target"];
                    transformConstraintData.Target = skeletonData.FindBone(text5);
                    if (transformConstraintData.Target == null)
                    {
                        throw new Exception("Transform constraint target bone not found: " + text5);
                    }
                    transformConstraintData.Local = SkeletonJSON_new.GetBoolean(dictionary6, "local", false);
                    transformConstraintData.Relative = SkeletonJSON_new.GetBoolean(dictionary6, "relative", false);
                    transformConstraintData.OffsetRotation = SkeletonJSON_new.GetFloat(dictionary6, "rotation", 0f);
                    transformConstraintData.OffsetX = SkeletonJSON_new.GetFloat(dictionary6, "x", 0f) * scale;
                    transformConstraintData.OffsetY = SkeletonJSON_new.GetFloat(dictionary6, "y", 0f) * scale;
                    transformConstraintData.OffsetScaleX = SkeletonJSON_new.GetFloat(dictionary6, "scaleX", 0f);
                    transformConstraintData.OffsetScaleY = SkeletonJSON_new.GetFloat(dictionary6, "scaleY", 0f);
                    transformConstraintData.OffsetShearY = SkeletonJSON_new.GetFloat(dictionary6, "shearY", 0f);
                    transformConstraintData.RotateMix = SkeletonJSON_new.GetFloat(dictionary6, "rotateMix", 1f);
                    transformConstraintData.TranslateMix = SkeletonJSON_new.GetFloat(dictionary6, "translateMix", 1f);
                    transformConstraintData.ScaleMix = SkeletonJSON_new.GetFloat(dictionary6, "scaleMix", 1f);
                    transformConstraintData.ShearMix = SkeletonJSON_new.GetFloat(dictionary6, "shearMix", 1f);
                    skeletonData.TransformConstraints.Add(transformConstraintData);
                }
            }
            if (dictionary.ContainsKey("path"))
            {
                foreach (object obj8 in ((List<object>)dictionary["path"]))
                {
                    Dictionary<string, object> dictionary7 = (Dictionary<string, object>)obj8;
                    PathConstraintData pathConstraintData = new PathConstraintData((string)dictionary7["name"])
                    {
                        Order = SkeletonJSON_new.GetInt(dictionary7, "order", 0)
                    };
                    if (dictionary7.ContainsKey("bones"))
                    {
                        foreach (object obj9 in ((List<object>)dictionary7["bones"]))
                        {
                            string text6 = (string)obj9;
                            BoneData boneData6 = skeletonData.FindBone(text6);
                            if (boneData6 == null)
                            {
                                throw new Exception("Path bone not found: " + text6);
                            }
                            pathConstraintData.Bones.Add(boneData6);
                        }
                    }
                    string text7 = (string)dictionary7["target"];
                    pathConstraintData.Target = skeletonData.FindSlot(text7);
                    if (pathConstraintData.Target == null)
                    {
                        throw new Exception("Path target slot not found: " + text7);
                    }
                    pathConstraintData.PositionMode = (PositionMode)Enum.Parse(typeof(PositionMode), SkeletonJSON_new.GetString(dictionary7, "positionMode", "percent"), true);
                    pathConstraintData.SpacingMode = (SpacingMode)Enum.Parse(typeof(SpacingMode), SkeletonJSON_new.GetString(dictionary7, "spacingMode", "length"), true);
                    pathConstraintData.RotateMode = (RotateMode)Enum.Parse(typeof(RotateMode), SkeletonJSON_new.GetString(dictionary7, "rotateMode", "tangent"), true);
                    pathConstraintData.OffsetRotation = SkeletonJSON_new.GetFloat(dictionary7, "rotation", 0f);
                    pathConstraintData.Position = SkeletonJSON_new.GetFloat(dictionary7, "position", 0f);
                    if (pathConstraintData.PositionMode == PositionMode.Fixed)
                    {
                        pathConstraintData.Position *= scale;
                    }
                    pathConstraintData.Spacing = SkeletonJSON_new.GetFloat(dictionary7, "spacing", 0f);
                    if (pathConstraintData.SpacingMode == SpacingMode.Length || pathConstraintData.SpacingMode == SpacingMode.Fixed)
                    {
                        pathConstraintData.Spacing *= scale;
                    }
                    pathConstraintData.RotateMix = SkeletonJSON_new.GetFloat(dictionary7, "rotateMix", 1f);
                    pathConstraintData.TranslateMix = SkeletonJSON_new.GetFloat(dictionary7, "translateMix", 1f);
                    skeletonData.PathConstraints.Add(pathConstraintData);
                }
            }
            if (dictionary.ContainsKey("skins"))
            {
                try
                {
                    foreach (KeyValuePair<string, object> keyValuePair in ((Dictionary<string, object>)dictionary["skins"]))
                    {
                        Skin skin = new Skin(keyValuePair.Key);
                        foreach (KeyValuePair<string, object> keyValuePair2 in ((Dictionary<string, object>)keyValuePair.Value))
                        {
                            int slotIndex = skeletonData.FindSlotIndex(keyValuePair2.Key);
                            foreach (KeyValuePair<string, object> keyValuePair3 in ((Dictionary<string, object>)keyValuePair2.Value))
                            {
                                Attachment attachment = SkeletonJSON_new.ReadAttachment(__instance, (Dictionary<string, object>)keyValuePair3.Value, skin, slotIndex, keyValuePair3.Key, skeletonData);
                                if (attachment != null)
                                {
                                    skin.SetAttachment(slotIndex, keyValuePair3.Key, attachment);
                                }
                            }
                        }
                        skeletonData.Skins.Add(skin);
                        if (skin.Name == "default")
                        {
                            skeletonData.DefaultSkin = skin;
                        }
                    }
                }
                catch (Exception)
                {
                    foreach (object obj10 in ((List<object>)dictionary["skins"]))
                    {
                        Dictionary<string, object> dictionary8 = (Dictionary<string, object>)obj10;
                        Skin skin2 = new Skin((string)dictionary8["name"]);
                        if (dictionary8.ContainsKey("attachments"))
                        {
                            foreach (KeyValuePair<string, object> keyValuePair4 in ((Dictionary<string, object>)dictionary8["attachments"]))
                            {
                                int slotIndex2 = skeletonData.FindSlotIndex(keyValuePair4.Key);
                                foreach (KeyValuePair<string, object> keyValuePair5 in ((Dictionary<string, object>)keyValuePair4.Value))
                                {
                                    try
                                    {
                                        Attachment attachment2 = SkeletonJSON_new.ReadAttachment_new(__instance, (Dictionary<string, object>)keyValuePair5.Value, skin2, slotIndex2, keyValuePair5.Key, skeletonData);
                                        if (attachment2 != null)
                                        {
                                            skin2.SetAttachment(slotIndex2, keyValuePair5.Key, attachment2);
                                        }
                                    }
                                    catch (Exception innerException)
                                    {
                                        throw new Exception(string.Concat(new object[]
                                        {
                                    "Error reading attachment: ",
                                    keyValuePair5.Key,
                                    ", skin: ",
                                    skin2
                                        }), innerException);
                                    }
                                }
                            }
                        }
                        skeletonData.Skins.Add(skin2);
                        if (skin2.Name == "default")
                        {
                            skeletonData.DefaultSkin = skin2;
                        }
                    }
                }
            }
            int i = 0;
            int count = SkeletonJSON_new.linkedMeshes.Count;
            while (i < count)
            {
                SkeletonJSON_new.LinkedMesh linkedMesh = SkeletonJSON_new.linkedMeshes[i];
                Skin skin3 = (linkedMesh.skin == null) ? skeletonData.DefaultSkin : skeletonData.FindSkin(linkedMesh.skin);
                if (skin3 == null)
                {
                    throw new Exception("Slot not found: " + linkedMesh.skin);
                }
                Attachment attachment3 = skin3.GetAttachment(linkedMesh.slotIndex, linkedMesh.parent);
                if (attachment3 == null)
                {
                    throw new Exception("Parent mesh not found: " + linkedMesh.parent);
                }
                linkedMesh.mesh.ParentMesh = (MeshAttachment)attachment3;
                linkedMesh.mesh.UpdateUVs();
                i++;
            }
            SkeletonJSON_new.linkedMeshes.Clear();
            if (dictionary.ContainsKey("events"))
            {
                foreach (KeyValuePair<string, object> keyValuePair6 in ((Dictionary<string, object>)dictionary["events"]))
                {
                    Dictionary<string, object> map = (Dictionary<string, object>)keyValuePair6.Value;
                    EventData eventData = new EventData(keyValuePair6.Key)
                    {
                        Int = SkeletonJSON_new.GetInt(map, "int", 0),
                        Float = SkeletonJSON_new.GetFloat(map, "float", 0f),
                        String = SkeletonJSON_new.GetString(map, "string", string.Empty)
                    };
                    skeletonData.Events.Add(eventData);
                }
            }
            if (dictionary.ContainsKey("animations"))
            {
                foreach (KeyValuePair<string, object> keyValuePair7 in ((Dictionary<string, object>)dictionary["animations"]))
                {
                    try
                    {
                        SkeletonJSON_new.ReadAnimation_new(__instance, (Dictionary<string, object>)keyValuePair7.Value, keyValuePair7.Key, skeletonData);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            SkeletonJSON_new.ReadAnimation(__instance, (Dictionary<string, object>)keyValuePair7.Value, keyValuePair7.Key, skeletonData);
                        }
                        catch (Exception innerException2)
                        {
                            throw new Exception("Error reading animation: " + keyValuePair7.Key, innerException2);
                        }
                    }
                }
            }
            skeletonData.Bones.TrimExcess();
            skeletonData.Slots.TrimExcess();
            skeletonData.Skins.TrimExcess();
            skeletonData.Events.TrimExcess();
            skeletonData.Animations.TrimExcess();
            skeletonData.IkConstraints.TrimExcess();
            return skeletonData;
        }
        //?
        private static bool RegionlessAttachmentLoader_get_EmptyRegion(ref AtlasRegion __result)
        {
            FieldInfo field = typeof(Spine.Unity.RegionlessAttachmentLoader).GetField("emptyRegion", AccessTools.all);
            if (field.GetValue(null) == null)
            {
                AtlasRegion value = new AtlasRegion
                {
                    name = "Empty AtlasRegion",
                    page = new AtlasPage
                    {
                        name = "Empty AtlasPage",
                        rendererObject = new Material(Shader.Find("UI/Default"))
                        {
                            name = "NoRender Material"
                        }
                    }
                };
                field.SetValue(null, value);
            }
            __result = (AtlasRegion)field.GetValue(null);
            return false;
        }

        private static string GetCardName(LorId cardID)
        {
            string result = "Not Found";
            if (cardID == 705010)
            {
                cardID = new LorId(707001);
            }
            if (Singleton<BattleCardDescXmlList>.Instance._dictionary.ContainsKey(cardID))
            {
                result = Singleton<BattleCardDescXmlList>.Instance._dictionary[cardID].cardName;
            }
            return result;
        }
        private static void GetArtWorks()
        {
            ArtWorks = new Dictionary<string, Sprite>();
            foreach (ModContent modContent in LoadedModContents)
            {
                DirectoryInfo directoryInfo = modContent._dirInfo;
                if (Directory.Exists(directoryInfo.FullName + "/ArtWork"))
                {
                    DirectoryInfo directoryInfo2 = new DirectoryInfo(directoryInfo.FullName + "/ArtWork");
                    if (directoryInfo2.GetDirectories().Length != 0)
                    {
                        DirectoryInfo[] directories = directoryInfo2.GetDirectories();
                        for (int i = 0; i < directories.Length; i++)
                        {
                            GetArtWorks(directories[i]);
                        }
                    }
                    foreach (FileInfo fileInfo in directoryInfo2.GetFiles())
                    {
                        Texture2D texture2D = new Texture2D(2, 2);
                        texture2D.LoadImage(File.ReadAllBytes(fileInfo.FullName));
                        Sprite value = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                        ArtWorks[fileNameWithoutExtension] = value;
                    }
                }
            }
        }
        private static void GetArtWorks(DirectoryInfo dir)
        {
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    GetArtWorks(directories[i]);
                }
            }
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                Texture2D texture2D = new Texture2D(2, 2);
                texture2D.LoadImage(File.ReadAllBytes(fileInfo.FullName));
                Sprite value = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                ArtWorks[fileNameWithoutExtension] = value;
            }
        }
        public static string BuildPath(params string[] paths)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Xml/");
            foreach (string value in paths)
            {
                stringBuilder.Append(value);
            }
            return stringBuilder.ToString();
        }
        public class ModStroyCG
        {
            public string path;
            public Sprite sprite;
        }

        private static string path = string.Empty;

        private static string Staticpath;

        private static string StoryStaticpath;

        private static string Storylocalizepath;

        private static string Localizepath;

        private static List<Assembly> AssemList;

        private static List<string> LoadedAssembly;

        public static Dictionary<string, Type> CustomEffects = new Dictionary<string, Type>();

        public static Dictionary<string, Type> CustomMapManager = new Dictionary<string, Type>();

        public static Dictionary<string, Type> CustomBattleDialogModel = new Dictionary<string, Type>();

        public static Dictionary<string, Type> CustomGiftPassive = new Dictionary<string, Type>();

        public static Dictionary<string, Type> CustomEmotionCardAbility = new Dictionary<string, Type>();

        public static Dictionary<string, int> CoreThumbDic = new Dictionary<string, int>();

        public static Dictionary<BattleCardBehaviourResult, List<EffectTypoData>> CustomEffectTypoData = new Dictionary<BattleCardBehaviourResult, List<EffectTypoData>>();

        public static Dictionary<string, Sprite> ArtWorks = null;

        public static Dictionary<LorId, Sprite> BookThumb = null;

        public static Dictionary<string, AudioClip> AudioClips = null;

        public static bool IsModStorySelected;

        private static List<string> ModPid;

        public static Dictionary<LorId, int> ModEpMatch;

        public static Dictionary<LorId, ModStroyCG> ModStoryCG = null;

        public static Dictionary<Assembly, string> ModWorkShopId;

        private static bool IsEditing = false;

        private static DiceCardXmlInfo errNullCard = null;

        public static UIEquipPageCustomizePanel uiEquipPageCustomizePanel;

        public static bool InitWorkshopSkinChangeButton;

        public static bool isModWorkshopSkin;

        public static Dictionary<int, LorId> ModWorkshopBookIndex;
    }
}
