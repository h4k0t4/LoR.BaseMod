using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HarmonyLib;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace ExtendedLoader
{
	class XLUtilInitializer : ModInitializer
	{
		public override void OnInitializeMod()
		{
			try
			{
				var harmony = new Harmony("Cyaminthe.ExtendedLoader");
				bool hasOldMain = false;
				try
				{
					var main = Assembly.Load("1ExtendedLoader");
					if (main != null && main.GetName().Version <= new Version(1, 1, 7))
					{
						hasOldMain = true;
					}
				}
				catch { }
				if (hasOldMain)
				{
					harmony.PatchAll(typeof(ThumbPatchBasic));
					harmony.PatchAll(typeof(CustomGiftPivotPatch));
					harmony.PatchAll(typeof(DestroyCharactersPatch));
					harmony.PatchAll(typeof(UnitLimitPatch));
					harmony.PatchAll(typeof(CardSkinChangePatch));
					harmony.PatchAll(typeof(FaceFixPatch));
				}
				else
				{
					harmony.PatchAll(Assembly.GetExecutingAssembly());
					LoadCoreSounds();
					FixLocalize(harmony);
					CustomBookUIPatch.IntegrateSearcher();
					SceneManager.sceneLoaded += DoReversePatches;
					XLConfig.Load();
				}
				_ = XLUtilRoot.persistentRoot;
				LoadCoreThumbs();
				LegacyCompatibilityPatchBasic.PrepareLegacy(harmony);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		static void DoReversePatches(Scene scene, LoadSceneMode _)
		{
			if (scene.name == "Stage_Hod_New")
			{
				SceneManager.sceneLoaded -= DoReversePatches;
				var harmony = new Harmony("Cyaminthe.ExtendedLoader.Reverse");
				foreach (var method in typeof(ReversePatches).GetMethods())
				{
					var target = method.GetCustomAttributes<HarmonyPatch>()?.FirstOrDefault();
					if (target != null)
					{
						var targetMethod = AccessTools.Method(target.info.declaringType, target.info.methodName, target.info.argumentTypes);
						if (targetMethod != null)
						{
							var patcher = harmony.CreateReversePatcher(targetMethod, new HarmonyMethod(method));
							if (Harmony.GetPatchInfo(targetMethod) != null)
							{
								patcher.Patch(HarmonyReversePatchType.Snapshot);
							}
							else
							{
								patcher.Patch(HarmonyReversePatchType.Original);
							}
						}
					}
				}
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
			var dic = XLUtilRoot.coreThumbDic;
			var list = BookXmlList.Instance._list;
			foreach (BookXmlInfo info in list)
			{
				if (info.id.IsWorkshop() || info.skinType != "Lor")
				{
					continue;
				}
				foreach (string skinName in info.CharacterSkin)
				{
					if (!dic.ContainsKey(skinName) && Resources.Load<Sprite>("Sprites/Books/Thumb/" + info._id) != null)
					{
						dic.Add(skinName, info._id);
					}
				}
			}
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
	}
}
