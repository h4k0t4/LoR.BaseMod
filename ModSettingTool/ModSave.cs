using BaseMod;
using GameSave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UI;
using UnityEngine;

namespace ModSettingTool
{
	public static class ModSaveTool
	{
		//存档拉屎内容
		public static string ModSavePath
		{
			get
			{
				return Path.Combine(SaveManager.savePath, "ModSaveFiles.dat");
			}
		}
		public static void LoadFromSaveData()
		{
			try
			{
				if (!File.Exists(ModSavePath))
				{
					using (FileStream fileStream = File.Create(ModSavePath))
					{
						new BinaryFormatter().Serialize(fileStream, ModSaveData.GetSerializedData());
					}
					return;
				}
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				FileStream saveFileStream;
				FileStream serializationStream = saveFileStream = File.Open(ModSavePath, FileMode.Open);
				object obj;
				try
				{
					obj = binaryFormatter.Deserialize(serializationStream);
				}
				finally
				{
					if (saveFileStream != null)
					{
						((IDisposable)saveFileStream).Dispose();
					}
				}
				if (obj == null)
				{
					throw new Exception();
				}
				ModSaveData.LoadFromSerializedData(obj);
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/LoadModSaveerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		public static void SaveModSaveData()
		{
			try
			{
				using (FileStream fileStream = File.Create(ModSavePath))
				{
					new BinaryFormatter().Serialize(fileStream, ModSaveData.GetSerializedData());
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/ModSaveerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		public static void RemoveUnknownSaves()
		{
			List<string> removeSaves = new List<string>();
			foreach (KeyValuePair<string, SaveData> keyValuePair in ModSaveData.GetDictionarySelf())
			{
				if (!LoadedModsWorkshopId.Contains(keyValuePair.Key))
				{
					removeSaves.Add(keyValuePair.Key);
				}
			}
			if (removeSaves.Count > 0)
			{
				Tools.SetAlarmText("BaseMod_ReMoveUnloadSave", UIAlarmButtonType.YesNo, delegate (bool flag)
				{
					if (flag)
					{
						foreach (string id in removeSaves)
						{
							ModSaveData.GetDictionarySelf().Remove(id);
						}
					}
				}, null);
			}
		}
		public static SaveData GetModSaveData(string WorkshopId = "")
		{
			if (string.IsNullOrWhiteSpace(WorkshopId))
			{
				WorkshopId = Tools.GetModId(Assembly.GetCallingAssembly());
			}
			if (string.IsNullOrWhiteSpace(WorkshopId))
			{
				return null;
			}
			if (ModSaveData.GetData(WorkshopId) == null)
			{
				ModSaveData.AddData(WorkshopId, new SaveData(SaveDataType.Dictionary));
			}
			return ModSaveData.GetData(WorkshopId);
		}
		public static void SaveString(string name, string value, string WorkshopId = "")
		{
			if (string.IsNullOrWhiteSpace(WorkshopId))
			{
				WorkshopId = Tools.GetModId(Assembly.GetCallingAssembly());
			}
			if (string.IsNullOrWhiteSpace(WorkshopId))
			{
				return;
			}
			GetModSaveData(WorkshopId).GetDictionarySelf()[name] = new SaveData(value);
		}
		public static void Saveint(string name, int value, string WorkshopId = "")
		{
			if (string.IsNullOrWhiteSpace(WorkshopId))
			{
				WorkshopId = Tools.GetModId(Assembly.GetCallingAssembly());
			}
			if (string.IsNullOrWhiteSpace(WorkshopId))
			{
				return;
			}
			GetModSaveData(WorkshopId).GetDictionarySelf()[name] = new SaveData(value);
		}
		public static void Saveulong(string name, ulong value, string WorkshopId = "")
		{
			if (string.IsNullOrWhiteSpace(WorkshopId))
			{
				WorkshopId = Tools.GetModId(Assembly.GetCallingAssembly());
			}
			if (string.IsNullOrWhiteSpace(WorkshopId))
			{
				return;
			}
			GetModSaveData(WorkshopId).GetDictionarySelf()[name] = new SaveData(value);
		}
		public static HashSet<string> LoadedModsWorkshopId = new HashSet<string>();

		public static SaveData ModSaveData = new SaveData(SaveDataType.Dictionary);
	}
}