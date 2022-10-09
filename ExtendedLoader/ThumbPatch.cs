using UI;
using UnityEngine;
using Workshop;
using System;
using HarmonyLib;

namespace ExtendedLoader
{
    public class ThumbPatch
    {
        [HarmonyPatch(typeof(UICustomizeClothsPanel), nameof(UICustomizeClothsPanel.SetPreviewPortrait), new Type[] { typeof(WorkshopSkinData) })]
        [HarmonyPrefix]
        static bool SetPreviewPortraitPrefix(WorkshopSkinData data, UICustomizeClothsPanel __instance)
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
    }
}
