using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace ExtendedLoader
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	public class SpriteCompressionConfig : Singleton<SpriteCompressionConfig>
	{
		[XmlElement("Disable")]
		public bool disableCompression = false;

		internal static void Load()
		{
			string path = Path.Combine(Application.persistentDataPath, "ModConfigs", "SpriteCompressionConfig.xml");
			try
			{
				if (File.Exists(path))
				{
					using (StreamReader streamReader = new StreamReader(path))
					{
						XmlSerializer xmlSerializer = new XmlSerializer(typeof(SpriteCompressionConfig));
						_instance = (SpriteCompressionConfig)xmlSerializer.Deserialize(streamReader);
						return;
					}
				}
			}
			catch { }

			Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "ModConfigs"));
			using (StreamWriter streamWriter = new StreamWriter(path))
			{
				new XmlSerializer(typeof(SpriteCompressionConfig)).Serialize(streamWriter, Instance);
			}
		}
	}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
