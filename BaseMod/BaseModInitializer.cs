using GameSave;
using HarmonyLib;
using Mod;
using ModSettingTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

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
            MethodInfo method = typeof(BaseModInitializer).GetMethod("ModContentManager_SaveSelectionData_Pre", AccessTools.all);
            baseMod.Patch(typeof(ModContentManager).GetMethod("SaveSelectionData", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
            method = typeof(BaseModInitializer).GetMethod("NotReferenceError", AccessTools.all);
            baseMod.Patch(typeof(EntryScene).GetMethod("CheckModError", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
            ClearReference();
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
            //RemoveWarnings();
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
        private static void ClearReference()
        {
            foreach (string name in References)
            {
                string path = Application.dataPath + "/Managed/" + name + ".dll";
                if (File.Exists(path))
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch { }
                }
            }
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
        private static bool ModContentManager_SaveSelectionData_Pre(List<ModContentInfo> allMods, List<ModContentInfo> activateds)
        {
            try
            {
                List<string> list = new List<string>();
                List<string> list2 = new List<string>();
                foreach (ModContentInfo modContentInfo in allMods)
                {
                    list.Add(modContentInfo.invInfo.workshopInfo.uniqueId);
                }
                foreach (ModContentInfo modContentInfo2 in activateds)
                {
                    list2.Add(modContentInfo2.invInfo.workshopInfo.uniqueId);
                }
                list.Remove("BaseMod");
                list.Insert(0, "BaseMod");
                list.Remove("UnityExplorer");
                list.Insert(0, "UnityExplorer");
                list2.Remove("BaseMod");
                list2.Insert(0, "BaseMod");
                list2.Remove("UnityExplorer");
                list2.Insert(0, "UnityExplorer");
                SaveData saveData = new SaveData();
                SaveData saveData2 = new SaveData(SaveDataType.List);
                SaveData saveData3 = new SaveData(SaveDataType.List);
                foreach (string value in list)
                {
                    saveData2.AddToList(new SaveData(value));
                }
                foreach (string value2 in list2)
                {
                    saveData3.AddToList(new SaveData(value2));
                }
                saveData.AddData("orders", saveData2);
                saveData.AddData("lastActivated", saveData3);
                Singleton<SaveManager>.Instance.SaveData(Path.Combine(SaveManager.savePath, "ModSetting.save"), saveData);
                return false;
            }
            catch { }
            return true;
        }
        private static void NotReferenceError()
        {
            Singleton<ModContentManager>.Instance._logs = Singleton<ModContentManager>.Instance._logs.FindAll((string x) => !x.Contains("The same assembly name already exists"));
        }
        private static readonly List<string> References = new List<string>()
        {
            "0Harmony",
            "Mono.Cecil",
            "Mono.Cecil.Mdb",
            "Mono.Cecil.Pdb",
            "Mono.Cecil.Rocks",
            "MonoMod.Common",
            "MonoMod.RuntimeDetour",
            "MonoMod.Utils",
        };
    }
}