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
        public static void ExportStory()
        {
            try
            {
                TextAsset[] array = Resources.LoadAll<TextAsset>("Xml/Story/StoryEffect/");
                for (int i = 0; i < array.Length; i++)
                {
                    StoryTextExport_str(array[i].text, Harmony_Patch.StoryPath_Static, array[i].name);
                }
                TextAsset[] array2 = Resources.LoadAll<TextAsset>("Xml/Story/" + TextDataModel.CurrentLanguage);
                for (int j = 0; j < array2.Length; j++)
                {
                    StoryTextExport_str(array2[j].text, Harmony_Patch.StoryPath_Localize, array2[j].name);
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/SSLSerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}