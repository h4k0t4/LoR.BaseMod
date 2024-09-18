using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using Workshop;

namespace ExtendedLoader
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	[HarmonyPatch]
	public class ExtendedClothCustomizeData : ClothCustomizeData
	{
		public bool hasSkinSprite => !string.IsNullOrEmpty(skinSpritePath);
		public bool hasFrontSkinSprite => !string.IsNullOrEmpty(frontSkinSpritePath);
		public bool hasBackSprite => !string.IsNullOrEmpty(backSpritePath);
		public bool hasBackSkinSprite => !string.IsNullOrEmpty(backSkinSpritePath);
		public bool hasEffectSprite => !string.IsNullOrEmpty(effectSpritePath);

		public Vector2Int size = new Vector2Int(512, 512);
		public Sprite _skinSprite = null;
		public Sprite _frontSkinSprite = null;
		public Sprite _backSprite = null;
		public Sprite _backSkinSprite = null;
		public Sprite _effectSprite = null;
		public string skinSpritePath = "";
		public string frontSkinSpritePath = "";
		public string backSpritePath = "";
		public string backSkinSpritePath = "";
		public string effectSpritePath = "";
		public float resolution = 50f;
		public List<EffectPivot> additionalPivots = new List<EffectPivot>();
		public EffectPivot headPivot;
		public FaceOverride faceOverride = FaceOverride.None;


		[HarmonyPatch(typeof(ClothCustomizeData), nameof(LoadSprite))]
		[HarmonyPrefix]
		static bool ClothCustomizeData_LoadSprite_Prefix(ClothCustomizeData __instance)
		{

			if (!(__instance is ExtendedClothCustomizeData instance))
			{
				return true;
			}
			if (instance.hasSpriteFile)
			{
				instance._sprite = SpriteUtilExtension.LoadCustomSizedPivotSprite(instance.spritePath, instance.pivotPos, instance.size, instance.resolution);
				return false;
			}
			return true;
		}

		[HarmonyPatch(typeof(ClothCustomizeData), nameof(LoadFrontSprite))]
		[HarmonyPrefix]
		static bool ClothCustomizeData_LoadFrontSprite_Prefix(ClothCustomizeData __instance)
		{
			if (!(__instance is ExtendedClothCustomizeData instance))
			{
				return true;
			}
			if (instance.hasFrontSpriteFile)
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

		public Sprite effectSprite
		{
			get
			{
				if (hasEffectSprite)
				{
					if (_effectSprite == null)
					{
						LoadEffectSprite();
					}
				}
				return _effectSprite;
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

		public void LoadEffectSprite()
		{
			if (hasEffectSprite)
			{
				_effectSprite = SpriteUtilExtension.LoadCustomSizedPivotSprite(effectSpritePath, pivotPos, size, resolution);
			}
		}

		public void LoadAllSprites()
		{
			_ = sprite;
			_ = skinSprite;
			_ = frontSprite;
			_ = frontSkinSprite;
			_ = backSprite;
			_ = backSkinSprite;
			_ = effectSprite;
		}
	}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
