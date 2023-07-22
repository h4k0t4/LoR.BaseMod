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
		public static XmlAttributeOverrides Overrides
		{
			get
			{
				if (_overrides == null)
				{
					var ignore = new XmlAttributes
					{
						XmlIgnore = true
					};
					_overrides = new XmlAttributeOverrides();
					_overrides.Add(typeof(FloorLevelXmlInfo), nameof(FloorLevelXmlInfo.sephirahType), ignore);
					_overrides.Add(typeof(FloorLevelXmlInfo), nameof(FloorLevelXmlInfo.stageId), ignore);
				}
				return _overrides;
			}
		}
		static XmlAttributeOverrides _overrides;
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
