using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

namespace ExtendedLoader
{
	public class XLConfig : Singleton<XLConfig>
	{
		[XmlElement("LogSkinRenderErrors")]
		public bool logRenderErrors;

		internal static void Load()
		{
			string path = Path.Combine(Application.persistentDataPath, "ModConfigs", "ExtendedLoader.xml");
			try
			{
				if (File.Exists(path))
				{
					using (StreamReader streamReader = new StreamReader(path))
					{
						XmlSerializer xmlSerializer = new XmlSerializer(typeof(XLConfig));
						_instance = (XLConfig)xmlSerializer.Deserialize(streamReader);
						return;
					}
				}
			}
			catch { }

			Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "ModConfigs"));
			using (StreamWriter streamWriter = new StreamWriter(path))
			{
				new XmlSerializer(typeof(XLConfig)).Serialize(streamWriter, Instance);
			}
		}
	}
}
