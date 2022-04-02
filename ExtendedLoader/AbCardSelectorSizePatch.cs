using HarmonyLib;
using UnityEngine;

namespace ExtendedLoader
{
    public class AbCardSelectorSizePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BattleUnitView), nameof(BattleUnitView.ChangeHeight))]
        static void ChangeHeightPostfix(BattleUnitView __instance, int height)
        {
            float num = height / 170f;
            if (__instance.abCardSelector?.transform != null)
            {
                __instance.abCardSelector.transform.localScale = new Vector3(0.7f / num, 0.7f / num, 1);
            }
        }
    }
}
