using System.Collections.Generic;
using System.Xml.Serialization;

//NewXml
namespace GTMDProjectMoon
{
	[XmlType("DeckXmlRoot")]
	public class DeckXmlRoot_V2 : XmlRoot
	{
		[XmlElement("Deck")]
		public List<DeckXmlInfo> deckXmlList = new List<DeckXmlInfo>();

		[XmlIgnore]
		public static XmlSerializer Serializer
		{
			get
			{
				if (_serializer == null)
				{
					var overrides = new XmlAttributeOverrides();
					overrides.Add(typeof(DeckXmlInfo), nameof(DeckXmlInfo.workshopId), new XmlAttributes { XmlIgnore = false, XmlAttribute = new XmlAttributeAttribute("Pid") });
					_serializer = new XmlSerializer(typeof(DeckXmlRoot_V2), overrides);
				}
				return _serializer;
			}
		}
		static XmlSerializer _serializer;
	}
}
