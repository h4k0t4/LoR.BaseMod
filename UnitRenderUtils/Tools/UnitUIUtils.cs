using HarmonyLib;
using System;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ExtendedLoader
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	public static class UnitUIUtils
	{
		public static void SetBattleSidebarCount(int count)
		{
			UnitLimitPatch.minEmotionSlots = Math.Max(UnitLimitPatch.minEmotionSlots, count);
		}

		public static void SetBattleSidebarDistance(float shift)
		{
			UnitLimitPatch.yShift = Math.Min(UnitLimitPatch.yShift, shift);
		}

		public static UILibrarianCharacterListPanel GetLibrarianCharacterListPanel()
		{
			return UI.UIController.Instance.GetUIPanel(UIPanelType.CharacterList_Right) as UILibrarianCharacterListPanel;
		}
		public static UIEnemyCharacterListPanel GetEnemyCharacterListPanel()
		{
			return UI.UIController.Instance.GetUIPanel(UIPanelType.CharacterList) as UIEnemyCharacterListPanel;
		}

		internal static void SetSlotCount(UICharacterList characterList, int count)
		{
			count = Math.Max(count, 5);
			if (count == characterList.slotList.Count)
			{
				return;
			}
			if (count < characterList.slotList.Count)
			{
				for (int i = characterList.slotList.Count - 1; i >= count; i--)
				{
					characterList.slotList[i].SetSlot(null, Color.black);
				}
			}
			else
			{
				var enemyListPanel = GetEnemyCharacterListPanel();
				if (enemyListPanel && enemyListPanel.CharacterList == characterList)
				{
					changedEnemySlotCount = true;
				}
				//var characterList = (CurrentTarget as GameObject).GetComponent<UICharacterList>();
				var scrollRect = characterList.GetComponent<ScrollRect>();
				if (!scrollRect)
				{
					scrollRect = characterList.gameObject.AddComponent<ScrollRect>();
					scrollRect.vertical = false;
					scrollRect.scrollSensitivity = 30f;
					scrollRect.movementType = ScrollRect.MovementType.Clamped;
					var contentRect = characterList.slotList[0].transform.parent as RectTransform;
					scrollRect.content = contentRect;
					var contentFitter = contentRect.GetComponent<ContentSizeFitter>();
					if (!contentFitter)
					{
						contentFitter = contentRect.gameObject.AddComponent<ContentSizeFitter>();
						contentFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
						contentFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
					}
					foreach (var character in characterList.slotList)
					{
						var trigger = character.GetComponentsInChildren<EventTrigger>(true).FirstOrDefault();
						var triggerEntry = new EventTrigger.Entry { eventID = EventTriggerType.Scroll };
						triggerEntry.callback.AddListener(bData => 
						{
							if (bData is PointerEventData pData && scrollRect && scrollRect.hScrollingNeeded)
							{
								scrollRect.OnScroll(pData);
							}
						});
						trigger.triggers.Add(triggerEntry);
						var effect = character.img_hpFill.GetComponent<_2dxFX_WaterAndBackgroundDeluxe>();
						if (effect)
						{
							effect.defaultMaterial = character.SelectedFrame.GetComponent<Image>().material;
						}
					}
					var maskRect = new GameObject("[Rect]Mask").AddComponent<RectTransform>();
					maskRect.SetParent(contentRect.parent);
					maskRect.anchorMin = Vector2.zero;
					maskRect.anchorMax = Vector2.one;
					maskRect.pivot = (contentRect.parent as RectTransform).pivot;
					maskRect.sizeDelta = Vector2.zero;
					maskRect.localRotation = Quaternion.identity;
					maskRect.localScale = Vector2.one;
					maskRect.anchoredPosition = Vector2.zero;
					//maskRect.gameObject.AddComponent<Image>();
					//maskRect.gameObject.AddComponent<Mask>().showMaskGraphic = false;
					maskRect.gameObject.AddComponent<RectMask2D>();
					maskRect.SetSiblingIndex(contentRect.GetSiblingIndex());
					maskRect.offsetMax += new Vector2(3, 5);
					maskRect.offsetMin -= new Vector2(3, 10);
					var posRect = new GameObject("[Rect]ListPos").AddComponent<RectTransform>();
					posRect.SetParent(maskRect);
					posRect.anchorMin = Vector2.zero;
					posRect.anchorMax = Vector2.one;
					posRect.pivot = (contentRect.parent as RectTransform).pivot;
					posRect.sizeDelta = Vector2.zero;
					posRect.localRotation = Quaternion.identity;
					posRect.localScale = Vector2.one;
					posRect.anchoredPosition = Vector2.zero;
					posRect.offsetMax -= new Vector2(3, 5);
					posRect.offsetMin += new Vector2(3, 10);
					contentRect.SetParent(posRect);
					contentRect.pivot = new Vector2(0, 1);

					Image leftArrow = UnityEngine.Object.Instantiate(((UICardPanel)UIPanel.Controller.GetUIPanel(UIPanelType.Page)).InvenCardList.scrollBar.ImageDown, characterList.transform, false);
					leftArrow.name = "[Image]Left";
					var leftRect = leftArrow.transform as RectTransform;
					leftRect.sizeDelta = new Vector2(60, 40);
					leftRect.localRotation = Quaternion.Euler(0, 0, -90);
					leftRect.pivot = new Vector2(1, 0);//new Vector2(0, 0);
					leftRect.anchorMin = leftRect.anchorMax = Vector2.zero;//new Vector2(0, 1);
					var leftHandler = leftArrow.gameObject.AddComponent<UnitScrollHandler>();
					leftHandler.basePos = leftRect.anchoredPosition = new Vector2(-5, 40);//new Vector2(-5, 0);
					leftHandler.image = leftArrow;
					leftHandler.scrollRect = scrollRect;
					var leftTrigger = leftArrow.GetComponent<EventTrigger>();
					if (leftTrigger)
					{
						leftTrigger.triggers.Clear();

						var pointerDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
						pointerDownEntry.callback.AddListener(leftHandler.PointerDown);
						leftTrigger.triggers.Add(pointerDownEntry);

						var pointerUpEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
						pointerUpEntry.callback.AddListener(leftHandler.PointerUp);
						leftTrigger.triggers.Add(pointerUpEntry);

						var pointerEnterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
						pointerEnterEntry.callback.AddListener(leftHandler.PointerEnter);
						leftTrigger.triggers.Add(pointerEnterEntry);

						var pointerExitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
						pointerExitEntry.callback.AddListener(leftHandler.PointerExit);
						leftTrigger.triggers.Add(pointerExitEntry);
					}
					leftArrow.gameObject.SetActive(false);

					Image rightArrow = UnityEngine.Object.Instantiate(leftArrow, characterList.transform, false);
					rightArrow.name = "[Image]Right";
					var rightRect = rightArrow.transform as RectTransform;
					rightRect.sizeDelta = new Vector2(60, 40);
					rightRect.localRotation = Quaternion.Euler(0, 0, 90);
					rightRect.pivot = Vector2.zero;//new Vector2(1, 0);
					rightRect.anchorMin = rightRect.anchorMax = new Vector2(1, 0);//new Vector2(1, 1);
					var rightHandler = rightArrow.GetComponent<UnitScrollHandler>();
					rightHandler.basePos = rightRect.anchoredPosition = new Vector2(5, 40);//new Vector2(5, 0);
					rightHandler.image = rightArrow;
					rightHandler.scrollRect = scrollRect;
					var rightTrigger = rightArrow.GetComponent<EventTrigger>();
					if (rightTrigger)
					{
						rightTrigger.triggers.Clear();

						var pointerDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
						pointerDownEntry.callback.AddListener(rightHandler.PointerDown);
						rightTrigger.triggers.Add(pointerDownEntry);

						var pointerUpEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
						pointerUpEntry.callback.AddListener(rightHandler.PointerUp);
						rightTrigger.triggers.Add(pointerUpEntry);

						var pointerEnterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
						pointerEnterEntry.callback.AddListener(rightHandler.PointerEnter);
						rightTrigger.triggers.Add(pointerEnterEntry);

						var pointerExitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
						pointerExitEntry.callback.AddListener(rightHandler.PointerExit);
						rightTrigger.triggers.Add(pointerExitEntry);
					}
					rightArrow.gameObject.SetActive(false);

					int minIn = 0;
					int maxIn = 4;
					scrollRect.onValueChanged.AddListener(pos => 
					{
						float overflow = scrollRect.GetBounds().size.x - scrollRect.viewRect.rect.size.x;
						leftArrow.gameObject.SetActive(scrollRect.hScrollingNeeded && (pos.x * overflow > 0.01f));
						rightArrow.gameObject.SetActive(scrollRect.hScrollingNeeded && ((1 - pos.x) * overflow > 0.01f));
						int newMinIn = -1;
						int newMaxIn = -1;
						int i = 0;
						float minX = maskRect.rect.xMin;
						float maxX = maskRect.rect.xMax;
						var worldToMask = maskRect.worldToLocalMatrix;
						foreach (var character in characterList.slotList)
						{
							if (!character.gameObject.activeSelf)
							{
								break;
							}
							RectTransform charRectT = character.transform as RectTransform;
							Rect charRect = charRectT.rect;
							Matrix4x4 charToMask = worldToMask * charRectT.localToWorldMatrix;
							if (charToMask.MultiplyPoint(charRect.center - new Vector2(charRect.width / 2, 0)).x >= minX 
								&& charToMask.MultiplyPoint(charRect.center + new Vector2(charRect.width / 2, 0)).x <= maxX)
							{
								newMaxIn = i;
								if (newMinIn == -1)
								{
									newMinIn = i;
								}
							}
							i++;
						}
						if (newMaxIn != maxIn || newMinIn != minIn)
						{
							for (i = 0; i < characterList.slotList.Count; i++)
							{
								bool inBounds = i >= newMinIn && i <= newMaxIn;
								bool wasInBounds = i >= minIn && i <= maxIn;
								if (inBounds != wasInBounds)
								{
									characterList.slotList[i].img_hpFill.GetComponent<_2dxFX_WaterAndBackgroundDeluxe>().enabled = inBounds;
								}
							}
							minIn = newMinIn;
							maxIn = newMaxIn;
						}
					});
					scrollRect.onValueChanged.Invoke(scrollRect.normalizedPosition);
					characterList.panel.graphics.graphics = characterList.panel.graphics.graphics.AddRangeToArray(new Graphic[]
					{
						leftArrow, rightArrow
					});

					var blockerRect = new GameObject("[Rect]StoryScrollBlocker") { transform = { parent = characterList.transform } }.AddComponent<RectTransform>();
					blockerRect.anchorMin = new Vector2(0, 0);
					blockerRect.anchorMax = new Vector2(1, 1);
					blockerRect.localScale = Vector3.one;
					blockerRect.sizeDelta = Vector2.zero;
					blockerRect.anchoredPosition = Vector2.zero;
					blockerRect.SetSiblingIndex(0);
					var blockerImg = blockerRect.gameObject.AddComponent<Image>();
					blockerImg.color = Color.clear;
				}
				for (int i = characterList.slotList.Count; i < count; i++)
				{
					var newSlot = UnityEngine.Object.Instantiate(characterList.slotList[1], characterList.slotList[1].transform.parent);
					newSlot.name = $"[Prefab]Character_Slot ({i})";
					var effect = newSlot.img_hpFill.GetComponent<_2dxFX_WaterAndBackgroundDeluxe>();
					if (effect)
					{
						effect.defaultMaterial = characterList.slotList[1].img_hpFill.GetComponent<_2dxFX_WaterAndBackgroundDeluxe>().defaultMaterial;
						effect.enabled = false;
					}
					characterList.slotList.Add(newSlot);
				}
				if (changedEnemySlotCount)
				{
					if (UIPanel.Controller.CurrentUIPhase == UIPhase.BattleSetting)
					{
						try
						{
							updatingLibrarianForWave = true;
							GetLibrarianCharacterListPanel().SetLibrarianCharacterListPanel_Battle();
						}
						finally
						{
							updatingLibrarianForWave = false;
						}
					}
					else
					{
						GetLibrarianCharacterListPanel().SetLibrarianCharacterListPanel_Default(UIPanel.Controller.CurrentSephirah);
					}
				}
			}
		}

		class UnitScrollHandler : MonoBehaviour
		{
			void Update()
			{
				(transform as RectTransform).anchoredPosition = basePos + 2.5f * (Vector2)(transform.up * Mathf.Sin(Time.time * 3));
				if (pressed && scrollRect.hScrollingNeeded)
				{
					scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(scrollRect.horizontalNormalizedPosition 
						- transform.up.x * scrollRect.scrollSensitivity * Time.deltaTime * 10 / (scrollRect.GetBounds().size.x - scrollRect.viewRect.rect.size.x));
				}
			}

			public void PointerEnter(BaseEventData _)
			{
				Color highlightedColor = UIColorManager.Manager.GetUIColor(UIColor.Highlighted);
				if (image.color != highlightedColor)
				{
					defaultColor = image.color;
				}
				image.color = highlightedColor;
			}

			public void PointerDown(BaseEventData _)
			{
				pressed = true;
			}

			public void PointerUp(BaseEventData _)
			{
				pressed = false;
			}

			public void PointerExit(BaseEventData _)
			{
				if (defaultColor != Color.clear)
				{
					image.color = defaultColor;
				}
			}

			void OnDisable()
			{
				PointerUp(null);
				if (image.color != defaultColor && image.color != UIColorManager.Manager.GetUIColor(UIColor.Highlighted))
				{
					defaultColor = image.color;
				}
				PointerExit(null);
			}

			bool pressed;
			public Vector2 basePos;
			public ScrollRect scrollRect;
			public Image image;
			Color defaultColor;
		}

		internal static bool updatingLibrarianForWave;

		internal static bool changedEnemySlotCount;
	}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
