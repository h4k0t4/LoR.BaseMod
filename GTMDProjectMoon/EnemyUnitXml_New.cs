using System.Collections.Generic;
using System.Xml.Serialization;

//EnemyUnitXml_New for other mods' drop
namespace GTMDProjectMoon
{
    public class EnemyUnitClassRoot
    {
        [XmlElement("Enemy")]
        public List<EnemyUnitClassInfo_New> list;
    }
    public class EnemyUnitClassInfo_New
    {
        [XmlIgnore]
        public LorId id
        {
            get
            {
                return new LorId(workshopID, _id);
            }
        }

        [XmlAttribute("ID")]
        public int _id;

        [XmlIgnore]
        public string workshopID = "";

        [XmlElement("Name")]
        public string name = string.Empty;

        [XmlElement("FaceType")]
        public UnitFaceType faceType;

        [XmlElement("NameID")]
        public int nameId;

        [XmlElement("MinHeight")]
        public int minHeight;

        [XmlElement("MaxHeight")]
        public int maxHeight;

        [XmlElement("Unknown")]
        public bool isUnknown;

        [XmlElement("Gender")]
        public Gender gender;

        [XmlElement("Retreat")]
        public bool retreat;

        [XmlIgnore]
        public int height;

        [XmlElement("BookId")]
        public List<int> bookId;

        [XmlElement("BodyId")]
        public int bodyId;

        [XmlElement("Exp")]
        public int exp;

        [XmlElement("DropBonus")]
        public float dropBonus;

        [XmlElement("DropTable")]
        public List<EnemyDropItemTable_New> dropTableList = new List<EnemyDropItemTable_New>();

        [XmlElement("Emotion")]
        public List<EmotionSetInfo> emotionCardList = new List<EmotionSetInfo>();

        [XmlElement("AiScript")]
        public string AiScript = "";
    }
    public class EnemyDropItemTable_New
    {
        [XmlAttribute("Level")]
        public int emotionLevel;

        [XmlElement("DropItem")]
        public List<EnemyDropItem_New> dropItemList;

        [XmlIgnore]
        public List<EnemyDropItem_ReNew> dropList = new List<EnemyDropItem_ReNew>();
    }
    public class EnemyDropItem_New
    {
        [XmlText]
        public int bookId;

        [XmlAttribute("Prob")]
        public float prob;

        [XmlAttribute("Pid")]
        public string workshopId = "";
    }
    public class EnemyDropItem_ReNew
    {
        public LorId bookId;

        public float prob;
    }
}
