using BaseMod;
using CustomInvitation;
using System.Collections.Generic;
using UnityEngine;
using Workshop;

namespace ExtendedLoader
{
    public class ExtendedWorkshopSkinData : WorkshopSkinData
    {
        public List<BookSoundInfo> motionSoundList = new List<BookSoundInfo>();
        public Dictionary<ActionDetail, EffectPivot> specialMotionPivotDic = new Dictionary<ActionDetail, EffectPivot>();
        public Dictionary<string, EffectPivot> atkEffectPivotDic = new Dictionary<string, EffectPivot>();
    }
    public class ExtendedWorkshopAppearanceInfo : WorkshopAppearanceInfo
    {
        public List<BookSoundInfo> motionSoundList = new List<BookSoundInfo>();
        public Dictionary<ActionDetail, EffectPivot> specialMotionPivotDic = new Dictionary<ActionDetail, EffectPivot>();
        public Dictionary<string, EffectPivot> atkEffectPivotDic = new Dictionary<string, EffectPivot>();
    }
    public class EffectPivot
    {
        public Vector3 localPosition = Vector3.zero;
        public Vector3 localScale = new Vector3(1, 1, 1);
        public Vector3 localEulerAngles = Vector3.zero;
    }
    public class SkinPartRenderer : WorkshopSkinDataSetter.PartRenderer
    {
        public SpriteRenderer rearSkin = null;
        public SpriteRenderer frontSkin = null;
        public SpriteRenderer rearest = null;
        public SpriteRenderer rearestSkin = null;
    }
    public class UIWorkshopSkinDataSetter : WorkshopSkinDataSetter
    {

    }
    public class ExtendedCharacterMotion : CharacterMotion
    {
        public FaceOverride faceOverride = FaceOverride.None;
    }
    public enum FaceOverride
    {
        None = -1,
        Normal = 0,
        Attack = 4,
        Hit = 6
    }
}
