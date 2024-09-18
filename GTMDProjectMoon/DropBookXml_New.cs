using CustomInvitation;
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
		public static XmlSerializer Serializer
		{
			get
			{
				if (_serializer == null)
				{
					var overrides = new XmlAttributeOverrides();
					overrides.Add(typeof(CardDropTableXmlInfo), nameof(CardDropTableXmlInfo.workshopId), new XmlAttributes { XmlIgnore = false, XmlAttribute = new XmlAttributeAttribute("Pid") });
					_serializer = new XmlSerializer(typeof(CardDropTableXmlRoot_V2), overrides);
				}
				return _serializer;
			}
		}
		static XmlSerializer _serializer;
	}

	[XmlType("BookUseXmlRoot")]
	public class BookUseXmlRoot_V2 : XmlRoot
	{
		[XmlElement("BookUse")]
		public List<DropBookXmlInfo> bookXmlList;

		[XmlIgnore]
		public static XmlSerializer Serializer
		{
			get
			{
				if (_serializer == null)
				{
					var overrides = new XmlAttributeOverrides();
					overrides.Add(typeof(DropBookXmlInfo), nameof(DropBookXmlInfo.workshopID), new XmlAttributes { XmlIgnore = false, XmlAttribute = new XmlAttributeAttribute("Pid") });
					_serializer = new XmlSerializer(typeof(BookUseXmlRoot_V2), overrides);
				}
				return _serializer;
			}
		}
		static XmlSerializer _serializer;
	}
}
