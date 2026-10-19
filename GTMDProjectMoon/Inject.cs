
/*
namespace GTMDProjectMoon
{
	public static class Utils
	{
		public static bool MakeBackup(string path)
		{
			try
			{
				if (File.Exists(path))
				{
					File.Copy(path, path + ".backup", true);
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
				return false;
			}
			return true;
		}
		public static bool MakeBackup(string[] arr)
		{
			try
			{
				foreach (string text in arr)
				{
					if (File.Exists(text))
					{
						File.Copy(text, text + ".backup_", true);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
				return false;
			}
			return true;
		}
		public static bool RestoreBackup(string path)
		{
			try
			{
				string text = path + ".backup_";
				if (File.Exists(text))
				{
					File.Copy(text, path, true);
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
				return false;
			}
			return true;
		}
		public static bool RestoreBackup(string[] arr)
		{
			try
			{
				foreach (string text in arr)
				{
					string text2 = text + ".backup_";
					if (File.Exists(text2))
					{
						File.Copy(text2, text, true);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
				return false;
			}
			return true;
		}
		public static bool DeleteBackup(string path)
		{
			try
			{
				string path2 = path + ".backup_";
				if (File.Exists(path2))
				{
					File.Delete(path2);
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
				return false;
			}
			return true;
		}
		public static bool DeleteBackup(string[] arr)
		{
			try
			{
				for (int i = 0; i < arr.Length; i++)
				{
					string path = arr[i] + ".backup_";
					if (File.Exists(path))
					{
						File.Delete(path);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
				return false;
			}
			return true;
		}
	}
}*/
/*
public static void InjectForRepair()
{
	Utils.MakeBackup(Application.dataPath + "/Managed/Assembly-CSharp.dll");
	File.Delete(Application.dataPath + "/Managed/Assembly-CSharp.dll");
	File.Copy(ModContentManager.Instance.GetModPath("BaseMod") + "/Sources/Assembly-CSharp.dll", Application.dataPath + "/Managed/Assembly-CSharp.dll");
	File.WriteAllText(Application.dataPath + "/Managed/AlreadyRepaira9.txt", "Success");
	try
	{
		File.Delete(Application.dataPath + "/Managed/AlreadyRepair.txt");
	}
	catch { }
}*/