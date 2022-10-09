using LOR_DiceSystem;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GTMDProjectMoon
{
    public class DiceCardXmlRoot
    {
        [XmlElement("Pid")]
        public string workshopID;

        [XmlElement("Card")]
        public List<DiceCardXmlInfo_New> cardXmlList = new List<DiceCardXmlInfo_New>();
    }

    public class DiceCardXmlInfo_New : DiceCardXmlInfo
    {
        [XmlIgnore]
        public List<BookCategory> categoryList = new List<BookCategory>();

        [XmlElement("CustomCategory")]
        public List<string> customCategoryList = new List<string>();
    }
}
