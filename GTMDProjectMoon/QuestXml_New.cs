using System.Collections.Generic;
using System.Xml.Serialization;

namespace GTMDProjectMoon
{
	[XmlType("QuestXmlRoot")]
	public class QuestXmlRoot_V2
	{
		[XmlElement("Quest")]
		public List<QuestXmlInfo_V2> list;

		[XmlIgnore]
		public static XmlSerializer Serializer
		{
			get
			{
				if (_serializer == null)
				{
					var ignore = new XmlAttributes
					{
						XmlIgnore = true
					};
					var overrides = new XmlAttributeOverrides();
					overrides.Add(typeof(QuestXmlInfo), nameof(QuestXmlInfo.sephirah), ignore);
					var missionOverride = new XmlAttributes();
					missionOverride.XmlElements.Add(new XmlElementAttribute("Mission", typeof(QuestMissionXmlInfo_V2)));
					overrides.Add(typeof(QuestXmlInfo), nameof(QuestXmlInfo.missionList), missionOverride);
					_serializer = new XmlSerializer(typeof(QuestXmlRoot_V2), overrides);
				}
				return _serializer;
			}
		}
		static XmlSerializer _serializer;
	}

	public class QuestXmlInfo_V2 : QuestXmlInfo
	{
		[XmlElement("Sephirah")]
		public string sephirahName;

		public void InitOldFields()
		{
			sephirah = string.IsNullOrWhiteSpace(sephirahName) ? SephirahType.None : BaseMod.Tools.MakeEnum<SephirahType>(sephirahName);
		}
	}

	public class QuestMissionXmlInfo_V2 : QuestMissionXmlInfo
	{
		[XmlAttribute("Script")]
		public string scriptName = string.Empty;
	}
}
