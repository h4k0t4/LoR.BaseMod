using LOR_DiceSystem;
using LOR_XML;
using System;
using System.Collections.Generic;
using HarmonyLib;
using BaseMod;

namespace GTMDProjectMoon
{
	public static class OrcTools
	{
		[Obsolete]
		public static StageClassInfo CopyStageClassInfo(this StageClassInfo stageClassInfo, StageClassInfo_New newinfo, string uniqueId = "")
		{
			List<int> needClearStageList = new List<int>();
			if (newinfo.extraCondition is StageExtraCondition_New extraNew)
			{
				foreach (LorId lorId in extraNew.stagecondition)
				{
					needClearStageList.Add(lorId.id);
				}
				StageConditionDic[stageClassInfo.extraCondition] = extraNew.stagecondition;
			}
			stageClassInfo._id = newinfo._id;
			stageClassInfo.workshopID = newinfo.workshopID;
			stageClassInfo.waveList = newinfo.waveList;
			stageClassInfo.stageType = newinfo.stageType;
			stageClassInfo.mapInfo = newinfo.mapInfo;
			stageClassInfo.floorNum = newinfo.floorNum;
			stageClassInfo.stageName = newinfo.stageName;
			stageClassInfo.chapter = newinfo.chapter;
			stageClassInfo.invitationInfo = newinfo.invitationInfo;
			stageClassInfo.extraCondition = new StageExtraCondition()
			{
				needClearStageList = needClearStageList,
				needLevel = newinfo.extraCondition.needLevel
			};
			stageClassInfo.storyList = newinfo.storyList;
			stageClassInfo.isChapterLast = newinfo.isChapterLast;
			stageClassInfo._storyType = newinfo._storyType;
			stageClassInfo.isStageFixedNormal = newinfo.isStageFixedNormal;
			stageClassInfo.floorOnlyList = newinfo.floorOnlyList;
			stageClassInfo.exceptFloorList = newinfo.exceptFloorList;
			stageClassInfo._rewardList = newinfo._rewardList;
			stageClassInfo.rewardList = newinfo.rewardList;
			return stageClassInfo;
		}

