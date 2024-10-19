using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using UI;
using UnityEngine.Events;
using UnityEngine;
using static HarmonyLib.AccessTools;
using static System.Reflection.Emit.OpCodes;
using Object = UnityEngine.Object;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ExtendedLoader
{
	[HarmonyPatch]
	class CustomBookUIPatch
	{
		[HarmonyPatch(typeof(UnitDataModel), nameof(UnitDataModel.EquipBook))]
		[HarmonyPostfix]
		static void UnitDataModel_EquipBook_Postfix(UnitDataModel __instance)
		{
			if (__instance._CustomBookItem != null)
			{
				if (__instance.bookItem.ClassInfo.RangeType == EquipRangeType.Hybrid && __instance._CustomBookItem.ClassInfo.RangeType != EquipRangeType.Hybrid)
				{
					__instance._CustomBookItem = null;
				}
			}
		}

		[HarmonyPatch(typeof(UICustomizeProfile), nameof(UICustomizeProfile.UpdatePortrait))]
		[HarmonyPostfix]
		static void UICustomizeProfile_UpdatePortrait_Postfix(UICustomizeProfile __instance)
		{
			var uichara = UICharacterRenderer.Instance.GetUICharacterByIndex(10);
			if (uichara.unitAppearance && !uichara.unitAppearance._customizable)
			{
				CustomizedAppearance head = null;
				try
				{
					head = CustomizingResourceLoader.Instance.CreateCustomizedAppearance(uichara.unitModel.customizeData, __instance.transform);
					if (head is SpecialCustomizedAppearance specialHead)
					{
						uichara.unitModel.customizeData.skinColor = specialHead.GetSkinColor();
						uichara.unitModel.customizeData.height = specialHead.height;
						__instance.rimg_Portrait.texture = UICharacterRenderer.Instance.GetRenderTextureByIndexAndSize(10);
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
				if (head)
				{
					GameObject.Destroy(head.gameObject);
				}
			}
		}

		[HarmonyPatch(typeof(UICustomizeClothsPanel), nameof(UICustomizeClothsPanel.InitData))]
		[HarmonyPostfix]
		static void UICustomizeClothsPanel_InitData_Postfix(UICustomizeClothsPanel __instance)
		{
			try
			{
				isModWorkshopSkin = false;
				if (clothsPanel == null)
				{
					clothsPanel = __instance;
					var workshopButton = __instance.workshopButton.transform;
					var parent = workshopButton.parent as RectTransform;
					var offMax = parent.offsetMax;
					if (offMax.y < -315f)
					{
						offMax.y = -315f;
						parent.offsetMax = offMax;
					}
					var textLoader = workshopButton.GetComponentInChildren<UITextDataLoader>();
					textLoader.key = "ui_invitation_customtoggle";
					workshopButton = Object.Instantiate(workshopButton, parent);
					workshopBookButton = workshopButton.gameObject;
					textLoader = workshopButton.GetComponentInChildren<UITextDataLoader>();
					textLoader.key = "ui_customcorebook_custommodtoggle";
					var selectable = workshopButton.GetComponent<UICustomSelectable>();
					var submitEvent = selectable.SubmitEvent;
					submitEvent.RemoveAllListeners();
					submitEvent.m_PersistentCalls.Clear();
					submitEvent.DirtyPersistentCalls();
					submitEvent.AddCall(new InvokableCall(OnClickWorkshopBook));
					workshopBookButton = workshopButton.gameObject;
				}
				ReloadCustomProjectionIndex();
				workshopBookButton.SetActive(equippableWorkshopBookCount > 0);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		internal static void OnClickWorkshopBook()
		{
			UISoundManager.instance.PlayEffectSound(UISoundType.Ui_Click);
			if (isModWorkshopSkin)
			{
				clothsPanel.EquipPageCustomizePanel.Init(clothsPanel.equipBookInfoList, false);
				clothsPanel.EquipPageCustomizePanel.UpdateList();
			}
			else
			{
				clothsPanel.EquipPageCustomizePanel.Init(modWorkshopBookList, false);
				clothsPanel.EquipPageCustomizePanel.UpdateList();
			}
		}

		internal static List<int> ReloadCustomProjectionIndex()
		{
			modWorkshopBookIndex = new Dictionary<int, LorId>();
			modWorkshopBookList = new List<int>();
			equippableWorkshopBookCount = 0;

			var unequippable = new List<int>();
			int index = 0;
			var userRange = UICustomizePopup.Instance.SelectedUnit.bookItem.ClassInfo.RangeType;

			Predicate<EquipRangeType> check = x => true;
			switch (userRange)
			{
				case EquipRangeType.Melee:
					check = x => x != EquipRangeType.Range;
					break;
				case EquipRangeType.Range:
					check = x => x != EquipRangeType.Melee;
					break;
				case EquipRangeType.Hybrid:
					check = x => x == EquipRangeType.Hybrid;
					break;
			}

			foreach (LorId lorId in BookInventoryModel.Instance.GetIdList_noDuplicate().OrderByDescending(id => id.packageId).ThenBy(id => id.id))
			{
				BookXmlInfo info = BookXmlList.Instance.GetData(lorId);
				if (lorId.IsWorkshop() && !info.isError && !info.canNotEquip)
				{
					modWorkshopBookIndex.Add(index, lorId);
					if (check(info.RangeType))
					{
						modWorkshopBookList.Add(index);
						equippableWorkshopBookCount++;
					}
					else
					{
						unequippable.Add(index);
					}
					index++;
				}
			}
			modWorkshopBookList.AddRange(unequippable);

			return modWorkshopBookList;
		}
		[HarmonyPatch(typeof(UIEquipPageCustomizePanel), nameof(UIEquipPageCustomizePanel.Init))]
		[HarmonyPrefix]
		static void UIEquipPageCustomizePanel_Init_Prefix(List<int> data)
		{
			isModWorkshopSkin = data == modWorkshopBookList;
		}
		/*
		[HarmonyPatch(typeof(UIEquipPageCustomizePanel), nameof(UIEquipPageCustomizePanel.ApplyFilterAll))]
		[HarmonyPrefix]
		[HarmonyAfter("calmmagma_keypagesearcher")]
		static bool UIEquipPageCustomizePanel_ApplyFilterAll_Prefix(UIEquipPageCustomizePanel __instance)
		{
			try
			{
				if (!isModWorkshopSkin)
				{
					if (__instance.isWorkshop)
					{
						__instance.filteredBookIdList.Clear();
						__instance.filteredBookIdList.AddRange(__instance.originBookIdList);
						__instance.filteredBookIdList.Sort((int a, int b) => b.CompareTo(a));
					}
					else
					{
						List<Grade> storyGradeFilter = __instance.gradeFilter.GetStoryGradeFilter();
						__instance.filteredBookIdList.Clear();
						if (storyGradeFilter.Count == 0)
						{
							__instance.filteredBookIdList.AddRange(__instance.originBookIdList);
						}
						else
						{
							foreach (Grade g in storyGradeFilter)
							{
								__instance.filteredBookIdList.AddRange(__instance.originBookIdList.FindAll((int x) => BookXmlList.Instance.GetData(x).Chapter == (int)g));
							}
						}
						Predicate<EquipRangeType> match = x => true;
						switch (__instance.panel.Parent.SelectedUnit.bookItem.ClassInfo.RangeType)
						{
							case EquipRangeType.Melee:
								match = x => x != EquipRangeType.Range;
								break;
							case EquipRangeType.Range:
								match = x => x != EquipRangeType.Melee;
								break;
							case EquipRangeType.Hybrid:
								match = x => x == EquipRangeType.Hybrid;
								break;
						}
						__instance.filteredBookIdList.Sort((int a, int b) => CompareBooks(a, b, match));
					}
					int num = __instance.GetMaxRow();
					__instance.scrollBar.SetScrollRectSize(__instance.column * __instance.slotWidth, (num + __instance.row - 1) * __instance.slotHeight);
					__instance.scrollBar.SetWindowPosition(0f, 0f);
					__instance.ParentSelectable.ChildSelectable = __instance.bookSlotList[0].selectable;
					__instance.UpdateBookListPage(false);
					return false;
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			return true;
		}
		static int CompareBooks(int a, int b, Predicate<EquipRangeType> match)
		{
			var ar = BookXmlList.Instance.GetData(a).RangeType;
			var br = BookXmlList.Instance.GetData(b).RangeType;
			if (match(ar))
			{
				if (!match(br))
				{
					return -1;
				}
			}
			else
			{
				if (match(br))
				{
					return 1;
				}
			}
			return b.CompareTo(a);
		}
		*/


		[HarmonyPatch(typeof(UIEquipPageCustomizePanel), nameof(UIEquipPageCustomizePanel.ApplyFilterAll))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> UIEquipPageCustomizePanel_ApplyFilterAll_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = instructions.ToList();

			var origFilterCode = codes.Find(c => c.opcode == Ldftn
				&& c.operand is MethodInfo info && info.ReturnType == typeof(bool)
				&& info.DeclaringType != null && info.DeclaringType.GetCustomAttributes<CompilerGeneratedAttribute>().Any());
			if (origFilterCode != null)
			{
				if (filterDelegate == null)
				{
					var origContextType = (origFilterCode.operand as MethodInfo).DeclaringType;
					var dynamicFilter = new DynamicMethod("CheckBookGradeForFilter", typeof(bool), new Type[] { typeof(object), typeof(int) }, true);
					ILGenerator ilgen = dynamicFilter.GetILGenerator(256);
					ilgen.Emit(Ldarg_0);
					ilgen.Emit(Castclass, origContextType);
					ilgen.Emit(Ldfld, Field(origContextType, "g"));
					ilgen.Emit(Ldarg_1);
					ilgen.Emit(Call, Method(typeof(CustomBookUIPatch), nameof(CustomBookUIPatch.FilterHelper)));
					ilgen.Emit(Ret);
					filterDelegate = (Func<object, int, bool>)dynamicFilter.CreateDelegate(typeof(Func<object, int, bool>));
				}
				origFilterCode.operand = Method(typeof(CustomBookUIPatch), nameof(CustomBookUIPatch.FilterWrapper));
			}

			var origSorterCode = codes.Find(c => c.opcode == Ldsfld
				&& c.operand is FieldInfo info && info.FieldType == typeof(Comparison<int>)
				&& info.DeclaringType != null && info.DeclaringType.GetCustomAttributes<CompilerGeneratedAttribute>().Any());
			if (origSorterCode != null)
			{
				origSorterCode.operand = Field(typeof(CustomBookUIPatch), nameof(CustomBookUIPatch.bookSorter));
			}

			return codes;
		}

		static Func<object, int, bool> filterDelegate = null;
		static bool FilterWrapper(object context, int bookId)
		{
			return filterDelegate.Invoke(context, bookId);
		}
		static bool FilterHelper(Grade g, int bookId)
		{
			return BookXmlList.Instance.GetData(bookId).Chapter == (int)g;
		}

		static readonly Comparison<int> bookSorter = (x, y) => CompareBooks(x, y);
		static int CompareBooks(int a, int b)
		{
			if (UICustomizePopup.Instance.clothsPanel.EquipPageCustomizePanel.isWorkshop)
			{
				return b.CompareTo(a);
			}

			Predicate<BookXmlInfo> match = x => x != null;
			switch (UICustomizePopup.Instance.SelectedUnit.bookItem.ClassInfo.RangeType)
			{
				case EquipRangeType.Melee:
					match = x => x != null && x.RangeType != EquipRangeType.Range;
					break;
				case EquipRangeType.Range:
					match = x => x != null && x.RangeType != EquipRangeType.Melee;
					break;
				case EquipRangeType.Hybrid:
					match = x => x != null && x.RangeType == EquipRangeType.Hybrid;
					break;
			}

			var adata = BookXmlList.Instance.GetData(a);
			var bdata = BookXmlList.Instance.GetData(b);
			if (match(adata))
			{
				if (!match(bdata))
				{
					return -1;
				}
			}
			else
			{
				if (match(bdata))
				{
					return 1;
				}
			}

			return b.CompareTo(a);
		}

		[HarmonyPatch(typeof(UIEquipPageCustomizePanel), nameof(UIEquipPageCustomizePanel.ApplyFilterAll))]
		[HarmonyPostfix]
		static void UIEquipPageCustomizePanel_ApplyFilterAll_Postfix(UIEquipPageCustomizePanel __instance)
		{
			try
			{
				if (isModWorkshopSkin)
				{
					List<Grade> storyGradeFilter = __instance.gradeFilter.GetStoryGradeFilter();
					__instance.filteredBookIdList.Clear();
					if (storyGradeFilter.Count == 0)
					{
						__instance.filteredBookIdList.AddRange(__instance.originBookIdList);
					}
					else
					{
						foreach (Grade g in storyGradeFilter)
						{
							__instance.filteredBookIdList.AddRange(__instance.originBookIdList.FindAll((int x) => GetModWorkshopBookData(x)?.Chapter == (int)g));
						}
					}
					Predicate<BookXmlInfo> match = x => x != null;
					switch (__instance.panel.Parent.SelectedUnit.bookItem.ClassInfo.RangeType)
					{
						case EquipRangeType.Melee:
							match = x => x != null && x.RangeType != EquipRangeType.Range;
							break;
						case EquipRangeType.Range:
							match = x => x != null && x.RangeType != EquipRangeType.Melee;
							break;
						case EquipRangeType.Hybrid:
							match = x => x != null && x.RangeType == EquipRangeType.Hybrid;
							break;
					}
					__instance.filteredBookIdList.Sort((int a, int b) => CompareBooksMod(a, b, match));
					int num = __instance.GetMaxRow();
					__instance.scrollBar.SetScrollRectSize(__instance.column * __instance.slotWidth, (num + __instance.row - 1) * __instance.slotHeight);
					__instance.scrollBar.SetWindowPosition(0f, 0f);
					__instance.ParentSelectable.ChildSelectable = __instance.bookSlotList[0].selectable;
					__instance.UpdateBookListPage(false);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				isModWorkshopSkin = false;
				__instance.ApplyFilterAll();
			}
		}
		static int CompareBooksMod(int a, int b, Predicate<BookXmlInfo> match)
		{
			var ar = GetModWorkshopBookData(a);
			var br = GetModWorkshopBookData(b);
			if (match(ar))
			{
				if (!match(br))
				{
					return -1;
				}
			}
			else
			{
				if (match(br))
				{
					return 1;
				}
			}
			return b.CompareTo(a);
		}

		[HarmonyPatch(typeof(UIEquipPageCustomizePanel), nameof(UIEquipPageCustomizePanel.UpdateEquipPageSlotList))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> UIEquipPageCustomizePanel_UpdateEquipPageSlotList_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilgen)
		{
			var method = Method(typeof(BookXmlList), nameof(BookXmlList.GetData), new Type[] { typeof(int) });
			foreach (var instruction in instructions)
			{
				if (instruction.Calls(method))
				{
					var local = ilgen.DeclareLocal(typeof(int));
					yield return new CodeInstruction(Dup);

					yield return new CodeInstruction(Stloc, local);
					yield return instruction;
					yield return new CodeInstruction(Ldloc, local);
					yield return new CodeInstruction(Call, Method(typeof(CustomBookUIPatch), nameof(CustomBookUIPatch.TryReplaceWorkshopBookData)));
				}
				else
				{
					yield return instruction;
				}
			}
		}
		static BookXmlInfo TryReplaceWorkshopBookData(BookXmlInfo bookXmlInfo, int i)
		{
			if (!isModWorkshopSkin)
			{
				return bookXmlInfo;
			}
			return GetModWorkshopBookData(i) ?? bookXmlInfo;
		}
		static BookXmlInfo GetModWorkshopBookData(int i)
		{
			if (modWorkshopBookIndex.TryGetValue(i, out var id) && BookXmlList.Instance.GetData(id) is BookXmlInfo info && !info.isError)
			{
				return info;
			}
			return null;
		}

		[HarmonyPatch(typeof(UICustomizeClothsPanel), nameof(UICustomizeClothsPanel.InitData))]
		[HarmonyTranspiler]
		[HarmonyPriority(Priority.High)]
		static IEnumerable<CodeInstruction> UICustomizeClothsPanel_InitData_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var equipCountField = Field(typeof(UICustomizeClothsPanel), nameof(UICustomizeClothsPanel.equippableCount));
			var equipListField = Field(typeof(UICustomizeClothsPanel), nameof(UICustomizeClothsPanel.equipBookInfoList));
			var listAddMethod = Method(typeof(List<int>), nameof(List<int>.Add));

			var codeMatcher = new CodeMatcher(instructions);

			var incrementMatch = new CodeMatch[] { new CodeMatch(Ldarg_0), new CodeMatch(Ldarg_0), new CodeMatch(Ldfld, equipCountField),
				new CodeMatch(Ldc_I4_1), new CodeMatch(Add), new CodeMatch(Stfld, equipCountField) };

			var addMatch = new CodeMatch[] { new CodeMatch(Callvirt, listAddMethod) };

			for (codeMatcher.MatchStartForward(addMatch); codeMatcher.IsValid; codeMatcher.MatchStartForward(addMatch))
			{
				if (codeMatcher.Pos > 1)
				{
					var listLoader = codeMatcher.InstructionAt(-2);
					if (listLoader.opcode == Ldloc_1)
					{
						if (codeMatcher.MatchSequence(codeMatcher.Pos + 1, incrementMatch))
						{
							codeMatcher.Advance(1);
							codeMatcher.RemoveInstructions(incrementMatch.Length);
							codeMatcher.Advance(-1);
						}
					}
					else if (listLoader.LoadsField(equipListField))
					{
						if (codeMatcher.Pos < codeMatcher.Length - 1 && codeMatcher.InstructionAt(1).opcode != Ldarg_0)
						{
							var valueLoader = codeMatcher.InstructionAt(-1);
							if (valueLoader.IsLdloc(4) || valueLoader.IsLdloc(6) || valueLoader.IsLdloc(8))
							{
								codeMatcher.Advance(1);
								codeMatcher.Insert(new CodeInstruction(Ldarg_0), new CodeInstruction(Ldarg_0), new CodeInstruction(Ldfld, equipCountField),
									new CodeInstruction(Ldc_I4_1), new CodeInstruction(Add), new CodeInstruction(Stfld, equipCountField));
							}
						}
					}
				}
				codeMatcher.Advance(1);
			}

			var codes = codeMatcher.InstructionEnumeration().ToList();
			for (int j = 0; j < codes.Count; j++)
			{
				if ((codes[j].opcode == Leave || codes[j].opcode == Leave_S) && codes[j + 1].opcode != Nop)
				{
					codes.Insert(j + 1, new CodeInstruction(Nop).MoveBlocksFrom(codes[j]));
				}
			}
			return codes;
		}


		[HarmonyPatch(typeof(UICustomizeClothsPanel), nameof(UICustomizeClothsPanel.RandomCustom))]
		[HarmonyPrefix]
		static void UICustomizeClothsPanel_RandomCustom_Prefix(UICustomizeClothsPanel __instance)
		{
			__instance.PreviewData.WorkshopSkinData = "";
		}

		[HarmonyPatch(typeof(UICustomizeClothsPanel), nameof(UICustomizeClothsPanel.RandomCustom))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> UICustomizeClothsPanel_RandomCustom_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var getDataMethod = Method(typeof(BookXmlList), nameof(BookXmlList.GetData), new Type[] { typeof(int) });
			var showMethod = Method(typeof(UICustomizeClothsPanel), nameof(UICustomizeClothsPanel.Show));
			foreach (var instruction in instructions)
			{
				if (instruction.Calls(showMethod))
				{
					yield return new CodeInstruction(Call, Method(typeof(CustomBookUIPatch), nameof(CustomBookUIPatch.ShowWithoutReset)));
				}
				else
				{
					yield return instruction;
					if (instruction.Calls(getDataMethod))
					{
						yield return new CodeInstruction(Ldarg_0);
						yield return new CodeInstruction(Call, Method(typeof(CustomBookUIPatch), nameof(CustomBookUIPatch.CheckRandomSkin)));
					}
				}
			}
		}
		static BookXmlInfo CheckRandomSkin(BookXmlInfo original, UICustomizeClothsPanel panel)
		{
			if (panel.EquipPageCustomizePanel.isWorkshop && panel.workshopSkinInfoList.Count != 0)
			{
				panel.PreviewData.WorkshopSkinData = CustomizingResourceLoader.Instance.GetWorkshopSkinData(RandomUtil.SelectOne(panel.workshopSkinInfoList))?.contentFolderIdx ?? "";
				return null;
			}
			if (isModWorkshopSkin && equippableWorkshopBookCount > 0)
			{
				return GetModWorkshopBookData(modWorkshopBookList[UnityEngine.Random.Range(0, equippableWorkshopBookCount)]);
			}
			return original;
		}
		static void ShowWithoutReset(UICustomizeClothsPanel panel)
		{
			panel.gameObject.SetActive(true);
			panel.sizeSlider.InitBarValue(panel.PreviewData.LookData.height);
			panel.OnExitCostume();
			panel.SetAppearanceButton();
			panel.UpdatePanel();
			panel.Parent.SelectablePanel_Right.ChildSelectable = panel.ListSelectable;
		}


		internal static bool isModWorkshopSkin;
		static GameObject workshopBookButton;
		internal static UICustomizeClothsPanel clothsPanel;
		internal static Dictionary<int, LorId> modWorkshopBookIndex = new Dictionary<int, LorId>();
		internal static List<int> modWorkshopBookList = new List<int>();
		internal static int equippableWorkshopBookCount = 0;


		internal static void IntegrateSearcher()
		{
			try
			{
				var searcherAssembly = (from a in AppDomain.CurrentDomain.GetAssemblies()
										where a.GetName().Name == "KeypageSearcher"
										select a into v
										orderby v.GetName().Version descending
										select v).FirstOrDefault();
				var registerListMethod = searcherAssembly?.GetType("KeypageSearcher.KeypageSearchPatches")?.GetMethod("AddCustomProjectionList", new Type[] { typeof(Func<bool>), typeof(Func<List<int>>), typeof(Func<int, string>) });
				registerListMethod?.Invoke(null, new object[] {
					new Func<bool>(() =>
					{
						return isModWorkshopSkin;
					}),
					new Func<List<int>>(() =>
					{
						return modWorkshopBookList;
					}),
					new Func<int,string>(x =>
					{
						return GetModWorkshopBookData(x)?.Name;
					})
				});
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
	}
}
