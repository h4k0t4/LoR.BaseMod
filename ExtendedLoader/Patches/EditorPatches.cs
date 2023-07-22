using System;
using HarmonyLib;
using UnityEngine;

namespace ExtendedLoader
{
	[HarmonyPatch]
	internal class EditorPatches
	{
		[HarmonyPatch(typeof(FaceEditor), nameof(FaceEditor.Init))]
		[HarmonyPostfix]
		static void FaceEditor_Init_Postfix(FaceEditor __instance)
		{
			try
			{
				(__instance.face.gameObject.transform as RectTransform).sizeDelta = __instance.face.sprite.pivot;
				(__instance.frontHair.gameObject.transform as RectTransform).sizeDelta = __instance.frontHair.sprite.pivot;
				(__instance.backHair.gameObject.transform as RectTransform).sizeDelta = __instance.backHair.sprite.pivot;
				(__instance.mouth.gameObject.transform as RectTransform).sizeDelta = __instance.mouth.sprite.pivot;
				(__instance.brow.gameObject.transform as RectTransform).sizeDelta = __instance.brow.sprite.pivot;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
	}
}