		[Obsolete]
		public static EnemyUnitClassInfo CopyEnemyUnitClassInfo(this EnemyUnitClassInfo enemyUnitClassInfo, EnemyUnitClassInfo_New newinfo, string uniqueId = "")
		{
			List<EnemyDropItemTable> enemyDropItemTables = new List<EnemyDropItemTable>();
			foreach (EnemyDropItemTable_New enemyDropItemTable_New in newinfo.dropTableList)
			{
				List<EnemyDropItem> enemyDropItems = new List<EnemyDropItem>();
				foreach (EnemyDropItem_New enemyDropItem_New in enemyDropItemTable_New.dropItemList)
				{
					enemyDropItems.Add(new EnemyDropItem()
					{
						prob = enemyDropItem_New.prob,
						bookId = enemyDropItem_New.bookId
					});
				}
				enemyDropItemTables.Add(new EnemyDropItemTable()
				{
					emotionLevel = enemyDropItemTable_New.emotionLevel,
					dropItemList = enemyDropItems
				});
			}
			enemyUnitClassInfo.AiScript = newinfo.AiScript;
			enemyUnitClassInfo.bodyId = newinfo.bodyId;
			enemyUnitClassInfo.bookId = newinfo.bookId;
			enemyUnitClassInfo.dropBonus = newinfo.dropBonus;
			enemyUnitClassInfo.dropTableList = enemyDropItemTables;
			enemyUnitClassInfo.emotionCardList = newinfo.emotionCardList;
			enemyUnitClassInfo.exp = newinfo.exp;
			enemyUnitClassInfo.faceType = newinfo.faceType;
			enemyUnitClassInfo.gender = newinfo.gender;
			enemyUnitClassInfo.height = newinfo.height;
			enemyUnitClassInfo.isUnknown = newinfo.isUnknown;
			enemyUnitClassInfo.maxHeight = newinfo.maxHeight;
			enemyUnitClassInfo.minHeight = newinfo.minHeight;
			enemyUnitClassInfo.name = newinfo.name;
			enemyUnitClassInfo.nameId = newinfo.nameId;
			enemyUnitClassInfo.retreat = newinfo.retreat;
			enemyUnitClassInfo.workshopID = newinfo.workshopID;
			enemyUnitClassInfo._id = newinfo._id;
			DropItemDic[new LorId(uniqueId, newinfo._id)] = newinfo.dropTableList;
			return enemyUnitClassInfo;
		}
		[Obsolete]
		public static LOR_XML.BattleDialogRoot CopyBattleDialogRoot(this LOR_XML.BattleDialogRoot battleDialogRoot, BattleDialogRoot newinfo, string uniqueId = "")
		{
			battleDialogRoot.characterList = new List<BattleDialogCharacter>();
			foreach (BattleDialogCharacter_New battleDialogCharacter_New in newinfo.characterList)
			{
				int dialogbookId = -1;
				try
				{
					dialogbookId = int.Parse(battleDialogCharacter_New.characterID);
				}
				catch { }
				battleDialogCharacter_New.workshopId = uniqueId;
				battleDialogCharacter_New.bookId = dialogbookId;
				battleDialogRoot.groupName = newinfo.groupName;
				battleDialogRoot.characterList.Add(new BattleDialogCharacter()
				{
					characterID = battleDialogCharacter_New.characterID,
					dialogTypeList = battleDialogCharacter_New.dialogTypeList,
					workshopId = uniqueId,
					bookId = dialogbookId,
				});
				if (string.IsNullOrWhiteSpace(battleDialogCharacter_New.characterName))
				{
					battleDialogCharacter_New.characterName = "";
				}
				if (dialogbookId == -1)
				{
					continue;
				}
				DialogDetail dialogDetail = DialogDetail.FindDialogInCharacterID(new LorId(uniqueId, dialogbookId));
				if (dialogDetail == null)
				{
					dialogDetail = new DialogDetail
					{
						GroupName = newinfo.groupName,
						BookId = new LorId(uniqueId, dialogbookId),
						CharacterID = new LorId(uniqueId, dialogbookId),
						CharacterName = battleDialogCharacter_New.characterName,
					};
					dialogDetails.Add(dialogDetail);
				}
				else
				{
					dialogDetail.CharacterName = battleDialogCharacter_New.characterName;
				}
			}
			return battleDialogRoot;
		}
		public static LOR_XML.BattleDialogRoot CopyBattleDialogRootNew(this BattleDialogRoot_V2 newinfo, string uniqueId = "")
		{
			var battleDialogRoot = new LOR_XML.BattleDialogRoot();
			battleDialogRoot.characterList = new List<BattleDialogCharacter>();
			battleDialogRoot.groupName = newinfo.groupName;
			foreach (var charNew in newinfo.characterList)
			{
				int dialogbookId = -1;
				try
				{
					dialogbookId = int.Parse(charNew.characterID);
				}
				catch { }
				var workshopId = Tools.ClarifyWorkshopId(charNew.workshopId, newinfo.customPid, uniqueId);
				charNew.workshopId = workshopId;
				charNew.bookId = dialogbookId;
				battleDialogRoot.characterList.Add(new BattleDialogCharacter()
				{
					characterID = charNew.characterID,
					dialogTypeList = charNew.dialogTypeList,
					workshopId = workshopId,
					bookId = dialogbookId,
				});
				charNew.characterName = charNew.characterName?.Trim() ?? string.Empty;
				if (dialogbookId == -1)
				{
					continue;
				}
				DialogDetail dialogDetail = DialogDetail.FindDialogInCharacterID(new LorId(uniqueId, dialogbookId));
				if (dialogDetail == null)
				{
					dialogDetail = new DialogDetail
					{
						GroupName = newinfo.groupName,
						BookId = new LorId(uniqueId, dialogbookId),
						CharacterID = new LorId(uniqueId, dialogbookId),
						CharacterName = charNew.characterName,
					};
					dialogDetails.Add(dialogDetail);
				}
				else
				{
					dialogDetail.CharacterName = charNew.characterName;
				}
			}
			return battleDialogRoot;
		}
		[Obsolete]
		public static List<BattleDialogRelationWithBookID> CopyBattleDialogRelation(this List<BattleDialogRelationWithBookID> BattleDialogRelation, List<BattleDialogRelationWithBookID_New> newinfo, string uniqueId = "")
		{
			foreach (BattleDialogRelationWithBookID_New battleDialogRelationWithBookID_New in newinfo)
			{
				if (string.IsNullOrWhiteSpace(battleDialogRelationWithBookID_New.workshopId))
				{
					battleDialogRelationWithBookID_New.workshopId = uniqueId;
				}
				if (battleDialogRelationWithBookID_New.workshopId.ToLower() == "@origin")
				{
					battleDialogRelationWithBookID_New.workshopId = "";
				}
				int characterId = -1;
				try
				{
					characterId = int.Parse(battleDialogRelationWithBookID_New.characterID);
				}
				catch { }
				BattleDialogRelation.Add(new BattleDialogRelationWithBookID
				{
					bookID = battleDialogRelationWithBookID_New.bookID,
					storyID = battleDialogRelationWithBookID_New.storyID,
					groupName = battleDialogRelationWithBookID_New.groupName,
					characterID = battleDialogRelationWithBookID_New.characterID,
				});
				if (string.IsNullOrWhiteSpace(battleDialogRelationWithBookID_New.groupName))
				{
					battleDialogRelationWithBookID_New.groupName = "";
				}
				if (characterId == -1)
				{
					continue;
				}
				DialogDetail dialogDetail = DialogDetail.FindDialogInBookID(new LorId(battleDialogRelationWithBookID_New.workshopId, battleDialogRelationWithBookID_New.bookID));
				if (dialogDetail == null)
				{
					dialogDetail = new DialogDetail
					{
						GroupName = battleDialogRelationWithBookID_New.groupName,
						BookId = new LorId(battleDialogRelationWithBookID_New.workshopId, battleDialogRelationWithBookID_New.bookID),
						CharacterID = new LorId(battleDialogRelationWithBookID_New.workshopId, characterId),
					};
					dialogDetails.Add(dialogDetail);
				}
				else
				{
					dialogDetail.GroupName = battleDialogRelationWithBookID_New.groupName;
					dialogDetail.CharacterID = new LorId(battleDialogRelationWithBookID_New.workshopId, characterId);
				}
			}
			return BattleDialogRelation;
		}
		public static List<BattleDialogRelationWithBookID> CopyBattleDialogRelationNew(this BattleDialogRelationRoot_V2 newinfo, string uniqueId = "")
		{
			var result = new List<BattleDialogRelationWithBookID>();
			foreach (var relationNew in newinfo.list)
			{
				relationNew.workshopId = Tools.ClarifyWorkshopId(relationNew.workshopId, newinfo.customPid, uniqueId);
				int characterId = -1;
				try
				{
					characterId = int.Parse(relationNew.characterID);
				}
				catch { }
				result.Add(new BattleDialogRelationWithBookID
				{
					bookID = relationNew.bookID,
					storyID = relationNew.storyID,
					groupName = relationNew.groupName,
					characterID = relationNew.characterID,
				});
				relationNew.groupName = relationNew.groupName?.Trim() ?? string.Empty;
				if (characterId == -1)
				{
					continue;
				}
				DialogDetail dialogDetail = DialogDetail.FindDialogInBookID(relationNew.bookLorId);
				if (dialogDetail == null)
				{
					dialogDetail = new DialogDetail
					{
						GroupName = relationNew.groupName,
						BookId = relationNew.bookLorId,
						CharacterID = new LorId(relationNew.workshopId, characterId),
					};
					dialogDetails.Add(dialogDetail);
				}
				else
				{
					dialogDetail.GroupName = relationNew.groupName;
					dialogDetail.CharacterID = new LorId(relationNew.workshopId, characterId);
				}
			}
			return result;
		}
		[Obsolete]
		public static BookXmlInfo CopyBookXmlInfo(this BookXmlInfo bookXml, BookXmlInfo_New newinfo, string uniqueId = "")
		{
			List<int> list = new List<int>();
			List<BookSoulCardInfo> list2 = new List<BookSoulCardInfo>();
			foreach (LorIdXml lorIdXml in newinfo.EquipEffect.OnlyCard)
			{
				list.Add(lorIdXml.xmlId);
			}
			foreach (BookSoulCardInfo_New bookSoulCardInfo_New in newinfo.EquipEffect.CardList)
			{
				bookSoulCardInfo_New.WorkshopId = Tools.ClarifyWorkshopId("", bookSoulCardInfo_New.WorkshopId, uniqueId);
				list2.Add(new BookSoulCardInfo
				{
					cardId = bookSoulCardInfo_New.cardId,
					requireLevel = bookSoulCardInfo_New.requireLevel,
					emotionLevel = bookSoulCardInfo_New.emotionLevel
				});
			}
			bookXml._id = newinfo._id;
			bookXml.isError = newinfo.isError;
			bookXml.workshopID = newinfo.workshopID;
			bookXml.InnerName = newinfo.InnerName;
			bookXml.TextId = newinfo.TextId;
			bookXml._bookIcon = newinfo._bookIcon;
			bookXml.optionList = newinfo.optionList;
			bookXml.categoryList = newinfo.categoryList;
			foreach (string category in newinfo.customCategoryList)
			{
				bookXml.categoryList.Add(GetBookCategory(category));
			}
			newinfo.customCategoryList.Clear();
			bookXml.EquipEffect = new BookEquipEffect
			{
				HpReduction = newinfo.EquipEffect.HpReduction,
				Hp = newinfo.EquipEffect.Hp,
				DeadLine = newinfo.EquipEffect.DeadLine,
				Break = newinfo.EquipEffect.Break,
				SpeedMin = newinfo.EquipEffect.SpeedMin,
				Speed = newinfo.EquipEffect.Speed,
				SpeedDiceNum = newinfo.EquipEffect.SpeedDiceNum,
				SResist = newinfo.EquipEffect.SResist,
				PResist = newinfo.EquipEffect.PResist,
				HResist = newinfo.EquipEffect.HResist,
				SBResist = newinfo.EquipEffect.SBResist,
				PBResist = newinfo.EquipEffect.PBResist,
				HBResist = newinfo.EquipEffect.HBResist,
				MaxPlayPoint = newinfo.EquipEffect.MaxPlayPoint,
				StartPlayPoint = newinfo.EquipEffect.StartPlayPoint,
				AddedStartDraw = newinfo.EquipEffect.AddedStartDraw,
				PassiveCost = newinfo.EquipEffect.PassiveCost,
				OnlyCard = list,
				CardList = list2,
				_PassiveList = newinfo.EquipEffect._PassiveList,
				PassiveList = newinfo.EquipEffect.PassiveList
			};
			bookXml.Rarity = newinfo.Rarity;
			bookXml.CharacterSkin = newinfo.CharacterSkin;
			bookXml.skinType = newinfo.skinType;
			bookXml.gender = newinfo.gender;
			bookXml.Chapter = newinfo.Chapter;
			try
			{
				bookXml.episode = newinfo.episode.xmlId;
			}
			catch
			{
				bookXml.episode = -1;
			}
			bookXml.RangeType = newinfo.RangeType;
			bookXml.canNotEquip = newinfo.canNotEquip;
			bookXml.RandomFace = newinfo.RandomFace;
			bookXml.speedDiceNumber = newinfo.speedDiceNumber;
			bookXml.SuccessionPossibleNumber = newinfo.SuccessionPossibleNumber;
			bookXml.motionSoundList = newinfo.motionSoundList;
			bookXml.remainRewardValue = newinfo.remainRewardValue;
			LorId key = new LorId(uniqueId, newinfo._id);
			List<LorId> onlyCards = new List<LorId>();
			LorId.InitializeLorIds(newinfo.EquipEffect.OnlyCard, onlyCards, uniqueId);
			OnlyCardDic[key] = onlyCards;
			SoulCardDic[key] = newinfo.EquipEffect.CardList;
			if (newinfo.episode.xmlId > 0)
			{
				EpisodeDic[key] = LorId.MakeLorId(newinfo.episode, uniqueId);
			}
			return bookXml;
		}
		[Obsolete]
		public static DiceCardXmlInfo CopyDiceCardXmlInfo(this DiceCardXmlInfo info, DiceCardXmlInfo_New newinfo)
		{
			info.Artwork = newinfo.Artwork;
			info.category = (string.IsNullOrWhiteSpace(newinfo.customCategory) ? newinfo.category : GetBookCategory(newinfo.customCategory));
			info.Chapter = newinfo.Chapter;
			info.DiceBehaviourList = newinfo.DiceBehaviourList;
			info.EgoMaxCooltimeValue = newinfo.EgoMaxCooltimeValue;
			info.Keywords = newinfo.Keywords;
			info.MapChange = newinfo.MapChange;
			info.MaxNum = newinfo.MaxNum;
			info.optionList = newinfo.optionList;
			info.Priority = newinfo.Priority;
			info.PriorityScript = newinfo.PriorityScript;
			info.Rarity = newinfo.Rarity;
			info.Script = newinfo.Script;
			info.ScriptDesc = newinfo.ScriptDesc;
			info.SkinChange = newinfo.SkinChange;
			info.SkinChangeType = newinfo.SkinChangeType;
			info.SkinHeight = newinfo.SkinHeight;
			info.Spec = newinfo.Spec;
			info.SpecialEffect = newinfo.SpecialEffect;
			info.workshopName = newinfo.workshopName;
			info._id = newinfo._id;
			info._textId = newinfo._textId;
			return info;
		}
		public static BookCategory GetBookCategory(string category)
		{
			return string.IsNullOrWhiteSpace(category) ? BookCategory.None : Tools.MakeEnum<BookCategory>(category);
		}
		public static BookOption GetBookOption(string option)
		{
			return string.IsNullOrWhiteSpace(option) ? BookOption.None : Tools.MakeEnum<BookOption>(option);
		}
		public static CardOption GetCardOption(string option)
		{
			return string.IsNullOrWhiteSpace(option) ? Tools.MakeEnum<CardOption>("None") : Tools.MakeEnum<CardOption>(option);
		}
		public class DialogDetail
		{
			public static DialogDetail FindDialogInBookID(LorId id)
			{
				if (id == null)
				{
					return null;
				}
				foreach (DialogDetail dialogDetail in dialogDetails)
				{
					if (dialogDetail.BookId == id)
					{
						return dialogDetail;
					}
				}
				return null;
			}
			public static DialogDetail FindDialogInCharacterName(string name)
			{
				if (string.IsNullOrWhiteSpace(name))
				{
					return null;
				}
				name = name.Trim();
				foreach (DialogDetail dialogDetail in dialogDetails)
				{
					if (dialogDetail.CharacterName == name)
					{
						return dialogDetail;
					}
				}
				return null;
			}
			public static DialogDetail FindDialogInGroupName(string name)
			{
				if (string.IsNullOrWhiteSpace(name))
				{
					return null;
				}
				name = name.Trim();
				foreach (DialogDetail dialogDetail in dialogDetails)
				{
					if (dialogDetail.GroupName == name)
					{
						return dialogDetail;
					}
				}
				return null;
			}
			public static DialogDetail FindDialogInCharacterID(LorId id)
			{
				if (id == null)
				{
					return null;
				}
				foreach (DialogDetail dialogDetail in dialogDetails)
				{
					if (dialogDetail.CharacterID == id)
					{
						return dialogDetail;
					}
				}
				return null;
			}

