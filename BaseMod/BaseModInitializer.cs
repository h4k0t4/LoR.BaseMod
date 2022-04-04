using GameSave;
using Mod;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BaseMod
{
    public class BaseModInitializer : ModInitializer
    {
        public BaseModInitializer()
        {
            //Reference
            SaveSelection();
            ClearReference();
        }
        public override void OnInitializeMod()
        {
            BaseModInitialize.OnInitializeMod();
        }
        private static void SaveSelection()
        {
            try
            {
                List<string> list = new List<string>();
                foreach (ModContentInfo modContentInfo in Singleton<ModContentManager>.Instance.GetAllMods())
                {
                    list.Add(modContentInfo.invInfo.workshopInfo.uniqueId);
                }
                list.Remove("BaseMod");
                list.Insert(0, "BaseMod");
                list.Remove("UnityExplorer");
                list.Insert(0, "UnityExplorer");
                SaveData saveData = new SaveData();
                SaveData saveData2 = new SaveData(SaveDataType.List);
                foreach (string value in list)
                {
                    saveData2.AddToList(new SaveData(value));
                }
                saveData.AddData("orders", saveData2);
                SaveData oldData = Singleton<SaveManager>.Instance.LoadData(Singleton<ModContentManager>.Instance.savePath);
                if (oldData != null && oldData.GetData("lastActivated") != null)
                {
                    saveData.AddData("lastActivated", oldData.GetData("lastActivated"));
                }
                Singleton<SaveManager>.Instance.SaveData(Path.Combine(SaveManager.savePath, "ModSetting.save"), saveData);
            }
            catch { }
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