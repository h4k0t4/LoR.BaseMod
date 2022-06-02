using System.Collections.Generic;
using System.Xml.Serialization;


namespace GTMDProjectMoon
{
    //BookXml_New for OnlyCard and SoulCard
    public class BookXmlRoot
    {
        [XmlElement("Pid")]
        public string workshopID;

        [XmlElement("Book")]
        public List<BookXmlInfo_New> bookXmlList;
    }
    public class BookXmlInfo_New : BookXmlInfo
    {
        [XmlElement("CustomCategory")]
        public List<string> customCategoryList = new List<string>();

        [XmlElement("EquipEffect")]
        public new BookEquipEffect_New EquipEffect = new BookEquipEffect_New();

        [XmlElement("Episode")]
        public new LorIdXml episode = new LorIdXml("", -2);

        [XmlIgnore]
        public LorId LorEpisode = new LorId(-1);
    }
    public class BookEquipEffect_New : BookEquipEffect
    {
        [XmlElement("OnlyCard")]
        public new List<LorIdXml> OnlyCard = new List<LorIdXml>();

        [XmlIgnore]
        public List<LorId> OnlyCards = new List<LorId>();

        [XmlElement("SoulCard")]
        public List<BookSoulCardInfo_New> SoulCardList = new List<BookSoulCardInfo_New>();
    }
    public class BookSoulCardInfo_New : BookSoulCardInfo
    {
        [XmlIgnore]
        public LorId id
        {
            get
            {
                return new LorId(WorkshopId, cardId);
            }
        }

        [XmlAttribute("Pid")]
        public string WorkshopId = "";
    }
}
