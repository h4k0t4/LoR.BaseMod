using LOR_XML;
using System.Collections.Generic;
using System.Xml.Serialization;

//NewXml
namespace GTMDProjectMoon
{
	public class DeckXmlRoot
	{
		[XmlElement("Pid")]
		public string workshopID = "";

		[XmlElement("Deck")]
		public List<DeckXmlInfo> deckXmlList = new List<DeckXmlInfo>();
	}
}
