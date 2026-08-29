using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Workshop;
using static System.Reflection.Emit.OpCodes;
using static HarmonyLib.AccessTools;

namespace ExtendedLoader
{
	[HarmonyPatch]
	internal class WorkshopSetterPatchBasic
	{
		[HarmonyPatch(typeof(WorkshopSkinDataSetter), nameof(WorkshopSkinDataSetter.SetData), new Type[] { typeof(WorkshopSkinData) })]
		[HarmonyPostfix]
		[HarmonyPriority(Priority.High)]
		static void WorkshopSkinDataSetter_SetData_Postfix(WorkshopSkinDataSetter __instance, WorkshopSkinData data)
		{
			WorkshopSkinDataCacher cache = __instance.GetComponent<WorkshopSkinDataCacher>();
			if (cache == null)
			{
				cache = __instance.gameObject.AddComponent<WorkshopSkinDataCacher>();
			}
			cache.data = data;
		}

		[HarmonyPatch(typeof(WorkshopSkinDataSetter), nameof(WorkshopSkinDataSetter.SetMotionData))]
		[HarmonyPrefix]
		[HarmonyPriority(Priority.VeryHigh)]
		static void WorkshopSkinDataSetter_SetMotionData_PrefixEarly(ActionDetail motion, WorkshopSkinDataSetter __instance)
		{
			try
			{
				CharacterMotion characterMotion = __instance.Appearance.GetCharacterMotion(motion);
				if (characterMotion == null)
				{
					Debug.LogError("MotionNull! " + motion.ToString());
					return;
				}
				foreach (Transform transform in characterMotion.transform)
				{
					if (transform.name == "Customize_Renderer")
					{
						SpriteRenderer spriteRenderer = transform.GetComponent<SpriteRenderer>();
						if (spriteRenderer && spriteRenderer.sortingOrder < 4)
						{
							spriteRenderer.sortingOrder = 4;
						}
						return;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		[HarmonyPatch(typeof(WorkshopSkinDataSetter), nameof(WorkshopSkinDataSetter.SetOrder))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> WorkshopSkinDataSetter_SetOrder_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var hookMethod = Method(typeof(Math), nameof(Math.Min), new Type[] { typeof(int), typeof(int) });
			var fixMethod = Method(typeof(WorkshopSetterPatchBasic), nameof(WorkshopSkinDataSetter_SetOrder_FixRearOrder));
			foreach (var instruction in instructions)
			{
				if (instruction.Calls(hookMethod))
				{
					yield return new CodeInstruction(Call, fixMethod);
				}
				yield return instruction;
			}
		}

		static int WorkshopSkinDataSetter_SetOrder_FixRearOrder(int order)
		{
			return Math.Max(order, 4);
		}

		[HarmonyPatch(typeof(CharacterSound), nameof(CharacterSound.LoadAudioCoroutine), MethodType.Enumerator)]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> CharacterSound_LoadAudioCoroutine_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			var enumeratorClass = original.DeclaringType;
			var internalEnumeratorField = enumeratorClass.GetFields(all).FirstOrDefault(t => t.FieldType == typeof(List<CharacterSound.ExternalSound>.Enumerator));
			var soundConstructor = Constructor(typeof(CharacterSound.Sound));
			var codes = instructions.ToList();
			for (int i = 0; i < codes.Count; i++)
			{
				yield return codes[i];
				if (codes[i].Is(Newobj, soundConstructor))
				{
					yield return new CodeInstruction(Dup);
					yield return new CodeInstruction(Ldarg_0);
					yield return new CodeInstruction(Ldflda, internalEnumeratorField);
					yield return new CodeInstruction(Call, PropertyGetter(typeof(List<CharacterSound.ExternalSound>.Enumerator), nameof(List<CharacterSound.ExternalSound>.Enumerator.Current)));
					yield return new CodeInstruction(Ldfld, Field(typeof(CharacterSound.ExternalSound), nameof(CharacterSound.ExternalSound.motion)));
					yield return new CodeInstruction(Stfld, Field(typeof(CharacterSound.Sound), nameof(CharacterSound.Sound.motion)));
				}
			}
		}
	}
}
