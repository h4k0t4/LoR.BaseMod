using Battle.DiceAttackEffect;
using GameSave;
using GTMDProjectMoon;
using HarmonyLib;
using LOR_DiceSystem;
using LOR_XML;
using Mod;
using ModSettingTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Xml.Serialization;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using ExtendedLoader;
using static System.Reflection.Emit.OpCodes;
using static HarmonyLib.AccessTools;
using EnumExtenderV2;
using System.Xml.Linq;

namespace BaseMod
{
	public static class Harmony_Patch
	{
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
				return ModContentManager.Instance._loadedContents;
			}
		}
		static bool VoidPre()
		{
			try
			{
				return false;
			}
			catch (Exception ex)
			{
				ModContentManager.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
				File.WriteAllText(Application.dataPath + "/Mods/error.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return true;
		}
		public static void Init()
		{
			try
			{
				path = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
				//IsEditing = false;
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
				try
				{
					CreateShortcuts();
					ExportDocuments();
				}
				catch { }
			}
			catch (Exception ex)
			{
				ModContentManager.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
				File.WriteAllText(Application.dataPath + "/Mods/error.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		static void CreateShortcuts()
		{
			string baseModPath = ModContentManager.Instance.GetModPath("BaseMod");
			UtilTools.CreateShortcut(Application.dataPath + "/Managed/BaseMod/", "BaseMod for Workshop", baseModPath, baseModPath, "Way to BaseMod Files");
			UtilTools.CreateShortcut(Application.dataPath + "/Mods/", "Player.log", SaveManager.savePath + "/Player.log", SaveManager.savePath, "Way to Player.log");
		}
		static void ExportDocuments()
		{
			string baseModPath = ModContentManager.Instance.GetModPath("BaseMod");
			try
			{
				if (File.Exists(baseModPath + "/SteamworkUploader.rar"))
				{
					File.Copy(baseModPath + "/SteamworkUploader.rar", Application.dataPath + "/Managed/BaseMod/SteamworkUploader.rar", true);
				}
			}
			catch { }
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
				ModContentManager.Instance._currentPid = modContent._itemUniqueId;
				DirectoryInfo _dirInfo = modContent._dirInfo;
				Assembly currentAssembly;
				var config = BasemodConfig.FindBasemodConfig(modContent._itemUniqueId);
				if (config.IgnoreStaticFiles)
				{
					continue;
				}
				foreach (FileInfo fileInfo in _dirInfo.GetFiles())
				{
					string errorname = "unknown";
					try
					{
						if (fileInfo.Name.Contains(".dll") && !LoadedAssembly.Contains(fileInfo.FullName))
						{
							LoadedAssembly.Add(fileInfo.Directory.FullName);
							errorname = "LoadAssembly";
							if (IsBasemodDebugMode)
							{
								Debug.Log("Basemod load : " + fileInfo.FullName);
							}
							currentAssembly = Assembly.LoadFile(fileInfo.FullName);
							errorname = "GetAssemblyTypes";
							IEnumerable<Type> types;
							try
							{
								types = currentAssembly.GetTypes();
							}
							catch (ReflectionTypeLoadException r)
							{
								ModContentManager.Instance.AddErrorLog("Load_" + fileInfo.Name + "_" + errorname + "_Error");
								File.WriteAllText(Application.dataPath + "/Mods/Load_" + fileInfo.Name + "_" + errorname + "_Error" + ".log", string.Join("\n", r.LoaderExceptions.Select(e => e.ToString())));
								types = r.Types.Where(t => t != null);
							}
							foreach (Type type in types)
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
								if (type.IsSubclassOf(typeof(QuestMissionScriptBase)) && name.StartsWith("QuestMissionScript_"))
								{
									CustomQuest[name.Substring("QuestMissionScript_".Length).Trim()] = type;
								}
								errorname = "LoadHarmonyPatch";
								if (name == "Harmony_Patch" || (type != null && type.BaseType != null && type.BaseType.Name == "Harmony_Patch"))
								{
									Activator.CreateInstance(type);
								}
							}
							errorname = "LoadOtherTypes";
							LoadTypesFromAssembly(types, fileInfo.Name);
							ModSaveTool.LoadedModsWorkshopId.Add(Tools.GetModId(currentAssembly));
							AssemList.Add(currentAssembly);
						}
					}
					catch (Exception ex)
					{
						ModContentManager.Instance.AddErrorLog("Load_" + fileInfo.Name + "_" + errorname + "_Error");
						File.WriteAllText(Application.dataPath + "/Mods/Load_" + fileInfo.Name + "_" + errorname + "_Error" + ".log", ex.ToString());
					}
				}
				ModContentManager.Instance._currentPid = "";
			}
			CallAllInitializer();
		}
		static void CallAllInitializer()
		{
			foreach (var (pid, filename, initializer) in allInitializers)
			{
				ModContentManager.Instance._currentPid = pid;
				try
				{
					initializer.OnInitializeMod();
				}
				catch (Exception ex)
				{
					ModContentManager.Instance.AddErrorLog($"OnInitialize_{filename}_{initializer.GetType().Name}_Error");
					File.WriteAllText(Application.dataPath + $"/Mods/OnInitialize_{filename}_{initializer.GetType().Name}_Error.log", ex.ToString());
				}
				ModContentManager.Instance._currentPid = "";
			}
			allInitializers.Clear();
		}
		static void LoadTypesFromAssembly(IEnumerable<Type> types, string filename)
		{
			AssemblyManager assemblyManager = AssemblyManager.Instance;
			foreach (Type type in types)
			{
				string name = type.Name;
				if (type.IsSubclassOf(typeof(DiceCardSelfAbilityBase)) && name.StartsWith("DiceCardSelfAbility_"))
				{
					assemblyManager._diceCardSelfAbilityDict.Add(name.Substring("DiceCardSelfAbility_".Length), type);
				}
				else if (type.IsSubclassOf(typeof(DiceCardAbilityBase)) && name.StartsWith("DiceCardAbility_"))
				{
					assemblyManager._diceCardAbilityDict.Add(name.Substring("DiceCardAbility_".Length), type);
				}
				else if (type.IsSubclassOf(typeof(BehaviourActionBase)) && name.StartsWith("BehaviourAction_"))
				{
					assemblyManager._behaviourActionDict.Add(name.Substring("BehaviourAction_".Length), type);
				}
				else if (type.IsSubclassOf(typeof(PassiveAbilityBase)) && name.StartsWith("PassiveAbility_"))
				{
					assemblyManager._passiveAbilityDict.Add(name.Substring("PassiveAbility_".Length), type);
				}
				else if (type.IsSubclassOf(typeof(DiceCardPriorityBase)) && name.StartsWith("DiceCardPriority_"))
				{
					assemblyManager._diceCardPriorityDict.Add(name.Substring("DiceCardPriority_".Length), type);
				}
				else if (type.IsSubclassOf(typeof(EnemyUnitAggroSetter)) && name.StartsWith("EnemyUnitAggroSetter_"))
				{
					assemblyManager._enemyUnitAggroSetterDict.Add(name.Substring("EnemyUnitAggroSetter_".Length), type);
				}
				else if (type.IsSubclassOf(typeof(EnemyTeamStageManager)) && name.StartsWith("EnemyTeamStageManager_"))
				{
					assemblyManager._enemyTeamStageManagerDict.Add(name.Substring("EnemyTeamStageManager_".Length), type);
				}
				else if (type.IsSubclassOf(typeof(EnemyUnitTargetSetter)) && name.StartsWith("EnemyUnitTargetSetter_"))
				{
					assemblyManager._enemyUnitTargetSetterDict.Add(name.Substring("EnemyUnitTargetSetter_".Length), type);
				}
				else if (type.IsSubclassOf(typeof(ModInitializer)))
				{
					allInitializers.Add((ModContentManager.Instance._currentPid, filename, Activator.CreateInstance(type) as ModInitializer));
				}
			}
		}
		public static void LoadModFiles()
		{
			Dictionary<string, List<Workshop.WorkshopSkinData>> _bookSkinData = CustomizingBookSkinLoader.Instance._bookSkinData;
			StaticDataLoader_New.ExportOriginalFiles();
			StaticDataLoader_New.LoadModFiles(LoadedModContents);
			LocalizedTextLoader_New.ExportOriginalFiles();
			LocalizedTextLoader_New.LoadModFiles(LoadedModContents);
			StorySerializer_new.ExportStory();
			LoadBookSkins(_bookSkinData);
			//fix issues caused by some patches possibly running this in an async context
			GetArtWorks();
		}
		static void ReloadModFiles()
		{
			GlobalGameManager.Instance.LoadStaticData();
			StageClassInfoList.Instance.GetAllWorkshopData().Clear();
			EnemyUnitClassInfoList.Instance.GetAllWorkshopData().Clear();
			BookXmlList.Instance.GetAllWorkshopData().Clear();
			CardDropTableXmlList.Instance.GetAllWorkshopData().Clear();
			DropBookXmlList.Instance.GetAllWorkshopData().Clear();
			ItemXmlDataList.instance.GetAllWorkshopData().Clear();
			BookDescXmlList.Instance._dictionaryWorkshop.Clear();
			CustomizingCardArtworkLoader.Instance._artworkData.Clear();
			CustomizingBookSkinLoader.Instance._bookSkinData.Clear();
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
			ModEpMatch.Clear();
			ModStoryCG = null;
			ModWorkShopId.Clear();
			OrcTools.StageNameDic.Clear();
			OrcTools.StageConditionDic.Clear();
			OrcTools.CharacterNameDic.Clear();
			OrcTools.EgoDic.Clear();
#pragma warning disable CS0612 // Type or member is obsolete
			OrcTools.DropItemDic.Clear();
#pragma warning restore CS0612 // Type or member is obsolete
			OrcTools.DropItemDicV2.Clear();
			OrcTools.dialogDetails.Clear();
		}
		static void CopyLoaderThumbsForCompat()
		{
			CoreThumbDic = XLRoot.CoreThumbDic;
			BookThumb = XLRoot.BookThumb;
		}
		static void LoadBookSkins(Dictionary<string, List<Workshop.WorkshopSkinData>> _bookSkinData)
		{
			var customizingBookSkinLoader = CustomizingBookSkinLoader.Instance;
			foreach (ModContent modContent in LoadedModContents)
			{
				var config = BasemodConfig.FindBasemodConfig(modContent._itemUniqueId);
				if (config.IgnoreStaticFiles)
				{
					continue;
				}
				string modDirectory = modContent._dirInfo.FullName;
				string modName = modContent._itemUniqueId;
				string modId = "";
				if (!modName.ToLower().EndsWith("@origin"))
				{
					modId = modName;
				}
				int oldSkins = 0;
				if (customizingBookSkinLoader._bookSkinData.TryGetValue(modName, out List<Workshop.WorkshopSkinData> skinlist))
				{
					oldSkins = skinlist.Count;
				}
				string charDirectory = Path.Combine(modDirectory, "Char");
				List<Workshop.WorkshopSkinData> list = new List<Workshop.WorkshopSkinData>();
				if (Directory.Exists(charDirectory))
				{
					string[] directories = Directory.GetDirectories(charDirectory);
					for (int i = 0; i < directories.Length; i++)
					{
						try
						{
							Workshop.WorkshopAppearanceInfo workshopAppearanceInfo = Workshop.WorkshopAppearanceItemLoader.LoadCustomAppearance(directories[i]);
							if (workshopAppearanceInfo != null)
							{
								string[] array = directories[i].Split(new char[]
								{
								'\\'
								});
								string bookName = array[array.Length - 1];
								workshopAppearanceInfo.path = directories[i];
								workshopAppearanceInfo.uniqueId = modId;
								workshopAppearanceInfo.bookName = "Custom_" + bookName;
								if (workshopAppearanceInfo.isClothCustom)
								{
									list.Add(new Workshop.WorkshopSkinData
									{
										dic = workshopAppearanceInfo.clothCustomInfo,
										dataName = workshopAppearanceInfo.bookName,
										contentFolderIdx = workshopAppearanceInfo.uniqueId,
										id = oldSkins + i
									});
								}
								if (workshopAppearanceInfo.faceCustomInfo != null && workshopAppearanceInfo.faceCustomInfo.Count > 0 && FaceData.GetExtraData(workshopAppearanceInfo.faceCustomInfo) == null)
								{
									XLRoot.LoadFaceCustom(workshopAppearanceInfo.faceCustomInfo, $"mod_{modName}:{bookName}");
								}
							}
						}
						catch (Exception ex)
						{
							Debug.LogError("BaseMod: error loading skin at " + directories[i]);
							Debug.LogError(ex);
						}
					}
					if (_bookSkinData.TryGetValue(modId, out var modSkinList) && modSkinList != null)
					{
						modSkinList.AddRange(list);
					}
					else
					{
						_bookSkinData[modId] = list;
					}
				}
			}
		}
		public static Workshop.WorkshopAppearanceInfo LoadCustomAppearance(string path)
		{
			return Workshop.WorkshopAppearanceItemLoader.LoadCustomAppearance(path);
		}/*
		//Clone special motion characterMotion
		[HarmonyPatch(typeof(Workshop.WorkshopSkinDataSetter), nameof(Workshop.WorkshopSkinDataSetter.SetMotionData))]
		[HarmonyPrefix]
		static void WorkshopSkinDataSetter_SetMotionData_Pre(Workshop.WorkshopSkinDataSetter __instance, ActionDetail motion)
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
		//no longer used
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

			void TryAddMotionSpriteSet(string name, CharacterAppearanceType type)
			{
				Transform transform = characterMotion.transform.Find(name);
				if (transform != null)
				{
					SpriteRenderer spriteRenderer = transform.GetComponent<SpriteRenderer>();
					if (spriteRenderer != null)
					{
						characterMotion.motionSpriteSet.Add(new SpriteSet(spriteRenderer, type));
					}
				}
			}

			TryAddMotionSpriteSet("Customize_Renderer", CharacterAppearanceType.Body);
			TryAddMotionSpriteSet("CustomizePivot/DummyHead", CharacterAppearanceType.Head);
			TryAddMotionSpriteSet("Customize_Renderer_Front", CharacterAppearanceType.Body);
			TryAddMotionSpriteSet("Customize_Renderer_Back", CharacterAppearanceType.Body);
			TryAddMotionSpriteSet("Customize_Renderer_Back_Skin", CharacterAppearanceType.Skin);
			TryAddMotionSpriteSet("Customize_Renderer_Skin", CharacterAppearanceType.Skin);
			TryAddMotionSpriteSet("Customize_Renderer_Front_Skin", CharacterAppearanceType.Skin);
			TryAddMotionSpriteSet("Customize_Renderer_Effect", CharacterAppearanceType.Effect);

			return characterMotion;
		}
		//CharacterSound
		//new version moved to ExtendedLoader
		/*
		[HarmonyPatch(typeof(CharacterSound), nameof(CharacterSound.LoadAudioCoroutine))]
		[HarmonyPrefix]
		static bool CharacterSound_LoadAudioCoroutine_Pre(CharacterSound __instance, string path, List<CharacterSound.ExternalSound> externalSoundList, ref IEnumerator __result)
		{
			try
			{
				__result = LoadAudioCoroutine(path, externalSoundList, __instance._motionSounds);
			}
			catch (Exception ex)
			{
				ModContentManager.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
				File.WriteAllText(Application.dataPath + "/Mods/LoadAudioCoroutineerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return false;
		}
		static IEnumerator LoadAudioCoroutine(string path, List<CharacterSound.ExternalSound> externalSoundList, List<CharacterSound.Sound> motionSounds)
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
								CharacterSound.Sound sound1 = motionSounds.Find(x => x.motion == motion);
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
									motionSounds.Add(sound2);
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
		*/
		//Remove Garbage Projection
		[HarmonyPatch(typeof(CustomCoreBookInventoryModel), nameof(CustomCoreBookInventoryModel.GetBookIdList_CustomCoreBook))]
		[HarmonyPostfix]
		static List<int> CustomCoreBookInventoryModel_GetBookIdList_CustomCoreBook_Post(List<int> idList)
		{
			try
			{
				var bookInventoryModel = BookInventoryModel.Instance;
				var bookXmlList = BookXmlList.Instance;
				idList.RemoveAll(x => bookInventoryModel.GetBookCount(x) < 1 && !(bookXmlList.GetData(x) is BookXmlInfo info && !info.isError && info.optionList != null && info.optionList.Contains(BookOption.Basic)));
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			return idList;
		}
		//save custom title
		[HarmonyPatch(typeof(UnitDataModel), nameof(UnitDataModel.GetSaveData))]
		[HarmonyPostfix]
		static void UnitDataModel_GetSaveData_Post(UnitDataModel __instance, SaveData __result)
		{
			try
			{
				var saveDict = __result.GetDictionarySelf();
				if (OrcTools.GiftAndTitleDic.TryGetValue(__instance.prefixID, out var prefixLorId) && prefixLorId != __instance.prefixID)
				{
					saveDict["prefixID"] = new SaveData(0);
					saveDict.Add("BasemodPrefixID", prefixLorId.GetSaveData());
				}
				if (OrcTools.GiftAndTitleDic.TryGetValue(__instance.postfixID, out var postfixLorId) && postfixLorId != __instance.postfixID)
				{
					saveDict["postfixID"] = new SaveData(0);
					saveDict.Add("BasemodPostfixID", postfixLorId.GetSaveData());
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/SaveCustomTitleerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//load custom title
		[HarmonyPatch(typeof(UnitDataModel), nameof(UnitDataModel.LoadFromSaveData))]
		[HarmonyPostfix]
		static void UnitDataModel_LoadFromSaveData_Post(UnitDataModel __instance, SaveData data)
		{
			try
			{
				SaveData customPrefix = data.GetData("BasemodPrefixID");
				if (customPrefix != null)
				{
					var customPrefixId = LorId.LoadFromSaveData(customPrefix);
					if (OrcTools.CustomPrefixes.TryGetValue(customPrefixId, out var prefix))
					{
						__instance.prefixID = prefix.ID;
					}
				}
				SaveData customPostfix = data.GetData("BasemodPostfixID");
				if (customPostfix != null)
				{
					var customPostfixId = LorId.LoadFromSaveData(customPostfix);
					if (OrcTools.CustomPostfixes.TryGetValue(customPostfixId, out var postfix))
					{
						__instance.postfixID = postfix.ID;
					}
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/LoadCustomTitleerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}

		//Book
		//BookIcon
		[HarmonyPatch(typeof(BookModel), nameof(BookModel.bookIcon), MethodType.Getter)]
		[HarmonyPrefix]
		static bool BookModel_get_bookIcon_Pre(BookModel __instance, ref Sprite __result)
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
		[HarmonyPatch(typeof(BookXmlInfo), nameof(BookXmlInfo.Name), MethodType.Getter)]
		[HarmonyPrefix]
		static bool BookXmlInfo_get_Name_Pre(BookXmlInfo __instance, ref string __result)
		{
			try
			{
				string bookName = BookDescXmlList.Instance.GetBookName(new LorId(__instance.workshopID, __instance.TextId));
				if (!string.IsNullOrWhiteSpace(bookName))
				{
					__result = bookName;
					return false;
				}
			}
			catch { }
			return true;
		}
		//BookStory
		[HarmonyPatch(typeof(BookXmlInfo), nameof(BookXmlInfo.Desc), MethodType.Getter)]
		[HarmonyPrefix]
		static bool BookXmlInfo_get_Desc_Pre(BookXmlInfo __instance, ref List<string> __result)
		{
			try
			{
				List<string> bookText = BookDescXmlList.Instance.GetBookText(new LorId(__instance.workshopID, __instance.TextId));
				if (bookText.Count > 0)
				{
					__result = bookText;
					return false;
				}
			}
			catch { }
			return true;
		}
		//OnlyCards
		[HarmonyPatch(typeof(BookModel), nameof(BookModel.SetXmlInfo))]
		[HarmonyPostfix]
		static void BookModel_SetXmlInfo_Post(BookModel __instance, BookXmlInfo classInfo)
		{
			try
			{
				if (!OrcTools.OnlyCardDic.TryGetValue(classInfo.id, out var onlyCardList))
				{
					return;
				}
				__instance._onlyCards = new List<DiceCardXmlInfo>();
				foreach (LorId id in onlyCardList)
				{
					DiceCardXmlInfo cardItem = ItemXmlDataList.instance.GetCardItem(id);
					if (cardItem == null)
					{
						Debug.LogError("onlycard not found");
					}
					else
					{
						__instance._onlyCards.Add(cardItem);
					}
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/SetOnlyCardserror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//SoulCard
		[HarmonyPatch(typeof(BookModel), nameof(BookModel.CreateSoulCard))]
		[HarmonyPrefix]
		static bool BookModel_CreateSoulCard_Pre(BookModel __instance, int emotionLevel, ref BattleDiceCardModel __result)
		{
			try
			{
				if (__instance._classInfo == null)
				{
					Debug.LogError("BookXmlInfo is null");
					return false;
				}
				if (!OrcTools.SoulCardDic.TryGetValue(__instance._classInfo.id, out var soulCardList))
				{
					return true;
				}
				var soulCardNew = soulCardList.Find((BookSoulCardInfo_New x) => x.emotionLevel == emotionLevel);
				if (soulCardNew == null)
				{
					__result = null;
					return false;
				}
				DiceCardXmlInfo cardItem = ItemXmlDataList.instance.GetCardItem(soulCardNew.lorId);
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
		[HarmonyPatch(typeof(StageNameXmlList), nameof(StageNameXmlList.GetName), new Type[] { typeof(StageClassInfo) })]
		[HarmonyPrefix]
		static bool StageNameXmlList_GetName_Pre(StageClassInfo stageInfo, ref string __result)
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
		[HarmonyPatch(typeof(CharactersNameXmlList), nameof(CharactersNameXmlList.GetName), new Type[] { typeof(LorId) })]
		[HarmonyPrefix]
		static bool CharactersNameXmlList_GetName_Pre(LorId id, ref string __result)
		{
			try
			{
				if (OrcTools.CharacterNameDic.TryGetValue(id, out string characterName))
				{
					__result = characterName;
					return false;
				}
			}
			catch { }
			return true;
		}
		//DropBookName
		[HarmonyPatch(typeof(DropBookXmlInfo), nameof(DropBookXmlInfo.Name), MethodType.Getter)]
		[HarmonyPrefix]
		static bool DropBookXmlInfo_get_Name_Pre(DropBookXmlInfo __instance, ref string __result)
		{
			try
			{
				string dropBookName = TextDataModel.GetText(__instance._targetText, Array.Empty<object>());
				if (!string.IsNullOrWhiteSpace(dropBookName))
				{
					__result = dropBookName;
					return false;
				}
			}
			catch { }
			return true;
		}
		//EnemyBookInject
		[HarmonyPatch(typeof(UnitDataModel), MethodType.Constructor, new Type[] { typeof(LorId), typeof(SephirahType), typeof(bool) })]
		[HarmonyPrefix]
		static void UnitDataModel_ctor_Pre(ref LorId defaultBook)
		{
			if (defaultBook != null && OrcTools.UnitBookDic.TryGetValue(defaultBook, out var realBook))
			{
				defaultBook = realBook;
			}
		}
		[HarmonyPatch(typeof(BookXmlList), nameof(BookXmlList.GetData), new Type[] { typeof(LorId), typeof(bool) })]
		[HarmonyPrefix]
		static void BookXmlList_GetData_Pre(ref LorId id)
		{
			if (id != null && OrcTools.UnitBookDic.TryGetValue(id, out var realBook))
			{
				id = realBook;
			}
		}
		//EnemyDrop
		[HarmonyPatch(typeof(UnitDataModel), nameof(UnitDataModel.SetByEnemyUnitClassInfo))]
		[HarmonyPostfix]
		static void UnitDataModel_SetByEnemyUnitClassInfo_Post(UnitDataModel __instance, EnemyUnitClassInfo classInfo)
		{
			try
			{
				if (OrcTools.CharacterNameDic.TryGetValue(new LorId(classInfo.workshopID, classInfo.nameId), out string characterName))
				{
					__instance._name = characterName;
				}
			}
			catch { }
		}
		//Drop
		[HarmonyPatch(typeof(UnitDataModel), nameof(UnitDataModel.SetEnemyDropTable))]
		[HarmonyPrefix]
		static bool UnitDataModel_SetEnemyDropTable_Pre(UnitDataModel __instance, EnemyUnitClassInfo classInfo)
		{
			try
			{
				if (classInfo == null)
				{
					return false;
				}
#pragma warning disable CS0612 // Type or member is obsolete
				if (!OrcTools.DropItemDicV2.TryGetValue(classInfo.id, out var dropTableListNew) & !OrcTools.DropItemDic.TryGetValue(classInfo.id, out var dropTableListOld))
				{
					return true;
				}
#pragma warning restore CS0612 // Type or member is obsolete
				__instance._dropTable.Clear();
				if (dropTableListNew != null)
				{
					foreach (var tableNew in dropTableListNew)
					{
						DropTable dropTable = __instance._dropTable.TryGetValue(tableNew.emotionLevel, out var drop) ? drop : new DropTable();
						foreach (var dropNew in tableNew.dropItemListNew)
						{
							dropTable.Add(dropNew.prob, dropNew.bookLorId);
						}
						__instance._dropTable[tableNew.emotionLevel] = dropTable;
					}
				}
				if (dropTableListOld != null)
				{
					foreach (var tableNewOld in dropTableListOld)
					{
						DropTable dropTable = __instance._dropTable.TryGetValue(tableNewOld.emotionLevel, out var drop) ? drop : new DropTable();
						foreach (var dropNew in tableNewOld.dropList)
						{
							dropTable.Add(dropNew.prob, dropNew.bookId);
						}
						__instance._dropTable[tableNewOld.emotionLevel] = dropTable;
					}
				}
				__instance._dropBonus = classInfo.dropBonus;
				__instance._expDrop = classInfo.exp;
				return false;
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/SetEnemyDropTableerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return true;
		}
		//DropUI
		[HarmonyPatch(typeof(UIInvitationStageInfoPanel), nameof(UIInvitationStageInfoPanel.SetData))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> UIInvitationStageInfoPanel_SetData_In(IEnumerable<CodeInstruction> instructions)
		{
			bool ready = true;
			foreach (var instruction in instructions)
			{
				yield return instruction;
				if (ready && instruction.IsStloc() && TryGetIntValue(instruction.operand, out var index) && index == 5)
				{
					ready = false;
					yield return new CodeInstruction(Ldarg_0);
					yield return new CodeInstruction(Ldarg_1);
					yield return new CodeInstruction(Ldarg_2);
					yield return new CodeInstruction(Ldloc, 5);
					yield return new CodeInstruction(Call, Method(typeof(Harmony_Patch), nameof(UIInvitationStageInfoPanel_SetData_CheckCustomDrop)));
				}
			}
		}
		internal static bool TryGetIntValue(object operand, out int value)
		{
			if (operand is IConvertible convertible)
			{
				try
				{
					value = convertible.ToInt32(null);
					return true;
				}
				catch
				{
				}
			}
			else
			{
				if (operand is LocalBuilder builder)
				{
					value = builder.LocalIndex;
					return true;
				}
			}
			value = 0;
			return false;
		}
		static void UIInvitationStageInfoPanel_SetData_CheckCustomDrop(UIInvitationStageInfoPanel panel, StageClassInfo stage, UIStoryLine story, List<LorId> dropBookIds)
		{
			CanvasGroup cg = panel.rewardBookList.cg;
			EnemyUnitClassInfoList enemyUnitClassInfoList = EnemyUnitClassInfoList.Instance;
			if (story == UIStoryLine.None && cg.interactable)
			{
				foreach (StageWaveInfo stageWaveInfo in stage.waveList)
				{
					foreach (LorId id in stageWaveInfo.enemyUnitIdList)
					{
						EnemyUnitClassInfo data = enemyUnitClassInfoList.GetData(id);
						if (OrcTools.DropItemDicV2.TryGetValue(data.id, out var listNew))
						{
							foreach (EnemyDropItemTable_V2 tableNew in listNew)
							{
								foreach (var dropNew in tableNew.dropItemListNew)
								{
									if (!dropBookIds.Contains(dropNew.bookLorId))
									{
										dropBookIds.Add(dropNew.bookLorId);
									}
								}
							}
						}
#pragma warning disable CS0612 // Type or member is obsolete
						if (OrcTools.DropItemDic.TryGetValue(data.id, out var listNewOld))
						{
							foreach (EnemyDropItemTable_New tableNewOld in listNewOld)
							{
								foreach (var dropNewOld in tableNewOld.dropList)
								{
									if (!dropBookIds.Contains(dropNewOld.bookId))
									{
										dropBookIds.Add(dropNewOld.bookId);
									}
								}
							}
						}
#pragma warning restore CS0612 // Type or member is obsolete
					}
				}
			}
		}
		//EnemyBattleDialog
		[HarmonyPatch(typeof(UnitDataModel), nameof(UnitDataModel.InitBattleDialogByDefaultBook))]
		[HarmonyPrefix]
		static bool UnitDataModel_InitBattleDialogByDefaultBook_Pre(UnitDataModel __instance, LorId lorId)
		{
			try
			{
				if (lorId.id <= 0)
				{
					return true;
				}
				else
				{
					OrcTools.DialogDetail dialogDetail = OrcTools.DialogDetail.FindDialogInBookID(lorId) ?? OrcTools.DialogDetail.FindDialogInCharacterID(lorId);
					if (dialogDetail == null)
					{
						return true;
					}
					BattleDialogCharacter characterData = null;
					if (!string.IsNullOrWhiteSpace(dialogDetail.GroupName))
					{
						characterData = BattleDialogXmlList.Instance.GetCharacterData(dialogDetail.GroupName, dialogDetail.CharacterID.id.ToString());
					}
					if (characterData == null)
					{
						characterData = BattleDialogXmlList.Instance.GetCharacterData_Mod(lorId.packageId, dialogDetail.CharacterID.id);
					}
					Type type = FindBattleDialogueModel(dialogDetail.CharacterName, characterData?.characterID);
					if (type == null)
					{
						__instance.battleDialogModel = new BattleDialogueModel(characterData);
						return false;
					}
					else
					{
						__instance.battleDialogModel = Activator.CreateInstance(type, new object[] { characterData }) as BattleDialogueModel;
						return false;
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
			if (string.IsNullOrWhiteSpace(name))
			{
				name = null;
			}
			else
			{
				name = name.Trim();
				if (CustomBattleDialogModel.TryGetValue(name, out Type type))
				{
					return type;
				}
			}

			if (string.IsNullOrWhiteSpace(id))
			{
				id = null;
			}
			else
			{
				id = id.Trim();
				if (CustomBattleDialogModel.TryGetValue(id, out Type type2))
				{
					return type2;
				}
			}

			Type result = null;
			if (!CoreDialogsLoaded)
			{
				var baseType = typeof(BattleDialogueModel);
				foreach (Type type3 in Assembly.Load("Assembly-CSharp").GetTypes())
				{
					if (baseType.IsAssignableFrom(type3) && type3.Name.StartsWith("BattleDialogueModel_"))
					{
						var typeName = type3.Name.Substring("BattleDialogueModel_".Length);
						if (!CustomBattleDialogModel.ContainsKey(typeName))
						{
							CustomBattleDialogModel[typeName] = type3;
							if (typeName == name || typeName == id && result == null)
							{
								result = type3;
							}
						}
					}
				}
				CoreDialogsLoaded = true;
			}
			return result;
		}
		//防止被动过多炸UI，存在字体问题待解决
		[HarmonyPatch(typeof(BattleUnitInformationUI_PassiveList), nameof(BattleUnitInformationUI_PassiveList.SetData))]
		[HarmonyPrefix]
		static void BattleUnitInformationUI_PassiveList_SetData_Pre(BattleUnitInformationUI_PassiveList __instance, List<PassiveAbilityBase> passivelist)
		{
			try
			{
				int unhidden = passivelist.Count(passive => !passive.isHide) + 1;
				if (__instance.passiveSlotList.Count < unhidden)
				{
					__instance.passiveSlotList.Capacity = unhidden;
					var copyHelper = __instance.passiveSlotList[0].Rect.GetComponent<BattleUnitInformationPassiveSlotCopyHelper>();
					if (copyHelper == null)
					{
						copyHelper = __instance.passiveSlotList[0].Rect.gameObject.AddComponent<BattleUnitInformationPassiveSlotCopyHelper>();
						copyHelper.Init(__instance.passiveSlotList[0]);
					}
					for (int i = __instance.passiveSlotList.Count; i < unhidden; i++)
					{
						__instance.passiveSlotList.Add(copyHelper.CopyThis());
					}
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/BUIUIPSerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		private class BattleUnitInformationPassiveSlotCopyHelper : MonoBehaviour
		{
			public TextMeshProUGUI txt_PassiveDesc;

			public Image img_Icon;

			public Image img_IconGlow;

			public RectTransform Rect;

			public void Init(BattleUnitInformationUI_PassiveList.BattleUnitInformationPassiveSlot origin)
			{
				txt_PassiveDesc = origin.txt_PassiveDesc;
				img_Icon = origin.img_Icon;
				img_IconGlow = origin.img_IconGlow;
				Rect = origin.Rect;
			}

			public BattleUnitInformationUI_PassiveList.BattleUnitInformationPassiveSlot CopyThis()
			{
				var helperCopy = Instantiate(this, transform.parent);
				helperCopy.gameObject.SetActive(false);
				var result = new BattleUnitInformationUI_PassiveList.BattleUnitInformationPassiveSlot
				{
					Rect = helperCopy.Rect,
					txt_PassiveDesc = helperCopy.txt_PassiveDesc,
					img_Icon = helperCopy.img_Icon,
					img_IconGlow = helperCopy.img_IconGlow
				};
				return result;
			}
		}
		//Card
		//CardName
		[HarmonyPatch(typeof(BattleDiceCardModel), nameof(BattleDiceCardModel.GetName))]
		[HarmonyPrefix]
		static bool BattleDiceCardModel_GetName_Pre(BattleDiceCardModel __instance, ref string __result)
		{
			try
			{
				string cardName;
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
		[HarmonyPatch(typeof(DiceCardItemModel), nameof(DiceCardItemModel.GetName))]
		[HarmonyPrefix]
		public static bool DiceCardItemModel_GetName_Pre(DiceCardItemModel __instance, ref string __result)
		{
			try
			{
				string cardName;
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
		[HarmonyPatch(typeof(DiceCardXmlInfo), nameof(DiceCardXmlInfo.Name), MethodType.Getter)]
		[HarmonyPrefix]
		static bool DiceCardXmlInfo_get_Name_Pre(DiceCardXmlInfo __instance, ref string __result)
		{
			try
			{
				string cardName;
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
		[HarmonyPatch(typeof(BattleDiceCardUI), nameof(BattleDiceCardUI.SetCard))]
		[HarmonyPostfix]
		[HarmonyPriority(Priority.High)]
		static void BattleDiceCardUI_SetCard_Post(BattleDiceCardUI __instance, BattleDiceCardModel cardModel)
		{
			try
			{
				string cardName;
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
					__instance.txt_cardName.text = cardName;
				}
			}
			catch { }
		}
		//Get CardSprite
		[HarmonyPatch(typeof(AssetBundleManagerRemake), nameof(AssetBundleManagerRemake.LoadCardSprite))]
		[HarmonyPrefix]
		static bool AssetBundleManagerRemake_LoadCardSprite_Pre(string name, ref Sprite __result)
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
		[HarmonyPatch(typeof(CustomizingCardArtworkLoader), nameof(CustomizingCardArtworkLoader.GetSpecificArtworkSprite))]
		[HarmonyPostfix]
		static Sprite CustomizingCardArtworkLoader_GetSpecificArtworkSprite_Post(Sprite cardArtwork, string name)
		{
			try
			{
				if (cardArtwork == null)
				{
					cardArtwork = AssetBundleManagerRemake.Instance.LoadCardSprite(name);
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/LoadCardSprite2error.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return cardArtwork;
		}
		//If has script and is not creating a new playingcard,return _script
		/*[HarmonyPatch(typeof(BattleDiceCardModel), nameof(BattleDiceCardModel.CreateDiceCardSelfAbilityScript))]
		[HarmonyPrefix]
		static bool BattleDiceCardModel_CreateDiceCardSelfAbilityScript_Pre(BattleDiceCardModel __instance, ref DiceCardSelfAbilityBase __result)
		{
			try
			{
				if (__instance._script != null)
				{
					System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
					string name = stackTrace.GetFrame(2).GetMethod().Name;
					if (!name.Contains("AddCard") && !name.Contains("AllowTargetChanging"))
					{
						__result = __instance._script;
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/ScriptFixPreerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return true;
		}*/
		//as it turns out, unconditionally persisting the original script causes issues with more than one thing
		//(pinocchio playing cards as wrong owner, wolf ego stacking without resetting, and probably others)
		//to not completely ruin compatibility going forwards, persisting must be disabled
		//however, type-checking is okay, and so is creating fake cards for scripts to not null-reference, so this is still done
		//if anything still needs persistent data, the "main" script can now be accessed as "card.card._script"
		[HarmonyPatch(typeof(BattleDiceCardModel), nameof(BattleDiceCardModel.CreateDiceCardSelfAbilityScript))]
		[HarmonyPostfix]
		static void BattleDiceCardModel_CreateDiceCardSelfAbilityScript_Post(BattleDiceCardModel __instance, ref DiceCardSelfAbilityBase __result)
		{
			try
			{
				if (__result == null)
				{
					__instance._script = null;
					return;
				}
				if (__instance._script == null || __instance._script.GetType() != __result.GetType())
				{
					__instance._script = __result;
				}
				else
				{
					FixScriptCard(__instance, __instance._script);
				}
				FixScriptCard(__instance, __result);
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/ScriptFixPosterror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		static void FixScriptCard(BattleDiceCardModel card, DiceCardSelfAbilityBase script)
		{
			if (script.card == null)
			{
				script.card = new BattlePlayingCardDataInUnitModel
				{
					owner = card.owner,
					card = card,
					cardAbility = script
				};
			}
			else
			{
				if (!script.card.isKeepedCard)
				{
					script.card.cardAbility = script;
				}
				if (script.card.owner == null)
				{
					script.card.owner = card.owner;
				}
			}
		}

		//Apply owner for personal card
		/*
		[HarmonyPatch(typeof(BattlePersonalEgoCardDetail), nameof(BattlePersonalEgoCardDetail.AddCard), new Type[] { typeof(LorId) })]
		[HarmonyPrefix]
		static bool BattlePersonalEgoCardDetail_AddCard_Pre(BattlePersonalEgoCardDetail __instance, LorId cardId)
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
				if (__instance._cardsAll.FindAll((BattleDiceCardModel x) => x.GetID() == cardId).Count == 0)
				{
					BattleDiceCardModel battleDiceCardModel = BattleDiceCardModel.CreatePlayingCard(cardItem);
					battleDiceCardModel.owner = __instance._self;
					__instance._cardsAll.Add(battleDiceCardModel);
					__instance._cardInHand.Add(battleDiceCardModel);
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
		*/
		[HarmonyPatch(typeof(BattlePersonalEgoCardDetail), nameof(BattlePersonalEgoCardDetail.AddCard), new Type[] { typeof(LorId) })]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> BattlePersonalEgoCardDetail_AddCard_In(IEnumerable<CodeInstruction> instructions)
		{
			var method = Method(typeof(BattleDiceCardModel), nameof(BattleDiceCardModel.CreatePlayingCard));
			foreach (var instruction in instructions)
			{
				yield return instruction;
				if (instruction.Is(Call, method))
				{
					yield return new CodeInstruction(Dup);
					yield return new CodeInstruction(Ldarg_0);
					yield return new CodeInstruction(Ldfld, Field(typeof(BattlePersonalEgoCardDetail), nameof(BattlePersonalEgoCardDetail._self)));
					yield return new CodeInstruction(Stfld, Field(typeof(BattleDiceCardModel), nameof(BattleDiceCardModel.owner)));
				}
			}
		}
		//CardSelfAbilityKeywords
		[HarmonyPatch(typeof(BattleCardAbilityDescXmlList), nameof(BattleCardAbilityDescXmlList.GetAbilityKeywords_byScript))]
		[HarmonyPrefix]
		[HarmonyPriority(Priority.HigherThanNormal)]
		static void BattleCardAbilityDescXmlList_GetAbilityKeywords_byScript_Pre(BattleCardAbilityDescXmlList __instance, string scriptName)
		{
			if (!__instance._dictionaryKeywordCache.ContainsKey(scriptName))
			{
				try
				{
					if (AssemblyManager.Instance.CreateInstance_DiceCardAbility(scriptName) is DiceCardAbilityBase cardAbility)
					{
						__instance._dictionaryKeywordCache[scriptName] = cardAbility.Keywords.ToList();
						return;
					}
				}
				catch (Exception ex)
				{
					File.WriteAllText(Application.dataPath + "/Mods/KeywordsbyScripterror.log", ex.Message + Environment.NewLine + ex.StackTrace);
				}
				try
				{
					if (AssemblyManager.Instance.CreateInstance_DiceCardSelfAbility(scriptName) is DiceCardSelfAbilityBase cardSelfAbility)
					{
						__instance._dictionaryKeywordCache[scriptName] = cardSelfAbility.Keywords.ToList();
						return;
					}
				}
				catch (Exception ex)
				{
					File.WriteAllText(Application.dataPath + "/Mods/KeywordsbyScripterror.log", ex.Message + Environment.NewLine + ex.StackTrace);
				}
				__instance._dictionaryKeywordCache[scriptName] = new List<string>();
			}
		}

		//RangeSpecial
		[HarmonyPatch(typeof(UISpriteDataManager), nameof(UISpriteDataManager.GetRangeIconSprite))]
		[HarmonyPrefix]
		static bool UISpriteDataManager_GetRangeIconSprite_Pre(ref Sprite __result, CardRange range)
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
		[HarmonyPatch(typeof(BattleDiceCardBuf), nameof(BattleDiceCardBuf.GetBufIcon))]
		[HarmonyPrefix]
		static bool BattleDiceCardBuf_GetBufIcon_Pre(BattleDiceCardBuf __instance, ref Sprite __result)
		{
			if (ArtWorks == null)
			{
				GetArtWorks();
			}
			if (!__instance._iconInit)
			{
				try
				{
					string keywordIconId = __instance.keywordIconId;
					if (!string.IsNullOrWhiteSpace(keywordIconId) && ArtWorks.TryGetValue("CardBuf_" + keywordIconId, out Sprite sprite) && sprite != null)
					{
						__instance._iconInit = true;
						__instance._bufIcon = sprite;
						__result = sprite;
						return false;
					}
				}
				catch { }
			}
			return true;
		}
		//costtozero real
		/*
		[HarmonyPatch(typeof(BattleDiceCardModel), nameof(BattleDiceCardModel.GetCost))]
		[HarmonyPrefix]
		static bool BattleDiceCardModel_GetCost_Pre(ref int __result, BattleDiceCardModel __instance)
		{
			try
			{
				if (__instance._script != null && __instance._script.IsFixedCost())
				{
					__result = __instance._xmlData.Spec.Cost;
					return false;
				}
				int baseCost = __instance._curCost;
				foreach (BattleDiceCardBuf battleDiceCardBuf in __instance._bufList)
				{
					baseCost = battleDiceCardBuf.GetCost(baseCost);
				}
				int abilityCostAdder = 0;
				if (__instance.owner != null)
				{
					if (!__instance.XmlData.IsPersonal())
					{
						abilityCostAdder += __instance.owner.emotionDetail.GetCardCostAdder(__instance);
						abilityCostAdder += __instance.owner.bufListDetail.GetCardCostAdder(__instance);
					}
					if (__instance._script != null)
					{
						abilityCostAdder += __instance._script.GetCostAdder(__instance.owner, __instance);
					}
				}
				int finalCost = baseCost + __instance._costAdder + abilityCostAdder;
				if (__instance._costZero)
				{
					finalCost = 0;
				}
				if (__instance.owner != null && __instance._script != null && __instance._script != null)
				{
					finalCost = __instance._script.GetCostLast(__instance.owner, __instance, finalCost);
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
		*/
		//costtozero real
		//also apply script GetCostAdder and GetCostLast for personal/ego
		[HarmonyPatch(typeof(BattleDiceCardModel), nameof(BattleDiceCardModel.GetCost))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> BattleDiceCardModel_GetCost_In(IEnumerable<CodeInstruction> instructions, ILGenerator ilgen)
		{
			var codes = instructions.ToList();
			CodeInstruction jump = null;
			var ownerField = Field(typeof(BattleDiceCardModel), nameof(BattleDiceCardModel.owner));
			var scriptField = Field(typeof(BattleDiceCardModel), nameof(BattleDiceCardModel._script));
			var fixMethod = Method(typeof(Harmony_Patch), nameof(BattleDiceCardModel_GetCost_TryFixOwnerForScript));
			int i = 0;
			for (; i < codes.Count; i++)
			{
				if (codes[i].Is(Ldfld, ownerField) && codes[i + 1].Branches(out var label))
				{
					codes.InsertRange(i + 1, new CodeInstruction[]
					{
						new CodeInstruction(Ldarg_0),
						new CodeInstruction(Call, fixMethod)
					});
					jump = new CodeInstruction(Brtrue, label);
					codes.InsertRange(i + 4, new CodeInstruction[]
					{
						new CodeInstruction(Ldarg_0),
						new CodeInstruction(Call, Method(typeof(Harmony_Patch), nameof(BattleDiceCardModel_GetCost_IsOnlySelfScript))),
						jump
					});

					for (i += 7; i < codes.Count; i++)
					{
						if (codes[i].opcode == Ldarg_0 && codes[i + 1].Is(Ldfld, scriptField))
						{
							var newLabel = ilgen.DefineLabel();
							codes[i].labels.Add(newLabel);
							jump.operand = newLabel;
							i += 2;
							break;
						}
					}
					break;
				}
			}
			bool checkZero = true;
			for (; i < codes.Count; i++)
			{
				if (checkZero && codes[i].opcode == Add && codes[i + 1].opcode == Stloc_2)
				{
					codes.InsertRange(i + 2, new CodeInstruction[]
					{
						new CodeInstruction(Ldloca, 2),
						new CodeInstruction(Ldarg_0),
						new CodeInstruction(Call, Method(typeof(Harmony_Patch), nameof(BattleDiceCardModel_GetCost_CheckZero)))
					});
					checkZero = false;
				}
				else if (codes[i].Is(Ldfld, ownerField))
				{
					codes.InsertRange(i + 1, new CodeInstruction[]
					{
						new CodeInstruction(Ldarg_0),
						new CodeInstruction(Call, fixMethod)
					});
				}
			}
			return codes;
		}
		static void BattleDiceCardModel_GetCost_CheckZero(ref int cost, BattleDiceCardModel card)
		{
			if (card._costZero)
			{
				cost = 0;
			}
		}
		static bool BattleDiceCardModel_GetCost_IsOnlySelfScript(BattleDiceCardModel card)
		{
			return card.XmlData.IsEgo() || card.XmlData.IsPersonal() || card.owner == null;
		}
		static BattleUnitModel BattleDiceCardModel_GetCost_TryFixOwnerForScript(BattleUnitModel owner, BattleDiceCardModel card)
		{
			return owner ?? card?._script?.card?.owner;
		}

		//set script owner for floor ego and other unusual cards (so that cost scripts can work for them too)
		[HarmonyPatch(typeof(BattleUnitCardsInHandUI), nameof(BattleUnitCardsInHandUI.UpdateCardList))]
		[HarmonyPostfix]
		[HarmonyPriority(Priority.VeryLow)]
		static void BattleUnitCardsInHandUI_UpdateCardList_Post(BattleUnitCardsInHandUI __instance)
		{
			try
			{
				bool reload = false;
				var owner = __instance.SelectedModel ?? __instance.HOveredModel;
				if (owner == null)
				{
					return;
				}
				foreach (var card in __instance._activatedCardList)
				{
					var bcard = card.CardModel?._script?.card;
					if (bcard != null && bcard.owner != owner)
					{
						bcard.owner = owner;
						reload = true;
					}
				}
				if (reload && !reloadGuard)
				{
					reloadGuard = true;
					try
					{
						__instance.UpdateCardList();
					}
					catch { }
					reloadGuard = false;
				}
			}
			catch { }
		}
		static bool reloadGuard = false;

		//BattleDiceBehavior ignorepower lead to dicevalue zero
		[HarmonyPatch(typeof(BattleDiceBehavior), nameof(BattleDiceBehavior.UpdateDiceFinalValue))]
		[HarmonyPrefix]
		[HarmonyPriority(Priority.Last)]
		static void BattleDiceBehavior_UpdateDiceFinalValue_Pre(BattleDiceBehavior __instance)
		{
			__instance._diceFinalResultValue = Math.Max(1, __instance._diceResultValue);
		}

		//Passive
		//PassiveName
		[HarmonyPatch(typeof(BookPassiveInfo), nameof(BookPassiveInfo.name), MethodType.Getter)]
		[HarmonyPrefix]
		static bool BookPassiveInfo_get_name_Pre(BookPassiveInfo __instance, ref string __result)
		{
			try
			{
				string passiveName = PassiveDescXmlList.Instance.GetName(__instance.passive.id);
				if (!string.IsNullOrWhiteSpace(passiveName))
				{
					__result = passiveName;
					return false;
				}
			}
			catch { }
			return true;
		}
		//PassiveDesc
		[HarmonyPatch(typeof(BookPassiveInfo), nameof(BookPassiveInfo.desc), MethodType.Getter)]
		[HarmonyPrefix]
		static bool BookPassiveInfo_get_desc_Pre(BookPassiveInfo __instance, ref string __result)
		{
			try
			{
				string passiveDesc = PassiveDescXmlList.Instance.GetDesc(__instance.passive.id);
				if (!string.IsNullOrWhiteSpace(passiveDesc))
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
		/*
		[HarmonyPatch(typeof(BattleUnitBufListDetail), nameof(BattleUnitBufListDetail.OnRoundStart))]
		[HarmonyPrefix]
		static bool BattleUnitBufListDetail_OnRoundStart_Pre(BattleUnitBufListDetail __instance)
		{
			try
			{
				foreach (BattleUnitBuf ReadyBuf in __instance._readyBufList)
				{
					if (!ReadyBuf.IsDestroyed())
					{
						BattleUnitBuf buf = __instance._bufList.Find((BattleUnitBuf x) => x.GetType() == ReadyBuf.GetType() && !x.IsDestroyed());
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
				__instance._readyBufList.Clear();
				foreach (BattleUnitBuf ReadyReadyBuf in __instance._readyReadyBufList)
				{
					if (!ReadyReadyBuf.IsDestroyed())
					{
						BattleUnitBuf rbuf = __instance._readyBufList.Find((BattleUnitBuf x) => x.GetType() == ReadyReadyBuf.GetType() && !x.IsDestroyed());
						if (rbuf != null && !ReadyReadyBuf.independentBufIcon && rbuf.GetBufIcon() != null)
						{
							rbuf.stack += ReadyReadyBuf.stack;
							rbuf.OnAddBuf(ReadyReadyBuf.stack);
						}
						else
						{
							__instance._readyBufList.Add(ReadyReadyBuf);
							ReadyReadyBuf.OnAddBuf(ReadyReadyBuf.stack);
						}
					}
				}
				__instance._readyReadyBufList.Clear();
				if (__instance._self.faction == Faction.Player && StageController.Instance.GetStageModel().ClassInfo.chapter == 3)
				{
					int kewordBufStack = __instance.GetKewordBufStack(KeywordBuf.Endurance);
					__instance._self.UnitData.historyInStage.maxEndurance = Mathf.Max(__instance._self.UnitData.historyInStage.maxEndurance, kewordBufStack);
				}
				foreach (BattleUnitBuf battleUnitBuf3 in __instance._bufList.ToArray())
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
		*/
		[HarmonyPatch(typeof(BattleUnitBufListDetail), nameof(BattleUnitBufListDetail.OnRoundStart))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> BattleUnitBufListDetail_OnRoundStart_In(IEnumerable<CodeInstruction> instructions, ILGenerator ilgen)
		{
			var newBufLocal = ilgen.DeclareLocal(typeof(BattleUnitBuf));
			var oldBufListLocal = ilgen.DeclareLocal(typeof(BattleUnitBuf));
			var bufPredicateLocal = ilgen.DeclareLocal(typeof(Predicate<BattleUnitBuf>));
			var currentBufProperty = PropertyGetter(typeof(List<BattleUnitBuf>.Enumerator), nameof(List<BattleUnitBuf>.Enumerator.Current));
			var independentIconProperty = PropertyGetter(typeof(BattleUnitBuf), nameof(BattleUnitBuf.independentBufIcon));
			var iconGetterMethod = Method(typeof(BattleUnitBuf), nameof(BattleUnitBuf.GetBufIcon));
			var unityEqualityMethod = Method(typeof(UnityEngine.Object), "op_Equality", new Type[] { typeof(UnityEngine.Object), typeof(UnityEngine.Object) });
			var bufFindMethod = Method(typeof(List<BattleUnitBuf>), nameof(List<BattleUnitBuf>.Find));
			var helper = Method(typeof(Harmony_Patch), nameof(BattleUnitBufListDetail_OnRoundStart_FindSameTypeBuf));
			foreach (var instruction in instructions)
			{
				var patchingFind = instruction.Is(Callvirt, bufFindMethod);
				if (patchingFind)
				{
					yield return new CodeInstruction(Stloc, bufPredicateLocal);
					yield return new CodeInstruction(Dup);
					yield return new CodeInstruction(Stloc, oldBufListLocal);
					yield return new CodeInstruction(Ldloc, bufPredicateLocal);
				}
				yield return instruction;
				if (patchingFind)
				{
					yield return new CodeInstruction(Ldloc, newBufLocal);
					yield return new CodeInstruction(Ldloc, oldBufListLocal);
					yield return new CodeInstruction(Call, helper);
				}
				else if (instruction.Is(Call, currentBufProperty))
				{
					yield return new CodeInstruction(Dup);
					yield return new CodeInstruction(Stloc, newBufLocal);
				}
				else if (instruction.Is(Callvirt, independentIconProperty))
				{
					yield return new CodeInstruction(Ldloc, newBufLocal);
					yield return new CodeInstruction(Callvirt, iconGetterMethod);
					yield return new CodeInstruction(Ldnull);
					yield return new CodeInstruction(Call, unityEqualityMethod);
					yield return new CodeInstruction(Or);
				}
			}
		}
		//if matching buf was already found, return it; otherwise do a new search according to the alternative condition
		static BattleUnitBuf BattleUnitBufListDetail_OnRoundStart_FindSameTypeBuf(BattleUnitBuf oldBuf, BattleUnitBuf newBuf, List<BattleUnitBuf> list)
		{
			if (oldBuf != null)
			{
				return oldBuf;
			}
			return list.Find((BattleUnitBuf x) => x.GetType() == newBuf.GetType() && !x.IsDestroyed());
		}
		//CanAddBuf Apply owner
		[HarmonyPatch(typeof(BattleUnitBufListDetail), nameof(BattleUnitBufListDetail.CanAddBuf))]
		[HarmonyPrefix]
		static void BattleUnitBufListDetail_CanAddBuf_Pre(BattleUnitBufListDetail __instance, BattleUnitBuf buf)
		{
			try
			{
				if (buf == null || __instance._self == null)
				{
					return;
				}
				buf._owner = __instance._self;
			}
			catch { }
		}
		//GetBufIcon
		[HarmonyPatch(typeof(BattleUnitBuf), nameof(BattleUnitBuf.GetBufIcon))]
		[HarmonyPrefix]
		static void BattleUnitBuf_GetBufIcon_Pre(BattleUnitBuf __instance)
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
						BattleUnitBuf._bufIconDictionary[array[i].name] = array[i];
					}
				}
			}
			string keywordIconId = __instance.keywordIconId;
			if (!string.IsNullOrWhiteSpace(keywordIconId) && !BattleUnitBuf._bufIconDictionary.ContainsKey(keywordIconId) && ArtWorks.TryGetValue(keywordIconId, out Sprite sprite))
			{
				BattleUnitBuf._bufIconDictionary[keywordIconId] = sprite;
			}
		}

		//EmotionCard
		//EmotionCardAbilityApply
		/*
		[HarmonyPatch(typeof(BattleEmotionCardModel), MethodType.Constructor, new Type[] { typeof(EmotionCardXmlInfo), typeof(BattleUnitModel) })]
		[HarmonyPostfix]
		static void BattleEmotionCardModel_ctor_Post(BattleEmotionCardModel __instance, EmotionCardXmlInfo xmlInfo, BattleUnitModel owner)
		{
			try
			{
				if (__instance._xmlInfo == null)
				{
					__instance._xmlInfo = xmlInfo;
				}
				if (__instance._owner == null)
				{
					__instance._owner = owner;
				}
				if (__instance._abilityList == null)
				{
					__instance._abilityList = new List<EmotionCardAbilityBase>();
				}
				foreach (string text in xmlInfo.Script)
				{
					EmotionCardAbilityBase emotionCardAbilityBase = FindEmotionCardAbility(text.Trim());
					if (emotionCardAbilityBase != null)
					{
						emotionCardAbilityBase.SetEmotionCard(__instance);
						__instance._abilityList.RemoveAll(x => x.GetType().Name.Substring("EmotionCardAbility_".Length).Trim() == text);
						__instance._abilityList.Add(emotionCardAbilityBase);
					}
				}/*
				List<string> list = new List<string>();
				list.AddRange(xmlInfo.Script);
				foreach (EmotionCardAbilityBase emotionCardAbility in __instance._abilityList)
				{
					list.Remove(emotionCardAbility.GetType().Name.Substring("EmotionCardAbility_".Length).Trim());
				}
				foreach (string text in list)
				{
					EmotionCardAbilityBase emotionCardAbilityBase = FindEmotionCardAbility(text.Trim());
					if (emotionCardAbilityBase != null)
					{
						emotionCardAbilityBase.SetEmotionCard(__instance);
						__instance._abilityList.Add(emotionCardAbilityBase);
					}
				}/
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/SetEmotionAbilityerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		*/

		[HarmonyPatch(typeof(BattleEmotionCardModel), MethodType.Constructor, new Type[] { typeof(EmotionCardXmlInfo), typeof(BattleUnitModel) })]
		[HarmonyPriority(Priority.Low)]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> BattleEmotionCardModel_ctor_In(IEnumerable<CodeInstruction> instructions, ILGenerator ilgen)
		{
			var codes = instructions.ToList();
			var trueJumpLabel = ilgen.DefineLabel();
			var falseJumpLabel = ilgen.DefineLabel();
			var moveNextMethod = Method(typeof(List<string>.Enumerator), nameof(List<string>.Enumerator.MoveNext));
			var createInstanceMethod = Method(typeof(Activator), nameof(Activator.CreateInstance), new Type[] { typeof(Type) });
			var getTypeMethod = Method(typeof(Type), nameof(Type.GetType), new Type[] { typeof(string) });
			for (int i = 0; i < codes.Count; i++)
			{
				if ((codes[i].opcode == Ldloca || codes[i].opcode == Ldloca_S) && TryGetIntValue(codes[i].operand, out int index) && index == 0)
				{
					if (i < codes.Count - 1 && codes[i + 1].Is(Call, moveNextMethod))
					{
						yield return new CodeInstruction(Nop).WithLabels(falseJumpLabel);
					}
				}
				else if (codes[i].Is(Call, createInstanceMethod))
				{
					yield return new CodeInstruction(Dup);
					yield return new CodeInstruction(Brtrue, trueJumpLabel);
					yield return new CodeInstruction(Pop);
					yield return new CodeInstruction(Br, falseJumpLabel);
					yield return new CodeInstruction(Nop).WithLabels(trueJumpLabel);
				}
				yield return codes[i];
				if (codes[i].Is(Call, getTypeMethod))
				{
					yield return new CodeInstruction(Ldloc_1);
					yield return new CodeInstruction(Call, Method(typeof(Harmony_Patch), nameof(BattleEmotionCardModel_ctor_CheckCustomAbility)));
				}
			}
		}
		static Type BattleEmotionCardModel_ctor_CheckCustomAbility(Type oldType, string name)
		{
			return FindEmotionCardAbilityType(name.Trim()) ?? oldType;
		}
		public static EmotionCardAbilityBase FindEmotionCardAbility(string name)
		{
			try
			{
				return Activator.CreateInstance(FindEmotionCardAbilityType(name)) as EmotionCardAbilityBase;
			}
			catch
			{
				return null;
			}
		}
		public static Type FindEmotionCardAbilityType(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				name = null;
			}
			else
			{
				name = name.Trim();
				if (CustomEmotionCardAbility.TryGetValue(name, out Type type))
				{
					return type;
				}
			}

			Type result = null;
			if (!CoreEmotionCardsLoaded)
			{
				var baseType = typeof(EmotionCardAbilityBase);
				foreach (Type type2 in Assembly.Load("Assembly-CSharp").GetTypes())
				{
					if (baseType.IsAssignableFrom(type2) && type2.Name.StartsWith("EmotionCardAbility_"))
					{
						var typeName = type2.Name.Substring("EmotionCardAbility_".Length);
						if (!CustomEmotionCardAbility.ContainsKey(typeName))
						{
							CustomEmotionCardAbility[typeName] = type2;
							if (typeName == name)
							{
								result = type2;
							}
						}
					}
				}
				CoreEmotionCardsLoaded = true;
			}
			return result;
		}
		//EmotionCard Preview Artwork
		[HarmonyPatch(typeof(UIAbnormalityCardPreviewSlot), nameof(UIAbnormalityCardPreviewSlot.Init))]
		[HarmonyPostfix]
		static void UIAbnormalityCardPreviewSlot_Init_Post(UIAbnormalityCardPreviewSlot __instance, EmotionCardXmlInfo card)
		{
			try
			{
				if (ArtWorks == null)
				{
					GetArtWorks();
				}
				if (ArtWorks.TryGetValue(card.Artwork, out var sprite))
				{
					__instance.artwork.sprite = sprite;
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/EmotionCardPreviewArtworkerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//EmotionCard DiceAction Artwork
		[HarmonyPatch(typeof(BattleUnitDiceActionUI_EmotionCard), nameof(BattleUnitDiceActionUI_EmotionCard.Init))]
		[HarmonyPostfix]
		static void BattleUnitDiceActionUI_EmotionCard_Init_Post(BattleUnitDiceActionUI_EmotionCard __instance, BattleEmotionCardModel card)
		{
			try
			{
				if (ArtWorks == null)
				{
					GetArtWorks();
				}
				string artwork = card.XmlInfo.Artwork;
				if (ArtWorks.TryGetValue(artwork, out var sprite))
				{
					__instance.artwork.sprite = sprite;
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/EmotionCardDiceActionArtworkerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//EmotionCardInven Passive Artwork
		[HarmonyPatch(typeof(UIEmotionPassiveCardInven), nameof(UIEmotionPassiveCardInven.SetSprites))]
		[HarmonyPostfix]
		static void UIEmotionPassiveCardInven_SetSprites_Post(UIEmotionPassiveCardInven __instance)
		{
			try
			{
				if (ArtWorks == null)
				{
					GetArtWorks();
				}
				string artwork = __instance._card.Artwork;
				if (ArtWorks.TryGetValue(artwork, out var sprite))
				{
					__instance._artwork.sprite = sprite;
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/EmotionCardInvenPassiveArtworkerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//EmotionCard Passive Artwork
		[HarmonyPatch(typeof(EmotionPassiveCardUI), nameof(EmotionPassiveCardUI.SetSprites))]
		[HarmonyPostfix]
		static void EmotionPassiveCardUI_SetSprites_Post(EmotionPassiveCardUI __instance)
		{
			try
			{
				if (ArtWorks == null)
				{
					GetArtWorks();
				}
				string artwork = __instance._card.Artwork;
				if (ArtWorks.TryGetValue(artwork, out var sprite))
				{
					__instance._artwork.sprite = sprite;
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/EmotionCardPassiveArtworkerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}

		//EGO
		//EGOCardName
		[HarmonyPatch(typeof(BattleDiceCardUI), nameof(BattleDiceCardUI.SetEgoCardForPopup))]
		[HarmonyPostfix]
		static void BattleDiceCardUI_SetEgoCardForPopup_Post(BattleDiceCardUI __instance, EmotionEgoXmlInfo egoxmlinfo)
		{
			try
			{
				DiceCardXmlInfo cardXmlInfo = ItemXmlDataList.instance.GetCardItem(egoxmlinfo.CardId);
				string cardName;
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
					__instance.txt_cardName.text = cardName;
				}
			}
			catch { }
		}
		//EGOId
		[HarmonyPatch(typeof(EmotionEgoXmlInfo), nameof(EmotionEgoXmlInfo.CardId), MethodType.Getter)]
		[HarmonyPrefix]
		static bool EmotionEgoXmlInfo_get_CardId_Pre(EmotionEgoXmlInfo __instance, ref LorId __result)
		{
			try
			{
				if (OrcTools.EgoDic.TryGetValue(__instance, out var id))
				{
					__result = id;
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
		[HarmonyPatch(typeof(EmotionEgoXmlList), nameof(EmotionEgoXmlList.GetData), new Type[] { typeof(LorId) })]
		[HarmonyPrefix]
		static bool EmotionEgoXmlList_GetData_Pre(LorId id, ref EmotionEgoXmlInfo __result)
		{
			try
			{
				OrcTools.CheckReverseEgoDic();
				if (OrcTools.ReverseEgoDic.TryGetValue(id, out var xml))
				{
					__result = xml;
					return false;
				}
			}
			catch (Exception ex)
			{
				ModContentManager.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
				File.WriteAllText(Application.dataPath + "/Mods/EgoCardIderror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return true;
		}
		//EGODataById
		[HarmonyPatch(typeof(EmotionEgoXmlList), nameof(EmotionEgoXmlList.GetData), new Type[] { typeof(LorId), typeof(SephirahType) })]
		[HarmonyPrefix]
		static bool EmotionEgoXmlList_GetData_2_Pre(LorId id, SephirahType sephirah, ref EmotionEgoXmlInfo __result)
		{
			try
			{
				if (OrcTools.CustomEmotionEgo.TryGetValue(sephirah, out var subdict) && subdict.TryGetValue(id, out var xml))
				{
					__result = xml;
					return false;
				}
			}
			catch (Exception ex)
			{
				ModContentManager.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
				File.WriteAllText(Application.dataPath + "/Mods/EgoCardIderror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return true;
		}
		//EGOName
		[HarmonyPatch(typeof(EmotionEgoCardUI), nameof(EmotionEgoCardUI.Init))]
		[HarmonyPrefix]
		static bool EmotionEgoCardUI_Init_Pre(EmotionEgoCardUI __instance, EmotionEgoXmlInfo card)
		{
			try
			{
				if (OrcTools.EgoDic.TryGetValue(card, out var id))
				{
					__instance._card = card;
					DiceCardXmlInfo cardItem = ItemXmlDataList.instance.GetCardItem(id, false);
					__instance._cardName.text = cardItem.Name;
					__instance.gameObject.SetActive(true);
					return false;
				}
			}
			catch (Exception ex)
			{
				ModContentManager.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
				File.WriteAllText(Application.dataPath + "/Mods/EgoCardUIIniterror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return true;
		}

		//custom quest script
		[HarmonyPatch(typeof(QuestMissionModel), MethodType.Constructor, new Type[] { typeof(QuestModel), typeof(QuestMissionXmlInfo) })]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> QuestMissionModel_ctor_In(IEnumerable<CodeInstruction> instructions, ILGenerator ilgen)
		{
			var codes = instructions.ToList();
			var concatMethod = Method(typeof(string), nameof(string.Concat), new Type[] { typeof(object[]) });
			var getTypeMethod = Method(typeof(Type), nameof(Type.GetType), new Type[] { typeof(string) });
			var customQuestTextMethod = Method(typeof(Harmony_Patch), nameof(QuestMissionModel_ctor_CheckCustomName));
			var customQuestTypeMethod = Method(typeof(Harmony_Patch), nameof(QuestMissionModel_ctor_CheckCustomType));
			foreach (var instruction in instructions)
			{
				yield return instruction;
				if (instruction.Calls(concatMethod))
				{
					yield return new CodeInstruction(Ldarg_2);
					yield return new CodeInstruction(Call, customQuestTextMethod);
				}
				else if (instruction.Calls(getTypeMethod))
				{
					yield return new CodeInstruction(Ldarg_2);
					yield return new CodeInstruction(Call, customQuestTypeMethod);
				}
			}
		}
		static string QuestMissionModel_ctor_CheckCustomName(string oldName, QuestMissionXmlInfo questInfo)
		{
			if (questInfo is QuestMissionXmlInfo_V2 questNew && !string.IsNullOrWhiteSpace(questNew.scriptName))
			{
				return "QuestMissionScript_" + questNew.scriptName.Trim();
			}
			return oldName;
		}
		static Type QuestMissionModel_ctor_CheckCustomType(Type oldType, QuestMissionXmlInfo questInfo)
		{
			if (questInfo is QuestMissionXmlInfo_V2 questNew && !string.IsNullOrWhiteSpace(questNew.scriptName))
			{
				var type = FindCustomQuestScriptType(questNew.scriptName);
				if (type != null)
				{
					return type;
				}
			}
			return oldType;
		}
		public static QuestMissionScriptBase FindCustomQuestScript(string name)
		{
			try
			{
				return Activator.CreateInstance(FindCustomQuestScriptType(name)) as QuestMissionScriptBase;
			}
			catch
			{
				return null;
			}
		}
		public static Type FindCustomQuestScriptType(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				name = null;
			}
			else
			{
				name = name.Trim();
				if (CustomQuest.TryGetValue(name, out Type type))
				{
					return type;
				}
			}

			Type result = null;
			if (!CoreQuestsLoaded)
			{
				var baseType = typeof(QuestMissionScriptBase);
				foreach (Type type2 in Assembly.Load("Assembly-CSharp").GetTypes())
				{
					if (baseType.IsAssignableFrom(type2) && type2.Name.StartsWith("QuestMissionScript_"))
					{
						var typeName = type2.Name.Substring("QuestMissionScript_".Length);
						if (!CustomQuest.ContainsKey(typeName))
						{
							CustomQuest[typeName] = type2;
							if (typeName == name)
							{
								result = type2;
							}
						}
					}
				}
				CoreQuestsLoaded = true;
			}
			return result;
		}

		//Story
		//StoryIcon
		[HarmonyPatch(typeof(UISpriteDataManager), nameof(UISpriteDataManager.GetStoryIcon))]
		[HarmonyPrefix]
		static bool UISpriteDataManager_GetStoryIcon_Pre(string story, ref UIIconManager.IconSet __result)
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
							icon = sprite,
							iconGlow = sprite
						};
						if (ArtWorks.TryGetValue(story + "_Glow", out var spriteGlow))
						{
							__result.iconGlow = spriteGlow;
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
		[HarmonyPatch(typeof(UIStoryArchivesPanel), nameof(UIStoryArchivesPanel.InitData))]
		[HarmonyPostfix]
		static void UIStoryArchivesPanel_InitData_Post(UIStoryArchivesPanel __instance)
		{
			try
			{
				if (__instance.episodeBooksData.Count == 0 && __instance.chapterBooksData.Count > 0)
				{
					__instance.bookStoryPanel.SetData();
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/StoryArchivesIniterror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}

		[HarmonyPatch(typeof(UIBookStoryPanel), nameof(UIBookStoryPanel.SetData))]
		[HarmonyPrefix]
		[HarmonyPriority(Priority.High)]
		static void UIBookStoryPanel_SetData_Pre(UIBookStoryPanel __instance)
		{
			try
			{
				var mainPanel = __instance.panel;
				int i;
				int j;
				BookXmlInfo bookXmlInfo;
				for (i = j = 0; i < mainPanel.chapterBooksData.Count; i++)
				{
					bookXmlInfo = mainPanel.chapterBooksData[i];
					if (bookXmlInfo.id.IsWorkshop() && OrcTools.EpisodeDic.ContainsKey(bookXmlInfo.id))
					{
						mainPanel.episodeBooksData.Add(bookXmlInfo);
					}
					else
					{
						mainPanel.chapterBooksData[j] = mainPanel.chapterBooksData[i];
						j++;
					}
				}
				mainPanel.chapterBooksData.RemoveRange(j, i - j);
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/BookStorySetDataerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}

		[HarmonyPatch(typeof(UIBookStoryPanel), nameof(UIBookStoryPanel.SetData))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> UIBookStoryPanel_SetData_In(IEnumerable<CodeInstruction> instructions)
		{
			var method1 = Method(typeof(Type), nameof(Type.GetTypeFromHandle));
			var method2 = Method(typeof(Enum), nameof(Enum.GetValues));
			var method3 = PropertyGetter(typeof(Array), nameof(Array.Length));
			var codes = instructions.ToList();
			for (int i = 0; i < codes.Count - 3; i++)
			{
				if (codes[i].Is(Ldtoken, typeof(UIStoryLine)) && codes[i + 1].Calls(method1) && codes[i + 2].Calls(method2)
					&& codes[i + 3].Calls(method3))
				{
					codes.RemoveRange(i + 1, 3);
					UIStoryLine[] originalValues = EnumExtender.GetOriginalValues<UIStoryLine>();
					int index = Array.BinarySearch(originalValues, UIStoryLine.Chapter1) - 1;
					codes[i] = new CodeInstruction(Ldc_I4, (int)originalValues[index]);
					break;
				}
			}
			return codes;
		}

		[HarmonyPatch(typeof(StoryTotal), nameof(StoryTotal.SetData))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> StoryTotal_SetData_In(IEnumerable<CodeInstruction> instructions)
		{
			var method1 = Method(typeof(Type), nameof(Type.GetTypeFromHandle));
			var method2 = Method(typeof(Enum), nameof(Enum.GetValues));
			var method3 = PropertyGetter(typeof(Array), nameof(Array.Length));
			var codes = instructions.ToList();
			for (int i = 0; i < codes.Count - 3; i++)
			{
				if (codes[i].Is(Ldtoken, typeof(UIStoryLine)) && codes[i + 1].Calls(method1) && codes[i + 2].Calls(method2)
					&& codes[i + 3].Calls(method3))
				{
					codes.RemoveRange(i + 1, 3);
					UIStoryLine[] originalValues = EnumExtender.GetOriginalValues<UIStoryLine>();
					int index = Array.BinarySearch(originalValues, UIStoryLine.Chapter1) - 1;
					codes[i] = new CodeInstruction(Ldc_I4, (int)originalValues[index] + 1);
					break;
				}
			}
			return codes;
		}

		//书库剧情槽位
		[HarmonyPatch(typeof(UIBookStoryChapterSlot), nameof(UIBookStoryChapterSlot.SetEpisodeSlots))]
		[HarmonyPostfix]
		static void UIBookStoryChapterSlot_SetEpisodeSlots_Post(UIBookStoryChapterSlot __instance)
		{
			try
			{
				bool hasEtc = false;
				List<BookXmlInfo> etcList = null;
				int j;
				for (j = __instance.EpisodeSlots.Count - 1; j >= 0; j--)
				{
					if (!__instance.EpisodeSlots[j].gameObject.activeSelf)
					{
						continue;
					}
					if (__instance.EpisodeSlots[j].isEtc)
					{
						hasEtc = true;
						etcList = __instance.EpisodeSlots[j].books;
					}
					break;
				}
				Dictionary<LorId, List<BookXmlInfo>> dictionary = new Dictionary<LorId, List<BookXmlInfo>>();
				StageClassInfoList stageClassInfoList = StageClassInfoList.Instance;
				foreach (BookXmlInfo bookXmlInfo in __instance.panel.panel.GetEpisodeBooksDataAll())
				{
					if (OrcTools.EpisodeDic.TryGetValue(bookXmlInfo.id, out var epId) && stageClassInfoList.GetData(epId).chapter == __instance.chapter && (!EnumExtender.IsValidEnumName(bookXmlInfo.BookIcon) || !EnumExtender.IsOriginalName<UIStoryLine>(bookXmlInfo.BookIcon)))
					{
						if (!dictionary.TryGetValue(epId, out var episodeBookList))
						{
							dictionary[epId] = episodeBookList = new List<BookXmlInfo>();
						}
						episodeBookList.Add(bookXmlInfo);
					}
				}
				foreach (KeyValuePair<LorId, List<BookXmlInfo>> keyValuePair in dictionary)
				{
					j++;
					if (__instance.EpisodeSlots.Count <= j)
					{
						__instance.InstatiateAdditionalSlot();
					}
					__instance.EpisodeSlots[j].Init(keyValuePair.Value, __instance);
					if (OrcTools.StageNameDic.TryGetValue(keyValuePair.Key, out string stagename))
					{
						__instance.EpisodeSlots[j].episodeText.text = stagename;
					}
				}
				if (hasEtc && etcList.Count > 0 && !__instance.EpisodeSlots[j].isEtc)
				{
					j++;
					if (__instance.EpisodeSlots.Count <= j)
					{
						__instance.InstatiateAdditionalSlot();
					}
					__instance.EpisodeSlots[j].Init(__instance.chapter, etcList, __instance);
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/UBSCSSESerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//EpisodeSlotName
		[HarmonyPatch(typeof(UIBookStoryPanel), nameof(UIBookStoryPanel.OnSelectEpisodeSlot))]
		[HarmonyPostfix]
		static void UIBookStoryPanel_OnSelectEpisodeSlot_Post(UIBookStoryPanel __instance, UIBookStoryEpisodeSlot slot)
		{
			try
			{
				if (slot?.books != null && slot.books.Count > 0 && OrcTools.EpisodeDic.TryGetValue(slot.books[0].id, out var epId) && OrcTools.StageNameDic.TryGetValue(epId, out string stagename))
				{
					__instance.selectedEpisodeText.text = stagename;
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/OnSelectEpisodeSloterror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//MoreLanguageStoryIdentify
		[HarmonyPatch(typeof(StorySerializer), nameof(StorySerializer.HasEffectFile))]
		[HarmonyPrefix]
		static bool StorySerializer_HasEffectFile_Pre(StageStoryInfo stageStoryInfo, ref bool __result)
		{
			try
			{
				if (stageStoryInfo == null)
				{
					return true;
				}
				if (stageStoryInfo.IsMod)
				{
					var config = BasemodConfig.FindBasemodConfig(stageStoryInfo.packageId);
					if (config.IgnoreStory)
					{
						return true;
					}
					string modPath = ModContentManager.Instance.GetModPath(stageStoryInfo.packageId);
					string storyFolderPath = Path.Combine(modPath, "Data", "StoryText");
					string[] array = stageStoryInfo.story.Split(new char[]
					{
						 '.'
					});
					if (Directory.Exists(storyFolderPath))
					{
						foreach (FileInfo fileInfo in new DirectoryInfo(storyFolderPath).GetFiles())
						{
							if (Path.GetFileNameWithoutExtension(fileInfo.FullName) == array[0])
							{
								__result = true;
								return false;
							}
						}
						var storyFolderPathLocalized = Path.Combine(storyFolderPath, TextDataModel.CurrentLanguage);
						if (!Directory.Exists(storyFolderPathLocalized))
						{
							storyFolderPathLocalized = Path.Combine(storyFolderPath, config.DefaultLocale);
						}
						if (Directory.Exists(storyFolderPathLocalized))
						{
							foreach (FileInfo fileInfo in new DirectoryInfo(storyFolderPathLocalized).GetFiles())
							{
								if (Path.GetFileNameWithoutExtension(fileInfo.FullName) == array[0])
								{
									__result = true;
									return false;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/HasEffectFileerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return true;
		}
		//GetModPath - prioritize activated mods
		[HarmonyPatch(typeof(ModContentManager), nameof(ModContentManager.GetModPath))]
		[HarmonyPrefix]
		static bool ModContentManager_GetModPath_Pre(ModContentManager __instance, string packageId, ref string __result)
		{
			try
			{
				if (__instance._loadedContents != null && __instance._loadedContents.Find(mod => mod._modInfo.invInfo.workshopInfo.uniqueId == packageId) is ModContent mod1 && mod1._dirInfo.FullName is string path && !string.IsNullOrWhiteSpace(path))
				{
					__result = path;
					return false;
				}
			}
			catch { }
			return true;
		}
		//LoadStageStory
		[HarmonyPatch(typeof(StorySerializer), nameof(StorySerializer.LoadStageStory))]
		[HarmonyPrefix]
		[HarmonyPriority(Priority.High)]
		static bool StorySerializer_LoadStageStory_Pre(StageStoryInfo stageStoryInfo, ref bool __result)
		{
			try
			{
				if (stageStoryInfo == null)
				{
					__result = false;
					return false;
				}
				if (stageStoryInfo.IsMod)
				{
					var config = BasemodConfig.FindBasemodConfig(stageStoryInfo.packageId);
					if (config.IgnoreStory)
					{
						return true;
					}
					string modPath = ModContentManager.Instance.GetModPath(stageStoryInfo.packageId);
					string storyFolderPath = Path.Combine(modPath, "Data", "StoryText");
					string effectFolderPath = Path.Combine(modPath, "Data", "StoryEffect");
					string storyPath = Path.Combine(storyFolderPath, stageStoryInfo.story);
					string effectPath = Path.Combine(effectFolderPath, stageStoryInfo.story);
					string[] array = stageStoryInfo.story.Split(new char[]
					{
						 '.'
					});
					if (Directory.Exists(effectFolderPath))
					{
						foreach (FileInfo fileInfo in new DirectoryInfo(effectFolderPath).GetFiles())
						{
							if (Path.GetFileNameWithoutExtension(fileInfo.FullName) == array[0])
							{
								effectPath = fileInfo.FullName;
							}
						}
					}
					if (Directory.Exists(storyFolderPath))
					{
						var storyFolderPathLocalized = Path.Combine(storyFolderPath, TextDataModel.CurrentLanguage);
						if (!Directory.Exists(storyFolderPathLocalized))
						{
							storyFolderPathLocalized = Path.Combine(storyFolderPath, config.DefaultLocale);
						}
						if (Directory.Exists(storyFolderPathLocalized))
						{
							foreach (FileInfo fileInfo in new DirectoryInfo(storyFolderPathLocalized).GetFiles())
							{
								if (Path.GetFileNameWithoutExtension(fileInfo.FullName) == array[0])
								{
									storyPath = fileInfo.FullName;
								}
							}
						}
						else
						{
							foreach (FileInfo fileInfo in new DirectoryInfo(storyFolderPath).GetFiles())
							{
								if (Path.GetFileNameWithoutExtension(fileInfo.FullName) == array[0])
								{
									storyPath = fileInfo.FullName;
								}
							}
						}
					}
					if (StorySerializer.LoadStoryFile(storyPath, effectPath, modPath))
					{
						__result = true;
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/LoadStageStoryerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return true;
		}
		//BattleStoryCG
		[HarmonyPatch(typeof(UIBattleStoryInfoPanel), nameof(UIBattleStoryInfoPanel.SetData))]
		[HarmonyPrefix]
		static void UIBattleStoryInfoPanel_SetData_Pre(UIBattleStoryInfoPanel __instance, StageClassInfo stage)
		{
			try
			{
				if (stage != null && stage.id.IsWorkshop())
				{
					if (BasemodConfig.FindBasemodConfig(stage.workshopID).IgnoreStory)
					{
						return;
					}
					string path = Path.Combine(ModContentManager.Instance.GetModPath(stage.workshopID), "Resource", "StoryBgSprite", StorySerializer.effectDefinition.cg.src);
					Sprite result = GetModStoryCG(stage.id, path);
					if (result != null)
					{
						__instance.CG.sprite = result;
					}
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/UIStoryInfo_SDerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//StorySceneCG
		[HarmonyPatch(typeof(StoryScene.StoryManager), nameof(StoryScene.StoryManager.LoadBackgroundSprite))]
		[HarmonyPrefix]
		static bool StoryManager_LoadBackgroundSprite_Pre(StoryScene.StoryManager __instance, string src, ref Sprite __result)
		{
			if (string.IsNullOrWhiteSpace(src))
			{
				return true;
			}
			try
			{
				var src1 = src;
				if (!__instance._loadedCustomSprites.TryGetValue("bgsprite:" + src, out var sprite) && !CheckedCustomSprites.Contains(src))
				{
					if (!File.Exists(Path.Combine(ModUtil.GetModBgSpritePath(StorySerializer.curModPath), src)))
					{
						src1 += ".png";
					}
					sprite = SpriteUtil.LoadSprite(Path.Combine(ModUtil.GetModBgSpritePath(StorySerializer.curModPath), src1), new Vector2(0.5f, 0.5f));
					if (sprite != null)
					{
						__instance._loadedCustomSprites.Add("bgsprite:" + src, sprite);
					}
					else
					{
						CheckedCustomSprites.Add(src);
					}
				}
				if (sprite != null)
				{
					__result = sprite;
					return false;
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/LoadBackgroundSpriteerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return true;
		}
		public static Sprite GetModStoryCG(LorId StageId, string Path)
		{
			if (StageId is null)
			{
				return null;
			}
			if (ModStoryCG.TryGetValue(StageId, out ModStroyCG result))
			{
				return result.sprite;
			}
			else if (CheckedModStoryCG.Contains(StageId))
			{
				return null;
			}
			try
			{
				if (File.Exists(Path))
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
			}
			catch { }
			CheckedModStoryCG.Add(StageId);
			return null;
		}
		//EntryCG
		[HarmonyPatch(typeof(StageController), nameof(StageController.GameOver))]
		[HarmonyPostfix]
		static void StageController_GameOver_Post(StageController __instance)
		{
			try
			{
				if (ModStoryCG.TryGetValue(__instance._stageModel.ClassInfo.id, out var modStory))
				{
					ModSaveTool.SaveString("ModLastStroyCG", modStory.path, "BaseMod");
				}
				else if (TryAddModStoryCG(__instance._stageModel.ClassInfo))
				{
					ModSaveTool.SaveString("ModLastStroyCG", ModStoryCG[__instance._stageModel.ClassInfo.id].path, "BaseMod");
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
		static bool TryAddModStoryCG(StageClassInfo stageClassInfo)
		{
			var startStory = stageClassInfo?.GetStartStory();
			if (startStory == null || string.IsNullOrWhiteSpace(startStory.story))
			{
				return false;
			}
			if (!stageClassInfo.id.IsWorkshop())
			{
				return false;
			}
			if (BasemodConfig.FindBasemodConfig(stageClassInfo.id.packageId).IgnoreStory)
			{
				return false;
			}
			string modPath = ModContentManager.Instance.GetModPath(startStory.packageId);
			if (string.IsNullOrWhiteSpace(modPath))
			{
				return false;
			}
			string effectPath = Path.Combine(modPath, "Data", "StoryEffect", startStory.story);
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
			if (string.IsNullOrWhiteSpace(cg))
			{
				return false;
			}
			string path = Path.Combine(modPath, "Resource", "StoryBgSprite", cg);
			return GetModStoryCG(stageClassInfo.id, path) != null;
		}
		//EntryCG
		[HarmonyPatch(typeof(EntryScene), nameof(EntryScene.SetCG))]
		[HarmonyPostfix]
		[HarmonyPriority(Priority.High)]
		static void EntryScene_SetCG_Post(EntryScene __instance)
		{
			try
			{
				string stroycg = ModSaveTool.GetModSaveData("BaseMod").GetString("ModLastStroyCG");
				if (!string.IsNullOrWhiteSpace(stroycg) && File.Exists(stroycg))
				{
					Texture2D texture2D = new Texture2D(1, 1);
					texture2D.LoadImage(File.ReadAllBytes(stroycg));
					Sprite value = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
					__instance.CGImage.sprite = value;
				}
			}
			catch (Exception ex)
			{
				ModContentManager.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
				File.WriteAllText(Application.dataPath + "/Mods/SetEntrySceneCGerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//Condition
		[HarmonyPatch(typeof(StageExtraCondition), nameof(StageExtraCondition.IsUnlocked))]
		[HarmonyPrefix]
		static bool StageExtraCondition_IsUnlocked_Pre(StageExtraCondition __instance, ref bool __result)
		{
			try
			{
				if (OrcTools.StageConditionDic.TryGetValue(__instance, out var conditions))
				{
					__result = true;
					foreach (LorId stageId in conditions)
					{
						if (LibraryModel.Instance.ClearInfo.GetClearCount(stageId) <= 0)
						{
							__result = false;
							break;
						}
					}
					return false;
				}
			}
			catch (Exception ex)
			{
				ModContentManager.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
				File.WriteAllText(Application.dataPath + "/Mods/StageConditionerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return true;
		}
		//Condition
		[HarmonyPatch(typeof(UIInvitationRightMainPanel), nameof(UIInvitationRightMainPanel.SendInvitation))]
		[HarmonyTranspiler]
		[HarmonyPriority(Priority.Low)]
		static IEnumerable<CodeInstruction> UIInvitationRightMainPanel_SendInvitation_In(IEnumerable<CodeInstruction> instructions)
		{
			bool waiting = true;
			foreach (var instruction in instructions)
			{
				if (instruction.opcode == Stloc_1 && waiting)
				{
					waiting = false;
					yield return new CodeInstruction(Ldloc_0);
					yield return new CodeInstruction(Call, Method(typeof(Harmony_Patch), nameof(UIInvitationRightMainPanel_SendInvitation_CheckCustomCondition)));
					yield return new CodeInstruction(And);
				}
				yield return instruction;
			}
		}
		static bool UIInvitationRightMainPanel_SendInvitation_CheckCustomCondition(StageClassInfo bookRecipe)
		{
			return bookRecipe.currentState != StoryState.Close;
		}
		//custom floor level stage
		[HarmonyPatch(typeof(DebugConsoleScript), nameof(DebugConsoleScript.EnterCreatureBattle))]
		[HarmonyPatch(typeof(UI.UIController), nameof(UI.UIController.OnClickStartCreatureStage))]
		[HarmonyPatch(typeof(UICreatureRebattleNumberSlot), nameof(UICreatureRebattleNumberSlot.OnPointerClick))]
		[HarmonyPatch(typeof(UICreatureRebattleNumberSlot), nameof(UICreatureRebattleNumberSlot.SetData))]
		[HarmonyPatch(typeof(UIMainPanel), nameof(UIMainPanel.OnClickLevelUp))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> FloorLevelXmlInfo_CustomStageId_In(IEnumerable<CodeInstruction> instructions, ILGenerator ilgen)
		{
			var stageField = Field(typeof(FloorLevelXmlInfo), nameof(FloorLevelXmlInfo.stageId));
			var getDataInt = Method(typeof(StageClassInfoList), nameof(StageClassInfoList.GetData), new Type[] { typeof(int) });
			var stageFixer = Method(typeof(Harmony_Patch), nameof(FloorLevelXmlInfo_TryFixStageInfo));
			var getNameInt = Method(typeof(StageNameXmlList), nameof(StageNameXmlList.GetName), new Type[] { typeof(int) });
			var nameFixer = Method(typeof(Harmony_Patch), nameof(FloorLevelXmlInfo_TryFixStageName));
			LocalBuilder local = null;
			var codes = instructions.ToList();
			for (int i = 0; i < codes.Count - 1; i++)
			{
				if (codes[i].LoadsField(stageField))
				{
					if (codes[i + 1].Calls(getDataInt))
					{
						if (local == null)
						{
							local = ilgen.DeclareLocal(typeof(FloorLevelXmlInfo));
						}
						codes.InsertRange(i, new CodeInstruction[]
						{
							new CodeInstruction(Dup),
							new CodeInstruction(Stloc, local)
						});
						codes.InsertRange(i + 4, new CodeInstruction[]
						{
							new CodeInstruction(Ldloc, local),
							new CodeInstruction(Call, stageFixer)
						});
						i += 4;
					}
					else if (codes[i + 1].Calls(getNameInt))
					{
						if (local == null)
						{
							local = ilgen.DeclareLocal(typeof(FloorLevelXmlInfo));
						}
						codes.InsertRange(i, new CodeInstruction[]
						{
							new CodeInstruction(Dup),
							new CodeInstruction(Stloc, local)
						});
						codes.InsertRange(i + 4, new CodeInstruction[]
						{
							new CodeInstruction(Ldloc, local),
							new CodeInstruction(Call, nameFixer)
						});
						i += 4;
					}
				}
			}
			return codes;
		}
		static string FloorLevelXmlInfo_TryFixStageName(string name, FloorLevelXmlInfo info)
		{
			if (OrcTools.FloorLevelStageDic.TryGetValue(info, out var stageId) && StageClassInfoList.Instance.GetData(stageId) is StageClassInfo stage && StageNameXmlList.Instance.GetName(stage) is string fixedName && !string.IsNullOrWhiteSpace(fixedName) && fixedName != "Not Found")
			{
				return fixedName;
			}
			return name;
		}
		static StageClassInfo FloorLevelXmlInfo_TryFixStageInfo(StageClassInfo stage, FloorLevelXmlInfo info)
		{
			if (OrcTools.FloorLevelStageDic.TryGetValue(info, out var stageId) && StageClassInfoList.Instance.GetData(stageId) is StageClassInfo fixedStage)
			{
				return fixedStage;
			}
			return stage;
		}

		//EquipPageInvenPanel
		//MoreEuipPageUI
		[HarmonyPatch(typeof(UIOriginEquipPageList), nameof(UIOriginEquipPageList.UpdateEquipPageList))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> UIOriginEquipPageList_UpdateEquipPageList_In(IEnumerable<CodeInstruction> instructions)
		{
			foreach (var instruction in instructions)
			{
				if (instruction.opcode == Ldc_I4_5)
				{
					yield return new CodeInstruction(Ldarg_0);
					yield return new CodeInstruction(Ldfld, Field(typeof(UIOriginEquipPageList), nameof(UIOriginEquipPageList.currentScreenBookModelList)));
					yield return new CodeInstruction(Callvirt, PropertyGetter(typeof(List<BookModel>), nameof(List<BookModel>.Count)));
					yield return new CodeInstruction(Ldarg_0);
					yield return new CodeInstruction(Ldfld, Field(typeof(UIOriginEquipPageList), nameof(UIOriginEquipPageList.equipPageSlotList)));
					yield return new CodeInstruction(Callvirt, PropertyGetter(typeof(List<UIOriginEquipPageSlot>), nameof(List<UIOriginEquipPageSlot>.Count)));
					yield return new CodeInstruction(Sub);
				}
				else
				{
					yield return instruction;
				}
			}
		}
		//UIEquipPageScrollList
		/*
		[HarmonyPatch(typeof(UIEquipPageScrollList), nameof(UIEquipPageScrollList.SetData))]
		[HarmonyPrefix]
		static bool UIEquipPageScrollList_SetData_Pre(UIEquipPageScrollList __instance, List<BookModel> books, UnitDataModel unit, bool init = false)
		{
			try
			{
				if (init)
				{
					__instance.CurrentSelectedBook = null;
					__instance.currentslotcount = 0;
				}
				__instance._selectedUnit = unit;
				__instance._originBookModelList.Clear();
				__instance.currentBookModelList.Clear();
				__instance.isActiveScrollBar = false;
				__instance._originBookModelList.AddRange(books);
				__instance.currentBookModelList.AddRange(books);
				__instance.currentBookModelList = __instance.FilterBookModels(__instance.currentBookModelList);
				__instance.totalkeysdata.Clear();
				__instance.CurrentSelectedBook = null;
				__instance.heightdatalist.Clear();
				__instance.currentStoryBooksDic.Clear();
				foreach (BookModel bookModel in __instance.currentBookModelList)
				{
					string bookIcon = bookModel.ClassInfo.BookIcon;
					UIStoryKeyData uistoryKeyData;
					if (bookModel.IsWorkshop || !Enum.IsDefined(typeof(UIStoryLine), bookIcon))
					{
						if (bookModel.ClassInfo is BookXmlInfo_New bookNew)
						{
							var storyline = GetModEpMatch(bookNew.LorEpisode);
							uistoryKeyData = __instance.totalkeysdata.Find((UIStoryKeyData x) => x.workshopId == bookModel.ClassInfo.id.packageId && x.chapter == bookModel.ClassInfo.Chapter && x.StoryLine == ModEpMatch[bookNew.LorEpisode]);
							if (uistoryKeyData == null)
							{
								uistoryKeyData = new UIStoryKeyData(bookModel.ClassInfo.Chapter, bookModel.ClassInfo.id.packageId)
								{
									StoryLine = storyline
								};
								__instance.totalkeysdata.Add(uistoryKeyData);
							}
						}
						else
						{
							uistoryKeyData = __instance.totalkeysdata.Find((UIStoryKeyData x) => x.chapter == bookModel.ClassInfo.Chapter && x.workshopId == bookModel.ClassInfo.workshopID);
							if (uistoryKeyData == null)
							{
								uistoryKeyData = new UIStoryKeyData(bookModel.ClassInfo.Chapter, bookModel.ClassInfo.id.packageId);
								__instance.totalkeysdata.Add(uistoryKeyData);
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
						uistoryKeyData = __instance.totalkeysdata.Find((UIStoryKeyData x) => x.chapter == bookModel.ClassInfo.Chapter && x.StoryLine == storyLine);
						if (uistoryKeyData == null)
						{
							uistoryKeyData = new UIStoryKeyData(bookModel.ClassInfo.Chapter, storyLine);
							__instance.totalkeysdata.Add(uistoryKeyData);
						}
					}
					if (!__instance.currentStoryBooksDic.ContainsKey(uistoryKeyData))
					{
						List<BookModel> list = new List<BookModel>
							{
								bookModel
							};
						__instance.currentStoryBooksDic.Add(uistoryKeyData, list);
					}
					else
					{
						__instance.currentStoryBooksDic[uistoryKeyData].Add(bookModel);
					}
				}
				__instance.totalkeysdata.Sort(delegate (UIStoryKeyData x, UIStoryKeyData y)
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
				__instance.totalkeysdata.Reverse();
				__instance.CalculateSlotsHeight();
				__instance.UpdateSlotList();
				__instance.SetScrollBar();
				__instance.isClickedUpArrow = false;
				__instance.isClickedDownArrow = false;
				LayoutRebuilder.ForceRebuildLayoutImmediate(__instance.rect_slotListRoot);
				UIOriginEquipPageSlot saveFirstChild = __instance._equipPagesPanelSlotList[0].EquipPageSlotList[0];
				__instance.SetSaveFirstChild(saveFirstChild);
				return false;
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/EPSLSDerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return true;
		}
		*/
		[HarmonyPatch(typeof(UISettingEquipPageScrollList), nameof(UISettingEquipPageScrollList.SetData))]
		[HarmonyPatch(typeof(UIEquipPageScrollList), nameof(UIEquipPageScrollList.SetData))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> UIEquipPageScrollList_SetData_In(IEnumerable<CodeInstruction> instructions, ILGenerator ilgen, MethodBase original)
		{
			bool firstFind = true;
			var findingMethod = Method(typeof(List<UIStoryKeyData>), nameof(List<UIStoryKeyData>.Find));
			var isWorkshopProperty = PropertyGetter(typeof(BookModel), nameof(BookModel.IsWorkshop));
			var currentBookProperty = PropertyGetter(typeof(List<BookModel>.Enumerator), nameof(List<BookModel>.Enumerator.Current));
			var currentBookLocal = ilgen.DeclareLocal(typeof(BookModel));
			foreach (var instruction in instructions)
			{
				yield return instruction;
				if (firstFind && instruction.Is(Callvirt, findingMethod))
				{
					firstFind = false;
					yield return new CodeInstruction(Ldloc, currentBookLocal);
					yield return new CodeInstruction(Ldarg_0);
					yield return new CodeInstruction(Ldfld, Field(original.DeclaringType, "totalkeysdata"));
					yield return new CodeInstruction(Call, Method(typeof(Harmony_Patch), nameof(UIEquipPageScrollList_SetData_FixCustomStory)));
				}
				else if (instruction.Is(Call, currentBookProperty))
				{
					yield return new CodeInstruction(Dup);
					yield return new CodeInstruction(Stloc, currentBookLocal);
				}
				else if (instruction.Is(Callvirt, isWorkshopProperty))
				{
					yield return new CodeInstruction(Ldloc, currentBookLocal);
					yield return new CodeInstruction(Call, Method(typeof(Harmony_Patch), nameof(UIEquipPageScrollList_SetData_IsCustomStory)));
				}
			}
		}
		static UIStoryKeyData UIEquipPageScrollList_SetData_FixCustomStory(UIStoryKeyData oldKey, BookModel bookModel, List<UIStoryKeyData> allKeys)
		{
			if (!OrcTools.EpisodeDic.TryGetValue(bookModel.BookId, out var epId))
			{
				return oldKey;
			}

			var storyline = GetModEpMatch(epId);
			var newKey = allKeys.Find((UIStoryKeyData x) => x.workshopId == bookModel.ClassInfo.id.packageId && x.chapter == bookModel.ClassInfo.Chapter && x.StoryLine == storyline);
			if (newKey == null)
			{
				newKey = new UIStoryKeyData(bookModel.ClassInfo.Chapter, bookModel.ClassInfo.id.packageId)
				{
					StoryLine = storyline
				};
				allKeys.Add(newKey);
			}
			return newKey;
		}
		static bool UIEquipPageScrollList_SetData_IsCustomStory(bool isWorkshop, BookModel book)
		{
			return isWorkshop || !EnumExtender.IsValidEnumName(book.ClassInfo.BookIcon) || !EnumExtender.IsOriginalName<UIStoryLine>(book.ClassInfo.BookIcon);
		}
		public static UIStoryLine GetModEpMatch(LorId episodeId)
		{
			if (!ModEpMatch.ContainsKey(episodeId))
			{
				EnumExtender.TryFindUnnamedValue<UIStoryLine>((UIStoryLine)ModEpMin, null, false, out var newStoryLine);
				EnumExtender.TryAddName($"BaseMod{(int)newStoryLine}", newStoryLine);
				ModEpMatch.Add(episodeId, newStoryLine);
			}
			return ModEpMatch[episodeId];
		}
		//UISettingEquipPageScrollList
		/*
		[HarmonyPatch(typeof(UISettingEquipPageScrollList), nameof(UISettingEquipPageScrollList.SetData))]
		[HarmonyPrefix]
		static bool UISettingEquipPageScrollList_SetData_Pre(UISettingEquipPageScrollList __instance, List<BookModel> books, UnitDataModel unit, bool init = false)
		{
			try
			{
				ModEpMatch = new Dictionary<LorId, UIStoryLine>();
				if (init)
				{
					__instance.CurrentSelectedBook = null;
					__instance.currentslotcount = 0;
				}
				__instance._selectedUnit = unit;
				__instance._originBookModelList.Clear();
				__instance.currentBookModelList.Clear();
				__instance.isActiveScrollBar = false;
				__instance._originBookModelList.AddRange(books);
				__instance.currentBookModelList.AddRange(books);
				__instance.currentBookModelList = __instance.FilterBookModels(__instance.currentBookModelList);
				__instance.totalkeysdata.Clear();
				__instance.CurrentSelectedBook = null;
				__instance.heightdatalist.Clear();
				__instance.currentStoryBooksDic.Clear();
				int num = 200;
				foreach (BookModel bookModel in __instance.currentBookModelList)
				{
					string bookIcon = bookModel.ClassInfo.BookIcon;
					UIStoryKeyData uistoryKeyData;
					if (bookModel.IsWorkshop || !Enum.IsDefined(typeof(UIStoryLine), bookIcon))
					{
						if (bookModel.ClassInfo is BookXmlInfo_New)
						{
							if (!ModEpMatch.ContainsKey((bookModel.ClassInfo as BookXmlInfo_New).LorEpisode))
							{
								num++;
								ModEpMatch.Add((bookModel.ClassInfo as BookXmlInfo_New).LorEpisode, (UIStoryLine)num);
								uistoryKeyData = new UIStoryKeyData(bookModel.ClassInfo.Chapter, bookModel.ClassInfo.id.packageId)
								{
									StoryLine = (UIStoryLine)num
								};
								__instance.totalkeysdata.Add(uistoryKeyData);
							}
							else
							{
								uistoryKeyData = __instance.totalkeysdata.Find((UIStoryKeyData x) => x.workshopId == bookModel.ClassInfo.id.packageId && x.chapter == bookModel.ClassInfo.Chapter && x.StoryLine == ModEpMatch[(bookModel.ClassInfo as BookXmlInfo_New).LorEpisode]);
							}
						}
						else
						{
							uistoryKeyData = __instance.totalkeysdata.Find((UIStoryKeyData x) => x.chapter == bookModel.ClassInfo.Chapter && x.workshopId == bookModel.ClassInfo.workshopID);
							if (uistoryKeyData == null)
							{
								uistoryKeyData = new UIStoryKeyData(bookModel.ClassInfo.Chapter, bookModel.ClassInfo.id.packageId);
								__instance.totalkeysdata.Add(uistoryKeyData);
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
						uistoryKeyData = __instance.totalkeysdata.Find((UIStoryKeyData x) => x.chapter == bookModel.ClassInfo.Chapter && x.StoryLine == storyLine);
						if (uistoryKeyData == null)
						{
							uistoryKeyData = new UIStoryKeyData(bookModel.ClassInfo.Chapter, storyLine);
							__instance.totalkeysdata.Add(uistoryKeyData);
						}
					}
					if (!__instance.currentStoryBooksDic.ContainsKey(uistoryKeyData))
					{
						List<BookModel> list = new List<BookModel>
							{
								bookModel
							};
						__instance.currentStoryBooksDic.Add(uistoryKeyData, list);
					}
					else
					{
						__instance.currentStoryBooksDic[uistoryKeyData].Add(bookModel);
					}
				}
				__instance.totalkeysdata.Sort(delegate (UIStoryKeyData x, UIStoryKeyData y)
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
				__instance.totalkeysdata.Reverse();
				__instance.CalculateSlotsHeight();
				__instance.UpdateSlotList();
				__instance.SetScrollBar();
				__instance.isClickedUpArrow = false;
				__instance.isClickedDownArrow = false;
				LayoutRebuilder.ForceRebuildLayoutImmediate(__instance.rect_slotListRoot);
				UIOriginEquipPageSlot saveFirstChild = __instance._equipPagesPanelSlotList[0].EquipPageSlotList[0];
				__instance.SetSaveFirstChild(saveFirstChild);
				return false;
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/SEPSLSDerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return true;
		}
		*/
		//UISettingInvenEquipPageListSlot
		[HarmonyPatch(typeof(UISettingInvenEquipPageListSlot), nameof(UISettingInvenEquipPageListSlot.SetBooksData))]
		[HarmonyPostfix]
		[HarmonyPriority(Priority.High)]
		static void UISettingInvenEquipPageListSlot_SetBooksData_Pre(UISettingInvenEquipPageListSlot __instance, List<BookModel> books, UIStoryKeyData storyKey)
		{
			try
			{
				if (books.Count <= 0)
				{
					return;
				}
				if (EnumExtender.IsOriginalValue(storyKey.StoryLine))
				{
					return;
				}
				if (!OrcTools.EpisodeDic.TryGetValue(books[0].BookId, out var epId))
				{
					return;
				}
				StageClassInfo data = StageClassInfoList.Instance.GetData(epId);
				if (data == null)
				{
					return;
				}
				UIIconManager.IconSet storyIcon = UISpriteDataManager.instance.GetStoryIcon(data.storyType);
				if (storyIcon != null)
				{
					__instance.img_IconGlow.enabled = true;
					__instance.img_Icon.enabled = true;
					__instance.img_Icon.sprite = storyIcon.icon;
					__instance.img_IconGlow.sprite = storyIcon.iconGlow;
				}
				if (OrcTools.StageNameDic.TryGetValue(data.id, out string stageName))
				{
					__instance.txt_StoryName.text = "workshop " + stageName;
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/SIEPLSSBDerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//UIInvenEquipPageListSlot
		[HarmonyPatch(typeof(UIInvenEquipPageListSlot), nameof(UIInvenEquipPageListSlot.SetBooksData))]
		[HarmonyPostfix]
		[HarmonyPriority(Priority.High)]
		static void UIInvenEquipPageListSlot_SetBooksData_Post(UIInvenEquipPageListSlot __instance, List<BookModel> books, UIStoryKeyData storyKey)
		{
			try
			{
				if (books.Count < 0)
				{
					return;
				}
				if (EnumExtender.IsOriginalValue(storyKey.StoryLine))
				{
					return;
				}
				if (!OrcTools.EpisodeDic.TryGetValue(books[0].BookId, out var epId))
				{
					return;
				}
				StageClassInfo data = StageClassInfoList.Instance.GetData(epId);
				if (data == null)
				{
					return;
				}
				UIIconManager.IconSet storyIcon = UISpriteDataManager.instance.GetStoryIcon(data.storyType);
				if (storyIcon != null)
				{
					__instance.img_IconGlow.enabled = true;
					__instance.img_Icon.enabled = true;
					__instance.img_Icon.sprite = storyIcon.icon;
					__instance.img_IconGlow.sprite = storyIcon.iconGlow;
				}
				if (OrcTools.StageNameDic.TryGetValue(data.id, out string stageName))
				{
					__instance.txt_StoryName.text = "workshop " + stageName;
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/IEPLSSBDerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}

		//CustomMapManager
		//InitCustomMap
		/*
		[HarmonyPatch(typeof(StageController), nameof(StageController.InitializeMap))]
		[HarmonyPrefix]
		static bool StageController_InitializeMap_Pre(StageController __instance)
		{
			try
			{
				if (__instance.stageType != StageType.Invitation)
				{
					return true;
				}
				else
				{
					BattleSceneRoot.Instance.HideAllFloorMap();
					List<string> mapInfo = __instance.GetStageModel().ClassInfo.mapInfo;
					if (mapInfo != null && mapInfo.Count > 0)
					{
						try
						{
							foreach (string text in mapInfo)
							{
								if (string.IsNullOrWhiteSpace(text))
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
									string resourcePath = ModContentManager.Instance.GetModPath(__instance.GetStageModel().ClassInfo.workshopID) + "/CustomMap_" + mapName;
									if (CustomMapManager.TryGetValue(mapName + "MapManager", out Type mapmanager))
									{
										Debug.Log("Find MapManager:" + mapName);
										if (mapmanager == null)
										{
											return true;
										}
										GameObject gameObject = Util.LoadPrefab("LibraryMaps/KETHER_Map", BattleSceneRoot.Instance.transform);
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
										BattleSceneRoot.Instance.InitInvitationMap(mapManager);
									}
									else if (Directory.Exists(resourcePath))
									{
										Debug.Log("Find SimpleMap:" + resourcePath);
										GameObject gameObject = Util.LoadPrefab("LibraryMaps/KETHER_Map", BattleSceneRoot.Instance.transform);
										GameObject borderFrame = gameObject.GetComponent<MapManager>().borderFrame;
										GameObject backgroundRoot = gameObject.GetComponent<MapManager>().backgroundRoot;
										UnityEngine.Object.Destroy(gameObject.GetComponent<MapManager>());
										gameObject.name = "InvitationMap_" + text;
										SimpleMapManager simpleMapManager = (SimpleMapManager)gameObject.AddComponent(typeof(SimpleMapManager));
										simpleMapManager.borderFrame = borderFrame;
										simpleMapManager.backgroundRoot = backgroundRoot;
										simpleMapManager.SimpleInit(resourcePath, mapName);
										simpleMapManager.CustomInit();
										BattleSceneRoot.Instance.InitInvitationMap(simpleMapManager);
									}
								}
								else
								{
									GameObject gameObject2 = Util.LoadPrefab("InvitationMaps/InvitationMap_" + text, BattleSceneRoot.Instance.transform);
									gameObject2.name = "InvitationMap_" + text;
									BattleSceneRoot.Instance.InitInvitationMap(gameObject2.GetComponent<MapManager>());
								}
							}
						}
						catch (Exception ex)
						{
							File.WriteAllText(Application.dataPath + "/Mods/stageerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
							BattleSceneRoot.Instance.InitFloorMap(__instance.CurrentFloor);
						}
					}
				}
				BattleSceneRoot.Instance.InitFloorMap(__instance.CurrentFloor);
				return false;
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/InitializeMaperror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return true;
		}
		*/
		[HarmonyPatch(typeof(StageController), nameof(StageController.InitializeMap))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> StageController_InitializeMap_In(IEnumerable<CodeInstruction> instructions)
		{
			var loadPrefabMethod = Method(typeof(Util), nameof(Util.LoadPrefab), new Type[] { typeof(string), typeof(Transform) });
			var getComponentMethod = Method(typeof(GameObject), nameof(GameObject.GetComponent), Array.Empty<Type>(), new Type[] { typeof(MapManager) });
			var getHelperMethod = Method(typeof(Harmony_Patch), nameof(GetMapComponentFixed));
			var currentMapNameProperty = PropertyGetter(typeof(List<string>.Enumerator), nameof(List<string>.Enumerator.Current));
			var helperMethod = Method(typeof(Harmony_Patch), nameof(StageController_InitializeMap_CheckCustomMap));
			foreach (var instruction in instructions)
			{
				if (instruction.Is(Callvirt, getComponentMethod))
				{
					yield return new CodeInstruction(Callvirt, getHelperMethod);
				}
				else
				{
					yield return instruction;
				}
				if (instruction.Is(Call, loadPrefabMethod))
				{
					yield return new CodeInstruction(Ldloca, 1);
					yield return new CodeInstruction(Call, currentMapNameProperty);
					yield return new CodeInstruction(Call, helperMethod);
				}
			}
		}
		static GameObject StageController_InitializeMap_CheckCustomMap(GameObject originalMapObject, string mapName)
		{
			try
			{
				if (!mapName.ToLower().StartsWith("custom_"))
				{
					return originalMapObject;
				}

				var customMapName = mapName.Substring("custom_".Length).Trim();
				if (CustomMapManager.TryGetValue(customMapName + "MapManager", out Type mapmanager))
				{
					Debug.Log("Find MapManager:" + customMapName);
					if (mapmanager == null)
					{
						return originalMapObject;
					}
					GameObject customMapObject = Util.LoadPrefab("LibraryMaps/KETHER_Map", BattleSceneRoot.Instance.transform);
					MapManager oldManager = customMapObject.GetComponent<MapManager>();
					MapManager newManager = (MapManager)customMapObject.AddComponent(mapmanager);
					newManager.borderFrame = oldManager.borderFrame;
					newManager.backgroundRoot = oldManager.backgroundRoot;
					newManager._obstacleRoot = oldManager._obstacleRoot;
					newManager._obstacles = oldManager._obstacles;
					newManager._roots = oldManager._roots;
					oldManager.enabled = false;
					UnityEngine.Object.Destroy(oldManager);
					customMapObject.name = "InvitationMap_" + mapName;
					if (newManager is CustomMapManager)
					{
						(newManager as CustomMapManager).CustomInit();
					}
					return customMapObject;
				}
				else
				{
					var workshopId = StageController.Instance.GetStageModel()?.ClassInfo.workshopID;
					if (!string.IsNullOrEmpty(workshopId))
					{
						string resourcePath = Path.Combine(ModContentManager.Instance.GetModPath(workshopId), "CustomMap_" + customMapName);
						if (Directory.Exists(resourcePath))
						{
							Debug.Log("Find SimpleMap:" + resourcePath);
							GameObject customMapObject = Util.LoadPrefab("LibraryMaps/KETHER_Map", BattleSceneRoot.Instance.transform);
							MapManager oldManager = customMapObject.GetComponent<MapManager>();
							SimpleMapManager simpleMapManager = customMapObject.AddComponent<SimpleMapManager>();
							simpleMapManager.borderFrame = oldManager.borderFrame;
							simpleMapManager.backgroundRoot = oldManager.backgroundRoot;
							simpleMapManager._obstacleRoot = oldManager._obstacleRoot;
							simpleMapManager._obstacles = oldManager._obstacles;
							simpleMapManager._roots = oldManager._roots;
							oldManager.enabled = false;
							UnityEngine.Object.Destroy(oldManager);
							customMapObject.name = "InvitationMap_" + mapName;
							simpleMapManager.SimpleInit(resourcePath, customMapName);
							simpleMapManager.CustomInit();
							return customMapObject;
						}
					}
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/InitializeMaperror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return originalMapObject;
		}
		static MapManager GetMapComponentFixed(GameObject gameObject)
		{
			return gameObject.GetComponents<MapManager>().LastOrDefault();
		}
		//EgoMap
		/*
		[HarmonyPatch(typeof(BattleSceneRoot), nameof(BattleSceneRoot.ChangeToEgoMap))]
		[HarmonyPrefix]
		static bool BattleSceneRoot_ChangeToEgoMap_Pre(string mapName)
		{
			try
			{
				MapChangeFilter mapChangeFilter = BattleSceneRoot.Instance._mapChangeFilter;
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
					gameObject = Util.LoadPrefab("LibraryMaps/KETHER_Map", BattleSceneRoot.Instance.transform);
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
						if (BattleSceneRoot.Instance.currentMapObject.isCreature)
						{
							UnityEngine.Object.Destroy(BattleSceneRoot.Instance.currentMapObject.gameObject);
						}
						else
						{
							BattleSceneRoot.Instance.currentMapObject.EnableMap(false);
						}
						if (component != null)
						{
							if (BattleSceneRoot.Instance.currentMapObject != null && BattleSceneRoot.Instance.currentMapObject.isCreature)
							{
								UnityEngine.Object.Destroy(BattleSceneRoot.Instance.currentMapObject.gameObject);
								BattleSceneRoot.Instance.currentMapObject = null;
							}
							BattleSceneRoot.Instance.currentMapObject = component;
							BattleSceneRoot.Instance.currentMapObject.ActiveMap(true);
							BattleSceneRoot.Instance.currentMapObject.InitializeMap();
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
		*/
		[HarmonyPatch(typeof(BattleSceneRoot), nameof(BattleSceneRoot.ChangeToEgoMap))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> BattleSceneRoot_ChangeToEgoMap_In(IEnumerable<CodeInstruction> instructions)
		{
			var loadPrefabMethod = Method(typeof(Util), nameof(Util.LoadPrefab), new Type[] { typeof(string), typeof(Transform) });
			var getComponentMethod = Method(typeof(GameObject), nameof(GameObject.GetComponent), Array.Empty<Type>(), new Type[] { typeof(MapManager) });
			var getHelperMethod = Method(typeof(Harmony_Patch), nameof(GetMapComponentFixed));
			var helperMethod = Method(typeof(Harmony_Patch), nameof(BattleSceneRoot_ChangeToEgoMap_CheckCustomMap));
			foreach (var instruction in instructions)
			{
				if (instruction.Is(Callvirt, getComponentMethod))
				{
					yield return new CodeInstruction(Callvirt, getHelperMethod);
				}
				else
				{
					yield return instruction;
				}
				if (instruction.Is(Call, loadPrefabMethod))
				{
					yield return new CodeInstruction(Ldarg_1);
					yield return new CodeInstruction(Call, helperMethod);
				}
			}
		}
		static GameObject BattleSceneRoot_ChangeToEgoMap_CheckCustomMap(GameObject originalMapObject, string mapName)
		{
			try
			{
				if (!mapName.ToLower().StartsWith("custom_"))
				{
					return originalMapObject;
				}

				var customMapName = mapName.Substring("custom_".Length).Trim();
				if (CustomMapManager.TryGetValue(customMapName + "MapManager", out Type mapmanager))
				{
					Debug.Log("Find EGO MapManager:" + customMapName);
					if (mapmanager == null)
					{
						return originalMapObject;
					}
					GameObject customMapObject = Util.LoadPrefab("LibraryMaps/KETHER_Map", BattleSceneRoot.Instance.transform);
					MapManager oldManager = customMapObject.GetComponent<MapManager>();
					MapManager newManager = (MapManager)customMapObject.AddComponent(mapmanager);
					newManager.borderFrame = oldManager.borderFrame;
					newManager.backgroundRoot = oldManager.backgroundRoot;
					newManager._obstacleRoot = oldManager._obstacleRoot;
					newManager._obstacles = oldManager._obstacles;
					newManager._roots = oldManager._roots;
					oldManager.enabled = false;
					UnityEngine.Object.Destroy(oldManager);
					customMapObject.name = "EGO_CardMap_" + mapName;
					if (newManager is CustomMapManager)
					{
						(newManager as CustomMapManager).CustomInit();
					}
					return customMapObject;
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/ChangeToEgoMaperror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return originalMapObject;
		}
		//CanChangeMap
		[HarmonyPatch(typeof(StageController), nameof(StageController.CanChangeMap))]
		[HarmonyPostfix]
		static void StageController_CanChangeMap_Post(ref bool __result)
		{
			if (BattleSceneRoot.Instance.currentMapObject is CustomMapManager customMap)
			{
				__result &= customMap.IsMapChangable();
			}
		}
		//check to prevent AddEgoMapByAssimilation and most of its custom variants
		[HarmonyPatch(typeof(StageController), nameof(StageController.IsTwistedArgaliaBattleEnd))]
		[HarmonyPostfix]
		static void StageController_IsTwistedArgaliaBattleEnd_Post(ref bool __result)
		{
			if (BattleSceneRoot.Instance.currentMapObject is CustomMapManager customMap)
			{
				var frame = new System.Diagnostics.StackFrame(2);
				if (frame.GetMethod().Name.Contains("MapByAssimilation"))
				{
					__result |= !customMap.IsMapChangableByAssimilation();
				}
			}
		}
		//Not be used now
		public static GameObject FindBaseMap(string name)
		{
			GameObject gameObject = null;
			if (name == "malkuth")
			{
				gameObject = Util.LoadPrefab("LibraryMaps/MALKUTH_Map", BattleSceneRoot.Instance.transform);
			}
			if (name == "yesod")
			{
				gameObject = Util.LoadPrefab("LibraryMaps/YESOD_Map", BattleSceneRoot.Instance.transform);
			}
			if (name == "hod")
			{
				gameObject = Util.LoadPrefab("LibraryMaps/HOD_Map", BattleSceneRoot.Instance.transform);
			}
			if (name == "netzach")
			{
				gameObject = Util.LoadPrefab("LibraryMaps/NETZACH_Map", BattleSceneRoot.Instance.transform);
			}
			if (name == "tiphereth")
			{
				gameObject = Util.LoadPrefab("LibraryMaps/TIPHERETH_Map", BattleSceneRoot.Instance.transform);
			}
			if (name == "gebura")
			{
				gameObject = Util.LoadPrefab("LibraryMaps/GEBURAH_Map", BattleSceneRoot.Instance.transform);
			}
			if (name == "chesed")
			{
				gameObject = Util.LoadPrefab("LibraryMaps/CHESED_Map", BattleSceneRoot.Instance.transform);
			}
			if (name == "keter")
			{
				gameObject = Util.LoadPrefab("LibraryMaps/KETHER_Map", BattleSceneRoot.Instance.transform);
			}
			if (name == "hokma")
			{
				gameObject = Util.LoadPrefab("LibraryMaps/HOKMA_Map", BattleSceneRoot.Instance.transform);
			}
			if (name == "binah")
			{
				gameObject = Util.LoadPrefab("LibraryMaps/BINAH_Map", BattleSceneRoot.Instance.transform);
			}
			if (gameObject == null)
			{
				try
				{
					gameObject = Util.LoadPrefab("InvitationMaps/InvitationMap_" + name, BattleSceneRoot.Instance.transform);
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
					gameObject = Util.LoadPrefab("CreatureMaps/CreatureMap_" + name, BattleSceneRoot.Instance.transform);
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
				result = null;
			}
			return result;
		}

		//Others
		//AutoMod1
		[HarmonyPatch(typeof(UIStoryProgressPanel), nameof(UIStoryProgressPanel.SelectedSlot))]
		[HarmonyPrefix]
		static void UIStoryProgressPanel_SelectedSlot_Pre(UIStoryProgressIconSlot slot)
		{
			try
			{
				IsModStorySelected = !string.IsNullOrWhiteSpace(slot?._storyData?.FirstOrDefault()?.workshopID);
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/CheckSelectedSloterror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//AutoMod2
		[HarmonyPatch(typeof(UIInvitationRightMainPanel), nameof(UIInvitationRightMainPanel.SetCustomInvToggle))]
		[HarmonyPrefix]
		static bool UIInvitationRightMainPanel_SetCustomInvToggle_Pre(UIInvitationRightMainPanel __instance, ref bool ison)
		{
			ison |= IsModStorySelected;
			__instance._workshopInvitationToggle.SetIsOnWithoutNotify(ison);
			__instance.customInvPanel.Close();
			__instance.currentSelectedNormalstage = null;
			return false;
		}
		//ErrorNull for delete card from deck
		[HarmonyPatch(typeof(ItemXmlDataList), nameof(ItemXmlDataList.GetCardItem), new Type[] { typeof(LorId), typeof(bool) })]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> ItemXmlDataList_GetCardItem_In(IEnumerable<CodeInstruction> instructions)
		{
			var constructor = Constructor(typeof(DiceCardXmlInfo), new Type[] { typeof(LorId) });
			foreach (var instruction in instructions)
			{
				if (instruction.Is(Newobj, constructor))
				{
					yield return new CodeInstruction(Call, Method(typeof(Harmony_Patch), nameof(ItemXmlDataList_GetCardItem_CheckNullCard)));
				}
				else
				{
					yield return instruction;
				}
			}
		}
		static DiceCardXmlInfo ItemXmlDataList_GetCardItem_CheckNullCard(LorId id)
		{
			if (errNullCard == null)
			{
				errNullCard = new DiceCardXmlInfo(LorId.None);
			}
			return errNullCard;
		}
		//0book to "∞"
		[HarmonyPatch(typeof(UIInvitationDropBookSlot), nameof(UIInvitationDropBookSlot.SetData_DropBook))]
		[HarmonyPostfix]
		static void UIInvitationDropBookSlot_SetData_DropBook_Post(UIInvitationDropBookSlot __instance, LorId bookId)
		{
			try
			{
				if (DropBookInventoryModel.Instance.GetBookCount(bookId) == 0)
				{
					__instance.txt_bookNum.text = "∞";
				}
			}
			catch { }
		}
		//ChangeLanguage
		[HarmonyPatch(typeof(TextDataModel), nameof(TextDataModel.InitTextData))]
		[HarmonyPostfix]
		static void TextDataModel_InitTextData_Post()
		{
			try
			{
				LocalizedTextLoader_New.ExportOriginalFiles();
				LocalizedTextLoader_New.LoadModFiles(ModContentManager.Instance._loadedContents);
			}
			catch (Exception ex)
			{
				ModContentManager.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
				File.WriteAllText(Application.dataPath + "/Mods/InitTextDataerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//LoadStory
		[HarmonyPatch(typeof(StorySerializer), nameof(StorySerializer.LoadStory), new Type[] { typeof(bool) })]
		[HarmonyPostfix]
		static void StorySerializer_LoadStory_Post()
		{
			try
			{
				StorySerializer_new.ExportStory();
			}
			catch (Exception ex)
			{
				ModContentManager.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
				File.WriteAllText(Application.dataPath + "/Mods/LoadStoryerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//DiceAttackEffect
		[HarmonyPatch(typeof(DiceEffectManager), nameof(DiceEffectManager.CreateBehaviourEffect))]
		[HarmonyPrefix]
		static bool DiceEffectManager_CreateBehaviourEffect_Pre(ref DiceAttackEffect __result, string resource, float scaleFactor, BattleUnitView self, BattleUnitView target, float time = 1f)
		{
			try
			{
				if (resource == null || string.IsNullOrWhiteSpace(resource))
				{
					__result = null;
					return false;
				}
				else
				{
					if (CustomEffects.TryGetValue(resource, out var componentType))
					{
						DiceAttackEffect diceAttackEffect = new GameObject(resource).AddComponent(componentType) as DiceAttackEffect;
						diceAttackEffect.Initialize(self, target, time);
						diceAttackEffect.SetScale(scaleFactor);
						__result = diceAttackEffect;
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
		[HarmonyPatch(typeof(UIBattleSettingWaveList), nameof(UIBattleSettingWaveList.SetData))]
		[HarmonyPrefix]
		static void UIBattleSettingWaveList_SetData_Pre(UIBattleSettingWaveList __instance, StageModel stage)
		{
			try
			{
				if (__instance.transform.parent.GetComponent<ScrollRect>() == null)
				{
					var target = __instance.gameObject.transform as RectTransform;
					var scrollView = new GameObject("[Rect]WaveListView");
					var scrollTransform = scrollView.AddComponent<RectTransform>();
					scrollTransform.SetParent(target.parent);
					scrollTransform.localPosition = new Vector3(0, -35, 0);
					scrollTransform.localEulerAngles = Vector3.zero;
					scrollTransform.localScale = Vector3.one;
					scrollTransform.sizeDelta = Vector2.one * 800;
					scrollView.AddComponent<RectMask2D>();
					target.SetParent(scrollTransform, true);
					var scrollRect = scrollView.AddComponent<ScrollRect>();
					scrollRect.content = target;
					scrollRect.scrollSensitivity = 15f;
					scrollRect.horizontal = false;
					scrollRect.movementType = ScrollRect.MovementType.Elastic;
					scrollRect.elasticity = 0.1f;
				}
				if (stage.waveList.Count > __instance.waveSlots.Count)
				{
					var newList = new List<UIBattleSettingWaveSlot>(stage.waveList.Count - __instance.waveSlots.Count);
					for (int i = __instance.waveSlots.Count; i < stage.waveList.Count; i++)
					{
						UIBattleSettingWaveSlot uibattleSettingWaveSlot = UnityEngine.Object.Instantiate(__instance.waveSlots[0], __instance.waveSlots[0].transform.parent);
						uibattleSettingWaveSlot.name = $"[Rect]WaveSlot ({i})";
						newList.Add(uibattleSettingWaveSlot);
					}
					newList.Reverse();
					__instance.waveSlots.InsertRange(0, newList);
				}
				InitUIBattleSettingWaveSlots(__instance.waveSlots);
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/UIBSWLerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		/* handled by SerializeField already
		static void InitUIBattleSettingWaveSlot(UIBattleSettingWaveSlot slot, UIBattleSettingWaveList list)
		{
			slot.panel = list;
			var rect = slot.transform as RectTransform;
			slot.rect = rect;
			var waveIcon = rect.Find("[Rect]WaveIcon");
			var circle = waveIcon.Find("[Image]CircleFrame");
			var circleGlow = waveIcon.Find("[Image]CircleFrameGlow");
			var icon = waveIcon.Find("[Image]Icon");
			var iconGlow = waveIcon.Find("[Image]IconGlow");
			slot.img_circle = circle.GetComponent<Image>();
			slot.hsv_Circle = circle.GetComponent<_2dxFX_HSV>();
			slot.img_circleglow = circleGlow.GetComponent<Image>();
			slot.hsv_CircleGlow = circleGlow.GetComponent<_2dxFX_HSV>();
			slot.img_Icon = icon.GetComponent<Image>();
			slot.hsv_Icon = icon.GetComponent<_2dxFX_HSV>();
			slot.img_IconGlow = iconGlow.GetComponent<Image>();
			slot.hsv_IconGlow = iconGlow.GetComponent<_2dxFX_HSV>();
			var text = rect.Find("[Text]AlarmText");
			slot.txt_Alarm = text.GetComponent<TextMeshProUGUI>();
			slot.materialsetter_txtAlarm = text.GetComponent<TextMeshProMaterialSetter>();
			slot.arrow = rect.Find("[Image]Arrow (1)").GetComponent<Image>();
			slot.defeatColor = new Color(0.454902f, 0.1098039f, 0f, 1f);
			slot.anim = slot.GetComponent<Animator>();
			slot.cg = slot.GetComponent<CanvasGroup>();
			slot.transform.localPosition = new Vector2(120f, 0f);
			slot.gameObject.SetActive(false);
		}
		*/
		static void InitUIBattleSettingWaveSlots(List<UIBattleSettingWaveSlot> slots)
		{
			for (int i = 0; i < slots.Count; i++)
			{
				slots[i].gameObject.transform.localScale = new Vector3(1f, 1f);
			}
		}
		//over 999 dicevalue
		[HarmonyPatch(typeof(BattleSimpleActionUI_Dice), nameof(BattleSimpleActionUI_Dice.SetDiceValue))]
		[HarmonyPrefix]
		static bool BattleSimpleActionUI_Dice_SetDiceValue_Pre(BattleSimpleActionUI_Dice __instance, bool enable, int diceValue)
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
		[HarmonyPatch(typeof(VersionViewer), nameof(VersionViewer.Start))]
		[HarmonyPrefix]
		static void VersionViewer_Start_Pre(VersionViewer __instance)
		{
			__instance.GetComponent<Text>().fontSize = 30;
			__instance.gameObject.transform.localPosition = new Vector3(-830f, -460f);
		}
		//CopyCheck
		[HarmonyPatch(typeof(DiceCardXmlInfo), nameof(DiceCardXmlInfo.Copy))]
		[HarmonyPostfix]
		static void DiceCardXmlInfo_Copy_Post(DiceCardXmlInfo __instance, ref DiceCardXmlInfo __result)
		{
			__result.Keywords = __instance.Keywords.ToList();
		}
		/*
		//Mod_Update
		//Using For Reload
		[HarmonyPatch(typeof(DebugConsoleScript), nameof(DebugConsoleScript.Update))]
		[HarmonyPrefix]
		static void Mod_Update()
		{
			try
			{
				if (!IsEditing && entryScene != null && Input.GetKeyDown(KeyCode.R))
				{
					File.WriteAllText(Application.dataPath + "/Mods/PressSuccess.log", "success");
					entryScene.OnCompleteInitializePlatform_xboxlive(true);
				}
				IsEditing = true;
			}
			catch (Exception ex)
			{
				ModContentManager.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
				File.WriteAllText(Application.dataPath + "/Mods/Updateerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//Using For Reload
		[HarmonyPatch(typeof(ModContentManager), nameof(ModContentManager.SetActiveContents))]
		[HarmonyPrefix]
		static void ModContentManager_SetActiveContents_Pre(ModContentManager __instance)
		{
			__instance._loadedContents.Clear();
		}
		//Using For Reload
		[HarmonyPatch(typeof(UIModPopup), nameof(UIModPopup.Close))]
		[HarmonyPrefix]
		static void UIModPopup_Close_Post()
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
				ModContentManager.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
				File.WriteAllText(Application.dataPath + "/Mods/ModSettingerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			IsEditing = false;
		}*/

		//ModItemSort
		//BookSort
		[HarmonyPatch(typeof(UIInvitationDropBookList), nameof(UIInvitationDropBookList.ApplyFilterAll))]
		[HarmonyPostfix]
		static void UIInvitationDropBookList_ApplyFilterAll_Post(UIInvitationDropBookList __instance)
		{
			try
			{
				var resorted = __instance._currentBookIdList.OrderBy(x => string.IsNullOrWhiteSpace(x.packageId) || x.packageId.EndsWith("@origin") ? "" : x.packageId).ToArray();
				__instance._currentBookIdList.Clear();
				__instance._currentBookIdList.AddRange(resorted);
				__instance.SelectablePanel.ChildSelectable = __instance.BookSlotList[0].selectable;
				__instance.UpdateBookListPage(false);
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/ModBookSort_Invi.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//BookSort
		[HarmonyPatch(typeof(UIInvenFeedBookList), nameof(UIInvenFeedBookList.ApplyFilterAll))]
		[HarmonyPostfix]
		static void UIInvenFeedBookList_ApplyFilterAll_Post(UIInvenFeedBookList __instance)
		{
			try
			{
				var resorted = __instance._currentBookIdList.OrderBy(x => string.IsNullOrWhiteSpace(x.packageId) || x.packageId.EndsWith("@origin") ? "" : x.packageId).ToArray();
				__instance._currentBookIdList.Clear();
				__instance._currentBookIdList.AddRange(resorted);
				__instance.UpdateBookListPage(false);
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/ModBookSort_Feed.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//CardSort
		[HarmonyPatch(typeof(UIInvenCardListScroll), nameof(UIInvenCardListScroll.ApplyFilterAll))]
		[HarmonyPrefix]
		static bool UIInvenCardListScroll_ApplyFilterAll_Pre(UIInvenCardListScroll __instance)
		{
			try
			{
				__instance._currentCardListForFilter.Clear();
				List<DiceCardItemModel> cardsByDetailFilterUI = __instance.GetCardsByDetailFilterUI(__instance.GetCardBySearchFilterUI(__instance.GetCardsByCostFilterUI(__instance.GetCardsByGradeFilterUI(__instance._originCardList))));
				cardsByDetailFilterUI.Sort(new Comparison<DiceCardItemModel>(ModCardItemSort));
				if (__instance._unitdata != null)
				{
					Predicate<DiceCardItemModel> cond1 = (DiceCardItemModel x) => true;
					switch (__instance._unitdata.bookItem.ClassInfo.RangeType)
					{
						case EquipRangeType.Melee:
							cond1 = (DiceCardItemModel x) => x.GetSpec().Ranged != CardRange.Far;
							break;
						case EquipRangeType.Range:
							cond1 = (DiceCardItemModel x) => x.GetSpec().Ranged != CardRange.Near;
							break;
						case EquipRangeType.Hybrid:
							cond1 = (DiceCardItemModel x) => true;
							break;
					}
					List<DiceCardXmlInfo> onlyCards = __instance._unitdata.bookItem.GetOnlyCards();
					Predicate<DiceCardItemModel> cond2 = ((DiceCardItemModel x) => onlyCards.Exists((DiceCardXmlInfo y) => y.id == x.GetID()));
					__instance._currentCardListForFilter.AddRange(cardsByDetailFilterUI.FindAll((DiceCardItemModel x) => x.ClassInfo.optionList.Contains(CardOption.OnlyPage) ? cond2(x) : cond1(x)));
					__instance._currentCardListForFilter.AddRange(cardsByDetailFilterUI.FindAll((DiceCardItemModel x) => x.ClassInfo.optionList.Contains(CardOption.OnlyPage) && !cond1(x)));
				}
				else
				{
					__instance._currentCardListForFilter.AddRange(cardsByDetailFilterUI);
				}
				int num = __instance.GetMaxRow();
				__instance.scrollBar.SetScrollRectSize(__instance.column * __instance.slotWidth, (num + (float)__instance.row - 1f) * __instance.slotHeight);
				__instance.scrollBar.SetWindowPosition(0f, 0f);
				__instance.selectablePanel.ChildSelectable = __instance.slotList[0].selectable;
				__instance.SetCardsData(__instance.GetCurrentPageList());
				return false;
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/ModCardSort.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return true;
		}
		static int ModCardItemSort(DiceCardItemModel a, DiceCardItemModel b)
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
		[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.SavePlayData))]
		[HarmonyPrefix]
		static void SaveManager_SavePlayData_Pre()
		{
			try
			{
				ModSaveTool.SaveModSaveData();/*
				if (File.Exists(SaveManager.savePath + "/Player.log"))
				{
					if (File.Exists(Application.dataPath + "/Mods/Player.log"))
					{
						File.Delete(Application.dataPath + "/Mods/Player.log");
					}
					File.Copy(SaveManager.savePath + "/Player.log", Application.dataPath + "/Mods/Player.log");
				}*/
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/SaveFailed.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//RemoveUnknownSaves
		/*
		[HarmonyPatch(typeof(GameOpeningController), nameof(GameOpeningController.StopOpening))]
		[HarmonyPostfix]
		static void OnLoadMainScene(Scene scene, LoadSceneMode _)
		{
			if (scene.name == "Stage_Hod_New")
			{
				SceneManager.sceneLoaded -= OnLoadMainScene;
				GameOpeningController_StopOpening_Post();
			}
		}
		static void GameOpeningController_StopOpening_Post()
		{
			try
			{
				LoadCoreThumbs();
				LoadCoreSounds();
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/LoadFromModSaveDataerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//CustomGift
		//CreateGiftData
		/*
		[HarmonyPatch(typeof(CharacterAppearance), nameof(CharacterAppearance.CreateGiftData))]
		[HarmonyPrefix]
		static bool CharacterAppearance_CreateGiftData_Pre(CharacterAppearance __instance, ref GiftAppearance __result, GiftModel gift, string resPath)
		{
			try
			{
				if (__instance._customAppearance == null)
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
		*/
		[HarmonyPatch(typeof(CharacterAppearance), nameof(CharacterAppearance.CreateGiftData))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> CharacterAppearance_CreateGiftData_In(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo loadMethod = GenericMethod(typeof(Resources), nameof(Resources.Load), new Type[] { typeof(GameObject) });
			foreach (var instruction in instructions)
			{
				yield return instruction;
				if (instruction.Is(Call, loadMethod))
				{
					yield return new CodeInstruction(Ldarg_2);
					yield return new CodeInstruction(Call, Method(typeof(Harmony_Patch), nameof(CharacterAppearance_CreateGiftData_CheckCustomGift)));
				}
			}
		}
		static MethodInfo GenericMethod(Type type, string name, Type[] generics)
		{
			return FirstMethod(type, method => method.Name == name && method.IsGenericMethod).MakeGenericMethod(generics);
		}
		static GameObject CharacterAppearance_CreateGiftData_CheckCustomGift(GameObject originalGift, string resPath)
		{
			try
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
					return originalGift;
				}
				else
				{
					var giftAppearance = CustomGiftAppearancePrefabObject;

					giftAppearance._frontSpriteRenderer.enabled = CustomGiftAppearance.GiftArtWork.TryGetValue(array2[2] + "_front", out var frontSprite);
					giftAppearance._frontSpriteRenderer.sprite = frontSprite;

					giftAppearance._sideSpriteRenderer.enabled = CustomGiftAppearance.GiftArtWork.TryGetValue(array2[2] + "_side", out var sideSprite);
					giftAppearance._sideSpriteRenderer.sprite = sideSprite;

					giftAppearance._frontBackSpriteRenderer.enabled = CustomGiftAppearance.GiftArtWork.TryGetValue(array2[2] + "_frontBack", out var frontBackSprite);
					giftAppearance._frontBackSpriteRenderer.sprite = frontBackSprite;

					giftAppearance._sideBackSpriteRenderer.enabled = CustomGiftAppearance.GiftArtWork.TryGetValue(array2[2] + "_sideBack", out var sideBackSprite);
					giftAppearance._sideBackSpriteRenderer.sprite = sideBackSprite;

					return giftAppearance.gameObject;
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/CACGDerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return originalGift;
		}
		internal static GiftAppearance CustomGiftAppearancePrefabObject
		{
			get
			{
				if (_giftAppearance == null)
				{
					_giftAppearance = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Gifts/Gifts_NeedRename/Gift_Challenger"), XLRoot.persistentRoot.transform).GetComponent<GiftAppearance>();
					_giftAppearance._frontSpriteRenderer.gameObject.transform.localScale = Vector2.one;
					_giftAppearance._sideSpriteRenderer.gameObject.transform.localScale = Vector2.one;
					_giftAppearance._frontBackSpriteRenderer.gameObject.transform.localScale = Vector2.one;
					_giftAppearance._sideBackSpriteRenderer.gameObject.transform.localScale = Vector2.one;
				}
				return _giftAppearance;
			}
		}

		//GiftPassive
		/*
		[HarmonyPatch(typeof(GiftModel), nameof(GiftModel.CreateScripts))]
		[HarmonyPrefix]
		static bool GiftModel_CreateScripts_Pre(GiftModel __instance, ref List<PassiveAbilityBase> __result)
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
		*/
		[HarmonyPatch(typeof(GiftModel), nameof(GiftModel.CreateScripts))]
		[HarmonyPriority(Priority.Low)]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> GiftModel_CreateScripts_In(IEnumerable<CodeInstruction> instructions)
		{
			var getTypeMethod = Method(typeof(Type), nameof(Type.GetType), new Type[] { typeof(string) });
			var hideMethod = Method(typeof(PassiveAbilityBase), nameof(PassiveAbilityBase.Hide));
			foreach (var instruction in instructions)
			{
				if (instruction.Is(Callvirt, hideMethod))
				{
					yield return new CodeInstruction(Pop);
					continue;
				}
				yield return instruction;
				if (instruction.Is(Call, getTypeMethod))
				{
					yield return new CodeInstruction(Ldloc_2);
					yield return new CodeInstruction(Call, Method(typeof(Harmony_Patch), nameof(GiftModel_CreateScripts_CheckCustomGiftAbility)));
				}
			}
		}
		[HarmonyPatch(typeof(GiftModel), nameof(GiftModel.CreateScripts))]
		[HarmonyPostfix]
		static void GiftModel_CreateScripts_Post(GiftModel __instance, List<PassiveAbilityBase> __result)
		{
			if (__instance.ClassInfo is GiftXmlInfo_V2 newInfo)
			{
				for (int i = 0; i < newInfo.CustomScriptList.Count; i++)
				{
					try
					{
						var ability = newInfo.CustomScriptList[i];
						Type type = FindGiftPassiveAbilityType(ability);
						PassiveAbilityBase passiveAbilityBase = null;
						if (type != null)
						{
							try
							{
								passiveAbilityBase = (PassiveAbilityBase)Activator.CreateInstance(type);
							}
							catch
							{

							}
						}
						if (passiveAbilityBase == null)
						{
							passiveAbilityBase = new PassiveAbilityBase();
						}
						passiveAbilityBase.name = __instance.GetName();
						passiveAbilityBase.desc = __instance.GiftDesc;
						__result.Add(passiveAbilityBase);
					}
					catch (Exception)
					{

					}
				}
			}
		}
		static Type GiftModel_CreateScripts_CheckCustomGiftAbility(Type oldType, int num)
		{
			return FindGiftPassiveAbilityType(num.ToString().Trim()) ?? oldType;
		}
		public static PassiveAbilityBase FindGiftPassiveAbility(string name)
		{
			try
			{
				return Activator.CreateInstance(FindGiftPassiveAbilityType(name)) as PassiveAbilityBase;
			}
			catch
			{
				return null;
			}
		}
		public static Type FindGiftPassiveAbilityType(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				name = null;
			}
			else
			{
				name = name.Trim();
				if (CustomGiftPassive.TryGetValue(name, out Type type))
				{
					return type;
				}
			}

			Type result = null;
			if (!CoreGiftPassivesLoaded)
			{
				var baseType = typeof(PassiveAbilityBase);
				foreach (Type type2 in Assembly.Load("Assembly-CSharp").GetTypes())
				{
					if (baseType.IsAssignableFrom(type2) && type2.Name.StartsWith("GiftPassiveAbility_"))
					{
						var typeName = type2.Name.Substring("GiftPassiveAbility_".Length);
						if (!CustomGiftPassive.ContainsKey(typeName))
						{
							CustomGiftPassive[typeName] = type2;
							if (typeName == name)
							{
								result = type2;
							}
						}
					}
				}
				CoreGiftPassivesLoaded = true;
			}
			return result;
		}
		[HarmonyPatch(typeof(BattleUnitPassiveDetail), nameof(BattleUnitPassiveDetail.Init))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> BattleUnitPassiveDetail_Init_In(IEnumerable<CodeInstruction> instructions)
		{
			var addRangeMethod = Method(typeof(List<PassiveAbilityBase>), nameof(List<PassiveAbilityBase>.AddRange));
			var firstRangeMethod = Method(typeof(Harmony_Patch), nameof(BattleUnitPassiveDetail_Init_InsertFirst));
			var bookPassivesMethod = Method(typeof(BookModel), nameof(BookModel.CreatePassiveList));
			bool book = false;
			foreach (var instruction in instructions)
			{
				if (book && instruction.Is(Callvirt, addRangeMethod))
				{
					yield return new CodeInstruction(Callvirt, firstRangeMethod);
				}
				else
				{
					yield return instruction;
					if (instruction.Is(Callvirt, bookPassivesMethod))
					{
						book = true;
					}
				}
			}
		}
		static void BattleUnitPassiveDetail_Init_InsertFirst(List<PassiveAbilityBase> passiveList, List<PassiveAbilityBase> bookPassiveList)
		{
			passiveList.InsertRange(0, bookPassiveList);
		}
		//GiftDataSlot
		/*
		[HarmonyPatch(typeof(UIGiftDataSlot), nameof(UIGiftDataSlot.SetData))]
		[HarmonyPrefix]
		static bool UIGiftDataSlot_SetData_Pre(UIGiftDataSlot __instance, GiftModel data)
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
					__instance.img_giftImage.enabled = true;
					__instance.img_xmark.enabled = false;
					__instance.img_giftMask.enabled = true;
					__instance.OpenInit();
					giftAppearance = CustomGiftAppearance.CreateCustomGift(array2);
					giftAppearance.gameObject.SetActive(false);
					if (giftAppearance != null)
					{
						if (giftAppearance is GiftAppearance_Aura)
						{
							__instance.img_giftImage.enabled = true;
							__instance.img_giftImage.sprite = UISpriteDataManager.instance.GiftAuraIcon;
							__instance.img_giftImage.rectTransform.localScale = new Vector2(0.8f, 0.8f);
						}
						else
						{
							__instance.img_giftImage.sprite = giftAppearance.GetGiftPreview();
							__instance.img_giftImage.rectTransform.localScale = Vector2.one;
						}
						if (__instance.img_giftImage.sprite == null)
						{
							__instance.img_giftImage.enabled = false;
						}
					}
					else
					{
						__instance.img_giftImage.enabled = false;
						__instance.img_giftMask.enabled = false;
					}
					__instance.txt_giftName.text = data.GetName();
					__instance.txt_giftNameDetail.text = data.GiftDesc;
					__instance.img_giftImage.gameObject.SetActive(true);
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
					__instance.txt_giftPartName.text = TextDataModel.GetText(id, Array.Empty<object>());
					return false;
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/GiftSetDataerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return true;
		}
		*/
		[HarmonyPatch(typeof(UIGiftInvenSlot), nameof(UIGiftInvenSlot.SetData))]
		[HarmonyPatch(typeof(UIGiftDataSlot), nameof(UIGiftDataSlot.SetData))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> UIGiftSlot_SetData_In(IEnumerable<CodeInstruction> instructions)
		{
			var loadGiftMethod = GenericMethod(typeof(Resources), nameof(Resources.Load), new Type[] { typeof(GiftAppearance) });
			foreach (var instruction in instructions)
			{
				yield return instruction;
				if (instruction.Is(Call, loadGiftMethod))
				{
					yield return new CodeInstruction(Ldarg_1);
					yield return new CodeInstruction(Call, Method(typeof(Harmony_Patch), nameof(UIGiftDataSlot_SetData_CheckCustomGift)));
				}
			}
		}
		static GiftAppearance UIGiftDataSlot_SetData_CheckCustomGift(GiftAppearance originalAppearance, GiftModel data)
		{
			try
			{
				if (data != null)
				{
					string[] array = data.GetResourcePath().Split(new char[]
					{
						 '/'
					});
					string[] array2 = array[array.Length - 1].Split(new char[]
					{
						 '_'
					});
					if (array2[1].ToLower() != "custom")
					{
						return originalAppearance;
					}
					return CustomGiftAppearance.CreateCustomGift(array2);
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/GiftSetDataerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return originalAppearance;
		}
		//UIGiftInvenSlot
		/*
		[HarmonyPatch(typeof(UIGiftInvenSlot), nameof(UIGiftInvenSlot.SetData))]
		[HarmonyPrefix]
		static bool UIGiftInvenSlot_SetData_Pre(UIGiftInvenSlot __instance, GiftModel gift, UIGiftInventory inven)
		{
			try
			{
				__instance.gameObject.SetActive(true);
				__instance.giftData = gift;
				__instance.panel = inven;
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
				__instance.img_Gift.enabled = true;
				if (giftAppearance != null)
				{
					if (giftAppearance is GiftAppearance_Aura)
					{
						__instance.img_Gift.sprite = UISpriteDataManager.instance.GiftAuraIcon;
						__instance.img_Gift.rectTransform.localScale = new Vector2(0.8f, 0.8f);
					}
					else
					{
						__instance.img_Gift.sprite = giftAppearance.GetGiftPreview();
						__instance.img_Gift.rectTransform.localScale = new Vector2(1f, 1f);
					}
				}
				if (__instance.img_Gift.sprite == null)
				{
					__instance.img_Gift.enabled = false;
				}
				__instance.txt_Part.text = TextDataModel.GetText(id, Array.Empty<object>());
				__instance.txt_Name.text = gift.GetName();
				__instance.txt_desc.text = gift.GiftDesc;
				__instance.txt_getcondition.text = gift.GiftAcquireCondition;
				__instance.conditionTextGameObject.SetActive(true);
				return false;
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/GiftInvSetDataerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return true;
		}
		*/
		//UIGiftPreviewSlot
		/*
		[HarmonyPatch(typeof(UIGiftPreviewSlot), nameof(UIGiftPreviewSlot.UpdateSlot))]
		[HarmonyPrefix]
		static bool UIGiftPreviewSlot_UpdateSlot_Pre(UIGiftPreviewSlot __instance)
		{
			try
			{
				if (__instance.Gift != null)
				{
					string[] array = __instance.Gift.GetResourcePath().Split(new char[]
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
						__instance.txt_GiftName.gameObject.SetActive(true);
						__instance.txt_GiftName.text = __instance.Gift.GetName();
						__instance.txt_GiftDesc.text = __instance.Gift.GiftDesc;
						__instance.img_Gift.gameObject.SetActive(true);
						__instance.img_Gift.enabled = true;
						giftAppearance = CustomGiftAppearance.CreateCustomGift(array2);
						giftAppearance.gameObject.SetActive(false);
					}
					if (giftAppearance != null)
					{
						if (giftAppearance is GiftAppearance_Aura)
						{
							__instance.img_Gift.sprite = UISpriteDataManager.instance.GiftAuraIcon;
							__instance.img_Gift.rectTransform.localScale = new Vector2(0.8f, 0.8f);
						}
						else
						{
							__instance.img_Gift.sprite = giftAppearance.GetGiftPreview();
							__instance.img_Gift.rectTransform.localScale = new Vector2(1f, 1f);
						}
					}
					if (__instance.img_Gift.sprite == null)
					{
						__instance.img_Gift.enabled = false;
					}
					__instance.detailcRect.SetActive(__instance.panel.giftDetailToggle.isOn);
				}
				else
				{
					__instance.txt_GiftName.gameObject.SetActive(false);
					__instance.img_Gift.gameObject.SetActive(false);
					__instance.detailcRect.SetActive(false);
				}
				__instance.SetEyeButton(__instance.isEyeOpen);
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
		*/
		[HarmonyPatch(typeof(UIGiftPreviewSlot), nameof(UIGiftPreviewSlot.UpdateSlot))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> UIGiftPreviewSlot_UpdateSlot_In(IEnumerable<CodeInstruction> instructions)
		{
			var loadGiftMethod = GenericMethod(typeof(Resources), nameof(Resources.Load), new Type[] { typeof(GiftAppearance) });
			foreach (var instruction in instructions)
			{
				yield return instruction;
				if (instruction.Is(Call, loadGiftMethod))
				{
					yield return new CodeInstruction(Ldarg_0);
					yield return new CodeInstruction(Ldfld, Field(typeof(UIGiftPreviewSlot), nameof(UIGiftPreviewSlot.Gift)));
					yield return new CodeInstruction(Call, Method(typeof(Harmony_Patch), nameof(UIGiftDataSlot_SetData_CheckCustomGift)));
				}
			}
		}
		//LoadGift
		[HarmonyPatch(typeof(GiftInventory), nameof(GiftInventory.LoadFromSaveData))]
		[HarmonyPrefix]
		static void GiftInventory_LoadFromSaveData_Pre(SaveData data)
		{
			var equipIds = new HashSet<int>();
			FixGiftSaveList(data, "equipList", returnIds: equipIds);
			FixGiftSaveList(data, "unequipList");
			FixGiftSaveList(data, "offList", checkIds: equipIds);
		}
		static void FixGiftSaveList(SaveData data, string giftListName, HashSet<int> returnIds = null, HashSet<int> checkIds = null)
		{
			var saveData = data.GetData(giftListName);
			if (saveData == null)
			{
				saveData = new SaveData(new List<int>());
				data.AddData(giftListName, saveData);
			}
			else if (saveData._list == null)
			{
				saveData._list = new List<SaveData>();
			}
			GiftXmlList giftXmlList = GiftXmlList.Instance;
			if (checkIds == null)
			{
				saveData._list.RemoveAll(x =>
				{
					try
					{
						return giftXmlList.GetData(x.GetIntSelf()) == null;
					}
					catch
					{
						return true;
					}
				});
			}
			else
			{
				saveData._list.RemoveAll(x =>
				{
					try
					{
						return !checkIds.Contains(x.GetIntSelf());
					}
					catch
					{
						return true;
					}
				});
			}
			returnIds?.UnionWith(saveData._list.ConvertAll(x => x.GetIntSelf()));
		}
		[HarmonyPatch(typeof(GiftInventory), nameof(GiftInventory.LoadFromSaveData))]
		[HarmonyPostfix]
		static void GiftInventory_LoadFromSaveData_Post(GiftInventory __instance)
		{
			var owner = __instance._owner;
			if (owner == null)
			{
				return;
			}
			var floor = LibraryModel.Instance.GetFloor(owner.OwnerSephirah);
			if (floor == null)
			{
				return;
			}
			var index = floor._unitDataList.FindIndex(unit => unit == owner);
			if (index < 0)
			{
				return;
			}
			var basemodGiftData = LibraryModel.Instance.CustomStorage.GetStageStorageData("BasemodGift");
			if (basemodGiftData == null)
			{
				return;
			}
			var floorData = basemodGiftData.GetData(owner.OwnerSephirah.ToString());
			if (floorData == null)
			{
				return;
			}
			var data = floorData.GetData(index.ToString());
			if (data == null)
			{
				return;
			}

			SaveData eqList = data.GetData("equipList");
			SaveData uneqList = data.GetData("unequipList");
			SaveData offList = data.GetData("offList");
			if (eqList != null)
			{
				foreach (SaveData saveData in eqList)
				{
					var id = LorId.LoadFromSaveData(saveData);
					if (OrcTools.CustomGifts.TryGetValue(id, out var gift))
					{
						var model = new GiftModel(gift);
						__instance.AddGift(model);
						__instance.Equip(model);
					}
				}
			}
			if (uneqList != null)
			{
				foreach (SaveData saveData in uneqList)
				{
					var id = LorId.LoadFromSaveData(saveData);
					if (OrcTools.CustomGifts.TryGetValue(id, out var gift))
					{
						var model = new GiftModel(gift);
						__instance.AddGift(model);
					}
				}
			}
			if (offList != null)
			{
				foreach (SaveData saveData in offList)
				{
					var id = LorId.LoadFromSaveData(saveData);
					if (OrcTools.CustomGifts.TryGetValue(id, out var gift))
					{
						var copy = __instance._equippedList.Find(model => model.ClassInfo == gift);
						if (copy != null)
						{
							copy.isShowEquipGift = false;
						}
					}
				}
			}
		}
		//SaveGift
		[HarmonyPatch(typeof(GiftInventory), nameof(GiftInventory.GetSaveData))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> GiftInventory_GetSaveData_In(IEnumerable<CodeInstruction> instructions)
		{
			var field1 = Field(typeof(GiftInventory), nameof(GiftInventory._equippedList));
			var field2 = Field(typeof(GiftInventory), nameof(GiftInventory._unequippedList));
			var helper = Method(typeof(Harmony_Patch), nameof(FilterGiftsForSave));
			foreach (var instruction in instructions)
			{
				yield return instruction;
				if (instruction.LoadsField(field1) || instruction.LoadsField(field2))
				{
					yield return new CodeInstruction(Call, helper);
				}
			}
		}
		static List<GiftModel> FilterGiftsForSave(List<GiftModel> unfiltered)
		{
			return unfiltered.FindAll(gift => !(gift.ClassInfo is GiftXmlInfo_V2 giftNew) || giftNew.dontRemove);
		}
		[HarmonyPatch(typeof(CustomSaveStorageModel), nameof(CustomSaveStorageModel.GetSaveData))]
		[HarmonyPrefix]
		static void CustomSaveStorageModel_GetSaveData_Pre(CustomSaveStorageModel __instance)
		{
			var rootSave = __instance.GetStageStorageData("BasemodGift") ?? new SaveData(SaveDataType.Dictionary);
			foreach (var floor in LibraryModel.Instance._floorList)
			{
				UpdateFloorGifts(rootSave, floor);
			}
			__instance.SetStageStorgeData("BasemodGift", rootSave);
		}
		static void UpdateFloorGifts(SaveData librarySave, LibraryFloorModel floor)
		{
			var floorSave = librarySave.GetData(floor.Sephirah.ToString()) ?? new SaveData(SaveDataType.Dictionary);
			for (int i = 0; i < floor._unitDataList.Count; i++)
			{
				UpdateUnitGifts(floorSave, floor._unitDataList[i], i);
			}
			librarySave.GetDictionarySelf()[floor.Sephirah.ToString()] = floorSave;
		}
		static void UpdateUnitGifts(SaveData floorSave, UnitDataModel unit, int index)
		{
			var data = floorSave.GetData(index.ToString()) ?? new SaveData(SaveDataType.Dictionary);

			SaveData eqSave = data.GetData("equipList");
			SaveData uneqSave = data.GetData("unequipList");
			SaveData offSave = data.GetData("offList");
			HashSet<LorId> eqIds = new HashSet<LorId>();
			HashSet<LorId> uneqIds = new HashSet<LorId>();
			HashSet<LorId> offIds = new HashSet<LorId>();
			if (eqSave != null)
			{
				foreach (var idSave in eqSave)
				{
					var id = LorId.LoadFromSaveData(idSave);
					eqIds.Add(id);
				}
			}
			if (offSave != null)
			{
				foreach (var idSave in offSave)
				{
					var id = LorId.LoadFromSaveData(idSave);
					offIds.Add(id);
				}
			}
			if (uneqSave != null)
			{
				foreach (var idSave in uneqSave)
				{
					var id = LorId.LoadFromSaveData(idSave);
					eqIds.Remove(id);
					uneqIds.Add(id);
				}
			}
			foreach (var gift in unit.giftInventory._equippedList)
			{
				if (gift.ClassInfo is GiftXmlInfo_V2 giftNew && !giftNew.dontRemove)
				{
					eqIds.Add(giftNew.lorId);
					if (!gift.isShowEquipGift)
					{
						offIds.Add(giftNew.lorId);
					}
					uneqIds.Remove(giftNew.lorId);
				}
			}
			foreach (var gift in unit.giftInventory._unequippedList)
			{
				if (gift.ClassInfo is GiftXmlInfo_V2 giftNew && !giftNew.dontRemove)
				{
					eqIds.Remove(giftNew.lorId);
					uneqIds.Add(giftNew.lorId);
				}
			}
			offIds.IntersectWith(eqIds);

			eqSave = new SaveData(SaveDataType.List);
			uneqSave = new SaveData(SaveDataType.List);
			offSave = new SaveData(SaveDataType.List);
			foreach (var id in eqIds)
			{
				eqSave.AddToList(id.GetSaveData());
			}
			foreach (var id in uneqIds)
			{
				uneqSave.AddToList(id.GetSaveData());
			}
			foreach (var id in offIds)
			{
				offSave.AddToList(id.GetSaveData());
			}

			data.GetDictionarySelf()["equipList"] = eqSave;
			data.GetDictionarySelf()["unequipList"] = uneqSave;
			data.GetDictionarySelf()["offList"] = offSave;

			floorSave.GetDictionarySelf()[index.ToString()] = data;
		}
		//BehaviorAbilityData
		[HarmonyPatch(typeof(BattleCardBehaviourResult), nameof(BattleCardBehaviourResult.GetAbilityDataAfterRoll))]
		[HarmonyPostfix]
		static List<EffectTypoData> BattleCardBehaviourResult_GetAbilityDataAfterRoll_Post(List<EffectTypoData> list, BattleCardBehaviourResult __instance)
		{
			try
			{
				if (CustomEffectTypoData.TryGetValue(__instance, out List<EffectTypoData> addedlist))
				{
					CustomEffectTypoData.Remove(__instance);
					list.AddRange(addedlist);
				}
			}
			catch { }
			return list;
		}
		[HarmonyPatch(typeof(BattleActionTypoSlot), nameof(BattleActionTypoSlot.SetData))]
		[HarmonyPostfix]
		static void BattleActionTypoSlot_SetData_Post(BattleActionTypoSlot __instance, EffectTypoData data)
		{
			try
			{
				if (data is EffectTypoData_New newData)
				{
					if (newData.battleUIPassiveSet != null)
					{
						EffectTypoData_New.BattleUIPassiveSet newBattleUIPassiveSet = newData.battleUIPassiveSet;
						BattleUIPassiveSet uIPassiveSet = new BattleUIPassiveSet()
						{
							type = newData.type,
							frame = newBattleUIPassiveSet.frame,
							Icon = newBattleUIPassiveSet.Icon,
							IconGlow = newBattleUIPassiveSet.IconGlow,
							textColor = newBattleUIPassiveSet.textColor,
							IconColor = newBattleUIPassiveSet.IconColor,
							IconGlowColor = newBattleUIPassiveSet.IconGlowColor,
						};
						UISpriteDataManager.instance.BattleUIEffectSetDic[uIPassiveSet.type] = uIPassiveSet;
					}
					if (UISpriteDataManager.instance.BattleUIEffectSetDic.TryGetValue(newData.type, out BattleUIPassiveSet battleUIPassiveSet))
					{
						__instance.img_Icon.sprite = battleUIPassiveSet.Icon;
						__instance.img_Icon.color = battleUIPassiveSet.IconColor;
						if (battleUIPassiveSet.IconGlow != null)
						{
							__instance.img_IconGlow.enabled = true;
							__instance.img_IconGlow.sprite = battleUIPassiveSet.IconGlow;
							__instance.img_IconGlow.color = battleUIPassiveSet.IconGlowColor;
						}
						else
						{
							__instance.img_IconGlow.enabled = false;
						}
						__instance.img_Frame.sprite = battleUIPassiveSet.frame;
						__instance.txt_desc.color = battleUIPassiveSet.textColor;
						__instance.txt_title.color = battleUIPassiveSet.textColor;
						Color underlayColor = DirectingDataSetter.Instance.OnGrayScale ? (battleUIPassiveSet.textColor * battleUIPassiveSet.textColor.grayscale) : (battleUIPassiveSet.textColor * DirectingDataSetter.Instance.graycolor);
						__instance.msetter_title.underlayColor = underlayColor;

						Vector2 sizeDelta2 = __instance.img_Frame.rectTransform.sizeDelta;
						sizeDelta2.y = ((data.Title != "") ? __instance.TitleFrameHeight : __instance.defaultFrameHeight);
						__instance.img_Frame.rectTransform.sizeDelta = sizeDelta2;
					}
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/CustomEffectUISeterror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}

		//Skeleton
		//[HarmonyPatch(typeof(SkeletonJson), nameof(SkeletonJson.ReadSkeletonData), new Type[] { typeof(TextReader) })]
		//[HarmonyPrefix]
		/*
		static bool SkeletonJson_ReadSkeletonData_Pre(ref SkeletonData __result, TextReader reader, SkeletonJson __instance)
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
					reader.GetType().GetField("_pos", all).SetValue(reader, 0);
					result = true;
				}
			}
			return result;
		}
		static SkeletonData ReadSkeletonData(SkeletonJson __instance, TextReader reader)
		{
			reader.GetType().GetField("_pos", all).SetValue(reader, 0);
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
		*/
		//?
		/*
		static void RegionlessAttachmentLoader_get_EmptyRegion()
		{
			if (Spine.Unity.RegionlessAttachmentLoader.emptyRegion == null)
			{
				AtlasRegion region = new AtlasRegion
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
				Spine.Unity.RegionlessAttachmentLoader.emptyRegion = region;
			}
		}
		*/

		static string GetCardName(LorId cardID)
		{
			return BattleCardDescXmlList.Instance.GetCardName(cardID);
		}
		static void GetArtWorks()
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
		static void GetArtWorks(DirectoryInfo dir)
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

		static string path = string.Empty;

		static string Staticpath;

		static string StoryStaticpath;

		static string Storylocalizepath;

		static string Localizepath;

		static List<Assembly> AssemList;

		static List<string> LoadedAssembly;

		public static Dictionary<string, Type> CustomEffects = new Dictionary<string, Type>();

		public static Dictionary<string, Type> CustomMapManager = new Dictionary<string, Type>();

		public static Dictionary<string, Type> CustomBattleDialogModel = new Dictionary<string, Type>();

		public static Dictionary<string, Type> CustomQuest = new Dictionary<string, Type>();

		static bool CoreDialogsLoaded = false;

		public static Dictionary<string, Type> CustomGiftPassive = new Dictionary<string, Type>();

		public static Dictionary<string, Type> CustomEmotionCardAbility = new Dictionary<string, Type>();

		public static Dictionary<string, int> CoreThumbDic;

		public static Dictionary<BattleCardBehaviourResult, List<EffectTypoData>> CustomEffectTypoData = new Dictionary<BattleCardBehaviourResult, List<EffectTypoData>>();

		public static Dictionary<string, Sprite> ArtWorks = null;

		public static Dictionary<LorId, Sprite> BookThumb;

		public static Dictionary<string, AudioClip> AudioClips = null;

		public static bool IsModStorySelected;

		public static Dictionary<LorId, UIStoryLine> ModEpMatch = new Dictionary<LorId, UIStoryLine>();

		static readonly int ModEpMin = 200;

		static readonly HashSet<string> CheckedCustomSprites = new HashSet<string>();

		public static Dictionary<LorId, ModStroyCG> ModStoryCG = null;

		static readonly HashSet<LorId> CheckedModStoryCG = new HashSet<LorId>();

		public static Dictionary<Assembly, string> ModWorkShopId;

		//static bool IsEditing = false;

		static DiceCardXmlInfo errNullCard = null;

		static readonly List<(string pid, string filename, ModInitializer initializer)> allInitializers = new List<(string, string, ModInitializer)>();

		static bool CoreEmotionCardsLoaded = false;

		static bool CoreQuestsLoaded = false;

		static GiftAppearance _giftAppearance;

		static bool CoreGiftPassivesLoaded = false;

		public static bool IsBasemodDebugMode = true;
	}
}