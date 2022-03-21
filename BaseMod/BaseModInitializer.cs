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
            Harmony BaseMod = new Harmony("LOR.BaseMod");
            //Initialize
            Harmony_Patch.Init();
            //Patch
            BaseMod.PatchAll(typeof(Harmony_Patch));
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
            Harmony SummonLiberation = new Harmony("LOR.SummonLiberation");
            SummonLiberation.PatchAll(typeof(SummonLiberation.Harmony_Patch));
            Harmony ExtendedLoader = new Harmony("LOR.BaseMod.Cyaminthe.ExtendedLoader");
            ExtendedLoader.PatchAll(typeof(ExtendedLoader.AbCardSelectorSizePatch));
            ExtendedLoader.PatchAll(typeof(ExtendedLoader.CharacterNameInputInjector));
            ExtendedLoader.PatchAll(typeof(ExtendedLoader.CharacterNameOutputInjector));
            ExtendedLoader.PatchAll(typeof(ExtendedLoader.CustomAppearancePrefabPatch));
            ExtendedLoader.PatchAll(typeof(ExtendedLoader.CustomProjectionLoadPatch));
            //ExtendedLoader.PatchAll(typeof(ExtendedLoader.GenderInjector));
            ExtendedLoader.PatchAll(typeof(ExtendedLoader.GiftOrderPatch));
            ExtendedLoader.PatchAll(typeof(ExtendedLoader.ThumbPatch));
            ExtendedLoader.PatchAll(typeof(ExtendedLoader.VanillaSkinCopyPatch));
            ExtendedLoader.PatchAll(typeof(ExtendedLoader.WorkshopSetterPatch));
        }
        private static void RemoveWarnings()
        {
            List<string> list = new List<string>();
            foreach (string errorLog in Singleton<ModContentManager>.Instance.GetErrorLogs())
            {
                if (errorLog.StartsWith("<color=yellow>") && errorLog.Contains("BaseMod"))
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