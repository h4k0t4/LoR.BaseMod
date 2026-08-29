using BaseBridge;
using BaseMod;
using LorIdExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

//StageXml_New for condition and invivation
namespace GTMDProjectMoon
{
	[XmlType("StageXmlRoot")]
	public class StageXmlRoot_V2 : XmlRoot
	{
		[XmlElement("Stage")]
		public List<StageClassInfo_V2> list;

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
					overrides.Add(typeof(StageClassInfo), nameof(StageClassInfo.floorOnlyList), ignore);
					overrides.Add(typeof(StageClassInfo), nameof(StageClassInfo.exceptFloorList), ignore);
					overrides.Add(typeof(StageClassInfo), nameof(StageClassInfo.workshopID), new XmlAttributes { XmlIgnore = false, XmlAttribute = new XmlAttributeAttribute("Pid") });
					var waveOverride = new XmlAttributes();
					waveOverride.XmlElements.Add(new XmlElementAttribute("Wave", typeof(StageWaveInfo_V2)));
					overrides.Add(typeof(StageClassInfo), nameof(StageClassInfo.waveList), waveOverride);
					var condOverride = new XmlAttributes();
					condOverride.XmlElements.Add(new XmlElementAttribute("Condition", typeof(StageExtraCondition_V2)));
					overrides.Add(typeof(StageClassInfo), nameof(StageClassInfo.extraCondition), condOverride);
					overrides.Add(typeof(StageWaveInfo), nameof(StageWaveInfo.formationId), ignore);
					overrides.Add(typeof(StageWaveInfo), nameof(StageWaveInfo.emotionCardList), ignore);
					overrides.Add(typeof(StageExtraCondition), nameof(StageExtraCondition.needClearStageList), ignore);
					overrides.Add(typeof(StageStoryInfo), nameof(StageStoryInfo.packageId), new XmlAttributes { XmlIgnore = false, XmlAttribute = new XmlAttributeAttribute("Pid") });
					overrides.Add(typeof(StageStoryInfo), nameof(StageStoryInfo.chapter), new XmlAttributes { XmlIgnore = false, XmlAttribute = new XmlAttributeAttribute("Chapter") });
					overrides.Add(typeof(StageStoryInfo), nameof(StageStoryInfo.group), new XmlAttributes { XmlIgnore = false, XmlAttribute = new XmlAttributeAttribute("Group") });
					overrides.Add(typeof(StageStoryInfo), nameof(StageStoryInfo.episode), new XmlAttributes { XmlIgnore = false, XmlAttribute = new XmlAttributeAttribute("Episode") });
					_serializer = new XmlSerializer(typeof(StageXmlRoot_V2), overrides);
				}
				return _serializer;
			}
		}
		static XmlSerializer _serializer;
	}
	public class StageClassInfo_V2 : StageClassInfo
	{
		public StageClassInfo_V2()
		{
			extraCondition = new StageExtraCondition_V2();
		}

		[XmlElement("FloorOnly")]
		public List<string> floorOnlyNamesList = new List<string>();

		[XmlElement("ExceptFloor")]
		public List<string> exceptFloorNamesList = new List<string>();

		public void InitOldFields(string uniqueId)
		{
			floorOnlyList = floorOnlyNamesList.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => Tools.MakeEnum<SephirahType>(s)).ToList();
			exceptFloorList = exceptFloorNamesList.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => Tools.MakeEnum<SephirahType>(s)).ToList();
			(extraCondition as StageExtraCondition_V2)?.InitOldFields(uniqueId);
			foreach (var wave in waveList)
			{
				(wave as StageWaveInfo_V2)?.InitOldFields(uniqueId);
			}
			foreach (var story in storyList)
			{
				story.packageId = Tools.ClarifyWorkshopId(story.packageId, workshopID, uniqueId);
				if (string.IsNullOrWhiteSpace(story.packageId) && !string.IsNullOrWhiteSpace(story.story) && story.episode == 0 && story.group == 0 && story.chapter == 0)
				{
					story.chapter = chapter;
					story.group = 1;
					story.episode = _id;
				}
				story.valid = true;
			}
			if (invitationInfo.combine == StageCombineType.BookRecipe)
			{
				invitationInfo.needsBooks.Sort();
				invitationInfo.needsBooks.Reverse();
			}
		}

		public void InitInjectedFields()
		{
			foreach (var wave in waveList)
			{
				(wave as StageWaveInfo_V2)?.InitInjectedFields();
			}
		}
	}
	public class StageExtraCondition_V2 : StageExtraCondition
	{
		[XmlElement("Stage")]
		public List<LorIdXml> needClearStageListNew = new List<LorIdXml>();

		[XmlIgnore]
		public List<LorId> stagecondition = new List<LorId>();

		public void InitOldFields(string uniqueId)
		{
			LorId.InitializeLorIds(needClearStageListNew, stagecondition, uniqueId);
			foreach (LorId lorId in stagecondition)
			{
				if (lorId.IsBasic())
				{
					needClearStageList.Add(lorId.id);
				}
			}
			OrcTools.StageConditionDic[this] = stagecondition;
		}
	}
	public class StageWaveInfo_V2 : StageWaveInfo
	{
		[XmlElement("Formation")]
		public LorIdXml formationLorIdXml = new LorIdXml("", 1);

		[XmlArray("emotionCardList")]
		[XmlArrayItem("int")]
		public List<LorIdXml> emotionCardListXml = new List<LorIdXml>();

		[XmlIgnore]
		public LorId formationLorId;

		[XmlIgnore]
		public List<LorId> emotionCardListNew = new List<LorId>();

		public string uniqueId = "";

		public void InitOldFields(string uniqueId)
		{
			this.uniqueId = uniqueId;
		}

		public void InitInjectedFields()
		{
			formationLorId = LorIdLegacy.MakeLorIdLegacy(formationLorIdXml, uniqueId);
			if (formationLorId.IsBasic())
			{
				formationId = formationLorId.id;
			}
			else if (BridgeManager.Instance.formationBridge.GetEntityByKey(formationLorId) is FormationXmlInfo formation)
			{
				formationId = formation.id;
			}
			LorIdLegacy.InitializeLorIdsLegacy(emotionCardListXml, emotionCardListNew, uniqueId);
			foreach (var emotionNew in emotionCardListNew)
			{
				if (emotionNew.IsBasic())
				{
					emotionCardList.Add(emotionNew.id);
				}
				else if (BridgeManager.Instance.emotionCardBridge.GetEntityByKey(emotionNew, SephirahType.None) is EmotionCardXmlInfo injectedCard)
				{
					emotionCardList.Add(injectedCard.id);
				}
			}
		}
	}



	[Obsolete]
	public class StageXmlRoot
	{
		[XmlElement("Stage")]
		public List<StageClassInfo_New> list;
	}
	[Obsolete]
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
	[Obsolete]
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
