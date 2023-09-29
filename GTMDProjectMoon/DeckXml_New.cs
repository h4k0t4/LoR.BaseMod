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
		public static XmlAttributeOverrides Overrides
		{
			get
			{
				if (_overrides == null)
				{
					_overrides = new XmlAttributeOverrides();
					_overrides.Add(typeof(DeckXmlInfo), nameof(DeckXmlInfo.workshopId), new XmlAttributes { XmlIgnore = false, XmlAttribute = new XmlAttributeAttribute("Pid") });
				}
				return _overrides;
			}
		}
		static XmlAttributeOverrides _overrides;
	}
}
