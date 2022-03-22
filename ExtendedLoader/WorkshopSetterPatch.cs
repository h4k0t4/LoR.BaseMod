﻿using BaseMod;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Workshop;

namespace ExtendedLoader
{
    [HarmonyPatch(typeof(WorkshopSkinDataSetter))]
    public class WorkshopSetterPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(WorkshopSkinDataSetter.SetData), new Type[] { typeof(WorkshopSkinData) })]
        static bool SetterSetDataPrefix(WorkshopSkinData data, WorkshopSkinDataSetter __instance)
        {
            if (__instance is UIWorkshopSkinDataSetter)
            {
                __instance.dic = new Dictionary<ActionDetail, ClothCustomizeData>()
                {
                    [ActionDetail.Default] = data.dic[ActionDetail.Default]
                };
                __instance.Init();
                return false;
            }
            CharacterAppearance characterAppearance = __instance.gameObject.GetComponent<CharacterAppearance>();
            characterAppearance._resourceName = data.dataName;
            List<CharacterMotion> disabledMotions = new List<CharacterMotion>();
            foreach (CharacterMotion characterMotion in characterAppearance._motionList)
            {
                if (!characterAppearance._characterMotionDic.ContainsKey(characterMotion.actionDetail))
                {
                    characterAppearance._characterMotionDic.Add(characterMotion.actionDetail, characterMotion);
                    characterMotion.gameObject.SetActive(false);
                }
            }
            foreach (CharacterMotion characterMotion in characterAppearance._motionList)
            {
                if (characterMotion.actionDetail == ActionDetail.Standing)
                {
                    continue;
                }
                if (!data.dic.ContainsKey(characterMotion.actionDetail))
                {
                    disabledMotions.Add(characterMotion);
                    //characterAppearance._motionList.Remove(characterMotion);
                }
            }
            foreach (ActionDetail actionDetail in characterAppearance._characterMotionDic.Keys.ToList())
            {
                if (actionDetail == ActionDetail.Standing)
                {
                    continue;
                }
                if (!data.dic.ContainsKey(actionDetail))
                {
                    disabledMotions.Add(characterAppearance._characterMotionDic[actionDetail]);
                    //characterAppearance._characterMotionDic.Remove(actionDetail);
                }
            }
            disabledMotions = disabledMotions.Distinct().ToList();
            foreach (CharacterMotion characterMotion in disabledMotions)
            {
                characterAppearance._motionList.Remove(characterMotion);
                characterAppearance._characterMotionDic.Remove(characterMotion.actionDetail);
                if (characterMotion.gameObject != null)
                {
                    characterMotion.gameObject.SetActive(false);
                }
            }
            if (data is ExtendedWorkshopSkinData extendedData)
            {
                if (extendedData.motionSoundList != null && extendedData.motionSoundList.Count > 0)
                {
                    string text = data.dic[ActionDetail.Default].spritePath;
                    DirectoryInfo skinRootPath = new DirectoryInfo(text).Parent.Parent;
                    string motionSoundPath = Path.Combine(skinRootPath.FullName, "MotionSound");
                    if (!Directory.Exists(motionSoundPath))
                    {
                        motionSoundPath = Path.Combine(skinRootPath.Parent.Parent.FullName, "/Resource/MotionSound");
                    }
                    characterAppearance.GetComponent<CharacterSound>()?.SetMotionSounds(extendedData.motionSoundList, motionSoundPath);
                }
                if (extendedData.atkEffectPivotDic != null)
                {
                    if (extendedData.atkEffectPivotDic.TryGetValue("atkEffectRoot", out Harmony_Patch.EffectPivot atkEffectRoot))
                    {
                        __instance.Appearance.atkEffectRoot = CreateTransform(__instance.Appearance, "atkEffectRoot", atkEffectRoot.localPosition, atkEffectRoot.localScale, atkEffectRoot.localEulerAngles);
                    }
                    if (extendedData.atkEffectPivotDic.TryGetValue("atkEffectPivot_H", out Harmony_Patch.EffectPivot atkEffectPivot_H))
                    {
                        __instance.Appearance.atkEffectPivot_H = CreateTransform(__instance.Appearance, "atkEffectPivot_H", atkEffectPivot_H.localPosition, atkEffectPivot_H.localScale, atkEffectPivot_H.localEulerAngles);
                    }
                    if (extendedData.atkEffectPivotDic.TryGetValue("atkEffectPivot_J", out Harmony_Patch.EffectPivot atkEffectPivot_J))
                    {
                        __instance.Appearance.atkEffectPivot_J = CreateTransform(__instance.Appearance, "atkEffectPivot_J", atkEffectPivot_J.localPosition, atkEffectPivot_J.localScale, atkEffectPivot_J.localEulerAngles);
                    }
                    if (extendedData.atkEffectPivotDic.TryGetValue("atkEffectPivot_Z", out Harmony_Patch.EffectPivot atkEffectPivot_Z))
                    {
                        __instance.Appearance.atkEffectPivot_Z = CreateTransform(__instance.Appearance, "atkEffectPivot_Z", atkEffectPivot_Z.localPosition, atkEffectPivot_Z.localScale, atkEffectPivot_Z.localEulerAngles);
                    }
                    if (extendedData.atkEffectPivotDic.TryGetValue("atkEffectPivot_G", out Harmony_Patch.EffectPivot atkEffectPivot_G))
                    {
                        __instance.Appearance.atkEffectPivot_G = CreateTransform(__instance.Appearance, "atkEffectPivot_G", atkEffectPivot_G.localPosition, atkEffectPivot_G.localScale, atkEffectPivot_G.localEulerAngles);
                    }
                    if (extendedData.atkEffectPivotDic.TryGetValue("atkEffectPivot_E", out Harmony_Patch.EffectPivot atkEffectPivot_E))
                    {
                        __instance.Appearance.atkEffectPivot_E = CreateTransform(__instance.Appearance, "atkEffectPivot_E", atkEffectPivot_E.localPosition, atkEffectPivot_E.localScale, atkEffectPivot_E.localEulerAngles);
                    }
                    if (extendedData.atkEffectPivotDic.TryGetValue("atkEffectPivot_S", out Harmony_Patch.EffectPivot atkEffectPivot_S))
                    {
                        __instance.Appearance.atkEffectPivot_S = CreateTransform(__instance.Appearance, "atkEffectPivot_S", atkEffectPivot_S.localPosition, atkEffectPivot_S.localScale, atkEffectPivot_S.localEulerAngles);
                    }
                    if (extendedData.atkEffectPivotDic.TryGetValue("atkEffectPivot_F", out Harmony_Patch.EffectPivot atkEffectPivot_F))
                    {
                        __instance.Appearance.atkEffectPivot_F = CreateTransform(__instance.Appearance, "atkEffectPivot_F", atkEffectPivot_F.localPosition, atkEffectPivot_F.localScale, atkEffectPivot_F.localEulerAngles);
                    }
                }
                if (extendedData.specialMotionPivotDic != null && extendedData.specialMotionPivotDic.Count > 0)
                {
                    if (__instance.Appearance._specialMotionPivotList == null)
                    {
                        __instance.Appearance._specialMotionPivotList = new List<CharacterAppearance.MotionPivot>();
                    }
                    foreach (KeyValuePair<ActionDetail, Harmony_Patch.EffectPivot> keyValuePair in extendedData.specialMotionPivotDic)
                    {
                        __instance.Appearance._specialMotionPivotList.Add(new CharacterAppearance.MotionPivot()
                        {
                            motion = keyValuePair.Key,
                            pivot = CreateTransform(__instance.Appearance, "Special_" + keyValuePair.Key.ToString(), keyValuePair.Value.localPosition, keyValuePair.Value.localScale, keyValuePair.Value.localEulerAngles)
                        });
                    }
                }
            }
            return true;
        }
        private static Transform CreateTransform(CharacterAppearance character, string name, Vector3 localPosition, Vector3 localScale, Vector3 localEulerAngles)
        {
            try
            {
                GameObject gameObject = new GameObject(name);
                gameObject.transform.SetParent(character.atkEffectRoot.transform);
                gameObject.transform.position = Vector3.zero;
                gameObject.transform.localPosition = localPosition;
                gameObject.transform.localScale = localScale;
                gameObject.transform.localEulerAngles = localEulerAngles;
                return gameObject.transform;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/CloneTransformerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return null;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(WorkshopSkinDataSetter.SetMotionData))]
        static bool SetMotionDataPrefix(ActionDetail motion, ClothCustomizeData data, WorkshopSkinDataSetter __instance)
        {
            try
            {
                CharacterMotion characterMotion = __instance.Appearance.GetCharacterMotion(motion);
                if (characterMotion == null)
                {
                    Debug.LogError("MotionNull!" + motion.ToString());
                    return true;
                }
                Transform transformHead = null;
                Transform transformMid = null;
                Transform transformBack = null;
                Transform transformFront = null;
                Transform transformMidSkin = null;
                Transform transformBackSkin = null;
                Transform transformFrontSkin = null;
                foreach (object obj in characterMotion.transform)
                {
                    Transform transform = (Transform)obj;
                    switch (transform.gameObject.name)
                    {
                        case "CustomizePivot":
                            transformHead = transform;
                            break;
                        case "Customize_Renderer_Back":
                            transformBack = transform;
                            break;
                        case "Customize_Renderer_Back_Skin":
                            transformBackSkin = transform;
                            break;
                        case "Customize_Renderer":
                            transformMid = transform;
                            break;
                        case "Customize_Renderer_Skin":
                            transformMidSkin = transform;
                            break;
                        case "Customize_Renderer_Front":
                            transformFront = transform;
                            break;
                        case "Customize_Renderer_Front_Skin":
                            transformFrontSkin = transform;
                            break;
                    }
                }
                ExtendedClothCustomizeData data1 = data as ExtendedClothCustomizeData;
                WorkshopSkinDataSetter.PartRenderer partRenderer = (data1 == null) ? new WorkshopSkinDataSetter.PartRenderer { action = motion }
                    : new SkinPartRenderer { action = motion };
                SpriteRenderer spriteRendererMid = transformMid?.gameObject.GetComponent<SpriteRenderer>();
                if (spriteRendererMid)
                {
                    spriteRendererMid.sprite = data.sprite;
                    partRenderer.rear = spriteRendererMid;
                }
                SpriteRenderer spriteRendererFront = transformFront?.gameObject.GetComponent<SpriteRenderer>();
                if (spriteRendererFront)
                {
                    spriteRendererFront.sprite = data.frontSprite;
                    if (data.frontSprite == null)
                    {
                        spriteRendererFront.gameObject.SetActive(false);
                    }
                    partRenderer.front = spriteRendererFront;
                }
                if (transformHead)
                {
                    transformHead.localPosition = data.headPos;
                    transformHead.localRotation = Quaternion.Euler(0f, 0f, data.headRotation);
                    transformHead.gameObject.SetActive(data.headEnabled);
                }
                if (data1 != null)
                {
                    SkinPartRenderer partRenderer1 = (SkinPartRenderer)partRenderer;
                    SpriteRenderer spriteRendererBack = transformBack?.gameObject.GetComponent<SpriteRenderer>();
                    if (spriteRendererBack)
                    {
                        spriteRendererBack.sprite = data1.backSprite;
                        if (data1.backSprite == null)
                        {
                            spriteRendererBack.gameObject.SetActive(false);
                        }
                        partRenderer1.rearest = spriteRendererBack;
                    }
                    SpriteRenderer spriteRendererBackSkin = transformBackSkin?.gameObject.GetComponent<SpriteRenderer>();
                    if (spriteRendererBackSkin)
                    {
                        spriteRendererBackSkin.sprite = data1.backSkinSprite;
                        if (data1.backSkinSprite == null)
                        {
                            spriteRendererBackSkin.gameObject.SetActive(false);
                        }
                        partRenderer1.rearestSkin = spriteRendererBackSkin;
                    }
                    SpriteRenderer spriteRendererMidSkin = transformMidSkin?.gameObject.GetComponent<SpriteRenderer>();
                    if (spriteRendererMidSkin)
                    {
                        spriteRendererMidSkin.sprite = data1.skinSprite;
                        if (data1.skinSprite == null)
                        {
                            spriteRendererMidSkin.gameObject.SetActive(false);
                        }
                        partRenderer1.rearSkin = spriteRendererMidSkin;
                    }
                    SpriteRenderer spriteRendererFrontSkin = transformFrontSkin?.gameObject.GetComponent<SpriteRenderer>();
                    if (spriteRendererFrontSkin)
                    {
                        spriteRendererFrontSkin.sprite = data1.frontSkinSprite;
                        if (data1.frontSkinSprite == null)
                        {
                            spriteRendererFrontSkin.gameObject.SetActive(false);
                        }
                        partRenderer1.frontSkin = spriteRendererFrontSkin;
                    }
                    foreach (Vector3 pos in data1.additionalPivots)
                    {
                        GameObject gameObject = new GameObject("AdditionalPivot");
                        Transform transform1 = gameObject.transform;
                        transform1.parent = characterMotion.transform;
                        transform1.localPosition = new Vector2(pos.x, pos.y);
                        transform1.localScale = new Vector3(1, 1, 1);
                        transform1.localRotation = Quaternion.Euler(0f, 0f, pos.z);
                        characterMotion.additionalPivotList.Add(transform1);
                    }
                }
                else
                {
                    SpriteRenderer spriteRendererBack = transformBack?.gameObject.GetComponent<SpriteRenderer>();
                    if (spriteRendererBack)
                    {
                        spriteRendererBack.gameObject.SetActive(false);
                    }
                    SpriteRenderer spriteRendererBackSkin = transformBackSkin?.gameObject.GetComponent<SpriteRenderer>();
                    if (spriteRendererBackSkin)
                    {
                        spriteRendererBackSkin.gameObject.SetActive(false);
                    }
                    SpriteRenderer spriteRendererMidSkin = transformMidSkin?.gameObject.GetComponent<SpriteRenderer>();
                    if (spriteRendererMidSkin)
                    {
                        spriteRendererMidSkin.gameObject.SetActive(false);
                    }
                    SpriteRenderer spriteRendererFrontSkin = transformFrontSkin?.gameObject.GetComponent<SpriteRenderer>();
                    if (spriteRendererFrontSkin)
                    {
                        spriteRendererFrontSkin.gameObject.SetActive(false);
                    }
                }
                __instance.parts.Add(motion, partRenderer);
                characterMotion.motionDirection = data.direction;
                return false;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/SetMotionDataPrefixerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(WorkshopSkinDataSetter.SetOrder))]
        static bool SetOrder(CustomizedAppearance ca, WorkshopSkinDataSetter __instance)
        {
            foreach (KeyValuePair<ActionDetail, WorkshopSkinDataSetter.PartRenderer> keyValuePair in __instance.parts)
            {
                int num = ca.GetRendererOrder(CharacterAppearanceType.RearHair, keyValuePair.Key);
                int num3 = num;
                int rendererOrder = ca.GetRendererOrder(CharacterAppearanceType.FrontHair, keyValuePair.Key);
                int num2 = ca.GetRendererOrder(CharacterAppearanceType.Head, keyValuePair.Key);
                SpriteRenderer rear = keyValuePair.Value.rear;
                SpriteRenderer front = keyValuePair.Value.front;
                SpriteRenderer rearSkin = (keyValuePair.Value as SkinPartRenderer)?.rearSkin;
                SpriteRenderer frontSkin = (keyValuePair.Value as SkinPartRenderer)?.frontSkin;
                SpriteRenderer rearest = (keyValuePair.Value as SkinPartRenderer)?.rearest;
                SpriteRenderer rearestSkin = (keyValuePair.Value as SkinPartRenderer)?.rearestSkin;
                if (rear)
                {
                    num2--;
                    num++;
                    if (rearSkin)
                    {
                        num2--;
                        rearSkin.sortingOrder = Math.Min(num2, num) + 1;
                    }
                    rear.sortingOrder = Math.Min(num2, num);
                }
                if (front)
                {
                    front.sortingOrder = rendererOrder + 100;
                    if (frontSkin)
                    {
                        frontSkin.sortingOrder = rendererOrder + 101;
                    }
                }
                if (rearest)
                {
                    num3--;
                    if (rearestSkin)
                    {
                        rearestSkin.sortingOrder = num3;
                        num3--;
                    }
                    rearest.sortingOrder = num3;
                }
            }
            return false;
        }
    }
}
