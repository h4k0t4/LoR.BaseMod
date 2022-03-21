using System.Collections.Generic;
using System.Xml.Serialization;


namespace GTMDProjectMoon
{
    //BookXml_New for OnlyCard and SoulCard
    public class BookXmlRoot
    {
        [XmlElement("Version")]
        public string version = "1.1";

        [XmlElement("Book")]
        public List<BookXmlInfo_New> bookXmlList;
    }
    public class BookXmlInfo_New
    {
        [XmlAttribute("ID")]
        public int _id;

        [XmlIgnore]
        public bool isError;

        [XmlIgnore]
        public string workshopID = "";

        [XmlElement("Name")]
        public string InnerName = "";

        [XmlElement("TextId")]
        public int TextId = -1;

        [XmlElement("BookIcon")]
        public string _bookIcon = "";

        [XmlElement("Option")]
        public List<BookOption> optionList = new List<BookOption>();

        [XmlElement("Category")]
        public List<BookCategory> categoryList = new List<BookCategory>();

        [XmlElement("CustomCategory")]
        public List<string> customCategoryList = new List<string>();

        [XmlElement]
        public BookEquipEffect_New EquipEffect = new BookEquipEffect_New();

        [XmlElement("Rarity")]
        public Rarity Rarity;

        [XmlElement("CharacterSkin")]
        public List<string> CharacterSkin = new List<string>();

        [XmlElement("CharacterSkinType")]
        public string skinType = "";

        [XmlElement("SkinGender")]
        public Gender gender = Gender.N;

        [XmlElement("Chapter")]
        public int Chapter = 1;

        [XmlElement("Episode")]
        public LorIdXml episode = new LorIdXml("", -2);

        [XmlElement("RangeType")]
        public EquipRangeType RangeType;

        [XmlElement("NotEquip")]
        public bool canNotEquip;

        [XmlElement("RandomFace")]
        public bool RandomFace;

        [XmlElement("SpeedDiceNum")]
        public int speedDiceNumber = 1;

        [XmlElement("SuccessionPossibleNumber")]
        public int SuccessionPossibleNumber = 9;

        [XmlElement("SoundInfo")]
        public List<CustomInvitation.BookSoundInfo> motionSoundList;

        [XmlIgnore]
        public int remainRewardValue;
    }
    public class BookEquipEffect_New
    {
        [XmlElement("HpReduction")]
        public int HpReduction;

        [XmlElement("HP")]
        public int Hp;

        [XmlElement("DeadLine")]
        public int DeadLine;

        [XmlElement]
        public int Break;

        [XmlElement("SpeedMin")]
        public int SpeedMin;

        [XmlElement]
        public int Speed;

        [XmlElement]
        public int SpeedDiceNum;

        [XmlElement]
        public AtkResist SResist = AtkResist.Normal;

        [XmlElement]
        public AtkResist PResist = AtkResist.Normal;

        [XmlElement]
        public AtkResist HResist = AtkResist.Normal;

        [XmlElement]
        public AtkResist SBResist = AtkResist.Normal;

        [XmlElement]
        public AtkResist PBResist = AtkResist.Normal;

        [XmlElement]
        public AtkResist HBResist = AtkResist.Normal;

        public int MaxPlayPoint = 3;

        [XmlElement("StartPlayPoint")]
        public int StartPlayPoint = 3;

        [XmlElement("AddedStartDraw")]
        public int AddedStartDraw;

        [XmlIgnore]
        public int PassiveCost = 10;

        [XmlElement("OnlyCard")]
        public List<LorIdXml> OnlyCard = new List<LorIdXml>();

        [XmlElement("Card")]
        public List<BookSoulCardInfo_New> CardList = new List<BookSoulCardInfo_New>();

        [XmlElement("Passive")]
        public List<LorIdXml> _PassiveList = new List<LorIdXml>();

        [XmlIgnore]
        public List<LorId> PassiveList = new List<LorId>();
    }
    public class BookSoulCardInfo_New
    {
        [XmlText]
        public int cardId;

        [XmlAttribute("Pid")]
        public string WorkshopId = "";

        [XmlAttribute("Level")]
        public int requireLevel;

        [XmlAttribute("Emotion")]
        public int emotionLevel = 1;

    }
}
