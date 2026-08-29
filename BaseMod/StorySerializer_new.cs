using System;
using System.IO;
using UnityEngine;

namespace BaseMod
{
	public class StorySerializer_new
	{
		public static void StoryTextExport(string path, string outpath, string outname)
		{
			string text = Resources.Load<TextAsset>(path).text;
			Directory.CreateDirectory(outpath);
			File.WriteAllText(outpath + "/" + outname + ".txt", text);
		}
		public static void StoryTextExport_str(string str, string outpath, string outname)
		{
			Directory.CreateDirectory(outpath);
			File.WriteAllText(outpath + "/" + outname + ".txt", str);
		}
		static bool CheckStaticReExportLock()
		{
			return File.Exists(Harmony_Patch.StoryPath_Static + "/DeleteThisToExportStaticStoryAgain");
		}
		static void CreateStaticReExportLock()
		{
			File.WriteAllText(Harmony_Patch.StoryPath_Static + "/DeleteThisToExportStaticStoryAgain", "yes");
		}
		static bool CheckLocalizeReExportLock()
		{
			return File.Exists(Harmony_Patch.StoryPath_Localize + "/DeleteThisToExportLocalizeStoryAgain");
		}
		static void CreateLocalizeReExportLock()
		{
			File.WriteAllText(Harmony_Patch.StoryPath_Localize + "/DeleteThisToExportLocalizeStoryAgain", "yes");
		}
		public static void ExportStory()
		{
			try
			{
				if (!CheckStaticReExportLock())
				{
					TextAsset[] array = Resources.LoadAll<TextAsset>("Xml/Story/StoryEffect/");
					for (int i = 0; i < array.Length; i++)
					{
						StoryTextExport_str(array[i].text, Harmony_Patch.StoryPath_Static, array[i].name);
					}
					CreateStaticReExportLock();
				}
				if (!CheckLocalizeReExportLock())
				{
					TextAsset[] array2 = Resources.LoadAll<TextAsset>("Xml/Story/" + TextDataModel.CurrentLanguage);
					for (int j = 0; j < array2.Length; j++)
					{
						StoryTextExport_str(array2[j].text, Harmony_Patch.StoryPath_Localize, array2[j].name);
					}
					CreateLocalizeReExportLock();
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/SSLSerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
	}
}