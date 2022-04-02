using LOR_DiceSystem;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GTMDProjectMoon
{
    public class DiceCardXmlRoot
    {
        [XmlElement("Card")]
        public List<DiceCardXmlInfo_New> cardXmlList;
    }

    public class DiceCardXmlInfo_New : DiceCardXmlInfo
    {
        [XmlElement("CustomCategory")]
        public string customCategory = "";
    }
}
