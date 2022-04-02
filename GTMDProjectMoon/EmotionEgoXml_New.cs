using System.Collections.Generic;
using System.Xml.Serialization;

//EmotionEgoXml_New Custom Ego 
namespace GTMDProjectMoon
{
    public class EmotionEgoXmlRoot
    {
        [XmlElement("EmotionEgo")]
        public List<EmotionEgoXmlInfo_New> egoXmlList;
    }
    public class EmotionEgoXmlInfo_New
    {
        [XmlElement("ID")]
        public int id;

        [XmlElement("Sephirah")]
        public SephirahType Sephirah;

        [XmlElement("Card")]
        public LorIdXml _CardId;

        [XmlElement("LockInBattle")]
        public bool isLock;
    }
    public class EgoCardDetail
    {
        [XmlElement("ID")]
        public int id;

        [XmlElement("Sephirah")]
        public SephirahType Sephirah;

        [XmlElement("Card")]
        public LorIdXml _CardId;

        [XmlElement("LockInBattle")]
        public bool isLock;
    }
}
