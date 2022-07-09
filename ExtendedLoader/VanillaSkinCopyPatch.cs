using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace ExtendedLoader
{
    public class VanillaSkinCopyPatch
    {
        [HarmonyPatch(typeof(BattleUnitBuf_Ozma_PumpkinToLibrarian), nameof(BattleUnitBuf_Ozma_PumpkinToLibrarian.ChangeSpec))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> OzmaPumpkinToLibrarianTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                if (i < codes.Count - 1 &&
                    Equals(codes[i].opcode, OpCodes.Callvirt) &&
                    Equals(codes[i].operand, AccessTools.PropertyGetter(typeof(BattleUnitModel), nameof(BattleUnitModel.Book))) &&
                    Equals(codes[i + 1].opcode, OpCodes.Callvirt) &&
                    Equals(codes[i + 1].operand, AccessTools.Method(typeof(BookModel), nameof(BookModel.GetCharacterName))))
                {
                    i++;
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(SkinChangeExtensions), nameof(SkinChangeExtensions.GetOriginalSkin)));
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(LorName), nameof(LorName.Compress)));
                }
                else
                {
                    yield return codes[i];
                }
            }
        }

        [HarmonyPatch(typeof(PassiveAbility_1309021), nameof(PassiveAbility_1309021.ChangeSpec))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> ShadePhase1Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilgen)
        {
            bool nameFound = false;
            LocalBuilder loc = default;
            var codes = instructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                if (i < codes.Count - 1 &&
                    Equals(codes[i].opcode, OpCodes.Callvirt) &&
                    Equals(codes[i].operand, AccessTools.PropertyGetter(typeof(BattleUnitModel), nameof(BattleUnitModel.Book))) &&
                    Equals(codes[i + 1].opcode, OpCodes.Callvirt) &&
                    Equals(codes[i + 1].operand, AccessTools.Method(typeof(BookModel), nameof(BookModel.GetCharacterName))))
                {
                    i++;
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(SkinChangeExtensions), nameof(SkinChangeExtensions.GetOriginalSkin)));
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(LorName), nameof(LorName.Compress)));
                    if (!nameFound)
                    {
                        nameFound = true;
                        loc = ilgen.DeclareLocal(typeof(string));
                        yield return new CodeInstruction(OpCodes.Dup);
                        yield return new CodeInstruction(OpCodes.Stloc, loc);
                    }
                }
                else if (nameFound &&
                    Equals(codes[i].opcode, OpCodes.Callvirt) &&
                    Equals(codes[i].operand, AccessTools.Method(typeof(BookModel), nameof(BookModel.GetCharacterName))))
                {
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Ldloc, loc);
                }
                else
                {
                    yield return codes[i];
                }
            }
        }

        [HarmonyPatch(typeof(PassiveAbility_1309031), nameof(PassiveAbility_1309031.ChangeSpec))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> ShadePhase2Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilgen)
        {
            bool nameFound = false;
            LocalBuilder loc = default;
            var codes = instructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                if (i < codes.Count - 1 &&
                    Equals(codes[i].opcode, OpCodes.Callvirt) &&
                    Equals(codes[i].operand, AccessTools.PropertyGetter(typeof(BattleUnitModel), nameof(BattleUnitModel.Book))) &&
                    Equals(codes[i + 1].opcode, OpCodes.Callvirt) &&
                    Equals(codes[i + 1].operand, AccessTools.Method(typeof(BookModel), nameof(BookModel.GetCharacterName))))
                {
                    i++;
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(SkinChangeExtensions), nameof(SkinChangeExtensions.GetOriginalSkin)));
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(LorName), nameof(LorName.Compress)));
                    if (!nameFound)
                    {
                        nameFound = true;
                        loc = ilgen.DeclareLocal(typeof(string));
                        yield return new CodeInstruction(OpCodes.Dup);
                        yield return new CodeInstruction(OpCodes.Stloc, loc);
                    }
                }
                else if (nameFound &&
                    Equals(codes[i].opcode, OpCodes.Callvirt) &&
                    Equals(codes[i].operand, AccessTools.Method(typeof(BookModel), nameof(BookModel.GetCharacterName))))
                {
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Ldloc, loc);
                }
                else
                {
                    yield return codes[i];
                }
            }
        }

        [HarmonyPatch(typeof(PassiveAbility_1409022), nameof(PassiveAbility_1409022.CopyUnit))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> ShadeRematchTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (var i = 0; i < codes.Count; i++)
            {
                if (i < codes.Count - 1 &&
                    Equals(codes[i].opcode, OpCodes.Callvirt) &&
                    Equals(codes[i].operand, AccessTools.PropertyGetter(typeof(BattleUnitModel), nameof(BattleUnitModel.Book))) &&
                    Equals(codes[i + 1].opcode, OpCodes.Callvirt) &&
                    Equals(codes[i + 1].operand, AccessTools.Method(typeof(BookModel), nameof(BookModel.GetCharacterName))))
                {
                    i++;
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(SkinChangeExtensions), nameof(SkinChangeExtensions.GetOriginalSkin)));
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(LorName), nameof(LorName.Compress)));
                }
                else
                {
                    yield return codes[i];
                }
            }
        }
    }
}
