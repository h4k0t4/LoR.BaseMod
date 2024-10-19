using UnityEngine;
using System;
using HarmonyLib;

namespace ExtendedLoader
{
	[HarmonyPatch]
	internal class ThumbPatchBasic
	{
		[HarmonyPatch(typeof(BookModel), nameof(BookModel.GetThumbSprite))]
		[HarmonyPrefix]
		[HarmonyPriority(Priority.LowerThanNormal)]
		static bool BookModel_GetThumbSprite_Prefix(BookModel __instance, ref Sprite __result)
		{
			return BookXmlInfo_GetThumbSprite_Prefix(__instance.ClassInfo, ref __result);
		}

		[HarmonyPatch(typeof(BookXmlInfo), nameof(BookXmlInfo.GetThumbSprite))]
		[HarmonyPrefix]
		[HarmonyPriority(Priority.LowerThanNormal)]
		static bool BookXmlInfo_GetThumbSprite_Prefix(BookXmlInfo __instance, ref Sprite __result)
		{
			if (__result != null || __instance.skinType == "Custom")
			{
				return true; 
			}
			try
			{
				var skin = __instance.GetCharacterSkin();
				if (skin == null)
				{
					return true;
				}
				if (XLUtilRoot.extendedCoreThumbDic.ContainsKey(skin))
				{
					__result = XLUtilRoot.extendedCoreThumbDic[skin];
					return false;
				}
				XLUtilRoot.coreThumbDic.TryGetValue(skin, out int index);
				if (index == 0)
				{
					int skip = skin.IndexOf(':');
					if (skip >= 0)
					{
						skin = skin.Substring(skip + 1);
						XLUtilRoot.coreThumbDic.TryGetValue(skin, out index);
					}
				}
				if (index > 0)
				{
					if ((__result = Resources.Load<Sprite>("Sprites/Books/Thumb/" + index)) != null)
					{
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			return true;
		}
	}
}
