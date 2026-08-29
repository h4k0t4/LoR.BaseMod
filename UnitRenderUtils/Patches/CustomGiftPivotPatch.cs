using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using static System.Reflection.Emit.OpCodes;
using static HarmonyLib.AccessTools;

namespace ExtendedLoader
{
	[HarmonyPatch]
	class CustomGiftPivotPatch
	{
		[HarmonyPatch(typeof(CharacterAppearance), nameof(CharacterAppearance.CreateGiftData))]
		[HarmonyPatch(typeof(CharacterAppearance), nameof(CharacterAppearance.InitGiftData))]
		[HarmonyPatch(typeof(CharacterAppearance), nameof(CharacterAppearance.SetTemporaryGift))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> CharacterAppearance_CreateGiftData_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = instructions.ToList();
			var headField = Field(typeof(CharacterAppearance), nameof(CharacterAppearance._customAppearance));
			var uoCmp = Method(typeof(UnityEngine.Object), "op_Equality", new Type[] { typeof(UnityEngine.Object), typeof(UnityEngine.Object) });
			var helper = Method(typeof(CustomGiftPivotPatch), nameof(CustomGiftPivotPatch.CheckAnyGiftPivot));
			for (int i = 0; i < codes.Count - 3; i++)
			{
				if (codes[i].LoadsField(headField) && codes[i + 1].opcode == Ldnull && codes[i + 2].Calls(uoCmp))
				{
					codes.InsertRange(i + 3, new CodeInstruction[]
					{
						new CodeInstruction(Ldarg_0),
						new CodeInstruction(Call, helper)
					});
					i += 4;
				}
			}
			return codes;
		}

		static bool CheckAnyGiftPivot(bool headMissing, CharacterAppearance character)
		{
			return headMissing && !(character._motionList != null && character._motionList.Exists(motion =>
				motion.giftPivotList != null && motion.giftPivotList.Count > 0));
		}

		[HarmonyPatch(typeof(CharacterAppearance), nameof(CharacterAppearance.ChangeMotion))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> CharacterAppearance_ChangeMotion_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = instructions.ToList();
			var headField = Field(typeof(CharacterAppearance), nameof(CharacterAppearance._customAppearance));
			var uoCmp = Method(typeof(UnityEngine.Object), "op_Inequality", new Type[] { typeof(UnityEngine.Object), typeof(UnityEngine.Object) });
			var helper = Method(typeof(CustomGiftPivotPatch), nameof(CustomGiftPivotPatch.CheckAnyGiftPivotByCurrentMotion));
			for (int i = 0; i < codes.Count - 3; i++)
			{
				if (codes[i].LoadsField(headField) && codes[i + 1].opcode == Ldnull && codes[i + 2].Calls(uoCmp))
				{
					codes.InsertRange(i + 3, new CodeInstruction[]
					{
						new CodeInstruction(Ldarg_0),
						new CodeInstruction(Call, helper)
					});
					break;
				}
			}
			return codes;
		}

		static bool CheckAnyGiftPivotByCurrentMotion(bool headExists, CharacterAppearance character)
		{
			return headExists || character._currentMotion.giftPivotList != null && character._currentMotion.giftPivotList.Count != 0;
		}

		[HarmonyPatch(typeof(GiftAppearance), nameof(GiftAppearance.RefreshAppearance))]
		[HarmonyPrefix]
		static void GiftAppearance_RefreshAppearance_Prefix(GiftAppearance __instance, CustomizedAppearance customized, ref CharacterMotion motion)
		{
			bool check = false;
			if (customized)
			{
				check = true;
			}
			else
			{
				if (!motion)
				{
					var appearance = __instance.GetComponentsInParent<CharacterAppearance>(true).FirstOrDefault();
					motion = appearance._motionList.Find(m => m.actionDetail == ActionDetail.Standing);
					if (!motion)
					{
						motion = appearance._motionList.Find(m => m.actionDetail == ActionDetail.Default);
						if (!motion)
						{
							motion = appearance._motionList.FirstOrDefault();
						}
					}
				}
				check = motion.giftPivotList != null && motion.giftPivotList.Exists(p => p.giftType == __instance.GetGiftType());
			}
			__instance.gameObject.SetActive(check);
		}

		[HarmonyPatch(typeof(GiftAppearance), nameof(GiftAppearance.RefreshAppearance))]
		[HarmonyPostfix]
		static void GiftAppearance_RefreshAppearance_Postfix(GiftAppearance __instance, CharacterMotion motion)
		{
			if (motion.giftPivotList != null)
			{
				var pivot = motion.giftPivotList.Find(p => p.giftType == __instance.GetGiftType());
				if (pivot.rootTransform)
				{
					__instance.transform.SetParent(pivot.rootTransform, false);
				}
			}
		}

