using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UI;

namespace ExtendedLoader
{
    public class CustomAppearancePrefabPatch
    {
        [HarmonyPatch(typeof(AssetBundleManagerRemake), nameof(AssetBundleManagerRemake.LoadCharacterPrefab_DefaultMotion))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> LoadCharacterPrefab_DefaultMotionTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                if (Equals(codes[i].opcode, OpCodes.Ldstr) && Equals(codes[i].operand, "Prefabs/Characters/[Prefab]Appearance_Custom"))
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(XLRoot), nameof(XLRoot.UICustomAppearancePrefab)));
                    i++;
                }
                else
                {
                    yield return codes[i];
                }
            }
        }

        [HarmonyPatch(typeof(AssetBundleManagerRemake), nameof(AssetBundleManagerRemake.LoadCharacterPrefab))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> LoadCharacterPrefabTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                if (Equals(codes[i].opcode, OpCodes.Ldstr) && Equals(codes[i].operand, "Prefabs/Characters/[Prefab]Appearance_Custom"))
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(XLRoot), nameof(XLRoot.CustomAppearancePrefab)));
                    i++;
                }
                else
                {
                    yield return codes[i];
                }
            }
        }

        [HarmonyPatch(typeof(AssetBundleManagerRemake), nameof(AssetBundleManagerRemake.LoadSdPrefab))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> LoadSdPrefabTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                if (Equals(codes[i].opcode, OpCodes.Ldstr) && Equals(codes[i].operand, "Prefabs/Characters/[Prefab]Appearance_Custom"))
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(XLRoot), nameof(XLRoot.CustomAppearancePrefab)));
                    i++;
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
            var codes = instructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                if (Equals(codes[i].opcode, OpCodes.Ldstr) && Equals(codes[i].operand, "Prefabs/Characters/[Prefab]Appearance_Custom"))
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(XLRoot), nameof(XLRoot.UICustomAppearancePrefab)));
                    i++;
                }
                else
                {
                    yield return codes[i];
                }
            }
        }

        [HarmonyPatch(typeof(SdCharacterUtil), nameof(SdCharacterUtil.LoadAppearance))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> LoadAppearanceTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                if (Equals(codes[i].opcode, OpCodes.Ldstr) && Equals(codes[i].operand, "Prefabs/Characters/[Prefab]Appearance_Custom"))
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(XLRoot), nameof(XLRoot.CustomAppearancePrefab)));
                    i++;
                }
                else
                {
                    yield return codes[i];
                }
            }
        }
    }
}
