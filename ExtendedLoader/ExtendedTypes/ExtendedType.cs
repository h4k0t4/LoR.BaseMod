using CustomInvitation;
using System.Collections.Generic;
using UnityEngine;
using Workshop;
using System.Runtime.CompilerServices;

namespace ExtendedLoader
{
	public class SkinData
	{
		public SkinData(Dictionary<ActionDetail, ClothCustomizeData> original)
		{
			ExtraDataDic.Remove(original);
			ExtraDataDic.Add(original, this);
		}

		public readonly List<BookSoundInfo> motionSoundList = new List<BookSoundInfo>();
		public readonly Dictionary<ActionDetail, EffectPivot> specialMotionPivotDic = new Dictionary<ActionDetail, EffectPivot>();
		public readonly Dictionary<string, EffectPivot> atkEffectPivotDic = new Dictionary<string, EffectPivot>();
		public FaceData faceData = null;

		internal static readonly ConditionalWeakTable<Dictionary<ActionDetail, ClothCustomizeData>, SkinData> ExtraDataDic = new ConditionalWeakTable<Dictionary<ActionDetail, ClothCustomizeData>, SkinData>();

		public static SkinData GetExtraData(Dictionary<ActionDetail, ClothCustomizeData> dict)
		{
			if (ExtraDataDic.TryGetValue(dict, out SkinData faceData))
			{
				return faceData;
			}
			return null;
		}
	}
	public class FaceData
	{
		public FaceData(Dictionary<Workshop.FaceCustomType, Sprite> original)
		{
			ExtraDataDic.Remove(original);
			ExtraDataDic.Add(original, this);
		}

		public readonly Dictionary<CustomizeType, string> customNames = new Dictionary<CustomizeType, string>();

		public readonly Dictionary<CustomizeType, int> customIds = new Dictionary<CustomizeType, int>();

		public readonly Dictionary<CustomizeColor, Color> customColors = new Dictionary<CustomizeColor, Color>();

		public readonly Dictionary<GiftPosition, string> customVisualGifts = new Dictionary<GiftPosition, string>();

		public static readonly Dictionary<CustomizeType, Dictionary<string, int>> idsByNames = new Dictionary<CustomizeType, Dictionary<string, int>> {
			[CustomizeType.FrontHair] = new Dictionary<string, int>(),
			[CustomizeType.RearHair] = new Dictionary<string, int>(),
			[CustomizeType.Eye] = new Dictionary<string, int>(),
			[CustomizeType.Brow] = new Dictionary<string, int>(),
			[CustomizeType.Mouth] = new Dictionary<string, int>(),
		};

		internal static readonly ConditionalWeakTable<Dictionary<Workshop.FaceCustomType, Sprite>, FaceData> ExtraDataDic = new ConditionalWeakTable<Dictionary<Workshop.FaceCustomType, Sprite>, FaceData>();

		public static FaceData GetExtraData(Dictionary<Workshop.FaceCustomType, Sprite> dict)
		{
			if (ExtraDataDic.TryGetValue(dict, out FaceData faceData))
			{
				return faceData;
			}
			return null;
		}

		public int TryGetId(CustomizeType type)
		{
			if (customIds.TryGetValue(type, out var id))
			{
				return id;
			}
			if (customNames.TryGetValue(type, out var name) && idsByNames[type].TryGetValue(name, out id))
			{
				customIds[type] = id;
				return id;
			}
			return -1;
		}

		public Color TryGetColor(CustomizeColor type)
		{
			if (customColors.TryGetValue(type, out var color))
			{
				return color;
			}
			switch (type)
			{
				case CustomizeColor.HairColor:
					color = RandomUtil.SelectOne(UnitCustomizingData.HairColorTable);
					break;
				case CustomizeColor.EyeColor:
					color = RandomUtil.SelectOne(UnitCustomizingData.EyeColorTable);
					break;
				case CustomizeColor.SkinColor:
					color = RandomUtil.SelectOne(UnitCustomizingData.SkinColorTable);
					break;
			}
			customColors[type] = color;
			return color;
		}
	}
	public enum CustomizeType
	{
		FrontHair,
		RearHair,
		Eye,
		Brow,
		Mouth
	}
	public enum CustomizeColor
	{
		HairColor,
		EyeColor,
		SkinColor
	}
	public class EffectPivot
	{
		public Vector3 localPosition = Vector3.zero;
		public Vector3 localScale = new Vector3(1, 1, 1);
		public Vector3 localEulerAngles = Vector3.zero;
		public bool isNested = true;
	}
	public class SkinPartRenderer : WorkshopSkinDataSetter.PartRenderer
	{
		public SpriteRenderer rearSkin = null;
		public SpriteRenderer frontSkin = null;
		public SpriteRenderer rearest = null;
		public SpriteRenderer rearestSkin = null;
		public SpriteRenderer effect = null;
	}
	public class UIWorkshopSkinDataSetter : WorkshopSkinDataSetter
	{

	}
	public class ExtendedCharacterMotion : MonoBehaviour
	{
		public FaceOverride faceOverride = FaceOverride.None;
	}
	public class CharacterFaceData : MonoBehaviour
	{
		public FaceData faceData;
	}
	public enum FaceOverride
	{
		None = -1,
		Normal = 0,
		Attack = 4,
		Hit = 6
	}
}
