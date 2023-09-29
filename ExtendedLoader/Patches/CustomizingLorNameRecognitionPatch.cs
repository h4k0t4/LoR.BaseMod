using HarmonyLib;
using System;
using Workshop;
using LorIdExtensions;
using UnityEngine;

namespace ExtendedLoader
{
	[HarmonyPatch]
	internal class CustomizingLorNameRecognitionPatch
	{
		[HarmonyPatch(typeof(CustomizingResourceLoader), nameof(CustomizingResourceLoader.GetWorkshopSkinData), new Type[] { typeof(string) })]
		[HarmonyPostfix]
		static void CustomizingResourceLoader_GetWorkshopSkinData_Postfix(ref WorkshopSkinData __result, string id)
		{
			if (__result == null && LorName.IsCompressed(id))
			{
				__result = SkinTools.GetWorkshopBookSkinData(new LorName(id), "");
			}
		}

		[HarmonyPatch(typeof(CustomizingBookSkinLoader), nameof(CustomizingBookSkinLoader.GetWorkshopBookSkinData), new Type[] { typeof(string), typeof(string) })]
		[HarmonyFinalizer]
		static Exception CustomizingBookSkinLoader_GetWorkshopBookSkinData_Finalizer(Exception __exception, ref WorkshopSkinData __result, string id, string name)
		{
			if (__exception != null)
			{
				Debug.Log($"Got an error trying to load skin data for {id}:{name}, this is most frequently caused by mods attempting to add special motions with hardcoded patches without null-checking to see if a skin has been returned to begin with: ");
				Debug.LogException(__exception);
			}
			if (__result == null)
			{
				if (LorName.IsCompressed(name))
				{
					__result = SkinTools.GetWorkshopBookSkinData(new LorName(name), "");
				}
				else if (LorName.IsWorkshopGenericId(id))
				{
					__result = CustomizingResourceLoader.Instance.GetWorkshopSkinData(name);
				}
			}
			return null;
		}
	}
}
