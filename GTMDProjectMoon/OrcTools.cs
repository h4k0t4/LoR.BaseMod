using LOR_DiceSystem;
using LOR_XML;
using System;
using System.Collections.Generic;

namespace GTMDProjectMoon
{
    public static class OrcTools
    {
        public static StageClassInfo CopyStageClassInfo(this StageClassInfo stageClassInfo, StageClassInfo_New newinfo, string uniqueId = "")
        {
            List<int> needClearStageList = new List<int>();
            foreach (LorId lorId in newinfo.extraCondition.stagecondition)
            {
                needClearStageList.Add(lorId.id);
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
            StageConditionDic[stageClassInfo.extraCondition] = newinfo.extraCondition.stagecondition;
            return stageClassInfo;
        }
        public static BookXmlInfo CopyBookXmlInfo(this BookXmlInfo bookXml, BookXmlInfo_New newinfo, string uniqueId = "")
        {
            List<int> onlycard = new List<int>();
            List<BookSoulCardInfo> soulcard = new List<BookSoulCardInfo>();
            foreach (LorIdXml xml in newinfo.EquipEffect.OnlyCard)
            {
                onlycard.Add(xml.xmlId);
            }
            foreach (BookSoulCardInfo_New soulCardInfo in newinfo.EquipEffect.CardList)
            {
                if (soulCardInfo.WorkshopId == "")
                {
                    soulCardInfo.WorkshopId = uniqueId;
                }
                if (soulCardInfo.WorkshopId.ToLower() == "@origin")
                {
                    soulCardInfo.WorkshopId = "";
                }
                soulcard.Add(new BookSoulCardInfo()
                {
                    cardId = soulCardInfo.cardId,
                    requireLevel = soulCardInfo.requireLevel,
                    emotionLevel = soulCardInfo.emotionLevel,
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
            bookXml.EquipEffect = new BookEquipEffect()
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
                OnlyCard = onlycard,
                CardList = soulcard,
                _PassiveList = newinfo.EquipEffect._PassiveList,
                PassiveList = newinfo.EquipEffect.PassiveList,
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
            LorId booklorid = new LorId(uniqueId, newinfo._id);
            List<LorId> onlycardlist = new List<LorId>();
            LorId.InitializeLorIds(newinfo.EquipEffect.OnlyCard, onlycardlist, uniqueId);
            OnlyCardDic[booklorid] = onlycardlist;
            SoulCardDic[booklorid] = newinfo.EquipEffect.CardList;
            if (newinfo.episode.xmlId > 0)
            {
                EpisodeDic[booklorid] = LorId.MakeLorId(newinfo.episode, uniqueId);
            }
            return bookXml;
        }
        public static DiceCardXmlInfo CopyDiceCardXmlInfo(this DiceCardXmlInfo info, DiceCardXmlInfo_New newinfo)
        {
            info.Artwork = newinfo.Artwork;
            info.category = string.IsNullOrWhiteSpace(newinfo.customCategory) ? newinfo.category : GetBookCategory(newinfo.customCategory);
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
        public static LOR_XML.BattleDialogRoot CopyBattleDialogRoot(this LOR_XML.BattleDialogRoot battleDialogRoot, BattleDialogRoot newinfo, string uniqueId = "")
        {
            battleDialogRoot.characterList = new List<BattleDialogCharacter>();
            foreach (BattleDialogCharacter_New battleDialogCharacter_New in newinfo.characterList)
            {
                int dialogbookId = 0;
                try
                {
                    dialogbookId = int.Parse(battleDialogCharacter_New.characterID);
                }
                catch { }
                battleDialogCharacter_New.workshopId = uniqueId;
                battleDialogCharacter_New.bookId = dialogbookId;
                battleDialogRoot.characterList.Add(new BattleDialogCharacter()
                {
                    characterID = battleDialogCharacter_New.characterID,
                    dialogTypeList = battleDialogCharacter_New.dialogTypeList,
                    workshopId = uniqueId,
                    bookId = dialogbookId,
                });
                if (string.IsNullOrEmpty(battleDialogCharacter_New.characterName))
                {
                    battleDialogCharacter_New.characterName = "";
                }
                DialogDic[new LorId(uniqueId, battleDialogCharacter_New.bookId)] = battleDialogCharacter_New.characterName;
            }
            return battleDialogRoot;
        }
        public static List<BattleDialogRelationWithBookID> CopyBattleDialogRelation(this List<BattleDialogRelationWithBookID> BattleDialogRelation, List<BattleDialogRelationWithBookID_New> newinfo, string uniqueId = "")
        {
            foreach (BattleDialogRelationWithBookID_New battleDialogRelationWithBookID_New in newinfo)
            {
                if (string.IsNullOrEmpty(battleDialogRelationWithBookID_New.workshopId))
                {
                    battleDialogRelationWithBookID_New.workshopId = uniqueId;
                }
                if (battleDialogRelationWithBookID_New.workshopId.ToLower() == "@origin")
                {
                    battleDialogRelationWithBookID_New.workshopId = "";
                }
                BattleDialogRelation.Add(new BattleDialogRelationWithBookID
                {
                    bookID = battleDialogRelationWithBookID_New.bookID,
                    storyID = battleDialogRelationWithBookID_New.storyID,
                    groupName = battleDialogRelationWithBookID_New.groupName,
                    characterID = battleDialogRelationWithBookID_New.characterID,
                });
                if (string.IsNullOrEmpty(battleDialogRelationWithBookID_New.groupName))
                {
                    battleDialogRelationWithBookID_New.groupName = "";
                }
                DialogRelationDic[new LorId(battleDialogRelationWithBookID_New.workshopId, battleDialogRelationWithBookID_New.bookID)] = battleDialogRelationWithBookID_New.groupName;
            }
            return BattleDialogRelation;
        }
        public static BookCategory GetBookCategory(string category)
        {
            if (!BookCategoryDic.ContainsKey(category))
            {
                maxBookCategory++;
                BookCategoryDic[category] = maxBookCategory;
            }
            return (BookCategory)BookCategoryDic[category];
        }
        static Dictionary<string, int> LoadDefaultBookCategories()
        {
            var dic = new Dictionary<string, int>();
            foreach (BookCategory cat in Enum.GetValues(typeof(BookCategory)))
            {
                dic.Add(cat.ToString(), (int)cat);
                if ((int)cat > maxBookCategory)
                {
                    maxBookCategory = (int)cat;
                }
            }
            return dic;
        }

        public static Dictionary<LorId, List<LorId>> OnlyCardDic = new Dictionary<LorId, List<LorId>>();

        public static Dictionary<LorId, List<BookSoulCardInfo_New>> SoulCardDic = new Dictionary<LorId, List<BookSoulCardInfo_New>>();

        public static Dictionary<LorId, LorId> EpisodeDic = new Dictionary<LorId, LorId>();

        public static Dictionary<LorId, string> StageNameDic = new Dictionary<LorId, string>();

        public static Dictionary<StageExtraCondition, List<LorId>> StageConditionDic = new Dictionary<StageExtraCondition, List<LorId>>();

        public static Dictionary<LorId, string> CharacterNameDic = new Dictionary<LorId, string>();

        public static Dictionary<EmotionEgoXmlInfo, LorId> EgoDic = new Dictionary<EmotionEgoXmlInfo, LorId>();

        public static Dictionary<LorId, List<EnemyDropItemTable_New>> DropItemDic = new Dictionary<LorId, List<EnemyDropItemTable_New>>();

        public static Dictionary<LorId, string> DialogDic = new Dictionary<LorId, string>();

        public static Dictionary<LorId, string> DialogRelationDic = new Dictionary<LorId, string>();

        public static Dictionary<string, int> BookCategoryDic = LoadDefaultBookCategories();

        public static int maxBookCategory = 0;
    }
}
