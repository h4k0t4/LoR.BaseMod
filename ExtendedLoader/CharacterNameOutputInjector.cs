using HarmonyLib;

namespace ExtendedLoader
{
    public class CharacterNameOutputInjector
    {
        [HarmonyPatch(typeof(BattleUnitView), nameof(BattleUnitView.CreateSkin))]
        [HarmonyPrefix]
        static void CreateSkinPrefix(BattleUnitView __instance)
        {
            string skinName;
            UnitDataModel data = __instance.model.UnitData.unitData;
            if (!string.IsNullOrWhiteSpace(data.workshopSkin))
            {
                skinName = new LorName(data.workshopSkin, true).Compress();
            }
            else
            {
                skinName = data.CustomBookItem.GetOriginalCharcterName();
                if (data.CustomBookItem.IsWorkshop && data.CustomBookItem.ClassInfo.skinType == "Custom" &&
                    !LorName.IsCompressed(skinName))
                {
                    skinName = new LorName(data.CustomBookItem.ClassInfo.id.packageId, skinName).Compress();
                }
            }
            __instance._skinInfo.skinName = skinName;
        }
    }
}
