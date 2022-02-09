using Battle.DiceAttackEffect;
using GameSave;
using GTMDProjectMoon;
using HarmonyLib;
using LOR_DiceSystem;
using LOR_XML;
using Mod;
using MyJsonTool;
using NAudio.Wave;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Opening;

namespace BaseMod
{
    public class Harmony_Patch : ModInitializer
    {
        public Harmony_Patch()
        {
            GlobalGameManager.Instance.ver = string.Concat(new string[]
            {
            GlobalGameManager.Instance.ver,
            Environment.NewLine,
            "BaseMod_New 2.11 ver",
            Environment.NewLine,
            "by Boss An's bug rasing floor"
            });
            if (File.Exists(Application.dataPath + "/Mods/Player.log"))
            {
                File.Delete(Application.dataPath + "/Mods/Player.log");
            }
            Application.logMessageReceivedThreaded += delegate (string condition, string stackTrace, LogType type)
            {
                using (StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Mods/Player.log", true))
                {
                    string value = string.Concat(new string[]
                    {
                DateTime.Now.ToString("HH:mm:ss"),
                "  ",
                type.ToString(),
                " :",
                condition,
                "\n"
                    });
                    if (type == LogType.Error)
                    {
                        value += stackTrace;
                        value += "\n";
                    }
                    streamWriter.Write(value);
                }
            };
            List<string> list = Singleton<ModContentManager>.Instance.GetErrorLogs();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Contains("BaseMod"))
                {
                    list.RemoveAt(i);
                }
            }
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
                Language = TextDataModel.CurrentLanguage;
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
                Language = StorySerializer.currentLn;
                Storylocalizepath = Directory.CreateDirectory(Application.dataPath + "/Managed/BaseMod/Story/Localize/" + Language).FullName;
                return Storylocalizepath;
            }
        }
        public static List<ModContent> LoadedModContents
        {
            get
            {
                return Singleton<ModContentManager>.Instance.GetType().GetField("_loadedContents", AccessTools.all).GetValue(Singleton<ModContentManager>.Instance) as List<ModContent>;
            }
        }
        public override void OnInitializeMod()
        {
            try
            {
                path = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
                IsEditing = false;
                patchNum = 1;
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
                _bufIcon = typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all);
                IsModStorySelected = false;
                Language = TextDataModel.CurrentLanguage;
                Harmony harmony = new Harmony("BaseMod");
                //原本功能移植 0
                MethodInfo method = typeof(Harmony_Patch).GetMethod("SdCharacterUtil_CreateSkin_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(SdCharacterUtil).GetMethod("CreateSkin", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("WorkshopSkinDataSetter_SetMotionData_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(Workshop.WorkshopSkinDataSetter).GetMethod("SetMotionData", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("AssetBundleManagerRemake_LoadCardSprite_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(AssetBundleManagerRemake).GetMethod("LoadCardSprite", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("CustomizingCardArtworkLoader_GetSpecificArtworkSprite_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(CustomizingCardArtworkLoader).GetMethod("GetSpecificArtworkSprite", AccessTools.all), PatchType.postfix);
                method = typeof(Harmony_Patch).GetMethod("UISpriteDataManager_GetStoryIcon_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UISpriteDataManager).GetMethod("GetStoryIcon", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UIAbnormalityCardPreviewSlot_Init_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIAbnormalityCardPreviewSlot).GetMethod("Init", AccessTools.all), PatchType.postfix);
                method = typeof(Harmony_Patch).GetMethod("BattleUnitDiceActionUI_EmotionCard_Init_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BattleUnitDiceActionUI_EmotionCard).GetMethod("Init", AccessTools.all), PatchType.postfix);
                method = typeof(Harmony_Patch).GetMethod("UIEmotionPassiveCardInven_SetSprites_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIEmotionPassiveCardInven).GetMethod("SetSprites", AccessTools.all), PatchType.postfix);
                method = typeof(Harmony_Patch).GetMethod("EmotionPassiveCardUI_SetSprites_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(EmotionPassiveCardUI).GetMethod("SetSprites", AccessTools.all), PatchType.postfix);
                method = typeof(Harmony_Patch).GetMethod("BookModel_get_bookIcon_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BookModel).GetMethod("get_bookIcon", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("BattleUnitInformationUI_PassiveList_SetData_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BattleUnitInformationUI_PassiveList).GetMethod("SetData", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("DiceEffectManager_CreateBehaviourEffect_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(DiceEffectManager).GetMethod("CreateBehaviourEffect", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UnitDataModel_InitBattleDialogByDefaultBook_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UnitDataModel).GetMethod("InitBattleDialogByDefaultBook", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("BattleEmotionCardModel_ctor_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BattleEmotionCardModel).GetConstructor(new Type[]
                {
                    typeof(EmotionCardXmlInfo),
                    typeof(BattleUnitModel)
                }), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UIBattleSettingWaveList_SetData_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIBattleSettingWaveList).GetMethod("SetData", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UIOriginEquipPageList_UpdateEquipPageList_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIOriginEquipPageList).GetMethod("UpdateEquipPageList", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UIBookStoryChapterSlot_SetEpisodeSlots_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIBookStoryChapterSlot).GetMethod("SetEpisodeSlots", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UIStoryArchivesPanel_InitData_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIStoryArchivesPanel).GetMethod("InitData", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UIBookStoryPanel_OnSelectEpisodeSlot_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIBookStoryPanel).GetMethod("OnSelectEpisodeSlot", AccessTools.all), PatchType.postfix);
                method = typeof(Harmony_Patch).GetMethod("StageController_InitializeMap_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(StageController).GetMethod("InitializeMap", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("BattleSceneRoot_ChangeToEgoMap_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BattleSceneRoot).GetMethod("ChangeToEgoMap", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("BattleSimpleActionUI_Dice_SetDiceValue_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BattleSimpleActionUI_Dice).GetMethod("SetDiceValue", AccessTools.all), PatchType.prefix);
                //重加载功能支持 22
                /*method = typeof(Harmony_Patch).GetMethod("Mod_Update", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIPopupWindowManager).GetMethod("Update", AccessTools.all), PatchType.postfix);*/
                method = typeof(Harmony_Patch).GetMethod("ModContentManager_SetActiveContents_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(ModContentManager).GetMethod("SetActiveContents", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UIModPopup_Close_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIModPopup).GetMethod("Close", AccessTools.all), PatchType.postfix);
                //GTMD月亮计划 24
                method = typeof(Harmony_Patch).GetMethod("BookModel_SetXmlInfo_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BookModel).GetMethod("SetXmlInfo", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("BookModel_CreateSoulCard_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BookModel).GetMethod("CreateSoulCard", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("BattleCardAbilityDescXmlList_GetAbilityKeywords_byScript_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BattleCardAbilityDescXmlList).GetMethod("GetAbilityKeywords_byScript", AccessTools.all), PatchType.prefix);
                //战斗书页名称与核心书页等名称读取 27
                method = typeof(Harmony_Patch).GetMethod("BookXmlInfo_get_Name_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BookXmlInfo).GetMethod("get_Name", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("BookXmlInfo_get_Desc_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BookXmlInfo).GetMethod("get_Desc", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("BattleDiceCardModel_GetName_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BattleDiceCardModel).GetMethod("GetName", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("DiceCardItemModel_GetName_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(DiceCardItemModel).GetMethod("GetName", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("DiceCardXmlInfo_get_Name_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(DiceCardXmlInfo).GetMethod("get_Name", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("DropBookXmlInfo_get_Name_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(DropBookXmlInfo).GetMethod("get_Name", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UnitDataModel_SetByEnemyUnitClassInfo_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UnitDataModel).GetMethod("SetByEnemyUnitClassInfo", AccessTools.all), PatchType.postfix);
                method = typeof(Harmony_Patch).GetMethod("CharactersNameXmlList_GetName_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(CharactersNameXmlList).GetMethod("GetName", AccessTools.all, null, new Type[]
                {
                    typeof(LorId)
                }, null), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("BookPassiveInfo_get_name_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BookPassiveInfo).GetMethod("get_name", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("BookPassiveInfo_get_desc_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BookPassiveInfo).GetMethod("get_desc", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("StageNameXmlList_GetName_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(StageNameXmlList).GetMethod("GetName", AccessTools.all, null, new Type[]
                {
                    typeof(StageClassInfo)
                }, null), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("BattleDiceCardUI_SetCard_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BattleDiceCardUI).GetMethod("SetCard", AccessTools.all), PatchType.postfix);
                method = typeof(Harmony_Patch).GetMethod("BattleDiceCardUI_SetEgoCardForPopup_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BattleDiceCardUI).GetMethod("SetEgoCardForPopup", AccessTools.all), PatchType.postfix);
                //Story读取修改 40       
                method = typeof(Harmony_Patch).GetMethod("StorySerializer_HasEffectFile_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(StorySerializer).GetMethod("HasEffectFile", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("StorySerializer_LoadStageStory_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(StorySerializer).GetMethod("LoadStageStory", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UIBattleStoryInfoPanel_SetData_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIBattleStoryInfoPanel).GetMethod("SetData", AccessTools.all), PatchType.postfix);
                method = typeof(Harmony_Patch).GetMethod("StoryManager_LoadBackgroundSprite_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(StoryScene.StoryManager).GetMethod("LoadBackgroundSprite", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UIStoryProgressPanel_SelectedSlot_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIStoryProgressPanel).GetMethod("SelectedSlot", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UIInvitationRightMainPanel_SetCustomInvToggle_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIInvitationRightMainPanel).GetMethod("SetCustomInvToggle", AccessTools.all), PatchType.postfix);
                //切换语言时重新加载XML 46
                method = typeof(Harmony_Patch).GetMethod("TextDataModel_InitTextData_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(TextDataModel).GetMethod("InitTextData", AccessTools.all), PatchType.postfix);
                method = typeof(Harmony_Patch).GetMethod("StorySerializer_LoadStory_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(StorySerializer).GetMethod("LoadStory", AccessTools.all, null, new Type[]
                {
                    typeof(bool)
                }, null), PatchType.postfix);
                //核心书页库槽位 48
                method = typeof(Harmony_Patch).GetMethod("UIEquipPageScrollList_SetData_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIEquipPageScrollList).GetMethod("SetData", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UISettingEquipPageScrollList_SetData_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UISettingEquipPageScrollList).GetMethod("SetData", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UISettingInvenEquipPageListSlot_SetBooksData_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UISettingInvenEquipPageListSlot).GetMethod("SetBooksData", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UIInvenEquipPageListSlot_SetBooksData_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIInvenEquipPageListSlot).GetMethod("SetBooksData", AccessTools.all), PatchType.prefix);
                //书页预览修改 52
                method = typeof(Harmony_Patch).GetMethod("BookModel_GetThumbSprite_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BookModel).GetMethod("GetThumbSprite", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("BookXmlInfo_GetThumbSprite_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BookXmlInfo).GetMethod("GetThumbSprite", AccessTools.all), PatchType.prefix);
                //饰品迁移  54
                method = typeof(Harmony_Patch).GetMethod("CharacterAppearance_CreateGiftData_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(CharacterAppearance).GetMethod("CreateGiftData", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("GiftModel_CreateScripts_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(GiftModel).GetMethod("CreateScripts", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UIGiftDataSlot_SetData_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIGiftDataSlot).GetMethod("SetData", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UIGiftInvenSlot_SetData_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIGiftInvenSlot).GetMethod("SetData", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UIGiftPreviewSlot_UpdateSlot_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIGiftPreviewSlot).GetMethod("UpdateSlot", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("GiftInventory_LoadFromSaveData_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(GiftInventory).GetMethod("LoadFromSaveData", AccessTools.all), PatchType.prefix);
                //皮肤切换 60
                method = typeof(Harmony_Patch).GetMethod("BattleUnitView_ChangeEgoSkin_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BattleUnitView).GetMethod("ChangeEgoSkin", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("BattleUnitView_ChangeSkin_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BattleUnitView).GetMethod("ChangeSkin", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("VersionViewer_Start_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(VersionViewer).GetMethod("Start", AccessTools.all), PatchType.prefix);
                //Spine迁移 63
                method = typeof(Harmony_Patch).GetMethod("SkeletonJson_ReadSkeletonData_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(SkeletonJson).GetMethod("ReadSkeletonData", AccessTools.all, null, new Type[]
                {
                   typeof(TextReader)
                }, null), PatchType.prefix);
                //小修小补
                //加载页面读取ModCG 64
                method = typeof(Harmony_Patch).GetMethod("StageController_GameOver_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(StageController).GetMethod("GameOver", AccessTools.all), PatchType.postfix);
                method = typeof(Harmony_Patch).GetMethod("EntryScene_SetCG_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(EntryScene).GetMethod("SetCG", AccessTools.all), PatchType.postfix);
                //自定义Ego扩展 66
                method = typeof(Harmony_Patch).GetMethod("EmotionEgoXmlInfo_get_CardId_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(EmotionEgoXmlInfo).GetMethod("get_CardId", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("EmotionEgoXmlList_GetData_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(EmotionEgoXmlList).GetMethod("GetData", AccessTools.all, null, new Type[]
                {
                    typeof(LorId)
                }, null), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("EmotionEgoCardUI_Init_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(EmotionEgoCardUI).GetMethod("Init", AccessTools.all), PatchType.prefix);
                //Mod物品排序 69
                method = typeof(Harmony_Patch).GetMethod("UIInvitationDropBookList_ApplyFilterAll_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIInvitationDropBookList).GetMethod("ApplyFilterAll", AccessTools.all), PatchType.postfix);
                method = typeof(Harmony_Patch).GetMethod("UIInvenFeedBookList_ApplyFilterAll_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIInvenFeedBookList).GetMethod("ApplyFilterAll", AccessTools.all), PatchType.postfix);
                method = typeof(Harmony_Patch).GetMethod("UIInvenCardListScroll_ApplyFilterAll_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIInvenCardListScroll).GetMethod("ApplyFilterAll", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UIEquipPageCustomizePanel_ApplyFilterAll_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIEquipPageCustomizePanel).GetMethod("ApplyFilterAll", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UIEquipPageCustomizePanel_UpdateEquipPageSlotList_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIEquipPageCustomizePanel).GetMethod("UpdateEquipPageSlotList", AccessTools.all), PatchType.prefix);
                //errNullCard 74
                method = typeof(Harmony_Patch).GetMethod("ItemXmlDataList_GetCardItem_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(ItemXmlDataList).GetMethod("GetCardItem", AccessTools.all, null, new Type[]
                {
                    typeof(LorId),
                    typeof(bool)
                }, null), PatchType.prefix);
                //StageExtraCondition 75
                method = typeof(Harmony_Patch).GetMethod("StageExtraCondition_IsUnlocked_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(StageExtraCondition).GetMethod("IsUnlocked", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UIInvitationRightMainPanel_SendInvitation_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIInvitationRightMainPanel).GetMethod("SendInvitation", AccessTools.all), PatchType.prefix);
                //BufIcon 77
                method = typeof(Harmony_Patch).GetMethod("BattleUnitBuf_GetBufIcon_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BattleUnitBuf).GetMethod("GetBufIcon", AccessTools.all), PatchType.prefix);
                //CardBufIcon 78
                method = typeof(Harmony_Patch).GetMethod("BattleDiceCardBuf_GetBufIcon_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BattleDiceCardBuf).GetMethod("GetBufIcon", AccessTools.all), PatchType.prefix);
                //DiceCardXmlInfo Copy 复制keywordlist 79 不合理，已弃用
                /*method = typeof(Harmony_Patch).GetMethod("DiceCardXmlInfo_Copy_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(DiceCardXmlInfo).GetMethod("Copy", AccessTools.all), PatchType.postfix);*/
                //0本书显示为无限 79
                method = typeof(Harmony_Patch).GetMethod("UIInvitationDropBookSlot_SetData_DropBook_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIInvitationDropBookSlot).GetMethod("SetData_DropBook", AccessTools.all), PatchType.postfix);
                //XML加载音效修复 80
                method = typeof(Harmony_Patch).GetMethod("CharacterSound_LoadAudioCoroutine_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(CharacterSound).GetMethod("LoadAudioCoroutine", AccessTools.all), PatchType.prefix);
                //掉落书籍LorId 81
                method = typeof(Harmony_Patch).GetMethod("UnitDataModel_SetEnemyDropTable_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UnitDataModel).GetMethod("SetEnemyDropTable", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("UIInvitationStageInfoPanel_SetData_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UIInvitationStageInfoPanel).GetMethod("SetData", AccessTools.all), PatchType.postfix);
                //ignorepower修复 82
                method = typeof(Harmony_Patch).GetMethod("BattleDiceBehavior_UpdateDiceFinalValue_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BattleDiceBehavior).GetMethod("UpdateDiceFinalValue", AccessTools.all), PatchType.prefix);
                //setcosttozero优先级提高 83
                method = typeof(Harmony_Patch).GetMethod("BattleDiceCardModel_GetCost_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(BattleDiceCardModel).GetMethod("GetCost", AccessTools.all), PatchType.prefix);
                //specialrangeIcon 84
                method = typeof(Harmony_Patch).GetMethod("UISpriteDataManager_GetRangeIconSprite_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(UISpriteDataManager).GetMethod("GetRangeIconSprite", AccessTools.all), PatchType.prefix);
                //存储Mod存档 85
                method = typeof(Harmony_Patch).GetMethod("SaveManager_SavePlayData_Pre", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(SaveManager).GetMethod("SavePlayData", AccessTools.all), PatchType.prefix);
                method = typeof(Harmony_Patch).GetMethod("GameOpeningController_StopOpening_Post", AccessTools.all);
                Patching(harmony, new HarmonyMethod(method), typeof(GameOpeningController).GetMethod("StopOpening", AccessTools.all), PatchType.postfix);
                //加载Mod
                LoadModFiles();
                LoadAssemblyFiles();
                ModSettingTool.ModSaveTool.LoadFromSaveData();
                //私货扩展
                ExcuteBufFix();
                ExcuteCardScriptFix();
                ExcuteSummonLiberation();
            }
            catch (Exception ex)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                File.WriteAllText(Application.dataPath + "/Mods/error.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            /*
            try
            {
                if (GlobalGameManager.Instance.ver.StartsWith("1.1.0.6a9") && !File.Exists(Application.dataPath + "/Managed/AlreadyRepaira9.txt"))
                {
                    OrcTools.InjectForRepair();
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/InjectForRepairerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }*/
        }
        private static void LoadAssemblyFiles()
        {
            ModSettingTool.ModSaveTool.LoadedModsWorkshopId.Add("BaseMod");
            foreach (ModContent modContent in LoadedModContents)
            {
                DirectoryInfo _dirInfo = modContent.GetType().GetField("_dirInfo", AccessTools.all).GetValue(modContent) as DirectoryInfo;
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
                            Singleton<AssemblyManager>.Instance.GetType().GetMethod("LoadTypesFromAssembly", AccessTools.all).Invoke(Singleton<AssemblyManager>.Instance, new object[]
                            {
                                currentAssembly
                            });
                            ModSettingTool.ModSaveTool.LoadedModsWorkshopId.Add(Tools.GetModId(currentAssembly));
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
        private static void LoadModFiles()
        {
            Dictionary<string, List<Workshop.WorkshopSkinData>> _bookSkinData = Singleton<CustomizingBookSkinLoader>.Instance.GetType().GetField("_bookSkinData", AccessTools.all).GetValue(Singleton<CustomizingBookSkinLoader>.Instance) as Dictionary<string, List<Workshop.WorkshopSkinData>>;
            StaticDataLoader_New.ExportOriginalFiles();
            StaticDataLoader_New.LoadModFiles(LoadedModContents);
            LocalizedTextLoader_New.ExportOriginalFiles();
            LocalizedTextLoader_New.LoadModFiles(LoadedModContents);
            StorySerializer_new.ExportStory();
            LoadBookSkins(_bookSkinData);
        }
        private static void ReloadModFiles()
        {
            GlobalGameManager.Instance.GetType().GetMethod("LoadStaticData", AccessTools.all).Invoke(GlobalGameManager.Instance, null);
            Singleton<StageClassInfoList>.Instance.GetAllWorkshopData().Clear();
            Singleton<EnemyUnitClassInfoList>.Instance.GetAllWorkshopData().Clear();
            Singleton<BookXmlList>.Instance.GetAllWorkshopData().Clear();
            Singleton<CardDropTableXmlList>.Instance.GetAllWorkshopData().Clear();
            Singleton<DropBookXmlList>.Instance.GetAllWorkshopData().Clear();
            ItemXmlDataList.instance.GetAllWorkshopData().Clear();
            (Singleton<BookDescXmlList>.Instance.GetType().GetField("_dictionaryWorkshop", AccessTools.all).GetValue(Singleton<BookDescXmlList>.Instance) as Dictionary<string, List<BookDesc>>).Clear();
            (Singleton<CustomizingCardArtworkLoader>.Instance.GetType().GetField("_artworkData", AccessTools.all).GetValue(Singleton<CustomizingCardArtworkLoader>.Instance) as Dictionary<string, List<Workshop.ArtworkCustomizeData>>).Clear();
            (Singleton<CustomizingBookSkinLoader>.Instance.GetType().GetField("_bookSkinData", AccessTools.all).GetValue(Singleton<CustomizingBookSkinLoader>.Instance) as Dictionary<string, List<Workshop.WorkshopSkinData>>).Clear();
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
                DirectoryInfo _dirInfo = modContent.GetType().GetField("_dirInfo", AccessTools.all).GetValue(modContent) as DirectoryInfo;
                string _itemUniqueId = modContent.GetType().GetField("_itemUniqueId", AccessTools.all).GetValue(modContent) as string;
                string path = _dirInfo.FullName + "\\Char";
                string modid = "";
                if (!_itemUniqueId.ToLower().EndsWith("@origin"))
                {
                    modid = _itemUniqueId;
                }
                List<Workshop.WorkshopSkinData> list = new List<Workshop.WorkshopSkinData>();
                if (Directory.Exists(path))
                {
                    string[] directories = Directory.GetDirectories(path);
                    for (int i = 0; i < directories.Length; i++)
                    {
                        Workshop.WorkshopAppearanceInfo workshopAppearanceInfo = LoadCustomAppearance(directories[i]);
                        if (workshopAppearanceInfo != null)
                        {
                            string[] array = directories[i].Split(new char[]
                            {
                                '\\'
                            });
                            string bookName = array[array.Length - 1];
                            workshopAppearanceInfo.path = directories[i];
                            workshopAppearanceInfo.uniqueId = modid;
                            workshopAppearanceInfo.bookName = "Custom_" + bookName;
                            if (workshopAppearanceInfo.isClothCustom)
                            {
                                list.Add(new Workshop.WorkshopSkinData
                                {
                                    dic = workshopAppearanceInfo.clothCustomInfo,
                                    dataName = workshopAppearanceInfo.bookName,
                                    contentFolderIdx = workshopAppearanceInfo.uniqueId,
                                    id = i
                                });
                            }
                        }
                    }
                    if (_bookSkinData.ContainsKey(modid))
                    {
                        _bookSkinData[modid].AddRange(list);
                    }
                    else
                    {
                        _bookSkinData[modid] = list;
                    }
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
        private static Workshop.WorkshopAppearanceInfo LoadCustomAppearanceInfo(string rootPath, string xml)
        {
            Workshop.WorkshopAppearanceInfo workshopAppearanceInfo = new Workshop.WorkshopAppearanceInfo();
            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }
            StreamReader streamReader = new StreamReader(xml);
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.LoadXml(streamReader.ReadToEnd());
                XmlNode xmlNode = xmlDocument.SelectSingleNode("ModInfo");
                XmlNode xmlNode2 = xmlNode.SelectSingleNode("FaceInfo");
                XmlNode xmlNode3 = xmlNode.SelectSingleNode("ClothInfo");
                if (xmlNode2 != null)
                {
                    Dictionary<Workshop.FaceCustomType, Sprite> faceCustomInfo = new Dictionary<Workshop.FaceCustomType, Sprite>();
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
                                faceCustomInfo.Add(key, sprite);
                            }
                        }
                    }
                    LoadFaceCustom(faceCustomInfo);
                }
                if (xmlNode3 != null)
                {
                    workshopAppearanceInfo.isClothCustom = true;
                    string innerText2 = xmlNode3.SelectSingleNode("Name").InnerText;
                    if (!string.IsNullOrEmpty(innerText2))
                    {
                        workshopAppearanceInfo.bookName = innerText2;
                    }
                    for (int j = 0; j <= 11; j++)
                    {
                        ActionDetail actionDetail = (ActionDetail)j;
                        if (actionDetail != ActionDetail.Standing && actionDetail != ActionDetail.NONE)
                        {
                            string text = actionDetail.ToString();
                            try
                            {
                                XmlNode xmlNode5 = xmlNode3.SelectSingleNode(text);
                                if (xmlNode5 == null)
                                {
                                    Debug.Log("Workshop :: " + text + "keyword null!");
                                    text = "Penetrate";
                                    xmlNode5 = xmlNode3.SelectSingleNode(text);
                                }
                                if (xmlNode5 != null)
                                {
                                    string text2 = rootPath + "/ClothCustom/" + text + ".png";
                                    string text3 = rootPath + "/ClothCustom/" + text + "_front.png";
                                    XmlNode xmlNode6 = xmlNode5.SelectSingleNode("Pivot");
                                    XmlNode namedItem = xmlNode6.Attributes.GetNamedItem("pivot_x");
                                    XmlNode namedItem2 = xmlNode6.Attributes.GetNamedItem("pivot_y");
                                    XmlNode xmlNode7 = xmlNode5.SelectSingleNode("Head");
                                    XmlNode namedItem3 = xmlNode7.Attributes.GetNamedItem("head_x");
                                    XmlNode namedItem4 = xmlNode7.Attributes.GetNamedItem("head_y");
                                    XmlNode namedItem5 = xmlNode7.Attributes.GetNamedItem("rotation");
                                    XmlNode xmlNode8 = xmlNode5.SelectSingleNode("Direction");
                                    XmlNode namedItem6 = xmlNode7.Attributes.GetNamedItem("head_enable");
                                    float num = float.Parse(namedItem.InnerText);
                                    float num2 = float.Parse(namedItem2.InnerText);
                                    float num3 = float.Parse(namedItem3.InnerText);
                                    float num4 = float.Parse(namedItem4.InnerText);
                                    float headRotation = float.Parse(namedItem5.InnerText);
                                    bool headEnabled = true;
                                    if (namedItem6 != null)
                                    {
                                        bool.TryParse(namedItem6.InnerText, out headEnabled);
                                    }
                                    Vector2 pivotPos = new Vector2((num + 512f) / 1024f, (num2 + 512f) / 1024f);
                                    Vector2 headPos = new Vector2(num3 / 100f, num4 / 100f);
                                    bool hasFrontSprite = false;
                                    string text4 = text2;
                                    string frontSpritePath = text3;
                                    bool hasSpriteFile = false;
                                    bool hasFrontSpriteFile = false;
                                    if (File.Exists(text2))
                                    {
                                        hasSpriteFile = true;
                                    }
                                    if (File.Exists(text3))
                                    {
                                        hasFrontSprite = true;
                                        hasFrontSpriteFile = true;
                                    }
                                    CharacterMotion.MotionDirection direction = CharacterMotion.MotionDirection.FrontView;
                                    if (xmlNode8.InnerText == "Side")
                                    {
                                        direction = CharacterMotion.MotionDirection.SideView;
                                    }
                                    Workshop.ClothCustomizeData value = new Workshop.ClothCustomizeData
                                    {
                                        spritePath = text4,
                                        frontSpritePath = frontSpritePath,
                                        hasFrontSprite = hasFrontSprite,
                                        pivotPos = pivotPos,
                                        headPos = headPos,
                                        headRotation = headRotation,
                                        direction = direction,
                                        headEnabled = headEnabled,
                                        hasFrontSpriteFile = hasFrontSpriteFile,
                                        hasSpriteFile = hasSpriteFile
                                    };
                                    if (text4 != null)
                                    {
                                        workshopAppearanceInfo.clothCustomInfo.Add(actionDetail, value);
                                    }
                                }
                            }
                            catch (Exception message)
                            {
                                Debug.LogError(message);
                            }
                        }
                    }
                    for (int k = 12; k < 31; k++)
                    {
                        ActionDetail actionDetail2 = (ActionDetail)k;
                        if (actionDetail2 != ActionDetail.Standing && actionDetail2 != ActionDetail.NONE)
                        {
                            string text5 = actionDetail2.ToString();
                            try
                            {
                                XmlNode xmlNode10 = xmlNode3.SelectSingleNode(text5);
                                if (xmlNode10 != null)
                                {
                                    string text6 = rootPath + "/ClothCustom/" + text5 + ".png";
                                    string text7 = rootPath + "/ClothCustom/" + text5 + "_front.png";
                                    XmlNode xmlNode11 = xmlNode10.SelectSingleNode("Pivot");
                                    XmlNode namedItem7 = xmlNode11.Attributes.GetNamedItem("pivot_x");
                                    XmlNode namedItem8 = xmlNode11.Attributes.GetNamedItem("pivot_y");
                                    XmlNode xmlNode12 = xmlNode10.SelectSingleNode("Head");
                                    XmlNode namedItem9 = xmlNode12.Attributes.GetNamedItem("head_x");
                                    XmlNode namedItem10 = xmlNode12.Attributes.GetNamedItem("head_y");
                                    XmlNode namedItem11 = xmlNode12.Attributes.GetNamedItem("rotation");
                                    XmlNode xmlNode13 = xmlNode10.SelectSingleNode("Direction");
                                    XmlNode namedItem12 = xmlNode12.Attributes.GetNamedItem("head_enable");
                                    float num5 = float.Parse(namedItem7.InnerText);
                                    float num6 = float.Parse(namedItem8.InnerText);
                                    float num7 = float.Parse(namedItem9.InnerText);
                                    float num8 = float.Parse(namedItem10.InnerText);
                                    float headRotation2 = float.Parse(namedItem11.InnerText);
                                    bool headEnabled2 = true;
                                    if (namedItem12 != null)
                                    {
                                        bool.TryParse(namedItem12.InnerText, out headEnabled2);
                                    }
                                    Vector2 pivotPos2 = new Vector2((num5 + 512f) / 1024f, (num6 + 512f) / 1024f);
                                    Vector2 headPos2 = new Vector2(num7 / 100f, num8 / 100f);
                                    bool hasFrontSprite2 = false;
                                    string text8 = text6;
                                    string frontSpritePath2 = text7;
                                    bool hasSpriteFile2 = false;
                                    bool hasFrontSpriteFile2 = false;
                                    if (File.Exists(text6))
                                    {
                                        hasSpriteFile2 = true;
                                    }
                                    if (File.Exists(text7))
                                    {
                                        hasFrontSprite2 = true;
                                        hasFrontSpriteFile2 = true;
                                    }
                                    CharacterMotion.MotionDirection direction2 = CharacterMotion.MotionDirection.FrontView;
                                    if (xmlNode13.InnerText == "Side")
                                    {
                                        direction2 = CharacterMotion.MotionDirection.SideView;
                                    }
                                    Workshop.ClothCustomizeData value2 = new Workshop.ClothCustomizeData
                                    {
                                        spritePath = text8,
                                        frontSpritePath = frontSpritePath2,
                                        hasFrontSprite = hasFrontSprite2,
                                        pivotPos = pivotPos2,
                                        headPos = headPos2,
                                        headRotation = headRotation2,
                                        direction = direction2,
                                        headEnabled = headEnabled2,
                                        hasFrontSpriteFile = hasFrontSpriteFile2,
                                        hasSpriteFile = hasSpriteFile2
                                    };
                                    if (text8 != null)
                                    {
                                        workshopAppearanceInfo.clothCustomInfo.Add(actionDetail2, value2);
                                    }
                                }
                            }
                            catch (Exception message2)
                            {
                                Debug.LogError(message2);
                            }
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
                ((List<FaceResourceSet>)typeof(CustomizingResourceLoader).GetField("_eyeResources", AccessTools.all).GetValue(Singleton<CustomizingResourceLoader>.Instance)).Add(faceResourceSet);
            }
            if (faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_Brow_Attack) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_Brow_Hit) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_Brow_Normal) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Side_Brow))
            {
                ((List<FaceResourceSet>)typeof(CustomizingResourceLoader).GetField("_browResources", AccessTools.all).GetValue(Singleton<CustomizingResourceLoader>.Instance)).Add(faceResourceSet2);
            }
            if (faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_Mouth_Attack) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_Mouth_Hit) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_Mouth_Normal) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Side_Mouth))
            {
                ((List<FaceResourceSet>)typeof(CustomizingResourceLoader).GetField("_mouthResources", AccessTools.all).GetValue(Singleton<CustomizingResourceLoader>.Instance)).Add(faceResourceSet3);
            }
            if (faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_FrontHair) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Side_FrontHair))
            {
                ((List<HairResourceSet>)typeof(CustomizingResourceLoader).GetField("_frontHairResources", AccessTools.all).GetValue(Singleton<CustomizingResourceLoader>.Instance)).Add(hairResourceSet);
            }
            if (faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_RearHair) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Side_RearHair_Front) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Side_RearHair_Rear))
            {
                ((List<HairResourceSet>)typeof(CustomizingResourceLoader).GetField("_rearHairResources", AccessTools.all).GetValue(Singleton<CustomizingResourceLoader>.Instance)).Add(hairResourceSet2);
            }
        }
        private static void ExcuteBufFix()
        {
            Harmony harmony = new Harmony("BufFix_Mod");
            MethodInfo method = typeof(Harmony_Patch).GetMethod("BattleUnitBufListDetail_OnRoundStart_Pre", AccessTools.all);
            Patching(harmony, new HarmonyMethod(method), typeof(BattleUnitBufListDetail).GetMethod("OnRoundStart", AccessTools.all), PatchType.prefix);
            method = typeof(Harmony_Patch).GetMethod("BattleUnitBufListDetail_CanAddBuf_Pre", AccessTools.all);
            Patching(harmony, new HarmonyMethod(method), typeof(BattleUnitBufListDetail).GetMethod("CanAddBuf", AccessTools.all), PatchType.prefix);
        }
        private static bool BattleUnitBufListDetail_OnRoundStart_Pre(BattleUnitBufListDetail __instance, BattleUnitModel ____self, List<BattleUnitBuf> ____bufList, List<BattleUnitBuf> ____readyBufList, List<BattleUnitBuf> ____readyReadyBufList)
        {
            try
            {
                foreach (BattleUnitBuf ReadyBuf in ____readyBufList)
                {
                    if (!ReadyBuf.IsDestroyed())
                    {
                        BattleUnitBuf buf = ____bufList.Find((BattleUnitBuf x) => x.GetType() == ReadyBuf.GetType() && !x.IsDestroyed());
                        if (buf != null && !ReadyBuf.independentBufIcon && ((Sprite)_bufIcon.GetValue(ReadyBuf)) != null)
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
                        if (rbuf != null && !ReadyReadyBuf.independentBufIcon)
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
                typeof(BattleUnitBufListDetail).GetMethod("CheckAchievements", AccessTools.all).Invoke(__instance, null);
                return false;
            }
            catch (Exception ex2)
            {
                File.WriteAllText(Application.dataPath + "/Mods/ReadyBufFixerror.log", ex2.Message + Environment.NewLine + ex2.StackTrace);
            }
            return true;
        }
        private static void BattleUnitBufListDetail_CanAddBuf_Pre(BattleUnitBufListDetail __instance, BattleUnitModel ____self, BattleUnitBuf buf)
        {
            try
            {
                if (buf == null || ____self == null)
                {
                    return;
                }
                typeof(BattleUnitBuf).GetField("_owner", AccessTools.all).SetValue(buf, ____self);
            }
            catch { }
        }
        private static void ExcuteCardScriptFix()
        {
            Harmony harmony = new Harmony("CardScriptFix_Mod");
            MethodInfo method = typeof(Harmony_Patch).GetMethod("BattleDiceCardModel_CreateDiceCardSelfAbilityScript_Pre", AccessTools.all);
            Patching(harmony, new HarmonyMethod(method), typeof(BattleDiceCardModel).GetMethod("CreateDiceCardSelfAbilityScript", AccessTools.all), PatchType.prefix);
            method = typeof(Harmony_Patch).GetMethod("BattleDiceCardModel_CreateDiceCardSelfAbilityScript_Post", AccessTools.all);
            Patching(harmony, new HarmonyMethod(method), typeof(BattleDiceCardModel).GetMethod("CreateDiceCardSelfAbilityScript", AccessTools.all), PatchType.postfix);
            method = typeof(Harmony_Patch).GetMethod("BattlePersonalEgoCardDetail_AddCard_Pre", AccessTools.all);
            Patching(harmony, new HarmonyMethod(method), typeof(BattlePersonalEgoCardDetail).GetMethod("AddCard", AccessTools.all, null, new Type[]
                {
                    typeof(LorId)
                }, null), PatchType.prefix);
        }
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
        private static void ExcuteSummonLiberation()
        {
            Activator.CreateInstance(typeof(SummonLiberation.Harmony_Patch));
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
        private static bool SdCharacterUtil_CreateSkin_Pre(UnitDataModel unit, Faction faction, Transform characterRoot, ref CharacterAppearance __result)
        {
            try
            {
                if (!string.IsNullOrEmpty(unit.workshopSkin))
                {
                    return true;
                }
                Workshop.WorkshopSkinData skinData = Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData(unit.CustomBookItem.BookId.packageId, unit.CustomBookItem.GetOriginalCharcterName());
                if (skinData != null && unit.CustomBookItem.ClassInfo.skinType == "Custom")
                {
                    UnitCustomizingData customizeData = unit.customizeData;
                    GiftInventory giftInventory = unit.giftInventory;
                    GameObject gameObject = CreateCustomCharacter_new(skinData, out string resourceName, characterRoot);
                    CharacterAppearance characterAppearance = null;
                    Workshop.WorkshopSkinDataSetter component = gameObject.GetComponent<Workshop.WorkshopSkinDataSetter>();
                    component.SetData(skinData);
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
                    characterAppearance.GetType().GetField("_initialized", AccessTools.all).SetValue(characterAppearance, false);
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
        }
        public static GameObject CreateCustomCharacter_new(Workshop.WorkshopSkinData workshopSkinData, out string resourceName, Transform characterRotationCenter = null)
        {
            GameObject result = null;
            resourceName = "";
            try
            {
                if (workshopSkinData != null)
                {
                    GameObject gameObject = (GameObject)Resources.Load("Prefabs/Characters/[Prefab]Appearance_Custom");
                    if (characterRotationCenter != null)
                    {
                        result = UnityEngine.Object.Instantiate(gameObject, characterRotationCenter);
                    }
                    else
                    {
                        result = UnityEngine.Object.Instantiate(gameObject);
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
        private static void UIAbnormalityCardPreviewSlot_Init_Post(UIAbnormalityCardPreviewSlot __instance, EmotionCardXmlInfo card)
        {
            try
            {
                if (ArtWorks == null)
                {
                    GetArtWorks();
                }
                Image image = (Image)__instance.GetType().GetField("artwork", AccessTools.all).GetValue(__instance);
                if (ArtWorks.ContainsKey(card.Artwork))
                {
                    image.sprite = ArtWorks[card.Artwork];
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/AbnormalityCardPreviewerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        private static void BattleUnitDiceActionUI_EmotionCard_Init_Post(BattleUnitDiceActionUI_EmotionCard __instance, BattleEmotionCardModel card)
        {
            try
            {
                if (ArtWorks == null)
                {
                    GetArtWorks();
                }
                if (ArtWorks.ContainsKey(card.XmlInfo.Artwork))
                {
                    __instance.artwork.sprite = ArtWorks[card.XmlInfo.Artwork];
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/DiceActionEmotionCarderror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        private static void UIEmotionPassiveCardInven_SetSprites_Post(UIEmotionPassiveCardInven __instance)
        {
            try
            {
                if (ArtWorks == null)
                {
                    GetArtWorks();
                }
                EmotionCardXmlInfo emotionCardXmlInfo = (EmotionCardXmlInfo)__instance.GetType().GetField("_card", AccessTools.all).GetValue(__instance);
                Image image = (Image)__instance.GetType().GetField("_artwork", AccessTools.all).GetValue(__instance);
                if (ArtWorks.ContainsKey(emotionCardXmlInfo.Artwork))
                {
                    image.sprite = ArtWorks[emotionCardXmlInfo.Artwork];
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/EmotionCardInveSnerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        private static void EmotionPassiveCardUI_SetSprites_Post(EmotionPassiveCardUI __instance)
        {
            try
            {
                if (ArtWorks == null)
                {
                    GetArtWorks();
                }
                EmotionCardXmlInfo emotionCardXmlInfo = (EmotionCardXmlInfo)__instance.GetType().GetField("_card", AccessTools.all).GetValue(__instance);
                Image image = (Image)__instance.GetType().GetField("_artwork", AccessTools.all).GetValue(__instance);
                if (ArtWorks.ContainsKey(emotionCardXmlInfo.Artwork))
                {
                    image.sprite = ArtWorks[emotionCardXmlInfo.Artwork];
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/EmotionCardSerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        private static bool BookUseEffect_get_bookIcon_Pre(DropBookXmlInfo __instance, ref Sprite __result)
        {
            try
            {
                if (ArtWorks == null)
                {
                    GetArtWorks();
                }
                if (ArtWorks.TryGetValue(__instance.BookIcon, out Sprite sprite))
                {
                    __result = sprite;
                    return false;
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/BookUseIconerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
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
        private static void GetArtWorks()
        {
            ArtWorks = new Dictionary<string, Sprite>();
            foreach (ModContent modContent in LoadedModContents)
            {
                DirectoryInfo directoryInfo = modContent.GetType().GetField("_dirInfo", AccessTools.all).GetValue(modContent) as DirectoryInfo;
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
        //防止被动过多炸UI，存在字体问题待解决
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
        private static bool BattleEmotionCardModel_ctor_Pre(BattleEmotionCardModel __instance, EmotionCardXmlInfo xmlInfo, BattleUnitModel owner, ref EmotionCardXmlInfo ____xmlInfo, ref BattleUnitModel ____owner, ref List<EmotionCardAbilityBase> ____abilityList)
        {
            try
            {
                ____xmlInfo = xmlInfo;
                ____owner = owner;
                ____abilityList = new List<EmotionCardAbilityBase>();
                try
                {
                    foreach (string text in xmlInfo.Script)
                    {
                        EmotionCardAbilityBase emotionCardAbilityBase = FindEmotionCardAbility(text.Trim());
                        emotionCardAbilityBase.SetEmotionCard(__instance);
                        ____abilityList.Add(emotionCardAbilityBase);
                    }
                }
                catch (Exception message)
                {
                    Debug.LogError(message);
                }
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/SetEmotionAbilityerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
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
        //更多波次的接待
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
        //书架扩容
        private static bool UIOriginEquipPageList_UpdateEquipPageList_Pre(UIOriginEquipPageList __instance)
        {
            try
            {
                List<UIOriginEquipPageSlot> list = (List<UIOriginEquipPageSlot>)__instance.GetType().GetField("equipPageSlotList", AccessTools.all).GetValue(__instance);
                List<BookModel> list2 = (List<BookModel>)__instance.GetType().GetField("currentScreenBookModelList", AccessTools.all).GetValue(__instance);
                int i;
                for (i = 0; i < list2.Count; i++)
                {
                    if (i == list.Count)
                    {
                        UIOriginEquipPageSlot uioriginEquipPageSlot = UtilTools.DuplicateEquipPageSlot(list[0], __instance);
                        uioriginEquipPageSlot.Initialized();
                        list.Add(uioriginEquipPageSlot);
                    }
                    list[i].SetActiveSlot(true);
                    list[i].SetData(list2[i]);
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
        //书库剧情
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
                            TextMeshProUGUI episodeText = (TextMeshProUGUI)___EpisodeSlots[j].GetType().GetField("episodeText", AccessTools.all).GetValue(___EpisodeSlots[j]);
                            episodeText.text = stagename;
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
        //自定义Map
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
                                if (array[0].ToLower() == "custom" && CustomMapManager.TryGetValue(text.Substring("custom_".Length).Trim() + "MapManager", out Type mapmanager))
                                {
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
        private static bool BattleSceneRoot_ChangeToEgoMap_Pre(string mapName)
        {
            try
            {
                MapChangeFilter mapChangeFilter = (MapChangeFilter)typeof(BattleSceneRoot).GetField("_mapChangeFilter", AccessTools.all).GetValue(SingletonBehavior<BattleSceneRoot>.Instance);
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
        private static void Mod_Update()
        {/*
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
            }*/
        }
        private static void ModContentManager_SetActiveContents_Pre(List<ModContent> ____loadedContents)
        {
            ____loadedContents.Clear();
        }
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
        }
        private static bool BookModel_SetXmlInfo_Pre(BookModel __instance, BookXmlInfo classInfo, ref BookXmlInfo ____classInfo, ref List<DiceCardXmlInfo> ____onlyCards, ref string ____selectedOriginalSkin, ref string ____characterSkin)
        {
            try
            {
                if (classInfo == null)
                {
                    Debug.LogError("BookXmlInfo is null");
                    return false;
                }
                if (!OrcTools.OnlyCardDic.ContainsKey(classInfo.id))
                {
                    return true;
                }
                ____classInfo = classInfo;
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
                __instance.SetOriginalResists();
                __instance.SetOriginalSpeed();
                __instance.SetOriginalSpeedNum();
                __instance.SetOriginalPlayPoint();
                __instance.SetOriginalStat();
                if (classInfo.CharacterSkin.Count > 0)
                {
                    ____selectedOriginalSkin = RandomUtil.SelectOne(classInfo.CharacterSkin);
                }
                else
                {
                    ____selectedOriginalSkin = "";
                }
                ____characterSkin = ____selectedOriginalSkin;
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/SetXmlInfoerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
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
                    __result = File.Exists(Path.Combine(Singleton<ModContentManager>.Instance.GetModPath(stageStoryInfo.packageId), "Data", "StoryText", stageStoryInfo.story)) || File.Exists(Path.Combine(Singleton<ModContentManager>.Instance.GetModPath(stageStoryInfo.packageId), "Data", "StoryText", TextDataModel.CurrentLanguage, stageStoryInfo.story));
                    return false;
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/HasEffectFileerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
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
                    string storyPath = Path.Combine(modPath, "Data", "StoryText", stageStoryInfo.story);
                    string effectPath = Path.Combine(modPath, "Data", "StoryEffect", stageStoryInfo.story);
                    string[] array = stageStoryInfo.story.Split(new char[]
                    {
                         '.'
                    });
                    if (File.Exists(Path.Combine(modPath, "Data", "StoryEffect", array[0] + ".xml")))
                    {
                        effectPath = Path.Combine(modPath, "Data", "StoryEffect", array[0] + ".xml");
                    }
                    if (File.Exists(Path.Combine(modPath, "Data", "StoryEffect", array[0] + ".txt")))
                    {
                        effectPath = Path.Combine(modPath, "Data", "StoryEffect", array[0] + ".txt");
                    }
                    if (File.Exists(Path.Combine(modPath, "Data", "StoryText", TextDataModel.CurrentLanguage, array[0] + ".xml")))
                    {
                        storyPath = Path.Combine(modPath, "Data", "StoryText", TextDataModel.CurrentLanguage, array[0] + ".xml");
                    }
                    if (File.Exists(Path.Combine(modPath, "Data", "StoryText", TextDataModel.CurrentLanguage, array[0] + ".txt")))
                    {
                        storyPath = Path.Combine(modPath, "Data", "StoryText", TextDataModel.CurrentLanguage, array[0] + ".txt");
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
        private static void UIInvitationRightMainPanel_SetCustomInvToggle_Post(ref Toggle ____workshopInvitationToggle)
        {
            if (IsModStorySelected)
            {
                ____workshopInvitationToggle.isOn = true;
            }
        }
        private static void TextDataModel_InitTextData_Post(string currentLanguage)
        {
            try
            {
                Language = currentLanguage;
                LocalizedTextLoader_New.ExportOriginalFiles();
                List<ModContent> _loadedContents = Singleton<ModContentManager>.Instance.GetType().GetField("_loadedContents", AccessTools.all).GetValue(Singleton<ModContentManager>.Instance) as List<ModContent>;
                LocalizedTextLoader_New.LoadModFiles(_loadedContents);
            }
            catch (Exception ex)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                File.WriteAllText(Application.dataPath + "/Mods/InitTextDataerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
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
                ___currentBookModelList = (List<BookModel>)__instance.GetType().GetMethod("FilterBookModels", AccessTools.all).Invoke(__instance, new object[]
                {
                    ___currentBookModelList
                });
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
                __instance.GetType().GetMethod("SetScrollBar", AccessTools.all).Invoke(__instance, null);
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
                ___currentBookModelList = (List<BookModel>)__instance.GetType().GetMethod("FilterBookModels", AccessTools.all).Invoke(__instance, new object[]
                {
                    ___currentBookModelList
                });
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
                __instance.GetType().GetMethod("SetScrollBar", AccessTools.all).Invoke(__instance, null);
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
                List<BookModel> list = new List<BookModel>((List<BookModel>)typeof(UISettingInvenEquipPageListSlot).GetMethod("ApplyFilterBooksInStory", AccessTools.all).Invoke(__instance, new object[]
                    {
                        books
                    }));
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
                List<BookModel> list = new List<BookModel>((List<BookModel>)typeof(UIInvenEquipPageListSlot).GetMethod("ApplyFilterBooksInStory", AccessTools.all).Invoke(__instance, new object[]
                    {
                        books
                    }));
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
        private static bool BookModel_GetThumbSprite_Pre(BookModel __instance, ref Sprite __result)
        {
            try
            {

                Workshop.WorkshopSkinData workshopBookSkinData = Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData(__instance.BookId.packageId, __instance.ClassInfo.GetCharacterSkin());
                if (workshopBookSkinData != null)
                {
                    string path = workshopBookSkinData.dic[ActionDetail.Default].spritePath;
                    DirectoryInfo dir = new DirectoryInfo(path);
                    path = dir.Parent.Parent.FullName + "/Thumb.png";
                    Sprite result = GetBookThumb(__instance.BookId, path);
                    if (result != null)
                    {
                        __result = result;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/Book_Thumberror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }
        private static bool BookXmlInfo_GetThumbSprite_Pre(BookXmlInfo __instance, ref Sprite __result)
        {
            try
            {

                Workshop.WorkshopSkinData workshopBookSkinData = Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData(__instance.id.packageId, __instance.GetCharacterSkin());
                if (workshopBookSkinData != null)
                {
                    string path = workshopBookSkinData.dic[ActionDetail.Default].spritePath;
                    DirectoryInfo dir = new DirectoryInfo(path);
                    path = dir.Parent.Parent.FullName + "/Thumb.png";
                    Sprite result = GetBookThumb(__instance.id, path);
                    if (result != null)
                    {
                        __result = result;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/BookXml_Thumberror.log", ex.Message + Environment.NewLine + ex.StackTrace);
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
                        Dictionary<GiftPosition, GiftAppearance> dictionary = (Dictionary<GiftPosition, GiftAppearance>)__instance.GetType().GetField("_giftAppearanceDic", AccessTools.all).GetValue(__instance);
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
                            SpriteRenderer spriteRenderer = (SpriteRenderer)giftAppearance.GetType().GetField("_frontSpriteRenderer", AccessTools.all).GetValue(giftAppearance);
                            SpriteRenderer spriteRenderer2 = (SpriteRenderer)giftAppearance.GetType().GetField("_sideSpriteRenderer", AccessTools.all).GetValue(giftAppearance);
                            SpriteRenderer spriteRenderer3 = (SpriteRenderer)giftAppearance.GetType().GetField("_frontBackSpriteRenderer", AccessTools.all).GetValue(giftAppearance);
                            SpriteRenderer spriteRenderer4 = (SpriteRenderer)giftAppearance.GetType().GetField("_sideBackSpriteRenderer", AccessTools.all).GetValue(giftAppearance);
                            spriteRenderer.gameObject.transform.localScale = new Vector2(1f, 1f);
                            if (CustomGiftAppearance.GiftArtWork.ContainsKey(array2[2] + "_front"))
                            {
                                spriteRenderer.sprite = CustomGiftAppearance.GiftArtWork[array2[2] + "_front"];
                            }
                            else
                            {
                                spriteRenderer.gameObject.SetActive(false);
                                giftAppearance.GetType().GetField("_frontSpriteRenderer", AccessTools.all).SetValue(giftAppearance, null);
                            }
                            spriteRenderer2.gameObject.transform.localScale = new Vector2(1f, 1f);
                            if (CustomGiftAppearance.GiftArtWork.ContainsKey(array2[2] + "_side"))
                            {
                                spriteRenderer2.sprite = CustomGiftAppearance.GiftArtWork[array2[2] + "_side"];
                            }
                            else
                            {
                                spriteRenderer2.gameObject.SetActive(false);
                                giftAppearance.GetType().GetField("_sideSpriteRenderer", AccessTools.all).SetValue(giftAppearance, null);
                            }
                            spriteRenderer3.gameObject.transform.localScale = new Vector2(1f, 1f);
                            if (CustomGiftAppearance.GiftArtWork.ContainsKey(array2[2] + "_frontBack"))
                            {
                                spriteRenderer3.sprite = CustomGiftAppearance.GiftArtWork[array2[2] + "_frontBack"];
                            }
                            else
                            {
                                spriteRenderer3.gameObject.SetActive(false);
                                giftAppearance.GetType().GetField("_frontBackSpriteRenderer", AccessTools.all).SetValue(giftAppearance, null);
                            }
                            spriteRenderer4.gameObject.transform.localScale = new Vector2(1f, 1f);
                            if (CustomGiftAppearance.GiftArtWork.ContainsKey(array2[2] + "_sideBack"))
                            {
                                spriteRenderer4.sprite = CustomGiftAppearance.GiftArtWork[array2[2] + "_sideBack"];
                            }
                            else
                            {
                                spriteRenderer4.gameObject.SetActive(false);
                                giftAppearance.GetType().GetField("_sideBackSpriteRenderer", AccessTools.all).SetValue(giftAppearance, null);
                            }
                            dictionary.Add(gift.ClassInfo.Position, giftAppearance);
                        }
                        if (giftAppearance != null)
                        {
                            string layer = (string)__instance.GetType().GetField("_layerName", AccessTools.all).GetValue(__instance);
                            CharacterMotion motion = (CharacterMotion)__instance.GetType().GetField("_currentMotion", AccessTools.all).GetValue(__instance);
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
                __instance.GetType().GetMethod("SetEyeButton", AccessTools.all).Invoke(__instance, new object[]
                {
                    ___isEyeOpen
                });
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
        private static bool BattleUnitView_ChangeEgoSkin_Pre(BattleUnitView __instance, ref BattleUnitView.SkinInfo ____skinInfo, string egoName, bool bookNameChange = true)
        {
            try
            {
                Workshop.WorkshopSkinData skinData = Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData(__instance.model.UnitData.unitData.bookItem.ClassInfo.workshopID, egoName);
                if (skinData == null)
                {
                    return true;
                }
                ____skinInfo.state = BattleUnitView.SkinState.EGO;
                ____skinInfo.skinName = egoName;
                UnitCustomizingData customizeData = __instance.model.UnitData.unitData.customizeData;
                GiftInventory giftInventory = __instance.model.UnitData.unitData.giftInventory;
                ActionDetail currentMotionDetail = __instance.charAppearance.GetCurrentMotionDetail();
                __instance.DestroySkin();
                GameObject gameObject = CreateCustomCharacter_new(skinData, out string resourceName, __instance.characterRotationCenter);
                gameObject.GetComponent<Workshop.WorkshopSkinDataSetter>().SetData(skinData);
                List<CustomInvitation.BookSoundInfo> motionSoundList = __instance.model.UnitData.unitData.bookItem.ClassInfo.motionSoundList;
                if (motionSoundList != null && motionSoundList.Count > 0)
                {
                    string motionSoundPath = ModUtil.GetMotionSoundPath(Singleton<ModContentManager>.Instance.GetModPath(__instance.model.UnitData.unitData.bookItem.ClassInfo.id.packageId));
                    CharacterSound component2 = gameObject.GetComponent<CharacterSound>();
                    if (component2 != null)
                    {
                        component2.SetMotionSounds(motionSoundList, motionSoundPath);
                    }
                }
                __instance.charAppearance = gameObject.GetComponent<CharacterAppearance>();
                __instance.charAppearance.GetType().GetField("_initialized", AccessTools.all).SetValue(__instance.charAppearance, false);
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
        private static bool BattleUnitView_ChangeSkin_Pre(BattleUnitView __instance, ref BattleUnitView.SkinInfo ____skinInfo, string charName)
        {
            try
            {
                Workshop.WorkshopSkinData skinData = Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData(__instance.model.UnitData.unitData.bookItem.ClassInfo.workshopID, charName);
                if (skinData == null)
                {
                    return true;
                }
                ____skinInfo.state = BattleUnitView.SkinState.Changed;
                ____skinInfo.skinName = charName;
                UnitCustomizingData customizeData = __instance.model.UnitData.unitData.customizeData;
                GiftInventory giftInventory = __instance.model.UnitData.unitData.giftInventory;
                ActionDetail currentMotionDetail = __instance.charAppearance.GetCurrentMotionDetail();
                __instance.DestroySkin();
                GameObject gameObject = CreateCustomCharacter_new(skinData, out string resourceName, __instance.characterRotationCenter);
                gameObject.GetComponent<Workshop.WorkshopSkinDataSetter>().SetData(skinData);
                List<CustomInvitation.BookSoundInfo> motionSoundList = __instance.model.UnitData.unitData.bookItem.ClassInfo.motionSoundList;
                if (motionSoundList != null && motionSoundList.Count > 0)
                {
                    string motionSoundPath = ModUtil.GetMotionSoundPath(Singleton<ModContentManager>.Instance.GetModPath(__instance.model.UnitData.unitData.bookItem.ClassInfo.id.packageId));
                    CharacterSound component2 = gameObject.GetComponent<CharacterSound>();
                    if (component2 != null)
                    {
                        component2.SetMotionSounds(motionSoundList, motionSoundPath);
                    }
                }
                __instance.charAppearance = gameObject.GetComponent<CharacterAppearance>();
                __instance.charAppearance.GetType().GetField("_initialized", AccessTools.all).SetValue(__instance.charAppearance, false);
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
        }
        private static bool VersionViewer_Start_Pre(VersionViewer __instance)
        {
            __instance.GetComponent<Text>().text = GlobalGameManager.Instance.ver;
            __instance.GetComponent<Text>().fontSize = 30;
            __instance.gameObject.transform.localPosition = new Vector3(-830f, -460f);
            return false;
        }
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
        private static void StageController_GameOver_Post(StageModel ____stageModel)
        {
            try
            {
                if (ModStoryCG.ContainsKey(____stageModel.ClassInfo.id))
                {
                    ModSettingTool.ModSaveTool.SaveString("ModLastStroyCG", ModStoryCG[____stageModel.ClassInfo.id].path, "BaseMod");
                }
                else if (TryAddModStoryCG(____stageModel.ClassInfo))
                {
                    ModSettingTool.ModSaveTool.SaveString("ModLastStroyCG", ModStoryCG[____stageModel.ClassInfo.id].path, "BaseMod");
                }
                else
                {

                    ModSettingTool.ModSaveTool.SaveString("ModLastStroyCG", "", "BaseMod");
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
            if (string.IsNullOrEmpty(modPath)|| string.IsNullOrEmpty(stageClassInfo.GetStartStory().story))
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
        private static void EntryScene_SetCG_Post(EntryScene __instance, ref LatestDataModel ____latestData)
        {
            try
            {
                string stroycg = ModSettingTool.ModSaveTool.GetModSaveData("BaseMod").GetString("ModLastStroyCG");
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
        private static void UIInvitationDropBookList_ApplyFilterAll_Post(UIInvitationDropBookList __instance, List<LorId> ____currentBookIdList)
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
                __instance.SelectablePanel.ChildSelectable = __instance.BookSlotList[0].selectable;
                __instance.UpdateBookListPage(false);
            }
            catch (Exception ex)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                File.WriteAllText(Application.dataPath + "/Mods/ModBookSort_Invi.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
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
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                File.WriteAllText(Application.dataPath + "/Mods/ModBookSort_Feed.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
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
                int num = (int)typeof(UIInvenCardListScroll).GetMethod("GetMaxRow", AccessTools.all).Invoke(__instance, new object[0]);
                ___scrollBar.SetScrollRectSize(___column * ___slotWidth, (num + (float)___row - 1f) * ___slotHeight);
                ___scrollBar.SetWindowPosition(0f, 0f);
                ___selectablePanel.ChildSelectable = ___slotList[0].selectable;
                __instance.SetCardsData(__instance.GetCurrentPageList());
                return false;
            }
            catch (Exception ex)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
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
                int num = (int)typeof(UIEquipPageCustomizePanel).GetMethod("GetMaxRow", AccessTools.all).Invoke(__instance, new object[0]);
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
                typeof(UIEquipPageCustomizePanel).GetMethod("UpdatePageButtons", AccessTools.all).Invoke(__instance, new object[0]);
                return false;
            }
            catch (Exception ex)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                File.WriteAllText(Application.dataPath + "/Mods/UIEquipPageCustomizePanel_UpdateEquipPageSlotList.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
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
            string keywordIconId = string.Empty;
            try
            {
                keywordIconId = (string)typeof(BattleUnitBuf).GetMethod("get_keywordIconId", AccessTools.all).Invoke(__instance, null);
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/GetIconIderror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            if (!string.IsNullOrEmpty(keywordIconId) && !BattleUnitBuf._bufIconDictionary.ContainsKey(keywordIconId) && ArtWorks.TryGetValue(keywordIconId, out Sprite sprite))
            {
                BattleUnitBuf._bufIconDictionary[keywordIconId] = sprite;
            }
        }
        private static bool BattleDiceCardBuf_GetBufIcon_Pre(BattleDiceCardBuf __instance, ref Sprite __result, ref bool ____iconInit, ref Sprite ____bufIcon)
        {
            if (ArtWorks == null)
            {
                GetArtWorks();
            }
            if (!____iconInit)
            {
                string keywordIconId;
                try
                {
                    keywordIconId = (string)typeof(BattleDiceCardBuf).GetMethod("get_keywordIconId", AccessTools.all).Invoke(__instance, null);
                }
                catch (Exception ex)
                {
                    File.WriteAllText(Application.dataPath + "/Mods/GetIconIderror.log", ex.Message + Environment.NewLine + ex.StackTrace);
                    return true;
                }
                try
                {
                    if (!string.IsNullOrEmpty(keywordIconId) && ArtWorks.TryGetValue(keywordIconId, out Sprite sprite) && sprite != null)
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
        /*
        private static void DiceCardXmlInfo_Copy_Post(DiceCardXmlInfo __instance, DiceCardXmlInfo __result)
        {
            __result.Keywords = __instance.Keywords;
        }*/
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
        private static void UIInvitationStageInfoPanel_SetData_Post(StageClassInfo stage, UIRewardDropBookList ___rewardBookList, UIStoryLine story = UIStoryLine.None)
        {
            CanvasGroup cg = (CanvasGroup)___rewardBookList.GetType().GetField("cg", AccessTools.all).GetValue(___rewardBookList);
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
        private static void BattleDiceBehavior_UpdateDiceFinalValue_Pre(ref int ____diceFinalResultValue, int ____diceResultValue)
        {
            try
            {
                ____diceFinalResultValue = ____diceResultValue;
            }
            catch { }
        }
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
        private static void GameOpeningController_StopOpening_Post()
        {
            try
            {
                ModSettingTool.ModSaveTool.RemoveUnknownSaves();
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/LoadFromModSaveDataerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        private static void SaveManager_SavePlayData_Pre()
        {
            try
            {
                ModSettingTool.ModSaveTool.SaveModSaveData();
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/SaveFailed.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        public static void DeepCopyGameObject(Transform original, Transform copyed)
        {
            copyed.localPosition = original.localPosition;
            copyed.localRotation = original.localRotation;
            copyed.localScale = original.localScale;
            copyed.gameObject.layer = original.gameObject.layer;
            for (int i = 0; i < copyed.childCount; i++)
            {
                DeepCopyGameObject(original.GetChild(i), copyed.GetChild(i));
            }
        }
        public static IEnumerator RenderCam_2(int index, UICharacterRenderer renderer)
        {
            yield return YieldCache.waitFrame;
            renderer.cameraList[index].targetTexture.Release();
            renderer.cameraList[index].Render();
            yield break;
        }
        private static string GetCardName(LorId cardID)
        {
            if (_CardDescdictionary == null)
            {
                _CardDescdictionary = Singleton<BattleCardDescXmlList>.Instance.GetType().GetField("_dictionary", AccessTools.all);
            }
            string result = "Not Found";
            if (cardID == 705010)
            {
                cardID = new LorId(707001);
            }
            if ((_CardDescdictionary.GetValue(Singleton<BattleCardDescXmlList>.Instance) as Dictionary<LorId, BattleCardDesc>).ContainsKey(cardID))
            {
                result = (_CardDescdictionary.GetValue(Singleton<BattleCardDescXmlList>.Instance) as Dictionary<LorId, BattleCardDesc>)[cardID].cardName;
            }
            return result;
        }
        public static void Patching(Harmony harmony, HarmonyMethod method, MethodBase original, PatchType type)
        {
            try
            {
                if (type == PatchType.prefix)
                {
                    harmony.Patch(original, method, null, null, null, null);
                }
                else
                {
                    harmony.Patch(original, null, method, null, null, null);
                }
                patchNum++;
            }
            catch (Exception ex)
            {
                if (method != null)
                {
                    File.WriteAllText(Application.dataPath + "/Mods/Base_" + patchNum.ToString() + ".log", ex.Message + Environment.NewLine + ex.StackTrace);
                }
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

        private static int patchNum = 1;

        private static string path = string.Empty;

        public static string Language;

        private static string Staticpath;

        private static string StoryStaticpath;

        private static string Storylocalizepath;

        private static string Localizepath;

        public static FieldInfo _bufIcon;

        private static List<Assembly> AssemList;

        private static List<string> LoadedAssembly;

        public static Dictionary<string, Type> CustomEffects = new Dictionary<string, Type>();

        public static Dictionary<string, Type> CustomMapManager = new Dictionary<string, Type>();

        public static Dictionary<string, Type> CustomBattleDialogModel = new Dictionary<string, Type>();

        public static Dictionary<string, Type> CustomGiftPassive = new Dictionary<string, Type>();

        public static Dictionary<string, Type> CustomEmotionCardAbility = new Dictionary<string, Type>();

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

        private static FieldInfo _CardDescdictionary;

    }
}

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
            ((List<BattleDiceCardModel>)cardDetail.GetType().GetField("_cardInReserved", AccessTools.all).GetValue(cardDetail)).Remove(card);
            ((List<BattleDiceCardModel>)cardDetail.GetType().GetField("_cardInUse", AccessTools.all).GetValue(cardDetail)).Remove(card);
            ((List<BattleDiceCardModel>)cardDetail.GetType().GetField("_cardInHand", AccessTools.all).GetValue(cardDetail)).Remove(card);
            ((List<BattleDiceCardModel>)cardDetail.GetType().GetField("_cardInDiscarded", AccessTools.all).GetValue(cardDetail)).Remove(card);
            ((List<BattleDiceCardModel>)cardDetail.GetType().GetField("_cardInDeck", AccessTools.all).GetValue(cardDetail)).Remove(card);
        }
        public static BattleDiceCardModel DrawCardSpecified(this BattleAllyCardDetail cardDetail, Predicate<BattleDiceCardModel> match)
        {
            BattleDiceCardModel result;
            if (cardDetail.GetHand().Count >= (int)cardDetail.GetType().GetField("_maxDrawHand", AccessTools.all).GetValue(cardDetail))
            {
                result = null;
            }
            else
            {
                try
                {
                    List<BattleDiceCardModel> list = (List<BattleDiceCardModel>)cardDetail.GetType().GetField("_cardInDeck", AccessTools.all).GetValue(cardDetail);
                    List<BattleDiceCardModel> list2 = (List<BattleDiceCardModel>)cardDetail.GetType().GetField("_cardInDiscarded", AccessTools.all).GetValue(cardDetail);
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
            cardModel.GetType().GetField("_iconAdder", AccessTools.all).SetValue(cardModel, resName);
            Sprite spr = Harmony_Patch.ArtWorks[resName];
            List<BattleDiceCardModel.CardIcon> list = (List<BattleDiceCardModel.CardIcon>)cardModel.GetType().GetField("_addedIcons", AccessTools.all).GetValue(cardModel);
            bool flag = list.Find((BattleDiceCardModel.CardIcon x) => x.Icon == spr) == null;
            if (flag)
            {
                BattleDiceCardModel.CardIcon item = new BattleDiceCardModel.CardIcon(spr, priority);
                list.Add(item);
                list.Sort((BattleDiceCardModel.CardIcon x, BattleDiceCardModel.CardIcon y) => y.Priority - x.Priority);
            }
        }
        public static void ChangeToModSkin(this BattleUnitView unitView, string workshopId, string charName, BattleUnitView.SkinState skinState = BattleUnitView.SkinState.Changed)
        {
            try
            {
                Workshop.WorkshopSkinData skinData = Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData(workshopId, charName);
                if (skinData == null)
                {
                    return;
                }
                BattleUnitView.SkinInfo _skinInfo = unitView.GetType().GetField("_skinInfo", AccessTools.all).GetValue(unitView) as BattleUnitView.SkinInfo;
                _skinInfo.state = skinState;
                _skinInfo.skinName = charName;
                UnitCustomizingData customizeData = unitView.model.UnitData.unitData.customizeData;
                GiftInventory giftInventory = unitView.model.UnitData.unitData.giftInventory;
                ActionDetail currentMotionDetail = unitView.charAppearance.GetCurrentMotionDetail();
                unitView.DestroySkin();
                GameObject gameObject = Harmony_Patch.CreateCustomCharacter_new(skinData, out string resourceName, unitView.characterRotationCenter);
                gameObject.GetComponent<Workshop.WorkshopSkinDataSetter>().SetData(skinData);
                List<CustomInvitation.BookSoundInfo> motionSoundList = unitView.model.UnitData.unitData.bookItem.ClassInfo.motionSoundList;
                if (motionSoundList != null && motionSoundList.Count > 0)
                {
                    string motionSoundPath = ModUtil.GetMotionSoundPath(Singleton<ModContentManager>.Instance.GetModPath(unitView.model.UnitData.unitData.bookItem.ClassInfo.id.packageId));
                    CharacterSound component2 = gameObject.GetComponent<CharacterSound>();
                    if (component2 != null)
                    {
                        component2.SetMotionSounds(motionSoundList, motionSoundPath);
                    }
                }
                unitView.charAppearance = gameObject.GetComponent<CharacterAppearance>();
                unitView.charAppearance.GetType().GetField("_initialized", AccessTools.all).SetValue(unitView.charAppearance, false);
                unitView.charAppearance.Initialize(resourceName);
                unitView.charAppearance.InitCustomData(customizeData, unitView.model.UnitData.unitData.defaultBook.GetBookClassInfoId());
                unitView.charAppearance.InitGiftDataAll(giftInventory.GetEquippedList());
                unitView.charAppearance.ChangeMotion(currentMotionDetail);
                unitView.charAppearance.ChangeLayer("Character");
                unitView.charAppearance.SetLibrarianOnlySprites(unitView.model.faction);
                if (customizeData != null)
                {
                    unitView.ChangeHeight(customizeData.height);
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/ChangeToModSkinerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        public static void SetAlarmText(string alarmtype, UIAlarmButtonType btnType = UIAlarmButtonType.Default, ConfirmEvent confirmFunc = null, params object[] args)
        {
            if (UIAlarmPopup.instance.IsOpened())
            {
                UIAlarmPopup.instance.Close();
            }
            try
            {
                typeof(UIAlarmPopup).GetField("currentAnimState", AccessTools.all).SetValue(UIAlarmPopup.instance, 0);
            }
            catch { Debug.LogError("set currentAnimState error"); }
            GameObject ob_blue = (GameObject)typeof(UIAlarmPopup).GetField("ob_blue", AccessTools.all).GetValue(UIAlarmPopup.instance);
            GameObject ob_normal = (GameObject)typeof(UIAlarmPopup).GetField("ob_normal", AccessTools.all).GetValue(UIAlarmPopup.instance);
            GameObject ob_Reward = (GameObject)typeof(UIAlarmPopup).GetField("ob_Reward", AccessTools.all).GetValue(UIAlarmPopup.instance);
            GameObject ob_BlackBg = (GameObject)typeof(UIAlarmPopup).GetField("ob_BlackBg", AccessTools.all).GetValue(UIAlarmPopup.instance);
            List<GameObject> ButtonRoots = (List<GameObject>)typeof(UIAlarmPopup).GetField("ButtonRoots", AccessTools.all).GetValue(UIAlarmPopup.instance);
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
            typeof(UIAlarmPopup).GetField("currentAlarmType", AccessTools.all).SetValue(UIAlarmPopup.instance, UIAlarmType.Default);
            typeof(UIAlarmPopup).GetField("buttonNumberType", AccessTools.all).SetValue(UIAlarmPopup.instance, btnType);
            typeof(UIAlarmPopup).GetField("currentmode", AccessTools.all).SetValue(UIAlarmPopup.instance, AnimatorUpdateMode.Normal);
            ((Animator)typeof(UIAlarmPopup).GetField("anim", AccessTools.all).GetValue(UIAlarmPopup.instance)).updateMode = AnimatorUpdateMode.Normal;
            TextMeshProUGUI txt_alarm = (TextMeshProUGUI)typeof(UIAlarmPopup).GetField("txt_alarm", AccessTools.all).GetValue(UIAlarmPopup.instance);
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
                UIControlManager.Instance.SelectSelectableForcely((UICustomSelectable)typeof(UIAlarmPopup).GetField("yesButton", AccessTools.all).GetValue(UIAlarmPopup.instance), false);
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

namespace BaseMod
{
    public enum PatchType
    {
        prefix,
        postfix
    }
}

namespace BaseMod
{
    public class CustomGiftAppearance : GiftAppearance
    {
        public static GiftAppearance CreateCustomGift(string[] array2)
        {
            bool flag = CreatedGifts.ContainsKey(array2[2]);
            GiftAppearance result;
            if (flag)
            {
                result = CreatedGifts[array2[2]];
            }
            else
            {
                GameObject original = Resources.Load<GameObject>("Prefabs/Gifts/Gifts_NeedRename/Gift_Challenger");
                GiftAppearance component = Instantiate(original).GetComponent<GiftAppearance>();
                SpriteRenderer spriteRenderer = (SpriteRenderer)component.GetType().GetField("_frontSpriteRenderer", AccessTools.all).GetValue(component);
                SpriteRenderer spriteRenderer2 = (SpriteRenderer)component.GetType().GetField("_sideSpriteRenderer", AccessTools.all).GetValue(component);
                SpriteRenderer spriteRenderer3 = (SpriteRenderer)component.GetType().GetField("_frontBackSpriteRenderer", AccessTools.all).GetValue(component);
                SpriteRenderer spriteRenderer4 = (SpriteRenderer)component.GetType().GetField("_sideBackSpriteRenderer", AccessTools.all).GetValue(component);
                spriteRenderer.gameObject.transform.localScale = new Vector2(1f, 1f);
                bool flag2 = GiftArtWork.ContainsKey(array2[2] + "_front");
                bool flag3 = flag2;
                if (flag3)
                {
                    spriteRenderer.sprite = GiftArtWork[array2[2] + "_front"];
                }
                else
                {
                    spriteRenderer.gameObject.SetActive(false);
                    component.GetType().GetField("_frontSpriteRenderer", AccessTools.all).SetValue(component, null);
                }
                spriteRenderer2.gameObject.transform.localScale = new Vector2(1f, 1f);
                bool flag4 = GiftArtWork.ContainsKey(array2[2] + "_side");
                bool flag5 = flag4;
                if (flag5)
                {
                    spriteRenderer2.sprite = GiftArtWork[array2[2] + "_side"];
                }
                else
                {
                    spriteRenderer2.gameObject.SetActive(false);
                    component.GetType().GetField("_sideSpriteRenderer", AccessTools.all).SetValue(component, null);
                }
                spriteRenderer3.gameObject.transform.localScale = new Vector2(1f, 1f);
                bool flag6 = GiftArtWork.ContainsKey(array2[2] + "_frontBack");
                bool flag7 = flag6;
                if (flag7)
                {
                    spriteRenderer3.sprite = GiftArtWork[array2[2] + "_frontBack"];
                }
                else
                {
                    spriteRenderer3.gameObject.SetActive(false);
                    component.GetType().GetField("_frontBackSpriteRenderer", AccessTools.all).SetValue(component, null);
                }
                spriteRenderer4.gameObject.transform.localScale = new Vector2(1f, 1f);
                bool flag8 = GiftArtWork.ContainsKey(array2[2] + "_sideBack");
                bool flag9 = flag8;
                if (flag9)
                {
                    spriteRenderer4.sprite = GiftArtWork[array2[2] + "_sideBack"];
                }
                else
                {
                    spriteRenderer4.gameObject.SetActive(false);
                    component.GetType().GetField("_sideBackSpriteRenderer", AccessTools.all).SetValue(component, null);
                }
                CreatedGifts[array2[2]] = component;
                result = component;
            }
            return result;
        }
        public static void GetGiftArtWork()
        {
            GiftArtWork = new Dictionary<string, Sprite>();
            foreach (ModContent modContent in Harmony_Patch.LoadedModContents)
            {
                DirectoryInfo _dirInfo = modContent.GetType().GetField("_dirInfo", AccessTools.all).GetValue(modContent) as DirectoryInfo;
                if (Directory.Exists(_dirInfo.FullName + "/GiftArtWork"))
                {
                    DirectoryInfo directoryInfo2 = new DirectoryInfo(_dirInfo.FullName + "/GiftArtWork");
                    foreach (FileInfo fileInfo in directoryInfo2.GetFiles())
                    {
                        Texture2D texture2D = new Texture2D(2, 2);
                        texture2D.LoadImage(File.ReadAllBytes(fileInfo.FullName));
                        Sprite value = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                        GiftArtWork[fileNameWithoutExtension] = value;
                    }
                }
            }
        }
        public static Sprite GetGiftArtWork(string name)
        {
            bool flag = GiftArtWork.ContainsKey(name);
            Sprite result;
            if (flag)
            {
                result = GiftArtWork[name];
            }
            else
            {
                result = null;
            }
            return result;
        }
        public void Awake()
        {
            bool flag = inited;
            if (!flag)
            {
                bool flag2 = _frontSpriteRenderer == null;
                if (flag2)
                {
                    GameObject gameObject = new GameObject("new");
                    gameObject.transform.SetParent(base.gameObject.transform);
                    gameObject.transform.localPosition = new Vector2(0f, 0f);
                    gameObject.transform.localScale = new Vector2(1f, 1f);
                    _frontSpriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                }
                bool flag3 = _frontBackSpriteRenderer == null;
                if (flag3)
                {
                    GameObject gameObject2 = new GameObject("new");
                    gameObject2.transform.SetParent(gameObject.transform);
                    gameObject2.transform.localPosition = new Vector2(0f, 0f);
                    gameObject2.transform.localScale = new Vector2(1f, 1f);
                    _frontBackSpriteRenderer = gameObject2.AddComponent<SpriteRenderer>();
                }
                bool flag4 = _sideSpriteRenderer == null;
                if (flag4)
                {
                    GameObject gameObject3 = new GameObject("new");
                    gameObject3.transform.SetParent(gameObject.transform);
                    gameObject3.transform.localPosition = new Vector2(0f, 0f);
                    gameObject3.transform.localScale = new Vector2(1f, 1f);
                    _sideSpriteRenderer = gameObject3.AddComponent<SpriteRenderer>();
                }
                bool flag5 = _sideBackSpriteRenderer == null;
                if (flag5)
                {
                    GameObject gameObject4 = new GameObject("new");
                    gameObject4.transform.SetParent(gameObject.transform);
                    gameObject4.transform.localPosition = new Vector2(0f, 0f);
                    gameObject4.transform.localScale = new Vector2(1f, 1f);
                    _sideBackSpriteRenderer = gameObject4.AddComponent<SpriteRenderer>();
                }
            }
        }
        public void CustomInit(string name)
        {
            File.WriteAllText(Application.dataPath + "/BaseMods/" + name, "");
            Sprite giftArtWork = GetGiftArtWork(name + "_front");
            Sprite giftArtWork2 = GetGiftArtWork(name + "_frontBack");
            Sprite giftArtWork3 = GetGiftArtWork(name + "_side");
            Sprite giftArtWork4 = GetGiftArtWork(name + "_sideBack");
            bool flag = giftArtWork != null;
            if (flag)
            {
                _frontSpriteRenderer.sprite = giftArtWork;
            }
            else
            {
                _frontSpriteRenderer = null;
            }
            bool flag2 = giftArtWork2 != null;
            if (flag2)
            {
                _frontSpriteRenderer.sprite = giftArtWork2;
            }
            else
            {
                _frontBackSpriteRenderer = null;
            }
            bool flag3 = giftArtWork3 != null;
            if (flag3)
            {
                _sideSpriteRenderer.sprite = giftArtWork3;
            }
            else
            {
                _sideSpriteRenderer = null;
            }
            bool flag4 = giftArtWork4 != null;
            if (flag4)
            {
                _sideBackSpriteRenderer.sprite = giftArtWork4;
            }
            else
            {
                _sideBackSpriteRenderer = null;
            }
        }
        public CustomGiftAppearance()
        {
            inited = false;
        }
        static CustomGiftAppearance()
        {
            CreatedGifts = new Dictionary<string, GiftAppearance>();
        }

        public static Dictionary<string, GiftAppearance> CreatedGifts;

        public static Dictionary<string, Sprite> GiftArtWork;

        public bool inited;
    }
}

namespace BaseMod
{
    public class CustomMapManager : CreatureMapManager
    {
        public virtual void CustomInit()
        {
        }
    }
}

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
            Dictionary<string, LOR_XML.BattleDialogRoot> BattleDialogues = Singleton<BattleDialogXmlList>.Instance.GetType().GetField("_dictionary", AccessTools.all).GetValue(Singleton<BattleDialogXmlList>.Instance) as Dictionary<string, LOR_XML.BattleDialogRoot>;
            List<BattleDialogRelationWithBookID> BattleDialoguesRelations = Singleton<BattleDialogXmlList>.Instance.GetType().GetField("_relationList", AccessTools.all).GetValue(Singleton<BattleDialogXmlList>.Instance) as List<BattleDialogRelationWithBookID>;
            Dictionary<int, string> CharactersName = Singleton<CharactersNameXmlList>.Instance.GetType().GetField("_dictionary", AccessTools.all).GetValue(Singleton<CharactersNameXmlList>.Instance) as Dictionary<int, string>;
            Dictionary<int, string> LibrariansName = Singleton<LibrariansNameXmlList>.Instance.GetType().GetField("_dictionary", AccessTools.all).GetValue(Singleton<LibrariansNameXmlList>.Instance) as Dictionary<int, string>;
            Dictionary<int, string> StageName = Singleton<StageNameXmlList>.Instance.GetType().GetField("_dictionary", AccessTools.all).GetValue(Singleton<StageNameXmlList>.Instance) as Dictionary<int, string>;
            Dictionary<LorId, PassiveDesc> PassiveDesc = Singleton<PassiveDescXmlList>.Instance.GetType().GetField("_dictionary", AccessTools.all).GetValue(Singleton<PassiveDescXmlList>.Instance) as Dictionary<LorId, PassiveDesc>;
            Dictionary<string, GiftText> GiftDesc = Singleton<GiftDescXmlList>.Instance.GetType().GetField("_dictionary", AccessTools.all).GetValue(Singleton<GiftDescXmlList>.Instance) as Dictionary<string, GiftText>;
            Dictionary<LorId, BattleCardDesc> BattleCardDesc = Singleton<BattleCardDescXmlList>.Instance.GetType().GetField("_dictionary", AccessTools.all).GetValue(Singleton<BattleCardDescXmlList>.Instance) as Dictionary<LorId, BattleCardDesc>;
            Dictionary<string, BattleCardAbilityDesc> BattleCardAbilityDesc = Singleton<BattleCardAbilityDescXmlList>.Instance.GetType().GetField("_dictionary", AccessTools.all).GetValue(Singleton<BattleCardAbilityDescXmlList>.Instance) as Dictionary<string, BattleCardAbilityDesc>;
            Dictionary<string, BattleEffectText> BattleEffectTexts = Singleton<BattleEffectTextsXmlList>.Instance.GetType().GetField("_dictionary", AccessTools.all).GetValue(Singleton<BattleEffectTextsXmlList>.Instance) as Dictionary<string, BattleEffectText>;
            Dictionary<string, AbnormalityCard> AbnormalityCardDesc = Singleton<AbnormalityCardDescXmlList>.Instance.GetType().GetField("_dictionary", AccessTools.all).GetValue(Singleton<AbnormalityCardDescXmlList>.Instance) as Dictionary<string, AbnormalityCard>;
            Dictionary<string, AbnormalityAbilityText> AbnormalityAbilityText = Singleton<AbnormalityAbilityTextXmlList>.Instance.GetType().GetField("_dictionary", AccessTools.all).GetValue(Singleton<AbnormalityAbilityTextXmlList>.Instance) as Dictionary<string, AbnormalityAbilityText>;
            Dictionary<string, List<BookDesc>> BookDesc = (Singleton<BookDescXmlList>.Instance.GetType().GetField("_dictionaryWorkshop", AccessTools.all).GetValue(Singleton<BookDescXmlList>.Instance) as Dictionary<string, List<BookDesc>>);
            Dictionary<int, BookDesc> BookDescOrigin = (Singleton<BookDescXmlList>.Instance.GetType().GetField("_dictionaryOrigin", AccessTools.all).GetValue(Singleton<BookDescXmlList>.Instance) as Dictionary<int, BookDesc>);

            foreach (ModContent modcontent in loadedContents)
            {
                DirectoryInfo _dirInfo = modcontent.GetType().GetField("_dirInfo", AccessTools.all).GetValue(modcontent) as DirectoryInfo;
                ModContentInfo _modInfo = modcontent.GetType().GetField("_modInfo", AccessTools.all).GetValue(modcontent) as ModContentInfo;
                string workshopId = _modInfo.invInfo.workshopInfo.uniqueId;
                if (workshopId.ToLower().EndsWith("@origin"))
                {
                    workshopId = "";
                }
                try
                {
                    LoadLocalizeFile_MOD(_dirInfo);
                    LoadBattleDialogues_MOD(_dirInfo, BattleDialogues, workshopId);
                    LoadBattleDialogues_Relations_MOD(_dirInfo, BattleDialoguesRelations, BattleDialogues, workshopId);
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

namespace BaseMod
{
    public class StaticDataLoader_New
    {
        private static string GetModdingPath(DirectoryInfo dir, string type)
        {
            return dir.FullName + "/StaticInfo/" + type;
        }
        public static void StaticDataExport(string path, string outpath, string outname)
        {
            string text = Resources.Load<TextAsset>(path).text;
            Directory.CreateDirectory(outpath);
            File.WriteAllText(outpath + "/" + outname + ".txt", text);
        }
        public static void StaticDataExport_str(string str, string outpath, string outname)
        {
            Directory.CreateDirectory(outpath);
            File.WriteAllText(outpath + "/" + outname + ".txt", str);
        }
        public static void ExportOriginalFiles()
        {
            try
            {
                ExportPassive();
                ExportCard();
                ExportDeck();
                ExportBook();
                ExportCardDropTable();
                ExportDropBook();
                ExportGift();
                ExportEmotionCard();
                ExportEmotionEgo();
                ExportToolTip();
                ExportTitle();
                ExportFormation();
                ExportQuest();
                ExportEnemyUnit();
                ExportStage();
                ExportFloorInfo();
                ExportFinalRewardInfo();
                ExportCreditInfo();
                ExportResourceInfo();
                ExportAttackEffectInfo();
            }
            catch (Exception ex)
            {
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                File.WriteAllText(Application.dataPath + "/Mods/ESDerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        public static void ExportPassive()
        {
            StaticDataExport("Xml/PassiveList", Harmony_Patch.StaticPath + "/PassiveList", "PassiveList");
            StaticDataExport("Xml/PassiveList_Creature", Harmony_Patch.StaticPath + "/PassiveList", "PassiveList_Creature");
            StaticDataExport("Xml/PassiveList_ch7_Philip", Harmony_Patch.StaticPath + "/PassiveList", "PassiveList_ch7_Philip");
            StaticDataExport("Xml/PassiveList_ch7_Eileen", Harmony_Patch.StaticPath + "/PassiveList", "PassiveList_ch7_Eileen");
            StaticDataExport("Xml/PassiveList_ch7_Greta", Harmony_Patch.StaticPath + "/PassiveList", "PassiveList_ch7_Greta");
            StaticDataExport("Xml/PassiveList_ch7_Bremen", Harmony_Patch.StaticPath + "/PassiveList", "PassiveList_ch7_Bremen");
            StaticDataExport("Xml/PassiveList_ch7_Oswald", Harmony_Patch.StaticPath + "/PassiveList", "PassiveList_ch7_Oswald");
            StaticDataExport("Xml/PassiveList_ch7_Jaeheon", Harmony_Patch.StaticPath + "/PassiveList", "PassiveList_ch7_Jaeheon");
            StaticDataExport("Xml/PassiveList_ch7_Elena", Harmony_Patch.StaticPath + "/PassiveList", "PassiveList_ch7_Elena");
            StaticDataExport("Xml/PassiveList_ch7_Pluto", Harmony_Patch.StaticPath + "/PassiveList", "PassiveList_ch7_Pluto");
        }
        private static void ExportCard()
        {
            StaticDataExport("Xml/Card/CardInfo_Basic", Harmony_Patch.StaticPath + "/Card", "CardInfo_Basic");
            StaticDataExport("Xml/Card/CardInfo_ch1", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch1");
            StaticDataExport("Xml/Card/CardInfo_ch2", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch2");
            StaticDataExport("Xml/Card/CardInfo_ch3", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch3");
            StaticDataExport("Xml/Card/CardInfo_ch4", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch4");
            StaticDataExport("Xml/Card/CardInfo_ch5", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch5");
            StaticDataExport("Xml/Card/CardInfo_ch5_2", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch5_2");
            StaticDataExport("Xml/Card/CardInfo_ch6", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch6");
            StaticDataExport("Xml/Card/CardInfo_ch6_2", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch6_2");
            StaticDataExport("Xml/Card/CardInfo_ch6_3", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch6_3");
            StaticDataExport("Xml/Card/CardInfo_ch7", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch7");
            StaticDataExport("Xml/Card/CardInfo_special", Harmony_Patch.StaticPath + "/Card", "CardInfo_special");
            StaticDataExport("Xml/Card/CardInfo_ego", Harmony_Patch.StaticPath + "/Card", "CardInfo_ego");
            StaticDataExport("Xml/Card/CardInfo_ego_whitenight", Harmony_Patch.StaticPath + "/Card", "CardInfo_ego_whitenight");
            StaticDataExport("Xml/Card/CardInfo_creature", Harmony_Patch.StaticPath + "/Card", "CardInfo_creature");
            StaticDataExport("Xml/Card/CardInfo_creature_final", Harmony_Patch.StaticPath + "/Card", "CardInfo_creature_final");
            StaticDataExport("Xml/Card/CardInfo_creature_final_hod", Harmony_Patch.StaticPath + "/Card", "CardInfo_creature_final_hod");
            StaticDataExport("Xml/Card/CardInfo_creature_final_netzach", Harmony_Patch.StaticPath + "/Card", "CardInfo_creature_final_netzach");
            StaticDataExport("Xml/Card/CardInfo_creature_binah", Harmony_Patch.StaticPath + "/Card", "CardInfo_creature_binah");
            StaticDataExport("Xml/Card/CardInfo_creature_hokma", Harmony_Patch.StaticPath + "/Card", "CardInfo_creature_hokma");
            StaticDataExport("Xml/Card/CardInfo_creature_gebura", Harmony_Patch.StaticPath + "/Card", "CardInfo_creature_gebura");
            StaticDataExport("Xml/Card/CardInfo_creature_final_tiphereth", Harmony_Patch.StaticPath + "/Card", "CardInfo_creature_final_tiphereth");
            StaticDataExport("Xml/Card/CardInfo_creature_final_gebura", Harmony_Patch.StaticPath + "/Card", "CardInfo_creature_final_gebura");
            StaticDataExport("Xml/Card/CardInfo_creature_final_chesed", Harmony_Patch.StaticPath + "/Card", "CardInfo_creature_final_chesed");
            StaticDataExport("Xml/Card/CardInfo_creature_chesed", Harmony_Patch.StaticPath + "/Card", "CardInfo_creature_chesed");
            StaticDataExport("Xml/Card/CardInfo_creature_final_binah", Harmony_Patch.StaticPath + "/Card", "CardInfo_creature_final_binah");
            StaticDataExport("Xml/Card/CardInfo_creature_final_hokma", Harmony_Patch.StaticPath + "/Card", "CardInfo_creature_final_hokma");
            StaticDataExport("Xml/Card/CardInfo_creature_final_keter", Harmony_Patch.StaticPath + "/Card", "CardInfo_creature_final_keter");
            StaticDataExport("Xml/Card/CardInfo_ch7_Philip", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch7_Philip");
            StaticDataExport("Xml/Card/CardInfo_ch7_Eileen", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch7_Eileen");
            StaticDataExport("Xml/Card/CardInfo_ch7_Greta", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch7_Greta");
            StaticDataExport("Xml/Card/CardInfo_ch7_Bremen", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch7_Bremen");
            StaticDataExport("Xml/Card/CardInfo_ch7_Oswald", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch7_Oswald");
            StaticDataExport("Xml/Card/CardInfo_ch7_Jaeheon", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch7_Jaeheon");
            StaticDataExport("Xml/Card/CardInfo_ch7_Elena", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch7_Elena");
            StaticDataExport("Xml/Card/CardInfo_ch7_Pluto", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch7_Pluto");
            StaticDataExport("Xml/Card/CardInfo_ch7_Argalia", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch7_Argalia");
            StaticDataExport("Xml/Card/CardInfo_ch7_Roland2Phase", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch7_Roland2Phase");
            StaticDataExport("Xml/Card/CardInfo_ch7_BlackSilence3Phase", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch7_BlackSilence3Phase");
            StaticDataExport("Xml/Card/CardInfo_ch7_Roland4Phase", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch7_Roland4Phase");
            StaticDataExport("Xml/Card/CardInfo_ch7_FinalBand", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch7_FinalBand");
            StaticDataExport("Xml/Card/CardInfo_ch7_FinalBand_Middle", Harmony_Patch.StaticPath + "/Card", "CardInfo_ch7_FinalBand_Middle");
            StaticDataExport("Xml/Card/CardInfo_final", Harmony_Patch.StaticPath + "/Card", "CardInfo_final");
        }
        private static void ExportDeck()
        {
            StaticDataExport("Xml/Card/Deck_basic", Harmony_Patch.StaticPath + "/Deck", "Deck_basic");
            StaticDataExport("Xml/Card/Deck_ch1", Harmony_Patch.StaticPath + "/Deck", "Deck_ch1");
            StaticDataExport("Xml/Card/Deck_ch2", Harmony_Patch.StaticPath + "/Deck", "Deck_ch2");
            StaticDataExport("Xml/Card/Deck_ch3", Harmony_Patch.StaticPath + "/Deck", "Deck_ch3");
            StaticDataExport("Xml/Card/Deck_ch4", Harmony_Patch.StaticPath + "/Deck", "Deck_ch4");
            StaticDataExport("Xml/Card/Deck_ch5", Harmony_Patch.StaticPath + "/Deck", "Deck_ch5");
            StaticDataExport("Xml/Card/Deck_enemy_ch1", Harmony_Patch.StaticPath + "/Deck", "Deck_enemy_ch1");
            StaticDataExport("Xml/Card/Deck_enemy_ch2", Harmony_Patch.StaticPath + "/Deck", "Deck_enemy_ch2");
            StaticDataExport("Xml/Card/Deck_enemy_ch3", Harmony_Patch.StaticPath + "/Deck", "Deck_enemy_ch3");
            StaticDataExport("Xml/Card/Deck_enemy_ch4", Harmony_Patch.StaticPath + "/Deck", "Deck_enemy_ch4");
            StaticDataExport("Xml/Card/Deck_enemy_ch5", Harmony_Patch.StaticPath + "/Deck", "Deck_enemy_ch5");
            StaticDataExport("Xml/Card/Deck_enemy_ch5_2", Harmony_Patch.StaticPath + "/Deck", "Deck_enemy_ch5_2");
            StaticDataExport("Xml/Card/Deck_enemy_ch6", Harmony_Patch.StaticPath + "/Deck", "Deck_enemy_ch6");
            StaticDataExport("Xml/Card/Deck_enemy_ch7", Harmony_Patch.StaticPath + "/Deck", "Deck_enemy_ch7");
            StaticDataExport("Xml/Card/Deck_creature", Harmony_Patch.StaticPath + "/Deck", "Deck_creature");
            StaticDataExport("Xml/Card/Deck_creature_final", Harmony_Patch.StaticPath + "/Deck", "Deck_creature_final");
            StaticDataExport("Xml/Card/Deck_creature_hokma", Harmony_Patch.StaticPath + "/Deck", "Deck_creature_hokma");
            StaticDataExport("Xml/Card/Deck_enemy_ch7_Philip", Harmony_Patch.StaticPath + "/Deck", "Deck_enemy_ch7_Philip");
            StaticDataExport("Xml/Card/Deck_enemy_ch7_Eileen", Harmony_Patch.StaticPath + "/Deck", "Deck_enemy_ch7_Eileen");
            StaticDataExport("Xml/Card/Deck_enemy_ch7_Greta", Harmony_Patch.StaticPath + "/Deck", "Deck_enemy_ch7_Greta");
            StaticDataExport("Xml/Card/Deck_enemy_ch7_Bremen", Harmony_Patch.StaticPath + "/Deck", "Deck_enemy_ch7_Bremen");
            StaticDataExport("Xml/Card/Deck_enemy_ch7_Oswald", Harmony_Patch.StaticPath + "/Deck", "Deck_enemy_ch7_Oswald");
            StaticDataExport("Xml/Card/Deck_enemy_ch7_Jaeheon", Harmony_Patch.StaticPath + "/Deck", "Deck_enemy_ch7_Jaeheon");
            StaticDataExport("Xml/Card/Deck_enemy_ch7_Elena", Harmony_Patch.StaticPath + "/Deck", "Deck_enemy_ch7_Elena");
            StaticDataExport("Xml/Card/Deck_enemy_ch7_Pluto", Harmony_Patch.StaticPath + "/Deck", "Deck_enemy_ch7_Pluto");
        }
        public static void ExportBook()
        {
            StaticDataExport("Xml/EquipPage_basic", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_basic");
            StaticDataExport("Xml/EquipPage_ch1", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_ch1");
            StaticDataExport("Xml/EquipPage_ch2", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_ch2");
            StaticDataExport("Xml/EquipPage_ch3", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_ch3");
            StaticDataExport("Xml/EquipPage_ch4", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_ch4");
            StaticDataExport("Xml/EquipPage_ch5", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_ch5");
            StaticDataExport("Xml/EquipPage_ch6", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_ch6");
            StaticDataExport("Xml/EquipPage_ch7", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_ch7");
            StaticDataExport("Xml/EquipPage_enemy_ch1", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_enemy_ch1");
            StaticDataExport("Xml/EquipPage_enemy_ch2", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_enemy_ch2");
            StaticDataExport("Xml/EquipPage_enemy_ch3", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_enemy_ch3");
            StaticDataExport("Xml/EquipPage_enemy_ch4", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_enemy_ch4");
            StaticDataExport("Xml/EquipPage_enemy_ch5", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_enemy_ch5");
            StaticDataExport("Xml/EquipPage_enemy_ch5_2", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_enemy_ch5_2");
            StaticDataExport("Xml/EquipPage_enemy_ch6", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_enemy_ch6");
            StaticDataExport("Xml/EquipPage_enemy_ch6_2", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_enemy_ch6_2");
            StaticDataExport("Xml/EquipPage_enemy_ch6_R", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_enemy_ch6_R");
            StaticDataExport("Xml/EquipPage_enemy_ch7", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_enemy_ch7");
            StaticDataExport("Xml/EquipPage_creature", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_creature");
            StaticDataExport("Xml/EquipPage_creature_hokma", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_creature_hokma");
            StaticDataExport("Xml/EquipPage_creature_final", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_creature_final");
            StaticDataExport("Xml/EquipPage_creature_final_hod", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_creature_final_hod");
            StaticDataExport("Xml/EquipPage_creature_final_netzach", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_creature_final_netzach");
            StaticDataExport("Xml/EquipPage_creature_gebura", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_creature_gebura");
            StaticDataExport("Xml/EquipPage_creature_final_tiphereth", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_creature_final_tiphereth");
            StaticDataExport("Xml/EquipPage_creature_final_gebura", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_creature_final_gebura");
            StaticDataExport("Xml/EquipPage_creature_final_chesed", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_creature_final_chesed");
            StaticDataExport("Xml/EquipPage_creature_final_binah", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_creature_final_binah");
            StaticDataExport("Xml/EquipPage_creature_final_hokma", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_creature_final_hokma");
            StaticDataExport("Xml/EquipPage_creature_final_keter", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_creature_final_keter");
            StaticDataExport("Xml/EquipPage_enemy_ch7_Philip", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_enemy_ch7_Philip");
            StaticDataExport("Xml/EquipPage_enemy_ch7_Eileen", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_enemy_ch7_Eileen");
            StaticDataExport("Xml/EquipPage_enemy_ch7_Greta", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_enemy_ch7_Greta");
            StaticDataExport("Xml/EquipPage_enemy_ch7_Bremen", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_enemy_ch7_Bremen");
            StaticDataExport("Xml/EquipPage_enemy_ch7_Oswald", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_enemy_ch7_Oswald");
            StaticDataExport("Xml/EquipPage_enemy_ch7_Jaeheon", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_enemy_ch7_Jaeheon");
            StaticDataExport("Xml/EquipPage_enemy_ch7_Elena", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_enemy_ch7_Elena");
            StaticDataExport("Xml/EquipPage_enemy_ch7_Pluto", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_enemy_ch7_Pluto");
            StaticDataExport("Xml/EquipPage_enemy_ch7_BandFinal", Harmony_Patch.StaticPath + "/EquipPage", "EquipPage_enemy_ch7_BandFinal");
        }
        public static void ExportCardDropTable()
        {
            StaticDataExport("Xml/CardDropTable_ch1", Harmony_Patch.StaticPath + "/CardDropTable", "CardDropTable_ch1");
            StaticDataExport("Xml/CardDropTable_ch2", Harmony_Patch.StaticPath + "/CardDropTable", "CardDropTable_ch2");
            StaticDataExport("Xml/CardDropTable_ch3", Harmony_Patch.StaticPath + "/CardDropTable", "CardDropTable_ch3");
            StaticDataExport("Xml/CardDropTable_ch4", Harmony_Patch.StaticPath + "/CardDropTable", "CardDropTable_ch4");
            StaticDataExport("Xml/CardDropTable_ch5", Harmony_Patch.StaticPath + "/CardDropTable", "CardDropTable_ch5");
            StaticDataExport("Xml/CardDropTable_ch6", Harmony_Patch.StaticPath + "/CardDropTable", "CardDropTable_ch6");
            StaticDataExport("Xml/CardDropTable_ch7", Harmony_Patch.StaticPath + "/CardDropTable", "CardDropTable_ch7");
        }
        public static void ExportDropBook()
        {
            StaticDataExport("Xml/DropBook_ch1", Harmony_Patch.StaticPath + "/DropBook", "DropBook_ch1");
            StaticDataExport("Xml/DropBook_ch2", Harmony_Patch.StaticPath + "/DropBook", "DropBook_ch2");
            StaticDataExport("Xml/DropBook_ch3", Harmony_Patch.StaticPath + "/DropBook", "DropBook_ch3");
            StaticDataExport("Xml/DropBook_ch4", Harmony_Patch.StaticPath + "/DropBook", "DropBook_ch4");
            StaticDataExport("Xml/DropBook_ch5", Harmony_Patch.StaticPath + "/DropBook", "DropBook_ch5");
            StaticDataExport("Xml/DropBook_ch6", Harmony_Patch.StaticPath + "/DropBook", "DropBook_ch6");
            StaticDataExport("Xml/DropBook_ch7", Harmony_Patch.StaticPath + "/DropBook", "DropBook_ch7");
        }
        public static void ExportGift()
        {
            StaticDataExport("Xml/GiftInfo", Harmony_Patch.StaticPath + "/GiftInfo", "GiftInfo");
        }
        public static void ExportEmotionCard()
        {
            StaticDataExport("Xml/Card/EmotionCard/EmotionCard_keter", Harmony_Patch.StaticPath + "/EmotionCard", "EmotionCard_keter");
            StaticDataExport("Xml/Card/EmotionCard/EmotionCard_malkuth", Harmony_Patch.StaticPath + "/EmotionCard", "EmotionCard_malkuth");
            StaticDataExport("Xml/Card/EmotionCard/EmotionCard_yesod", Harmony_Patch.StaticPath + "/EmotionCard", "EmotionCard_yesod");
            StaticDataExport("Xml/Card/EmotionCard/EmotionCard_hod", Harmony_Patch.StaticPath + "/EmotionCard", "EmotionCard_hod");
            StaticDataExport("Xml/Card/EmotionCard/EmotionCard_netzach", Harmony_Patch.StaticPath + "/EmotionCard", "EmotionCard_netzach");
            StaticDataExport("Xml/Card/EmotionCard/EmotionCard_tiphereth", Harmony_Patch.StaticPath + "/EmotionCard", "EmotionCard_tiphereth");
            StaticDataExport("Xml/Card/EmotionCard/EmotionCard_geburah", Harmony_Patch.StaticPath + "/EmotionCard", "EmotionCard_geburah");
            StaticDataExport("Xml/Card/EmotionCard/EmotionCard_chesed", Harmony_Patch.StaticPath + "/EmotionCard", "EmotionCard_chesed");
            StaticDataExport("Xml/Card/EmotionCard/EmotionCard_enemy", Harmony_Patch.StaticPath + "/EmotionCard", "EmotionCard_enemy");
            StaticDataExport("Xml/Card/EmotionCard/EmotionCard_hokma", Harmony_Patch.StaticPath + "/EmotionCard", "EmotionCard_hokma");
            StaticDataExport("Xml/Card/EmotionCard/EmotionCard_binah", Harmony_Patch.StaticPath + "/EmotionCard", "EmotionCard_binah");
        }
        public static void ExportEmotionEgo()
        {
            StaticDataExport("Xml/Card/EmotionCard/EmotionEgo", Harmony_Patch.StaticPath + "/EmotionEgo", "EmotionEgo");
        }
        public static void ExportToolTip()
        {
            StaticDataExport("Xml/XmlToolTips", Harmony_Patch.StaticPath + "/XmlToolTips", "XmlToolTips");
        }
        public static void ExportTitle()
        {
            StaticDataExport("Xml/Titles", Harmony_Patch.StaticPath + "/Titles", "Titles");
        }
        public static void ExportFormation()
        {
            StaticDataExport("Xml/FormationInfo", Harmony_Patch.StaticPath + "/FormationInfo", "FormationInfo");
        }
        public static void ExportQuest()
        {
            StaticDataExport("Xml/QuestInfo", Harmony_Patch.StaticPath + "/QuestInfo", "QuestInfo");
        }
        public static void ExportEnemyUnit()
        {
            StaticDataExport("Xml/EnemyUnitInfo", Harmony_Patch.StaticPath + "/EnemyUnitInfo", "EnemyUnitInfo");
            StaticDataExport("Xml/EnemyUnitInfo_ch2", Harmony_Patch.StaticPath + "/EnemyUnitInfo", "EnemyUnitInfo_ch2");
            StaticDataExport("Xml/EnemyUnitInfo_ch3", Harmony_Patch.StaticPath + "/EnemyUnitInfo", "EnemyUnitInfo_ch3");
            StaticDataExport("Xml/EnemyUnitInfo_ch4", Harmony_Patch.StaticPath + "/EnemyUnitInfo", "EnemyUnitInfo_ch4");
            StaticDataExport("Xml/EnemyUnitInfo_ch5", Harmony_Patch.StaticPath + "/EnemyUnitInfo", "EnemyUnitInfo_ch5");
            StaticDataExport("Xml/EnemyUnitInfo_ch5_2", Harmony_Patch.StaticPath + "/EnemyUnitInfo", "EnemyUnitInfo_ch5_2");
            StaticDataExport("Xml/EnemyUnitInfo_ch6", Harmony_Patch.StaticPath + "/EnemyUnitInfo", "EnemyUnitInfo_ch6");
            StaticDataExport("Xml/EnemyUnitInfo_ch7", Harmony_Patch.StaticPath + "/EnemyUnitInfo", "EnemyUnitInfo_ch7");
            StaticDataExport("Xml/EnemyUnitInfo_creature", Harmony_Patch.StaticPath + "/EnemyUnitInfo", "EnemyUnitInfo_creature");
            StaticDataExport("Xml/EnemyUnitInfo_creature_final", Harmony_Patch.StaticPath + "/EnemyUnitInfo", "EnemyUnitInfo_creature_final");
            StaticDataExport("Xml/EnemyUnitInfo_ch7_Philip", Harmony_Patch.StaticPath + "/EnemyUnitInfo", "EnemyUnitInfo_ch7_Philip");
            StaticDataExport("Xml/EnemyUnitInfo_ch7_Eileen", Harmony_Patch.StaticPath + "/EnemyUnitInfo", "EnemyUnitInfo_ch7_Eileen");
            StaticDataExport("Xml/EnemyUnitInfo_ch7_Greta", Harmony_Patch.StaticPath + "/EnemyUnitInfo", "EnemyUnitInfo_ch7_Greta");
            StaticDataExport("Xml/EnemyUnitInfo_ch7_Bremen", Harmony_Patch.StaticPath + "/EnemyUnitInfo", "EnemyUnitInfo_ch7_Bremen");
            StaticDataExport("Xml/EnemyUnitInfo_ch7_Oswald", Harmony_Patch.StaticPath + "/EnemyUnitInfo", "EnemyUnitInfo_ch7_Oswald");
            StaticDataExport("Xml/EnemyUnitInfo_ch7_Jaeheon", Harmony_Patch.StaticPath + "/EnemyUnitInfo", "EnemyUnitInfo_ch7_Jaeheon");
            StaticDataExport("Xml/EnemyUnitInfo_ch7_Elena", Harmony_Patch.StaticPath + "/EnemyUnitInfo", "EnemyUnitInfo_ch7_Elena");
            StaticDataExport("Xml/EnemyUnitInfo_ch7_Pluto", Harmony_Patch.StaticPath + "/EnemyUnitInfo", "EnemyUnitInfo_ch7_Pluto");
            StaticDataExport("Xml/EnemyUnitInfo_ch7_BandFinal", Harmony_Patch.StaticPath + "/EnemyUnitInfo", "EnemyUnitInfo_ch7_BandFinal");
        }
        public static void ExportStage()
        {
            StaticDataExport("Xml/StageInfo", Harmony_Patch.StaticPath + "/StageInfo", "StageInfo");
            StaticDataExport("Xml/StageInfo_creature", Harmony_Patch.StaticPath + "/StageInfo", "StageInfo_creature");
            StaticDataExport("Xml/StageInfo_normal", Harmony_Patch.StaticPath + "/StageInfo", "StageInfo_normal");
        }
        public static void ExportFloorInfo()
        {
            StaticDataExport("Xml/FloorLevelInfo", Harmony_Patch.StaticPath + "/FloorLevelInfo", "FloorLevelInfo");
        }
        public static void ExportFinalRewardInfo()
        {
            StaticDataExport("Xml/Card/FinalBandReward", Harmony_Patch.StaticPath + "/FinalBandReward", "FinalBandReward");
        }
        public static void ExportCreditInfo()
        {
            StaticDataExport("Xml/EndingCredit/CreditPerson", Harmony_Patch.StaticPath + "/EndingCredit", "CreditPerson");
        }
        public static void ExportResourceInfo()
        {
            StaticDataExport("Xml/ResourcesInfo", Harmony_Patch.StaticPath + "/ResourceInfo", "ResourceInfo");
        }
        public static void ExportAttackEffectInfo()
        {
            StaticDataExport("Xml/AttackEffectPathInfo", Harmony_Patch.StaticPath + "/AttackEffectPathInfo", "AttackEffectPathInfo");
        }
        public static void LoadModFiles(List<ModContent> loadedContents)
        {
            int i = 1;
            try
            {
                Dictionary<string, List<DiceCardXmlInfo>> _workshopCard = ItemXmlDataList.instance.GetType().GetField("_workshopDict", AccessTools.all).GetValue(ItemXmlDataList.instance) as Dictionary<string, List<DiceCardXmlInfo>>;
                i++;
                List<DiceCardXmlInfo> _cardInfoList = ItemXmlDataList.instance.GetType().GetField("_cardInfoList", AccessTools.all).GetValue(ItemXmlDataList.instance) as List<DiceCardXmlInfo>;
                i++;
                Dictionary<LorId, DiceCardXmlInfo> _cardInfoTable = ItemXmlDataList.instance.GetType().GetField("_cardInfoTable", AccessTools.all).GetValue(ItemXmlDataList.instance) as Dictionary<LorId, DiceCardXmlInfo>;
                i++;
                List<DiceCardXmlInfo> _basicCardList = ItemXmlDataList.instance.GetType().GetField("_basicCardList", AccessTools.all).GetValue(ItemXmlDataList.instance) as List<DiceCardXmlInfo>;
                i++;
                List<DeckXmlInfo> DeckXml = Singleton<DeckXmlList>.Instance.GetType().GetField("_list", AccessTools.all).GetValue(Singleton<DeckXmlList>.Instance) as List<DeckXmlInfo>;
                i++;
                Dictionary<string, List<BookXmlInfo>> _workshopBookDict = Singleton<BookXmlList>.Instance.GetType().GetField("_workshopBookDict", AccessTools.all).GetValue(Singleton<BookXmlList>.Instance) as Dictionary<string, List<BookXmlInfo>>;
                i++;
                List<BookXmlInfo> _equiplist = Singleton<BookXmlList>.Instance.GetType().GetField("_list", AccessTools.all).GetValue(Singleton<BookXmlList>.Instance) as List<BookXmlInfo>;
                i++;
                Dictionary<LorId, BookXmlInfo> _equipdictionary = Singleton<BookXmlList>.Instance.GetType().GetField("_dictionary", AccessTools.all).GetValue(Singleton<BookXmlList>.Instance) as Dictionary<LorId, BookXmlInfo>;
                i++;
                Dictionary<string, List<CardDropTableXmlInfo>> _workshopCardDropDict = Singleton<CardDropTableXmlList>.Instance.GetType().GetField("_workshopDict", AccessTools.all).GetValue(Singleton<CardDropTableXmlList>.Instance) as Dictionary<string, List<CardDropTableXmlInfo>>;
                i++;
                List<CardDropTableXmlInfo> _CardDroplist = Singleton<CardDropTableXmlList>.Instance.GetType().GetField("_list", AccessTools.all).GetValue(Singleton<CardDropTableXmlList>.Instance) as List<CardDropTableXmlInfo>;
                i++;
                Dictionary<string, List<DropBookXmlInfo>> _workshopDropBookDict = Singleton<DropBookXmlList>.Instance.GetType().GetField("_workshopDict", AccessTools.all).GetValue(Singleton<DropBookXmlList>.Instance) as Dictionary<string, List<DropBookXmlInfo>>;
                i++;
                List<DropBookXmlInfo> _DropBooklist = Singleton<DropBookXmlList>.Instance.GetType().GetField("_list", AccessTools.all).GetValue(Singleton<DropBookXmlList>.Instance) as List<DropBookXmlInfo>;
                i++;
                Dictionary<LorId, DropBookXmlInfo> _DropBookdictionary = Singleton<DropBookXmlList>.Instance.GetType().GetField("_dict", AccessTools.all).GetValue(Singleton<DropBookXmlList>.Instance) as Dictionary<LorId, DropBookXmlInfo>;
                Dictionary<string, List<EnemyUnitClassInfo>> _workshopEnemyDict = Singleton<EnemyUnitClassInfoList>.Instance.GetType().GetField("_workshopEnemyDict", AccessTools.all).GetValue(Singleton<EnemyUnitClassInfoList>.Instance) as Dictionary<string, List<EnemyUnitClassInfo>>;
                i++;
                List<EnemyUnitClassInfo> _Enemylist = Singleton<EnemyUnitClassInfoList>.Instance.GetType().GetField("_list", AccessTools.all).GetValue(Singleton<EnemyUnitClassInfoList>.Instance) as List<EnemyUnitClassInfo>;
                i++;
                List<StageClassInfo> _Stagelist = Singleton<StageClassInfoList>.Instance.GetType().GetField("_list", AccessTools.all).GetValue(Singleton<StageClassInfoList>.Instance) as List<StageClassInfo>;
                i++;
                Dictionary<string, List<StageClassInfo>> _workshopStageDict = Singleton<StageClassInfoList>.Instance.GetType().GetField("_workshopStageDict", AccessTools.all).GetValue(Singleton<StageClassInfoList>.Instance) as Dictionary<string, List<StageClassInfo>>;
                i++;
                List<StageClassInfo> _recipeCondList = Singleton<StageClassInfoList>.Instance.GetType().GetField("_recipeCondList", AccessTools.all).GetValue(Singleton<StageClassInfoList>.Instance) as List<StageClassInfo>;
                i++;
                Dictionary<int, List<StageClassInfo>> _valueCondList = Singleton<StageClassInfoList>.Instance.GetType().GetField("_valueCondList", AccessTools.all).GetValue(Singleton<StageClassInfoList>.Instance) as Dictionary<int, List<StageClassInfo>>;
                i++;
                List<GiftXmlInfo> GiftXml = Singleton<GiftXmlList>.Instance.GetType().GetField("_list", AccessTools.all).GetValue(Singleton<GiftXmlList>.Instance) as List<GiftXmlInfo>;
                i++;
                List<EmotionCardXmlInfo> EmotionCardXml = Singleton<EmotionCardXmlList>.Instance.GetType().GetField("_list", AccessTools.all).GetValue(Singleton<EmotionCardXmlList>.Instance) as List<EmotionCardXmlInfo>;
                i++;
                List<EmotionEgoXmlInfo> EmotionEgoXml = Singleton<EmotionEgoXmlList>.Instance.GetType().GetField("_list", AccessTools.all).GetValue(Singleton<EmotionEgoXmlList>.Instance) as List<EmotionEgoXmlInfo>;
                i++;
                List<ToolTipXmlInfo> ToolTipXml = Singleton<ToolTipXmlList>.Instance.GetType().GetField("_list", AccessTools.all).GetValue(Singleton<ToolTipXmlList>.Instance) as List<ToolTipXmlInfo>;
                i++;
                List<TitleXmlInfo> prefixList = Singleton<TitleXmlList>.Instance.GetType().GetField("_prefixList", AccessTools.all).GetValue(Singleton<TitleXmlList>.Instance) as List<TitleXmlInfo>;
                i++;
                List<TitleXmlInfo> postfixList = Singleton<TitleXmlList>.Instance.GetType().GetField("_postfixList", AccessTools.all).GetValue(Singleton<TitleXmlList>.Instance) as List<TitleXmlInfo>;
                i++;
                List<FormationXmlInfo> FormationXml = Singleton<FormationXmlList>.Instance.GetType().GetField("_list", AccessTools.all).GetValue(Singleton<FormationXmlList>.Instance) as List<FormationXmlInfo>;
                i++;
                List<QuestXmlInfo> QuestXml = Singleton<QuestXmlList>.Instance.GetType().GetField("_list", AccessTools.all).GetValue(Singleton<QuestXmlList>.Instance) as List<QuestXmlInfo>;
                i++;
                List<FloorLevelXmlInfo> FloorLevelXml = Singleton<FloorLevelXmlList>.Instance.GetType().GetField("_list", AccessTools.all).GetValue(Singleton<FloorLevelXmlList>.Instance) as List<FloorLevelXmlInfo>;
                i++;
                foreach (ModContent modcontent in loadedContents)
                {
                    DirectoryInfo _dirInfo = modcontent.GetType().GetField("_dirInfo", AccessTools.all).GetValue(modcontent) as DirectoryInfo;
                    ModContentInfo _modInfo = modcontent.GetType().GetField("_modInfo", AccessTools.all).GetValue(modcontent) as ModContentInfo;
                    string workshopId = _modInfo.invInfo.workshopInfo.uniqueId;
                    if (workshopId.ToLower().EndsWith("@origin"))
                    {
                        workshopId = "";
                    }
                    try
                    {
                        LoadPassive_MOD(_dirInfo, workshopId);
                        LoadCard_MOD(_dirInfo, workshopId, _workshopCard, _cardInfoList, _cardInfoTable, _basicCardList);
                        LoadDeck_MOD(_dirInfo, workshopId, DeckXml);
                        LoadBook_MOD(_dirInfo, workshopId, _workshopBookDict, _equiplist, _equipdictionary);
                        LoadCardDropTable_MOD(_dirInfo, workshopId, _workshopCardDropDict, _CardDroplist);
                        LoadDropBook_MOD(_dirInfo, workshopId, _workshopDropBookDict, _DropBooklist, _DropBookdictionary);
                        LoadGift_MOD(_dirInfo, GiftXml);
                        LoadEmotionCard_MOD(_dirInfo, EmotionCardXml);
                        LoadEmotionEgo_MOD(_dirInfo, EmotionEgoXml, workshopId);
                        LoadToolTip_MOD(_dirInfo, ToolTipXml);
                        LoadTitle_MOD(_dirInfo, prefixList, postfixList);
                        LoadFormation_MOD(_dirInfo, FormationXml);
                        LoadQuest_MOD(_dirInfo, QuestXml);
                        LoadEnemyUnit_MOD(_dirInfo, workshopId, _workshopEnemyDict, _Enemylist);
                        if (workshopId == "")
                        {
                            LoadStage_MODorigin(_dirInfo, _recipeCondList, _valueCondList, _Stagelist);
                        }
                        else
                        {
                            LoadStage_MOD(_dirInfo, workshopId, _workshopStageDict, _Stagelist);
                        }
                        LoadFloorInfo_MOD(_dirInfo, FloorLevelXml);
                    }
                    catch (Exception ex)
                    {
                        Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
                        File.WriteAllText(Application.dataPath + "/Mods/" + _dirInfo.Name + "_SDLerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                }
            }
            catch (Exception exe)
            {
                File.WriteAllText(Application.dataPath + "/Mods/Loaderror.log", i.ToString() + Environment.NewLine + exe.Message + Environment.NewLine + exe.StackTrace);
            }
        }
        private static void LoadPassive_MOD(DirectoryInfo dir, string uniqueId)
        {
            string moddingPath = GetModdingPath(dir, "PassiveList");
            if (Directory.Exists(moddingPath))
            {
                LoadPassive_MOD_Checking(new DirectoryInfo(moddingPath), uniqueId);
            }
        }
        private static void LoadPassive_MOD_Checking(DirectoryInfo dir, string uniqueId)
        {
            List<PassiveXmlInfo> list = new List<PassiveXmlInfo>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                list.AddRange(LoadNewPassive(File.ReadAllText(fileInfo.FullName)).list);
            }
            foreach (PassiveXmlInfo passiveXmlInfo in list)
            {
                passiveXmlInfo.workshopID = uniqueId;
            }
            if (list != null && list.Count > 0)
            {
                Singleton<PassiveXmlList>.Instance.GetDataAll().RemoveAll((PassiveXmlInfo x) => list.Exists((PassiveXmlInfo y) => x.id == y.id));
                Singleton<PassiveXmlList>.Instance.GetDataAll().AddRange(list);
            }
        }
        private static PassiveXmlRoot LoadNewPassive(string str)
        {
            PassiveXmlRoot result;
            using (StringReader stringReader = new StringReader(str))
            {
                result = (PassiveXmlRoot)new XmlSerializer(typeof(PassiveXmlRoot)).Deserialize(stringReader);
            }
            return result;
        }
        private static void LoadCard_MOD(DirectoryInfo dir, string uniqueId, Dictionary<string, List<DiceCardXmlInfo>> _workshopDict, List<DiceCardXmlInfo> _cardInfoList, Dictionary<LorId, DiceCardXmlInfo> _cardInfoTable, List<DiceCardXmlInfo> _basicCardList)
        {
            string moddingPath = GetModdingPath(dir, "Card");
            if (Directory.Exists(moddingPath))
            {
                LoadCard_MOD_Checking(new DirectoryInfo(moddingPath), uniqueId, _workshopDict, _cardInfoList, _cardInfoTable, _basicCardList);
            }
        }
        private static void LoadCard_MOD_Checking(DirectoryInfo dir, string uniqueId, Dictionary<string, List<DiceCardXmlInfo>> _workshopDict, List<DiceCardXmlInfo> _cardInfoList, Dictionary<LorId, DiceCardXmlInfo> _cardInfoTable, List<DiceCardXmlInfo> _basicCardList)
        {
            List<DiceCardXmlInfo> list = new List<DiceCardXmlInfo>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                list.AddRange(LoadNewCard(File.ReadAllText(fileInfo.FullName)).cardXmlList);
            }
            if (_basicCardList == null)
            {
                _basicCardList = new List<DiceCardXmlInfo>();
            }
            foreach (DiceCardXmlInfo diceCardXmlInfo in list)
            {
                diceCardXmlInfo.workshopID = uniqueId;
                if (diceCardXmlInfo.optionList.Contains(CardOption.Basic))
                {
                    _basicCardList.RemoveAll((DiceCardXmlInfo x) => x.id == diceCardXmlInfo.id);
                    _basicCardList.Add(diceCardXmlInfo);
                }
            }
            AddCardInfoByMod(uniqueId, list, _workshopDict, _cardInfoList, _cardInfoTable);
        }
        private static void AddCardInfoByMod(string workshopId, List<DiceCardXmlInfo> list, Dictionary<string, List<DiceCardXmlInfo>> _workshopDict, List<DiceCardXmlInfo> _cardInfoList, Dictionary<LorId, DiceCardXmlInfo> _cardInfoTable)
        {
            if (_workshopDict == null)
            {
                _workshopDict = new Dictionary<string, List<DiceCardXmlInfo>>();
            }
            if (!_workshopDict.ContainsKey(workshopId))
            {
                _workshopDict.Add(workshopId, list);
            }
            else
            {
                _workshopDict[workshopId].RemoveAll((DiceCardXmlInfo x) => list.Exists((DiceCardXmlInfo y) => x.id == y.id));
                _workshopDict[workshopId].AddRange(list);
            }
            if (_cardInfoList != null)
            {
                _cardInfoList.RemoveAll((DiceCardXmlInfo x) => list.Exists((DiceCardXmlInfo y) => x.id == y.id));
                _cardInfoList.AddRange(list);
            }
            if (_cardInfoTable != null)
            {
                foreach (DiceCardXmlInfo diceCardXmlInfo in list)
                {
                    _cardInfoTable[diceCardXmlInfo.id] = diceCardXmlInfo;
                }
            }
        }
        private static DiceCardXmlRoot LoadNewCard(string str)
        {
            DiceCardXmlRoot result;
            using (StringReader stringReader = new StringReader(str))
            {
                result = (DiceCardXmlRoot)new XmlSerializer(typeof(DiceCardXmlRoot)).Deserialize(stringReader);
            }
            return result;
        }
        private static void LoadDeck_MOD(DirectoryInfo dir, string uniqueId, List<DeckXmlInfo> DeckXml)
        {
            string moddingPath = GetModdingPath(dir, "Deck");
            if (Directory.Exists(moddingPath))
            {
                LoadDeck_MOD_Checking(new DirectoryInfo(moddingPath), uniqueId, DeckXml);
            }
        }
        private static void LoadDeck_MOD_Checking(DirectoryInfo dir, string uniqueId, List<DeckXmlInfo> DeckXml)
        {
            List<DeckXmlInfo> list = new List<DeckXmlInfo>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                list.AddRange(LoadNewDeck(File.ReadAllText(fileInfo.FullName)).deckXmlList);
            }
            foreach (DeckXmlInfo deckXmlInfo in list)
            {
                deckXmlInfo.workshopId = uniqueId;
                deckXmlInfo.cardIdList.Clear();
                LorId.InitializeLorIds(deckXmlInfo._cardIdList, deckXmlInfo.cardIdList, uniqueId);
            }
            if (DeckXml != null && DeckXml.Count > 0)
            {
                DeckXml.RemoveAll((DeckXmlInfo x) => list.Exists((DeckXmlInfo y) => x.id == y.id));
                DeckXml.AddRange(list);
            }
        }
        private static DeckXmlRoot LoadNewDeck(string str)
        {
            DeckXmlRoot result;
            using (StringReader stringReader = new StringReader(str))
            {
                result = (DeckXmlRoot)new XmlSerializer(typeof(DeckXmlRoot)).Deserialize(stringReader);
            }
            return result;
        }
        private static void LoadBook_MOD(DirectoryInfo dir, string uniqueId, Dictionary<string, List<BookXmlInfo>> _workshopBookDict, List<BookXmlInfo> _list, Dictionary<LorId, BookXmlInfo> _dictionary)
        {
            string moddingPath = GetModdingPath(dir, "EquipPage");
            if (Directory.Exists(moddingPath))
            {
                LoadBook_MOD_Checking(new DirectoryInfo(moddingPath), uniqueId, _workshopBookDict, _list, _dictionary);
            }
        }
        private static void LoadBook_MOD_Checking(DirectoryInfo dir, string uniqueId, Dictionary<string, List<BookXmlInfo>> _workshopBookDict, List<BookXmlInfo> _list, Dictionary<LorId, BookXmlInfo> _dictionary)
        {
            List<BookXmlInfo_New> list = new List<BookXmlInfo_New>();
            List<BookXmlInfo> list2 = new List<BookXmlInfo>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                list.AddRange(LoadNewCorePage(File.ReadAllText(fileInfo.FullName)).bookXmlList);
            }
            foreach (BookXmlInfo_New bookXmlInfo in list)
            {
                LorId booklorid = new LorId(uniqueId, bookXmlInfo._id);
                List<LorId> onlycardlist = new List<LorId>();
                bookXmlInfo.workshopID = uniqueId;
                if (!string.IsNullOrEmpty(bookXmlInfo.skinType))
                {
                    if (bookXmlInfo.skinType == "UNKNOWN")
                    {
                        bookXmlInfo.skinType = "Lor";
                    }
                    else if (bookXmlInfo.skinType == "CUSTOM")
                    {
                        bookXmlInfo.skinType = "Custom";
                    }
                    else if (bookXmlInfo.skinType == "LOR")
                    {
                        bookXmlInfo.skinType = "Lor";
                    }
                }
                else if (bookXmlInfo.CharacterSkin[0].StartsWith("Custom"))
                {
                    bookXmlInfo.skinType = "Custom";
                }
                else
                {
                    bookXmlInfo.skinType = "Lor";
                }
                LorId.InitializeLorIds(bookXmlInfo.EquipEffect._PassiveList, bookXmlInfo.EquipEffect.PassiveList, uniqueId);
                list2.Add(new BookXmlInfo().CopyBookXmlInfo(bookXmlInfo));
                LorId.InitializeLorIds(bookXmlInfo.EquipEffect.OnlyCard, onlycardlist, uniqueId);
                OrcTools.OnlyCardDic[booklorid] = onlycardlist;
                OrcTools.SoulCardDic[booklorid] = bookXmlInfo.EquipEffect.CardList;
                if (bookXmlInfo.episode.xmlId > 0)
                {
                    OrcTools.EpisodeDic[booklorid] = LorId.MakeLorId(bookXmlInfo.episode, uniqueId);
                }
            }
            AddEquipPageByMod(uniqueId, list2, _workshopBookDict, _list, _dictionary);
        }
        private static void AddEquipPageByMod(string workshopId, List<BookXmlInfo> list, Dictionary<string, List<BookXmlInfo>> _workshopBookDict, List<BookXmlInfo> _list, Dictionary<LorId, BookXmlInfo> _dictionary)
        {
            if (_workshopBookDict == null)
            {
                _workshopBookDict = new Dictionary<string, List<BookXmlInfo>>();
            }
            if (!_workshopBookDict.ContainsKey(workshopId))
            {
                _workshopBookDict.Add(workshopId, list);
            }
            else
            {
                _workshopBookDict[workshopId].RemoveAll((BookXmlInfo x) => list.Exists((BookXmlInfo y) => x.id == y.id));
                _workshopBookDict[workshopId].AddRange(list);
            }
            if (_list != null)
            {
                _list.RemoveAll((BookXmlInfo x) => list.Exists((BookXmlInfo y) => x.id == y.id));
                _list.AddRange(list);
            }
            if (_dictionary != null)
            {
                foreach (BookXmlInfo bookXmlInfo in list)
                {
                    _dictionary[bookXmlInfo.id] = bookXmlInfo;
                }
            }
        }
        private static GTMDProjectMoon.BookXmlRoot LoadNewCorePage(string str)
        {
            GTMDProjectMoon.BookXmlRoot result;
            using (StringReader stringReader = new StringReader(str))
            {
                result = (GTMDProjectMoon.BookXmlRoot)new XmlSerializer(typeof(GTMDProjectMoon.BookXmlRoot)).Deserialize(stringReader);
            }
            return result;
        }
        private static void LoadCardDropTable_MOD(DirectoryInfo dir, string uniqueId, Dictionary<string, List<CardDropTableXmlInfo>> _workshopDict, List<CardDropTableXmlInfo> _list)
        {
            string moddingPath = GetModdingPath(dir, "CardDropTable");
            if (Directory.Exists(moddingPath))
            {
                LoadCardDropTable_MOD_Checking(new DirectoryInfo(moddingPath), uniqueId, _workshopDict, _list);
            }
        }
        private static void LoadCardDropTable_MOD_Checking(DirectoryInfo dir, string uniqueId, Dictionary<string, List<CardDropTableXmlInfo>> _workshopDict, List<CardDropTableXmlInfo> _list)
        {
            List<CardDropTableXmlInfo> list = new List<CardDropTableXmlInfo>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                list.AddRange(LoadNewCardDropTable(File.ReadAllText(fileInfo.FullName)).dropTableXmlList);
            }
            foreach (CardDropTableXmlInfo cardDropTableXmlInfo in list)
            {
                cardDropTableXmlInfo.workshopId = uniqueId;
                cardDropTableXmlInfo.cardIdList.Clear();
                LorId.InitializeLorIds(cardDropTableXmlInfo._cardIdList, cardDropTableXmlInfo.cardIdList, uniqueId);
            }
            AddCardDropTableByMod(uniqueId, list, _workshopDict, _list);
        }
        private static void AddCardDropTableByMod(string uniqueId, List<CardDropTableXmlInfo> list, Dictionary<string, List<CardDropTableXmlInfo>> _workshopDict, List<CardDropTableXmlInfo> _list)
        {
            if (_workshopDict == null)
            {
                _workshopDict = new Dictionary<string, List<CardDropTableXmlInfo>>();
            }
            if (!_workshopDict.ContainsKey(uniqueId))
            {
                _workshopDict.Add(uniqueId, list);
            }
            else
            {
                _workshopDict[uniqueId].RemoveAll((CardDropTableXmlInfo x) => list.Exists((CardDropTableXmlInfo y) => x.id == y.id));
                _workshopDict[uniqueId].AddRange(list);
            }
            if (_list != null)
            {
                _list.RemoveAll((CardDropTableXmlInfo x) => list.Exists((CardDropTableXmlInfo y) => x.id == y.id));
                _list.AddRange(list);
            }
        }
        private static CardDropTableXmlRoot LoadNewCardDropTable(string str)
        {
            CardDropTableXmlRoot result;
            using (StringReader stringReader = new StringReader(str))
            {
                result = (CardDropTableXmlRoot)new XmlSerializer(typeof(CardDropTableXmlRoot)).Deserialize(stringReader);
            }
            return result;
        }
        private static void LoadDropBook_MOD(DirectoryInfo dir, string uniqueId, Dictionary<string, List<DropBookXmlInfo>> _workshopDict, List<DropBookXmlInfo> _list, Dictionary<LorId, DropBookXmlInfo> _dict)
        {
            string moddingPath = GetModdingPath(dir, "DropBook");
            if (Directory.Exists(moddingPath))
            {
                LoadDropBook_MOD_Checking(new DirectoryInfo(moddingPath), uniqueId, _workshopDict, _list, _dict);
            }
        }
        private static void LoadDropBook_MOD_Checking(DirectoryInfo dir, string uniqueId, Dictionary<string, List<DropBookXmlInfo>> _workshopDict, List<DropBookXmlInfo> _list, Dictionary<LorId, DropBookXmlInfo> _dict)
        {
            List<DropBookXmlInfo> list = new List<DropBookXmlInfo>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                list.AddRange(LoadNewDropBook(File.ReadAllText(fileInfo.FullName)).bookXmlList);
            }
            foreach (DropBookXmlInfo dropBookXmlInfo in list)
            {
                dropBookXmlInfo.workshopID = uniqueId;
                CardDropTableXmlInfo workshopData = Singleton<CardDropTableXmlList>.Instance.GetWorkshopData(uniqueId, dropBookXmlInfo.id.id);
                dropBookXmlInfo.InitializeDropItemList(uniqueId);
                if (workshopData != null)
                {
                    foreach (LorId id in workshopData.cardIdList)
                    {
                        dropBookXmlInfo.DropItemList.Add(new BookDropItemInfo(id)
                        {
                            itemType = DropItemType.Card
                        });
                    }
                }
            }
            AddBookByMod(uniqueId, list, _workshopDict, _list, _dict);
        }
        private static void AddBookByMod(string workshopId, List<DropBookXmlInfo> list, Dictionary<string, List<DropBookXmlInfo>> _workshopDict, List<DropBookXmlInfo> _list, Dictionary<LorId, DropBookXmlInfo> _dict)
        {
            if (_workshopDict == null)
            {
                _workshopDict = new Dictionary<string, List<DropBookXmlInfo>>();
            }
            if (!_workshopDict.ContainsKey(workshopId))
            {
                _workshopDict.Add(workshopId, list);
            }
            else
            {
                _workshopDict[workshopId].RemoveAll((DropBookXmlInfo x) => list.Exists((DropBookXmlInfo y) => x.id == y.id));
                _workshopDict[workshopId].AddRange(list);
            }
            if (_list != null)
            {
                _list.RemoveAll((DropBookXmlInfo x) => list.Exists((DropBookXmlInfo y) => x.id == y.id));
                _list.AddRange(list);
            }
            if (_dict != null)
            {
                foreach (DropBookXmlInfo bookXmlInfo in list)
                {
                    _dict[bookXmlInfo.id] = bookXmlInfo;
                }
            }
        }
        private static BookUseXmlRoot LoadNewDropBook(string str)
        {
            BookUseXmlRoot result;
            using (StringReader stringReader = new StringReader(str))
            {
                result = (BookUseXmlRoot)new XmlSerializer(typeof(BookUseXmlRoot)).Deserialize(stringReader);
            }
            return result;
        }
        private static void LoadGift_MOD(DirectoryInfo dir, List<GiftXmlInfo> root)
        {
            string moddingPath = GetModdingPath(dir, "GiftInfo");
            if (Directory.Exists(moddingPath))
            {
                LoadGift_MOD_Checking(new DirectoryInfo(moddingPath), root);
            }
        }
        private static void LoadGift_MOD_Checking(DirectoryInfo dir, List<GiftXmlInfo> root)
        {
            List<GiftXmlInfo> list = new List<GiftXmlInfo>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                list.AddRange(LoadNewGift(File.ReadAllText(fileInfo.FullName)).giftXmlList);
            }
            foreach (GiftXmlInfo giftXmlInfo in list)
            {
                bool flag = false;
                GiftXmlInfo item = null;
                foreach (GiftXmlInfo giftXmlInfo2 in root)
                {
                    if (giftXmlInfo2.id == giftXmlInfo.id)
                    {
                        flag = true;
                        item = giftXmlInfo2;
                        break;
                    }
                }
                if (flag)
                {
                    root.Remove(item);
                }
                root.Add(giftXmlInfo);
            }
        }
        private static GiftXmlRoot LoadNewGift(string str)
        {
            GiftXmlRoot result;
            using (StringReader stringReader = new StringReader(str))
            {
                result = (GiftXmlRoot)new XmlSerializer(typeof(GiftXmlRoot)).Deserialize(stringReader);
            }
            return result;
        }

        private static void LoadEmotionCard_MOD(DirectoryInfo dir, List<EmotionCardXmlInfo> root)
        {
            string moddingPath = GetModdingPath(dir, "EmotionCard");
            if (Directory.Exists(moddingPath))
            {
                LoadEmotionCard_MOD_Checking(new DirectoryInfo(moddingPath), root);
            }
        }
        private static void LoadEmotionCard_MOD_Checking(DirectoryInfo dir, List<EmotionCardXmlInfo> root)
        {
            List<EmotionCardXmlInfo> list = new List<EmotionCardXmlInfo>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                list.AddRange(LoadNewEmotionCard(File.ReadAllText(fileInfo.FullName)).emotionCardXmlList);
            }
            foreach (EmotionCardXmlInfo emotionCardXmlInfo in list)
            {
                bool flag = false;
                EmotionCardXmlInfo item = null;
                foreach (EmotionCardXmlInfo emotionCardXmlInfo2 in root)
                {
                    bool flag2 = emotionCardXmlInfo2.id == emotionCardXmlInfo.id && emotionCardXmlInfo2.Sephirah == emotionCardXmlInfo.Sephirah;
                    if (flag2)
                    {
                        flag = true;
                        item = emotionCardXmlInfo2;
                        break;
                    }
                }
                bool flag3 = flag;
                if (flag3)
                {
                    root.Remove(item);
                }
                root.Add(emotionCardXmlInfo);
            }
        }
        private static EmotionCardXmlRoot LoadNewEmotionCard(string str)
        {
            EmotionCardXmlRoot result;
            using (StringReader stringReader = new StringReader(str))
            {
                result = (EmotionCardXmlRoot)new XmlSerializer(typeof(EmotionCardXmlRoot)).Deserialize(stringReader);
            }
            return result;
        }
        private static void LoadEmotionEgo_MOD(DirectoryInfo dir, List<EmotionEgoXmlInfo> root, string workshopId)
        {
            string moddingPath = GetModdingPath(dir, "EmotionEgo");
            if (Directory.Exists(moddingPath))
            {
                LoadEmotionEgo_MOD_Checking(new DirectoryInfo(moddingPath), root, workshopId);
            }
        }
        private static void LoadEmotionEgo_MOD_Checking(DirectoryInfo dir, List<EmotionEgoXmlInfo> root, string workshopId)
        {
            List<EmotionEgoXmlInfo> list = new List<EmotionEgoXmlInfo>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                list.AddRange(LoadNewEmotionEgo(File.ReadAllText(fileInfo.FullName), workshopId).egoXmlList);
            }
            foreach (EmotionEgoXmlInfo emotionEgoXmlInfo in list)
            {
                bool flag = false;
                EmotionEgoXmlInfo item = null;
                foreach (EmotionEgoXmlInfo emotionEgoXmlInfo2 in root)
                {
                    if (emotionEgoXmlInfo2.id == emotionEgoXmlInfo.id)
                    {
                        flag = true;
                        item = emotionEgoXmlInfo2;
                        break;
                    }
                }
                if (flag)
                {
                    root.Remove(item);
                }
                root.Add(emotionEgoXmlInfo);
            }
        }
        private static EmotionEgoXmlRoot LoadNewEmotionEgo(string str, string workshopId)
        {
            GTMDProjectMoon.EmotionEgoXmlRoot emotionEgoXmlRoot_New;
            EmotionEgoXmlRoot emotionEgoXmlRoot = new EmotionEgoXmlRoot()
            {
                egoXmlList = new List<EmotionEgoXmlInfo>()
            };
            using (StringReader stringReader = new StringReader(str))
            {
                emotionEgoXmlRoot_New = (GTMDProjectMoon.EmotionEgoXmlRoot)new XmlSerializer(typeof(GTMDProjectMoon.EmotionEgoXmlRoot)).Deserialize(stringReader);
            }
            foreach (EmotionEgoXmlInfo_New egoXmlInfo_New in emotionEgoXmlRoot_New.egoXmlList)
            {
                EmotionEgoXmlInfo egoXmlInfo = new EmotionEgoXmlInfo()
                {
                    id = egoXmlInfo_New.id,
                    Sephirah = egoXmlInfo_New.Sephirah,
                    _CardId = egoXmlInfo_New._CardId.xmlId,
                    isLock = egoXmlInfo_New.isLock,
                };
                OrcTools.EgoDic.Add(egoXmlInfo, LorId.MakeLorId(egoXmlInfo_New._CardId, workshopId));
                emotionEgoXmlRoot.egoXmlList.Add(egoXmlInfo);
            }
            return emotionEgoXmlRoot;
        }
        private static void LoadToolTip_MOD(DirectoryInfo dir, List<ToolTipXmlInfo> root)
        {
            string moddingPath = GetModdingPath(dir, "XmlToolTips");
            if (Directory.Exists(moddingPath))
            {
                LoadToolTip_MOD_Checking(new DirectoryInfo(moddingPath), root);
            }
        }
        private static void LoadToolTip_MOD_Checking(DirectoryInfo dir, List<ToolTipXmlInfo> root)
        {
            List<ToolTipXmlInfo> list = new List<ToolTipXmlInfo>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                list.AddRange(LoadNewToolTip(File.ReadAllText(fileInfo.FullName)).toolTipXmlList);
            }
            foreach (ToolTipXmlInfo toolTipXmlInfo in list)
            {
                bool flag = false;
                ToolTipXmlInfo item = null;
                foreach (ToolTipXmlInfo toolTipXmlInfo2 in root)
                {
                    if (toolTipXmlInfo2.ID == toolTipXmlInfo.ID)
                    {
                        flag = true;
                        item = toolTipXmlInfo2;
                        break;
                    }
                }
                if (flag)
                {
                    root.Remove(item);
                }
                root.Add(toolTipXmlInfo);
            }
        }
        private static ToolTipXmlRoot LoadNewToolTip(string str)
        {
            ToolTipXmlRoot result;
            using (StringReader stringReader = new StringReader(str))
            {
                result = (ToolTipXmlRoot)new XmlSerializer(typeof(ToolTipXmlRoot)).Deserialize(stringReader);
            }
            return result;
        }
        private static void LoadTitle_MOD(DirectoryInfo dir, List<TitleXmlInfo> root1, List<TitleXmlInfo> root2)
        {
            string moddingPath = GetModdingPath(dir, "Titles");
            if (Directory.Exists(moddingPath))
            {
                LoadTitle_MOD_Checking(new DirectoryInfo(moddingPath), root1, root2);
            }
        }
        private static void LoadTitle_MOD_Checking(DirectoryInfo dir, List<TitleXmlInfo> root1, List<TitleXmlInfo> root2)
        {
            List<TitleXmlInfo> list = new List<TitleXmlInfo>();
            List<TitleXmlInfo> list2 = new List<TitleXmlInfo>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                TitleXmlRoot titleXmlRoot = LoadNewTitle(File.ReadAllText(fileInfo.FullName));
                list.AddRange(titleXmlRoot.prefixXmlList.List);
                list2.AddRange(titleXmlRoot.postfixXmlList.List);
            }
            foreach (TitleXmlInfo titleXmlInfo in list)
            {
                bool flag = false;
                TitleXmlInfo item = null;
                foreach (TitleXmlInfo titleXmlInfo2 in root1)
                {
                    if (titleXmlInfo2.ID == titleXmlInfo.ID)
                    {
                        flag = true;
                        item = titleXmlInfo2;
                        break;
                    }
                }
                if (flag)
                {
                    root1.Remove(item);
                }
                root1.Add(titleXmlInfo);
            }
            foreach (TitleXmlInfo titleXmlInfo3 in list2)
            {
                bool flag4 = false;
                TitleXmlInfo item2 = null;
                foreach (TitleXmlInfo titleXmlInfo4 in root2)
                {
                    if (titleXmlInfo4.ID == titleXmlInfo3.ID)
                    {
                        flag4 = true;
                        item2 = titleXmlInfo4;
                        break;
                    }
                }
                if (flag4)
                {
                    root1.Remove(item2);
                }
                root1.Add(titleXmlInfo3);
            }
        }
        public static TitleXmlRoot LoadNewTitle(string str)
        {
            TitleXmlRoot result;
            using (StringReader stringReader = new StringReader(str))
            {
                result = (TitleXmlRoot)new XmlSerializer(typeof(TitleXmlRoot)).Deserialize(stringReader);
            }
            return result;
        }
        private static void LoadFormation_MOD(DirectoryInfo dir, List<FormationXmlInfo> root)
        {
            string moddingPath = GetModdingPath(dir, "FormationInfo");
            if (Directory.Exists(moddingPath))
            {
                LoadFormation_MOD_Checking(new DirectoryInfo(moddingPath), root);
            }
        }
        private static void LoadFormation_MOD_Checking(DirectoryInfo dir, List<FormationXmlInfo> root)
        {
            List<FormationXmlInfo> list = new List<FormationXmlInfo>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                list.AddRange(LoadNewFormation(File.ReadAllText(fileInfo.FullName)).list);
            }
            foreach (FormationXmlInfo formationXmlInfo in list)
            {
                bool flag = false;
                FormationXmlInfo item = null;
                foreach (FormationXmlInfo formationXmlInfo2 in root)
                {
                    if (formationXmlInfo2.id == formationXmlInfo.id)
                    {
                        flag = true;
                        item = formationXmlInfo2;
                        break;
                    }
                }
                if (flag)
                {
                    root.Remove(item);
                }
                root.Add(formationXmlInfo);
            }
        }
        private static FormationXmlRoot LoadNewFormation(string str)
        {
            FormationXmlRoot result;
            using (StringReader stringReader = new StringReader(str))
            {
                result = (FormationXmlRoot)new XmlSerializer(typeof(FormationXmlRoot)).Deserialize(stringReader);
            }
            return result;
        }
        private static void LoadQuest_MOD(DirectoryInfo dir, List<QuestXmlInfo> root)
        {
            string moddingPath = GetModdingPath(dir, "QuestInfo");
            if (Directory.Exists(moddingPath))
            {
                LoadQuest_MOD_Checking(new DirectoryInfo(moddingPath), root);
            }
        }

        // Token: 0x06009666 RID: 38502 RVA: 0x003446F4 File Offset: 0x003428F4
        private static void LoadQuest_MOD_Checking(DirectoryInfo dir, List<QuestXmlInfo> root)
        {
            List<QuestXmlInfo> list = new List<QuestXmlInfo>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                list.AddRange(LoadNewQuest(File.ReadAllText(fileInfo.FullName)).list);
            }
            foreach (QuestXmlInfo questXmlInfo in list)
            {
                bool flag = false;
                QuestXmlInfo item = null;
                foreach (QuestXmlInfo questXmlInfo2 in root)
                {
                    if (questXmlInfo2.id == questXmlInfo.id)
                    {
                        flag = true;
                        item = questXmlInfo2;
                        break;
                    }
                }
                if (flag)
                {
                    root.Remove(item);
                }
                root.Add(questXmlInfo);
            }
        }
        private static QuestXmlRoot LoadNewQuest(string str)
        {
            QuestXmlRoot result;
            using (StringReader stringReader = new StringReader(str))
            {
                result = (QuestXmlRoot)new XmlSerializer(typeof(QuestXmlRoot)).Deserialize(stringReader);
            }
            return result;
        }
        private static void LoadEnemyUnit_MOD(DirectoryInfo dir, string uniqueId, Dictionary<string, List<EnemyUnitClassInfo>> _workshopEnemyDict, List<EnemyUnitClassInfo> _list)
        {
            string moddingPath = GetModdingPath(dir, "EnemyUnitInfo");
            if (Directory.Exists(moddingPath))
            {
                LoadEnemyUnit_MOD_Checking(new DirectoryInfo(moddingPath), uniqueId, _workshopEnemyDict, _list);
            }
        }
        private static void LoadEnemyUnit_MOD_Checking(DirectoryInfo dir, string uniqueId, Dictionary<string, List<EnemyUnitClassInfo>> _workshopEnemyDict, List<EnemyUnitClassInfo> _list)
        {
            List<EnemyUnitClassInfo> list = new List<EnemyUnitClassInfo>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                list.AddRange(LoadNewEnemyUnit(File.ReadAllText(fileInfo.FullName), uniqueId).list);
            }
            foreach (EnemyUnitClassInfo enemyUnitClassInfo in list)
            {
            }
            AddEnemyUnitByMod(uniqueId, list, _workshopEnemyDict, _list);
        }
        private static void AddEnemyUnitByMod(string workshopId, List<EnemyUnitClassInfo> list, Dictionary<string, List<EnemyUnitClassInfo>> _workshopEnemyDict, List<EnemyUnitClassInfo> _list)
        {
            if (_workshopEnemyDict == null)
            {
                _workshopEnemyDict = new Dictionary<string, List<EnemyUnitClassInfo>>();
            }
            if (!_workshopEnemyDict.ContainsKey(workshopId))
            {
                _workshopEnemyDict.Add(workshopId, list);
            }
            else
            {
                _workshopEnemyDict[workshopId].RemoveAll((EnemyUnitClassInfo x) => list.Exists((EnemyUnitClassInfo y) => x.id == y.id));
                _workshopEnemyDict[workshopId].AddRange(list);
            }
            if (_list != null)
            {
                _list.RemoveAll((EnemyUnitClassInfo x) => list.Exists((EnemyUnitClassInfo y) => x.id == y.id));
                _list.AddRange(list);
            }
        }
        private static EnemyUnitClassRoot LoadNewEnemyUnit(string str, string uniqueId)
        {
            EnemyUnitClassRoot result = new EnemyUnitClassRoot()
            {
                list = new List<EnemyUnitClassInfo>(),
            };
            GTMDProjectMoon.EnemyUnitClassRoot readyresult;
            using (StringReader stringReader = new StringReader(str))
            {
                readyresult = (GTMDProjectMoon.EnemyUnitClassRoot)new XmlSerializer(typeof(GTMDProjectMoon.EnemyUnitClassRoot)).Deserialize(stringReader);
            }
            foreach (EnemyUnitClassInfo_New enemyUnitClassInfo_New in readyresult.list)
            {
                enemyUnitClassInfo_New.workshopID = uniqueId;
                enemyUnitClassInfo_New.height = RandomUtil.Range(enemyUnitClassInfo_New.minHeight, enemyUnitClassInfo_New.maxHeight);
                foreach (EnemyDropItemTable_New enemyDropItemTable_New in enemyUnitClassInfo_New.dropTableList)
                {
                    foreach (EnemyDropItem_New enemyDropItem_New in enemyDropItemTable_New.dropItemList)
                    {
                        if (string.IsNullOrEmpty(enemyDropItem_New.workshopId))
                        {
                            enemyDropItemTable_New.dropList.Add(new EnemyDropItem_ReNew()
                            {
                                prob = enemyDropItem_New.prob,
                                bookId = new LorId(uniqueId, enemyDropItem_New.bookId)
                            });
                        }
                        else if (enemyDropItem_New.workshopId.ToLower() == "@origin")
                        {
                            enemyDropItemTable_New.dropList.Add(new EnemyDropItem_ReNew()
                            {
                                prob = enemyDropItem_New.prob,
                                bookId = new LorId(enemyDropItem_New.bookId)
                            });
                        }
                        else
                        {
                            enemyDropItemTable_New.dropList.Add(new EnemyDropItem_ReNew()
                            {
                                prob = enemyDropItem_New.prob,
                                bookId = new LorId(enemyDropItem_New.workshopId, enemyDropItem_New.bookId)
                            });
                        }
                    }
                }
                result.list.Add(new EnemyUnitClassInfo().CopyEnemyUnitClassInfo(enemyUnitClassInfo_New, uniqueId));
                OrcTools.DropItemDic[new LorId(uniqueId, enemyUnitClassInfo_New._id)] = enemyUnitClassInfo_New.dropTableList;
            }
            return result;
        }
        private static void LoadStage_MOD(DirectoryInfo dir, string uniqueId, Dictionary<string, List<StageClassInfo>> _workshopStageDict, List<StageClassInfo> _list)
        {
            string moddingPath = GetModdingPath(dir, "StageInfo");
            if (Directory.Exists(moddingPath))
            {
                LoadStage_MOD_Checking(new DirectoryInfo(moddingPath), uniqueId, _workshopStageDict, _list);
            }
        }
        private static void LoadStage_MOD_Checking(DirectoryInfo dir, string uniqueId, Dictionary<string, List<StageClassInfo>> _workshopStageDict, List<StageClassInfo> _list)
        {
            List<StageClassInfo> list = new List<StageClassInfo>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                list.AddRange(LoadNewStage(File.ReadAllText(fileInfo.FullName), uniqueId).list);
            }
            foreach (StageClassInfo stageClassInfo in list)
            {
                stageClassInfo.workshopID = uniqueId;
                stageClassInfo.InitializeIds(uniqueId);
                foreach (StageStoryInfo stageStoryInfo in stageClassInfo.storyList)
                {
                    stageStoryInfo.packageId = uniqueId;
                    stageStoryInfo.valid = true;
                }
            }
            AddStageByMod(uniqueId, list, _workshopStageDict, _list);
        }
        private static void AddStageByMod(string workshopId, List<StageClassInfo> list, Dictionary<string, List<StageClassInfo>> _workshopStageDict, List<StageClassInfo> _list)
        {
            if (_workshopStageDict == null)
            {
                _workshopStageDict = new Dictionary<string, List<StageClassInfo>>();
            }
            if (!_workshopStageDict.ContainsKey(workshopId))
            {
                _workshopStageDict.Add(workshopId, list);
                ClassifyWorkshopInvitation(list);
            }
            else
            {
                _workshopStageDict[workshopId].RemoveAll((StageClassInfo x) => list.Exists((StageClassInfo y) => x.id == y.id));
                _workshopStageDict[workshopId].AddRange(list);
                ClassifyWorkshopInvitation(list);
            }
            if (_list != null)
            {
                foreach (StageClassInfo item in list)
                {
                    _list.RemoveAll((StageClassInfo x) => list.Exists((StageClassInfo y) => x.id == y.id));
                    _list.AddRange(list);
                }
            }
        }
        private static void ClassifyWorkshopInvitation(List<StageClassInfo> list)
        {
            List<StageClassInfo> _workshopRecipeList = Singleton<StageClassInfoList>.Instance.GetType().GetField("_workshopRecipeList", AccessTools.all).GetValue(Singleton<StageClassInfoList>.Instance) as List<StageClassInfo>;
            Dictionary<int, List<StageClassInfo>> _workshopValueDict = Singleton<StageClassInfoList>.Instance.GetType().GetField("_workshopValueDict", AccessTools.all).GetValue(Singleton<StageClassInfoList>.Instance) as Dictionary<int, List<StageClassInfo>>;
            foreach (StageClassInfo stageClassInfo in list)
            {
                if (stageClassInfo.invitationInfo.combine == StageCombineType.BookRecipe)
                {
                    stageClassInfo.invitationInfo.needsBooks.Sort();
                    stageClassInfo.invitationInfo.needsBooks.Reverse();
                    _workshopRecipeList.RemoveAll((StageClassInfo x) => x.id == stageClassInfo.id);
                    _workshopRecipeList.Add(stageClassInfo);
                }
                else if (stageClassInfo.invitationInfo.combine == StageCombineType.BookValue)
                {
                    int bookNum = stageClassInfo.invitationInfo.bookNum;
                    if (bookNum >= 1 && bookNum <= 3)
                    {
                        _workshopValueDict[bookNum].RemoveAll((StageClassInfo x) => x.id == stageClassInfo.id);
                        _workshopValueDict[bookNum].Add(stageClassInfo);
                        _workshopValueDict[1].Sort((StageClassInfo info1, StageClassInfo info2) => (int)(10f * (info2.invitationInfo.bookValue - info1.invitationInfo.bookValue)));
                        _workshopValueDict[2].Sort((StageClassInfo info1, StageClassInfo info2) => (int)(10f * (info2.invitationInfo.bookValue - info1.invitationInfo.bookValue)));
                        _workshopValueDict[3].Sort((StageClassInfo info1, StageClassInfo info2) => (int)(10f * (info2.invitationInfo.bookValue - info1.invitationInfo.bookValue)));
                    }
                }
            }
        }
        private static void LoadStage_MODorigin(DirectoryInfo dir, List<StageClassInfo> _recipeCondList, Dictionary<int, List<StageClassInfo>> _valueCondList, List<StageClassInfo> _list)
        {
            string moddingPath = GetModdingPath(dir, "StageInfo");
            if (Directory.Exists(moddingPath))
            {
                LoadStage_MODorigin_Checking(new DirectoryInfo(moddingPath), _recipeCondList, _valueCondList, _list);
            }
        }
        private static void LoadStage_MODorigin_Checking(DirectoryInfo dir, List<StageClassInfo> _recipeCondList, Dictionary<int, List<StageClassInfo>> _valueCondList, List<StageClassInfo> _list)
        {
            List<StageClassInfo> list = new List<StageClassInfo>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                list.AddRange(LoadNewStage(File.ReadAllText(fileInfo.FullName), "").list);
            }
            foreach (StageClassInfo stageClassInfo in list)
            {
                stageClassInfo.workshopID = "";
                stageClassInfo.InitializeIds("");
                foreach (StageStoryInfo stageStoryInfo in stageClassInfo.storyList)
                {
                    stageStoryInfo.packageId = "";
                    stageStoryInfo.valid = true;
                }
            }
            AddStageByModorigin(list, _recipeCondList, _valueCondList, _list);
        }
        private static void AddStageByModorigin(List<StageClassInfo> list, List<StageClassInfo> _recipeCondList, Dictionary<int, List<StageClassInfo>> _valueCondList, List<StageClassInfo> _list)
        {
            if (_list != null)
            {
                foreach (StageClassInfo item in list)
                {
                    _list.RemoveAll((StageClassInfo x) => list.Exists((StageClassInfo y) => x.id == y.id));
                    _list.AddRange(list);
                }
            }
            foreach (StageClassInfo stageClassInfo in list)
            {
                foreach (StageStoryInfo stageStoryInfo in stageClassInfo.storyList)
                {
                    if (!(stageStoryInfo.story == ""))
                    {
                        stageStoryInfo.chapter = stageClassInfo.chapter;
                        stageStoryInfo.group = 1;
                        stageStoryInfo.episode = stageClassInfo._id;
                        stageStoryInfo.valid = true;
                    }
                }
                if (stageClassInfo.stageType == StageType.Invitation)
                {
                    stageClassInfo.invitationInfo.needsBooks.Sort();
                    stageClassInfo.invitationInfo.needsBooks.Reverse();
                    if (stageClassInfo.invitationInfo.combine == StageCombineType.BookRecipe)
                    {
                        _recipeCondList.Add(stageClassInfo);
                    }
                    else if (stageClassInfo.invitationInfo.combine == StageCombineType.BookValue)
                    {
                        if (stageClassInfo.invitationInfo.bookNum >= 1 && stageClassInfo.invitationInfo.bookNum <= 3)
                        {
                            _valueCondList[stageClassInfo.invitationInfo.bookNum].Add(stageClassInfo);
                        }
                        else
                        {
                            Debug.LogError("invalid bookNum");
                        }
                    }
                }
            }
            Comparison<StageClassInfo> comparison = (StageClassInfo info1, StageClassInfo info2) => (int)(10f * (info2.invitationInfo.bookValue - info1.invitationInfo.bookValue));
            _valueCondList[1].Sort(comparison);
            _valueCondList[2].Sort(comparison);
            _valueCondList[3].Sort(comparison);
        }
        private static StageXmlRoot LoadNewStage(string str, string uniqueId)
        {
            StageXmlRoot result = new StageXmlRoot
            {
                list = new List<StageClassInfo>()
            };
            GTMDProjectMoon.StageXmlRoot readyresult;
            using (StringReader stringReader = new StringReader(str))
            {
                readyresult = (GTMDProjectMoon.StageXmlRoot)new XmlSerializer(typeof(GTMDProjectMoon.StageXmlRoot)).Deserialize(stringReader);
            }
            foreach (StageClassInfo_New stageClassInfo_New in readyresult.list)
            {
                stageClassInfo_New.workshopID = uniqueId;
                LorId.InitializeLorIds(stageClassInfo_New.extraCondition.needClearStageList, stageClassInfo_New.extraCondition.stagecondition, uniqueId);
                result.list.Add(new StageClassInfo().CopyStageClassInfo(stageClassInfo_New, uniqueId));
                OrcTools.StageConditionDic[result.list[result.list.Count - 1].extraCondition] = stageClassInfo_New.extraCondition.stagecondition;
            }
            return result;
        }
        private static void LoadFloorInfo_MOD(DirectoryInfo dir, List<FloorLevelXmlInfo> root)
        {
            string moddingPath = GetModdingPath(dir, "FloorLevelInfo");
            if (Directory.Exists(moddingPath))
            {
                LoadFloorInfo_MOD_Checking(new DirectoryInfo(moddingPath), root);
            }
        }
        private static void LoadFloorInfo_MOD_Checking(DirectoryInfo dir, List<FloorLevelXmlInfo> root)
        {
            List<FloorLevelXmlInfo> list = new List<FloorLevelXmlInfo>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                list.AddRange(LoadNewFloorInfo(File.ReadAllText(fileInfo.FullName)).list);
            }
            foreach (FloorLevelXmlInfo floorLevelXmlInfo in list)
            {
                bool flag = false;
                FloorLevelXmlInfo item = null;
                foreach (FloorLevelXmlInfo floorLevelXmlInfo2 in root)
                {
                    if (floorLevelXmlInfo2.stageId == floorLevelXmlInfo.stageId)
                    {
                        flag = true;
                        item = floorLevelXmlInfo2;
                        break;
                    }
                }
                if (flag)
                {
                    root.Remove(item);
                }
                root.Add(floorLevelXmlInfo);
            }
        }
        private static FloorLevelXmlRoot LoadNewFloorInfo(string str)
        {
            FloorLevelXmlRoot result;
            using (StringReader stringReader = new StringReader(str))
            {
                result = (FloorLevelXmlRoot)new XmlSerializer(typeof(FloorLevelXmlRoot)).Deserialize(stringReader);
            }
            return result;
        }
    }
}

namespace BaseMod
{
    public class StorySerializer_new
    {
        public static void StoryTextExport(string path, string outpath, string outname)
        {
            string text = Resources.Load<TextAsset>(path).text;
            Directory.CreateDirectory(outpath);
            File.WriteAllText(outpath + "/" + outname + ".txt", text);
        }
        public static void StoryTextExport_str(string str, string outpath, string outname)
        {
            Directory.CreateDirectory(outpath);
            File.WriteAllText(outpath + "/" + outname + ".txt", str);
        }
        public static void ExportStory()
        {
            try
            {
                TextAsset[] array = Resources.LoadAll<TextAsset>("Xml/Story/StoryEffect/");
                for (int i = 0; i < array.Length; i++)
                {
                    StoryTextExport_str(array[i].text, Harmony_Patch.StoryPath_Static, array[i].name);
                }
                TextAsset[] array2 = Resources.LoadAll<TextAsset>("Xml/Story/" + TextDataModel.CurrentLanguage);
                for (int j = 0; j < array2.Length; j++)
                {
                    StoryTextExport_str(array2[j].text, Harmony_Patch.StoryPath_Localize, array2[j].name);
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/SSLSerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}
namespace BaseMod
{
    public class SkeletonJSON_new
    {
        public static void ReadAnimation_new(SkeletonJson __instance, Dictionary<string, object> map, string name, SkeletonData skeletonData)
        {
            float num = 1f;
            ExposedList<Timeline> exposedList = new ExposedList<Timeline>();
            float num2 = 0f;
            bool flag = map.ContainsKey("slots");
            if (flag)
            {
                foreach (KeyValuePair<string, object> keyValuePair in ((Dictionary<string, object>)map["slots"]))
                {
                    string key = keyValuePair.Key;
                    int slotIndex = skeletonData.FindSlotIndex(key);
                    foreach (KeyValuePair<string, object> keyValuePair2 in ((Dictionary<string, object>)keyValuePair.Value))
                    {
                        List<object> list = (List<object>)keyValuePair2.Value;
                        string key2 = keyValuePair2.Key;
                        if (key2 == "attachment")
                        {
                            AttachmentTimeline attachmentTimeline = new AttachmentTimeline(list.Count)
                            {
                                SlotIndex = slotIndex
                            };
                            int num3 = 0;
                            foreach (object obj in list)
                            {
                                Dictionary<string, object> dictionary = (Dictionary<string, object>)obj;
                                float time = (float)dictionary["time"];
                                attachmentTimeline.SetFrame(num3++, time, (string)dictionary["name"]);
                            }
                            exposedList.Add(attachmentTimeline);
                            num2 = Math.Max(num2, attachmentTimeline.Frames[attachmentTimeline.FrameCount - 1]);
                        }
                        else
                        {
                            if (key2 == "color")
                            {
                                ColorTimeline colorTimeline = new ColorTimeline(list.Count)
                                {
                                    SlotIndex = slotIndex
                                };
                                int num4 = 0;
                                foreach (object obj2 in list)
                                {
                                    Dictionary<string, object> dictionary2 = (Dictionary<string, object>)obj2;
                                    float time2 = (float)dictionary2["time"];
                                    string hexString = (string)dictionary2["color"];
                                    colorTimeline.SetFrame(num4, time2, ToColor(hexString, 0, 8), ToColor(hexString, 1, 8), ToColor(hexString, 2, 8), ToColor(hexString, 3, 8));
                                    ReadCurve(dictionary2, colorTimeline, num4);
                                    num4++;
                                }
                                exposedList.Add(colorTimeline);
                                num2 = Math.Max(num2, colorTimeline.Frames[(colorTimeline.FrameCount - 1) * 5]);
                            }
                            else
                            {
                                if (!(key2 == "twoColor"))
                                {
                                    throw new Exception(string.Concat(new string[]
                                    {
                                        "Invalid timeline type for a slot: ",
                                        key2,
                                        " (",
                                        key,
                                        ")"
                                    }));
                                }
                                TwoColorTimeline twoColorTimeline = new TwoColorTimeline(list.Count)
                                {
                                    SlotIndex = slotIndex
                                };
                                int num5 = 0;
                                foreach (object obj3 in list)
                                {
                                    Dictionary<string, object> dictionary3 = (Dictionary<string, object>)obj3;
                                    float time3 = (float)dictionary3["time"];
                                    string hexString2 = (string)dictionary3["light"];
                                    string hexString3 = (string)dictionary3["dark"];
                                    twoColorTimeline.SetFrame(num5, time3, ToColor(hexString2, 0, 8), ToColor(hexString2, 1, 8), ToColor(hexString2, 2, 8), ToColor(hexString2, 3, 8), ToColor(hexString3, 0, 6), ToColor(hexString3, 1, 6), ToColor(hexString3, 2, 6));
                                    ReadCurve(dictionary3, twoColorTimeline, num5);
                                    num5++;
                                }
                                exposedList.Add(twoColorTimeline);
                                num2 = Math.Max(num2, twoColorTimeline.Frames[(twoColorTimeline.FrameCount - 1) * 8]);
                            }
                        }
                    }
                }
            }
            if (map.ContainsKey("bones"))
            {
                foreach (KeyValuePair<string, object> keyValuePair3 in ((Dictionary<string, object>)map["bones"]))
                {
                    string key3 = keyValuePair3.Key;
                    int num6 = skeletonData.FindBoneIndex(key3);
                    if (num6 == -1)
                    {
                        throw new Exception("Bone not found: " + key3);
                    }
                    foreach (KeyValuePair<string, object> keyValuePair4 in ((Dictionary<string, object>)keyValuePair3.Value))
                    {
                        List<object> list2 = (List<object>)keyValuePair4.Value;
                        string key4 = keyValuePair4.Key;
                        if (key4 == "rotate")
                        {
                            RotateTimeline rotateTimeline = new RotateTimeline(list2.Count)
                            {
                                BoneIndex = num6
                            };
                            int num7 = 0;
                            foreach (object obj4 in list2)
                            {
                                Dictionary<string, object> dictionary4 = (Dictionary<string, object>)obj4;
                                rotateTimeline.SetFrame(num7, (float)dictionary4["time"], (float)dictionary4["angle"]);
                                ReadCurve(dictionary4, rotateTimeline, num7);
                                num7++;
                            }
                            exposedList.Add(rotateTimeline);
                            num2 = Math.Max(num2, rotateTimeline.Frames[(rotateTimeline.FrameCount - 1) * 2]);
                        }
                        else
                        {
                            if (!(key4 == "translate") && !(key4 == "scale") && !(key4 == "shear"))
                            {
                                throw new Exception(string.Concat(new string[]
                                {
                                    "Invalid timeline type for a bone: ",
                                    key4,
                                    " (",
                                    key3,
                                    ")"
                                }));
                            }
                            float num8 = 1f;
                            bool flag9 = key4 == "scale";
                            TranslateTimeline translateTimeline;
                            if (flag9)
                            {
                                translateTimeline = new ScaleTimeline(list2.Count);
                            }
                            else
                            {
                                if (key4 == "shear")
                                {
                                    translateTimeline = new ShearTimeline(list2.Count);
                                }
                                else
                                {
                                    translateTimeline = new TranslateTimeline(list2.Count);
                                    num8 = num;
                                }
                            }
                            translateTimeline.BoneIndex = num6;
                            int num9 = 0;
                            foreach (object obj5 in list2)
                            {
                                Dictionary<string, object> dictionary5 = (Dictionary<string, object>)obj5;
                                float time4 = (float)dictionary5["time"];
                                float @float = GetFloat(dictionary5, "x", 0f);
                                float float2 = GetFloat(dictionary5, "y", 0f);
                                translateTimeline.SetFrame(num9, time4, @float * num8, float2 * num8);
                                ReadCurve(dictionary5, translateTimeline, num9);
                                num9++;
                            }
                            exposedList.Add(translateTimeline);
                            num2 = Math.Max(num2, translateTimeline.Frames[(translateTimeline.FrameCount - 1) * 3]);
                        }
                    }
                }
            }
            if (map.ContainsKey("ik"))
            {
                foreach (KeyValuePair<string, object> keyValuePair5 in ((Dictionary<string, object>)map["ik"]))
                {
                    IkConstraintData item = skeletonData.FindIkConstraint(keyValuePair5.Key);
                    List<object> list3 = (List<object>)keyValuePair5.Value;
                    IkConstraintTimeline ikConstraintTimeline = new IkConstraintTimeline(list3.Count)
                    {
                        IkConstraintIndex = skeletonData.IkConstraints.IndexOf(item)
                    };
                    int num10 = 0;
                    foreach (object obj6 in list3)
                    {
                        Dictionary<string, object> dictionary6 = (Dictionary<string, object>)obj6;
                        float time5 = (float)dictionary6["time"];
                        float float3 = GetFloat(dictionary6, "mix", 1f);
                        bool boolean = GetBoolean(dictionary6, "bendPositive", true);
                        ikConstraintTimeline.SetFrame(num10, time5, float3, 0f, (!boolean) ? -1 : 1, false, false);
                        ReadCurve(dictionary6, ikConstraintTimeline, num10);
                        num10++;
                    }
                    exposedList.Add(ikConstraintTimeline);
                    num2 = Math.Max(num2, ikConstraintTimeline.Frames[(ikConstraintTimeline.FrameCount - 1) * 3]);
                }
            }
            if (map.ContainsKey("transform"))
            {
                foreach (KeyValuePair<string, object> keyValuePair6 in ((Dictionary<string, object>)map["transform"]))
                {
                    TransformConstraintData item2 = skeletonData.FindTransformConstraint(keyValuePair6.Key);
                    List<object> list4 = (List<object>)keyValuePair6.Value;
                    TransformConstraintTimeline transformConstraintTimeline = new TransformConstraintTimeline(list4.Count)
                    {
                        TransformConstraintIndex = skeletonData.TransformConstraints.IndexOf(item2)
                    };
                    int num11 = 0;
                    foreach (object obj7 in list4)
                    {
                        Dictionary<string, object> dictionary7 = (Dictionary<string, object>)obj7;
                        float time6 = (float)dictionary7["time"];
                        float float4 = GetFloat(dictionary7, "rotateMix", 1f);
                        float float5 = GetFloat(dictionary7, "translateMix", 1f);
                        float float6 = GetFloat(dictionary7, "scaleMix", 1f);
                        float float7 = GetFloat(dictionary7, "shearMix", 1f);
                        transformConstraintTimeline.SetFrame(num11, time6, float4, float5, float6, float7);
                        ReadCurve(dictionary7, transformConstraintTimeline, num11);
                        num11++;
                    }
                    exposedList.Add(transformConstraintTimeline);
                    num2 = Math.Max(num2, transformConstraintTimeline.Frames[(transformConstraintTimeline.FrameCount - 1) * 5]);
                }
            }
            if (map.ContainsKey("paths"))
            {
                foreach (KeyValuePair<string, object> keyValuePair7 in ((Dictionary<string, object>)map["paths"]))
                {
                    int num12 = skeletonData.FindPathConstraintIndex(keyValuePair7.Key);
                    if (num12 == -1)
                    {
                        throw new Exception("Path constraint not found: " + keyValuePair7.Key);
                    }
                    PathConstraintData pathConstraintData = skeletonData.PathConstraints.Items[num12];
                    foreach (KeyValuePair<string, object> keyValuePair8 in ((Dictionary<string, object>)keyValuePair7.Value))
                    {
                        List<object> list5 = (List<object>)keyValuePair8.Value;
                        string key5 = keyValuePair8.Key;
                        if (key5 == "position" || key5 == "spacing")
                        {
                            float num13 = 1f;
                            PathConstraintPositionTimeline pathConstraintPositionTimeline;
                            if (key5 == "spacing")
                            {
                                pathConstraintPositionTimeline = new PathConstraintSpacingTimeline(list5.Count);
                                if (pathConstraintData.SpacingMode == SpacingMode.Length || pathConstraintData.SpacingMode == SpacingMode.Fixed)
                                {
                                    num13 = num;
                                }
                            }
                            else
                            {
                                pathConstraintPositionTimeline = new PathConstraintPositionTimeline(list5.Count);
                                if (pathConstraintData.PositionMode == PositionMode.Fixed)
                                {
                                    num13 = num;
                                }
                            }
                            pathConstraintPositionTimeline.PathConstraintIndex = num12;
                            int num14 = 0;
                            foreach (object obj8 in list5)
                            {
                                Dictionary<string, object> dictionary8 = (Dictionary<string, object>)obj8;
                                pathConstraintPositionTimeline.SetFrame(num14, (float)dictionary8["time"], GetFloat(dictionary8, key5, 0f) * num13);
                                ReadCurve(dictionary8, pathConstraintPositionTimeline, num14);
                                num14++;
                            }
                            exposedList.Add(pathConstraintPositionTimeline);
                            num2 = Math.Max(num2, pathConstraintPositionTimeline.Frames[(pathConstraintPositionTimeline.FrameCount - 1) * 2]);
                        }
                        else
                        {
                            if (key5 == "mix")
                            {
                                PathConstraintMixTimeline pathConstraintMixTimeline = new PathConstraintMixTimeline(list5.Count)
                                {
                                    PathConstraintIndex = num12
                                };
                                int num15 = 0;
                                foreach (object obj9 in list5)
                                {
                                    Dictionary<string, object> dictionary9 = (Dictionary<string, object>)obj9;
                                    pathConstraintMixTimeline.SetFrame(num15, (float)dictionary9["time"], GetFloat(dictionary9, "rotateMix", 1f), GetFloat(dictionary9, "translateMix", 1f));
                                    ReadCurve(dictionary9, pathConstraintMixTimeline, num15);
                                    num15++;
                                }
                                exposedList.Add(pathConstraintMixTimeline);
                                num2 = Math.Max(num2, pathConstraintMixTimeline.Frames[(pathConstraintMixTimeline.FrameCount - 1) * 3]);
                            }
                        }
                    }
                }
            }
            if (map.ContainsKey("deform"))
            {
                foreach (KeyValuePair<string, object> keyValuePair9 in ((Dictionary<string, object>)map["deform"]))
                {
                    Skin skin = skeletonData.FindSkin(keyValuePair9.Key);
                    foreach (KeyValuePair<string, object> keyValuePair10 in ((Dictionary<string, object>)keyValuePair9.Value))
                    {
                        int num16 = skeletonData.FindSlotIndex(keyValuePair10.Key);
                        if (num16 == -1)
                        {
                            throw new Exception("Slot not found: " + keyValuePair10.Key);
                        }
                        foreach (KeyValuePair<string, object> keyValuePair11 in ((Dictionary<string, object>)keyValuePair10.Value))
                        {
                            List<object> list6 = (List<object>)keyValuePair11.Value;
                            VertexAttachment vertexAttachment = (VertexAttachment)skin.GetAttachment(num16, keyValuePair11.Key);
                            if (vertexAttachment == null)
                            {
                                throw new Exception("Deform attachment not found: " + keyValuePair11.Key);
                            }
                            bool flag23 = vertexAttachment.Bones != null;
                            float[] vertices = vertexAttachment.Vertices;
                            int num17 = (!flag23) ? vertices.Length : (vertices.Length / 3 * 2);
                            DeformTimeline deformTimeline = new DeformTimeline(list6.Count)
                            {
                                SlotIndex = num16,
                                Attachment = vertexAttachment
                            };
                            int num18 = 0;
                            foreach (object obj10 in list6)
                            {
                                Dictionary<string, object> dictionary10 = (Dictionary<string, object>)obj10;
                                bool flag24 = !dictionary10.ContainsKey("vertices");
                                float[] array;
                                if (flag24)
                                {
                                    array = ((!flag23) ? vertices : new float[num17]);
                                }
                                else
                                {
                                    array = new float[num17];
                                    int @int = GetInt(dictionary10, "offset", 0);
                                    float[] floatArray = GetFloatArray(dictionary10, "vertices", 1f);
                                    Array.Copy(floatArray, 0, array, @int, floatArray.Length);
                                    if (num != 1f)
                                    {
                                        int i = @int;
                                        int num19 = i + floatArray.Length;
                                        while (i < num19)
                                        {
                                            array[i] *= num;
                                            i++;
                                        }
                                    }
                                    if (!flag23)
                                    {
                                        for (int j = 0; j < num17; j++)
                                        {
                                            array[j] += vertices[j];
                                        }
                                    }
                                }
                                deformTimeline.SetFrame(num18, (float)dictionary10["time"], array);
                                ReadCurve(dictionary10, deformTimeline, num18);
                                num18++;
                            }
                            exposedList.Add(deformTimeline);
                            num2 = Math.Max(num2, deformTimeline.Frames[deformTimeline.FrameCount - 1]);
                        }
                    }
                }
            }
            if (map.ContainsKey("drawOrder") || map.ContainsKey("draworder"))
            {
                List<object> list7 = (List<object>)map[(!map.ContainsKey("drawOrder")) ? "draworder" : "drawOrder"];
                DrawOrderTimeline drawOrderTimeline = new DrawOrderTimeline(list7.Count);
                int count = skeletonData.Slots.Count;
                int num20 = 0;
                foreach (object obj11 in list7)
                {
                    Dictionary<string, object> dictionary11 = (Dictionary<string, object>)obj11;
                    int[] array2 = null;
                    if (dictionary11.ContainsKey("offsets"))
                    {
                        array2 = new int[count];
                        for (int k = count - 1; k >= 0; k--)
                        {
                            array2[k] = -1;
                        }
                        List<object> list8 = (List<object>)dictionary11["offsets"];
                        int[] array3 = new int[count - list8.Count];
                        int l = 0;
                        int num21 = 0;
                        foreach (object obj12 in list8)
                        {
                            Dictionary<string, object> dictionary12 = (Dictionary<string, object>)obj12;
                            int num22 = skeletonData.FindSlotIndex((string)dictionary12["slot"]);
                            if (num22 == -1)
                            {
                                string str = "Slot not found: ";
                                object obj13 = dictionary12["slot"];
                                throw new Exception(str + (obj13?.ToString()));
                            }
                            while (l != num22)
                            {
                                array3[num21++] = l++;
                            }
                            int num23 = l + (int)((float)dictionary12["offset"]);
                            array2[num23] = l++;
                        }
                        while (l < count)
                        {
                            array3[num21++] = l++;
                        }
                        for (int m = count - 1; m >= 0; m--)
                        {
                            if (array2[m] == -1)
                            {
                                array2[m] = array3[--num21];
                            }
                        }
                    }
                    drawOrderTimeline.SetFrame(num20++, (float)dictionary11["time"], array2);
                }
                exposedList.Add(drawOrderTimeline);
                num2 = Math.Max(num2, drawOrderTimeline.Frames[drawOrderTimeline.FrameCount - 1]);
            }
            if (map.ContainsKey("events"))
            {
                List<object> list9 = (List<object>)map["events"];
                EventTimeline eventTimeline = new EventTimeline(list9.Count);
                int num24 = 0;
                foreach (object obj14 in list9)
                {
                    Dictionary<string, object> dictionary13 = (Dictionary<string, object>)obj14;
                    EventData eventData = skeletonData.FindEvent((string)dictionary13["name"]);
                    bool flag32 = eventData == null;
                    if (flag32)
                    {
                        string str2 = "Event not found: ";
                        object obj15 = dictionary13["name"];
                        throw new Exception(str2 + (obj15?.ToString()));
                    }
                    Event @event = new Event((float)dictionary13["time"], eventData)
                    {
                        Int = GetInt(dictionary13, "int", eventData.Int),
                        Float = GetFloat(dictionary13, "float", eventData.Float),
                        String = GetString(dictionary13, "string", eventData.String)
                    };
                    eventTimeline.SetFrame(num24++, @event);
                }
                exposedList.Add(eventTimeline);
                num2 = Math.Max(num2, eventTimeline.Frames[eventTimeline.FrameCount - 1]);
            }
            exposedList.TrimExcess();
            skeletonData.Animations.Add(new Spine.Animation(name, exposedList, num2));
        }
        public static void ReadAnimation(SkeletonJson __instance, Dictionary<string, object> map, string name, SkeletonData skeletonData)
        {
            float scale = __instance.Scale;
            ExposedList<Timeline> exposedList = new ExposedList<Timeline>();
            float num = 0f;
            if (map.ContainsKey("slots"))
            {
                foreach (KeyValuePair<string, object> keyValuePair in ((Dictionary<string, object>)map["slots"]))
                {
                    string key = keyValuePair.Key;
                    int slotIndex = skeletonData.FindSlotIndex(key);
                    foreach (KeyValuePair<string, object> keyValuePair2 in ((Dictionary<string, object>)keyValuePair.Value))
                    {
                        List<object> list = (List<object>)keyValuePair2.Value;
                        string key2 = keyValuePair2.Key;
                        if (key2 == "attachment")
                        {
                            AttachmentTimeline attachmentTimeline = new AttachmentTimeline(list.Count)
                            {
                                SlotIndex = slotIndex
                            };
                            int num2 = 0;
                            foreach (object obj in list)
                            {
                                Dictionary<string, object> dictionary = (Dictionary<string, object>)obj;
                                float @float = GetFloat(dictionary, "time", 0f);
                                attachmentTimeline.SetFrame(num2++, @float, (string)dictionary["name"]);
                            }
                            exposedList.Add(attachmentTimeline);
                            num = Math.Max(num, attachmentTimeline.Frames[attachmentTimeline.FrameCount - 1]);
                        }
                        else
                        {
                            if (key2 == "color")
                            {
                                ColorTimeline colorTimeline = new ColorTimeline(list.Count)
                                {
                                    SlotIndex = slotIndex
                                };
                                int num3 = 0;
                                foreach (object obj2 in list)
                                {
                                    Dictionary<string, object> dictionary2 = (Dictionary<string, object>)obj2;
                                    float float2 = GetFloat(dictionary2, "time", 0f);
                                    string hexString = (string)dictionary2["color"];
                                    colorTimeline.SetFrame(num3, float2, ToColor(hexString, 0, 8), ToColor(hexString, 1, 8), ToColor(hexString, 2, 8), ToColor(hexString, 3, 8));
                                    ReadCurve(dictionary2, colorTimeline, num3);
                                    num3++;
                                }
                                exposedList.Add(colorTimeline);
                                num = Math.Max(num, colorTimeline.Frames[(colorTimeline.FrameCount - 1) * 5]);
                            }
                            else
                            {
                                if (!(key2 == "twoColor"))
                                {
                                    throw new Exception(string.Concat(new string[]
                                    {
                                        "Invalid timeline type for a slot: ",
                                        key2,
                                        " (",
                                        key,
                                        ")"
                                    }));
                                }
                                TwoColorTimeline twoColorTimeline = new TwoColorTimeline(list.Count)
                                {
                                    SlotIndex = slotIndex
                                };
                                int num4 = 0;
                                foreach (object obj3 in list)
                                {
                                    Dictionary<string, object> dictionary3 = (Dictionary<string, object>)obj3;
                                    float float3 = GetFloat(dictionary3, "time", 0f);
                                    string hexString2 = (string)dictionary3["light"];
                                    string hexString3 = (string)dictionary3["dark"];
                                    twoColorTimeline.SetFrame(num4, float3, ToColor(hexString2, 0, 8), ToColor(hexString2, 1, 8), ToColor(hexString2, 2, 8), ToColor(hexString2, 3, 8), ToColor(hexString3, 0, 6), ToColor(hexString3, 1, 6), ToColor(hexString3, 2, 6));
                                    ReadCurve(dictionary3, twoColorTimeline, num4);
                                    num4++;
                                }
                                exposedList.Add(twoColorTimeline);
                                num = Math.Max(num, twoColorTimeline.Frames[(twoColorTimeline.FrameCount - 1) * 8]);
                            }
                        }
                    }
                }
            }
            if (map.ContainsKey("bones"))
            {
                foreach (KeyValuePair<string, object> keyValuePair3 in ((Dictionary<string, object>)map["bones"]))
                {
                    string key3 = keyValuePair3.Key;
                    int num5 = skeletonData.FindBoneIndex(key3);
                    bool flag6 = num5 == -1;
                    if (num5 == -1)
                    {
                        throw new Exception("Bone not found: " + key3);
                    }
                    foreach (KeyValuePair<string, object> keyValuePair4 in ((Dictionary<string, object>)keyValuePair3.Value))
                    {
                        List<object> list2 = (List<object>)keyValuePair4.Value;
                        string key4 = keyValuePair4.Key;
                        bool flag7 = key4 == "rotate";
                        if (key4 == "rotate")
                        {
                            RotateTimeline rotateTimeline = new RotateTimeline(list2.Count)
                            {
                                BoneIndex = num5
                            };
                            int num6 = 0;
                            foreach (object obj4 in list2)
                            {
                                Dictionary<string, object> dictionary4 = (Dictionary<string, object>)obj4;
                                rotateTimeline.SetFrame(num6, GetFloat(dictionary4, "time", 0f), GetFloat(dictionary4, "angle", 0f));
                                ReadCurve(dictionary4, rotateTimeline, num6);
                                num6++;
                            }
                            exposedList.Add(rotateTimeline);
                            num = Math.Max(num, rotateTimeline.Frames[(rotateTimeline.FrameCount - 1) * 2]);
                        }
                        else
                        {
                            bool flag8 = !(key4 == "translate") && !(key4 == "scale") && !(key4 == "shear");
                            if (!(key4 == "translate") && !(key4 == "scale") && !(key4 == "shear"))
                            {
                                throw new Exception(string.Concat(new string[]
                                {
                                    "Invalid timeline type for a bone: ",
                                    key4,
                                    " (",
                                    key3,
                                    ")"
                                }));
                            }
                            float num7 = 1f;
                            float defaultValue = 0f;
                            bool flag9 = key4 == "scale";
                            TranslateTimeline translateTimeline;
                            if (key4 == "scale")
                            {
                                translateTimeline = new ScaleTimeline(list2.Count);
                                defaultValue = 1f;
                            }
                            else
                            {
                                bool flag10 = key4 == "shear";
                                if (key4 == "shear")
                                {
                                    translateTimeline = new ShearTimeline(list2.Count);
                                }
                                else
                                {
                                    translateTimeline = new TranslateTimeline(list2.Count);
                                    num7 = scale;
                                }
                            }
                            translateTimeline.BoneIndex = num5;
                            int num8 = 0;
                            foreach (object obj5 in list2)
                            {
                                Dictionary<string, object> dictionary5 = (Dictionary<string, object>)obj5;
                                float float4 = GetFloat(dictionary5, "time", 0f);
                                float float5 = GetFloat(dictionary5, "x", defaultValue);
                                float float6 = GetFloat(dictionary5, "y", defaultValue);
                                translateTimeline.SetFrame(num8, float4, float5 * num7, float6 * num7);
                                ReadCurve(dictionary5, translateTimeline, num8);
                                num8++;
                            }
                            exposedList.Add(translateTimeline);
                            num = Math.Max(num, translateTimeline.Frames[(translateTimeline.FrameCount - 1) * 3]);
                        }
                    }
                }
            }
            if (map.ContainsKey("ik"))
            {
                foreach (KeyValuePair<string, object> keyValuePair5 in ((Dictionary<string, object>)map["ik"]))
                {
                    IkConstraintData item = skeletonData.FindIkConstraint(keyValuePair5.Key);
                    List<object> list3 = (List<object>)keyValuePair5.Value;
                    IkConstraintTimeline ikConstraintTimeline = new IkConstraintTimeline(list3.Count)
                    {
                        IkConstraintIndex = skeletonData.IkConstraints.IndexOf(item)
                    };
                    int num9 = 0;
                    foreach (object obj6 in list3)
                    {
                        Dictionary<string, object> dictionary6 = (Dictionary<string, object>)obj6;
                        ikConstraintTimeline.SetFrame(num9, GetFloat(dictionary6, "time", 0f), GetFloat(dictionary6, "mix", 1f), GetFloat(dictionary6, "softness", 0f) * scale, GetBoolean(dictionary6, "bendPositive", true) ? 1 : -1, GetBoolean(dictionary6, "compress", false), GetBoolean(dictionary6, "stretch", false));
                        ReadCurve(dictionary6, ikConstraintTimeline, num9);
                        num9++;
                    }
                    exposedList.Add(ikConstraintTimeline);
                    num = Math.Max(num, ikConstraintTimeline.Frames[(ikConstraintTimeline.FrameCount - 1) * 6]);
                }
            }
            if (map.ContainsKey("transform"))
            {
                foreach (KeyValuePair<string, object> keyValuePair6 in ((Dictionary<string, object>)map["transform"]))
                {
                    TransformConstraintData item2 = skeletonData.FindTransformConstraint(keyValuePair6.Key);
                    List<object> list4 = (List<object>)keyValuePair6.Value;
                    TransformConstraintTimeline transformConstraintTimeline = new TransformConstraintTimeline(list4.Count)
                    {
                        TransformConstraintIndex = skeletonData.TransformConstraints.IndexOf(item2)
                    };
                    int num10 = 0;
                    foreach (object obj7 in list4)
                    {
                        Dictionary<string, object> dictionary7 = (Dictionary<string, object>)obj7;
                        transformConstraintTimeline.SetFrame(num10, GetFloat(dictionary7, "time", 0f), GetFloat(dictionary7, "rotateMix", 1f), GetFloat(dictionary7, "translateMix", 1f), GetFloat(dictionary7, "scaleMix", 1f), GetFloat(dictionary7, "shearMix", 1f));
                        ReadCurve(dictionary7, transformConstraintTimeline, num10);
                        num10++;
                    }
                    exposedList.Add(transformConstraintTimeline);
                    num = Math.Max(num, transformConstraintTimeline.Frames[(transformConstraintTimeline.FrameCount - 1) * 5]);
                }
            }
            if (map.ContainsKey("path"))
            {
                foreach (KeyValuePair<string, object> keyValuePair7 in ((Dictionary<string, object>)map["path"]))
                {
                    int num11 = skeletonData.FindPathConstraintIndex(keyValuePair7.Key);
                    bool flag14 = num11 == -1;
                    if (num11 == -1)
                    {
                        throw new Exception("Path constraint not found: " + keyValuePair7.Key);
                    }
                    PathConstraintData pathConstraintData = skeletonData.PathConstraints.Items[num11];
                    foreach (KeyValuePair<string, object> keyValuePair8 in ((Dictionary<string, object>)keyValuePair7.Value))
                    {
                        List<object> list5 = (List<object>)keyValuePair8.Value;
                        string key5 = keyValuePair8.Key;
                        bool flag15 = key5 == "position" || key5 == "spacing";
                        if (key5 == "position" || key5 == "spacing")
                        {
                            float num12 = 1f;
                            bool flag16 = key5 == "spacing";
                            PathConstraintPositionTimeline pathConstraintPositionTimeline;
                            if (key5 == "spacing")
                            {
                                pathConstraintPositionTimeline = new PathConstraintSpacingTimeline(list5.Count);
                                bool flag17 = pathConstraintData.SpacingMode == SpacingMode.Length || pathConstraintData.SpacingMode == SpacingMode.Fixed;
                                if (pathConstraintData.SpacingMode == SpacingMode.Length || pathConstraintData.SpacingMode == SpacingMode.Fixed)
                                {
                                    num12 = scale;
                                }
                            }
                            else
                            {
                                pathConstraintPositionTimeline = new PathConstraintPositionTimeline(list5.Count);
                                bool flag18 = pathConstraintData.PositionMode == PositionMode.Fixed;
                                if (pathConstraintData.PositionMode == PositionMode.Fixed)
                                {
                                    num12 = scale;
                                }
                            }
                            pathConstraintPositionTimeline.PathConstraintIndex = num11;
                            int num13 = 0;
                            foreach (object obj8 in list5)
                            {
                                Dictionary<string, object> dictionary8 = (Dictionary<string, object>)obj8;
                                pathConstraintPositionTimeline.SetFrame(num13, GetFloat(dictionary8, "time", 0f), GetFloat(dictionary8, key5, 0f) * num12);
                                ReadCurve(dictionary8, pathConstraintPositionTimeline, num13);
                                num13++;
                            }
                            exposedList.Add(pathConstraintPositionTimeline);
                            num = Math.Max(num, pathConstraintPositionTimeline.Frames[(pathConstraintPositionTimeline.FrameCount - 1) * 2]);
                        }
                        else
                        {
                            bool flag19 = key5 == "mix";
                            if (key5 == "mix")
                            {
                                PathConstraintMixTimeline pathConstraintMixTimeline = new PathConstraintMixTimeline(list5.Count)
                                {
                                    PathConstraintIndex = num11
                                };
                                int num14 = 0;
                                foreach (object obj9 in list5)
                                {
                                    Dictionary<string, object> dictionary9 = (Dictionary<string, object>)obj9;
                                    pathConstraintMixTimeline.SetFrame(num14, GetFloat(dictionary9, "time", 0f), GetFloat(dictionary9, "rotateMix", 1f), GetFloat(dictionary9, "translateMix", 1f));
                                    ReadCurve(dictionary9, pathConstraintMixTimeline, num14);
                                    num14++;
                                }
                                exposedList.Add(pathConstraintMixTimeline);
                                num = Math.Max(num, pathConstraintMixTimeline.Frames[(pathConstraintMixTimeline.FrameCount - 1) * 3]);
                            }
                        }
                    }
                }
            }
            if (map.ContainsKey("deform"))
            {
                foreach (KeyValuePair<string, object> keyValuePair9 in ((Dictionary<string, object>)map["deform"]))
                {
                    Skin skin = skeletonData.FindSkin(keyValuePair9.Key);
                    foreach (KeyValuePair<string, object> keyValuePair10 in ((Dictionary<string, object>)keyValuePair9.Value))
                    {
                        int num15 = skeletonData.FindSlotIndex(keyValuePair10.Key);
                        bool flag21 = num15 == -1;
                        if (num15 == -1)
                        {
                            throw new Exception("Slot not found: " + keyValuePair10.Key);
                        }
                        foreach (KeyValuePair<string, object> keyValuePair11 in ((Dictionary<string, object>)keyValuePair10.Value))
                        {
                            List<object> list6 = (List<object>)keyValuePair11.Value;
                            VertexAttachment vertexAttachment = (VertexAttachment)skin.GetAttachment(num15, keyValuePair11.Key);
                            bool flag22 = vertexAttachment == null;
                            if (vertexAttachment == null)
                            {
                                throw new Exception("Deform attachment not found: " + keyValuePair11.Key);
                            }
                            bool flag23 = vertexAttachment.Bones != null;
                            float[] vertices = vertexAttachment.Vertices;
                            int num16 = flag23 ? (vertices.Length / 3 * 2) : vertices.Length;
                            DeformTimeline deformTimeline = new DeformTimeline(list6.Count)
                            {
                                SlotIndex = num15,
                                Attachment = vertexAttachment
                            };
                            int num17 = 0;
                            foreach (object obj10 in list6)
                            {
                                Dictionary<string, object> dictionary10 = (Dictionary<string, object>)obj10;
                                bool flag24 = !dictionary10.ContainsKey("vertices");
                                float[] array;
                                if (flag24)
                                {
                                    array = (flag23 ? new float[num16] : vertices);
                                }
                                else
                                {
                                    array = new float[num16];
                                    int @int = GetInt(dictionary10, "offset", 0);
                                    float[] floatArray = GetFloatArray(dictionary10, "vertices", 1f);
                                    Array.Copy(floatArray, 0, array, @int, floatArray.Length);
                                    bool flag25 = scale != 1f;
                                    if (scale != 1f)
                                    {
                                        int i = @int;
                                        int num18 = i + floatArray.Length;
                                        while (i < num18)
                                        {
                                            array[i] *= scale;
                                            i++;
                                        }
                                    }
                                    bool flag26 = !flag23;
                                    if (!flag23)
                                    {
                                        for (int j = 0; j < num16; j++)
                                        {
                                            array[j] += vertices[j];
                                        }
                                    }
                                }
                                deformTimeline.SetFrame(num17, GetFloat(dictionary10, "time", 0f), array);
                                ReadCurve(dictionary10, deformTimeline, num17);
                                num17++;
                            }
                            exposedList.Add(deformTimeline);
                            num = Math.Max(num, deformTimeline.Frames[deformTimeline.FrameCount - 1]);
                        }
                    }
                }
            }
            if (map.ContainsKey("drawOrder") || map.ContainsKey("draworder"))
            {
                List<object> list7 = (List<object>)map[map.ContainsKey("drawOrder") ? "drawOrder" : "draworder"];
                DrawOrderTimeline drawOrderTimeline = new DrawOrderTimeline(list7.Count);
                int count = skeletonData.Slots.Count;
                int num19 = 0;
                foreach (object obj11 in list7)
                {
                    Dictionary<string, object> dictionary11 = (Dictionary<string, object>)obj11;
                    int[] array2 = null;
                    bool flag28 = dictionary11.ContainsKey("offsets");
                    if (dictionary11.ContainsKey("offsets"))
                    {
                        array2 = new int[count];
                        for (int k = count - 1; k >= 0; k--)
                        {
                            array2[k] = -1;
                        }
                        List<object> list8 = (List<object>)dictionary11["offsets"];
                        int[] array3 = new int[count - list8.Count];
                        int num20 = 0;
                        int num21 = 0;
                        foreach (object obj12 in list8)
                        {
                            Dictionary<string, object> dictionary12 = (Dictionary<string, object>)obj12;
                            int num22 = skeletonData.FindSlotIndex((string)dictionary12["slot"]);
                            bool flag29 = num22 == -1;
                            if (num22 == -1)
                            {
                                string str = "Slot not found: ";
                                object obj13 = dictionary12["slot"];
                                throw new Exception(str + (obj13?.ToString()));
                            }
                            while (num20 != num22)
                            {
                                array3[num21++] = num20++;
                            }
                            int num23 = num20 + (int)((float)dictionary12["offset"]);
                            array2[num23] = num20++;
                        }
                        for (; ; )
                        {
                            bool flag30 = num20 >= count;
                            if (num20 >= count)
                            {
                                break;
                            }
                            array3[num21++] = num20++;
                        }
                        for (int l = count - 1; l >= 0; l--)
                        {
                            bool flag31 = array2[l] == -1;
                            if (array2[l] == -1)
                            {
                                array2[l] = array3[--num21];
                            }
                        }
                    }
                    drawOrderTimeline.SetFrame(num19++, GetFloat(dictionary11, "time", 0f), array2);
                }
                exposedList.Add(drawOrderTimeline);
                num = Math.Max(num, drawOrderTimeline.Frames[drawOrderTimeline.FrameCount - 1]);
            }
            if (map.ContainsKey("events"))
            {
                List<object> list9 = (List<object>)map["events"];
                EventTimeline eventTimeline = new EventTimeline(list9.Count);
                int num24 = 0;
                foreach (object obj14 in list9)
                {
                    Dictionary<string, object> dictionary13 = (Dictionary<string, object>)obj14;
                    EventData eventData = skeletonData.FindEvent((string)dictionary13["name"]);
                    bool flag33 = eventData == null;
                    if (eventData == null)
                    {
                        string str2 = "Event not found: ";
                        object obj15 = dictionary13["name"];
                        throw new Exception(str2 + (obj15?.ToString()));
                    }
                    Event @event = new Event(GetFloat(dictionary13, "time", 0f), eventData)
                    {
                        Int = GetInt(dictionary13, "int", eventData.Int),
                        Float = GetFloat(dictionary13, "float", eventData.Float),
                        String = GetString(dictionary13, "string", eventData.String)
                    };
                    bool flag34 = @event.Data.AudioPath != null;
                    if (@event.Data.AudioPath != null)
                    {
                        @event.Volume = GetFloat(dictionary13, "volume", eventData.Volume);
                        @event.Balance = GetFloat(dictionary13, "balance", eventData.Balance);
                    }
                    eventTimeline.SetFrame(num24++, @event);
                }
                exposedList.Add(eventTimeline);
                num = Math.Max(num, eventTimeline.Frames[eventTimeline.FrameCount - 1]);
            }
            exposedList.TrimExcess();
            skeletonData.Animations.Add(new Spine.Animation(name, exposedList, num));
        }
        public static void ReadVertices(SkeletonJson __instance, Dictionary<string, object> map, VertexAttachment attachment, int verticesLength)
        {
            attachment.WorldVerticesLength = verticesLength;
            float[] floatArray = GetFloatArray(map, "vertices", 1f);
            float scale = __instance.Scale;
            if (verticesLength == floatArray.Length)
            {
                if (scale != 1f)
                {
                    for (int i = 0; i < floatArray.Length; i++)
                    {
                        floatArray[i] *= scale;
                    }
                }
                attachment.Vertices = floatArray;
            }
            else
            {
                ExposedList<float> exposedList = new ExposedList<float>(verticesLength * 3 * 3);
                ExposedList<int> exposedList2 = new ExposedList<int>(verticesLength * 3);
                int j = 0;
                int num = floatArray.Length;
                while (j < num)
                {
                    int num2 = (int)floatArray[j++];
                    exposedList2.Add(num2);
                    int num3 = j + num2 * 4;
                    while (j < num3)
                    {
                        exposedList2.Add((int)floatArray[j]);
                        exposedList.Add(floatArray[j + 1] * __instance.Scale);
                        exposedList.Add(floatArray[j + 2] * __instance.Scale);
                        exposedList.Add(floatArray[j + 3]);
                        j += 4;
                    }
                }
                attachment.Bones = exposedList2.ToArray();
                attachment.Vertices = exposedList.ToArray();
            }
        }
        public static Attachment ReadAttachment(SkeletonJson __instance, Dictionary<string, object> map, Skin skin, int slotIndex, string name, SkeletonData skeletonData)
        {
            float scale = __instance.Scale;
            AttachmentLoader attachmentLoader;
            try
            {
                attachmentLoader = (AttachmentLoader)__instance.GetType().GetField("attachmentLoader", AccessTools.all).GetValue(__instance);
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/BaseMods/attachmenterror.log", ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
            name = GetString(map, "name", name);
            string @string = GetString(map, "type", "region");
            AttachmentType attachmentType = (AttachmentType)Enum.Parse(typeof(AttachmentType), @string, true);
            string string2 = GetString(map, "path", name);
            Attachment result;
            switch (attachmentType)
            {
                case AttachmentType.Region:
                    {
                        RegionAttachment regionAttachment = attachmentLoader.NewRegionAttachment(skin, name, string2);
                        if (regionAttachment == null)
                        {
                            result = null;
                        }
                        else
                        {
                            regionAttachment.Path = string2;
                            regionAttachment.X = GetFloat(map, "x", 0f) * scale;
                            regionAttachment.Y = GetFloat(map, "y", 0f) * scale;
                            regionAttachment.ScaleX = GetFloat(map, "scaleX", 1f);
                            regionAttachment.ScaleY = GetFloat(map, "scaleY", 1f);
                            regionAttachment.Rotation = GetFloat(map, "rotation", 0f);
                            regionAttachment.Width = GetFloat(map, "width", 32f) * scale;
                            regionAttachment.Height = GetFloat(map, "height", 32f) * scale;
                            if (map.ContainsKey("color"))
                            {
                                string hexString = (string)map["color"];
                                regionAttachment.R = ToColor(hexString, 0, 8);
                                regionAttachment.G = ToColor(hexString, 1, 8);
                                regionAttachment.B = ToColor(hexString, 2, 8);
                                regionAttachment.A = ToColor(hexString, 3, 8);
                            }
                            regionAttachment.UpdateOffset();
                            result = regionAttachment;
                        }
                        break;
                    }
                case AttachmentType.Boundingbox:
                    {
                        BoundingBoxAttachment boundingBoxAttachment = attachmentLoader.NewBoundingBoxAttachment(skin, name);
                        if (boundingBoxAttachment == null)
                        {
                            result = null;
                        }
                        else
                        {
                            ReadVertices(__instance, map, boundingBoxAttachment, GetInt(map, "vertexCount", 0) << 1);
                            result = boundingBoxAttachment;
                        }
                        break;
                    }
                case AttachmentType.Mesh:
                case AttachmentType.Linkedmesh:
                    {
                        MeshAttachment meshAttachment = attachmentLoader.NewMeshAttachment(skin, name, string2);
                        if (meshAttachment == null)
                        {
                            result = null;
                        }
                        else
                        {
                            meshAttachment.Path = string2;
                            if (map.ContainsKey("color"))
                            {
                                string hexString2 = (string)map["color"];
                                meshAttachment.R = ToColor(hexString2, 0, 8);
                                meshAttachment.G = ToColor(hexString2, 1, 8);
                                meshAttachment.B = ToColor(hexString2, 2, 8);
                                meshAttachment.A = ToColor(hexString2, 3, 8);
                            }
                            meshAttachment.Width = GetFloat(map, "width", 0f) * scale;
                            meshAttachment.Height = GetFloat(map, "height", 0f) * scale;
                            string string3 = GetString(map, "parent", null);
                            if (string3 != null)
                            {
                                linkedMeshes.Add(new LinkedMesh(meshAttachment, GetString(map, "skin", null), slotIndex, string3, GetBoolean(map, "deform", true)));
                                result = meshAttachment;
                            }
                            else
                            {
                                float[] floatArray = GetFloatArray(map, "uvs", 1f);
                                ReadVertices(__instance, map, meshAttachment, floatArray.Length);
                                meshAttachment.Triangles = GetIntArray(map, "triangles");
                                meshAttachment.RegionUVs = floatArray;
                                meshAttachment.UpdateUVs();
                                if (map.ContainsKey("hull"))
                                {
                                    meshAttachment.HullLength = GetInt(map, "hull", 0) * 2;
                                }
                                if (map.ContainsKey("edges"))
                                {
                                    meshAttachment.Edges = GetIntArray(map, "edges");
                                }
                                result = meshAttachment;
                            }
                        }
                        break;
                    }
                case AttachmentType.Path:
                    {
                        PathAttachment pathAttachment = attachmentLoader.NewPathAttachment(skin, name);
                        if (pathAttachment == null)
                        {
                            result = null;
                        }
                        else
                        {
                            pathAttachment.Closed = GetBoolean(map, "closed", false);
                            pathAttachment.ConstantSpeed = GetBoolean(map, "constantSpeed", true);
                            int @int = GetInt(map, "vertexCount", 0);
                            ReadVertices(__instance, map, pathAttachment, @int << 1);
                            pathAttachment.Lengths = GetFloatArray(map, "lengths", scale);
                            result = pathAttachment;
                        }
                        break;
                    }
                case AttachmentType.Point:
                    {
                        PointAttachment pointAttachment = attachmentLoader.NewPointAttachment(skin, name);
                        if (pointAttachment == null)
                        {
                            result = null;
                        }
                        else
                        {
                            pointAttachment.X = GetFloat(map, "x", 0f) * scale;
                            pointAttachment.Y = GetFloat(map, "y", 0f) * scale;
                            pointAttachment.Rotation = GetFloat(map, "rotation", 0f);
                            result = pointAttachment;
                        }
                        break;
                    }
                case AttachmentType.Clipping:
                    {
                        ClippingAttachment clippingAttachment = attachmentLoader.NewClippingAttachment(skin, name);
                        if (clippingAttachment == null)
                        {
                            result = null;
                        }
                        else
                        {
                            string string4 = GetString(map, "end", null);
                            if (string4 != null)
                            {
                                SlotData slotData = skeletonData.FindSlot(string4);
                                if (slotData == null)
                                {
                                    throw new Exception("Clipping end slot not found: " + string4);
                                }
                                clippingAttachment.EndSlot = slotData;
                            }
                            ReadVertices(__instance, map, clippingAttachment, GetInt(map, "vertexCount", 0) << 1);
                            result = clippingAttachment;
                        }
                        break;
                    }
                default:
                    result = null;
                    break;
            }
            return result;
        }
        public static Attachment ReadAttachment_new(SkeletonJson __instance, Dictionary<string, object> map, Skin skin, int slotIndex, string name, SkeletonData skeletonData)
        {
            AttachmentLoader attachmentLoader = (AttachmentLoader)__instance.GetType().GetField("attachmentLoader", AccessTools.all).GetValue(__instance);
            float scale = __instance.Scale;
            name = GetString(map, "name", name);
            string @string = GetString(map, "type", "region");
            AttachmentType attachmentType = (AttachmentType)Enum.Parse(typeof(AttachmentType), @string, true);
            string string2 = GetString(map, "path", name);
            Attachment result;
            switch (attachmentType)
            {
                case AttachmentType.Region:
                    {
                        RegionAttachment regionAttachment = attachmentLoader.NewRegionAttachment(skin, name, string2);
                        if (regionAttachment == null)
                        {
                            result = null;
                        }
                        else
                        {
                            regionAttachment.Path = string2;
                            regionAttachment.X = GetFloat(map, "x", 0f) * scale;
                            regionAttachment.Y = GetFloat(map, "y", 0f) * scale;
                            regionAttachment.ScaleX = GetFloat(map, "scaleX", 1f);
                            regionAttachment.ScaleY = GetFloat(map, "scaleY", 1f);
                            regionAttachment.Rotation = GetFloat(map, "rotation", 0f);
                            regionAttachment.Width = GetFloat(map, "width", 32f) * scale;
                            regionAttachment.Height = GetFloat(map, "height", 32f) * scale;
                            if (map.ContainsKey("color"))
                            {
                                string hexString = (string)map["color"];
                                regionAttachment.R = ToColor(hexString, 0, 8);
                                regionAttachment.G = ToColor(hexString, 1, 8);
                                regionAttachment.B = ToColor(hexString, 2, 8);
                                regionAttachment.A = ToColor(hexString, 3, 8);
                            }
                            regionAttachment.UpdateOffset();
                            result = regionAttachment;
                        }
                        break;
                    }
                case AttachmentType.Boundingbox:
                    {
                        BoundingBoxAttachment boundingBoxAttachment = attachmentLoader.NewBoundingBoxAttachment(skin, name);
                        if (boundingBoxAttachment == null)
                        {
                            result = null;
                        }
                        else
                        {
                            ReadVertices(__instance, map, boundingBoxAttachment, GetInt(map, "vertexCount", 0) << 1);
                            result = boundingBoxAttachment;
                        }
                        break;
                    }
                case AttachmentType.Mesh:
                case AttachmentType.Linkedmesh:
                    {
                        MeshAttachment meshAttachment = attachmentLoader.NewMeshAttachment(skin, name, string2);
                        if (meshAttachment == null)
                        {
                            result = null;
                        }
                        else
                        {
                            meshAttachment.Path = string2;
                            if (map.ContainsKey("color"))
                            {
                                string hexString2 = (string)map["color"];
                                meshAttachment.R = ToColor(hexString2, 0, 8);
                                meshAttachment.G = ToColor(hexString2, 1, 8);
                                meshAttachment.B = ToColor(hexString2, 2, 8);
                                meshAttachment.A = ToColor(hexString2, 3, 8);
                            }
                            meshAttachment.Width = GetFloat(map, "width", 0f) * scale;
                            meshAttachment.Height = GetFloat(map, "height", 0f) * scale;
                            string string3 = GetString(map, "parent", null);
                            if (string3 != null)
                            {
                                linkedMeshes.Add(new LinkedMesh(meshAttachment, GetString(map, "skin", null), slotIndex, string3, true));
                                result = meshAttachment;
                            }
                            else
                            {
                                float[] floatArray = GetFloatArray(map, "uvs", 1f);
                                ReadVertices(__instance, map, meshAttachment, floatArray.Length);
                                meshAttachment.Triangles = GetIntArray(map, "triangles");
                                meshAttachment.RegionUVs = floatArray;
                                meshAttachment.UpdateUVs();
                                if (map.ContainsKey("hull"))
                                {
                                    meshAttachment.HullLength = GetInt(map, "hull", 0) * 2;
                                }
                                if (map.ContainsKey("edges"))
                                {
                                    meshAttachment.Edges = GetIntArray(map, "edges");
                                }
                                result = meshAttachment;
                            }
                        }
                        break;
                    }
                case AttachmentType.Path:
                    {
                        PathAttachment pathAttachment = attachmentLoader.NewPathAttachment(skin, name);
                        if (pathAttachment == null)
                        {
                            result = null;
                        }
                        else
                        {
                            pathAttachment.Closed = GetBoolean(map, "closed", false);
                            pathAttachment.ConstantSpeed = GetBoolean(map, "constantSpeed", true);
                            int @int = GetInt(map, "vertexCount", 0);
                            ReadVertices(__instance, map, pathAttachment, @int << 1);
                            pathAttachment.Lengths = GetFloatArray(map, "lengths", scale);
                            result = pathAttachment;
                        }
                        break;
                    }
                case AttachmentType.Point:
                    {
                        PointAttachment pointAttachment = attachmentLoader.NewPointAttachment(skin, name);
                        if (pointAttachment == null)
                        {
                            result = null;
                        }
                        else
                        {
                            pointAttachment.X = GetFloat(map, "x", 0f) * scale;
                            pointAttachment.Y = GetFloat(map, "y", 0f) * scale;
                            pointAttachment.Rotation = GetFloat(map, "rotation", 0f);
                            result = pointAttachment;
                        }
                        break;
                    }
                case AttachmentType.Clipping:
                    {
                        ClippingAttachment clippingAttachment = attachmentLoader.NewClippingAttachment(skin, name);
                        if (clippingAttachment == null)
                        {
                            result = null;
                        }
                        else
                        {
                            string string4 = GetString(map, "end", null);
                            if (string4 != null)
                            {
                                SlotData slotData = skeletonData.FindSlot(string4);
                                if (slotData == null)
                                {
                                    throw new Exception("Clipping end slot not found: " + string4);
                                }
                                clippingAttachment.EndSlot = slotData;
                            }
                            ReadVertices(__instance, map, clippingAttachment, GetInt(map, "vertexCount", 0) << 1);
                            result = clippingAttachment;
                        }
                        break;
                    }
                default:
                    result = null;
                    break;
            }
            return result;
        }
        public static float GetFloat(Dictionary<string, object> map, string name, float defaultValue)
        {
            float result;
            if (!map.ContainsKey(name))
            {
                result = defaultValue;
            }
            else
            {
                result = (float)map[name];
            }
            return result;
        }
        public static int GetInt(Dictionary<string, object> map, string name, int defaultValue)
        {
            int result;
            if (!map.ContainsKey(name))
            {
                result = defaultValue;
            }
            else
            {
                result = (int)((float)map[name]);
            }
            return result;
        }
        public static bool GetBoolean(Dictionary<string, object> map, string name, bool defaultValue)
        {
            bool result;
            if (!map.ContainsKey(name))
            {
                result = defaultValue;
            }
            else
            {
                result = (bool)map[name];
            }
            return result;
        }
        public static string GetString(Dictionary<string, object> map, string name, string defaultValue)
        {
            string result;
            if (!map.ContainsKey(name))
            {
                result = defaultValue;
            }
            else
            {
                result = (string)map[name];
            }
            return result;
        }
        public static float[] GetFloatArray(Dictionary<string, object> map, string name, float scale)
        {
            List<object> list = (List<object>)map[name];
            float[] array = new float[list.Count];
            if (scale == 1f)
            {
                int i = 0;
                int count = list.Count;
                while (i < count)
                {
                    array[i] = (float)list[i];
                    i++;
                }
            }
            else
            {
                int j = 0;
                int count2 = list.Count;
                while (j < count2)
                {
                    array[j] = (float)list[j] * scale;
                    j++;
                }
            }
            return array;
        }
        public static int[] GetIntArray(Dictionary<string, object> map, string name)
        {
            List<object> list = (List<object>)map[name];
            int[] array = new int[list.Count];
            int i = 0;
            int count = list.Count;
            while (i < count)
            {
                array[i] = (int)((float)list[i]);
                i++;
            }
            return array;
        }
        public static void ReadCurve(Dictionary<string, object> valueMap, CurveTimeline timeline, int frameIndex)
        {
            if (valueMap.ContainsKey("curve"))
            {
                object obj = valueMap["curve"];
                if (obj is string)
                {
                    timeline.SetStepped(frameIndex);
                }
                else
                {
                    timeline.SetCurve(frameIndex, (float)obj, GetFloat(valueMap, "c2", 0f), GetFloat(valueMap, "c3", 1f), GetFloat(valueMap, "c4", 1f));
                }
            }
        }
        public static float ToColor(string hexString, int colorIndex, int expectedLength = 8)
        {
            if (hexString.Length != expectedLength)
            {
                throw new ArgumentException(string.Concat(new object[]
                {
                    "Color hexidecimal length must be ",
                    expectedLength,
                    ", recieved: ",
                    hexString
                }), "hexString");
            }
            return Convert.ToInt32(hexString.Substring(colorIndex * 2, 2), 16) / 255f;
        }
        static SkeletonJSON_new()
        {
            linkedMeshes = new List<LinkedMesh>();
        }
        public static List<LinkedMesh> linkedMeshes;

        public class LinkedMesh
        {
            public LinkedMesh(MeshAttachment mesh, string skin, int slotIndex, string parent, bool inheritDeform)
            {
                this.mesh = mesh;
                this.skin = skin;
                this.slotIndex = slotIndex;
                this.parent = parent;
                this.inheritDeform = inheritDeform;
            }

            internal string parent;

            internal string skin;

            internal int slotIndex;

            internal MeshAttachment mesh;

            internal bool inheritDeform;
        }
    }
}

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
        private static void Debug(GameObject UIPanel)
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
            if (string.IsNullOrEmpty(input))
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
        private static string GetTransformPath(Transform transform, bool includeSelf = false)
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

namespace BaseMod
{
    public class UtilTools
    {
        public static Font _DefFont
        {
            get
            {
                if (DefFont == null)
                {
                    DefFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    DefFontColor = UIColorManager.Manager.GetUIColor(UIColor.Default);
                }
                return DefFont;
            }
        }
        public static Color _DefFontColor
        {
            get
            {
                return DefFontColor;
            }
        }
        public static InputField CreateInputField(Transform parent, string Imagepath, Vector2 position, TextAnchor tanchor, int fsize, Color tcolor, Font font)
        {
            GameObject gameObject = CreateImage(parent, Imagepath, new Vector2(1f, 1f), position).gameObject;
            Text text = CreateText(gameObject.transform, new Vector2(0f, 0f), fsize, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 0f), tanchor, tcolor, font);
            text.text = "";
            InputField inputField = gameObject.AddComponent<InputField>();
            inputField.targetGraphic = gameObject.GetComponent<Image>();
            inputField.textComponent = text;
            return inputField;
        }
        public static Button AddButton(Image target)
        {
            Button button = target.gameObject.AddComponent<Button>();
            button.targetGraphic = target;
            return button;
        }
        public static Image CreateImage(Transform parent, string Imagepath, Vector2 scale, Vector2 position)
        {
            GameObject gameObject = new GameObject("Image");
            Image image = gameObject.AddComponent<Image>();
            image.transform.SetParent(parent);
            Texture2D texture2D = new Texture2D(2, 2);
            texture2D.LoadImage(File.ReadAllBytes(Imagepath));
            Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
            image.sprite = sprite;
            image.rectTransform.sizeDelta = new Vector2(texture2D.width, texture2D.height);
            gameObject.SetActive(true);
            gameObject.transform.localScale = scale;
            gameObject.transform.localPosition = position;
            return image;
        }
        public static Image CreateImage(Transform parent, Sprite Image, Vector2 scale, Vector2 position)
        {
            GameObject gameObject = new GameObject("Image");
            Image image = gameObject.AddComponent<Image>();
            image.transform.SetParent(parent);
            new Texture2D(2, 2);
            image.sprite = Image;
            image.rectTransform.sizeDelta = new Vector2(Image.texture.width, Image.texture.height);
            gameObject.SetActive(true);
            gameObject.transform.localScale = scale;
            gameObject.transform.localPosition = position;
            return image;
        }
        public static Text CreateText(Transform target, Vector2 position, int fsize, Vector2 anchormin, Vector2 anchormax, Vector2 anchorposition, TextAnchor anchor, Color tcolor, Font font)
        {
            GameObject gameObject = new GameObject("Text");
            Text text = gameObject.AddComponent<Text>();
            gameObject.transform.SetParent(target);
            text.rectTransform.sizeDelta = Vector2.zero;
            text.rectTransform.anchorMin = anchormin;
            text.rectTransform.anchorMax = anchormax;
            text.rectTransform.anchoredPosition = anchorposition;
            text.text = " ";
            text.font = font;
            text.fontSize = fsize;
            text.color = tcolor;
            text.alignment = anchor;
            gameObject.transform.localScale = new Vector3(1f, 1f);
            gameObject.transform.localPosition = position;
            gameObject.SetActive(true);
            return text;
        }
        public static TextMeshProUGUI CreateText_TMP(Transform target, Vector2 position, int fsize, Vector2 anchormin, Vector2 anchormax, Vector2 anchorposition, TextAlignmentOptions anchor, Color tcolor, TMP_FontAsset font)
        {
            GameObject gameObject = new GameObject("Text");
            TextMeshProUGUI textMeshProUGUI = gameObject.AddComponent<TextMeshProUGUI>();
            gameObject.transform.SetParent(target);
            textMeshProUGUI.rectTransform.sizeDelta = Vector2.zero;
            textMeshProUGUI.rectTransform.anchorMin = anchormin;
            textMeshProUGUI.rectTransform.anchorMax = anchormax;
            textMeshProUGUI.rectTransform.anchoredPosition = anchorposition;
            textMeshProUGUI.text = " ";
            textMeshProUGUI.font = font;
            textMeshProUGUI.fontSize = fsize;
            textMeshProUGUI.color = tcolor;
            textMeshProUGUI.alignment = anchor;
            gameObject.transform.localScale = new Vector3(1f, 1f);
            gameObject.transform.localPosition = position;
            gameObject.SetActive(true);
            return textMeshProUGUI;
        }
        public static Text CreateText(Transform target)
        {
            return CreateText(target, new Vector2(0f, 0f), 10, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 0f), TextAnchor.UpperLeft, Color.black, DefFont);
        }
        public static Button CreateButton(Transform parent, string Imagepath, Vector2 scale, Vector2 position)
        {
            Image image = CreateImage(parent, Imagepath, scale, position);
            GameObject gameObject = image.gameObject;
            Button button = gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            return button;
        }
        public static Button CreateButton(Transform parent, Sprite Image, Vector2 scale, Vector2 position)
        {
            Image image = CreateImage(parent, Image, scale, position);
            Button button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            return button;
        }
        public static Button CreateButton(Transform parent, string Imagepath)
        {
            return CreateButton(parent, Imagepath, new Vector2(1f, 1f), new Vector2(0f, 0f));
        }
        public static UIOriginEquipPageSlot DuplicateEquipPageSlot(UIOriginEquipPageSlot origin, UIOriginEquipPageList parent)
        {
            UIOriginEquipPageSlot uioriginEquipPageSlot;
            if (origin is UISettingInvenEquipPageSlot slot)
            {
                uioriginEquipPageSlot = UnityEngine.Object.Instantiate(slot, origin.transform.parent);
            }
            else
            {
                uioriginEquipPageSlot = UnityEngine.Object.Instantiate((UIInvenEquipPageSlot)origin, origin.transform.parent);
            }
            Type typeFromHandle = typeof(UIOriginEquipPageSlot);
            RectTransform value = (RectTransform)uioriginEquipPageSlot.transform.GetChild(0);
            typeFromHandle.GetField("Pivot", AccessTools.all).SetValue(uioriginEquipPageSlot, value);
            CanvasGroup component = uioriginEquipPageSlot.GetComponent<CanvasGroup>();
            typeFromHandle.GetField("cg", AccessTools.all).SetValue(uioriginEquipPageSlot, component);
            if (uioriginEquipPageSlot is UISettingInvenEquipPageSlot)
            {
                Image component2 = uioriginEquipPageSlot.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(1).gameObject.GetComponent<Image>();
                uioriginEquipPageSlot.GetType().GetField("Frame", AccessTools.all).SetValue(uioriginEquipPageSlot, component2);
            }
            else
            {
                Image component2 = uioriginEquipPageSlot.transform.GetChild(0).GetChild(0).GetChild(2).GetChild(1).gameObject.GetComponent<Image>();
                uioriginEquipPageSlot.GetType().GetField("Frame", AccessTools.all).SetValue(uioriginEquipPageSlot, component2);
            }
            if (uioriginEquipPageSlot is UISettingInvenEquipPageSlot)
            {
                Image component3 = uioriginEquipPageSlot.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<Image>();
                uioriginEquipPageSlot.GetType().GetField("FrameGlow", AccessTools.all).SetValue(uioriginEquipPageSlot, component3);
            }
            else
            {
                Image component3 = uioriginEquipPageSlot.transform.GetChild(0).GetChild(0).GetChild(2).GetChild(0).gameObject.GetComponent<Image>();
                uioriginEquipPageSlot.GetType().GetField("FrameGlow", AccessTools.all).SetValue(uioriginEquipPageSlot, component3);
            }
            Image component4 = uioriginEquipPageSlot.transform.GetChild(0).GetChild(2).GetChild(1).gameObject.GetComponent<Image>();
            uioriginEquipPageSlot.GetType().GetField("Icon", AccessTools.all).SetValue(uioriginEquipPageSlot, component4);
            Image component5 = uioriginEquipPageSlot.transform.GetChild(0).GetChild(2).GetChild(0).gameObject.GetComponent<Image>();
            uioriginEquipPageSlot.GetType().GetField("IconGlow", AccessTools.all).SetValue(uioriginEquipPageSlot, component5);
            TextMeshProUGUI component6 = uioriginEquipPageSlot.transform.GetChild(0).GetChild(3).gameObject.GetComponent<TextMeshProUGUI>();
            uioriginEquipPageSlot.GetType().GetField("BookName", AccessTools.all).SetValue(uioriginEquipPageSlot, component6);
            TextMeshProMaterialSetter component7 = uioriginEquipPageSlot.transform.GetChild(0).GetChild(3).gameObject.GetComponent<TextMeshProMaterialSetter>();
            uioriginEquipPageSlot.GetType().GetField("setter_BookName", AccessTools.all).SetValue(uioriginEquipPageSlot, component7);
            if (uioriginEquipPageSlot is UISettingInvenEquipPageSlot)
            {
                UISettingInvenEquipPageSlot uisettingInvenEquipPageSlot = uioriginEquipPageSlot as UISettingInvenEquipPageSlot;
                Button component8 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(4).GetChild(2).GetChild(0).GetChild(0).gameObject.GetComponent<UIEquipPageOperatingButton>();
                uisettingInvenEquipPageSlot.GetType().GetField("button_BookMark", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component8);
                Button component9 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(4).GetChild(2).GetChild(4).GetChild(0).gameObject.GetComponent<UIEquipPageOperatingButton>();
                uisettingInvenEquipPageSlot.GetType().GetField("button_EmptyDeck", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component9);
                Button component10 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(4).GetChild(2).GetChild(2).GetChild(0).gameObject.GetComponent<UIEquipPageOperatingButton>();
                uisettingInvenEquipPageSlot.GetType().GetField("button_Equip", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component10);
                CanvasGroup component11 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(1).gameObject.GetComponent<CanvasGroup>();
                uisettingInvenEquipPageSlot.GetType().GetField("cg_equiproot", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component11);
                CanvasGroup component12 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(4).gameObject.GetComponent<CanvasGroup>();
                uisettingInvenEquipPageSlot.GetType().GetField("cg_operatingPanel", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component12);
                FaceEditor component13 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<FaceEditor>();
                uisettingInvenEquipPageSlot.GetType().GetField("faceEditor", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component13);
                Image component14 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(4).GetChild(2).GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Image>();
                uisettingInvenEquipPageSlot.GetType().GetField("img_bookmarkbuttonIcon", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component14);
                Image component15 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(4).GetChild(2).GetChild(2).GetChild(0).GetChild(0).gameObject.GetComponent<Image>();
                uisettingInvenEquipPageSlot.GetType().GetField("img_equipbuttonIcon", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component15);
                Image component16 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<Image>();
                uisettingInvenEquipPageSlot.GetType().GetField("img_equipFrame", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component16);
                UISettingEquipPageScrollList value2 = (UISettingEquipPageScrollList)origin.GetType().GetField("listRoot", AccessTools.all).GetValue(origin);
                uisettingInvenEquipPageSlot.GetType().GetField("listRoot", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, value2);
                GameObject gameObject = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(1).gameObject;
                uisettingInvenEquipPageSlot.GetType().GetField("ob_equipRoot", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, gameObject);
                GameObject gameObject2 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(4).gameObject;
                uisettingInvenEquipPageSlot.GetType().GetField("ob_OperatingPanel", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, gameObject2);
                TextMeshProUGUI component17 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(4).GetChild(2).GetChild(0).GetChild(0).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
                uisettingInvenEquipPageSlot.GetType().GetField("txt_bookmarkButton", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component17);
                TextMeshProUGUI component18 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(4).GetChild(2).GetChild(2).GetChild(0).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
                uisettingInvenEquipPageSlot.GetType().GetField("txt_equipButton", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component18);
            }
            else
            {
                UIInvenEquipPageSlot uiinvenEquipPageSlot = uioriginEquipPageSlot as UIInvenEquipPageSlot;
                object component19 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).GetChild(2).GetChild(0).GetChild(0).gameObject.GetComponent<UICustomGraphicObject>();
                uiinvenEquipPageSlot.GetType().GetField("button_BookMark", AccessTools.all).SetValue(uiinvenEquipPageSlot, component19);
                object component20 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).GetChild(2).GetChild(4).GetChild(0).gameObject.GetComponent<UICustomGraphicObject>();
                uiinvenEquipPageSlot.GetType().GetField("button_EmptyDeck", AccessTools.all).SetValue(uiinvenEquipPageSlot, component20);
                object component21 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).GetChild(2).GetChild(2).GetChild(0).gameObject.GetComponent<UICustomGraphicObject>();
                uiinvenEquipPageSlot.GetType().GetField("button_Equip", AccessTools.all).SetValue(uiinvenEquipPageSlot, component21);
                object component22 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).GetChild(2).GetChild(1).GetChild(0).gameObject.GetComponent<UICustomGraphicObject>();
                uiinvenEquipPageSlot.GetType().GetField("button_PassiveSuccession", AccessTools.all).SetValue(uiinvenEquipPageSlot, component22);
                object component23 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).GetChild(2).GetChild(3).GetChild(0).gameObject.GetComponent<UICustomGraphicObject>();
                uiinvenEquipPageSlot.GetType().GetField("button_ReleaseButton", AccessTools.all).SetValue(uiinvenEquipPageSlot, component23);
                CanvasGroup component24 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(1).gameObject.GetComponent<CanvasGroup>();
                uiinvenEquipPageSlot.GetType().GetField("cg_equiproot", AccessTools.all).SetValue(uiinvenEquipPageSlot, component24);
                CanvasGroup component25 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).gameObject.GetComponent<CanvasGroup>();
                uiinvenEquipPageSlot.GetType().GetField("cg_operatingPanel", AccessTools.all).SetValue(uiinvenEquipPageSlot, component25);
                FaceEditor component26 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<FaceEditor>();
                uiinvenEquipPageSlot.GetType().GetField("faceEditor", AccessTools.all).SetValue(uiinvenEquipPageSlot, component26);
                Image component27 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).GetChild(2).GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<Image>();
                uiinvenEquipPageSlot.GetType().GetField("img_bookmarkbuttonIcon", AccessTools.all).SetValue(uiinvenEquipPageSlot, component27);
                Image component28 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).GetChild(2).GetChild(2).GetChild(0).GetChild(1).gameObject.GetComponent<Image>();
                uiinvenEquipPageSlot.GetType().GetField("img_equipbuttonIcon", AccessTools.all).SetValue(uiinvenEquipPageSlot, component28);
                Image component29 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<Image>();
                uiinvenEquipPageSlot.GetType().GetField("img_equipFrame", AccessTools.all).SetValue(uiinvenEquipPageSlot, component29);
                UIEquipPageScrollList value3 = (UIEquipPageScrollList)origin.GetType().GetField("listRoot", AccessTools.all).GetValue(origin);
                uiinvenEquipPageSlot.GetType().GetField("listRoot", AccessTools.all).SetValue(uiinvenEquipPageSlot, value3);
                GameObject gameObject3 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(1).gameObject;
                uiinvenEquipPageSlot.GetType().GetField("ob_equipRoot", AccessTools.all).SetValue(uiinvenEquipPageSlot, gameObject3);
                GameObject gameObject4 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).gameObject;
                uiinvenEquipPageSlot.GetType().GetField("ob_OperatingPanel", AccessTools.all).SetValue(uiinvenEquipPageSlot, gameObject4);
                TextMeshProUGUI component30 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).GetChild(2).GetChild(0).GetChild(0).GetChild(3).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
                uiinvenEquipPageSlot.GetType().GetField("txt_bookmarkButton", AccessTools.all).SetValue(uiinvenEquipPageSlot, component30);
                TextMeshProUGUI component31 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).GetChild(2).GetChild(2).GetChild(0).GetChild(3).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
                uiinvenEquipPageSlot.GetType().GetField("txt_equipButton", AccessTools.all).SetValue(uiinvenEquipPageSlot, component31);
                TextMeshProUGUI component32 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).GetChild(2).GetChild(3).GetChild(0).GetChild(3).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
                uiinvenEquipPageSlot.GetType().GetField("txt_releaseButton", AccessTools.all).SetValue(uiinvenEquipPageSlot, component32);
            }
            return uioriginEquipPageSlot;
        }
        public static void LoadFromSaveData_GiftInventory(GiftInventory __instance, SaveData data)
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
        }
        public static void SpriteTrace(string path, Sprite sprite)
        {
            string text = sprite.name + Environment.NewLine;
            text = text + sprite.rect.ToString() + Environment.NewLine;
            text = text + sprite.border.ToString() + Environment.NewLine;
            text = text + sprite.pivot.ToString() + Environment.NewLine;
            File.WriteAllText(path, text);
        }
        public static DirectoryInfo FindExistDir(string path)
        {
            foreach (ModContent modContent in Harmony_Patch.LoadedModContents)
            {
                DirectoryInfo _dirInfo = modContent.GetType().GetField("_dirInfo", AccessTools.all).GetValue(modContent) as DirectoryInfo;
                if (Directory.Exists(_dirInfo.FullName + "/" + path))
                {
                    return new DirectoryInfo(_dirInfo.FullName + "/" + path);
                }
            }
            return null;
        }

        public static Font DefFont;

        public static TMP_FontAsset DefFont_TMP;

        public static Color DefFontColor;

        public class EmotionEgoXmlInfo2
        {
            [XmlAttribute("ID")]
            public int id;

            [XmlElement("Sephirah")]
            public SephirahType Sephirah;

            [XmlElement("Card")]
            public int CardId;

            [XmlElement("LockInBattle")]
            public bool isLock;
        }
        public class EmotionEgoXmlRoot2
        {
            [XmlElement("EmotionEgo")]
            public List<EmotionEgoXmlInfo2> egoXmlList;
        }
    }
}

namespace BaseMod
{
    public class WAV
    {
        private static float bytesToFloat(byte firstByte, byte secondByte)
        {
            short num = (short)(secondByte << 8 | firstByte);
            return num / 32768f;
        }
        private static int bytesToInt(byte[] bytes, int offset = 0)
        {
            int num = 0;
            for (int i = 0; i < 4; i++)
            {
                num |= bytes[offset + i] << i * 8;
            }
            return num;
        }
        private static byte[] GetBytes(string filename)
        {
            return File.ReadAllBytes(filename);
        }
        public float[] LeftChannel
        {
            get; internal set;
        }
        public float[] RightChannel
        {
            get; internal set;
        }
        public int ChannelCount
        {
            get; internal set;
        }
        public int SampleCount
        {
            get; internal set;
        }
        public int Frequency
        {
            get; internal set;
        }
        public WAV(string filename) : this(GetBytes(filename))
        {
        }
        public WAV(byte[] wav)
        {
            ChannelCount = wav[22];
            Frequency = bytesToInt(wav, 24);
            int i = 12;
            while (wav[i] != 100 || wav[i + 1] != 97 || wav[i + 2] != 116 || wav[i + 3] != 97)
            {
                i += 4;
                int num = wav[i] + wav[i + 1] * 256 + wav[i + 2] * 65536 + wav[i + 3] * 16777216;
                i += 4 + num;
            }
            i += 8;
            SampleCount = (wav.Length - i) / 2;
            bool flag = ChannelCount == 2;
            if (flag)
            {
                SampleCount /= 2;
            }
            LeftChannel = new float[SampleCount];
            bool flag2 = ChannelCount == 2;
            if (flag2)
            {
                RightChannel = new float[SampleCount];
            }
            else
            {
                RightChannel = null;
            }
            int num2 = 0;
            while (i < wav.Length)
            {
                LeftChannel[num2] = bytesToFloat(wav[i], wav[i + 1]);
                i += 2;
                bool flag3 = ChannelCount == 2;
                if (flag3)
                {
                    RightChannel[num2] = bytesToFloat(wav[i], wav[i + 1]);
                    i += 2;
                }
                num2++;
            }
        }
        public override string ToString()
        {
            return string.Format("[WAV: LeftChannel={0}, RightChannel={1}, ChannelCount={2}, SampleCount={3}, Frequency={4}]", new object[]
            {
                LeftChannel,
                RightChannel,
                ChannelCount,
                SampleCount,
                Frequency
            });
        }
    }
}
