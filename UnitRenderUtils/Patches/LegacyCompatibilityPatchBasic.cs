using HarmonyLib;
using System;
using System.Reflection;
using UI;
using UnityEngine;

namespace ExtendedLoader
{
	internal class LegacyCompatibilityPatchBasic
	{
		public static void PrepareLegacy(Harmony h)
		{
			harmony = h;
			try
			{
				basemod = Assembly.Load("BaseMod");
			}
			catch {}
			if (basemod != null)
			{
				try
				{
					var giftMethod = basemod.GetType("BaseMod.Harmony_Patch")?.GetMethod("CharacterAppearance_CreateGiftData_Pre");
					if (giftMethod != null)
					{
						harmony.Patch(giftMethod, postfix: new HarmonyMethod(typeof(LegacyCompatibilityPatchBasic), nameof(FixCustomGiftPivot)));
					}
					if (Harmony.HasAnyPatches(BasemodHarmonyId))
					{
						UnpatchLegacy();
					}
					else
					{
						harmony.Patch(basemod.GetType("BaseMod.BaseModInitializer").GetMethod("OnInitializeMod"), finalizer: new HarmonyMethod(typeof(LegacyCompatibilityPatchBasic), nameof(UnpatchLegacy)));
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}
		}

		static void UnpatchLegacy()
		{
			if (basemod.GetType("ExtendedLoader.LorName") != null)
			{
				foreach (var target in legacyPrefixTargets)
				{
					harmony.Unpatch(target, HarmonyPatchType.Prefix, BasemodHarmonyId);
				}
				foreach (var target in legacyPostfixTargets)
				{
					harmony.Unpatch(target, HarmonyPatchType.Postfix, BasemodHarmonyId);
				}
			}
			foreach (var SummonLiberationHarmonyId in OldUnitUtilHarmonyIds)
			{
				if (Harmony.HasAnyPatches(SummonLiberationHarmonyId))
				{
					var harmonyHelper = new Harmony(SummonLiberationHarmonyId);
					var patchedMethods = harmonyHelper.GetPatchedMethods();
					foreach (var patchedMethod in patchedMethods)
					{
						if (legacySummonTypeTargets.Contains(patchedMethod.DeclaringType))
						{
							harmony.Unpatch(patchedMethod, HarmonyPatchType.All, SummonLiberationHarmonyId);
						}
					}
					foreach (var target in legacySummonMethodTargets)
					{
						harmony.Unpatch(target, HarmonyPatchType.All, SummonLiberationHarmonyId);
					}
				}
			}
		}

		static void FixCustomGiftPivot(CharacterAppearance __0, ref bool __result)
		{
			if (__0._customAppearance == null)
			{
				__result = true;
			}
		}

		static readonly MethodInfo[] legacyPrefixTargets = new MethodInfo[]
		{
			AccessTools.Method(typeof(CharacterSound), nameof(CharacterSound.LoadAudioCoroutine)),
			AccessTools.Method(typeof(UIEquipPageCustomizePanel), nameof(UIEquipPageCustomizePanel.ApplyFilterAll)),
			AccessTools.Method(typeof(UIEquipPageCustomizePanel), nameof(UIEquipPageCustomizePanel.UpdateEquipPageSlotList)),
		};

		static readonly MethodInfo[] legacyPostfixTargets = new MethodInfo[]
		{
			AccessTools.Method(typeof(UnitDataModel), nameof(UnitDataModel.LoadFromSaveData)),
		};

		static readonly Type[] legacySummonTypeTargets = new Type[]
		{
			typeof(BattleEmotionCoinUI),
			typeof(BattleEmotionRewardInfoUI),
			typeof(BattleUnitInfoManagerUI),
			typeof(UIBattleSettingPanel),
			typeof(UICharacterList),
			typeof(UICharacterRenderer),
			typeof(UIEnemyCharacterListPanel),
			typeof(UILibrarianCharacterListPanel),
		};

		static readonly MethodInfo[] legacySummonMethodTargets = new MethodInfo[]
		{
			AccessTools.Method(typeof(StageWaveModel), nameof(StageWaveModel.GetUnitBattleDataListByFormation)),
		};

		static Assembly basemod = null;
		static Harmony harmony = null;
		const string BasemodHarmonyId = "LOR.BaseMod";
		readonly static string[] OldUnitUtilHarmonyIds = new string[] { "LOR.SummonLiberation", "LOR.BigDLL4221HarmonyPatch_MOD", "LOR.1UtilLoader21341_MOD" };
	}
}
