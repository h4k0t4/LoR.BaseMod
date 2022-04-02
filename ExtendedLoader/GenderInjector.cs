using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UI;

namespace ExtendedLoader
{
    public class GenderInjector
    {
        [HarmonyPatch(typeof(BookModel), nameof(BookModel.GetThumbSprite))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> BookThumbTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (Equals(codes[i].opcode, OpCodes.Callvirt) && Equals(codes[i].operand,
                    AccessTools.Method(typeof(CustomizingBookSkinLoader), nameof(CustomizingBookSkinLoader.GetWorkshopBookSkinData), new Type[] { typeof(string), typeof(string) })))
                {
                    yield return new CodeInstruction(OpCodes.Ldstr, "_");
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(BookModel), nameof(BookModel.ClassInfo)));
                    yield return new CodeInstruction(OpCodes.Ldflda, AccessTools.Field(typeof(BookXmlInfo), nameof(BookXmlInfo.gender)));
                    yield return new CodeInstruction(OpCodes.Constrained, typeof(Gender));
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(object), nameof(object.ToString)));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), new Type[] { typeof(string), typeof(string) }));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SkinChangeExtensions), nameof(SkinChangeExtensions.GetWorkshopBookSkinData),
                        new Type[] { typeof(CustomizingBookSkinLoader), typeof(string), typeof(string), typeof(string) }));
                }
                else
                {
                    yield return codes[i];
                }
            }
        }

        [HarmonyPatch(typeof(BookXmlInfo), nameof(BookXmlInfo.GetThumbSprite))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> BookXmlThumbTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (Equals(codes[i].opcode, OpCodes.Callvirt) && Equals(codes[i].operand,
                    AccessTools.Method(typeof(CustomizingBookSkinLoader), nameof(CustomizingBookSkinLoader.GetWorkshopBookSkinData), new Type[] { typeof(string), typeof(string) })))
                {
                    yield return new CodeInstruction(OpCodes.Ldstr, "_");
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldflda, AccessTools.Field(typeof(BookXmlInfo), nameof(BookXmlInfo.gender)));
                    yield return new CodeInstruction(OpCodes.Constrained, typeof(Gender));
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(object), nameof(object.ToString)));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), new Type[] { typeof(string), typeof(string) }));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SkinChangeExtensions), nameof(SkinChangeExtensions.GetWorkshopBookSkinData),
                        new Type[] { typeof(CustomizingBookSkinLoader), typeof(string), typeof(string), typeof(string) }));
                }
                else
                {
                    yield return codes[i];
                }
            }
        }

        [HarmonyPatch(typeof(SdCharacterUtil), nameof(SdCharacterUtil.CreateSkin))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> CreateSkinTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (Equals(codes[i].opcode, OpCodes.Callvirt) && Equals(codes[i].operand,
                    AccessTools.Method(typeof(CustomizingBookSkinLoader), nameof(CustomizingBookSkinLoader.GetWorkshopBookSkinData), new Type[] { typeof(string), typeof(string) })))
                {
                    yield return new CodeInstruction(OpCodes.Ldstr, "_");
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldflda, AccessTools.Field(typeof(UnitDataModel), nameof(UnitDataModel.appearanceType)));
                    yield return new CodeInstruction(OpCodes.Constrained, typeof(Gender));
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(object), nameof(object.ToString)));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), new Type[] { typeof(string), typeof(string) }));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SkinChangeExtensions), nameof(SkinChangeExtensions.GetWorkshopBookSkinData),
                        new Type[] { typeof(CustomizingBookSkinLoader), typeof(string), typeof(string), typeof(string) }));
                }
                else
                {
                    yield return codes[i];
                }
            }
        }

        [HarmonyPatch(typeof(UICharacterRenderer), nameof(UICharacterRenderer.SetCharacter))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> SetCharacterTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                if (Equals(codes[i].opcode, OpCodes.Callvirt) && Equals(codes[i].operand,
                    AccessTools.Method(typeof(CustomizingBookSkinLoader), nameof(CustomizingBookSkinLoader.GetWorkshopBookSkinData), new Type[] { typeof(string), typeof(string) })))
                {
                    yield return new CodeInstruction(OpCodes.Ldstr, "_");
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldflda, AccessTools.Field(typeof(UnitDataModel), nameof(UnitDataModel.appearanceType)));
                    yield return new CodeInstruction(OpCodes.Constrained, typeof(Gender));
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(object), nameof(object.ToString)));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), new Type[] { typeof(string), typeof(string) }));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SkinChangeExtensions), nameof(SkinChangeExtensions.GetWorkshopBookSkinData),
                        new Type[] { typeof(CustomizingBookSkinLoader), typeof(string), typeof(string), typeof(string) }));
                }
                else
                {
                    yield return codes[i];
                }
            }
        }
    }
}
