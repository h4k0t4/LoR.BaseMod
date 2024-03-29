﻿using LOR_DiceSystem;
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
                    dialogDetails.Add(new DialogDetail
                    {
                        GroupName = newinfo.groupName,
                        BookId = new LorId(uniqueId, dialogbookId),
                        CharacterID = new LorId(uniqueId, dialogbookId),
                        CharacterName = battleDialogCharacter_New.characterName,
                    });
                }
                else
                {
                    dialogDetail.CharacterName = battleDialogCharacter_New.characterName;
                }
            }
            return battleDialogRoot;
        }
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
                    dialogDetails.Add(new DialogDetail
                    {
                        GroupName = battleDialogRelationWithBookID_New.groupName,
                        BookId = new LorId(battleDialogRelationWithBookID_New.workshopId, battleDialogRelationWithBookID_New.bookID),
                        CharacterID = new LorId(battleDialogRelationWithBookID_New.workshopId, characterId),
                    });
                }
                else
                {
                    dialogDetail.GroupName = battleDialogRelationWithBookID_New.groupName;
                    dialogDetail.CharacterID = new LorId(battleDialogRelationWithBookID_New.workshopId, characterId);
                }
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
        public static Dictionary<string, int> LoadDefaultBookCategories()
        {
            BookCategoryDic = new Dictionary<string, int>();
            foreach (BookCategory category in Enum.GetValues(typeof(BookCategory)))
            {
                BookCategoryDic.Add(category.ToString(), (int)category);
                if ((int)category > maxBookCategory)
                {
                    maxBookCategory = (int)category;
                }
            }
            return BookCategoryDic;
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

        public static List<DialogDetail> dialogDetails = new List<DialogDetail>();

        public static Dictionary<LorId, string> StageNameDic = new Dictionary<LorId, string>();

        public static Dictionary<StageExtraCondition, List<LorId>> StageConditionDic = new Dictionary<StageExtraCondition, List<LorId>>();

        public static Dictionary<LorId, string> CharacterNameDic = new Dictionary<LorId, string>();

        public static Dictionary<EmotionEgoXmlInfo, LorId> EgoDic = new Dictionary<EmotionEgoXmlInfo, LorId>();

        public static Dictionary<LorId, List<EnemyDropItemTable_New>> DropItemDic = new Dictionary<LorId, List<EnemyDropItemTable_New>>();

        public static Dictionary<string, int> BookCategoryDic;

        public static int maxBookCategory = 0;
    }
}
