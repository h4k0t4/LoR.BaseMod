using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UI;
using UnityEngine;
using Workshop;

namespace ExtendedLoader
{
    public class CustomProjectionLoadPatch
    {
        [HarmonyPatch(typeof(SdCharacterUtil), nameof(SdCharacterUtil.CreateSkin))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> CreateSkinTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            Label label = default;
            bool injectedCustomCheck = false;
            bool obtainedFlag = false;
            LocalBuilder local = null;
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (Equals(codes[i].opcode, OpCodes.Callvirt) && Equals(codes[i].operand, AccessTools.PropertyGetter(typeof(BookModel), nameof(BookModel.IsWorkshop))))
                {
                    label = (Label)codes[i + 1].operand;
                }
                if (Equals(codes[i].opcode, OpCodes.Ldfld) && !injectedCustomCheck && Equals(codes[i].operand, AccessTools.Field(typeof(BookXmlInfo), nameof(BookXmlInfo.CharacterSkin))))
                {
                    injectedCustomCheck = true;
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(BookXmlInfo), nameof(BookXmlInfo.skinType)));
                    yield return new CodeInstruction(OpCodes.Ldstr, "Custom");
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(string), "op_Equality", new Type[] { typeof(string), typeof(string) }));
                    yield return new CodeInstruction(OpCodes.Brfalse, label);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(UnitDataModel), nameof(UnitDataModel.CustomBookItem)));
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(BookModel), nameof(BookModel.ClassInfo)));
                }
                if (Equals(codes[i].opcode, OpCodes.Callvirt) && Equals(codes[i].operand, AccessTools.PropertyGetter(typeof(UnitDataModel), nameof(UnitDataModel.bookItem))))
                {
                    codes[i] = new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(UnitDataModel), nameof(UnitDataModel.CustomBookItem)));
                }
                yield return codes[i];
                if (Equals(codes[i].opcode, OpCodes.Callvirt) && Equals(codes[i].operand,
                    AccessTools.Method(typeof(WorkshopSkinDataSetter), nameof(WorkshopSkinDataSetter.SetData), new Type[] { typeof(WorkshopSkinData) })))
                {
                    if (!obtainedFlag)
                    {
                        int j;
                        for (j = i + 1; j < codes.Count; j++)
                        {
                            local = codes[j].operand as LocalBuilder;
                            if (codes[j].Branches(out Label? _))
                            {
                                j = codes.Count;
                            }
                            if (local != null && local.LocalType == typeof(bool))
                            {
                                obtainedFlag = true;
                                break;
                            }
                        }
                        if (j == codes.Count)
                        {
                            Debug.Log("BaseMod XL: Failed to obtain LateInit flag for CreateSkin");
                        }

                    }
                    else
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_I4_1);
                        yield return new CodeInstruction(OpCodes.Stloc_S, local);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SdCharacterUtil), nameof(SdCharacterUtil.LoadAppearance))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> LoadAppearanceTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (Equals(codes[i].opcode, OpCodes.Callvirt) && Equals(codes[i].operand, AccessTools.PropertyGetter(typeof(UnitDataModel), nameof(UnitDataModel.bookItem))))
                {
                    codes[i] = new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(UnitDataModel), nameof(UnitDataModel.CustomBookItem)));
                }
                yield return codes[i];
            }
        }

        [HarmonyPatch(typeof(UICharacterRenderer), nameof(UICharacterRenderer.SetCharacter))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> SetCharacterTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            bool obtainedFlag = false;
            LocalBuilder local = null;
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (Equals(codes[i].opcode, OpCodes.Callvirt) && Equals(codes[i].operand, AccessTools.PropertyGetter(typeof(UnitDataModel), nameof(UnitDataModel.bookItem))))
                {
                    codes[i] = new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(UnitDataModel), nameof(UnitDataModel.CustomBookItem)));
                }
                yield return codes[i];
                if (Equals(codes[i].opcode, OpCodes.Callvirt) && Equals(codes[i].operand,
                    AccessTools.Method(typeof(WorkshopSkinDataSetter), nameof(WorkshopSkinDataSetter.SetData), new Type[] { typeof(WorkshopSkinData) })))
                {
                    if (!obtainedFlag)
                    {
                        int j;
                        for (j = i + 1; j < codes.Count; j++)
                        {
                            local = codes[j].operand as LocalBuilder;
                            if (codes[j].Branches(out Label? _))
                            {
                                j = codes.Count;
                            }
                            if (local != null && local.LocalType == typeof(bool))
                            {
                                obtainedFlag = true;
                                break;
                            }
                        }
                        if (j == codes.Count)
                        {
                            Debug.Log("BaseMod XL: failed to obtain LateInit flag for SetCharacter");
                        }

                    }
                    else
                    {
                        yield return new CodeInstruction(OpCodes.Ldc_I4_1);
                        yield return new CodeInstruction(OpCodes.Stloc_S, local);
                    }
                }
            }
        }
    }
}
