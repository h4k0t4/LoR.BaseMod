using GTMDProjectMoon;
using LOR_DiceSystem;
using Mod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

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
            try
            {
                Dictionary<string, List<DiceCardXmlInfo>> _workshopCard = ItemXmlDataList.instance._workshopDict;
                List<DiceCardXmlInfo> _cardInfoList = ItemXmlDataList.instance._cardInfoList;
                Dictionary<LorId, DiceCardXmlInfo> _cardInfoTable = ItemXmlDataList.instance._cardInfoTable;
                List<DiceCardXmlInfo> _basicCardList = ItemXmlDataList.instance._basicCardList;
                List<DeckXmlInfo> DeckXml = Singleton<DeckXmlList>.Instance._list;
                Dictionary<string, List<BookXmlInfo>> _workshopBookDict = Singleton<BookXmlList>.Instance._workshopBookDict;
                List<BookXmlInfo> _equiplist = Singleton<BookXmlList>.Instance._list;
                Dictionary<LorId, BookXmlInfo> _equipdictionary = Singleton<BookXmlList>.Instance._dictionary;
                Dictionary<string, List<CardDropTableXmlInfo>> _workshopCardDropDict = Singleton<CardDropTableXmlList>.Instance._workshopDict;
                List<CardDropTableXmlInfo> _CardDroplist = Singleton<CardDropTableXmlList>.Instance._list;
                Dictionary<string, List<DropBookXmlInfo>> _workshopDropBookDict = Singleton<DropBookXmlList>.Instance._workshopDict;
                List<DropBookXmlInfo> _DropBooklist = Singleton<DropBookXmlList>.Instance._list;
                Dictionary<LorId, DropBookXmlInfo> _DropBookdictionary = Singleton<DropBookXmlList>.Instance._dict;
                Dictionary<string, List<EnemyUnitClassInfo>> _workshopEnemyDict = Singleton<EnemyUnitClassInfoList>.Instance._workshopEnemyDict;
                List<EnemyUnitClassInfo> _Enemylist = Singleton<EnemyUnitClassInfoList>.Instance._list;
                List<StageClassInfo> _Stagelist = Singleton<StageClassInfoList>.Instance._list;
                Dictionary<string, List<StageClassInfo>> _workshopStageDict = Singleton<StageClassInfoList>.Instance._workshopStageDict;
                List<StageClassInfo> _recipeCondList = Singleton<StageClassInfoList>.Instance._recipeCondList;
                Dictionary<int, List<StageClassInfo>> _valueCondList = Singleton<StageClassInfoList>.Instance._valueCondList;
                List<GiftXmlInfo> GiftXml = Singleton<GiftXmlList>.Instance._list;
                List<EmotionCardXmlInfo> EmotionCardXml = Singleton<EmotionCardXmlList>.Instance._list;
                List<EmotionEgoXmlInfo> EmotionEgoXml = Singleton<EmotionEgoXmlList>.Instance._list;
                List<ToolTipXmlInfo> ToolTipXml = Singleton<ToolTipXmlList>.Instance._list;
                List<TitleXmlInfo> prefixList = Singleton<TitleXmlList>.Instance._prefixList;
                List<TitleXmlInfo> postfixList = Singleton<TitleXmlList>.Instance._postfixList;
                List<FormationXmlInfo> FormationXml = Singleton<FormationXmlList>.Instance._list;
                List<QuestXmlInfo> QuestXml = Singleton<QuestXmlList>.Instance._list;
                List<FloorLevelXmlInfo> FloorLevelXml = Singleton<FloorLevelXmlList>.Instance._list;
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
                        string moddingPath = GetModdingPath(_dirInfo, "PassiveList");
                        DirectoryInfo directory = new DirectoryInfo(moddingPath);
                        if (Directory.Exists(moddingPath))
                        {
                            LoadPassive_MOD(directory, workshopId);
                        }
                        moddingPath = GetModdingPath(_dirInfo, "Card");
                        directory = new DirectoryInfo(moddingPath);
                        if (Directory.Exists(moddingPath))
                        {
                            LoadCard_MOD(directory, workshopId, _workshopCard, _cardInfoList, _cardInfoTable, _basicCardList);
                        }
                        moddingPath = GetModdingPath(_dirInfo, "Deck");
                        directory = new DirectoryInfo(moddingPath);
                        if (Directory.Exists(moddingPath))
                        {
                            LoadDeck_MOD(directory, workshopId, DeckXml);
                        }
                        moddingPath = GetModdingPath(_dirInfo, "EquipPage");
                        directory = new DirectoryInfo(moddingPath);
                        if (Directory.Exists(moddingPath))
                        {
                            LoadBook_MOD(directory, workshopId, _workshopBookDict, _equiplist, _equipdictionary);
                        }
                        moddingPath = GetModdingPath(_dirInfo, "CardDropTable");
                        directory = new DirectoryInfo(moddingPath);
                        if (Directory.Exists(moddingPath))
                        {
                            LoadCardDropTable_MOD(directory, workshopId, _workshopCardDropDict, _CardDroplist);
                        }
                        moddingPath = GetModdingPath(_dirInfo, "DropBook");
                        directory = new DirectoryInfo(moddingPath);
                        if (Directory.Exists(moddingPath))
                        {
                            LoadDropBook_MOD(directory, workshopId, _workshopDropBookDict, _DropBooklist, _DropBookdictionary);
                        }
                        moddingPath = GetModdingPath(_dirInfo, "GiftInfo");
                        directory = new DirectoryInfo(moddingPath);
                        if (Directory.Exists(moddingPath))
                        {
                            LoadGift_MOD(directory, GiftXml);
                        }
                        moddingPath = GetModdingPath(_dirInfo, "EmotionCard");
                        directory = new DirectoryInfo(moddingPath);
                        if (Directory.Exists(moddingPath))
                        {
                            LoadEmotionCard_MOD(directory, EmotionCardXml);
                        }
                        moddingPath = GetModdingPath(_dirInfo, "EmotionEgo");
                        directory = new DirectoryInfo(moddingPath);
                        if (Directory.Exists(moddingPath))
                        {
                            LoadEmotionEgo_MOD(directory, EmotionEgoXml, workshopId);
                        }
                        moddingPath = GetModdingPath(_dirInfo, "XmlToolTips");
                        directory = new DirectoryInfo(moddingPath);
                        if (Directory.Exists(moddingPath))
                        {
                            LoadToolTip_MOD(directory, ToolTipXml);
                        }
                        moddingPath = GetModdingPath(_dirInfo, "Titles");
                        directory = new DirectoryInfo(moddingPath);
                        if (Directory.Exists(moddingPath))
                        {
                            LoadTitle_MOD(directory, prefixList, postfixList);
                        }
                        moddingPath = GetModdingPath(_dirInfo, "FormationInfo");
                        directory = new DirectoryInfo(moddingPath);
                        if (Directory.Exists(moddingPath))
                        {
                            LoadFormation_MOD(directory, FormationXml);
                        }
                        moddingPath = GetModdingPath(_dirInfo, "QuestInfo");
                        directory = new DirectoryInfo(moddingPath);
                        if (Directory.Exists(moddingPath))
                        {
                            LoadQuest_MOD(directory, QuestXml);
                        }
                        moddingPath = GetModdingPath(_dirInfo, "EnemyUnitInfo");
                        directory = new DirectoryInfo(moddingPath);
                        if (Directory.Exists(moddingPath))
                        {
                            LoadEnemyUnit_MOD(directory, workshopId, _workshopEnemyDict, _Enemylist);
                        }
                        moddingPath = GetModdingPath(_dirInfo, "StageInfo");
                        directory = new DirectoryInfo(moddingPath);
                        if (Directory.Exists(moddingPath))
                        {
                            if (workshopId == "")
                            {
                                LoadStage_MODorigin(directory, _recipeCondList, _valueCondList, _Stagelist);
                            }
                            else
                            {
                                LoadStage_MOD(directory, workshopId, _workshopStageDict, _Stagelist);
                            }
                        }
                        moddingPath = GetModdingPath(_dirInfo, "FloorLevelInfo");
                        directory = new DirectoryInfo(moddingPath);
                        if (Directory.Exists(moddingPath))
                        {
                            LoadFloorInfo_MOD(directory, FloorLevelXml);
                        }
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
                File.WriteAllText(Application.dataPath + "/Mods/Loaderror.log", Environment.NewLine + exe.Message + Environment.NewLine + exe.StackTrace);
            }
        }
        private static void LoadPassive_MOD(DirectoryInfo dir, string uniqueId)
        {
            LoadPassive_MOD_Checking(dir, uniqueId);
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    LoadPassive_MOD(directories[i], uniqueId);
                }
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
            LoadCard_MOD_Checking(dir, uniqueId, _workshopDict, _cardInfoList, _cardInfoTable, _basicCardList);
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    LoadCard_MOD(directories[i], uniqueId, _workshopDict, _cardInfoList, _cardInfoTable, _basicCardList);
                }
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
        private static LOR_DiceSystem.DiceCardXmlRoot LoadNewCard(string str)
        {
            LOR_DiceSystem.DiceCardXmlRoot result = new LOR_DiceSystem.DiceCardXmlRoot()
            {
                cardXmlList = new List<LOR_DiceSystem.DiceCardXmlInfo>()
            };
            GTMDProjectMoon.DiceCardXmlRoot root;
            using (StringReader stringReader = new StringReader(str))
            {
                root = (GTMDProjectMoon.DiceCardXmlRoot)new XmlSerializer(typeof(GTMDProjectMoon.DiceCardXmlRoot)).Deserialize(stringReader);
            }
            foreach (DiceCardXmlInfo_New cardInfo_new in root.cardXmlList)
            {
                DiceCardXmlInfo cardInfo = new DiceCardXmlInfo().CopyDiceCardXmlInfo(cardInfo_new);
                result.cardXmlList.Add(cardInfo);
            }
            return result;
        }
        private static void LoadDeck_MOD(DirectoryInfo dir, string uniqueId, List<DeckXmlInfo> DeckXml)
        {
            LoadDeck_MOD_Checking(dir, uniqueId, DeckXml);
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    LoadDeck_MOD(directories[i], uniqueId, DeckXml);
                }
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
            LoadBook_MOD_Checking(dir, uniqueId, _workshopBookDict, _list, _dictionary);
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    LoadBook_MOD(directories[i], uniqueId, _workshopBookDict, _list, _dictionary);
                }
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
            foreach (BookXmlInfo_New bookXmlInfo_New in list)
            {
                bookXmlInfo_New.workshopID = uniqueId;
                if (!string.IsNullOrEmpty(bookXmlInfo_New.skinType))
                {
                    if (bookXmlInfo_New.skinType == "UNKNOWN")
                    {
                        bookXmlInfo_New.skinType = "Lor";
                    }
                    else if (bookXmlInfo_New.skinType == "CUSTOM")
                    {
                        bookXmlInfo_New.skinType = "Custom";
                    }
                    else if (bookXmlInfo_New.skinType == "LOR")
                    {
                        bookXmlInfo_New.skinType = "Lor";
                    }
                }
                else if (bookXmlInfo_New.CharacterSkin[0].StartsWith("Custom"))
                {
                    bookXmlInfo_New.skinType = "Custom";
                }
                else
                {
                    bookXmlInfo_New.skinType = "Lor";
                }
                LorId.InitializeLorIds(bookXmlInfo_New.EquipEffect._PassiveList, bookXmlInfo_New.EquipEffect.PassiveList, uniqueId);
                list2.Add(new BookXmlInfo().CopyBookXmlInfo(bookXmlInfo_New, uniqueId));
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
            LoadCardDropTable_MOD_Checking(dir, uniqueId, _workshopDict, _list);
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    LoadCardDropTable_MOD(directories[i], uniqueId, _workshopDict, _list);
                }
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
            LoadDropBook_MOD_Checking(dir, uniqueId, _workshopDict, _list, _dict);
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    LoadDropBook_MOD(directories[i], uniqueId, _workshopDict, _list, _dict);
                }
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
            LoadGift_MOD_Checking(dir, root);
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    LoadGift_MOD(directories[i], root);
                }
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
            LoadEmotionCard_MOD_Checking(dir, root);
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    LoadEmotionCard_MOD(directories[i], root);
                }
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
            LoadEmotionEgo_MOD_Checking(dir, root, workshopId);
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    LoadEmotionEgo_MOD(directories[i], root, workshopId);
                }
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
            LoadToolTip_MOD_Checking(dir, root);
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    LoadToolTip_MOD(directories[i], root);
                }
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
            LoadTitle_MOD_Checking(dir, root1, root2);
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    LoadTitle_MOD(directories[i], root1, root2);
                }
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
            LoadFormation_MOD_Checking(dir, root);
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    LoadFormation_MOD(directories[i], root);
                }
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
            LoadQuest_MOD_Checking(dir, root);
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    LoadQuest_MOD(directories[i], root);
                }
            }
        }
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
            LoadEnemyUnit_MOD_Checking(dir, uniqueId, _workshopEnemyDict, _list);
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    LoadEnemyUnit_MOD(directories[i], uniqueId, _workshopEnemyDict, _list);
                }
            }
        }
        private static void LoadEnemyUnit_MOD_Checking(DirectoryInfo dir, string uniqueId, Dictionary<string, List<EnemyUnitClassInfo>> _workshopEnemyDict, List<EnemyUnitClassInfo> _list)
        {
            List<EnemyUnitClassInfo> list = new List<EnemyUnitClassInfo>();
            foreach (FileInfo fileInfo in dir.GetFiles())
            {
                list.AddRange(LoadNewEnemyUnit(File.ReadAllText(fileInfo.FullName), uniqueId).list);
            }
            AddEnemyUnitByMod(uniqueId, list, _workshopEnemyDict, _list);
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
            }
            return result;
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
        private static void LoadStage_MOD(DirectoryInfo dir, string uniqueId, Dictionary<string, List<StageClassInfo>> _workshopStageDict, List<StageClassInfo> _list)
        {
            LoadStage_MOD_Checking(dir, uniqueId, _workshopStageDict, _list);
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    LoadStage_MOD(directories[i], uniqueId, _workshopStageDict, _list);
                }
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
            List<StageClassInfo> _workshopRecipeList = Singleton<StageClassInfoList>.Instance._workshopRecipeList;
            Dictionary<int, List<StageClassInfo>> _workshopValueDict = Singleton<StageClassInfoList>.Instance._workshopValueDict;
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
            LoadStage_MODorigin_Checking(dir, _recipeCondList, _valueCondList, _list);
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    LoadStage_MODorigin(directories[i], _recipeCondList, _valueCondList, _list);
                }
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
                        _recipeCondList.RemoveAll(x => x.id == stageClassInfo.id);
                        _recipeCondList.Add(stageClassInfo);
                    }
                    else if (stageClassInfo.invitationInfo.combine == StageCombineType.BookValue)
                    {
                        if (stageClassInfo.invitationInfo.bookNum >= 1 && stageClassInfo.invitationInfo.bookNum <= 3)
                        {
                            _valueCondList[stageClassInfo.invitationInfo.bookNum].RemoveAll(x => x.id == stageClassInfo.id);
                            _valueCondList[stageClassInfo.invitationInfo.bookNum].Add(stageClassInfo);
                        }
                        else
                        {
                            Debug.LogError("invalid bookNum");
                        }
                    }
                }
            }
            int comparison(StageClassInfo info1, StageClassInfo info2) => (int)(10f * (info2.invitationInfo.bookValue - info1.invitationInfo.bookValue));
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
            }
            return result;
        }
        private static void LoadFloorInfo_MOD(DirectoryInfo dir, List<FloorLevelXmlInfo> root)
        {
            LoadFloorInfo_MOD_Checking(dir, root);
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    LoadFloorInfo_MOD(directories[i], root);
                }
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
