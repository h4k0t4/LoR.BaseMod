using LOR_XML;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GTMDProjectMoon
{
	[XmlType("CharactersNameRoot")]
	public class CharactersNameRoot_V2 : XmlRoot
	{
		[XmlElement("Name")]
		public List<CharacterName_V2> nameList;
	}

	public class CharacterName_V2 : CharacterName
	{
		[XmlAttribute("Pid")]
		public string workshopId;

		[XmlIgnore]
		public LorId lorId => new LorId(workshopId, ID);
	}
}
