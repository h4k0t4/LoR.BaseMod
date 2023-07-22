using System.Collections.Generic;
using System.Xml.Serialization;

//NewXml
namespace GTMDProjectMoon
{
	[XmlType("CardDropTableXmlRoot")]
	public class CardDropTableXmlRoot_V2 : XmlRoot
	{
		[XmlElement("DropTable")]
		public List<CardDropTableXmlInfo> dropTableXmlList;

		[XmlIgnore]
		public static XmlAttributeOverrides Overrides
		{
			get
			{
				if (_overrides == null)
				{
					_overrides = new XmlAttributeOverrides();
					_overrides.Add(typeof(CardDropTableXmlInfo), nameof(CardDropTableXmlInfo.workshopId), new XmlAttributes { XmlIgnore = false, XmlAttribute = new XmlAttributeAttribute("Pid") });
				}
				return _overrides;
			}
		}
		static XmlAttributeOverrides _overrides;
	}

	[XmlType("BookUseXmlRoot")]
	public class BookUseXmlRoot_V2 : XmlRoot
	{
		[XmlElement("BookUse")]
		public List<DropBookXmlInfo> bookXmlList;

		[XmlIgnore]
		public static XmlAttributeOverrides Overrides
		{
			get
			{
				if (_overrides == null)
				{
					_overrides = new XmlAttributeOverrides();
					_overrides.Add(typeof(DropBookXmlInfo), nameof(DropBookXmlInfo.workshopID), new XmlAttributes { XmlIgnore = false, XmlAttribute = new XmlAttributeAttribute("Pid") });
				}
				return _overrides;
			}
		}
		static XmlAttributeOverrides _overrides;
	}
}
