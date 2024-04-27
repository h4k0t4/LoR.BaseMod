using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

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
				foreach (var image in new Image[] {__instance.face, __instance.frontHair, __instance.backHair, __instance.mouth, __instance.brow})
				{
					FixPivot(image);
				}
			}	
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		static void FixPivot(Image image)
		{
			if (image && image.sprite && image.gameObject.transform is RectTransform rect)
			{
				rect.sizeDelta = image.sprite.pivot;
			}
		}
	}
}
