using Mod;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace GTMDProjectMoon
{
	public class BasemodConfig
	{

		[XmlElement]
		public string DefaultLocale = "default";

		[XmlElement]
		public bool IgnoreLocalize = false;

		[XmlElement]
		public bool IgnoreStory = false;

		[XmlElement]
		public bool IgnoreStaticFiles = false;

		[XmlIgnore]
		public static readonly BasemodConfig BasemodDefault = new BasemodConfig();

		[XmlIgnore]
		public static readonly BasemodConfig NonBasemodDefault = new BasemodConfig() { IgnoreLocalize = true, IgnoreStory = true, IgnoreStaticFiles = true };

		[XmlIgnore]
		public static readonly Dictionary<string, BasemodConfig> LoadedConfigs = new Dictionary<string, BasemodConfig>();
		public static BasemodConfig FindBasemodConfig(string modId)
		{
			if (string.IsNullOrEmpty(modId))
			{
				return NonBasemodDefault;
			}
			if (LoadedConfigs.TryGetValue(modId, out var config) && config != null)
			{
				return LoadedConfigs[modId];
			}
			var modPath = ModContentManager.Instance.GetModPath(modId);
			if (string.IsNullOrEmpty(modPath))
			{
				return null;
			}
			var path = Path.Combine(modPath, "BasemodConfig.xml");
			config = null;
			if (File.Exists(path))
			{
				try
				{
					using (StringReader stringReader = new StringReader(File.ReadAllText(path)))
					{
						config = new XmlSerializer(typeof(BasemodConfig)).Deserialize(stringReader) as BasemodConfig;
					}
				}
				catch { }
			}
			if (config == null)
			{
				if (CheckBasemodLoading(modPath))
				{
					config = BasemodDefault;
				}
				else
				{
					config = NonBasemodDefault;
				}
				/*
				try
				{
					XmlSerializer serializer = new XmlSerializer(typeof(BasemodConfig));
					using (StreamWriter writer = new StreamWriter(path))
					{
						serializer.Serialize(writer, config);
					}
				}
				catch { }
				*/
			}
			LoadedConfigs[modId] = config;
			return config;
		}
		//improve later
		static bool CheckBasemodLoading(string modFolder)
		{
			return true;
		}
	}
}
