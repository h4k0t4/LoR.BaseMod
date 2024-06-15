using HarmonyLib;
using LorIdExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UI;
using UnityEngine;
using Workshop;
using static HarmonyLib.AccessTools;
using static System.Reflection.Emit.OpCodes;

namespace ExtendedLoader
{
	[HarmonyPatch]
	internal class CustomSkinCreatePatch
	{
		static bool IsBrokenSkin(UnitDataModel unit, string skinName, CharacterAppearance appearance)
		{
			if (appearance == null)
			{
				Debug.Log("XL: Failed to create CharacterAppearance! Trying to recreate...");
				return true;
			}
			else
			{
				var setter = appearance.GetComponent<WorkshopSkinDataSetter>();
				if (setter == null)
				{
					return false;
				}
				var appliedSkinData = appearance.GetComponent<WorkshopSkinDataCacher>()?.data;
				if (appliedSkinData == null)
				{
					Debug.Log("XL: Found WorkshopSetter, but no data in WorkshopCacher! Trying to recreate...");
					return true;
				}
				WorkshopSkinData baseSkinData = null;
				WorkshopSkinData baseSkinDataAlt = null;
				WorkshopSkinData upgradeSkinData = null;
				if (skinName == null)
				{
					if (unit.workshopSkin == null && unit.CustomBookItem.ClassInfo.skinType == "Custom" && (unit.CustomBookItem.IsWorkshop || CustomizingBookSkinLoader.Instance.GetWorkshopBookSkinData("") != null))
					{
						upgradeSkinData = SkinTools.GetWorkshopBookSkinData(unit.CustomBookItem.BookId.packageId, unit.CustomBookItem.GetCharacterName(), "_" + unit.appearanceType);
						if (unit.CustomBookItem.ClassInfo.skinType == "Custom" || unit.CustomBookItem.IsWorkshop)
						{
							baseSkinData = CustomizingBookSkinLoader.Instance.GetWorkshopBookSkinData(unit.CustomBookItem.BookId.packageId, unit.CustomBookItem.GetCharacterName());
						}
						if (unit._CustomBookItem != unit.bookItem && (unit.bookItem.ClassInfo.skinType == "Custom" || unit.bookItem.IsWorkshop))
						{
							baseSkinDataAlt = CustomizingBookSkinLoader.Instance.GetWorkshopBookSkinData(unit.bookItem.BookId.packageId, unit.bookItem.GetCharacterName());
						}
					}
				}
				else
				{
					upgradeSkinData = SkinTools.GetWorkshopBookSkinData(new LorName(unit.bookItem.BookId.packageId, skinName), "");
					if (unit.workshopSkin == null)
					{
						if (unit.CustomBookItem.ClassInfo.skinType == "Custom" || unit.CustomBookItem.IsWorkshop)
						{
							baseSkinData = CustomizingBookSkinLoader.Instance.GetWorkshopBookSkinData(unit.CustomBookItem.BookId.packageId, unit.CustomBookItem.GetCharacterName());
						}
						if (unit._CustomBookItem != unit.bookItem && (unit.bookItem.ClassInfo.skinType == "Custom" || unit.bookItem.IsWorkshop))
						{
							baseSkinDataAlt = CustomizingBookSkinLoader.Instance.GetWorkshopBookSkinData(unit.bookItem.BookId.packageId, unit.bookItem.GetCharacterName());
						}
					}
					else
					{
						baseSkinData = CustomizingResourceLoader.Instance.GetWorkshopSkinData(unit.workshopSkin);
					}
				}
				if (appliedSkinData != upgradeSkinData && upgradeSkinData != null)
				{
					if (appliedSkinData == baseSkinData || appliedSkinData == baseSkinDataAlt)
					{
						Debug.Log($"XL: Found cached data for {appliedSkinData.dataName} {appliedSkinData.contentFolderIdx} in WorkshopCacher, but also a possible upgrade to {upgradeSkinData.dataName} {upgradeSkinData.contentFolderIdx} (changing to {(skinName ?? "null")}); trying to recreate...");
						return true;
					}
				}
				return false;
			}
		}

