using BookSoundInfo = CustomInvitation.BookSoundInfo;
using LOR_DiceSystem;
using LOR_XML;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using System;

namespace GTMDProjectMoon
{
    public static class OrcTools
    {
        public static Dictionary<string, int> BookCategoryDic = LoadDefaultBookCategories();
        public static int maxBookCategory = 0;
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

        /*
        public static void InjectForRepair()
        {
            Utils.MakeBackup(Application.dataPath + "/Managed/Assembly-CSharp.dll");
            File.Delete(Application.dataPath + "/Managed/Assembly-CSharp.dll");
            File.Copy(Singleton<ModContentManager>.Instance.GetModPath("BaseMod") + "/Sources/Assembly-CSharp.dll", Application.dataPath + "/Managed/Assembly-CSharp.dll");
            File.WriteAllText(Application.dataPath + "/Managed/AlreadyRepaira9.txt", "Success");
            try
            {
                File.Delete(Application.dataPath + "/Managed/AlreadyRepair.txt");
            }
            catch { }
        }*/
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
            return stageClassInfo;
        }
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
                bool flag = bookSoulCardInfo_New.WorkshopId == "";
                if (flag)
                {
                    bookSoulCardInfo_New.WorkshopId = uniqueId;
                }
                bool flag2 = bookSoulCardInfo_New.WorkshopId.ToLower() == "@origin";
                if (flag2)
                {
                    bookSoulCardInfo_New.WorkshopId = "";
                }
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
            return bookXml;
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
        public static bool ContainsCategory(this BookModel book, string category)
        {
            return book.ContainsCategory(GetBookCategory(category));
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
    }
}

namespace GTMDProjectMoon
{
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
namespace GTMDProjectMoon
{
    public class DiceCardXmlRoot
    {
        [XmlElement("Card")]
        public List<DiceCardXmlInfo_New> cardXmlList;
    }
}
namespace GTMDProjectMoon
{
    public class DiceCardXmlInfo_New : DiceCardXmlInfo
    {
        [XmlElement("CustomCategory")]
        public string customCategory = "";
    }
}
namespace GTMDProjectMoon
{
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

namespace GTMDProjectMoon
{
    public class EmotionEgoXmlRoot
    {
        [XmlElement("EmotionEgo")]
        public List<EmotionEgoXmlInfo_New> egoXmlList;
    }
    public class EmotionEgoXmlInfo_New
    {
        [XmlElement("ID")]
        public int id;

        [XmlElement("Sephirah")]
        public SephirahType Sephirah;

        [XmlElement("Card")]
        public LorIdXml _CardId;

        [XmlElement("LockInBattle")]
        public bool isLock;
    }
    public class EgoCardDetail
    {
        [XmlElement("ID")]
        public int id;

        [XmlElement("Sephirah")]
        public SephirahType Sephirah;

        [XmlElement("Card")]
        public LorIdXml _CardId;

        [XmlElement("LockInBattle")]
        public bool isLock;
    }
}

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

namespace GTMDProjectMoon
{
    public class EnemyUnitClassRoot
    {
        [XmlElement("Enemy")]
        public List<EnemyUnitClassInfo_New> list;
    }
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
    public class EnemyDropItemTable_New
    {
        [XmlAttribute("Level")]
        public int emotionLevel;

        [XmlElement("DropItem")]
        public List<EnemyDropItem_New> dropItemList;

        [XmlIgnore]
        public List<EnemyDropItem_ReNew> dropList = new List<EnemyDropItem_ReNew>();
    }
    public class EnemyDropItem_New
    {
        [XmlText]
        public int bookId;

        [XmlAttribute("Prob")]
        public float prob;

        [XmlAttribute("Pid")]
        public string workshopId = "";
    }
    public class EnemyDropItem_ReNew
    {
        public LorId bookId;

        public float prob;
    }
}

namespace GTMDProjectMoon
{
    public class BattleDialogRoot
    {
        [XmlElement("GroupName")]
        public string groupName;

