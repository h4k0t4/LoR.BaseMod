using UI;
using UnityEngine;
using Workshop;
using System;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;

namespace ExtendedLoader
{
	[HarmonyPatch]
	internal class ThumbPatch
	{
		[HarmonyPatch(typeof(UICustomizeClothsPanel), nameof(UICustomizeClothsPanel.SetPreviewPortrait), new Type[] { typeof(WorkshopSkinData) })]
		[HarmonyPrefix]
		static bool UICustomizeClothsPanel_SetPreviewPortrait_Prefix(WorkshopSkinData data, UICustomizeClothsPanel __instance)
		{
			Sprite customThumb = data.GetThumbSprite();
			if (customThumb != null)
			{
				__instance.PreviewImage.sprite = customThumb;
				__instance.PreviewImage.SetNativeSize();
				__instance.PreviewImage.rectTransform.anchoredPosition = new Vector3(5.5f, -15f, 0f);
				__instance.PreviewImage.rectTransform.localScale = new Vector3(0.7f, 0.7f, 1f);
				return false;
			}
			return true;
		}
		[HarmonyPatch(typeof(BookModel), nameof(BookModel.GetThumbSprite))]
		[HarmonyPrefix]
		static bool BookModel_GetThumbSprite_Prefix(BookModel __instance, ref Sprite __result)
		{
			return BookXmlInfo_GetThumbSprite_Prefix(__instance.ClassInfo, ref __result);
		}
		[HarmonyPatch(typeof(BookXmlInfo), nameof(BookXmlInfo.GetThumbSprite))]
		[HarmonyPrefix]
		static bool BookXmlInfo_GetThumbSprite_Prefix(BookXmlInfo __instance, ref Sprite __result)
		{
			if (__result != null)
			{
				return true; 
			}
			try
			{
				WorkshopSkinData workshopBookSkinData = SkinTools.GetWorkshopBookSkinData(__instance.id.packageId, __instance.GetCharacterSkin(), "_" + __instance.gender);
				string thumbPath = null;
				if (workshopBookSkinData != null)
				{
					if (!workshopBookSkinData.dic.TryGetValue(ActionDetail.Default, out var defaultData))
					{
						return true;
					}
					if (!File.Exists(defaultData.spritePath))
					{
						return true;
					}
					DirectoryInfo spriteDir = new DirectoryInfo(defaultData.spritePath);
					thumbPath = Path.Combine(spriteDir.Parent.Parent.FullName, "Thumb.png");
				}
				Sprite bookThumb = GetBookThumb(__instance, thumbPath);
				if (bookThumb != null)
				{
					__result = bookThumb;
					return false;
				}
				if (workshopBookSkinData != null)
				{
					XLRoot.MakeThumbnail(workshopBookSkinData.dic[ActionDetail.Default]);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			return true;
		}
		static Sprite GetBookThumb(BookXmlInfo BookInfo, string Path)
		{
			var BookId = BookInfo.id;
			if (XLRoot.BookThumb == null)
			{
				XLRoot.BookThumb = new Dictionary<LorId, Sprite>();
			}
			if (XLRoot.BookThumb.TryGetValue(BookId, out Sprite result))
			{
				return result;
			}
			else if (Path != null && File.Exists(Path))
			{
				Texture2D texture2D = new Texture2D(2, 2);
				texture2D.LoadImage(File.ReadAllBytes(Path));
				result = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
			}
			if (result != null)
			{
				XLRoot.BookThumb[BookId] = result;
			}
			return result;
		}
	}
}
