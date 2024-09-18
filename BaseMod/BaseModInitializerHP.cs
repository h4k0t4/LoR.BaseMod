using HarmonyLib;
using Mod;
using ModSettingTool;
using System;
using System.Reflection;

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
	public static class BaseModInitialize
	{
		public static void OnInitializeMod()
		{
			Harmony baseMod = new Harmony("LOR.BaseMod");
			//Initialize
			Harmony_Patch.Init();
			//RemoveWarnings
			MethodInfo method = typeof(BaseModInitialize).GetMethod("NoReferenceError", AccessTools.all);
			baseMod.Patch(typeof(EntryScene).GetMethod("CheckModError", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
			/*method = typeof(BaseModInitialize).GetMethod("ModContentManager_SaveSelectionData_Pre", AccessTools.all);
			baseMod.Patch(typeof(ModContentManager).GetMethod("SaveSelectionData", AccessTools.all), new HarmonyMethod(method), null, null, null, null);*/
			//Patch
			baseMod.PatchAll(typeof(Harmony_Patch));
			//Add BaseMod ver
			GlobalGameManager.Instance.ver = string.Join(Environment.NewLine,
				new string[] {
					GlobalGameManager.Instance.ver,
					"BaseMod for workshop 2.3 ver",
					//"(NIGHTLY BUILD)",
					//"(featuring Cya from the Toolbox series)",
				}
			);
			//Load OtherMod
			Harmony_Patch.LoadModFiles();
			Harmony_Patch.LoadAssemblyFiles();
			ModSaveTool.LoadFromSaveData();
			Harmony summonLiberation = new Harmony("LOR.SummonLiberation");
			summonLiberation.PatchAll(typeof(SummonLiberation.Harmony_Patch));
		}/*
		static void RemoveWarnings()
		{
			List<string> list = new List<string>();
			foreach (string errorLog in ModContentManager.Instance.GetErrorLogs())
			{
				if (errorLog.StartsWith("<color=yellow>") && errorLog.Contains("BaseMod"))
				{
					list.Add(errorLog);
				}
			}
			foreach (string errorLog in list)
			{
				ModContentManager.Instance.GetErrorLogs().Remove(errorLog);
			}
		}*/
		static void NoReferenceError()
		{
			ModContentManager.Instance._logs.RemoveAll(x => x.Contains("The same assembly name already exists"));
		}
	}
}