using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using System.Reflection;
using System.IO;
using Workshop;
using UnityEngine.SceneManagement;
using Mod;

namespace ExtendedLoader
{
	public class XLInitializer : ModInitializer
	{
		public XLInitializer()
		{
			try
			{
				new Harmony("Cyaminthe.ExtendedLoader.HighPriority").PatchAll(typeof(EarlyPatches));
				XLRoot.LoadModFolders();
				ReLoadWorkshopCustomAppearance();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public override void OnInitializeMod()
		{
			try
			{
				RaisePriority();
				var harmony = new Harmony("Cyaminthe.ExtendedLoader");
				harmony.PatchAll(Assembly.GetExecutingAssembly());
				LoadCoreThumbs();
				LoadCoreSounds();
				FixLocalize(harmony);
				SceneManager.sceneLoaded += DoReversePatches;
				XLRoot.EnsureInit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		static void RaisePriority()
		{
			List<ModContentInfo> originList = ModContentManager.Instance._allMods;
			List<ModContentInfo> uexList = new List<ModContentInfo>();
			List<ModContentInfo> xlList = new List<ModContentInfo>();
			List<ModContentInfo> bmList = new List<ModContentInfo>();
			originList.RemoveAll(modContentInfo =>
			{
				switch (modContentInfo.invInfo.workshopInfo.uniqueId)
				{
					case "BaseMod":
						if (modContentInfo.activated)
						{
							bmList.Insert(0, modContentInfo);
						}
						else
						{
							bmList.Add(modContentInfo);
						}
						return true;
					case "UnityExplorer":
					case "HarmonyLoadOrderFix":
						if (modContentInfo.activated)
						{
							uexList.Insert(0, modContentInfo);
						}
						else
						{
							uexList.Add(modContentInfo);
						}
						return true;
					case "ExtendedLoaderStandalone":
						if (modContentInfo.activated)
						{
							xlList.Insert(0, modContentInfo);
						}
						else
						{
							xlList.Add(modContentInfo);
						}
						return true;
					default: return false;
				}
			});
			uexList.AddRange(xlList);
			uexList.AddRange(bmList);
			originList.InsertRange(0, uexList);
		}

		static void DoReversePatches(Scene scene, LoadSceneMode _)
		{
			if (scene.name == "Stage_Hod_New")
			{
				SceneManager.sceneLoaded -= DoReversePatches;
				new Harmony("Cyaminthe.ExtendedLoader.Reverse").PatchAll(typeof(ReversePatches));
			}
		}

		static void FixLocalize(Harmony harmony)
		{
			bool locCompat = false;
			try
			{
				var addOnLocalize = (from a in AppDomain.CurrentDomain.GetAssemblies()
									 where a.GetName().Name == "LoRLocalizationManager"
									 select a into v
									 orderby v.GetName().Version descending
									 select v).FirstOrDefault()?.GetType("LoRLocalizationManager.LocalizationUtil")?.GetMethod("AddOnLocalizeAction");
				if (addOnLocalize != null)
				{
					addOnLocalize.Invoke(null, new object[] { new Action<string>(x => CustomBookLabelPatch.SetCustomBookLabel()) });
					locCompat = true;
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			if (!locCompat)
			{
				try
				{
					harmony.PatchAll(typeof(CustomBookLabelPatch));
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}
		}

		static class CustomBookLabelPatch
		{
			[HarmonyPatch(typeof(TextDataModel), nameof(TextDataModel.InitTextData))]
			[HarmonyPostfix]
			[HarmonyPriority(Priority.Low)]
			internal static void SetCustomBookLabel()
			{
				TextDataModel.textDic["ui_customcorebook_custommodtoggle"] = $"{TextDataModel.textDic.GetValueSafe("ui_corepage") ?? "Key Page"} ({TextDataModel.textDic.GetValueSafe("ui_invitation_customtoggle") ?? "Workshop"})";
			}
		}

		static void LoadCoreThumbs()
		{
			var dic = new Dictionary<string, int>();
			var list = BookXmlList.Instance._list;
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
			XLRoot.CoreThumbDic = dic;
		}
		static void LoadCoreSounds()
		{
			if (CharacterSound._motionSoundResources == null)
			{
				CharacterSound._motionSoundResources = new Dictionary<string, AudioClip>();
			}
			foreach (AudioClip audioClip in Resources.LoadAll<AudioClip>("Sounds/MotionSound"))
			{
				if (!CharacterSound._motionSoundResources.ContainsKey(audioClip.name))
				{
					CharacterSound._motionSoundResources.Add(audioClip.name, audioClip);
				}
			}
		}

		static void ReLoadWorkshopCustomAppearance()
		{
			SetOriginalIndexes();
			ReloadExternalData();
			LoadWorkshopCustomAppearanceFolder(PlatformManager.Instance.GetWorkshopDirPath());
			ResetIndexes();
			LoadWorkshopCustomAppearanceFolder(Path.Combine(Application.dataPath, "Mods"));
		}
		static void LoadWorkshopCustomAppearanceFolder(string path)
		{
			if (Directory.Exists(path))
			{
				var allSkinData = CustomizingResourceLoader.Instance._skinData;
				foreach (string text in Directory.GetDirectories(path))
				{
					try
					{
						var info = WorkshopAppearanceItemLoader.LoadCustomAppearance(text);
						if (info != null)
						{
							string[] array = text.Split(new char[]
							{
							'\\'
							});
							string text2 = array[array.Length - 1];
							info.path = text;
							info.uniqueId = text2;
							if (string.IsNullOrWhiteSpace(info.bookName))
							{
								info.bookName = text2;
							}
							if (info.faceCustomInfo != null && info.faceCustomInfo.Count > 0 && FaceData.GetExtraData(info.faceCustomInfo) == null)
							{
								XLRoot.LoadFaceCustom(info.faceCustomInfo, $"skin_{text2}");
							}
							if (info.isClothCustom)
							{
								var data = new WorkshopSkinData
								{
									dic = info.clothCustomInfo,
									dataName = info.bookName,
									contentFolderIdx = info.uniqueId
								};
								if (allSkinData.TryGetValue(data.contentFolderIdx, out var oldData))
								{
									data.id = oldData.id;
								}
								else
								{
									data.id = allSkinData.Count;
								}
								allSkinData[data.contentFolderIdx] = data;
							}
						}
					}
					catch (Exception ex)
					{
						Debug.LogError("Extended Loader: error loading skin at " + text);
						Debug.LogException(ex);
					}
				}
			}
		}
		static void SetOriginalIndexes()
		{
			var loaderDir = CustomizingResourceLoader.Instance.InternalCustomDir;
			originalEyeIndex = GetOriginalIndex(loaderDir, "/Eyes_Normal", "/Eyes_Side_Normal");
			originalBrowIndex = GetOriginalIndex(loaderDir, "/Brows_Normal", "/Brows_Attack", "/Brows_Side_Attack", "/Brows_Hit");
			originalMouthIndex = GetOriginalIndex(loaderDir, "/Mouths_Normal", "/Mouths_Attack", "/Mouths_Side_Attack", "/Mouths_Hit");
			originalFrontHairIndex = GetOriginalIndex(loaderDir, "/FrontHair", "/FrontHair_Side");
			originalRearHairIndex = GetOriginalIndex(loaderDir, "/RearHair", "/RearHair_Side", "/RearHair_Side_FrontLayer");
		}
		static void ResetIndexes()
		{
			originalBrowIndex = originalEyeIndex = originalFrontHairIndex = originalMouthIndex = originalRearHairIndex = -1;
		}
		static int GetOriginalIndex(string baseFolder, params string[] subFolders)
		{
			int max = 0;
			foreach (string subFolder in subFolders)
			{
				max = Mathf.Max(max, Resources.LoadAll<Sprite>(baseFolder + subFolder).Length);
			}
			return max;
		}
		static void ReloadExternalData()
		{
			var loader = CustomizingResourceLoader.Instance;
			ReloadExternalFaceSets(loader.ExternalEyeDir, CustomizeType.Eye, ref originalEyeIndex, loader._eyeResources);
			ReloadExternalFaceSets(loader.ExternalBrowDir, CustomizeType.Brow, ref originalBrowIndex, loader._browResources);
			ReloadExternalFaceSets(loader.ExternalMouthDir, CustomizeType.Mouth, ref originalMouthIndex, loader._mouthResources);
			ReloadExternalHairSets(loader.ExternalFrontHairDir, CustomizeType.FrontHair, ref originalFrontHairIndex, loader._frontHairResources);
			ReloadExternalHairSets(loader.ExternalRearHairDir, CustomizeType.RearHair, ref originalRearHairIndex, loader._rearHairResources);
		}
		static void ReloadExternalFaceSets(string dirPath, CustomizeType type, ref int index, List<FaceResourceSet> resList)
		{
			var loader = CustomizingResourceLoader.Instance;
			string[] targetName = new string[]
			{
				loader.ExternalFaceNormalFileName,
				loader.ExternalFaceAttackFileName,
				loader.ExternalFaceHitFileName,
				loader.ExternalFaceSideAttackFileName
			};
			DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
			if (!directoryInfo.Exists)
			{
				directoryInfo.Create();
			}
			foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
			{
				FaceResourceSet face = new FaceResourceSet();
				Dictionary<string, Sprite> externalSpriteSet = GetExternalSpriteSet(dir, targetName);
				if (externalSpriteSet.ContainsKey(loader.ExternalFaceNormalFileName))
				{
					face.normal = externalSpriteSet[loader.ExternalFaceNormalFileName];
				}
				if (externalSpriteSet.ContainsKey(loader.ExternalFaceAttackFileName))
				{
					face.atk = externalSpriteSet[loader.ExternalFaceAttackFileName];
				}
				if (externalSpriteSet.ContainsKey(loader.ExternalFaceHitFileName))
				{
					face.hit = externalSpriteSet[loader.ExternalFaceHitFileName];
				}
				if (externalSpriteSet.ContainsKey(loader.ExternalFaceSideAttackFileName))
				{
					face.atk_side = externalSpriteSet[loader.ExternalFaceSideAttackFileName];
				}
				if (face.FillSprite())
				{
					if (index >= 0 && index < resList.Count)
					{
						resList[index] = face;
						index++;
					}
					else
					{
						resList.Add(face);
						index = -1;
					}
					XLRoot.TrySetIndex(null, $"external_{dir.Name}", type, index, resList);
				}
			}
		}

		static void ReloadExternalHairSets(string dirPath, CustomizeType type, ref int index, List<HairResourceSet> resList)
		{
			bool rear = type == CustomizeType.RearHair;
			var loader = CustomizingResourceLoader.Instance;
			string[] targetName = new string[]
			{
				loader.ExternalHairDefaultFileName,
				loader.ExternalFrontHairSideFileName,
				loader.ExternalRearHairSideFileName,
				loader.ExternalRearHairSideBackFileName
			};
			DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
			if (!directoryInfo.Exists)
			{
				directoryInfo.Create();
			}
			foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
			{
				HairResourceSet hair = new HairResourceSet();
				Dictionary<string, Sprite> externalSpriteSet = GetExternalSpriteSet(dir, targetName);
				if (externalSpriteSet.ContainsKey(loader.ExternalHairDefaultFileName))
				{
					hair.Default = externalSpriteSet[loader.ExternalHairDefaultFileName];
				}
				if (rear)
				{
					if (externalSpriteSet.ContainsKey(loader.ExternalRearHairSideFileName))
					{
						hair.Side_Front = externalSpriteSet[loader.ExternalRearHairSideFileName];
					}
					if (externalSpriteSet.ContainsKey(loader.ExternalRearHairSideBackFileName))
					{
						hair.Side_Back = externalSpriteSet[loader.ExternalRearHairSideBackFileName];
					}
				}
				else if (externalSpriteSet.ContainsKey(loader.ExternalFrontHairSideFileName))
				{
					hair.Side_Front = externalSpriteSet[loader.ExternalFrontHairSideFileName];
				}
				if (hair.Default != null)
				{
					if (index >= 0 && index < resList.Count)
					{
						resList[index] = hair;
						index++;
					}
					else
					{
						resList.Add(hair);
						index = -1;
					}
					XLRoot.TrySetIndex(null, $"external_{dir.Name}", type, index, resList);
				}
			}
		}

		static Dictionary<string, Sprite> GetExternalSpriteSet(DirectoryInfo dir, string[] targetName)
		{
			Dictionary<string, Sprite> dictionary = new Dictionary<string, Sprite>();
			Vector2 pivot = new Vector2(0.5f, 0.5f);
			foreach (System.IO.FileInfo fileInfo in dir.GetFiles())
			{
				string text = Path.GetFileNameWithoutExtension(fileInfo.Name).ToLower();
				if (targetName.Contains(text))
				{
					try
					{
						Sprite sprite = SpriteUtil.LoadSprite(fileInfo.FullName, pivot);
						if (sprite != null && !dictionary.ContainsKey(text))
						{
							dictionary.Add(text, sprite);
						}
					}
					catch (Exception)
					{
						Debug.LogError("invalid image file " + fileInfo.FullName);
					}
				}
			}
			return dictionary;
		}

		internal static int originalEyeIndex = -1;
		internal static int originalBrowIndex = -1;
		internal static int originalMouthIndex = -1;
		internal static int originalFrontHairIndex = -1;
		internal static int originalRearHairIndex = -1;
	}
}
