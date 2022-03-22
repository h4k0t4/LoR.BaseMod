using HarmonyLib;
using Mod;
using ModSettingTool;
using System;
using System.Collections.Generic;

namespace BaseMod
{
    public enum PatchType
    {
        prefix,
        postfix
    }
}

namespace BaseMod
{
    public class BaseModInitializer : ModInitializer
    {
        public override void OnInitializeMod()
        {
            Harmony baseMod = new Harmony("LOR.BaseMod");
            //Initialize
            Harmony_Patch.Init();
            //Patch
            baseMod.PatchAll(typeof(Harmony_Patch));
            //Add BaseMod ver
            GlobalGameManager.Instance.ver = string.Concat(new string[]
            {
            GlobalGameManager.Instance.ver,
            Environment.NewLine,
            "BaseMod_New 2.12 ver",
            Environment.NewLine,
            "by Boss An's bug rasing floor"
            });
            //RemoveWarnings
            RemoveWarnings();
            //Load OtherMod
            Harmony_Patch.LoadModFiles();
            Harmony_Patch.LoadAssemblyFiles();
            ModSaveTool.LoadFromSaveData();
            Harmony summonLiberation = new Harmony("LOR.SummonLiberation");
            SummonLiberation.Harmony_Patch.Init();
            summonLiberation.PatchAll(typeof(SummonLiberation.Harmony_Patch));
            Harmony extendedLoader = new Harmony("LOR.BaseMod.Cyaminthe.ExtendedLoader");
            extendedLoader.PatchAll(typeof(ExtendedLoader.AbCardSelectorSizePatch));
            extendedLoader.PatchAll(typeof(ExtendedLoader.CharacterNameInputInjector));
            extendedLoader.PatchAll(typeof(ExtendedLoader.CharacterNameOutputInjector));
            extendedLoader.PatchAll(typeof(ExtendedLoader.CustomAppearancePrefabPatch));
            extendedLoader.PatchAll(typeof(ExtendedLoader.CustomProjectionLoadPatch));
            extendedLoader.PatchAll(typeof(ExtendedLoader.ExtendedClothCustomizeData));
            extendedLoader.PatchAll(typeof(ExtendedLoader.GenderInjector));
            extendedLoader.PatchAll(typeof(ExtendedLoader.GiftOrderPatch));
            extendedLoader.PatchAll(typeof(ExtendedLoader.ThumbPatch));
            extendedLoader.PatchAll(typeof(ExtendedLoader.VanillaSkinCopyPatch));
            extendedLoader.PatchAll(typeof(ExtendedLoader.WorkshopSetterPatch));
        }
        private static void RemoveWarnings()
        {
            List<string> list = new List<string>();
            foreach (string errorLog in Singleton<ModContentManager>.Instance.GetErrorLogs())
            {
                if (/*errorLog.StartsWith("<color=yellow>") &&*/  errorLog.Contains("BaseMod"))
                {
                    list.Add(errorLog);
                }
            }
            foreach (string errorLog in list)
            {
                Singleton<ModContentManager>.Instance.GetErrorLogs().Remove(errorLog);
            }
        }
    }
}