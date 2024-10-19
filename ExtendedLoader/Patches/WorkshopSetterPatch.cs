using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Workshop;
using static System.Reflection.Emit.OpCodes;
using static HarmonyLib.AccessTools;

namespace ExtendedLoader
{
	[HarmonyPatch]
	internal class WorkshopSetterPatch
	{
		[HarmonyPatch(typeof(WorkshopSkinDataSetter), nameof(WorkshopSkinDataSetter.SetData), new Type[] { typeof(WorkshopSkinData) })]
		[HarmonyPrefix]
		static bool WorkshopSkinDataSetter_SetData_Prefix(WorkshopSkinData data, WorkshopSkinDataSetter __instance)
		{
			var extendedData = SkinData.GetExtraData(data.dic);
			if (extendedData != null && extendedData.faceData != null)
			{
				var dataHolder = __instance.GetComponent<CharacterFaceData>();
				if (dataHolder == null)
				{
					dataHolder = __instance.gameObject.AddComponent<CharacterFaceData>();
				}
				dataHolder.faceData = extendedData.faceData;
			}
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
				characterMotion.gameObject?.SetActive(false);
			}
			if (extendedData != null)
			{
				if (extendedData.motionSoundList != null && extendedData.motionSoundList.Count > 0)
				{
					string text = data.dic[ActionDetail.Default].spritePath;
					DirectoryInfo skinRootPath = new DirectoryInfo(text).Parent.Parent;
					string motionSoundPath = Path.Combine(skinRootPath.FullName, "MotionSound");
					if (!Directory.Exists(motionSoundPath))
					{
						DirectoryInfo charFolderParent = skinRootPath.Parent.Parent;
						motionSoundPath = charFolderParent.Name == "Resource" ? Path.Combine(charFolderParent.FullName, "MotionSound") : Path.Combine(charFolderParent.FullName, "Resource", "MotionSound");
					}
					characterAppearance.GetComponent<CharacterSound>()?.SetMotionSounds(extendedData.motionSoundList, motionSoundPath);
				}
				if (extendedData.atkEffectPivotDic != null)
				{
					Transform root = characterAppearance.atkEffectRoot;
					Transform parent = characterAppearance.atkEffectRoot.parent;
					if (extendedData.atkEffectPivotDic.TryGetValue("atkEffectRoot", out EffectPivot atkEffectRoot))
					{
						characterAppearance.atkEffectRoot.position = Vector3.zero;
						characterAppearance.atkEffectRoot.localPosition = atkEffectRoot.localPosition;
						characterAppearance.atkEffectRoot.localScale = atkEffectRoot.localScale;
						characterAppearance.atkEffectRoot.localEulerAngles = atkEffectRoot.localEulerAngles;
						if (!atkEffectRoot.isNested)
						{
							root = parent;
						}
					}
					if (extendedData.atkEffectPivotDic.TryGetValue("atkEffectPivot_H", out EffectPivot atkEffectPivot_H))
					{
						characterAppearance.atkEffectPivot_H = CreateTransform(atkEffectPivot_H.isNested ? root : parent, "atkEffectPivot_H", atkEffectPivot_H);
					}
					if (extendedData.atkEffectPivotDic.TryGetValue("atkEffectPivot_J", out EffectPivot atkEffectPivot_J))
					{
						characterAppearance.atkEffectPivot_J = CreateTransform(atkEffectPivot_J.isNested ? root : parent, "atkEffectPivot_J", atkEffectPivot_J);
					}
					if (extendedData.atkEffectPivotDic.TryGetValue("atkEffectPivot_Z", out EffectPivot atkEffectPivot_Z))
					{
						characterAppearance.atkEffectPivot_Z = CreateTransform(atkEffectPivot_Z.isNested ? root : parent, "atkEffectPivot_Z", atkEffectPivot_Z);
					}
					if (extendedData.atkEffectPivotDic.TryGetValue("atkEffectPivot_G", out EffectPivot atkEffectPivot_G))
					{
						characterAppearance.atkEffectPivot_G = CreateTransform(atkEffectPivot_G.isNested ? root : parent, "atkEffectPivot_G", atkEffectPivot_G);
					}
					if (extendedData.atkEffectPivotDic.TryGetValue("atkEffectPivot_E", out EffectPivot atkEffectPivot_E))
					{
						characterAppearance.atkEffectPivot_E = CreateTransform(atkEffectPivot_E.isNested ? root : parent, "atkEffectPivot_E", atkEffectPivot_E);
					}
					if (extendedData.atkEffectPivotDic.TryGetValue("atkEffectPivot_S", out EffectPivot atkEffectPivot_S))
					{
						characterAppearance.atkEffectPivot_S = CreateTransform(atkEffectPivot_S.isNested ? root : parent, "atkEffectPivot_S", atkEffectPivot_S);
					}
					if (extendedData.atkEffectPivotDic.TryGetValue("atkEffectPivot_F", out EffectPivot atkEffectPivot_F))
					{
						characterAppearance.atkEffectPivot_F = CreateTransform(atkEffectPivot_F.isNested ? root : parent, "atkEffectPivot_F", atkEffectPivot_F);
					}
				}
				if (extendedData.specialMotionPivotDic != null && extendedData.specialMotionPivotDic.Count > 0)
				{
					Transform SpecialPivot = CreateTransform(characterAppearance.transform, "SpecialPivot", Vector3.zero, new Vector3(1, 1, 1), Vector3.zero);
					if (characterAppearance._specialMotionPivotList == null)
					{
						characterAppearance._specialMotionPivotList = new List<CharacterAppearance.MotionPivot>();
					}
					foreach (KeyValuePair<ActionDetail, EffectPivot> keyValuePair in extendedData.specialMotionPivotDic)
					{
						characterAppearance._specialMotionPivotList.Add(new CharacterAppearance.MotionPivot()
						{
							motion = keyValuePair.Key,
							pivot = CreateTransform(SpecialPivot, "Special_" + keyValuePair.Key.ToString(), keyValuePair.Value)
						});
					}
				}
			}
			return true;
		}

