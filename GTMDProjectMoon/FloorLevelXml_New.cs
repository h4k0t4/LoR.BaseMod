using BaseMod;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GTMDProjectMoon
{
	[XmlType("FloorLevelXmlRoot")]
	public class FloorLevelXmlRoot_V2 : XmlRoot
	{
		[XmlElement("FloorLevelXmlInfo")]
		public List<FloorLevelXmlInfo_V2> list;

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
					overrides.Add(typeof(FloorLevelXmlInfo), nameof(FloorLevelXmlInfo.sephirahType), ignore);
					overrides.Add(typeof(FloorLevelXmlInfo), nameof(FloorLevelXmlInfo.stageId), ignore);
					_serializer = new XmlSerializer(typeof(FloorLevelXmlRoot_V2), overrides);
				}
				return _serializer;
			}
		}
		static XmlSerializer _serializer;
	}
	public class FloorLevelXmlInfo_V2 : FloorLevelXmlInfo
	{
		[XmlElement("Sephirah")]
		public string sephirahName;

		[XmlElement("Stage")]
		public LorIdXml stageLorIdXml;

		[XmlIgnore]
		public LorId stageLorId;

		public void InitOldFields(string packageId)
		{
			sephirahType = string.IsNullOrWhiteSpace(sephirahName) ? SephirahType.None : Tools.MakeEnum<SephirahType>(sephirahName);
			stageLorIdXml.pid = Tools.ClarifyWorkshopIdLegacy("", stageLorIdXml.pid, packageId);
			stageLorId = LorId.MakeLorId(stageLorIdXml, "");
			if (stageLorId.IsBasic())
			{
				stageId = stageLorId.id;
			}
		}

		public FloorLevelXmlInfo_V2()
		{
			stageId = -1;
		}
	}
}
