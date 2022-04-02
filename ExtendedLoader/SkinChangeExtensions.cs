using HarmonyLib;
using System;
using System.IO;
using UnityEngine;
using Workshop;

namespace ExtendedLoader
{
    public static class SkinChangeExtensions
    {
        public class ModSkinInfo : BattleUnitView.SkinInfo
        {
            public string packageId;
            public LorName lorName;

            public ModSkinInfo()
            {
                state = BattleUnitView.SkinState.Default;
                lorName = LorName.None;
                packageId = "";
                skinName = "";
            }

            public ModSkinInfo(BattleUnitView.SkinInfo skinInfo)
            {
                state = skinInfo.state;
                lorName = new LorName(skinInfo.skinName);
                skinName = lorName.name;
                packageId = lorName.packageId;
            }

            public BattleUnitView.SkinInfo Compress()
            {
                return new BattleUnitView.SkinInfo()
                {
                    state = state,
                    skinName = lorName.Compress()
                };
            }
        }

        public static void ChangeSkinBySkinInfo(this BattleUnitView view, BattleUnitView.SkinInfo skinInfo, bool forceChange)
        {
            BattleUnitView.SkinInfo skinInfo1 = skinInfo;
            if (forceChange && skinInfo1.state == BattleUnitView.SkinState.Default)
            {
                skinInfo1 = new BattleUnitView.SkinInfo()
                {
                    state = BattleUnitView.SkinState.Changed,
                    skinName = skinInfo1.skinName
                };
            }
            view.ChangeSkinBySkinInfo(skinInfo1);
        }
        public static void ChangeSkin(this BattleUnitView view, LorName charName)
        {
            if (charName.IsBasic())
            {
                view.ChangeSkin(charName.name);
                return;
            }
            view._skinInfo.state = BattleUnitView.SkinState.Changed;
            view._skinInfo.skinName = charName.Compress();
            ActionDetail currentMotionDetail = view.charAppearance.GetCurrentMotionDetail();
            view.DestroySkin();
            GameObject gameObject = XLRoot.CustomAppearancePrefab;
            if (gameObject != null)
            {
                try
                {
                    UnitCustomizingData customizeData = view.model.UnitData.unitData.customizeData;
                    GiftInventory giftInventory = view.model.UnitData.unitData.giftInventory;
                    GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, view.characterRotationCenter);
                    WorkshopSkinData workshopBookSkinData = Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData(charName, "");
                    string resourceName = "";
                    if (workshopBookSkinData != null)
                    {
                        gameObject2.GetComponent<WorkshopSkinDataSetter>().SetData(workshopBookSkinData);
                        resourceName = workshopBookSkinData.dataName;
                    }
                    else
                    {
                        Debug.Log(charName + " Character Render Failed");
                    }
                    view.charAppearance = gameObject2.GetComponent<CharacterAppearance>();
                    view.charAppearance.Initialize(resourceName);
                    view.charAppearance.InitCustomData(customizeData, view.model.UnitData.unitData.defaultBook.GetBookClassInfoId());
                    view.charAppearance.InitGiftDataAll(giftInventory.GetEquippedList());
                    view.charAppearance.ChangeMotion(currentMotionDetail);
                    view.charAppearance.ChangeLayer("Character");
                    view.charAppearance.SetLibrarianOnlySprites(view.model.faction);
                    if (customizeData != null)
                    {
                        view.ChangeHeight(customizeData.height);
                    }
                    if (workshopBookSkinData != null)
                    {
                        gameObject2.GetComponent<WorkshopSkinDataSetter>().LateInit();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("BaseMod XL: error changing to skin " + charName);
                    Debug.LogError(ex);
                    view.CreateSkin();
                }
            }
        }
        public static void ChangeEgoSkin(this BattleUnitView view, LorName egoName, bool bookNameChange = true)
        {
            if (egoName.IsBasic())
            {
                view.ChangeEgoSkin(egoName.name, bookNameChange);
                return;
            }
            view._skinInfo.state = BattleUnitView.SkinState.EGO;
            view._skinInfo.skinName = egoName.Compress();
            ActionDetail currentMotionDetail = view.charAppearance.GetCurrentMotionDetail();
            view.DestroySkin();
            GameObject gameObject = XLRoot.CustomAppearancePrefab;
            if (gameObject != null)
            {
                try
                {
                    UnitCustomizingData customizeData = view.model.UnitData.unitData.customizeData;
                    GiftInventory giftInventory = view.model.UnitData.unitData.giftInventory;
                    GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, view.characterRotationCenter);
                    WorkshopSkinData workshopBookSkinData = Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData(egoName, "");
                    string resourceName = "";
                    if (workshopBookSkinData != null)
                    {
                        gameObject2.GetComponent<WorkshopSkinDataSetter>().SetData(workshopBookSkinData);
                        resourceName = workshopBookSkinData.dataName;
                    }
                    else
                    {
                        Debug.Log(egoName + " EGO Render Failed");
                    }
                    view.charAppearance = gameObject2.GetComponent<CharacterAppearance>();
                    view.charAppearance.Initialize("");
                    view.charAppearance.InitCustomData(customizeData, view.model.UnitData.unitData.defaultBook.GetBookClassInfoId());
                    view.charAppearance.InitGiftDataAll(giftInventory.GetEquippedList());
                    view.charAppearance.ChangeMotion(currentMotionDetail);
                    view.charAppearance.ChangeLayer("Character");
                    view.charAppearance.SetLibrarianOnlySprites(view.model.faction);
                    if (bookNameChange)
                    {
                        view.model.customBook.SetCharacterName(egoName.Compress());
                    }
                    if (customizeData != null)
                    {
                        view.ChangeHeight(customizeData.height);
                    }
                    if (workshopBookSkinData != null)
                    {
                        gameObject2.GetComponent<WorkshopSkinDataSetter>().LateInit();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("BaseMod XL: error changing to EGO skin " + egoName);
                    Debug.LogError(ex);
                    view.CreateSkin();
                }
            }
        }
        public static void ChangeCreatureSkin(this BattleUnitView view, LorName creatureName)
        {
            if (creatureName.IsBasic())
            {
                view.ChangeCreatureSkin(creatureName.name);
                return;
            }
            view._skinInfo.state = BattleUnitView.SkinState.Creature;
            view._skinInfo.skinName = creatureName.Compress();
            ActionDetail currentMotionDetail = view.charAppearance.GetCurrentMotionDetail();
            view.DestroySkin();
            GameObject gameObject = XLRoot.CustomAppearancePrefab;
            if (gameObject != null)
            {
                try
                {
                    UnitCustomizingData customizeData = view.model.UnitData.unitData.customizeData;
                    GiftInventory giftInventory = view.model.UnitData.unitData.giftInventory;
                    GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, view.characterRotationCenter);
                    WorkshopSkinData workshopBookSkinData = Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData(creatureName, "");
                    string resourceName = "";
                    if (workshopBookSkinData != null)
                    {
                        gameObject2.GetComponent<WorkshopSkinDataSetter>().SetData(workshopBookSkinData);
                        resourceName = workshopBookSkinData.dataName;
                    }
                    else
                    {
                        Debug.Log(creatureName + " Creature Render Failed");
                    }
                    view.charAppearance = gameObject2.GetComponent<CharacterAppearance>();
                    view.charAppearance._initialized = false;
                    view.charAppearance.Initialize(resourceName);
                    view.charAppearance.InitCustomData(customizeData, view.model.UnitData.unitData.defaultBook.GetBookClassInfoId());
                    view.charAppearance.InitGiftDataAll(giftInventory.GetEquippedList());
                    view.charAppearance.ChangeMotion(currentMotionDetail);
                    view.charAppearance.ChangeLayer("Character");
                    if (customizeData != null)
                    {
                        view.ChangeHeight(customizeData.height);
                    }
                    if (workshopBookSkinData != null)
                    {
                        gameObject2.GetComponent<WorkshopSkinDataSetter>().LateInit();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("error changing to Creature skin " + creatureName);
                    Debug.LogError(ex);
                    view.CreateSkin();
                }
            }
        }
        public static void ChangeSkinByModSkinInfo(this BattleUnitView view, ModSkinInfo info, bool forceChange = false)
        {
            if (info == null)
            {
                return;
            }
            switch (info.state)
            {
                case BattleUnitView.SkinState.Default:
                    if (forceChange)
                    {
                        view.ChangeSkin(info.lorName);
                    }
                    else
                    {
                        view.CreateSkin();
                    }
                    return;
                case BattleUnitView.SkinState.Changed:
                    view.ChangeSkin(info.lorName);
                    return;
                case BattleUnitView.SkinState.Creature:
                    view.ChangeCreatureSkin(info.lorName);
                    return;
                case BattleUnitView.SkinState.EGO:
                    view.ChangeEgoSkin(info.lorName, true);
                    return;
            }
        }
        public static ModSkinInfo GetCurrentModSkinInfo(this BattleUnitView view)
        {
            return new ModSkinInfo(view.GetCurrentSkinInfo());
        }
        public static void ChangeSkinWorkshop(this BattleUnitView view, string skinName)
        {
            view.ChangeSkin(new LorName(skinName, true));
        }
        public static void ChangeEgoSkinWorkshop(this BattleUnitView view, string egoName, bool bookNameChange = true)
        {
            view.ChangeEgoSkin(new LorName(egoName, true), bookNameChange);
        }
        public static void ChangeCreatureSkinWorkshop(this BattleUnitView view, string creatureName)
        {
            view.ChangeCreatureSkin(new LorName(creatureName, true));
        }
        public static void SetAltSkin(this BattleUnitView view, LorName skinName)
        {
            view.SetAltSkin(skinName.Compress());
        }
        public static void SetAltSkinWorkshop(this BattleUnitView view, string skinName)
        {
            view.SetAltSkin(new LorName(skinName, true).Compress());
        }
        public static Sprite GetThumbSprite(this WorkshopSkinData data)
        {
            int skinId = data.id;
            if (XLRoot.SkinThumb.ContainsKey(skinId))
            {
                return XLRoot.SkinThumb[skinId];
            }
            ClothCustomizeData defaultData = data.dic.GetValueSafe(ActionDetail.Default);
            if (defaultData != null)
            {
                try
                {
                    if (defaultData.sprite != null)
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(defaultData.spritePath);
                        string thumbPath = directoryInfo.Parent.Parent.FullName + "/Thumb.png";
                        if (File.Exists(thumbPath))
                        {
                            Texture2D texture2D = new Texture2D(2, 2);
                            texture2D.LoadImage(File.ReadAllBytes(thumbPath));
                            Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
                            XLRoot.SkinThumb.Add(skinId, sprite);
                            return sprite;
                        }
                        Sprite newThumb = XLRoot.MakeThumbnail(data.dic[ActionDetail.Default]);
                        if (newThumb != null)
                        {
                            XLRoot.SkinThumb.Add(skinId, newThumb);
                            return newThumb;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            return null;
        }
        public static LorName GetOriginalSkin(this BattleUnitModel model)
        {
            string workshopSkin = model._unitData.unitData.workshopSkin;
            if (!string.IsNullOrEmpty(workshopSkin))
            {
                return new LorName(workshopSkin, true);
            }
            string packageId = "";
            if (model.customBook._classInfo.skinType == "Custom")
            {
                packageId = model.customBook._classInfo.workshopID;
            }
            return new LorName(packageId, model.customBook.GetOriginalCharcterName());
        }
        public static WorkshopSkinData GetWorkshopBookSkinData(this CustomizingBookSkinLoader loader, string id, string name, string gender)
        {
            return loader.GetWorkshopBookSkinData(new LorName(id, name), gender);
        }
        public static WorkshopSkinData GetWorkshopBookSkinData(this CustomizingBookSkinLoader loader, LorName name, string gender)
        {
            if (name.packageId == LorName.BASEMOD_ID)
            {
                return Singleton<CustomizingResourceLoader>.Instance.GetWorkshopSkinData(name.name);
            }
            WorkshopSkinData result = loader.GetWorkshopBookSkinData(name.packageId, name.name + gender);
            if (result == null)
            {
                result = loader.GetWorkshopBookSkinData(name.packageId, name.name);
            }
            return result;
        }
    }
}