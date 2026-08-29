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
		public static XmlSerializer Serializer
		{
			get
			{
				if (_serializer == null)
				{
					var overrides = new XmlAttributeOverrides();
					overrides.Add(typeof(PassiveXmlInfo), nameof(PassiveXmlInfo.workshopID), new XmlAttributes { XmlIgnore = false, XmlAttribute = new XmlAttributeAttribute("Pid") });
					_serializer = new XmlSerializer(typeof(PassiveXmlRoot_V2), overrides);
				}
				return _serializer;
			}
		}
		static XmlSerializer _serializer;
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
		public static XmlSerializer Serializer
		{
			get
			{
				if (_serializer == null)
				{
					var overrides = new XmlAttributeOverrides();
					overrides.Add(typeof(PassiveDesc), nameof(PassiveDesc.workshopID), new XmlAttributes { XmlIgnore = false, XmlAttribute = new XmlAttributeAttribute("Pid") });
					_serializer = new XmlSerializer(typeof(PassiveDescRoot_V2), overrides);
				}
				return _serializer;
			}
		}
		static XmlSerializer _serializer;
	}
}
