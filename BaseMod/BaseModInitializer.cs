using GameSave;
using Mod;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Reflection;
using System;

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
		static void SaveSelection()
		{
			try
			{
				List<string> names = new List<string>();
				List<ModContentInfo> originList = ModContentManager.Instance._allMods;
				List<ModContentInfo> uexList = new List<ModContentInfo>();
				List<ModContentInfo> bmList = new List<ModContentInfo>();
				DirectoryInfo thisModDirectory = new FileInfo(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path)).Directory.Parent;
				originList.RemoveAll(modContentInfo =>
				{
					names.Add(modContentInfo.invInfo.workshopInfo.uniqueId);
					switch (modContentInfo.invInfo.workshopInfo.uniqueId)
					{
						case "BaseMod":
							if (modContentInfo.dirInfo == thisModDirectory)
							{
								bmList.Insert(0, modContentInfo);
							}
							else
							{
								bmList.Add(modContentInfo);
							}
							return true;
						case "UnityExplorer":
							if (modContentInfo.activated)
							{
								uexList.Insert(0, modContentInfo);
							}
							else
							{
								uexList.Add(modContentInfo);
							}
							return true;
						default: return false;
					}
				});
				uexList.AddRange(bmList);
				originList.InsertRange(0, uexList);
				int uexCount = 1;
				int bmCount = 1;
				names.RemoveAll(name => (name == "BaseMod" && ++bmCount > 0) || (name == "UnityExplorer" && ++uexCount > 0));
				List<string> priorityNames = new List<string>(bmCount + uexCount + names.Count);
				priorityNames.AddRange(Enumerable.Repeat("UnityExplorer", uexCount));
				priorityNames.AddRange(Enumerable.Repeat("BaseMod", bmCount));
				priorityNames.AddRange(names);
				names = priorityNames;
				SaveData modSettingData = new SaveData();
				SaveData modOrderData = new SaveData(SaveDataType.List);
				foreach (string name in names)
				{
					modOrderData.AddToList(new SaveData(name));
				}
				modSettingData.AddData(ModContentManager.save_orders, modOrderData);
				SaveData oldData = SaveManager.Instance.LoadData(ModContentManager.Instance.savePath);
				if (oldData != null && oldData.GetData(ModContentManager.save_lastActivated) != null)
				{
					modSettingData.AddData(ModContentManager.save_lastActivated, oldData.GetData(ModContentManager.save_lastActivated));
				}
				SaveManager.Instance.SaveData(ModContentManager.Instance.savePath, modSettingData);
			}
			catch { }
		}
		static void ClearReference()
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
		internal static readonly List<string> References = new List<string>()
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