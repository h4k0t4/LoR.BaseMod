using HarmonyLib;
using UnityEngine;

namespace ExtendedLoader
{
	[HarmonyPatch]
	internal class AbCardSelectorSizePatch
	{
		[HarmonyPatch(typeof(BattleUnitView), nameof(BattleUnitView.ChangeHeight))]
		[HarmonyPrefix]
		static void BattleUnitView_ChangeHeight_Prefix(BattleUnitView __instance, int height)
		{
			float num = height / 170f;
			if (__instance.abCardSelector?.transform != null)
			{
				__instance.abCardSelector.transform.localScale = new Vector3(0.7f / num, 0.7f / num, 1);
			}
		}
	}
}