		[HarmonyPatch(typeof(GiftAppearance_Aura), nameof(GiftAppearance_Aura.RefreshAppearance))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> GiftAppearance_Aura_RefreshAppearance_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilgen)
		{
			var codes = instructions.ToList();
			var appearanceGetter = PropertyGetter(typeof(CustomizedAppearance), nameof(CustomizedAppearance.appearance));
			var helper = Method(typeof(CustomGiftPivotPatch), nameof(CustomGiftPivotPatch.GetAppearanceByAnything));
			LocalBuilder local = null;
			for (int i = 0; i < codes.Count - 2; i++)
			{
				if (codes[i].opcode == Ldarg_1 && codes[i + 1].Calls(appearanceGetter))
				{
					if (local == null)
					{
						local = ilgen.DeclareLocal(typeof(CharacterAppearance));
						codes[i + 1] = new CodeInstruction(Ldarg_2);
						codes.InsertRange(i + 2, new CodeInstruction[]
						{
							new CodeInstruction(Ldarg_0),
							new CodeInstruction(Call, helper),
							new CodeInstruction(Dup),
							new CodeInstruction(Stloc, local)
						});
					}
					else
					{
						codes[i] = new CodeInstruction(Ldloc, local);
						codes.RemoveAt(i + 1);
					}
				}
			}
			return codes;
		}

		static CharacterAppearance GetAppearanceByAnything(CustomizedAppearance head, CharacterMotion motion, GiftAppearance giftAppearance)
		{
			if (head && head.appearance)
			{
				return head.appearance;
			}
			Component searchBase = motion ? (Component)motion : giftAppearance;
			return searchBase.GetComponentsInParent<CharacterAppearance>(true).FirstOrDefault();
		}

		[HarmonyPatch(typeof(CharacterAppearance), nameof(CharacterAppearance.Initialize))]
		[HarmonyPrefix]
		static void CharacterAppearance_Initialize_Prefix(CharacterAppearance __instance)
		{
			switch (__instance.name)
			{
				case "[Prefab]Appearance_Roland(Clone)":
				case "[Prefab]DefaultMotion_Roland(Clone)":
					FixGiftPivots_Roland(__instance);
					break;
				case "[Prefab]Appearance_Binah(Clone)":
				case "[Prefab]DefaultMotion_Binah(Clone)":
					FixGiftPivots_Binah(__instance);
					FixLayers(__instance);
					break;
				case "[Prefab]Appearance_BlackSilence(Clone)":
				case "[Prefab]DefaultMotion_BlackSilence(Clone)":
				case "[Prefab]Appearance_BlackSilenceMask(Clone)":
				case "[Prefab]DefaultMotion_BlackSilenceMask(Clone)":
					FixGiftPivots_BlackSilence(__instance);
					FixLayers(__instance);
					break;
			}
		}

		static void FixGiftPivots_Roland(CharacterAppearance appearance)
		{
			foreach (var motion in appearance._motionList)
			{
				motion.giftPivotList.Clear();
			}
		}

		static void FixGiftPivots_Binah(CharacterAppearance appearance)
		{
			foreach (var motion in appearance._motionList)
			{
				if (motion.giftPivotList == null)
				{
					motion.giftPivotList = new List<CharacterMotion.GiftPivotData>();
				}
				else if (motion.giftPivotList.Count > 0)
				{
					continue;
				}
				Vector2 position;
				float rotation = 0f;
				switch (motion.actionDetail)
				{
					case ActionDetail.Default:
					case ActionDetail.Standing:
					case ActionDetail.S4:
						position = new Vector2(-0.116f, 3.37f);
						break;
					case ActionDetail.Guard:
						position = new Vector2(1.405f, 3.13f);
						break;
					case ActionDetail.Evade:
						position = new Vector2(1.61f, 3.452f);
						break;
					case ActionDetail.Damaged:
						position = new Vector2(0.425f, 3.545f);
						rotation = 9.2f;
						break;
					case ActionDetail.Slash:
					case ActionDetail.Penetrate:
					case ActionDetail.Special:
					case ActionDetail.S1:
					case ActionDetail.S2:
					case ActionDetail.S3:
					case ActionDetail.S5:
						position = new Vector2(-0.985f, 3.37f);
						break;
					case ActionDetail.Hit:
						position = new Vector2(-1.14f, 3.13f);
						break;
					case ActionDetail.Move:
						position = new Vector2(-1.74f, 3.295f);
						break;
					default:
						continue;
				}
				var giftPivot = (new GameObject("GiftPivot") { transform = { parent = motion.transform } }).transform;
				giftPivot.localPosition = position;
				giftPivot.localRotation = Quaternion.Euler(0f, 0f, rotation);
				giftPivot.localScale = Vector3.one;
				for (int type = 0; type < 10; type++)
				{
					motion.giftPivotList.Add(new CharacterMotion.GiftPivotData { giftType = (GiftPosition)type, rootTransform = giftPivot });
				}
			}
		}

