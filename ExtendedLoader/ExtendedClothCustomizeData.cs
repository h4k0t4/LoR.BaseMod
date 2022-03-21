using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using Workshop;

namespace ExtendedLoader
{
    public class ExtendedClothCustomizeData : ClothCustomizeData
    {
        public bool hasSkinSprite = false;
        public bool hasFrontSkinSprite = false;
        public bool hasBackSprite = false;
        public bool hasBackSkinSprite = false;
        public bool isCustomSize = false;
        public Vector2Int size = new Vector2Int(512, 512);
        public Sprite _skinSprite = null;
        public Sprite _frontSkinSprite = null;
        public Sprite _backSprite = null;
        public Sprite _backSkinSprite = null;
        public string skinSpritePath = "";
        public string frontSkinSpritePath = "";
        public string backSpritePath = "";
        public string backSkinSpritePath = "";
        public float resolution = 50f;
        public List<Vector3> additionalPivots = new List<Vector3>();


        [HarmonyPatch(typeof(ClothCustomizeData), nameof(ClothCustomizeData.LoadSprite))]
        [HarmonyPrefix]
        static bool LoadSpritePrefix(ClothCustomizeData __instance)
        {

            if (!(__instance is ExtendedClothCustomizeData instance))
            {
                return true;
            }
            if (instance.isCustomSize && instance.hasSpriteFile)
            {
                instance._sprite = SpriteUtilExtension.LoadCustomSizedPivotSprite(instance.spritePath, instance.pivotPos, instance.size, instance.resolution);
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(ClothCustomizeData), nameof(ClothCustomizeData.LoadFrontSprite))]
        [HarmonyPrefix]
        static bool LoadFrontSpritePrefix(ClothCustomizeData __instance)
        {
            if (!(__instance is ExtendedClothCustomizeData instance))
            {
                return true;
            }
            if (instance.isCustomSize && instance.hasFrontSpriteFile)
            {
                instance._frontSprite = SpriteUtilExtension.LoadCustomSizedPivotSprite(instance.frontSpritePath, instance.pivotPos, instance.size, instance.resolution);
                return false;
            }
            return true;
        }

        public Sprite skinSprite
        {
            get
            {
                if (hasSkinSprite)
                {
                    if (_skinSprite == null)
                    {
                        LoadSkinSprite();
                    }
                }
                return _skinSprite;
            }
        }

        public Sprite frontSkinSprite
        {
            get
            {
                if (hasFrontSkinSprite)
                {
                    if (_frontSkinSprite == null)
                    {
                        LoadFrontSkinSprite();
                    }
                }
                return _frontSkinSprite;
            }
        }

        public Sprite backSprite
        {
            get
            {
                if (hasBackSprite)
                {
                    if (_backSprite == null)
                    {
                        LoadBackSprite();
                    }
                }
                return _backSprite;
            }
        }

        public Sprite backSkinSprite
        {
            get
            {
                if (hasBackSkinSprite)
                {
                    if (_backSkinSprite == null)
                    {
                        LoadBackSkinSprite();
                    }
                }
                return _backSkinSprite;
            }
        }

        public void LoadSkinSprite()
        {
            if (hasSkinSprite)
            {
                _skinSprite = SpriteUtilExtension.LoadCustomSizedPivotSprite(skinSpritePath, pivotPos, size, resolution);
            }
        }

        public void LoadFrontSkinSprite()
        {
            if (hasFrontSkinSprite)
            {
                _frontSkinSprite = SpriteUtilExtension.LoadCustomSizedPivotSprite(frontSkinSpritePath, pivotPos, size, resolution);
            }
        }

        public void LoadBackSprite()
        {
            if (hasBackSprite)
            {
                _backSprite = SpriteUtilExtension.LoadCustomSizedPivotSprite(backSpritePath, pivotPos, size, resolution);
            }
        }

        public void LoadBackSkinSprite()
        {
            if (hasBackSkinSprite)
            {
                _backSkinSprite = SpriteUtilExtension.LoadCustomSizedPivotSprite(backSkinSpritePath, pivotPos, size, resolution);
            }
        }
    }
}