		//catch extra index errors
		[HarmonyPatch(typeof(UICharacterRenderer), nameof(UICharacterRenderer.SetCharacter))]
		[HarmonyPrefix]
		[HarmonyPriority(Priority.HigherThanNormal)]
		static void UICharacterRenderer_SetCharacter_Prefix(UICharacterRenderer __instance, int index, ref UnitDataModel __state)
		{
			if (index > 10 && index < __instance.characterList.Count && SkipCustomizationIndex())
			{
				__state = __instance.characterList[index].unitModel;
			}
		}

		[HarmonyPatch(typeof(UICharacterRenderer), nameof(UICharacterRenderer.SetCharacter))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> UICharacterRenderer_SetCharacter_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilgen)
		{
			var bookGetter = PropertyGetter(typeof(UnitDataModel), nameof(UnitDataModel.bookItem));
			var customBookGetter = PropertyGetter(typeof(UnitDataModel), nameof(UnitDataModel.CustomBookItem));
			var setDataMethod = Method(typeof(WorkshopSkinDataSetter), nameof(WorkshopSkinDataSetter.SetData), new Type[] { typeof(WorkshopSkinData) });
			var customPrefabGetter = PropertyGetter(typeof(XLRoot), nameof(XLRoot.UICustomAppearancePrefab));
			var unitGenderField = Field(typeof(UnitDataModel), nameof(UnitDataModel.gender));
			var fixGenderMethods = new MethodInfo[] { Method(typeof(CustomSkinCreatePatch), nameof(TryInjectCreatureGender)), Method(typeof(CustomSkinCreatePatch), nameof(TryInjectEgoGender)) };
			var isWorkshopGetter = PropertyGetter(typeof(BookModel), nameof(BookModel.IsWorkshop));
			var checkPseudoCoreMethod = Method(typeof(CustomSkinCreatePatch), nameof(TryCheckPseudoCoreSkins));
			bool obtainedFlag = false;
			LocalBuilder local = null;
			var codes = new List<CodeInstruction>(instructions);
			int genderInjectCounter = 0;
			for (var i = 0; i < codes.Count; i++)
			{
				if (codes[i].opcode == Callvirt)
				{
					if ((MethodInfo)codes[i].operand == bookGetter)
					{
						codes[i] = new CodeInstruction(Callvirt, customBookGetter);
					}
					else if ((MethodInfo)codes[i].operand == setDataMethod)
					{
						int j;
						if (!obtainedFlag)
						{
							for (j = i + 1; j < codes.Count; j++)
							{
								if (codes[j].Branches(out Label? _))
								{
									j = codes.Count;
								}
								else
								{
									if (codes[j].IsStloc())
									{
										local = codes[j].operand as LocalBuilder;
										if (local != null && local.LocalType == typeof(bool))
										{
											obtainedFlag = true;
											break;
										}
									}
								}
							}
							if (j == codes.Count)
							{
								Debug.Log("Extended Loader: failed to obtain LateInit flag for SetCharacter");
							}
						}
						else
						{
							codes.InsertRange(i + 1, new CodeInstruction[]
							{
								new CodeInstruction(Ldc_I4_1),
								new CodeInstruction(Stloc_S, local)
							});
							i += 2;
						}
					}
					else if ((MethodInfo)codes[i].operand == isWorkshopGetter)
					{
						codes.Insert(i + 1, new CodeInstruction(Call, checkPseudoCoreMethod));
						i++;
					}
				}
				else if (codes[i].Is(Ldstr, "Prefabs/Characters/[Prefab]Appearance_Custom"))
				{
					codes[i] = new CodeInstruction(Call, customPrefabGetter);
					codes.RemoveAt(i + 1);
				}
				else if (codes[i].Is(Ldfld, unitGenderField))
				{
					if (genderInjectCounter < 2)
					{
						codes.InsertRange(i + 1, new CodeInstruction[]
						{
							new CodeInstruction(Ldloca, 3),
							new CodeInstruction(Call, fixGenderMethods[genderInjectCounter])
						});
						i += 2;
						genderInjectCounter++;
					}
				}
			}
			return codes;
		}
		static Gender TryInjectEgoGender(Gender original, ref string characterName)
		{
			if (characterName.StartsWith("EGO:"))
			{
				characterName = characterName.Substring("EGO:".Length);
				return Gender.EGO;
			}
			return original;
		}
		static Gender TryInjectCreatureGender(Gender original, ref string characterName)
		{
			if (characterName.StartsWith("Creature:"))
			{
				characterName = characterName.Substring("Creature:".Length);
				return Gender.Creature;
			}
			return original;
		}
		static bool TryCheckPseudoCoreSkins(bool isWorkshop)
		{
			return isWorkshop || CustomizingBookSkinLoader.Instance.GetWorkshopBookSkinData("") != null;
		}
		static int UICharacterRenderer_SetCharacter_GetMaxWithoutSkip(UICharacterRenderer renderer)
		{
			int count = renderer.characterList.Count;
			if (count >= 11 && SkipCustomizationIndex())
			{
				count--;
			}
			return count;
		}
		static int UICharacterRenderer_SetCharacter_GetIndexWithSkip(int index)
		{
			if (index >= 10 && SkipCustomizationIndex())
			{
				index++;
			}
			return index;
		}
		static bool SkipCustomizationIndex()
		{
			return Singleton<StageController>.Instance.State == StageController.StageState.Battle && GameSceneManager.Instance.battleScene.gameObject.activeSelf;
		}

