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

namespace ExtendedLoader
{
	[HarmonyPatch]
	class CustomBookUIPatch
	{
		[HarmonyPatch(typeof(UICustomizeClothsPanel), nameof(UICustomizeClothsPanel.InitData))]
		[HarmonyPostfix]
		static void UICustomizeClothsPanel_InitData_Prefix(UICustomizeClothsPanel __instance)
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
					submitEvent.m_PersistentCalls.Clear();
					submitEvent.AddCall(new InvokableCall(XLRoot.Instance, Method(typeof(XLRoot), nameof(XLRoot.OnClickWorkshopBook))));
					workshopBookButton = workshopButton.gameObject;
				}
				ReloadCustomProjectionIndex();
				workshopBookButton.SetActive(modWorkshopBookList.Count > 0);
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
			int num = 0;
			foreach (LorId lorId in BookInventoryModel.Instance.GetIdList_noDuplicate().OrderByDescending(id => id.packageId).ThenBy(id => id.id))
			{
				BookXmlInfo info = BookXmlList.Instance.GetData(lorId);
				if (lorId.IsWorkshop() && !info.isError && !info.canNotEquip)
				{
					modWorkshopBookIndex.Add(num, lorId);
					modWorkshopBookList.Add(num);
					num++;
				}
			}
			return modWorkshopBookList;
		}
		[HarmonyPatch(typeof(UIEquipPageCustomizePanel), nameof(UIEquipPageCustomizePanel.Init))]
		[HarmonyPrefix]
		static void UIEquipPageCustomizePanel_Init_Prefix(List<int> data)
		{
			isModWorkshopSkin = data == modWorkshopBookList;
		}
		[HarmonyPatch(typeof(UIEquipPageCustomizePanel), nameof(UIEquipPageCustomizePanel.ApplyFilterAll))]
		[HarmonyPrefix]
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
							__instance.filteredBookIdList.AddRange(__instance.originBookIdList.FindAll((int x) => GetModWorkshopBookData(x).Chapter == (int)g));
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
		static int CompareBooksMod(int a, int b, Predicate<EquipRangeType> match)
		{
			var ar = GetModWorkshopBookData(a).RangeType;
			var br = GetModWorkshopBookData(b).RangeType;
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
					yield return new CodeInstruction(Call, Method(typeof(CustomBookUIPatch), nameof(TryReplaceWorkshopBookData)));
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
			return GetModWorkshopBookData(i);
		}
		static BookXmlInfo GetModWorkshopBookData(int i)
		{
			if (modWorkshopBookIndex.TryGetValue(i, out var id))
			{
				return BookXmlList.Instance.GetData(id);
			}
			return BookXmlList.Instance.GetData(12);
		}

		internal static bool isModWorkshopSkin;
		static GameObject workshopBookButton;
		internal static UICustomizeClothsPanel clothsPanel;
		internal static Dictionary<int, LorId> modWorkshopBookIndex = new Dictionary<int, LorId>();
		internal static List<int> modWorkshopBookList = new List<int>();
	}
}