			public string CharacterName = "";

			public string GroupName = "";

			public LorId CharacterID = LorId.None;

			public LorId BookId = LorId.None;
		}

		public static readonly List<DialogDetail> dialogDetails = new List<DialogDetail>();

		public static Dictionary<LorId, List<LorId>> OnlyCardDic = new Dictionary<LorId, List<LorId>>();

		public static Dictionary<LorId, List<BookSoulCardInfo_New>> SoulCardDic = new Dictionary<LorId, List<BookSoulCardInfo_New>>();

		public static readonly Dictionary<LorId, string> StageNameDic = new Dictionary<LorId, string>();

		public static readonly Dictionary<StageExtraCondition, List<LorId>> StageConditionDic = new Dictionary<StageExtraCondition, List<LorId>>();

		public static readonly Dictionary<LorId, string> CharacterNameDic = new Dictionary<LorId, string>();

		public static readonly Dictionary<EmotionEgoXmlInfo, LorId> EgoDic = new Dictionary<EmotionEgoXmlInfo, LorId>();
		/*
		//the following mechanism improves performance by not having patches iterate through all values of EgoDic every time
		//at the same time it preserves backwards compatibility with possible modifications of EgoDic by other mods
		static int egoDicCheckedVersion = 0;
		internal static void CheckReverseEgoDic()
		{
			var curVersion = Traverse.Create(EgoDic).Field<int>("version").Value;
			if (curVersion != egoDicCheckedVersion)
			{
				egoDicCheckedVersion = curVersion;
				ReverseEgoDic.Clear();
				foreach (var kvp in EgoDic)
				{
					if (!ReverseEgoDic.ContainsKey(kvp.Value))
					{
						ReverseEgoDic.Add(kvp.Value, kvp.Key);
					}
				}
			}
		}
		internal static Dictionary<LorId, EmotionEgoXmlInfo> ReverseEgoDic = new Dictionary<LorId, EmotionEgoXmlInfo>();
		*/

