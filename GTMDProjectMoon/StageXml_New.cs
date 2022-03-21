using System.Collections.Generic;
using System.Xml.Serialization;

//StageXml_New for condition and invivation
namespace GTMDProjectMoon
{
    public class StageXmlRoot
    {
        [XmlElement("Stage")]
        public List<StageClassInfo_New> list;
    }
    public class StageClassInfo_New
    {
        [XmlAttribute("id")]
        public int _id;

        [XmlIgnore]
        public string workshopID = "";

        [XmlElement("Wave")]
        public List<StageWaveInfo> waveList = new List<StageWaveInfo>();

        [XmlElement("StageType")]
        public StageType stageType;

        [XmlElement("MapInfo")]
        public List<string> mapInfo = new List<string>();

        [XmlElement("FloorNum")]
        public int floorNum = 1;

        [XmlElement("Name")]
        public string stageName;

        [XmlElement("Chapter")]
        public int chapter;

        [XmlElement("Invitation")]
        public StageInvitationInfo invitationInfo = new StageInvitationInfo();

        [XmlElement("Condition")]
        public StageExtraCondition_New extraCondition = new StageExtraCondition_New();

        [XmlElement("Story")]
        public List<StageStoryInfo> storyList = new List<StageStoryInfo>();

        [XmlElement("IsChapterLast")]
        public bool isChapterLast;

        [XmlElement("StoryType")]
        public string _storyType;

        [XmlElement("invitationtype")]
        public bool isStageFixedNormal;

        [XmlElement("FloorOnly")]
        public List<SephirahType> floorOnlyList = new List<SephirahType>();

        [XmlElement("ExceptFloor")]
        public List<SephirahType> exceptFloorList = new List<SephirahType>();

        [XmlElement("RewardItems")]
        public List<BookDropItemXmlInfo> _rewardList = new List<BookDropItemXmlInfo>();

        [XmlIgnore]
        public List<BookDropItemInfo> rewardList = new List<BookDropItemInfo>();
    }
    public class StageExtraCondition_New
    {
        [XmlElement("Stage")]
        public List<LorIdXml> needClearStageList = new List<LorIdXml>();

        [XmlIgnore]
        public List<LorId> stagecondition = new List<LorId>();

        [XmlElement("Level")]
        public int needLevel;
    }
}
