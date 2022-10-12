using GameSave;
using Mod;
using System.Linq;
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
                List<string> names = new List<string>();
                List<ModContentInfo> originList = Singleton<ModContentManager>.Instance._allMods;
                foreach (ModContentInfo modContentInfo in originList.ToList())
                {
                    names.Add(modContentInfo.invInfo.workshopInfo.uniqueId);
                    if (modContentInfo.invInfo.workshopInfo.uniqueId == "BaseMod")
					{
                        originList.Remove(modContentInfo);
                        originList.Insert(0, modContentInfo);
					}
                }
                names.Remove("BaseMod");
                names.Insert(0, "BaseMod");
                names.Remove("UnityExplorer");
                names.Insert(0, "UnityExplorer");
                SaveData modSettingData = new SaveData();
                SaveData modOrderData = new SaveData(SaveDataType.List);
                foreach (string name in names)
                {
                    modOrderData.AddToList(new SaveData(name));
                }
                modSettingData.AddData("orders", modOrderData);
                SaveData oldData = Singleton<SaveManager>.Instance.LoadData(Singleton<ModContentManager>.Instance.savePath);
                if (oldData != null && oldData.GetData("lastActivated") != null)
                {
                    modSettingData.AddData("lastActivated", oldData.GetData("lastActivated"));
                }
                Singleton<SaveManager>.Instance.SaveData(Path.Combine(SaveManager.savePath, "ModSetting.save"), modSettingData);
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