using GTMDProjectMoon;
using LOR_DiceSystem;
using Mod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using static BaseMod.OptimizedReplacer;

namespace BaseMod
{
	public static class StaticDataLoader_New
	{
		static string GetModdingPath(DirectoryInfo dir, string type)
		{
			return dir.FullName + "/StaticInfo/" + type;
		}
		static bool ExistsModdingPath(DirectoryInfo dir, string type, out DirectoryInfo subDir)
		{
			var path = GetModdingPath(dir, type);
			if (Directory.Exists(path))
			{
				subDir = new DirectoryInfo(path);
				return true;
			}
			subDir = null;
			return false;
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
		static bool CheckReExportLock()
		{
			return File.Exists(Harmony_Patch.StaticPath + "/DeleteThisToExportStaticAgain");
		}
		static void CreateReExportLock()
		{
			File.WriteAllText(Harmony_Patch.StaticPath + "/DeleteThisToExportStaticAgain", "yes");
		}
		public static void ExportOriginalFiles()
		{
			try
			{
				if (!CheckReExportLock())
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
					CreateReExportLock();
				}
			}
			catch (Exception ex)
			{
				ModContentManager.Instance.AddErrorLog(ex.ToString());
				File.WriteAllText(Application.dataPath + "/Mods/ESDerror.log", ex.ToString());
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
		static void ExportCard()
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
		static void ExportDeck()
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
				List<ModContent> loadableMods = new List<ModContent>();
				foreach (var modContent in loadedContents)
				{
					var config = BasemodConfig.FindBasemodConfig(modContent._itemUniqueId);
					if (!config.IgnoreStaticFiles)
					{
						loadableMods.Add(modContent);
					}
				}
				LoadAllPassive_MOD(loadableMods);
				LoadAllCard_MOD(loadableMods);
				LoadAllDeck_MOD(loadableMods);
				LoadAllBook_MOD(loadableMods);
				LoadAllCardDropTable_MOD(loadableMods);
				LoadAllDropBook_MOD(loadableMods);
				LoadAllGiftAndTitle_MOD(loadableMods);
				LoadAllEmotionCard_MOD(loadableMods);
				LoadAllEmotionEgo_MOD(loadableMods);
				LoadAllToolTip_MOD(loadableMods);
				LoadAllFormation_MOD(loadableMods);
				LoadAllQuest_MOD(loadableMods);
				LoadAllEnemyUnit_MOD(loadableMods);
				LoadAllStage_MOD(loadableMods);
				LoadAllFloorInfo_MOD(loadableMods);
			}
			catch (Exception ex)
			{
				ModContentManager.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
				File.WriteAllText(Application.dataPath + "/Mods/LoadStaticInfoError.log", Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}


		static void LoadAllPassive_MOD(List<ModContent> mods)
		{
			TrackerDict<PassiveXmlInfo_V2> passiveDict = new TrackerDict<PassiveXmlInfo_V2>();
			foreach (ModContent modcontent in mods)
			{
				DirectoryInfo _dirInfo = modcontent._dirInfo;
				string workshopId = modcontent._itemUniqueId;
				if (workshopId.ToLower().EndsWith("@origin"))
				{
					workshopId = "";
				}
				try
				{
					if (ExistsModdingPath(_dirInfo, "PassiveList", out var directory))
					{
						LoadPassive_MOD(directory, workshopId, passiveDict);
					}
				}
				catch (Exception ex)
				{
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + _dirInfo.Name + "_StaticInfoPassiveError.log", ex.ToString());
				}
			}
			AddPassiveByMod(passiveDict);
		}
		static void LoadPassive_MOD(DirectoryInfo dir, string uniqueId, TrackerDict<PassiveXmlInfo_V2> dict)
		{
			LoadPassive_MOD_Checking(dir, uniqueId, dict);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadPassive_MOD(directories[i], uniqueId, dict);
				}
			}
		}
		static void LoadPassive_MOD_Checking(DirectoryInfo dir, string uniqueId, TrackerDict<PassiveXmlInfo_V2> dict)
		{
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					LoadNewPassive(File.ReadAllText(fileInfo.FullName), uniqueId, dict);
				}
				catch (Exception ex)
				{
					UtilTools.CreateShortcut(Application.dataPath + "/Mods/", "Error from  " + uniqueId + " " + fileInfo.Name, fileInfo.FullName, fileInfo.DirectoryName, "Error Xml Files");
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + "Error in " + uniqueId + " " + fileInfo.Name + ".log", ex.ToString());
				}
			}
		}
		static void LoadNewPassive(string str, string uniqueId, TrackerDict<PassiveXmlInfo_V2> dict)
		{
			GTMDProjectMoon.PassiveXmlRoot_V2 xmlRoot;
			using (StringReader stringReader = new StringReader(str))
			{
				xmlRoot = (GTMDProjectMoon.PassiveXmlRoot_V2)new XmlSerializer(typeof(GTMDProjectMoon.PassiveXmlRoot_V2), GTMDProjectMoon.PassiveXmlRoot_V2.Overrides).Deserialize(stringReader);
			}
			foreach (var passive in xmlRoot.list)
			{
				string newId = Tools.ClarifyWorkshopId(passive.workshopID, xmlRoot.customPid, uniqueId);
				passive.workshopID = newId;
				dict[passive.id] = new AddTracker<PassiveXmlInfo_V2>(passive);
				if (!string.IsNullOrWhiteSpace(passive.CustomInnerType))
				{
					passive.CustomInnerType = passive.CustomInnerType.Trim();
				}
				passive.CopyInnerType = LorId.MakeLorId(passive.CopyInnerTypeXml, "");
			}
		}
		static void AddPassiveByMod(TrackerDict<PassiveXmlInfo_V2> dict)
		{
			AddOrReplace(dict, PassiveXmlList.Instance._list, p => p.id);
			var dictAll = new Dictionary<LorId, PassiveXmlInfo>();
			foreach (var passive in PassiveXmlList.Instance._list)
			{
				dictAll[passive.id] = passive;
			}
			HashSet<int> usedInnerTypes = new HashSet<int>();
			int minCheckedId = MinInjectedId - 1;
			foreach (var passive in PassiveXmlList.Instance._list)
			{
				if (passive.InnerTypeId != -1)
				{
					if (passive is PassiveXmlInfo_V2 passiveNew && !string.IsNullOrWhiteSpace(passiveNew.CustomInnerType))
					{
						OrcTools.CustomInnerTypeDic[passiveNew.CustomInnerType] = passive.InnerTypeId;
					}
					else
					{
						usedInnerTypes.Add(passive.InnerTypeId);
					}
				}
			}
			foreach (var kvp in dict)
			{
				var passive = kvp.Value.element;
				if (passive.InnerTypeId == -1 && passive.CopyInnerType != LorId.None)
				{
					if (dictAll.TryGetValue(passive.CopyInnerType, out var oldPassive))
					{
						if (oldPassive.InnerTypeId != -1)
						{
							passive.InnerTypeId = oldPassive.InnerTypeId;
						}
						else
						{
							minCheckedId++;
							while (usedInnerTypes.Contains(minCheckedId))
							{
								minCheckedId++;
							}
							passive.InnerTypeId = minCheckedId;
							oldPassive.InnerTypeId = minCheckedId;
							if (!string.IsNullOrWhiteSpace(passive.CustomInnerType))
							{
								OrcTools.CustomInnerTypeDic[passive.CustomInnerType] = minCheckedId;
							}
							if (oldPassive is PassiveXmlInfo_V2 oldPassiveNew && !string.IsNullOrWhiteSpace(oldPassiveNew.CustomInnerType))
							{
								OrcTools.CustomInnerTypeDic[oldPassiveNew.CustomInnerType] = minCheckedId;
							}
						}
					}
				}
			}
			foreach (var kvp in dict)
			{
				var passive = kvp.Value.element;
				if (passive.InnerTypeId == -1 && !string.IsNullOrWhiteSpace(passive.CustomInnerType))
				{
					if (OrcTools.CustomInnerTypeDic.TryGetValue(passive.CustomInnerType, out var innerType))
					{
						passive.InnerTypeId = innerType;
					}
					else
					{
						minCheckedId++;
						while (usedInnerTypes.Contains(minCheckedId))
						{
							minCheckedId++;
						}
						passive.InnerTypeId = minCheckedId;
						OrcTools.CustomInnerTypeDic[passive.CustomInnerType] = minCheckedId;
					}
				}
			}
		}


		static void LoadAllCard_MOD(List<ModContent> mods)
		{
			TrackerDict<DiceCardXmlInfo> cardDict = new TrackerDict<DiceCardXmlInfo>();
			SplitTrackerDict<string, DiceCardXmlInfo> splitCardDict = new SplitTrackerDict<string, DiceCardXmlInfo>();
			foreach (ModContent modcontent in mods)
			{
				DirectoryInfo _dirInfo = modcontent._dirInfo;
				string workshopId = modcontent._itemUniqueId;
				if (workshopId.ToLower().EndsWith("@origin"))
				{
					workshopId = "";
				}
				try
				{
					if (ExistsModdingPath(_dirInfo, "Card", out var directory))
					{
						LoadCard_MOD(directory, workshopId, cardDict, splitCardDict);
					}
				}
				catch (Exception ex)
				{
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + _dirInfo.Name + "_StaticInfoCardError.log", ex.ToString());
				}
			}
			AddCardInfoByMod(cardDict, splitCardDict);
		}
		static void LoadCard_MOD(DirectoryInfo dir, string uniqueId, TrackerDict<DiceCardXmlInfo> dict, SplitTrackerDict<string, DiceCardXmlInfo> splitDict)
		{
			LoadCard_MOD_Checking(dir, uniqueId, dict, splitDict);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadCard_MOD(directories[i], uniqueId, dict, splitDict);
				}
			}
		}
		static void LoadCard_MOD_Checking(DirectoryInfo dir, string uniqueId, TrackerDict<DiceCardXmlInfo> dict, SplitTrackerDict<string, DiceCardXmlInfo> splitDict)
		{
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					LoadNewCard(File.ReadAllText(fileInfo.FullName), uniqueId, dict, splitDict);
				}
				catch (Exception ex)
				{
					UtilTools.CreateShortcut(Application.dataPath + "/Mods/", "Error from  " + uniqueId + " " + fileInfo.Name, fileInfo.FullName, fileInfo.DirectoryName, "Error Xml Files");
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + "Error in " + uniqueId + " " + fileInfo.Name + ".log", ex.ToString());
				}
			}
		}
		static void LoadNewCard(string str, string uniqueId, TrackerDict<DiceCardXmlInfo> dict, SplitTrackerDict<string, DiceCardXmlInfo> splitDict)
		{
			DiceCardXmlRoot_V2 xmlRoot;
			using (StringReader stringReader = new StringReader(str))
			{
				xmlRoot = (DiceCardXmlRoot_V2)new XmlSerializer(typeof(DiceCardXmlRoot_V2), DiceCardXmlRoot_V2.Overrides).Deserialize(stringReader);
			}
			if (!splitDict.TryGetValue(uniqueId, out var subDict))
			{
				splitDict.Add(uniqueId, subDict = new TrackerDict<DiceCardXmlInfo>());
			}
			foreach (var card in xmlRoot.cardXmlList)
			{
				string newId = Tools.ClarifyWorkshopId(card.workshopID, xmlRoot.customPid, uniqueId);
				card.workshopID = newId;
				card.InitOldFields();
				dict[card.id] = subDict[card.id] = new AddTracker<DiceCardXmlInfo>(card);
			}
		}
		static void AddCardInfoByMod(TrackerDict<DiceCardXmlInfo> dict, SplitTrackerDict<string, DiceCardXmlInfo> splitDict)
		{
			if (ItemXmlDataList.instance._basicCardList == null)
			{
				ItemXmlDataList.instance._basicCardList = new List<DiceCardXmlInfo>();
			}
			AddOrReplace(dict, ItemXmlDataList.instance._basicCardList, card => card.id, card => card.optionList.Contains(CardOption.Basic));

			if (ItemXmlDataList.instance._cardInfoList == null)
			{
				ItemXmlDataList.instance._cardInfoList = new List<DiceCardXmlInfo>();
			}
			AddOrReplace(dict, ItemXmlDataList.instance._cardInfoList, card => card.id);

			if (ItemXmlDataList.instance._cardInfoTable == null)
			{
				ItemXmlDataList.instance._cardInfoTable = new Dictionary<LorId, DiceCardXmlInfo>();
			}
			AddOrReplace(dict, ItemXmlDataList.instance._cardInfoTable, card => card.id);

			AddOrReplace(splitDict, ItemXmlDataList.instance._workshopDict, card => card.id);
		}


		static void LoadAllDeck_MOD(List<ModContent> mods)
		{
			TrackerDict<DeckXmlInfo> deckDict = new TrackerDict<DeckXmlInfo>();
			foreach (ModContent modcontent in mods)
			{
				DirectoryInfo _dirInfo = modcontent._dirInfo;
				string workshopId = modcontent._itemUniqueId;
				if (workshopId.ToLower().EndsWith("@origin"))
				{
					workshopId = "";
				}
				try
				{
					if (ExistsModdingPath(_dirInfo, "Deck", out var directory))
					{
						LoadDeck_MOD(directory, workshopId, deckDict);
					}
				}
				catch (Exception ex)
				{
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + _dirInfo.Name + "_StaticInfoDeckError.log", ex.ToString());
				}
			}

			if (DeckXmlList.Instance._list == null)
			{
				DeckXmlList.Instance._list = new List<DeckXmlInfo>();
			}
			AddOrReplace(deckDict, DeckXmlList.Instance._list, d => d.id);
		}
		static void LoadDeck_MOD(DirectoryInfo dir, string uniqueId, TrackerDict<DeckXmlInfo> dict)
		{
			LoadDeck_MOD_Checking(dir, uniqueId, dict);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadDeck_MOD(directories[i], uniqueId, dict);
				}
			}
		}
		static void LoadDeck_MOD_Checking(DirectoryInfo dir, string uniqueId, TrackerDict<DeckXmlInfo> dict)
		{
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					LoadNewDeck(File.ReadAllText(fileInfo.FullName), uniqueId, dict);
				}
				catch (Exception ex)
				{
					UtilTools.CreateShortcut(Application.dataPath + "/Mods/", "Error from  " + uniqueId + " " + fileInfo.Name, fileInfo.FullName, fileInfo.DirectoryName, "Error Xml Files");
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + "Error in " + uniqueId + " " + fileInfo.Name + ".log", ex.ToString());
				}
			}
		}
		static void LoadNewDeck(string str, string uniqueId, TrackerDict<DeckXmlInfo> dict)
		{
			DeckXmlRoot_V2 xmlRoot;
			using (StringReader stringReader = new StringReader(str))
			{
				xmlRoot = (DeckXmlRoot_V2)new XmlSerializer(typeof(DeckXmlRoot_V2), DeckXmlRoot_V2.Overrides).Deserialize(stringReader);
			}
			foreach (var deck in xmlRoot.deckXmlList)
			{
				string newId = Tools.ClarifyWorkshopId(deck.workshopId, xmlRoot.customPid, uniqueId);
				deck.workshopId = newId;
				LorId.InitializeLorIds(deck._cardIdList, deck.cardIdList, newId);
				dict[deck.id] = new AddTracker<DeckXmlInfo>(deck);
			}
		}


		static void LoadAllBook_MOD(List<ModContent> mods)
		{
			TrackerDict<BookXmlInfo_V2> bookDict = new TrackerDict<BookXmlInfo_V2>();
			SplitTrackerDict<string, BookXmlInfo_V2> splitBookDict = new SplitTrackerDict<string, BookXmlInfo_V2>();
			foreach (ModContent modcontent in mods)
			{
				DirectoryInfo _dirInfo = modcontent._dirInfo;
				string workshopId = modcontent._itemUniqueId;
				if (workshopId.ToLower().EndsWith("@origin"))
				{
					workshopId = "";
				}
				try
				{
					if (ExistsModdingPath(_dirInfo, "EquipPage", out var directory))
					{
						LoadBook_MOD(directory, workshopId, bookDict, splitBookDict);
					}
				}
				catch (Exception ex)
				{
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + _dirInfo.Name + "_StaticInfoEquipPageError.log", ex.ToString());
				}
			}
			AddEquipPageByMod(bookDict, splitBookDict);
		}
		static void LoadBook_MOD(DirectoryInfo dir, string uniqueId, TrackerDict<BookXmlInfo_V2> dict, SplitTrackerDict<string, BookXmlInfo_V2> splitDict)
		{
			LoadBook_MOD_Checking(dir, uniqueId, dict, splitDict);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadBook_MOD(directories[i], uniqueId, dict, splitDict);
				}
			}
		}
		static void LoadBook_MOD_Checking(DirectoryInfo dir, string uniqueId, TrackerDict<BookXmlInfo_V2> dict, SplitTrackerDict<string, BookXmlInfo_V2> splitDict)
		{
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					LoadNewCorePage(File.ReadAllText(fileInfo.FullName), uniqueId, dict, splitDict);
				}
				catch (Exception ex)
				{
					UtilTools.CreateShortcut(Application.dataPath + "/Mods/", "Error from  " + uniqueId + " " + fileInfo.Name, fileInfo.FullName, fileInfo.DirectoryName, "Error Xml Files");
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + "Error in " + uniqueId + " " + fileInfo.Name + ".log", ex.ToString());
				}
			}
		}
		static void LoadNewCorePage(string str, string uniqueId, TrackerDict<BookXmlInfo_V2> dict, SplitTrackerDict<string, BookXmlInfo_V2> splitDict)
		{
			BookXmlRoot_V2 xmlRoot;
			using (StringReader stringReader = new StringReader(str))
			{
				xmlRoot = (BookXmlRoot_V2)new XmlSerializer(typeof(BookXmlRoot_V2), BookXmlRoot_V2.Overrides).Deserialize(stringReader);
			}
			if (!splitDict.TryGetValue(uniqueId, out var subDict))
			{
				splitDict.Add(uniqueId, subDict = new TrackerDict<BookXmlInfo_V2>());
			}
			foreach (var book in xmlRoot.bookXmlList)
			{
				string newId = Tools.ClarifyWorkshopId(book.workshopID, xmlRoot.customPid, uniqueId);
				book.InitOldFields(newId);
				subDict[book.id] = dict[book.id] = new AddTracker<BookXmlInfo_V2>(book);
			}
		}
		static void AddEquipPageByMod(TrackerDict<BookXmlInfo_V2> dict, SplitTrackerDict<string, BookXmlInfo_V2> splitDict)
		{
			AddOrReplace(dict, BookXmlList.Instance._list, ep => ep.id);
			AddOrReplace(dict, BookXmlList.Instance._dictionary, ep => ep.id);
			AddOrReplace(splitDict, BookXmlList.Instance._workshopBookDict, ep => ep.id);
			foreach (var tracker in dict.Values)
			{
				var book = tracker.element;
				if (book.LorEpisode != null)
				{
					OrcTools.EpisodeDic[book.id] = book.LorEpisode;
				}
			}
		}


		static void LoadAllCardDropTable_MOD(List<ModContent> mods)
		{
			TrackerDict<CardDropTableXmlInfo> cardDropDict = new TrackerDict<CardDropTableXmlInfo>();
			SplitTrackerDict<string, CardDropTableXmlInfo> splitCardDropDict = new SplitTrackerDict<string, CardDropTableXmlInfo>();
			foreach (ModContent modcontent in mods)
			{
				DirectoryInfo _dirInfo = modcontent._dirInfo;
				string workshopId = modcontent._itemUniqueId;
				if (workshopId.ToLower().EndsWith("@origin"))
				{
					workshopId = "";
				}
				try
				{
					if (ExistsModdingPath(_dirInfo, "CardDropTable", out var directory))
					{
						LoadCardDropTable_MOD(directory, workshopId, cardDropDict, splitCardDropDict);
					}
				}
				catch (Exception ex)
				{
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + _dirInfo.Name + "_StaticInfoCardDropTableError.log", ex.ToString());
				}
			}
			AddCardDropTableByMod(cardDropDict, splitCardDropDict);
		}
		static void LoadCardDropTable_MOD(DirectoryInfo dir, string uniqueId, TrackerDict<CardDropTableXmlInfo> dict, SplitTrackerDict<string, CardDropTableXmlInfo> splitDict)
		{
			LoadCardDropTable_MOD_Checking(dir, uniqueId, dict, splitDict);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadCardDropTable_MOD(directories[i], uniqueId, dict, splitDict);
				}
			}
		}
		static void LoadCardDropTable_MOD_Checking(DirectoryInfo dir, string uniqueId, TrackerDict<CardDropTableXmlInfo> dict, SplitTrackerDict<string, CardDropTableXmlInfo> splitDict)
		{
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					LoadNewCardDropTable(File.ReadAllText(fileInfo.FullName), uniqueId, dict, splitDict);
				}
				catch (Exception ex)
				{
					UtilTools.CreateShortcut(Application.dataPath + "/Mods/", "Error from  " + uniqueId + " " + fileInfo.Name, fileInfo.FullName, fileInfo.DirectoryName, "Error Xml Files");
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + "Error in " + uniqueId + " " + fileInfo.Name + ".log", ex.ToString());
				}
			}
		}
		static void LoadNewCardDropTable(string str, string uniqueId, TrackerDict<CardDropTableXmlInfo> dict, SplitTrackerDict<string, CardDropTableXmlInfo> splitDict)
		{
			CardDropTableXmlRoot_V2 xmlRoot;
			using (StringReader stringReader = new StringReader(str))
			{
				xmlRoot = (CardDropTableXmlRoot_V2)new XmlSerializer(typeof(CardDropTableXmlRoot_V2), CardDropTableXmlRoot_V2.Overrides).Deserialize(stringReader);
			}
			if (!splitDict.TryGetValue(uniqueId, out var subDict))
			{
				splitDict.Add(uniqueId, subDict = new TrackerDict<CardDropTableXmlInfo>());
			}
			foreach (var cardDropTable in xmlRoot.dropTableXmlList)
			{
				string newId = Tools.ClarifyWorkshopId(cardDropTable.workshopId, xmlRoot.customPid, uniqueId);
				cardDropTable.workshopId = newId;
				LorId.InitializeLorIds(cardDropTable._cardIdList, cardDropTable.cardIdList, newId);
				subDict[cardDropTable.id] = dict[cardDropTable.id] = new AddTracker<CardDropTableXmlInfo>(cardDropTable);
			}
		}
		static void AddCardDropTableByMod(TrackerDict<CardDropTableXmlInfo> dict, SplitTrackerDict<string, CardDropTableXmlInfo> splitDict)
		{
			if (CardDropTableXmlList.Instance._list == null)
			{
				CardDropTableXmlList.Instance._list = new List<CardDropTableXmlInfo>();
			}
			AddOrReplace(dict, CardDropTableXmlList.Instance._list, cdt => cdt.id);

			AddOrReplace(splitDict, CardDropTableXmlList.Instance._workshopDict, cdt => cdt.id);
		}


		static void LoadAllDropBook_MOD(List<ModContent> mods)
		{
			TrackerDict<DropBookXmlInfo> dropBookDict = new TrackerDict<DropBookXmlInfo>();
			SplitTrackerDict<string, DropBookXmlInfo> splitDropBookDict = new SplitTrackerDict<string, DropBookXmlInfo>();
			foreach (ModContent modcontent in mods)
			{
				DirectoryInfo _dirInfo = modcontent._dirInfo;
				string workshopId = modcontent._itemUniqueId;
				if (workshopId.ToLower().EndsWith("@origin"))
				{
					workshopId = "";
				}
				try
				{
					if (ExistsModdingPath(_dirInfo, "DropBook", out var directory))
					{
						LoadDropBook_MOD(directory, workshopId, dropBookDict, splitDropBookDict);
					}
				}
				catch (Exception ex)
				{
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + _dirInfo.Name + "_StaticInfoDropBookError.log", ex.ToString());
				}
			}
			AddDropBookByMod(dropBookDict, splitDropBookDict);
		}
		static void LoadDropBook_MOD(DirectoryInfo dir, string uniqueId, TrackerDict<DropBookXmlInfo> dict, SplitTrackerDict<string, DropBookXmlInfo> splitDict)
		{
			LoadDropBook_MOD_Checking(dir, uniqueId, dict, splitDict);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadDropBook_MOD(directories[i], uniqueId, dict, splitDict);
				}
			}
		}
		static void LoadDropBook_MOD_Checking(DirectoryInfo dir, string uniqueId, TrackerDict<DropBookXmlInfo> dict, SplitTrackerDict<string, DropBookXmlInfo> splitDict)
		{
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					LoadNewDropBook(File.ReadAllText(fileInfo.FullName), uniqueId, dict, splitDict);
				}
				catch (Exception ex)
				{
					UtilTools.CreateShortcut(Application.dataPath + "/Mods/", "Error from  " + uniqueId + " " + fileInfo.Name, fileInfo.FullName, fileInfo.DirectoryName, "Error Xml Files");
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + "Error in " + uniqueId + " " + fileInfo.Name + ".log", ex.ToString());
				}
			}
		}
		static void LoadNewDropBook(string str, string uniqueId, TrackerDict<DropBookXmlInfo> dict, SplitTrackerDict<string, DropBookXmlInfo> splitDict)
		{
			BookUseXmlRoot_V2 xmlRoot;
			using (StringReader stringReader = new StringReader(str))
			{
				xmlRoot = (BookUseXmlRoot_V2)new XmlSerializer(typeof(BookUseXmlRoot_V2), BookUseXmlRoot_V2.Overrides).Deserialize(stringReader);
			}
			if (!splitDict.TryGetValue(uniqueId, out var subDict))
			{
				splitDict.Add(uniqueId, subDict = new TrackerDict<DropBookXmlInfo>());
			}
			foreach (var dropBook in xmlRoot.bookXmlList)
			{
				string newId = Tools.ClarifyWorkshopId(dropBook.workshopID, xmlRoot.customPid, uniqueId);
				dropBook.workshopID = newId;
				dropBook.InitializeDropItemList(newId);
				CardDropTableXmlInfo workshopData = CardDropTableXmlList.Instance.GetWorkshopData(newId, dropBook.id.id);
				if (workshopData != null)
				{
					foreach (LorId id in workshopData.cardIdList)
					{
						dropBook.DropItemList.Add(new BookDropItemInfo(id)
						{
							itemType = DropItemType.Card
						});
					}
				}
				subDict[dropBook.id] = dict[dropBook.id] = new AddTracker<DropBookXmlInfo>(dropBook);
			}
		}
		static void AddDropBookByMod(TrackerDict<DropBookXmlInfo> dict, SplitTrackerDict<string, DropBookXmlInfo> splitDict)
		{
			AddOrReplace(dict, DropBookXmlList.Instance._list, db => db.id);
			AddOrReplace(dict, DropBookXmlList.Instance._dict, db => db.id);

			if (DropBookXmlList.Instance._workshopDict == null)
			{
				DropBookXmlList.Instance._workshopDict = new Dictionary<string, List<DropBookXmlInfo>>();
			}
			AddOrReplace(splitDict, DropBookXmlList.Instance._workshopDict, db => db.id);
		}


		static void LoadAllGiftAndTitle_MOD(List<ModContent> mods)
		{
			TrackerDict<GiftXmlInfo_V2> giftDict = new TrackerDict<GiftXmlInfo_V2>();
			TrackerDict<TitleXmlInfo_V2> prefixDict = new TrackerDict<TitleXmlInfo_V2>();
			TrackerDict<TitleXmlInfo_V2> postfixDict = new TrackerDict<TitleXmlInfo_V2>();
			foreach (ModContent modcontent in mods)
			{
				DirectoryInfo _dirInfo = modcontent._dirInfo;
				string workshopId = modcontent._itemUniqueId;
				if (workshopId.ToLower().EndsWith("@origin"))
				{
					workshopId = "";
				}
				try
				{
					if (ExistsModdingPath(_dirInfo, "GiftInfo", out var directory))
					{
						LoadGift_MOD(directory, workshopId, giftDict);
					}
				}
				catch (Exception ex)
				{
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + _dirInfo.Name + "_StaticInfoGiftError.log", ex.ToString());
				}
				try
				{
					if (ExistsModdingPath(_dirInfo, "Titles", out var directory))
					{
						LoadTitle_MOD(directory, workshopId, prefixDict, postfixDict);
					}
				}
				catch (Exception ex)
				{
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + _dirInfo.Name + "_StaticInfoTitlesError.log", ex.ToString());
				}
			}
			AddGiftAndTitleByMod(giftDict, prefixDict, postfixDict);
		}
		static void LoadGift_MOD(DirectoryInfo dir, string uniqueId, TrackerDict<GiftXmlInfo_V2> dict)
		{
			LoadGift_MOD_Checking(dir, uniqueId, dict);
			bool flag = dir.GetDirectories().Length != 0;
			if (flag)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadGift_MOD(directories[i], uniqueId, dict);
				}
			}
		}
		static void LoadGift_MOD_Checking(DirectoryInfo dir, string uniqueId, TrackerDict<GiftXmlInfo_V2> dict)
		{
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					LoadNewGift(File.ReadAllText(fileInfo.FullName), uniqueId, dict);
				}
				catch (Exception ex)
				{
					UtilTools.CreateShortcut(Application.dataPath + "/Mods/", "Error from  " + uniqueId + " " + fileInfo.Name, fileInfo.FullName, fileInfo.DirectoryName, "Error Xml Files");
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + "Error in " + uniqueId + " " + fileInfo.Name + ".log", ex.ToString());
				}
			}
		}
		static void LoadNewGift(string str, string uniqueId, TrackerDict<GiftXmlInfo_V2> dict)
		{
			GiftXmlRoot_V2 xmlRoot;
			using (StringReader stringReader = new StringReader(str))
			{
				xmlRoot = (GiftXmlRoot_V2)new XmlSerializer(typeof(GiftXmlRoot_V2), GiftXmlRoot_V2.Overrides).Deserialize(stringReader);
			}
			foreach (GiftXmlInfo_V2 newGift in xmlRoot.giftXmlList)
			{
				newGift.WorkshopId = Tools.ClarifyWorkshopIdLegacy(newGift.WorkshopId, xmlRoot.customPid, uniqueId);
				dict[newGift.lorId] = new AddTracker<GiftXmlInfo_V2>(newGift);
				OrcTools.CustomGifts[newGift.lorId] = newGift;
			}
		}
		static void LoadTitle_MOD(DirectoryInfo dir, string uniqueId, TrackerDict<TitleXmlInfo_V2> prefixDict, TrackerDict<TitleXmlInfo_V2> postfixDict)
		{
			LoadTitle_MOD_Checking(dir, uniqueId, prefixDict, postfixDict);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadTitle_MOD(directories[i], uniqueId, prefixDict, postfixDict);
				}
			}
		}
		static void LoadTitle_MOD_Checking(DirectoryInfo dir, string uniqueId, TrackerDict<TitleXmlInfo_V2> prefixDict, TrackerDict<TitleXmlInfo_V2> postfixDict)
		{
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					LoadNewTitle(File.ReadAllText(fileInfo.FullName), uniqueId, prefixDict, postfixDict);
				}
				catch (Exception ex)
				{
					UtilTools.CreateShortcut(Application.dataPath + "/Mods/", "Error from  " + uniqueId + " " + fileInfo.Name, fileInfo.FullName, fileInfo.DirectoryName, "Error Xml Files");
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + "Error in " + uniqueId + " " + fileInfo.Name + ".log", ex.ToString());
				}
			}
		}
		static void LoadNewTitle(string str, string uniqueId, TrackerDict<TitleXmlInfo_V2> prefixDict, TrackerDict<TitleXmlInfo_V2> postfixDict)
		{
			TitleXmlRoot_V2 xmlRoot;
			using (StringReader stringReader = new StringReader(str))
			{
				xmlRoot = (TitleXmlRoot_V2)new XmlSerializer(typeof(TitleXmlRoot_V2)).Deserialize(stringReader);
			}
			LoadHalfTitle(xmlRoot.prefixXmlList.List, uniqueId, xmlRoot.customPid, prefixDict);
			LoadHalfTitle(xmlRoot.postfixXmlList.List, uniqueId, xmlRoot.customPid, postfixDict);
		}
		static void LoadHalfTitle(List<TitleXmlInfo_V2> list, string uniqueId, string rootId, TrackerDict<TitleXmlInfo_V2> dict)
		{
			foreach (var title in list)
			{
				title.WorkshopId = Tools.ClarifyWorkshopIdLegacy(title.WorkshopId, rootId, uniqueId);
				dict[title.lorId] = new AddTracker<TitleXmlInfo_V2>(title);
			}
		}
		static void AddGiftAndTitleByMod(TrackerDict<GiftXmlInfo_V2> giftDict, TrackerDict<TitleXmlInfo_V2> prefixDict, TrackerDict<TitleXmlInfo_V2> postfixDict)
		{
			foreach (var gift in GiftXmlList.Instance._giftDict.Keys)
			{
				if (giftDict.TryGetValue(new LorId(gift), out var customGift))
				{
					customGift.element.dontRemove = true;
				}
			}
			AddOrReplace(giftDict, GiftXmlList.Instance._list, gift => gift.id, gift => gift.lorId.IsBasic());
			AddOrReplace(giftDict, GiftXmlList.Instance._giftDict, gift => gift.id, gift => gift.lorId.IsBasic());
			AddOrReplace(prefixDict, TitleXmlList.Instance._prefixList, title => title.ID, title => title.lorId.IsBasic());
			AddOrReplace(postfixDict, TitleXmlList.Instance._postfixList, title => title.ID, title => title.lorId.IsBasic());
			HashSet<int> usedIds = new HashSet<int>();
			int minCheckedId = MinInjectedId;
			foreach (var gift in GiftXmlList.Instance._list)
			{
				usedIds.Add(gift.id);
			}
			foreach (var title in TitleXmlList.Instance._prefixList)
			{
				usedIds.Add(title.ID);
			}
			foreach (var title in TitleXmlList.Instance._postfixList)
			{
				usedIds.Add(title.ID);
			}
			AddTracker<GiftXmlInfo_V2> gift1 = null;
			AddTracker<TitleXmlInfo_V2> prefix = null;
			AddTracker<TitleXmlInfo_V2> postfix = null;
			foreach (var lorid in giftDict.Keys.Concat(prefixDict.Keys).Concat(postfixDict.Keys))
			{
				if (lorid.IsBasic())
				{
					continue;
				}
				if ((!giftDict.TryGetValue(lorid, out gift1) || gift1.added) && (!prefixDict.TryGetValue(lorid, out prefix) || prefix.added) && (!postfixDict.TryGetValue(lorid, out postfix) || postfix.added))
				{
					continue;
				}

				while (usedIds.Contains(minCheckedId))
				{
					minCheckedId++;
				}
				if (gift1 != null && !gift1.added)
				{
					gift1.element.InjectId(minCheckedId);
					GiftXmlList.Instance._list.Add(gift1.element);
					GiftXmlList.Instance._giftDict.Add(minCheckedId, gift1.element);
				}
				if (prefix != null && !prefix.added)
				{
					prefix.element.InjectId(minCheckedId);
					TitleXmlList.Instance._prefixList.Add(prefix.element);
				}
				if (postfix != null && !postfix.added)
				{
					postfix.element.InjectId(minCheckedId);
					TitleXmlList.Instance._postfixList.Add(postfix.element);
				}
				minCheckedId++;
			}
		}


		static void LoadAllEmotionCard_MOD(List<ModContent> mods)
		{
			SplitTrackerDict<SephirahType, EmotionCardXmlInfo_V2> emotionCardDict = new SplitTrackerDict<SephirahType, EmotionCardXmlInfo_V2>();
			foreach (ModContent modcontent in mods)
			{
				DirectoryInfo _dirInfo = modcontent._dirInfo;
				string workshopId = modcontent._itemUniqueId;
				if (workshopId.ToLower().EndsWith("@origin"))
				{
					workshopId = "";
				}
				try
				{
					if (ExistsModdingPath(_dirInfo, "EmotionCard", out var directory))
					{
						LoadEmotionCard_MOD(directory, workshopId, emotionCardDict);
					}
				}
				catch (Exception ex)
				{
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + _dirInfo.Name + "_StaticInfoEmotionCardError.log", ex.ToString());
				}
			}
			AddEmotionCardByMod(emotionCardDict);
		}
		static void LoadEmotionCard_MOD(DirectoryInfo dir, string uniqueId, SplitTrackerDict<SephirahType, EmotionCardXmlInfo_V2> dict)
		{
			LoadEmotionCard_MOD_Checking(dir, uniqueId, dict);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadEmotionCard_MOD(directories[i], uniqueId, dict);
				}
			}
		}
		static void LoadEmotionCard_MOD_Checking(DirectoryInfo dir, string uniqueId, SplitTrackerDict<SephirahType, EmotionCardXmlInfo_V2> dict)
		{
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					LoadNewEmotionCard(File.ReadAllText(fileInfo.FullName), uniqueId, dict);
				}
				catch (Exception ex)
				{
					UtilTools.CreateShortcut(Application.dataPath + "/Mods/", "Error from  " + uniqueId + " " + fileInfo.Name, fileInfo.FullName, fileInfo.DirectoryName, "Error Xml Files");
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + "Error in " + uniqueId + " " + fileInfo.Name + ".log", ex.ToString());
				}
			}
		}
		static void LoadNewEmotionCard(string str, string uniqueId, SplitTrackerDict<SephirahType, EmotionCardXmlInfo_V2> dict)
		{
			EmotionCardXmlRoot_V2 xmlRoot;
			using (StringReader stringReader = new StringReader(str))
			{
				xmlRoot = (EmotionCardXmlRoot_V2)new XmlSerializer(typeof(EmotionCardXmlRoot_V2), EmotionCardXmlRoot_V2.Overrides).Deserialize(stringReader);
			}
			foreach (var card in xmlRoot.emotionCardXmlList)
			{
				card.InitOldFields();
				card.WorkshopId = Tools.ClarifyWorkshopIdLegacy(card.WorkshopId, xmlRoot.customPid, uniqueId);
				if (!dict.TryGetValue(card.Sephirah, out var subdict))
				{
					dict[card.Sephirah] = subdict = new TrackerDict<EmotionCardXmlInfo_V2>();
				}
				subdict[card.lorId] = new AddTracker<EmotionCardXmlInfo_V2>(card);
				if (!OrcTools.CustomEmotionCards.TryGetValue(card.Sephirah, out var subdict1))
				{
					OrcTools.CustomEmotionCards[card.Sephirah] = subdict1 = new Dictionary<LorId, EmotionCardXmlInfo>();
				}
				subdict1[card.lorId] = card;
			}
		}
		static void AddEmotionCardByMod(SplitTrackerDict<SephirahType, EmotionCardXmlInfo_V2> dict)
		{
			Dictionary<SephirahType, HashSet<int>> usedIds = new Dictionary<SephirahType, HashSet<int>>();
			Dictionary<SephirahType, int> minCheckedIds = new Dictionary<SephirahType, int>();
			var originList = EmotionCardXmlList.Instance._list;
			AddOrReplaceWithInject(dict, originList, card => card.id, card => card.Sephirah, injCard =>
			{
				if (!minCheckedIds.TryGetValue(injCard.Sephirah, out var nextValue))
				{
					minCheckedIds[injCard.Sephirah] = nextValue = MinInjectedId;
				}
				else
				{
					nextValue++;
				}
				if (usedIds.TryGetValue(injCard.Sephirah, out var usedSephIds))
				{
					while (usedSephIds.Contains(nextValue))
					{
						nextValue++;
					}
				}
				minCheckedIds[injCard.Sephirah] = nextValue;
				return nextValue;
			}, addCard =>
			{
				if (!usedIds.TryGetValue(addCard.Sephirah, out var usedSephIds))
				{
					usedIds[addCard.Sephirah] = usedSephIds = new HashSet<int>();
				}
				usedSephIds.Add(addCard.id);
			});
		}


		static void LoadAllEmotionEgo_MOD(List<ModContent> mods)
		{
			FixEmotionEgoIds();
			SplitTrackerDict<SephirahType, EmotionEgoXmlInfo_V2> egoDict = new SplitTrackerDict<SephirahType, EmotionEgoXmlInfo_V2>();
			foreach (ModContent modcontent in mods)
			{
				DirectoryInfo _dirInfo = modcontent._dirInfo;
				string workshopId = modcontent._itemUniqueId;
				if (workshopId.ToLower().EndsWith("@origin"))
				{
					workshopId = "";
				}
				try
				{
					if (ExistsModdingPath(_dirInfo, "EmotionEgo", out var directory))
					{
						LoadEmotionEgo_MOD(directory, workshopId, egoDict);
					}
				}
				catch (Exception ex)
				{
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + _dirInfo.Name + "_StaticInfoEmotionEgoError.log", ex.ToString());
				}
			}
			AddEmotionEgoByMod(egoDict);
		}
		static void LoadEmotionEgo_MOD(DirectoryInfo dir, string uniqueId, SplitTrackerDict<SephirahType, EmotionEgoXmlInfo_V2> dict)
		{
			LoadEmotionEgo_MOD_Checking(dir, uniqueId, dict);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadEmotionEgo_MOD(directories[i], uniqueId, dict);
				}
			}
		}
		static void LoadEmotionEgo_MOD_Checking(DirectoryInfo dir, string uniqueId, SplitTrackerDict<SephirahType, EmotionEgoXmlInfo_V2> dict)
		{
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					LoadNewEmotionEgo(File.ReadAllText(fileInfo.FullName), uniqueId, dict);
				}
				catch (Exception ex)
				{
					UtilTools.CreateShortcut(Application.dataPath + "/Mods/", "Error from  " + uniqueId + " " + fileInfo.Name, fileInfo.FullName, fileInfo.DirectoryName, "Error Xml Files");
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + "Error in " + uniqueId + " " + fileInfo.Name + ".log", ex.ToString());
				}
			}
		}
		static void LoadNewEmotionEgo(string str, string uniqueId, SplitTrackerDict<SephirahType, EmotionEgoXmlInfo_V2> dict)
		{
			EmotionEgoXmlRoot_V2 xmlRoot;
			using (StringReader stringReader = new StringReader(str))
			{
				xmlRoot = (EmotionEgoXmlRoot_V2)new XmlSerializer(typeof(EmotionEgoXmlRoot_V2), EmotionEgoXmlRoot_V2.Overrides).Deserialize(stringReader);
			}
			foreach (var emotionEgo in xmlRoot.egoXmlList)
			{
				var newId = Tools.ClarifyWorkshopId("", xmlRoot.customPid, uniqueId);
				emotionEgo.InitOldFields(newId);
				if (!dict.TryGetValue(emotionEgo.Sephirah, out var subdict))
				{
					dict[emotionEgo.Sephirah] = subdict = new TrackerDict<EmotionEgoXmlInfo_V2>();
				}
				subdict[emotionEgo.lorId] = new AddTracker<EmotionEgoXmlInfo_V2>(emotionEgo);
				if (!OrcTools.CustomEmotionEgo.TryGetValue(emotionEgo.Sephirah, out var subdict1))
				{
					OrcTools.CustomEmotionEgo[emotionEgo.Sephirah] = subdict1 = new Dictionary<LorId, EmotionEgoXmlInfo>();
				}
				subdict1[emotionEgo.lorId] = emotionEgo;
			}
		}
		static void AddEmotionEgoByMod(SplitTrackerDict<SephirahType, EmotionEgoXmlInfo_V2> dict)
		{
			Dictionary<SephirahType, HashSet<int>> usedIds = new Dictionary<SephirahType, HashSet<int>>();
			Dictionary<SephirahType, int> minCheckedIds = new Dictionary<SephirahType, int>();
			var originList = EmotionEgoXmlList.Instance._list;
			AddOrReplaceWithInject(dict, originList, card => card.id, card => card.Sephirah, injCard =>
			{
				if (!minCheckedIds.TryGetValue(injCard.Sephirah, out var nextValue))
				{
					minCheckedIds[injCard.Sephirah] = nextValue = MinInjectedId;
				}
				else
				{
					nextValue++;
				}
				if (usedIds.TryGetValue(injCard.Sephirah, out var usedSephIds))
				{
					while (usedSephIds.Contains(nextValue))
					{
						nextValue++;
					}
				}
				minCheckedIds[injCard.Sephirah] = nextValue;
				return nextValue;
			}, addCard =>
			{
				if (!usedIds.TryGetValue(addCard.Sephirah, out var usedSephIds))
				{
					usedIds[addCard.Sephirah] = usedSephIds = new HashSet<int>();
				}
				usedSephIds.Add(addCard.id);
			});
		}
		static void FixEmotionEgoIds()
		{
			foreach (var emotionEgo in EmotionEgoXmlList.Instance._list)
			{
				if (emotionEgo.id == 0)
				{
					var cardId = emotionEgo.CardId;
					if (cardId.IsBasic() && cardId.id > 910000 && cardId.id < 920000)
					{
						emotionEgo.id = cardId.id - 910000;
					}
				}
			}
		}


		static void LoadAllToolTip_MOD(List<ModContent> mods)
		{
			TrackerDict<ToolTipXmlInfo> dict = new TrackerDict<ToolTipXmlInfo>();
			foreach (ModContent modcontent in mods)
			{
				DirectoryInfo _dirInfo = modcontent._dirInfo;
				string workshopId = modcontent._itemUniqueId;
				if (workshopId.ToLower().EndsWith("@origin"))
				{
					workshopId = "";
				}
				try
				{
					if (ExistsModdingPath(_dirInfo, "XmlToolTips", out var directory))
					{
						LoadToolTip_MOD(directory, workshopId, dict);
					}
				}
				catch (Exception ex)
				{
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + _dirInfo.Name + "_StaticInfoToolTipError.log", ex.ToString());
				}
			}
			AddToolTipByMod(dict);
		}
		static void LoadToolTip_MOD(DirectoryInfo dir, string uniqueId, TrackerDict<ToolTipXmlInfo> dict)
		{
			LoadToolTip_MOD_Checking(dir, uniqueId, dict);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadToolTip_MOD(directories[i], uniqueId, dict);
				}
			}
		}
		static void LoadToolTip_MOD_Checking(DirectoryInfo dir, string uniqueId, TrackerDict<ToolTipXmlInfo> dict)
		{
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					LoadNewToolTip(File.ReadAllText(fileInfo.FullName), dict);
				}
				catch (Exception ex)
				{
					UtilTools.CreateShortcut(Application.dataPath + "/Mods/", "Error from  " + uniqueId + " " + fileInfo.Name, fileInfo.FullName, fileInfo.DirectoryName, "Error Xml Files");
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + "Error in " + uniqueId + " " + fileInfo.Name + ".log", ex.ToString());
				}
			}
		}
		static void LoadNewToolTip(string str, TrackerDict<ToolTipXmlInfo> dict)
		{
			ToolTipXmlRoot_V2 xmlRoot;
			using (StringReader stringReader = new StringReader(str))
			{
				xmlRoot = (ToolTipXmlRoot_V2)new XmlSerializer(typeof(ToolTipXmlRoot_V2), ToolTipXmlRoot_V2.Overrides).Deserialize(stringReader);
			}
			foreach (var toolTip in xmlRoot.toolTipXmlList)
			{
				toolTip.InitOldFields();
				dict[new LorId((int)toolTip.ID)] = new AddTracker<ToolTipXmlInfo>(toolTip);
			}
		}
		static void AddToolTipByMod(TrackerDict<ToolTipXmlInfo> dict)
		{
			AddOrReplace(dict, ToolTipXmlList.Instance._list, tt => (int)tt.ID);
		}


		//no idea why this was public to begin with, but now it's being kept public for backwards compatibility
		[Obsolete]
		public static TitleXmlRoot LoadNewTitle(string str)
		{
			TitleXmlRoot result;
			using (StringReader stringReader = new StringReader(str))
			{
				result = (TitleXmlRoot)new XmlSerializer(typeof(TitleXmlRoot)).Deserialize(stringReader);
			}
			return result;
		}


		static void LoadAllFormation_MOD(List<ModContent> mods)
		{
			TrackerDict<FormationXmlInfo_V2> formDict = new TrackerDict<FormationXmlInfo_V2>();
			foreach (ModContent modcontent in mods)
			{
				DirectoryInfo _dirInfo = modcontent._dirInfo;
				string workshopId = modcontent._itemUniqueId;
				if (workshopId.ToLower().EndsWith("@origin"))
				{
					workshopId = "";
				}
				try
				{
					if (ExistsModdingPath(_dirInfo, "FormationInfo", out var directory))
					{
						LoadFormation_MOD(directory, workshopId, formDict);
					}
				}
				catch (Exception ex)
				{
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + _dirInfo.Name + "_StaticInfoFormationError.log", ex.ToString());
				}
			}
			AddFormationByMod(formDict);
		}
		static void LoadFormation_MOD(DirectoryInfo dir, string uniqueId, TrackerDict<FormationXmlInfo_V2> dict)
		{
			LoadFormation_MOD_Checking(dir, uniqueId, dict);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadFormation_MOD(directories[i], uniqueId, dict);
				}
			}
		}
		static void LoadFormation_MOD_Checking(DirectoryInfo dir, string uniqueId, TrackerDict<FormationXmlInfo_V2> dict)
		{
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					LoadNewFormation(File.ReadAllText(fileInfo.FullName), uniqueId, dict);
				}
				catch (Exception ex)
				{
					UtilTools.CreateShortcut(Application.dataPath + "/Mods/", "Error from  " + uniqueId + " " + fileInfo.Name, fileInfo.FullName, fileInfo.DirectoryName, "Error Xml Files");
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + "Error in " + uniqueId + " " + fileInfo.Name + ".log", ex.ToString());
				}
			}
		}
		static void LoadNewFormation(string str, string uniqueId, TrackerDict<FormationXmlInfo_V2> dict)
		{
			FormationXmlRoot_V2 xmlRoot;
			using (StringReader stringReader = new StringReader(str))
			{
				xmlRoot = (FormationXmlRoot_V2)new XmlSerializer(typeof(FormationXmlRoot_V2)).Deserialize(stringReader);
			}
			foreach (FormationXmlInfo_V2 formation in xmlRoot.list)
			{
				formation.WorkshopId = Tools.ClarifyWorkshopIdLegacy(formation.WorkshopId, xmlRoot.customPid, uniqueId);
				dict[formation.lorId] = new AddTracker<FormationXmlInfo_V2>(formation);
				OrcTools.CustomFormations[formation.lorId] = formation;
			}
		}
		static void AddFormationByMod(TrackerDict<FormationXmlInfo_V2> dict)
		{
			HashSet<int> usedIds = new HashSet<int>();
			int minCheckedId = MinInjectedId - 1;
			var originList = FormationXmlList.Instance._list;
			AddOrReplaceWithInject(dict, originList, form => form.id, injForm =>
			{
				minCheckedId++;
				while (usedIds.Contains(minCheckedId))
				{
					minCheckedId++;
				}
				return minCheckedId;
			}, addForm => usedIds.Add(addForm.id));
		}


		static void LoadAllQuest_MOD(List<ModContent> mods)
		{
			SplitTrackerDict<SephirahType, QuestXmlInfo_V2> questDict = new SplitTrackerDict<SephirahType, QuestXmlInfo_V2>();
			foreach (ModContent modcontent in mods)
			{
				DirectoryInfo _dirInfo = modcontent._dirInfo;
				string workshopId = modcontent._itemUniqueId;
				if (workshopId.ToLower().EndsWith("@origin"))
				{
					workshopId = "";
				}
				try
				{
					if (ExistsModdingPath(_dirInfo, "QuestInfo", out DirectoryInfo directory))
					{
						LoadQuest_MOD(directory, workshopId, questDict);
					}
				}
				catch (Exception ex)
				{
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + _dirInfo.Name + "_StaticInfoQuestError.log", ex.ToString());
				}
			}
			AddQuestByMod(questDict);
		}
		static void LoadQuest_MOD(DirectoryInfo dir, string uniqueId, SplitTrackerDict<SephirahType, QuestXmlInfo_V2> dict)
		{
			LoadQuest_MOD_Checking(dir, uniqueId, dict);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadQuest_MOD(directories[i], uniqueId, dict);
				}
			}
		}
		static void LoadQuest_MOD_Checking(DirectoryInfo dir, string uniqueId, SplitTrackerDict<SephirahType, QuestXmlInfo_V2> dict)
		{
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					LoadNewQuest(File.ReadAllText(fileInfo.FullName), dict);
				}
				catch (Exception ex)
				{
					UtilTools.CreateShortcut(Application.dataPath + "/Mods/", "Error from  " + uniqueId + " " + fileInfo.Name, fileInfo.FullName, fileInfo.DirectoryName, "Error Xml Files");
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + "Error in " + uniqueId + " " + fileInfo.Name + ".log", ex.ToString());
				}
			}
		}
		static void LoadNewQuest(string str, SplitTrackerDict<SephirahType, QuestXmlInfo_V2> dict)
		{
			QuestXmlRoot_V2 xmlRoot;
			using (StringReader stringReader = new StringReader(str))
			{
				xmlRoot = (QuestXmlRoot_V2)new XmlSerializer(typeof(QuestXmlRoot_V2), QuestXmlRoot_V2.Overrides).Deserialize(stringReader);
			}
			foreach (var quest in xmlRoot.list)
			{
				quest.InitOldFields();
				if (!dict.TryGetValue(quest.sephirah, out var subdict))
				{
					dict[quest.sephirah] = subdict = new TrackerDict<QuestXmlInfo_V2>();
				}
				subdict[new LorId(quest.level)] = new AddTracker<QuestXmlInfo_V2>(quest);
			}
		}
		static void AddQuestByMod(SplitTrackerDict<SephirahType, QuestXmlInfo_V2> dict)
		{
			AddOrReplace(dict, QuestXmlList.Instance._list, quest => quest.level, quest => quest.sephirah);
		}


		static void LoadAllEnemyUnit_MOD(List<ModContent> mods)
		{
			TrackerDict<EnemyUnitClassInfo_V2> enemyDict = new TrackerDict<EnemyUnitClassInfo_V2>();
			SplitTrackerDict<string, EnemyUnitClassInfo_V2> splitEnemyDict = new SplitTrackerDict<string, EnemyUnitClassInfo_V2>();
			foreach (ModContent modcontent in mods)
			{
				DirectoryInfo _dirInfo = modcontent._dirInfo;
				string workshopId = modcontent._itemUniqueId;
				if (workshopId.ToLower().EndsWith("@origin"))
				{
					workshopId = "";
				}
				try
				{
					if (ExistsModdingPath(_dirInfo, "EnemyUnitInfo", out var directory))
					{
						LoadEnemyUnit_MOD(directory, workshopId, enemyDict, splitEnemyDict);
					}
				}
				catch (Exception ex)
				{
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + _dirInfo.Name + "_StaticInfoUnitError.log", ex.ToString());
				}
			}
			AddEnemyUnitByMod(enemyDict, splitEnemyDict);
		}
		static void LoadEnemyUnit_MOD(DirectoryInfo dir, string uniqueId, TrackerDict<EnemyUnitClassInfo_V2> dict, SplitTrackerDict<string, EnemyUnitClassInfo_V2> splitDict)
		{
			LoadEnemyUnit_MOD_Checking(dir, uniqueId, dict, splitDict);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadEnemyUnit_MOD(directories[i], uniqueId, dict, splitDict);
				}
			}
		}
		static void LoadEnemyUnit_MOD_Checking(DirectoryInfo dir, string uniqueId, TrackerDict<EnemyUnitClassInfo_V2> dict, SplitTrackerDict<string, EnemyUnitClassInfo_V2> splitDict)
		{
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					LoadNewEnemyUnit(File.ReadAllText(fileInfo.FullName), uniqueId, dict, splitDict);
				}
				catch (Exception ex)
				{
					UtilTools.CreateShortcut(Application.dataPath + "/Mods/", "Error from  " + uniqueId + " " + fileInfo.Name, fileInfo.FullName, fileInfo.DirectoryName, "Error Xml Files");
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + "Error in " + uniqueId + " " + fileInfo.Name + ".log", ex.ToString());
				}
			}
		}
		static void LoadNewEnemyUnit(string str, string uniqueId, TrackerDict<EnemyUnitClassInfo_V2> dict, SplitTrackerDict<string, EnemyUnitClassInfo_V2> splitDict)
		{
			EnemyUnitClassRoot_V2 xmlRoot;
			using (StringReader stringReader = new StringReader(str))
			{
				xmlRoot = (EnemyUnitClassRoot_V2)new XmlSerializer(typeof(EnemyUnitClassRoot_V2), EnemyUnitClassRoot_V2.Overrides).Deserialize(stringReader);
			}
			if (!splitDict.TryGetValue(uniqueId, out var subDict))
			{
				splitDict[uniqueId] = subDict = new TrackerDict<EnemyUnitClassInfo_V2>();
			}
			foreach (var enemy in xmlRoot.list)
			{
				var newId = Tools.ClarifyWorkshopId(enemy.workshopID, xmlRoot.customPid, uniqueId);
				enemy.workshopID = newId;
				enemy.InitOldFields(newId);
				OrcTools.DropItemDicV2[enemy.id] = enemy.dropTableListNew;
				subDict[enemy.id] = dict[enemy.id] = new AddTracker<EnemyUnitClassInfo_V2>(enemy);
			}
		}
		static void AddEnemyUnitByMod(TrackerDict<EnemyUnitClassInfo_V2> dict, SplitTrackerDict<string, EnemyUnitClassInfo_V2> splitDict)
		{
			if (EnemyUnitClassInfoList.Instance._list == null)
			{
				EnemyUnitClassInfoList.Instance._list = new List<EnemyUnitClassInfo>();
			}
			AddOrReplace(dict, EnemyUnitClassInfoList.Instance._list, eu => eu.id);
			if (EnemyUnitClassInfoList.Instance._workshopEnemyDict == null)
			{
				EnemyUnitClassInfoList.Instance._workshopEnemyDict = new Dictionary<string, List<EnemyUnitClassInfo>>();
			}
			AddOrReplace(splitDict, EnemyUnitClassInfoList.Instance._workshopEnemyDict, eu => eu.id);
			Dictionary<string, HashSet<int>> usedBookIds = new Dictionary<string, HashSet<int>>();
			Dictionary<string, int> minInjectedIds = new Dictionary<string, int>();
			foreach (var id in BookXmlList.Instance._dictionary.Keys)
			{
				if (!usedBookIds.TryGetValue(id.packageId, out var usedIds))
				{
					usedBookIds[id.packageId] = usedIds = new HashSet<int>();
				}
				usedIds.Add(id.id);
			}
			Dictionary<LorId, Dictionary<string, LorId>> injectedIds = new Dictionary<LorId, Dictionary<string, LorId>>();
			foreach (var tracker in dict.Values)
			{
				var unit = tracker.element;
				foreach (var bookId in unit.bookLorId)
				{
					if (bookId.packageId == unit.workshopID)
					{
						unit.bookId.Add(bookId.id);
					}
					else
					{
						if (!injectedIds.TryGetValue(bookId, out var subDict))
						{
							injectedIds[bookId] = subDict = new Dictionary<string, LorId>();
						}
						if (!subDict.TryGetValue(unit.workshopID, out var injectedId))
						{
							if (!minInjectedIds.TryGetValue(unit.workshopID, out var minInjectedId))
							{
								minInjectedId = MinInjectedId;
							}
							else
							{
								minInjectedId++;
							}
							if (usedBookIds.TryGetValue(unit.workshopID, out var usedIds))
							{
								while (usedIds.Contains(minInjectedId))
								{
									minInjectedId++;
								}
							}
							minInjectedIds[unit.workshopID] = minInjectedId;
							injectedId = subDict[unit.workshopID] = new LorId(unit.workshopID, minInjectedId);
							OrcTools.UnitBookDic[injectedId] = bookId;
						}
						unit.bookId.Add(injectedId.id);
					}
				}
			}
		}


		static void LoadAllStage_MOD(List<ModContent> mods)
		{
			TrackerDict<StageClassInfo_V2> stageDict = new TrackerDict<StageClassInfo_V2>();
			SplitTrackerDict<string, StageClassInfo_V2> splitStageDict = new SplitTrackerDict<string, StageClassInfo_V2>();
			foreach (ModContent modcontent in mods)
			{
				DirectoryInfo _dirInfo = modcontent._dirInfo;
				string workshopId = modcontent._itemUniqueId;
				if (workshopId.ToLower().EndsWith("@origin"))
				{
					workshopId = "";
				}
				try
				{
					if (ExistsModdingPath(_dirInfo, "StageInfo", out var directory))
					{
						LoadStage_MOD(directory, workshopId, stageDict, splitStageDict);
					}
				}
				catch (Exception ex)
				{
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + _dirInfo.Name + "_StaticInfoGiftError.log", ex.ToString());
				}
			}
			AddStageByMod(stageDict, splitStageDict);
		}
		static void LoadStage_MOD(DirectoryInfo dir, string uniqueId, TrackerDict<StageClassInfo_V2> dict, SplitTrackerDict<string, StageClassInfo_V2> splitDict)
		{
			LoadStage_MOD_Checking(dir, uniqueId, dict, splitDict);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadStage_MOD(directories[i], uniqueId, dict, splitDict);
				}
			}
		}
		static void LoadStage_MOD_Checking(DirectoryInfo dir, string uniqueId, TrackerDict<StageClassInfo_V2> dict, SplitTrackerDict<string, StageClassInfo_V2> splitDict)
		{
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					LoadNewStage(File.ReadAllText(fileInfo.FullName), uniqueId, dict, splitDict);
				}
				catch (Exception ex)
				{
					UtilTools.CreateShortcut(Application.dataPath + "/Mods/", "Error from  " + uniqueId + " " + fileInfo.Name, fileInfo.FullName, fileInfo.DirectoryName, "Error Xml Files");
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + "Error in " + uniqueId + " " + fileInfo.Name + ".log", ex.ToString());
				}
			}
		}
		static void AddStageByMod(TrackerDict<StageClassInfo_V2> dict, SplitTrackerDict<string, StageClassInfo_V2> splitDict)
		{
			if (StageClassInfoList.Instance._workshopStageDict == null)
			{
				StageClassInfoList.Instance._workshopStageDict = new Dictionary<string, List<StageClassInfo>>();
			}
			AddOrReplace(splitDict, StageClassInfoList.Instance._workshopStageDict, stage => stage.id);
			AddOrReplace(dict, StageClassInfoList.Instance._list, stage => stage.id);
			ClassifyWorkshopInvitation(dict);
		}
		static void ClassifyWorkshopInvitation(TrackerDict<StageClassInfo_V2> dict)
		{
			var originRecipeList = StageClassInfoList.Instance._recipeCondList;
			var originValueDict = StageClassInfoList.Instance._valueCondList;
			var workshopRecipeList = StageClassInfoList.Instance._workshopRecipeList;
			var workshopValueDict = StageClassInfoList.Instance._workshopValueDict;
			AddOrReplace(dict, originRecipeList, stage => stage.id, stage => stage.id.IsBasic() && stage.invitationInfo.combine == StageCombineType.BookRecipe);
			AddOrReplace(dict, originValueDict[1], stage => stage.id, stage => stage.id.IsBasic() && stage.invitationInfo.combine == StageCombineType.BookValue && stage.invitationInfo.bookNum == 1);
			AddOrReplace(dict, originValueDict[2], stage => stage.id, stage => stage.id.IsBasic() && stage.invitationInfo.combine == StageCombineType.BookValue && stage.invitationInfo.bookNum == 2);
			AddOrReplace(dict, originValueDict[3], stage => stage.id, stage => stage.id.IsBasic() && stage.invitationInfo.combine == StageCombineType.BookValue && stage.invitationInfo.bookNum == 3);
			AddOrReplace(dict, workshopRecipeList, stage => stage.id, stage => stage.id.IsWorkshop() && stage.invitationInfo.combine == StageCombineType.BookRecipe);
			AddOrReplace(dict, workshopValueDict[1], stage => stage.id, stage => stage.id.IsWorkshop() && stage.invitationInfo.combine == StageCombineType.BookValue && stage.invitationInfo.bookNum == 1);
			AddOrReplace(dict, workshopValueDict[2], stage => stage.id, stage => stage.id.IsWorkshop() && stage.invitationInfo.combine == StageCombineType.BookValue && stage.invitationInfo.bookNum == 2);
			AddOrReplace(dict, workshopValueDict[3], stage => stage.id, stage => stage.id.IsWorkshop() && stage.invitationInfo.combine == StageCombineType.BookValue && stage.invitationInfo.bookNum == 3);
			int comparison(StageClassInfo info1, StageClassInfo info2) => (int)(10f * (info2.invitationInfo.bookValue - info1.invitationInfo.bookValue));
			originValueDict[1].Sort(comparison);
			originValueDict[2].Sort(comparison);
			originValueDict[3].Sort(comparison);
			workshopValueDict[1].Sort(comparison);
			workshopValueDict[2].Sort(comparison);
			workshopValueDict[3].Sort(comparison);
		}
		static void LoadNewStage(string str, string uniqueId, TrackerDict<StageClassInfo_V2> dict, SplitTrackerDict<string, StageClassInfo_V2> splitDict)
		{
			StageXmlRoot_V2 xmlRoot;
			using (StringReader stringReader = new StringReader(str))
			{
				xmlRoot = (StageXmlRoot_V2)new XmlSerializer(typeof(StageXmlRoot_V2), StageXmlRoot_V2.Overrides).Deserialize(stringReader);
			}
			foreach (var stage in xmlRoot.list)
			{
				var workshopId = Tools.ClarifyWorkshopId(stage.workshopID, xmlRoot.customPid, uniqueId);
				stage.workshopID = workshopId;
				stage.InitializeIds(workshopId);
				stage.InitOldFields(workshopId);
				if (workshopId != "")
				{
					if (!splitDict.TryGetValue(uniqueId, out var subDict))
					{
						splitDict[uniqueId] = subDict = new TrackerDict<StageClassInfo_V2>();
					}
					subDict[stage.id] = dict[stage.id] = new AddTracker<StageClassInfo_V2>(stage);
				}
				else
				{
					dict[stage.id] = new AddTracker<StageClassInfo_V2>(stage);
				}
			}
		}


		static void LoadAllFloorInfo_MOD(List<ModContent> mods)
		{
			SplitTrackerDict<SephirahType, FloorLevelXmlInfo_V2> levelDict = new SplitTrackerDict<SephirahType, FloorLevelXmlInfo_V2>();
			foreach (ModContent modcontent in mods)
			{
				DirectoryInfo _dirInfo = modcontent._dirInfo;
				string workshopId = modcontent._itemUniqueId;
				if (workshopId.ToLower().EndsWith("@origin"))
				{
					workshopId = "";
				}
				try
				{
					if (ExistsModdingPath(_dirInfo, "FloorLevelInfo", out var directory))
					{
						LoadFloorInfo_MOD(directory, workshopId, levelDict);
					}
				}
				catch (Exception ex)
				{
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + _dirInfo.Name + "_StaticInfoFloorInfoError.log", ex.ToString());
				}
			}
			AddFloorInfoByMod(levelDict);
		}
		static void LoadFloorInfo_MOD(DirectoryInfo dir, string uniqueId, SplitTrackerDict<SephirahType, FloorLevelXmlInfo_V2> dict)
		{
			LoadFloorInfo_MOD_Checking(dir, uniqueId, dict);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadFloorInfo_MOD(directories[i], uniqueId, dict);
				}
			}
		}
		static void LoadFloorInfo_MOD_Checking(DirectoryInfo dir, string uniqueId, SplitTrackerDict<SephirahType, FloorLevelXmlInfo_V2> dict)
		{
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					LoadNewFloorInfo(File.ReadAllText(fileInfo.FullName), uniqueId, dict);
				}
				catch (Exception ex)
				{
					UtilTools.CreateShortcut(Application.dataPath + "/Mods/", "Error from  " + uniqueId + " " + fileInfo.Name, fileInfo.FullName, fileInfo.DirectoryName, "Error Xml Files");
					ModContentManager.Instance.AddErrorLog(ex.ToString());
					File.WriteAllText(Application.dataPath + "/Mods/" + "Error in " + uniqueId + " " + fileInfo.Name + ".log", ex.ToString());
				}
			}
		}
		static void LoadNewFloorInfo(string str, string uniqueId, SplitTrackerDict<SephirahType, FloorLevelXmlInfo_V2> dict)
		{
			FloorLevelXmlRoot_V2 xmlRoot;
			using (StringReader stringReader = new StringReader(str))
			{
				xmlRoot = (FloorLevelXmlRoot_V2)new XmlSerializer(typeof(FloorLevelXmlRoot_V2), FloorLevelXmlRoot_V2.Overrides).Deserialize(stringReader);
			}
			foreach (var floorLevel in xmlRoot.list)
			{
				floorLevel.InitOldFields(uniqueId);
				if (!dict.TryGetValue(floorLevel.sephirahType, out var subdict))
				{
					dict[floorLevel.sephirahType] = subdict = new TrackerDict<FloorLevelXmlInfo_V2>();
				}
				subdict[new LorId(floorLevel.level)] = new AddTracker<FloorLevelXmlInfo_V2>(floorLevel);
				OrcTools.FloorLevelStageDic[floorLevel] = floorLevel.stageLorId;
			}
		}
		static void AddFloorInfoByMod(SplitTrackerDict<SephirahType, FloorLevelXmlInfo_V2> dict)
		{
			AddOrReplace(dict, FloorLevelXmlList.Instance._list, fl => fl.level, fl => fl.sephirahType);
		}

		static readonly int MinInjectedId = 1000000000;
	}
}
