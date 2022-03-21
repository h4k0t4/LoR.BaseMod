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
        public Dictionary<ActionDetail, Harmony_Patch.EffectPivot> specialMotionPivotDic = new Dictionary<ActionDetail, Harmony_Patch.EffectPivot>();
        public Dictionary<string, Harmony_Patch.EffectPivot> atkEffectPivotDic = new Dictionary<string, Harmony_Patch.EffectPivot>();
    }
    public class ExtendedWorkshopAppearanceInfo : WorkshopAppearanceInfo
    {
        public List<BookSoundInfo> motionSoundList = new List<BookSoundInfo>();
        public Dictionary<ActionDetail, Harmony_Patch.EffectPivot> specialMotionPivotDic = new Dictionary<ActionDetail, Harmony_Patch.EffectPivot>();
        public Dictionary<string, Harmony_Patch.EffectPivot> atkEffectPivotDic = new Dictionary<string, Harmony_Patch.EffectPivot>();
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
}
