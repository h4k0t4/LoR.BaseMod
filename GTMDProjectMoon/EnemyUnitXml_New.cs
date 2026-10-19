using BaseMod;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using BaseBridge;

//EnemyUnitXml_New for other mods' drop
namespace GTMDProjectMoon
{
	[XmlType("EnemyUnitClassRoot")]
	public class EnemyUnitClassRoot_V2 : XmlRoot
	{
		[XmlElement("Enemy")]
		public List<EnemyUnitClassInfo_V2> list;

		[XmlIgnore]
		public static XmlSerializer Serializer
		{
			get
			{
				if (_serializer == null)
				{
					var ignore = new XmlAttributes
					{
						XmlIgnore = true
					};
					var overrides = new XmlAttributeOverrides();
					overrides.Add(typeof(EnemyUnitClassInfo), nameof(EnemyUnitClassInfo.bookId), ignore);
					overrides.Add(typeof(EnemyUnitClassInfo), nameof(EnemyUnitClassInfo.dropTableList), ignore);
					overrides.Add(typeof(EnemyUnitClassInfo), nameof(EnemyUnitClassInfo.emotionCardList), ignore);
					overrides.Add(typeof(EnemyDropItemTable), nameof(EnemyDropItemTable.dropItemList), ignore);
					overrides.Add(typeof(EnemyUnitClassInfo), nameof(EnemyUnitClassInfo.workshopID), new XmlAttributes { XmlIgnore = false, XmlAttribute = new XmlAttributeAttribute("Pid") });
					_serializer = new XmlSerializer(typeof(EnemyUnitClassRoot_V2), overrides);
				}
				return _serializer;
			}
		}
		static XmlSerializer _serializer;
	}
	public class EnemyUnitClassInfo_V2 : EnemyUnitClassInfo
	{
		[XmlElement("BookId")]
		public List<LorIdXml> bookLorIdXml = new List<LorIdXml>();

		[XmlIgnore]
		public List<LorId> bookLorId = new List<LorId>();

		[XmlElement("DropTable")]
		public List<EnemyDropItemTable_V2> dropTableListNew = new List<EnemyDropItemTable_V2>();

		[XmlElement("Emotion")]
		public List<EmotionSetInfo_V2> emotionCardListNew = new List<EmotionSetInfo_V2>();

		public void InitOldFields(string uniqueId)
		{
			workshopID = uniqueId;
			height = RandomUtil.Range(minHeight, maxHeight);
			foreach (EnemyDropItemTable_V2 enemyDropItemTable_New in dropTableListNew)
			{
				foreach (EnemyDropItem_New dropNew in enemyDropItemTable_New.dropItemListNew)
				{
					dropNew.workshopId = Tools.ClarifyWorkshopId("", dropNew.workshopId, uniqueId);
				}
			}
			foreach (var emotion in emotionCardListNew)
			{
				emotion.WorkshopId = Tools.ClarifyWorkshopIdLegacy("", emotion.WorkshopId, uniqueId);
			}
			LorId.InitializeLorIds(bookLorIdXml, bookLorId, uniqueId);
			bookId = new List<int>();
		}

		public void InitInjectedFields()
		{
			foreach (var emotion in emotionCardListNew)
			{
				if (emotion.lorId.IsBasic())
				{
					emotionCardList.Add(emotion);
				}
				else if (BridgeManager.Instance.emotionCardBridge.GetEntityByKey(emotion.lorId, SephirahType.None) is EmotionCardXmlInfo injectedCard)
				{
					emotion.InjectId(injectedCard.id);
					emotionCardList.Add(emotion);
				}
			}
		}
	}
	public class EnemyDropItemTable_V2 : EnemyDropItemTable
	{
		[XmlElement("DropItem")]
		public List<EnemyDropItem_New> dropItemListNew;
	}
	public class EnemyDropItem_New
	{
		[XmlText]
		public int bookId;

		[XmlAttribute("Prob")]
		public float prob;

		[XmlAttribute("Pid")]
		public string workshopId = "";

		[XmlIgnore]
		public LorId bookLorId => new LorId(workshopId, bookId);
	}
	public class EmotionSetInfo_V2 : EmotionSetInfo
	{
		[XmlIgnore]
		public LorId lorId
		{
			get
			{
				return new LorId(WorkshopId, originalEmotionId ?? emotionId);
			}
		}

		[XmlIgnore]
		int? originalEmotionId = null;

		public void InjectId(int injectedId)
		{
			if (originalEmotionId == null)
			{
				originalEmotionId = emotionId;
				emotionId = injectedId;
			}
		}

		[XmlAttribute("Pid")]
		public string WorkshopId = "";
	}



	[Obsolete]
	public class EnemyUnitClassInfo_New
	{
		[XmlIgnore]
		public LorId id
		{
			get
			{
				return new LorId(this.workshopID, this._id);
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
	[Obsolete]
	public class EnemyDropItemTable_New
	{
		[XmlAttribute("Level")]
		public int emotionLevel;

		[XmlElement("DropItem")]
		public List<EnemyDropItem_New> dropItemList;

		[XmlIgnore]
		public List<EnemyDropItem_ReNew> dropList = new List<EnemyDropItem_ReNew>();
	}
	[Obsolete]
	public class EnemyDropItem_ReNew
	{
		public LorId bookId;

		public float prob;
	}
}
