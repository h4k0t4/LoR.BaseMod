using LOR_XML;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

//BattleDialogXml_New for battledialog
namespace GTMDProjectMoon
{
	[XmlType("BattleDialogRoot")]
	public class BattleDialogRoot_V2 : XmlRoot
	{
		[XmlElement("GroupName")]
		public string groupName;

		[XmlElement("Character")]
		public List<BattleDialogCharacter_V2> characterList = new List<BattleDialogCharacter_V2>();

		[XmlIgnore]
		public static XmlSerializer Serializer
		{
			get
			{
				if (_serializer == null)
				{
					var overrides = new XmlAttributeOverrides();
					overrides.Add(typeof(BattleDialogCharacter), nameof(BattleDialogCharacter.workshopId), new XmlAttributes { XmlIgnore = false, XmlAttribute = new XmlAttributeAttribute("Pid") });
					_serializer = new XmlSerializer(typeof(BattleDialogRoot_V2), overrides);
				}
				return _serializer;
			}
		}
		static XmlSerializer _serializer;
	}
	public class BattleDialogCharacter_V2 : BattleDialogCharacter
	{
		[XmlAttribute("Name")]
		public string characterName = "";
	}
	[XmlType("BattleDialogRelationRoot")]
	public class BattleDialogRelationRoot_V2 : XmlRoot 
	{
		[XmlElement("Relation")]
		public List<BattleDialogRelationWithBookID_V2> list;
	}
	public class BattleDialogRelationWithBookID_V2 : BattleDialogRelationWithBookID
	{
		[XmlAttribute("Pid")]
		public string workshopId = "";

		[XmlIgnore]
		public LorId bookLorId => new LorId(workshopId, bookID);
	}


	[Obsolete]
	public class BattleDialogRoot
	{
		[XmlElement("GroupName")]
		public string groupName;

		[XmlElement("Character")]
		public List<BattleDialogCharacter_New> characterList = new List<BattleDialogCharacter_New>();
	}
	[Obsolete]
	public class BattleDialogCharacter_New
	{
		[XmlIgnore]
		public LorId id
		{
			get
			{
				return new LorId(this.workshopId, this.bookId);
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
	[Obsolete]
	public class BattleDialogRelationRoot
	{
		public BattleDialogRelationRoot()
		{
		}

		[XmlElement("Relation")]
		public List<BattleDialogRelationWithBookID_New> list;
	}
	[Obsolete]
	public class BattleDialogRelationWithBookID_New
	{
		public BattleDialogRelationWithBookID_New()
		{
		}

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
