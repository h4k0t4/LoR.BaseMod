using LOR_XML;
using System.Collections.Generic;
using System.Xml.Serialization;

//NewXml
namespace GTMDProjectMoon
{
    public class CardDropTableXmlRoot
    {
        [XmlElement("Pid")]
        public string workshopID;

        [XmlElement("DropTable")]
        public List<CardDropTableXmlInfo> dropTableXmlList;
    }
    public class BookUseXmlRoot
    {
        [XmlElement("Pid")]
        public string workshopID;

        [XmlElement("BookUse")]
        public List<DropBookXmlInfo> bookXmlList;
    }
}