		static void FixGiftPivots_BlackSilence(CharacterAppearance appearance)
		{
			foreach (var motion in appearance._motionList)
			{
				if (motion.giftPivotList == null)
				{
					motion.giftPivotList = new List<CharacterMotion.GiftPivotData>();
				}
				else if (motion.giftPivotList.Count > 0)
				{
					continue;
				}
				Vector2 position;
				float rotation = 0f;
				switch (motion.actionDetail)
				{
					case ActionDetail.Default:
					case ActionDetail.Standing:
						position = new Vector2(-0.245f, 4.025f);
						break;
					case ActionDetail.Guard:
						position = new Vector2(-0.34f, 3.57f);
						rotation = 4.6f;
						break;
					case ActionDetail.Evade:
						position = new Vector2(-0.275f, 3.835f);
						rotation = 6.2f;
						break;
					case ActionDetail.Damaged:
						position = new Vector2(-0.45f, 3.86f);
						rotation = 9.3f;
						break;
					case ActionDetail.Slash:
					case ActionDetail.S15:
						position = new Vector2(-0.88f, 2.955f);
						rotation = -7f;
						break;
					case ActionDetail.Penetrate:
						position = new Vector2(-0.9f, 2.98f);
						rotation = -4.5f;
						break;
					case ActionDetail.Hit:
						position = new Vector2(-0.53f, 3.695f);
						rotation = -7.5f;
						break;
					case ActionDetail.Move:
						position = new Vector2(-1.465f, 2.955f);
						rotation = 2.3f;
						break;
					case ActionDetail.S1:
						position = new Vector2(-0.29f, 3.99f);
						rotation = -10f;
						break;
					case ActionDetail.S2:
						position = new Vector2(-0.26f, 4.03f);
						rotation = 0.8f;
						break;
					case ActionDetail.S3:
						position = new Vector2(-0.66f, 3.4f);
						rotation = 7.3f;
						break;
					case ActionDetail.S4:
						position = new Vector2(-0.71f, 3.365f);
						rotation = 9.8f;
						break;
					case ActionDetail.S5:
					case ActionDetail.S7:
						position = new Vector2(-0.9f, 2.97f);
						rotation = -3f;
						break;
					case ActionDetail.S6:
						position = new Vector2(-0.85f, 3.06f);
						rotation = -3f;
						break;
					case ActionDetail.S8:
						position = new Vector2(-0.68f, 2.72f);
						rotation = -4.3f;
						break;
					case ActionDetail.S9:
						position = new Vector2(-0.14f, 3.375f);
						rotation = 2f;
						break;
					case ActionDetail.S10:
						position = new Vector2(-0.65f, 2.745f);
						rotation = -2.3f;
						break;
					case ActionDetail.S11:
						position = new Vector2(-0.545f, 3.125f);
						rotation = -5f;
						break;
					case ActionDetail.S12:
					case ActionDetail.S13:
						position = new Vector2(-0.63f, 3.555f);
						rotation = -5f;
						break;
					case ActionDetail.S14:
						position = new Vector2(-0.6f, 3.465f);
						rotation = 9.3f;
						break;
					default:
						continue;
				}
				var giftPivot = (new GameObject("GiftPivot") { transform = { parent = motion.transform } }).transform;
				giftPivot.localPosition = position;
				giftPivot.localRotation = Quaternion.Euler(0f, 0f, rotation);
				giftPivot.localScale = Vector3.one;
				for (int type = 0; type < 10; type++)
				{
					motion.giftPivotList.Add(new CharacterMotion.GiftPivotData { giftType = (GiftPosition)type, rootTransform = giftPivot });
				}
			}
		}

