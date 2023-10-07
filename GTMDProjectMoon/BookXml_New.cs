using BaseMod;
using CustomInvitation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace GTMDProjectMoon
{
	//BookXml_New for OnlyCard and SoulCard
	[XmlType("BookXmlRoot")]
	public class BookXmlRoot_V2 : XmlRoot
	{
		[XmlElement("Book")]
		public List<BookXmlInfo_V2> bookXmlList = new List<BookXmlInfo_V2>();

		[XmlIgnore]
		public static XmlAttributeOverrides Overrides
		{
			get
			{
				if (_overrides == null)
				{
					var ignore = new XmlAttributes
					{
						XmlIgnore = true
					};
					_overrides = new XmlAttributeOverrides();
					_overrides.Add(typeof(BookEquipEffect), nameof(BookEquipEffect.OnlyCard), ignore);
					_overrides.Add(typeof(BookEquipEffect), nameof(BookEquipEffect.CardList), ignore);
					_overrides.Add(typeof(BookXmlInfo), nameof(BookXmlInfo.optionList), ignore);
					_overrides.Add(typeof(BookXmlInfo), nameof(BookXmlInfo.categoryList), ignore);
					_overrides.Add(typeof(BookXmlInfo), nameof(BookXmlInfo.EquipEffect), ignore);
					_overrides.Add(typeof(BookXmlInfo), nameof(BookXmlInfo.episode), ignore);
					_overrides.Add(typeof(BookXmlInfo), nameof(BookXmlInfo.skinType), ignore);
					_overrides.Add(typeof(BookXmlInfo), nameof(BookXmlInfo.workshopID), new XmlAttributes { XmlIgnore = false, XmlAttribute = new XmlAttributeAttribute("Pid") });
#pragma warning disable CS0618 // Type or member is obsolete
					var unignore = new XmlAttributes
					{
						XmlIgnore = false
					};
					unignore.XmlElements.Add(new XmlElementAttribute("CustomCategory"));
					_overrides.Add(typeof(BookXmlInfo_V2), nameof(BookXmlInfo_V2.customCategoryListFallback), unignore);
#pragma warning restore CS0618 // Type or member is obsolete
				}
				return _overrides;
			}
		}
		static XmlAttributeOverrides _overrides;
	}
	public class BookXmlInfo_V2 : BookXmlInfo
	{
		[XmlElement("Option")]
		public List<string> customOptionList = new List<string>();

		[XmlElement("Category")]
		public List<string> customCategoryList = new List<string>();

		[XmlElement("CustomCategory")]
		[Obsolete("Just use the Category tag", false)]
		public List<string> customCategoryListFallback;

		[XmlElement("EquipEffect")]
		public BookEquipEffect_V2 EquipEffectNew = new BookEquipEffect_V2();

		[XmlElement("Episode")]
		public LorIdXml episodeXml = new LorIdXml("", -2);

		[XmlIgnore]
		public LorId LorEpisode = LorId.None;

		[XmlElement("SkinType")]
		public string newSkinType = "";

		[XmlElement("CharacterSkinType")]
		public string legacySkinType = "";

		public void InitOldFields(string packageId)
		{
			workshopID = packageId;
			//EquipEffect
			EquipEffectNew.InitOldFields(packageId);
			EquipEffect = EquipEffectNew;
			if (EquipEffectNew.OnlyCards.Count > 0)
			{
				OrcTools.OnlyCardDic[id] = EquipEffectNew.OnlyCards;
			}
			if (EquipEffectNew.CardList.Count > 0)
			{
				OrcTools.SoulCardDic[id] = EquipEffectNew.CardList;
			}
			//Episode
			if (episodeXml.xmlId > 0)
			{
				LorEpisode = LorId.MakeLorId(episodeXml, packageId);
			}
			episode = LorEpisode.id;
			//SkinType
			if (!string.IsNullOrWhiteSpace(legacySkinType))
			{
				newSkinType = legacySkinType;
			}
			if (!string.IsNullOrWhiteSpace(newSkinType))
			{
				if (newSkinType == "UNKNOWN")
				{
					newSkinType = "Lor";
				}
				else if (newSkinType == "CUSTOM")
				{
					newSkinType = "Custom";
				}
				else if (newSkinType == "LOR")
				{
					newSkinType = "Lor";
				}
			}
			else if (CharacterSkin[0].StartsWith("Custom"))
			{
				newSkinType = "Custom";
			}
			else
			{
				newSkinType = "Lor";
			}
			skinType = newSkinType;

			optionList = customOptionList?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => Tools.MakeEnum<BookOption>(x)).ToList() ?? new List<BookOption>();
			categoryList = customCategoryList?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => Tools.MakeEnum<BookCategory>(x)).ToList();
			if (categoryList == null || categoryList.Count == 0)
			{
#pragma warning disable CS0618 // Type or member is obsolete
				categoryList = customCategoryListFallback?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => Tools.MakeEnum<BookCategory>(x)).ToList() ?? new List<BookCategory>();
#pragma warning restore CS0618 // Type or member is obsolete
			}
		}
	}
	public class BookEquipEffect_V2 : BookEquipEffect
	{
		[XmlElement("OnlyCard")]
		public new List<LorIdXml> OnlyCard = new List<LorIdXml>();

		[XmlIgnore]
		public List<LorId> OnlyCards = new List<LorId>();

		[XmlElement("SoulCard")]
		public new List<BookSoulCardInfo_New> CardList = new List<BookSoulCardInfo_New>();

		public void InitOldFields(string packageId)
		{
			//PassiveList
			LorId.InitializeLorIds(_PassiveList, PassiveList, packageId);
			//OnlyCard
			LorId.InitializeLorIds(OnlyCard, OnlyCards, packageId);
			foreach (LorId onlyCard in OnlyCards)
			{
				if (onlyCard.IsBasic())
				{
					base.OnlyCard.Add(onlyCard.id);
				}
			}
			//SoulCard
			foreach (var soulCardInfo_New in CardList)
			{
				soulCardInfo_New.WorkshopId = Tools.ClarifyWorkshopId("", soulCardInfo_New.WorkshopId, packageId);
			}
		}
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

		[XmlIgnore]
		public LorId lorId => new LorId(WorkshopId, cardId);
	}

	[XmlType("BookDescRoot")]
	public class BookDescRoot_V2 : XmlRoot
	{
		[XmlArrayItem("BookDesc")]
		public List<BookDesc_V2> bookDescList;
	}
	public class BookDesc_V2 : LOR_XML.BookDesc
	{
		[XmlAttribute("Pid")]
		public string workshopId;

		[XmlIgnore]
		public LorId lorId => new LorId(workshopId, bookID);
	}


	[Obsolete]
	public class BookXmlRoot
	{
		[XmlElement("Version")]
		public string version = "1.1";

		[XmlElement("Book")]
		public List<BookXmlInfo_New> bookXmlList;
	}
	[Obsolete]
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
		public List<global::BookOption> optionList = new List<global::BookOption>();

		[XmlElement("Category")]
		public List<global::BookCategory> categoryList = new List<global::BookCategory>();

		[XmlElement("CustomCategory")]
		public List<string> customCategoryList = new List<string>();

		[XmlElement]
		public BookEquipEffect_New EquipEffect = new BookEquipEffect_New();

		[XmlElement("Rarity")]
		public global::Rarity Rarity;

		[XmlElement("CharacterSkin")]
		public List<string> CharacterSkin = new List<string>();

		[XmlElement("CharacterSkinType")]
		public string skinType = "";

		[XmlElement("SkinGender")]
		public global::Gender gender = global::Gender.N;

		[XmlElement("Chapter")]
		public int Chapter = 1;

		[XmlElement("Episode")]
		public LorIdXml episode = new LorIdXml("", -2);

		[XmlElement("RangeType")]
		public global::EquipRangeType RangeType;

		[XmlElement("NotEquip")]
		public bool canNotEquip;

		[XmlElement("RandomFace")]
		public bool RandomFace;

		[XmlElement("SpeedDiceNum")]
		public int speedDiceNumber = 1;

		[XmlElement("SuccessionPossibleNumber")]
		public int SuccessionPossibleNumber = 9;

		[XmlElement("SoundInfo")]
		public List<BookSoundInfo> motionSoundList;

		[XmlIgnore]
		public int remainRewardValue;
	}
	[Obsolete]
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
}