		[Obsolete]
		public static readonly Dictionary<LorId, List<EnemyDropItemTable_New>> DropItemDic = new Dictionary<LorId, List<EnemyDropItemTable_New>>();

		public static readonly Dictionary<LorId, List<EnemyDropItemTable_V2>> DropItemDicV2 = new Dictionary<LorId, List<EnemyDropItemTable_V2>>();

		public static readonly Dictionary<SephirahType, Dictionary<LorId, EmotionCardXmlInfo>> CustomEmotionCards = new Dictionary<SephirahType, Dictionary<LorId, EmotionCardXmlInfo>>();

		public static readonly Dictionary<SephirahType, Dictionary<LorId, EmotionEgoXmlInfo>> CustomEmotionEgo = new Dictionary<SephirahType, Dictionary<LorId, EmotionEgoXmlInfo>>();

		public static readonly Dictionary<LorId, GiftXmlInfo> CustomGifts = new Dictionary<LorId, GiftXmlInfo>();
		public static readonly Dictionary<LorId, TitleXmlInfo> CustomPrefixes = new Dictionary<LorId, TitleXmlInfo>();
		public static readonly Dictionary<LorId, TitleXmlInfo> CustomPostfixes = new Dictionary<LorId, TitleXmlInfo>();
		public static readonly Dictionary<int, LorId> GiftAndTitleDic = new Dictionary<int, LorId>();

		public static readonly Dictionary<LorId, LorId> UnitBookDic = new Dictionary<LorId, LorId>();

		public static readonly Dictionary<FloorLevelXmlInfo, LorId> FloorLevelStageDic = new Dictionary<FloorLevelXmlInfo, LorId>();

		public static readonly Dictionary<LorId, FormationXmlInfo> CustomFormations = new Dictionary<LorId, FormationXmlInfo>();

		public static readonly Dictionary<LorId, LorId> EpisodeDic = new Dictionary<LorId, LorId>();
	}
}