		static void FixLayers(CharacterAppearance appearance)
		{
			var sprites = new List<SpriteSet>();
			foreach (var motion in appearance._motionList)
			{
				sprites.AddRange(motion.motionSpriteSet);
				sprites.Sort((x, y) => x.sprRenderer.sortingOrder - y.sprRenderer.sortingOrder);
				int minNextOrder = 0;
				int count = sprites.Count;
				for (int i = 0; i < count - 1; i++)
				{
					if (sprites[i].sprType == CharacterAppearanceType.FrontHair && sprites[i + 1].sprType == CharacterAppearanceType.Face)
					{
						(sprites[i].sprRenderer.sortingOrder, sprites[i + 1].sprRenderer.sortingOrder) = (sprites[i + 1].sprRenderer.sortingOrder, sprites[i].sprRenderer.sortingOrder);
						(sprites[i], sprites[i + 1]) = (sprites[i + 1], sprites[i]);
					}
					if (sprites[i].sprType == CharacterAppearanceType.Face)
					{
						if (sprites[i + 1].sprType != CharacterAppearanceType.Face)
						{
							minNextOrder = sprites[i].sprRenderer.sortingOrder + 3;
						}
					}
					else
					{
						if (minNextOrder > 0)
						{
							if (minNextOrder > sprites[i].sprRenderer.sortingOrder)
							{
								sprites[i].sprRenderer.sortingOrder = minNextOrder;
								minNextOrder++;
							}
							else
							{
								minNextOrder = 0;
							}
						}
						if (sprites[i].sprType == CharacterAppearanceType.FrontHair)
						{
							if (sprites[i + 1].sprType != CharacterAppearanceType.FrontHair)
							{
								minNextOrder = sprites[i].sprRenderer.sortingOrder + 6;
							}
						}
					}
				}
				if (minNextOrder > 0 && minNextOrder > sprites[count - 1].sprRenderer.sortingOrder)
				{
					sprites[count - 1].sprRenderer.sortingOrder = minNextOrder;
				}
				sprites.Clear();
			}
		}

		[HarmonyPatch(typeof(CharacterAppearance), nameof(CharacterAppearance.RefreshAppearanceByGifts))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> CharacterAppearance_RefreshAppearanceByGifts_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var field = Field(typeof(CharacterAppearance), nameof(CharacterAppearance._customAppearance));
			var codes = instructions.ToList();
			for (int i = 0; i < codes.Count; i++)
			{
				if (codes[i].LoadsField(field))
				{
					codes.InsertRange(i, new CodeInstruction[]
					{
						new CodeInstruction(Ldarg_0),
						new CodeInstruction(Ldloc_2),
						new CodeInstruction(Ldloc_3),
						new CodeInstruction(Ldloc_S, 4),
						new CodeInstruction(Call, Method(typeof(CustomGiftPivotPatch), nameof(CustomGiftPivotPatch.CheckMotionSprites)))
					});
					break;
				}
			}
			return codes;
		}

		static void CheckMotionSprites(CharacterAppearance characterAppearance, bool hideHead, bool hideFrontHair, bool hideRearHair)
		{
			if (characterAppearance._currentMotion)
			{
				bool enableHead = !hideHead;
				bool enableFront = !hideFrontHair;
				bool enableRear = !hideRearHair;
				foreach (var spr in characterAppearance._currentMotion.motionSpriteSet)
				{
					switch (spr.sprType)
					{
						case CharacterAppearanceType.FrontHair:
							TrySetRendererEnabled(spr.sprRenderer, enableFront);
							break;
						case CharacterAppearanceType.RearHair:
							TrySetRendererEnabled(spr.sprRenderer, enableRear);
							break;
						case CharacterAppearanceType.Head:
							TrySetRendererEnabled(spr.sprRenderer, enableHead);
							break;
					}
				}
			}
		}

		static void TrySetRendererEnabled(SpriteRenderer renderer, bool enabled)
		{
			var cache = renderer.GetComponent<MotionSpriteDefaultStateCache>();
			if (!cache)
			{
				cache = renderer.gameObject.AddComponent<MotionSpriteDefaultStateCache>();
				cache.enable = renderer.enabled;
			}
			renderer.enabled = enabled && cache.enable;
		}

		class MotionSpriteDefaultStateCache : MonoBehaviour
		{
			public bool enable;
		}

		[HarmonyPatch(typeof(CharacterAppearance), nameof(CharacterAppearance.ChangeMotion))]
		[HarmonyPostfix]
		static void CharacterAppearance_ChangeMotion_Postfix(CharacterAppearance __instance)
		{
			__instance.RefreshAppearanceByGifts();
		}
	}
}
