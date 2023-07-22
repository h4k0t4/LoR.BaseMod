using GTMDProjectMoon;
using Mod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using static BaseMod.OptimizedReplacer;

namespace BaseMod
{
	public static class LocalizedTextLoader_New
	{
		static string GetModdingPath(DirectoryInfo dir, string type, string defaultLocale = "default")
		{
			string path = Path.Combine(dir.FullName, "Localize", TextDataModel.CurrentLanguage, type);
			if (!Directory.Exists(path))
			{
				path = Path.Combine(dir.FullName, "Localize", defaultLocale, type);
			}
			return path;
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
		static bool CheckReExportLock()
		{
			return File.Exists(Harmony_Patch.LocalizePath + "/DeleteThisToExportLocalizeAgain");
		}
		static void CreateReExportLock()
		{
			File.WriteAllText(Harmony_Patch.LocalizePath + "/DeleteThisToExportLocalizeAgain", "yes");
		}
		public static void ExportOriginalFiles()
		{
			try
			{
				if (!CheckReExportLock())
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
					CreateReExportLock();
				}
			}
			catch (Exception ex)
			{
				ModContentManager.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
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
		static void ExportBattleDialogues(string language)
		{
			TextAsset[] array = Resources.LoadAll<TextAsset>("Xml/BattleDialogues/" + language + "/");
			for (int i = 0; i < array.Length; i++)
			{
				LocalizeTextExport_str(array[i].text, Harmony_Patch.LocalizePath + "/BattleDialogues", array[i].name);
			}
		}
		static void ExportBattleDialoguesRelations()
		{
			LocalizeTextExport("Xml/BattleDialogues/Book_BattleDlg_Relations", Harmony_Patch.LocalizePath + "/Book_BattleDlg_Relations", "Book_BattleDlg_Relations");
		}
		static void ExportCharactersName(string language)
		{
			TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_CharactersName", language));
			LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/CharactersName", "CharactersName");
			LocalizeTextExport(string.Format("Xml/Localize/{0}/{0}_CreatureName", language), Harmony_Patch.LocalizePath + "/CharactersName", "CreatureName");
		}
		static void ExportLibrariansName(string language)
		{
			TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_NormalLibrariansNamePreset", language));
			LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/NormalLibrariansNamePreset", "NormalLibrariansNamePreset");
		}
		static void ExportStageName(string language)
		{
			TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_StageName", language));
			LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/StageName", "StageName");
		}
		static void ExportPassiveDesc(string language)
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
		static void ExportGiftDesc(string language)
		{
			TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_GiftTexts", language));
			LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/GiftTexts", "GiftTexts");
		}
		static void ExportBattleCardDescriptions(string language)
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
		static void ExportBattleCardAbilityDescriptions(string language)
		{
			LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCardAbilities", language, language), Harmony_Patch.LocalizePath + "/BattleCardAbilities", "BattleCardAbilities");
			LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCardAbilities_elena", language, language), Harmony_Patch.LocalizePath + "/BattleCardAbilities", "BattleCardAbilities_elena");
			LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCardAbilities_Eileen", language, language), Harmony_Patch.LocalizePath + "/BattleCardAbilities", "BattleCardAbilities_Eileen");
			LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCardAbilities_Argalia", language, language), Harmony_Patch.LocalizePath + "/BattleCardAbilities", "BattleCardAbilities_Argalia");
			LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCardAbilities_Tanya", language, language), Harmony_Patch.LocalizePath + "/BattleCardAbilities", "BattleCardAbilities_Tanya");
			LocalizeTextExport(string.Format("Xml/Localize/{0}/{1}_BattleCardAbilities_BandFinal", language, language), Harmony_Patch.LocalizePath + "/BattleCardAbilities", "BattleCardAbilities_BandFinal");
		}
		static void ExportBattleEffectTexts(string language)
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
		static void ExportAbnormalityCardDescriptions(string language)
		{
			TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_AbnormalityCards", language));
			LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/AbnormalityCards", "AbnormalityCards");
		}
		static void ExportAbnormalityAbilityDescription(string language)
		{
			TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_AbnormalityAbilities", language));
			LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/AbnormalityAbilities", "AbnormalityAbilities");
		}
		static void ExportBookDescriptions(string language)
		{
			TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_Books", language));
			LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/Books", "Books");
		}
		static void ExportOpeningLyrics(string language)
		{
			TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_OpeningLyrics", language));
			LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/OpeningLyrics", "_OpeningLyrics");
		}
		static void ExportEndingLyrics(string language)
		{
			TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_EndingLyrics", language));
			LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/EndingLyrics", "EndingLyrics");
		}
		static void ExportBossBirdText(string language)
		{
			TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_Bossbird", language));
			LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/BossBirdText", "BossBirdText");
		}
		static void ExportWhiteNightText(string language)
		{
			TextAsset textAsset = Resources.Load<TextAsset>(string.Format("Xml/Localize/{0}/{0}_WhiteNight", language));
			LocalizeTextExport_str(textAsset.text, Harmony_Patch.LocalizePath + "/WhiteNightText", "WhiteNightText");
		}
		public static void LoadModFiles(List<ModContent> loadedContents)
		{
			try
			{
				SplitTrackerDict<string, BookDesc_V2> bookDict = new SplitTrackerDict<string, BookDesc_V2>();
				SplitTrackerDict<string, LOR_XML.BattleDialogRelationWithBookID> relationDict = new SplitTrackerDict<string, LOR_XML.BattleDialogRelationWithBookID>();
				foreach (ModContent modcontent in loadedContents)
				{
					var config = BasemodConfig.FindBasemodConfig(modcontent._itemUniqueId);
					if (config.IgnoreLocalize)
					{
						continue;
					}
					var defaultLocale = config.DefaultLocale;
					DirectoryInfo _dirInfo = modcontent._dirInfo;
					ModContentInfo _modInfo = modcontent._modInfo;
					string workshopId = _modInfo.invInfo.workshopInfo.uniqueId;
					if (workshopId.ToLower().EndsWith("@origin"))
					{
						workshopId = "";
					}
					try
					{
						string moddingPath = GetModdingPath(_dirInfo, "etc", defaultLocale);
						DirectoryInfo directory = new DirectoryInfo(moddingPath);
						if (Directory.Exists(moddingPath))
						{
							LoadLocalizeFile_MOD(directory);
						}
						moddingPath = GetModdingPath(_dirInfo, "Book_BattleDlg_Relations", defaultLocale);
						directory = new DirectoryInfo(moddingPath);
						if (Directory.Exists(moddingPath))
						{
							LoadBattleDialogues_Relations_MOD(directory, workshopId, relationDict);
						}
						moddingPath = GetModdingPath(_dirInfo, "BattleDialogues", defaultLocale);
						directory = new DirectoryInfo(moddingPath);
						if (Directory.Exists(moddingPath))
						{
							LoadBattleDialogues_MOD(directory, workshopId);
						}
						moddingPath = GetModdingPath(_dirInfo, "CharactersName", defaultLocale);
						directory = new DirectoryInfo(moddingPath);
						if (Directory.Exists(moddingPath))
						{
							LoadCharactersName_MOD(directory, workshopId);
						}
						moddingPath = GetModdingPath(_dirInfo, "NormalLibrariansNamePreset", defaultLocale);
						directory = new DirectoryInfo(moddingPath);
						if (Directory.Exists(moddingPath))
						{
							LoadLibrariansName_MOD(directory, workshopId);
						}
						moddingPath = GetModdingPath(_dirInfo, "StageName", defaultLocale);
						directory = new DirectoryInfo(moddingPath);
						if (Directory.Exists(moddingPath))
						{
							LoadStageName_MOD(directory, workshopId);
						}
						moddingPath = GetModdingPath(_dirInfo, "PassiveDesc", defaultLocale);
						directory = new DirectoryInfo(moddingPath);
						if (Directory.Exists(moddingPath))
						{
							LoadPassiveDesc_MOD(directory, workshopId);
						}
						moddingPath = GetModdingPath(_dirInfo, "GiftTexts", defaultLocale);
						directory = new DirectoryInfo(moddingPath);
						if (Directory.Exists(moddingPath))
						{
							LoadGiftDesc_MOD(directory);
						}
						moddingPath = GetModdingPath(_dirInfo, "BattlesCards", defaultLocale);
						directory = new DirectoryInfo(moddingPath);
						if (Directory.Exists(moddingPath))
						{
							LoadBattleCardDescriptions_MOD(directory, workshopId);
						}
						moddingPath = GetModdingPath(_dirInfo, "BattleCardAbilities", defaultLocale);
						directory = new DirectoryInfo(moddingPath);
						if (Directory.Exists(moddingPath))
						{
							LoadBattleCardAbilityDescriptions_MOD(directory);
						}
						moddingPath = GetModdingPath(_dirInfo, "EffectTexts", defaultLocale);
						directory = new DirectoryInfo(moddingPath);
						if (Directory.Exists(moddingPath))
						{
							LoadBattleEffectTexts_MOD(directory);
						}
						moddingPath = GetModdingPath(_dirInfo, "AbnormalityCards", defaultLocale);
						directory = new DirectoryInfo(moddingPath);
						if (Directory.Exists(moddingPath))
						{
							LoadAbnormalityCardDescriptions_MOD(directory);
						}
						moddingPath = GetModdingPath(_dirInfo, "AbnormalityAbilities", defaultLocale);
						directory = new DirectoryInfo(moddingPath);
						if (Directory.Exists(moddingPath))
						{
							LoadAbnormalityAbilityDescription_MOD(directory);
						}
						moddingPath = GetModdingPath(_dirInfo, "Books", defaultLocale);
						directory = new DirectoryInfo(moddingPath);
						if (Directory.Exists(moddingPath))
						{
							LoadBookDescriptions_MOD(directory, workshopId, bookDict);
						}
					}
					catch (Exception ex)
					{
						ModContentManager.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
						File.WriteAllText(Application.dataPath + "/Mods/" + _dirInfo.Name + "_LTLerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
					}
				}
				AddBookDescriptions_MOD(bookDict);
				AddDialogRelations_MOD(relationDict);
			}
			catch (Exception ex)
			{
				ModContentManager.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
				File.WriteAllText(Application.dataPath + "/Mods/" + "LTLerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}


		public static void LoadLocalizeFile_MOD(DirectoryInfo dir)
		{
			LoadLocalizeFile_MOD_Checking(dir, TextDataModel.textDic);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadLocalizeFile_MOD(directories[i]);
				}
			}
		}


		public static void LoadLocalizeFile_MOD_Checking(DirectoryInfo dir, Dictionary<string, string> dic)
		{
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(File.ReadAllText(fileInfo.FullName));
					foreach (object obj in xmlDocument.SelectNodes("localize/text"))
					{
						XmlNode xmlNode = (XmlNode)obj;
						string key = xmlNode.Attributes.GetNamedItem("id")?.InnerText ?? string.Empty;
						dic[key] = xmlNode.InnerText;
					}
				}
				catch (Exception ex)
				{
					Debug.Log("BasemodLocalize: error loading file at " + fileInfo.FullName);
					Debug.LogException(ex);
				}
			}
		}


		static void LoadBattleDialogues_Relations_MOD(DirectoryInfo dir, string uniqueId, SplitTrackerDict<string, LOR_XML.BattleDialogRelationWithBookID> dict)
		{
			LoadBattleDialogues_Relations_MOD_Checking(dir, uniqueId, dict);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadBattleDialogues_Relations_MOD(directories[i], uniqueId, dict);
				}
			}
		}
		static void LoadBattleDialogues_Relations_MOD_Checking(DirectoryInfo dir, string uniqueId, SplitTrackerDict<string, LOR_XML.BattleDialogRelationWithBookID> dict)
		{
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
					{
						var relationRoot = (BattleDialogRelationRoot_V2)new XmlSerializer(typeof(BattleDialogRelationRoot_V2)).Deserialize(stringReader);
						foreach (var relation in relationRoot.CopyBattleDialogRelationNew(uniqueId))
						{
							if (!string.IsNullOrWhiteSpace(relation.groupName))
							{
								if (!dict.TryGetValue(relation.groupName, out var subdict))
								{
									dict[relation.groupName] = subdict = new TrackerDict<LOR_XML.BattleDialogRelationWithBookID>();
								}
								subdict[new LorId(relation.bookID)] = new AddTracker<LOR_XML.BattleDialogRelationWithBookID>(relation);
							}
						}
					}
				}
				catch (Exception ex)
				{
					Debug.Log("BasemodLocalize: error loading file at " + fileInfo.FullName);
					Debug.LogException(ex);
				}
			}
		}
		static void AddDialogRelations_MOD(SplitTrackerDict<string, LOR_XML.BattleDialogRelationWithBookID> dict)
		{
			AddOrReplace(dict, BattleDialogXmlList.Instance._relationList, relation => relation.bookID, relation => relation.groupName);
		}

		static void LoadBattleDialogues_MOD(DirectoryInfo dir, string uniqueId)
		{
			LoadBattleDialogues_MOD_Checking(dir, uniqueId);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadBattleDialogues_MOD(directories[i], uniqueId);
				}
			}
		}
		static void LoadBattleDialogues_MOD_Checking(DirectoryInfo dir, string uniqueId)
		{
			var dict = BattleDialogXmlList.Instance._dictionary;
			List<LOR_XML.BattleDialogCharacter> list = new List<LOR_XML.BattleDialogCharacter>();
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
					{
						BattleDialogRoot_V2 battleDialogRoot = (BattleDialogRoot_V2)new XmlSerializer(typeof(BattleDialogRoot_V2), BattleDialogRoot_V2.Overrides).Deserialize(stringReader);
						if (!string.IsNullOrWhiteSpace(battleDialogRoot.groupName))
						{
							var rootNew = battleDialogRoot.CopyBattleDialogRootNew(uniqueId);
							dict[battleDialogRoot.groupName] = rootNew;
							list.AddRange(rootNew.characterList);
						}
					}
				}
				catch (Exception ex)
				{
					Debug.Log("BasemodLocalize: error loading file at " + fileInfo.FullName);
					Debug.LogException(ex);
				}
			}		
			BattleDialogXmlList.Instance.AddDialogByMod(list);
		}


		static void LoadCharactersName_MOD(DirectoryInfo dir, string uniqueId)
		{
			LoadCharactersName_MOD_Checking(dir, uniqueId);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadCharactersName_MOD(directories[i], uniqueId);
				}
			}
		}
		static void LoadCharactersName_MOD_Checking(DirectoryInfo dir, string uniqueId)
		{
			var originDict = CharactersNameXmlList.Instance._dictionary;
			var customDict = OrcTools.CharacterNameDic;
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
					{
						CharactersNameRoot_V2 charactersNameRoot = (CharactersNameRoot_V2)new XmlSerializer(typeof(CharactersNameRoot_V2)).Deserialize(stringReader);
						foreach (var name in charactersNameRoot.nameList)
						{
							name.workshopId = Tools.ClarifyWorkshopId(name.workshopId, charactersNameRoot.customPid, uniqueId);
							var id = name.lorId;
							if (id.IsBasic())
							{
								originDict[name.ID] = name.name;
							}
							else
							{
								customDict[id] = name.name;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Debug.Log("BasemodLocalize: error loading file at " + fileInfo.FullName);
					Debug.LogException(ex);
				}
			}
		}


		static void LoadLibrariansName_MOD(DirectoryInfo dir, string uniqueId)
		{
			LoadLibrariansName_MOD_Checking(dir, uniqueId);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadLibrariansName_MOD(directories[i], uniqueId);
				}
			}
		}
		static void LoadLibrariansName_MOD_Checking(DirectoryInfo dir, string uniqueId)
		{
			var originDict = CharactersNameXmlList.Instance._dictionary;
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
					{
						CharactersNameRoot_V2 charactersNameRoot = (CharactersNameRoot_V2)new XmlSerializer(typeof(CharactersNameRoot_V2)).Deserialize(stringReader);
						foreach (var name in charactersNameRoot.nameList)
						{
							name.workshopId = Tools.ClarifyWorkshopIdLegacy(name.workshopId, charactersNameRoot.customPid, uniqueId);
							var id = name.lorId;
							if (id.IsBasic())
							{
								originDict[name.ID] = name.name;
							}
							else
							{
								//customDict[id] = name.name;
								//less important, figure this out later
							}
						}
					}
				}
				catch (Exception ex)
				{
					Debug.Log("BasemodLocalize: error loading file at " + fileInfo.FullName);
					Debug.LogException(ex);
				}
			}
		}


		static void LoadStageName_MOD(DirectoryInfo dir, string uniqueId)
		{
			LoadStageName_MOD_Checking(dir, uniqueId);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadStageName_MOD(directories[i], uniqueId);
				}
			}
		}
		static void LoadStageName_MOD_Checking(DirectoryInfo dir, string uniqueId)
		{
			var originDict = StageNameXmlList.Instance._dictionary;
			var customDict = OrcTools.StageNameDic;
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
					{
						CharactersNameRoot_V2 charactersNameRoot = (CharactersNameRoot_V2)new XmlSerializer(typeof(CharactersNameRoot_V2)).Deserialize(stringReader);
						foreach (var name in charactersNameRoot.nameList)
						{
							name.workshopId = Tools.ClarifyWorkshopId(name.workshopId, charactersNameRoot.customPid, uniqueId);
							var id = name.lorId;
							if (id.IsBasic())
							{
								originDict[name.ID] = name.name;
							}
							else
							{
								customDict[id] = name.name;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Debug.Log("BasemodLocalize: error loading file at " + fileInfo.FullName);
					Debug.LogException(ex);
				}
			}
		}


		static void LoadPassiveDesc_MOD(DirectoryInfo dir, string uniqueId)
		{
			LoadPassiveDesc_MOD_Checking(dir, uniqueId);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadPassiveDesc_MOD(directories[i], uniqueId);
				}
			}
		}
		static void LoadPassiveDesc_MOD_Checking(DirectoryInfo dir, string uniqueId)
		{
			var dict = PassiveDescXmlList.Instance._dictionary;
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
					{
						PassiveDescRoot_V2 passiveDescRoot = (PassiveDescRoot_V2)new XmlSerializer(typeof(PassiveDescRoot_V2), PassiveDescRoot_V2.Overrides).Deserialize(stringReader);
						foreach (var passive in passiveDescRoot.descList)
						{
							passive.workshopID = Tools.ClarifyWorkshopId(passive.workshopID, passiveDescRoot.customPid, uniqueId);
							dict[passive.ID] = passive;
						}
					}
				}
				catch (Exception ex)
				{
					Debug.Log("BasemodLocalize: error loading file at " + fileInfo.FullName);
					Debug.LogException(ex);
				}
			}
		}


		static void LoadGiftDesc_MOD(DirectoryInfo dir)
		{
			LoadGiftDesc_MOD_Checking(dir);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadGiftDesc_MOD(directories[i]);
				}
			}
		}
		static void LoadGiftDesc_MOD_Checking(DirectoryInfo dir)
		{
			var dict = GiftDescXmlList.Instance._dictionary;
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
					{
						LOR_XML.GiftTextRoot giftTextRoot = (LOR_XML.GiftTextRoot)new XmlSerializer(typeof(LOR_XML.GiftTextRoot)).Deserialize(stringReader);
						foreach (var giftDesc in giftTextRoot.giftList)
						{
							dict[giftDesc.id] = giftDesc;
						}
					}
				}
				catch (Exception ex)
				{
					Debug.Log("BasemodLocalize: error loading file at " + fileInfo.FullName);
					Debug.LogException(ex);
				}
			}
		}


		static void LoadBattleCardDescriptions_MOD(DirectoryInfo dir, string uniqueId)
		{
			LoadBattleCardDescriptions_MOD_Checking(dir, uniqueId);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadBattleCardDescriptions_MOD(directories[i], uniqueId);
				}
			}
		}
		static void LoadBattleCardDescriptions_MOD_Checking(DirectoryInfo dir, string uniqueId)
		{
			var dict = BattleCardDescXmlList.Instance._dictionary;
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
					{
						BattleCardDescRoot_V2 cardDescRoot = (BattleCardDescRoot_V2)new XmlSerializer(typeof(BattleCardDescRoot_V2)).Deserialize(stringReader);
						foreach (var card in cardDescRoot.cardDescList)
						{
							card.workshopId = Tools.ClarifyWorkshopId(card.workshopId, cardDescRoot.customPid, uniqueId);
							dict[card.lorId] = card;
						}
					}
				}
				catch (Exception ex)
				{
					Debug.Log("BasemodLocalize: error loading file at " + fileInfo.FullName);
					Debug.LogException(ex);
				}
			}
		}


		static void LoadBattleCardAbilityDescriptions_MOD(DirectoryInfo dir)
		{
			LoadBattleCardAbilityDescriptions_MOD_Checking(dir);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadBattleCardAbilityDescriptions_MOD(directories[i]);
				}
			}
		}
		static void LoadBattleCardAbilityDescriptions_MOD_Checking(DirectoryInfo dir)
		{
			var root = BattleCardAbilityDescXmlList.Instance._dictionary;
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
					{
						LOR_XML.BattleCardAbilityDescRoot descRoot = (LOR_XML.BattleCardAbilityDescRoot)new XmlSerializer(typeof(LOR_XML.BattleCardAbilityDescRoot)).Deserialize(stringReader);
						foreach (var cardDesc in descRoot.cardDescList)
						{
							root[cardDesc.id] = cardDesc;
						}
					}
				}
				catch (Exception ex)
				{
					Debug.Log("BasemodLocalize: error loading file at " + fileInfo.FullName);
					Debug.LogException(ex);
				}
			}
		}


		static void LoadBattleEffectTexts_MOD(DirectoryInfo dir)
		{
			LoadBattleEffectTexts_MOD_Checking(dir);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadBattleEffectTexts_MOD(directories[i]);
				}
			}
		}
		static void LoadBattleEffectTexts_MOD_Checking(DirectoryInfo dir)
		{
			var dict = BattleEffectTextsXmlList._instance._dictionary;
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
					{
						LOR_XML.BattleEffectTextRoot textRoot = (LOR_XML.BattleEffectTextRoot)new XmlSerializer(typeof(LOR_XML.BattleEffectTextRoot)).Deserialize(stringReader);
						foreach (var effectText in textRoot.effectTextList)
						{
							dict[effectText.ID] = effectText;
						}
					}
				}
				catch (Exception ex)
				{
					Debug.Log("BasemodLocalize: error loading file at " + fileInfo.FullName);
					Debug.LogException(ex);
				}
			}
		}


		static void LoadAbnormalityCardDescriptions_MOD(DirectoryInfo dir)
		{
			LoadAbnormalityCardDescriptions_MOD_Checking(dir);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadAbnormalityCardDescriptions_MOD(directories[i]);
				}
			}
		}
		static void LoadAbnormalityCardDescriptions_MOD_Checking(DirectoryInfo dir)
		{
			var dict = AbnormalityCardDescXmlList.Instance._dictionary;
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
					{
						foreach (var sephirah in ((AbnormalityCardsRoot_V2)new XmlSerializer(typeof(AbnormalityCardsRoot_V2), AbnormalityCardsRoot_V2.Overrides).Deserialize(stringReader)).sephirahList)
						{
							sephirah.InitOldFields();
							foreach (var card in sephirah.list)
							{
								dict[card.id] = card;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Debug.Log("BasemodLocalize: error loading file at " + fileInfo.FullName);
					Debug.LogException(ex);
				}
			}
		}


		static void LoadAbnormalityAbilityDescription_MOD(DirectoryInfo dir)
		{
			LoadAbnormalityAbilityDescription_MOD_Checking(dir);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadAbnormalityAbilityDescription_MOD(directories[i]);
				}
			}
		}
		static void LoadAbnormalityAbilityDescription_MOD_Checking(DirectoryInfo dir)
		{
			var dict = AbnormalityAbilityTextXmlList.Instance._dictionary;
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
					{
						foreach (var abilityText in ((LOR_XML.AbnormalityAbilityRoot)new XmlSerializer(typeof(LOR_XML.AbnormalityAbilityRoot)).Deserialize(stringReader)).abnormalityList)
						{
							dict[abilityText.id] = abilityText;
						}
					}
				}
				catch (Exception ex)
				{
					Debug.Log("BasemodLocalize: error loading file at " + fileInfo.FullName);
					Debug.LogException(ex);
				}
			}
		}


		static void LoadBookDescriptions_MOD(DirectoryInfo dir, string uniqueId, SplitTrackerDict<string, BookDesc_V2> dict)
		{
			LoadBookDescriptions_MOD_Checking(dir, uniqueId, dict);
			if (dir.GetDirectories().Length != 0)
			{
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int i = 0; i < directories.Length; i++)
				{
					LoadBookDescriptions_MOD(directories[i], uniqueId, dict);
				}
			}
		}
		static void LoadBookDescriptions_MOD_Checking(DirectoryInfo dir, string uniqueId, SplitTrackerDict<string, BookDesc_V2> dict)
		{
			foreach (FileInfo fileInfo in dir.GetFiles())
			{
				try
				{
					using (StringReader stringReader = new StringReader(File.ReadAllText(fileInfo.FullName)))
					{
						BookDescRoot_V2 bookDescRoot = (BookDescRoot_V2)new XmlSerializer(typeof(BookDescRoot_V2)).Deserialize(stringReader);
						foreach (var book in bookDescRoot.bookDescList)
						{
							book.workshopId = Tools.ClarifyWorkshopId(book.workshopId, bookDescRoot.customPid, uniqueId);
							if (!dict.TryGetValue(book.workshopId, out var subdict))
							{
								dict[book.workshopId] = subdict = new TrackerDict<BookDesc_V2>();
							}
							subdict[book.lorId] = new AddTracker<BookDesc_V2>(book);
						}
					}
				}
				catch (Exception ex)
				{
					Debug.Log("BasemodLocalize: error loading file at " + fileInfo.FullName);
					Debug.LogException(ex);
				}
			}
		}
		static void AddBookDescriptions_MOD(SplitTrackerDict<string, BookDesc_V2> dict)
		{
			if (dict.TryGetValue("", out var originDict))
			{
				AddOrReplace(originDict, BookDescXmlList.Instance._dictionaryOrigin, book => book.bookID);
			}
			AddOrReplace(dict, BookDescXmlList.Instance._dictionaryWorkshop, book => book.bookID, book => book.lorId.IsWorkshop());
		}
	}
}