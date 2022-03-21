using LOR_XML;
using System.Collections.Generic;
using System.Xml.Serialization;

//BattleDialogXml_New for battledialog
namespace GTMDProjectMoon
{
    public class BattleDialogRoot
    {
        [XmlElement("GroupName")]
        public string groupName;

        [XmlElement("Character")]
        public List<BattleDialogCharacter_New> characterList = new List<BattleDialogCharacter_New>();
    }
    public class BattleDialogCharacter_New
    {
        [XmlIgnore]
        public LorId id
        {
            get
            {
                return new LorId(workshopId, bookId);
            }
        }

        [XmlAttribute("Name")]
        public string characterName = "";

        [XmlAttribute("ID")]
        public string characterID;

        [XmlElement("Type")]
        public List<BattleDialogType> dialogTypeList = new List<BattleDialogType>();

        [XmlIgnore]
        public string workshopId = "";

        [XmlIgnore]
        public int bookId;
    }
    public class BattleDialogRelationRoot
    {
        [XmlElement("Relation")]
        public List<BattleDialogRelationWithBookID_New> list;
    }
    public class BattleDialogRelationWithBookID_New
    {
        [XmlAttribute("Pid")]
        public string workshopId = "";

        [XmlAttribute("BookID")]
        public int bookID;

        [XmlAttribute("StoryID")]
        public int storyID;

        [XmlElement("GroupName")]
        public string groupName;

        [XmlElement("CharacterID")]
        public string characterID;
    }
}