		//catch other mods breaking things
		[HarmonyPatch(typeof(UICharacterRenderer), nameof(UICharacterRenderer.SetCharacter))]
		[HarmonyFinalizer]
		static Exception UICharacterRenderer_SetCharacter_Finalizer(Exception __exception, UICharacterRenderer __instance, UnitDataModel __state, UnitDataModel unit, int index, bool forcelyReload, bool renderRealtime)
		{
			if (__exception != null && XLConfig.Instance.logRenderErrors)
			{
				Debug.LogException(__exception);
			}
			int fixedIndex = UICharacterRenderer_SetCharacter_GetIndexWithSkip(index);
			if (fixedIndex >= UICharacterRenderer.Instance.characterList.Count)
			{
				return null;
			}
			var uichar = __instance.characterList[fixedIndex];
			if (IsBrokenSkin(unit, null, uichar.unitAppearance))
			{
				try
				{
					ReversePatches.UICharacterRenderer_SetCharacter_Snapshot(__instance, unit, index, true, renderRealtime);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}
			if (__state != null)
			{
				try
				{
					var unfixedUiChar = __instance.characterList[index];
					if (unfixedUiChar.unitModel != __state || IsBrokenSkin(__state, null, unfixedUiChar.unitAppearance))
					{
						ReversePatches.UICharacterRenderer_SetCharacter_Snapshot(__instance, __state, index - 1, true, renderRealtime);
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}
			return null;
		}

		//CreateSkin
		[HarmonyPatch(typeof(SdCharacterUtil), nameof(SdCharacterUtil.CreateSkin))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> SdCharacterUtil_CreateSkin_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilgen)
		{
			var bookGetter = PropertyGetter(typeof(UnitDataModel), nameof(UnitDataModel.bookItem));
			var customBookGetter = PropertyGetter(typeof(UnitDataModel), nameof(UnitDataModel.CustomBookItem));
			var setDataMethod = Method(typeof(WorkshopSkinDataSetter), nameof(WorkshopSkinDataSetter.SetData), new Type[] { typeof(WorkshopSkinData) });
			var isWorkshopGetter = PropertyGetter(typeof(BookModel), nameof(BookModel.IsWorkshop));
			var checkPseudoCoreMethod = Method(typeof(CustomSkinCreatePatch), nameof(TryCheckPseudoCoreSkins));
			var checkCustomMethod = Method(typeof(CustomSkinCreatePatch), nameof(CheckSkinCustomType));
			bool obtainedFlag = false;
			LocalBuilder local = null;
			var codes = new List<CodeInstruction>(instructions);
			for (var i = 0; i < codes.Count; i++)
			{
				if (codes[i].Calls(isWorkshopGetter))
				{
					var bookLocal = ilgen.DeclareLocal(typeof(BookModel));
					codes.InsertRange(i, new CodeInstruction[]
					{
						new CodeInstruction(Dup),
						new CodeInstruction(Stloc, bookLocal)
					});
					codes.InsertRange(i + 3, new CodeInstruction[]
					{
						new CodeInstruction(Call, checkPseudoCoreMethod),
						new CodeInstruction(Ldloc, bookLocal),
						new CodeInstruction(Call, checkCustomMethod)
					});
					i += 5;
				}
				else if (codes[i].Calls(bookGetter))
				{
					codes[i] = new CodeInstruction(Callvirt, customBookGetter);
				}
				else if (codes[i].Calls(setDataMethod))
				{
					if (!obtainedFlag)
					{
						int j;
						for (j = i + 1; j < codes.Count; j++)
						{
							if (codes[j].Branches(out Label? _))
							{
								j = codes.Count;
							}
							else
							{
								if (codes[j].IsStloc())
								{
									local = codes[j].operand as LocalBuilder;
									if (local != null && local.LocalType == typeof(bool))
									{
										obtainedFlag = true;
										break;
									}
								}
							}
						}
						if (j == codes.Count)
						{
							Debug.Log("Extended Loader: Failed to obtain LateInit flag for CreateSkin");
						}
					}
					else
					{
						codes.InsertRange(i + 1, new CodeInstruction[]
						{
							new CodeInstruction(Ldc_I4_1),
							new CodeInstruction(Stloc_S, local)
						});
						i += 2;
					}
				}
			}
			return codes;
		}
		static bool CheckSkinCustomType(bool isWorkshop, BookModel book)
		{
			return isWorkshop && book.ClassInfo.skinType == "Custom";
		}

		//catch other mods breaking things
		[HarmonyPatch(typeof(SdCharacterUtil), nameof(SdCharacterUtil.CreateSkin))]
		[HarmonyFinalizer]
		static Exception SdCharacterUtil_CreateSkin_Finalizer(Exception __exception, ref CharacterAppearance __result, UnitDataModel unit, Faction faction, Transform characterRoot)
		{
			if (__exception != null && XLConfig.Instance.logRenderErrors)
			{
				Debug.LogException(__exception);
			}
			try
			{
				if (IsBrokenSkin(unit, null, __result))
				{
					var oldres = __result;
					__result = ReversePatches.SdCharacterUtil_CreateSkin_Snapshot(unit, faction, characterRoot);
					if (IsBrokenSkin(unit, null, __result))
					{
						if (__result != null)
						{
							UnityEngine.Object.Destroy(__result.gameObject);
						}
						__result = oldres;
					}
					else
					{
						if (oldres != null)
						{
							UnityEngine.Object.Destroy(oldres.gameObject);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			return null;
		}

		//ChangeSkin
		[HarmonyPatch(typeof(BattleUnitView), nameof(BattleUnitView.ChangeSkin))]
		[HarmonyTranspiler]
		[HarmonyPriority(Priority.Low)]
		static IEnumerable<CodeInstruction> BattleUnitView_ChangeSkin_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var componentMethod = Method(typeof(GameObject), nameof(GameObject.GetComponent), Array.Empty<Type>(), new Type[] { typeof(CharacterAppearance) });
			var codes = instructions.ToList();
			var index = codes.FindIndex(code => code.Calls(componentMethod));
			if (index >= 0)
			{
				codes.InsertRange(index, new CodeInstruction[]
				{
					new CodeInstruction(Dup),
					new CodeInstruction(Ldarg_0),
					new CodeInstruction(Ldarg_1),
					new CodeInstruction(Call, Method(typeof(CustomSkinCreatePatch), nameof(TryApplyWorkshopData)))
				});
			}
			return codes;
		}
		static void TryApplyWorkshopData(GameObject appearance, BattleUnitView view, string charName)
		{
			var setter = appearance.GetComponent<WorkshopSkinDataSetter>();
			if (setter != null && (setter.dic == null || setter.dic.Count == 0))
			{
				var data = SkinTools.GetWorkshopBookSkinData(view.model.Book.BookId.packageId, charName, "");
				if (data != null)
				{
					setter.SetData(data);
				}
			}
		}

		//catch other mods breaking things
		[HarmonyPatch(typeof(BattleUnitView), nameof(BattleUnitView.ChangeSkin))]
		[HarmonyFinalizer]
		static Exception BattleUnitView_ChangeSkin_Finalizer(Exception __exception, BattleUnitView __instance, string charName)
		{
			if (__exception != null && XLConfig.Instance.logRenderErrors)
			{
				Debug.LogException(__exception);
			}
			try
			{
				if (IsBrokenSkin(__instance.model.UnitData.unitData, charName, __instance.charAppearance))
				{
					ReversePatches.BattleUnitView_ChangeSkin_Snapshot(__instance, charName);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			try
			{
				if (IsBrokenSkin(__instance.model.UnitData.unitData, charName, __instance.charAppearance) && LorName.IsCompressed(charName))
				{
					ReversePatches.BattleUnitView_ChangeSkin_Snapshot(__instance, new LorName(charName).name);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			return null;
		}

		//ChangeEgoSkin
		[HarmonyPatch(typeof(BattleUnitView), nameof(BattleUnitView.ChangeEgoSkin))]
		[HarmonyTranspiler]
		[HarmonyPriority(Priority.Low)]
		static IEnumerable<CodeInstruction> BattleUnitView_ChangeEgoSkin_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var componentMethod = Method(typeof(GameObject), nameof(GameObject.GetComponent), Array.Empty<Type>(), new Type[] { typeof(CharacterAppearance) });
			var codes = instructions.ToList();
			var index = codes.FindIndex(code => code.Calls(componentMethod));
			if (index >= 0)
			{
				codes.InsertRange(index, new CodeInstruction[]
				{
					new CodeInstruction(Dup),
					new CodeInstruction(Ldarg_0),
					new CodeInstruction(Ldarg_1),
					new CodeInstruction(Call, Method(typeof(CustomSkinCreatePatch), nameof(TryApplyWorkshopData)))
				});
			}
			return codes;
		}

		//catch other mods breaking things
		[HarmonyPatch(typeof(BattleUnitView), nameof(BattleUnitView.ChangeEgoSkin))]
		[HarmonyFinalizer]
		static Exception BattleUnitView_ChangeEgoSkin_Finalizer(Exception __exception, BattleUnitView __instance, string egoName, bool bookNameChange)
		{
			if (__exception != null && XLConfig.Instance.logRenderErrors)
			{
				Debug.LogException(__exception);
			}
			try
			{
				if (IsBrokenSkin(__instance.model.UnitData.unitData, egoName, __instance.charAppearance))
				{
					ReversePatches.BattleUnitView_ChangeEgoSkin_Snapshot(__instance, egoName, bookNameChange);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			try
			{
				if (IsBrokenSkin(__instance.model.UnitData.unitData, egoName, __instance.charAppearance) && LorName.IsCompressed(egoName))
				{
					ReversePatches.BattleUnitView_ChangeEgoSkin_Snapshot(__instance, new LorName(egoName).name, bookNameChange);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			return null;
		}

		[HarmonyPatch(typeof(WorkshopSkinDataSetter), nameof(WorkshopSkinDataSetter.LateInit))]
		[HarmonyFinalizer]
		static Exception WorkshopSkinDataSetter_LateInit_Finalizer(Exception __exception)
		{
			if (__exception is NullReferenceException)
			{
				return null;
			}
			return __exception;
		}

		[HarmonyPatch(typeof(SdCharacterUtil), nameof(SdCharacterUtil.LoadAppearance))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> SdCharacterUtil_LoadAppearance_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var bookGetter = PropertyGetter(typeof(UnitDataModel), nameof(UnitDataModel.bookItem));
			var customBookGetter = PropertyGetter(typeof(UnitDataModel), nameof(UnitDataModel.CustomBookItem));
			var unitGenderField = Field(typeof(UnitDataModel), nameof(UnitDataModel.gender));
			var fixGenderMethod = Method(typeof(CustomSkinCreatePatch), nameof(TryInjectAbnormalGender));
			var customPrefabGetter = PropertyGetter(typeof(XLRoot), nameof(XLRoot.CustomAppearancePrefab));
			var codes = new List<CodeInstruction>(instructions);
			for (var i = 0; i < codes.Count; i++)
			{
				if (codes[i].Calls(bookGetter))
				{
					yield return new CodeInstruction(Callvirt, customBookGetter);
				}
				else if (codes[i].Is(Ldstr, "Prefabs/Characters/[Prefab]Appearance_Custom"))
				{
					yield return new CodeInstruction(Call, customPrefabGetter);
					i++;
				}
				else if (codes[i].Is(Ldfld, unitGenderField))
				{
					yield return codes[i];
					yield return new CodeInstruction(Ldloca, 1);
					yield return new CodeInstruction(Call, fixGenderMethod);
				}
				else
				{
					yield return codes[i];
				}
			}
		}
		static Gender TryInjectAbnormalGender(Gender original, ref string characterName)
		{
			if (characterName.StartsWith("EGO:"))
			{
				characterName = characterName.Substring("EGO:".Length);
				return Gender.EGO;
			}
			if (characterName.StartsWith("Creature:"))
			{
				characterName = characterName.Substring("Creature:".Length);
				return Gender.Creature;
			}
			return original;
		}

		[HarmonyPatch(typeof(AssetBundleManagerRemake), nameof(AssetBundleManagerRemake.LoadCharacterPrefab_DefaultMotion))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> AssetBundleManagerRemake_LoadCharacterPrefab_DefaultMotion_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = instructions.ToList();
			for (var i = 0; i < codes.Count; i++)
			{
				if (codes[i].Is(Ldstr, "Prefabs/Characters/[Prefab]Appearance_Custom"))
				{
					yield return new CodeInstruction(Call, PropertyGetter(typeof(XLRoot), nameof(XLRoot.UICustomAppearancePrefab)));
					i++;
				}
				else
				{
					yield return codes[i];
				}
			}
		}

		[HarmonyPatch(typeof(AssetBundleManagerRemake), nameof(AssetBundleManagerRemake.LoadCharacterPrefab))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> AssetBundleManagerRemake_LoadCharacterPrefab_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = instructions.ToList();
			for (var i = 0; i < codes.Count; i++)
			{
				if (codes[i].Is(Ldstr, "Prefabs/Characters/[Prefab]Appearance_Custom"))
				{
					yield return new CodeInstruction(Call, PropertyGetter(typeof(XLRoot), nameof(XLRoot.CustomAppearancePrefab)));
					i++;
				}
				else
				{
					yield return codes[i];
				}
			}
		}

		[HarmonyPatch(typeof(AssetBundleManagerRemake), nameof(AssetBundleManagerRemake.LoadSdPrefab))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> AssetBundleManagerRemake_LoadSdPrefab_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = instructions.ToList();
			for (var i = 0; i < codes.Count; i++)
			{
				if (codes[i].Is(Ldstr, "Prefabs/Characters/[Prefab]Appearance_Custom"))
				{
					yield return new CodeInstruction(Call, PropertyGetter(typeof(XLRoot), nameof(XLRoot.CustomAppearancePrefab)));
					i++;
				}
				else
				{
					yield return codes[i];
				}
			}
		}
	}

	class WorkshopSkinDataCacher : MonoBehaviour
	{
		internal WorkshopSkinData data = null;
	}
}