        [XmlElement("Character")]
        public List<BattleDialogCharacter_New> characterList = new List<BattleDialogCharacter_New>();
    }
}

namespace GTMDProjectMoon
{
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
}

namespace GTMDProjectMoon
{
    public class BattleDialogRelationRoot
    {
        [XmlElement("Relation")]
        public List<BattleDialogRelationWithBookID_New> list;
    }
}

namespace GTMDProjectMoon
{
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

namespace GTMDProjectMoon
{
    public class WorkshopSkinChangeButton : MonoBehaviour
    {
        private void Update()
        {
            if (!(BaseMod.Harmony_Patch.uiEquipPageCustomizePanel == null || !BaseMod.Harmony_Patch.uiEquipPageCustomizePanel.isActiveAndEnabled || BaseMod.Harmony_Patch.uiEquipPageCustomizePanel.isWorkshop || !Input.GetKeyDown(KeyCode.Tab)))
            {
                BaseMod.Harmony_Patch.isModWorkshopSkin = !BaseMod.Harmony_Patch.isModWorkshopSkin;
                if (!BaseMod.Harmony_Patch.isModWorkshopSkin)
                {
                    BaseMod.Harmony_Patch.uiEquipPageCustomizePanel.Init(Singleton<CustomCoreBookInventoryModel>.Instance.GetBookIdList_CustomCoreBook(BaseMod.Harmony_Patch.uiEquipPageCustomizePanel.panel.Parent.SelectedUnit.OwnerSephirah, BaseMod.Harmony_Patch.uiEquipPageCustomizePanel.panel.Parent.SelectedUnit.isSephirah), false);
                    BaseMod.Harmony_Patch.uiEquipPageCustomizePanel.UpdateList();
                }
                else
                {
                    BaseMod.Harmony_Patch.ModWorkshopBookIndex = new Dictionary<int, LorId>();
                    int num = 0;
                    List<int> list = new List<int>();
                    foreach (LorId lorId in Singleton<BookInventoryModel>.Instance.GetIdList_noDuplicate())
                    {
                        BookXmlInfo info = Singleton<BookXmlList>.Instance.GetData(lorId);
                        if (lorId.IsWorkshop() && !info.isError && !info.canNotEquip)
                        {
                            BaseMod.Harmony_Patch.ModWorkshopBookIndex.Add(num, lorId);
                            list.Add(num);
                            num++;
                        }
                    }
                    BaseMod.Harmony_Patch.uiEquipPageCustomizePanel.Init(list, false);
                    BaseMod.Harmony_Patch.uiEquipPageCustomizePanel.UpdateList();
                }
            }
        }
    }
}
/*
namespace GTMDProjectMoon
{
    public static class Utils
    {
        public static bool MakeBackup(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Copy(path, path + ".backup", true);
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                return false;
            }
            return true;
        }
        public static bool MakeBackup(string[] arr)
        {
            try
            {
                foreach (string text in arr)
                {
                    if (File.Exists(text))
                    {
                        File.Copy(text, text + ".backup_", true);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                return false;
            }
            return true;
        }
        public static bool RestoreBackup(string path)
        {
            try
            {
                string text = path + ".backup_";
                if (File.Exists(text))
                {
                    File.Copy(text, path, true);
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                return false;
            }
            return true;
        }
        public static bool RestoreBackup(string[] arr)
        {
            try
            {
                foreach (string text in arr)
                {
                    string text2 = text + ".backup_";
                    if (File.Exists(text2))
                    {
                        File.Copy(text2, text, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                return false;
            }
            return true;
        }
        public static bool DeleteBackup(string path)
        {
            try
            {
                string path2 = path + ".backup_";
                if (File.Exists(path2))
                {
                    File.Delete(path2);
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                return false;
            }
            return true;
        }
        public static bool DeleteBackup(string[] arr)
        {
            try
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    string path = arr[i] + ".backup_";
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                return false;
            }
            return true;
        }
    }
}*/