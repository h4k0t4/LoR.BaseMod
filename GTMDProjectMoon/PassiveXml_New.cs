using LOR_XML;
using System.Collections.Generic;
using System.Xml.Serialization;

//NewXml
namespace GTMDProjectMoon
{
	public class PassiveXmlRoot
	{
		[XmlElement("Pid")]
		public string workshopID = "";

		[XmlElement("Passive")]
		public List<PassiveXmlInfo_New> list;
	}
	public class PassiveXmlInfo_New : PassiveXmlInfo
	{
		[XmlElement("CustomInnerType")]
		public string CustomInnerType = "";
	}
}
