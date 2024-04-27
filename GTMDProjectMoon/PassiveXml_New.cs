using LOR_XML;
using System.Collections.Generic;
using System.Xml.Serialization;

//NewXml
namespace GTMDProjectMoon
{
	[XmlType("PassiveXmlRoot")]
	public class PassiveXmlRoot_V2 : XmlRoot
	{
		[XmlElement("Passive")]
		public List<PassiveXmlInfo_V2> list;

		[XmlIgnore]
		public static XmlAttributeOverrides Overrides
		{
			get
			{
				if (_overrides == null)
				{
					_overrides = new XmlAttributeOverrides();
					_overrides.Add(typeof(PassiveXmlInfo), nameof(PassiveXmlInfo.workshopID), new XmlAttributes { XmlIgnore = false, XmlAttribute = new XmlAttributeAttribute("Pid") });
				}
				return _overrides;
			}
		}
		static XmlAttributeOverrides _overrides;
	}
	public class PassiveXmlInfo_V2 : PassiveXmlInfo
	{
		[XmlElement("CopyInnerType")]
		public LorIdXml CopyInnerTypeXml = new LorIdXml("", -1);

		[XmlIgnore]
		public LorId CopyInnerType = LorId.None;

		[XmlElement("CustomInnerType")]
		public string CustomInnerType = "";
	}

	[XmlType("PassiveDescRoot")]
	public class PassiveDescRoot_V2 : XmlRoot
	{
		[XmlElement("PassiveDesc")]
		public List<PassiveDesc> descList;

		[XmlIgnore]
		public static XmlAttributeOverrides Overrides
		{
			get
			{
				if (_overrides == null)
				{
					_overrides = new XmlAttributeOverrides();
					_overrides.Add(typeof(PassiveDesc), nameof(PassiveDesc.workshopID), new XmlAttributes { XmlIgnore = false, XmlAttribute = new XmlAttributeAttribute("Pid") });
				}
				return _overrides;
			}
		}
		static XmlAttributeOverrides _overrides;
	}
}