		[HarmonyPatch(typeof(WorkshopSkinDataSetter), nameof(WorkshopSkinDataSetter.SetData), new Type[] { typeof(WorkshopSkinData) })]
		[HarmonyPostfix]
		[HarmonyPriority(Priority.High)]
		static void WorkshopSkinDataSetter_SetData_Postfix(WorkshopSkinDataSetter __instance, WorkshopSkinData data)
		{
			if (!(__instance is UIWorkshopSkinDataSetter))
			{
				CharacterAppearance characterAppearance = __instance.gameObject.GetComponent<CharacterAppearance>();
				characterAppearance._resourceName = data.dataName;
				characterAppearance.Initialize(data.dataName);
			}
		}
		static Transform CreateTransform(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Vector3 localEulerAngles)
		{
			try
			{
				Transform transform = new GameObject(name).transform;
				transform.SetParent(parent);
				transform.localPosition = localPosition;
				transform.localScale = localScale;
				transform.localEulerAngles = localEulerAngles;
				return transform;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			return null;
		}
		static Transform CreateTransform(Transform parent, string name, EffectPivot pivotData)
		{
			try
			{
				Transform transform = new GameObject(name).transform;
				transform.SetParent(parent);
				transform.localPosition = pivotData.localPosition;
				transform.localScale = pivotData.localScale;
				transform.localEulerAngles = pivotData.localEulerAngles;
				return transform;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			return null;
		}

		[HarmonyPatch(typeof(WorkshopSkinDataSetter), nameof(WorkshopSkinDataSetter.SetMotionData))]
		[HarmonyPrefix]
		[HarmonyPriority(Priority.HigherThanNormal)]
		static bool WorkshopSkinDataSetter_SetMotionData_Prefix(ActionDetail motion, ClothCustomizeData data, WorkshopSkinDataSetter __instance)
		{
			try
			{
				CharacterMotion characterMotion = __instance.Appearance.GetCharacterMotion(motion);
				if (characterMotion == null)
				{
					return true;
				}
				if (data is ExtendedClothCustomizeData data1)
				{
					if (!characterMotion.name.StartsWith("Custom_Extended_"))
					{
						__instance._appearance._motionList.RemoveAll(charMotion => charMotion.actionDetail == motion);
						__instance._appearance._characterMotionDic.Remove(motion);
						var parent = characterMotion.transform.parent;
						UnityEngine.Object.Destroy(characterMotion);
						characterMotion = UnityEngine.Object.Instantiate(XLRoot.CustomAppearancePrefab.GetComponent<CharacterAppearance>().GetCharacterMotion(ActionDetail.Default), parent);
						characterMotion.name = $"Custom_Extended_{motion}";
						characterMotion.actionDetail = motion;
						__instance._appearance._motionList.Add(characterMotion);
						__instance._appearance._characterMotionDic[motion] = characterMotion;
						characterMotion.gameObject.SetActive(false);
					}
					Transform transformHead = null;
					Transform transformMid = null;
					Transform transformBack = null;
					Transform transformFront = null;
					Transform transformMidSkin = null;
					Transform transformBackSkin = null;
					Transform transformFrontSkin = null;
					Transform transformEffect = null;
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
							case "Customize_Renderer_Effect":
								transformEffect = transform;
								break;
						}
					}
					var partRenderer = new SkinPartRenderer { action = motion };
					SpriteRenderer spriteRendererMid = transformMid?.GetComponent<SpriteRenderer>();
					if (spriteRendererMid)
					{
						spriteRendererMid.sprite = data1.sprite;
						spriteRendererMid.gameObject.SetActive(data1.sprite);
						partRenderer.rear = spriteRendererMid;
					}
					SpriteRenderer spriteRendererFront = transformFront?.GetComponent<SpriteRenderer>();
					if (spriteRendererFront)
					{
						spriteRendererFront.sprite = data1.frontSprite;
						spriteRendererFront.gameObject.SetActive(data1.frontSprite);
						partRenderer.front = spriteRendererFront;
					}
					SpriteRenderer spriteRendererBack = transformBack?.GetComponent<SpriteRenderer>();
					if (spriteRendererBack)
					{
						spriteRendererBack.sprite = data1.backSprite;
						spriteRendererBack.gameObject.SetActive(data1.backSprite);
						partRenderer.rearest = spriteRendererBack;
					}
					SpriteRenderer spriteRendererBackSkin = transformBackSkin?.GetComponent<SpriteRenderer>();
					if (spriteRendererBackSkin)
					{
						spriteRendererBackSkin.sprite = data1.backSkinSprite;
						if (data1.backSkinSprite == null)
						{
							spriteRendererBackSkin.gameObject.SetActive(false);
						}
						partRenderer.rearestSkin = spriteRendererBackSkin;
					}
					SpriteRenderer spriteRendererMidSkin = transformMidSkin?.GetComponent<SpriteRenderer>();
					if (spriteRendererMidSkin)
					{
						spriteRendererMidSkin.sprite = data1.skinSprite;
						spriteRendererMidSkin.gameObject.SetActive(data1.skinSprite);
						partRenderer.rearSkin = spriteRendererMidSkin;
					}
					SpriteRenderer spriteRendererFrontSkin = transformFrontSkin?.GetComponent<SpriteRenderer>();
					if (spriteRendererFrontSkin)
					{
						spriteRendererFrontSkin.sprite = data1.frontSkinSprite;
						spriteRendererFrontSkin.gameObject.SetActive(data1.frontSkinSprite);
						partRenderer.frontSkin = spriteRendererFrontSkin;
					}
					SpriteRenderer spriteRendererEffect = transformEffect?.GetComponent<SpriteRenderer>();
					if (transformEffect)
					{
						spriteRendererEffect.sprite = data1.effectSprite;
						spriteRendererEffect.gameObject.SetActive(data1.effectSprite);
						partRenderer.effect = spriteRendererEffect;
					}
					if (transformHead)
					{
						transformHead.localPosition = data1.headPivot.localPosition;
						transformHead.localScale = data1.headPivot.localScale;
						transformHead.localRotation = Quaternion.Euler(data1.headPivot.localEulerAngles);
						transformHead.gameObject.SetActive(data.headEnabled);
					}
					ExtendedCharacterMotion faceData = characterMotion.GetComponent<ExtendedCharacterMotion>();
					if (faceData == null)
					{
						faceData = characterMotion.gameObject.AddComponent<ExtendedCharacterMotion>();
					}
					faceData.faceOverride = data1.faceOverride;
					if (data1.additionalPivots != null)
					{
						if (characterMotion.additionalPivotList == null)
						{
							characterMotion.additionalPivotList = new List<Transform>();
						}
						else
						{
							characterMotion.additionalPivotList.Clear();
						}
						for (var i = 0; i < data1.additionalPivots.Count; i++)
						{
							EffectPivot pivot = data1.additionalPivots[i];
							characterMotion.additionalPivotList.Add(CreateTransform(characterMotion.transform, "AdditionalPivot" + i, pivot));
						}
					}
					__instance.parts.Add(motion, partRenderer);
					characterMotion.motionDirection = data.direction;
					return false;
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			return true;
		}

		[HarmonyPatch(typeof(WorkshopSkinDataSetter), nameof(WorkshopSkinDataSetter.SetOrder))]
		[HarmonyPostfix]
		static void WorkshopSkinDataSetter_SetOrder_Postfix(CustomizedAppearance ca, WorkshopSkinDataSetter __instance)
		{
			foreach (KeyValuePair<ActionDetail, WorkshopSkinDataSetter.PartRenderer> keyValuePair in __instance.parts)
			{
				if (keyValuePair.Value is SkinPartRenderer partRenderer)
				{
					int aboveRearHair = ca.GetRendererOrder(CharacterAppearanceType.RearHair, keyValuePair.Key);
					int belowRearHair = aboveRearHair;
					aboveRearHair = Math.Max(aboveRearHair, 3) + 1;
					belowRearHair--;
					int aboveFrontHair = ca.GetRendererOrder(CharacterAppearanceType.FrontHair, keyValuePair.Key) + 100;
					int belowHead = ca.GetRendererOrder(CharacterAppearanceType.Head, keyValuePair.Key);
					belowHead--;
					SpriteRenderer rearest = partRenderer.rearest;
					bool rearestExists = rearest && rearest.sprite;
					SpriteRenderer rearestSkin = partRenderer.rearestSkin;
					bool rearestSkinExists = rearestSkin && rearestSkin.sprite;
					SpriteRenderer rear = partRenderer.rear;
					bool rearExists = rear && rear.sprite;
					SpriteRenderer rearSkin = partRenderer.rearSkin;
					bool rearSkinExists = rearSkin && rearSkin.sprite;
					SpriteRenderer front = partRenderer.front;
					bool frontExists = front && front.sprite;
					SpriteRenderer frontSkin = partRenderer.frontSkin;
					bool frontSkinExists = frontSkin && frontSkin.sprite;
					SpriteRenderer effect = partRenderer.effect;
					SpriteRenderer nonEffect = frontSkinExists ? frontSkin : frontExists ? front : rearSkinExists ? rearSkin : rearExists ? rear : rearestSkinExists ? rearestSkin : rearest;
					if (rearExists || rearSkinExists)
					{
						if (rearExists && rearSkinExists)
						{
							belowHead--;
							rearSkin.sortingOrder = Math.Min(belowHead, aboveRearHair) + 1;
							rear.sortingOrder = Math.Min(belowHead, aboveRearHair);
						}
						else if (rearExists)
						{
							rear.sortingOrder = Math.Min(belowHead, aboveRearHair);
						}
						else
						{
							rearSkin.sortingOrder = Math.Min(belowHead, aboveRearHair);
						}
						belowHead--;
					}
					if (frontExists)
					{
						front.sortingOrder = aboveFrontHair;
						aboveFrontHair++;
					}
					if (frontSkinExists)
					{
						frontSkin.sortingOrder = aboveFrontHair;
						aboveFrontHair++;
					}
					if (nonEffect && nonEffect.sortingLayerName == "Effect")
					{
						if (effect && effect.sprite && effect.sortingOrder < aboveFrontHair)
						{
							effect.sortingOrder = aboveFrontHair;
						}
					}
					belowRearHair = Math.Min(belowHead, belowRearHair);
					if (rearestSkinExists)
					{
						rearestSkin.sortingOrder = belowRearHair;
						belowRearHair--;
					}
					if (rearestExists)
					{
						rearest.sortingOrder = belowRearHair;
					}
				}
			}
		}

		[HarmonyPatch(typeof(CharacterAppearance), nameof(CharacterAppearance.InitCustomData))]
		[HarmonyPrefix]
		static void CharacterAppearance_InitCustomData_Prefix(CharacterAppearance __instance, ref UnitCustomizingData customizeData, ref Dictionary<GiftPosition, string> __state)
		{
			if (!customizeData.UseCustomData)
			{
				var container = __instance.GetComponent<CharacterFaceData>();
				if (container != null && container.faceData != null)
				{
					var faceData = container.faceData;
					customizeData = new UnitCustomizingData(customizeData.specialCustomID, true)
					{
						frontHairID = faceData.TryGetId(CustomizeType.FrontHair),
						backHairID = faceData.TryGetId(CustomizeType.RearHair),
						eyeID = faceData.TryGetId(CustomizeType.Eye),
						browID = faceData.TryGetId(CustomizeType.Brow),
						mouthID = faceData.TryGetId(CustomizeType.Mouth),
						hairColor = faceData.TryGetColor(CustomizeColor.HairColor),
						eyeColor = faceData.TryGetColor(CustomizeColor.EyeColor),
						skinColor = faceData.TryGetColor(CustomizeColor.SkinColor)
					};
					if (faceData.customVisualGifts.Count > 0)
					{
						__state = faceData.customVisualGifts;
					}
				}
			}
		}

		[HarmonyPatch(typeof(CharacterAppearance), nameof(CharacterAppearance.InitCustomData))]
		[HarmonyPostfix]
		static void CharacterAppearance_InitCustomData_Postfix(CharacterAppearance __instance, ref Dictionary<GiftPosition, string> __state)
		{
			__instance.GetComponent<WorkshopSkinDataSetter>()?.LateInit();
			if (__state != null && __state.Count > 0)
			{
				foreach (var kvp in __state)
				{
					__instance.SetTemporaryGift(kvp.Value, kvp.Key, false);
				}
				__instance.RefreshAppearanceByGifts();
			}
		}
	}
}
