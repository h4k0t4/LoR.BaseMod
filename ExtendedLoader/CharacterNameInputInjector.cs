using HarmonyLib;
using System;
using Workshop;

namespace ExtendedLoader
{
    public class CharacterNameInputInjector
    {
        [HarmonyPatch(typeof(CustomizingResourceLoader), nameof(CustomizingResourceLoader.GetWorkshopSkinData), new Type[] { typeof(string) })]
        [HarmonyPostfix]
        static void GetWorkshopSkinDataPostfix(ref WorkshopSkinData __result, string id)
        {
            if (__result == null && LorName.IsCompressed(id))
            {
                __result = Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData(new LorName(id), "");
            }
        }

        [HarmonyPatch(typeof(CustomizingBookSkinLoader), nameof(CustomizingBookSkinLoader.GetWorkshopBookSkinData), new Type[] { typeof(string), typeof(string) })]
        [HarmonyPostfix]
        static void GetWorkshopBookSkinDataPostfix(CustomizingBookSkinLoader __instance, ref WorkshopSkinData __result, string id, string name)
        {
            if (__result == null)
            {
                if (LorName.IsCompressed(name))
                {
                    __result = __instance.GetWorkshopBookSkinData(new LorName(name), "");
                }
                else if (id == LorName.BASEMOD_ID)
                {
                    __result = Singleton<CustomizingResourceLoader>.Instance.GetWorkshopSkinData(name);
                }
            }
        }
    }
}
