using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using LorIdExtensions;

namespace ExtendedLoader
{
	[HarmonyPatch]
	internal class VanillaSkinCopyPatch
	{
		[HarmonyPatch(typeof(BattleUnitBuf_Ozma_PumpkinToLibrarian), nameof(BattleUnitBuf_Ozma_PumpkinToLibrarian.ChangeSpec))]
		[HarmonyPatch(typeof(PassiveAbility_1309021), nameof(PassiveAbility_1309021.ChangeSpec))]
		[HarmonyPatch(typeof(PassiveAbility_1309031), nameof(PassiveAbility_1309031.ChangeSpec))]
		[HarmonyPatch(typeof(PassiveAbility_1409022), nameof(PassiveAbility_1409022.CopyUnit))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> ChangeSpec_CopyUnit_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilgen)
		{
			bool nameFound = false;
			LocalBuilder loc = default;
			var bookProperty = AccessTools.PropertyGetter(typeof(BattleUnitModel), nameof(BattleUnitModel.Book));
			var nameMethod = AccessTools.Method(typeof(BookModel), nameof(BookModel.GetCharacterName));
			var codes = instructions.ToList();
			for (var i = 0; i < codes.Count; i++)
			{
				if (codes[i].Is(OpCodes.Callvirt, bookProperty) && i < codes.Count - 1 && codes[i + 1].Is(OpCodes.Callvirt, nameMethod))
				{
					i++;
					yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(SkinExtensions), nameof(SkinExtensions.GetOriginalSkin)));
					yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(LorName), nameof(LorName.Compress)));
					if (!nameFound)
					{
						nameFound = true;
						loc = ilgen.DeclareLocal(typeof(string));
						yield return new CodeInstruction(OpCodes.Dup);
						yield return new CodeInstruction(OpCodes.Stloc, loc);
					}
				}
				else if (nameFound && codes[i].Is(OpCodes.Callvirt, nameMethod))
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
	}
}
