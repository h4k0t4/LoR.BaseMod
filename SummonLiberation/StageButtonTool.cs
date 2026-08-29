using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SummonLiberation
{
	public class ButtonColor : EventTrigger
	{
		void Update()
		{
		}
		public override void OnPointerEnter(PointerEventData eventData)
		{
			Image.color = OnEnterColor;
		}
		public override void OnPointerExit(PointerEventData eventData)
		{
			Image.color = DefaultColor;
		}
		public override void OnPointerUp(PointerEventData eventData)
		{
			Image.color = DefaultColor;
		}

		public Color DefaultColor = Color.white;

		public static Color OnEnterColor;

		public Image Image;
	}

	[Obsolete("StageButtonTool has been deprecated due to UnitRenderUtils providing scrolling functionality", true)]
	public static class StageButtonTool
	{
		public static void Init()
		{
		}

		public static void RefreshEnemy()
		{
		}

		public static void RefreshLibrarian()
		{
		}

		public static void OnClickEnemyUP()
		{
		}

		public static void OnClickEnemyDown()
		{
		}

		public static void OnClickLibrarianUP()
		{
		}

		public static void OnClickLibrarianDown()
		{
		}

		public static void UpdateEnemyCharacterList()
		{
		}

		public static void UpdateLibrarianCharacterList()
		{
		}
	}
}
