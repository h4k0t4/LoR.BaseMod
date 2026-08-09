using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static HarmonyLib.AccessTools;
using static System.Reflection.Emit.OpCodes;

namespace ExtendedLoader
{
	[HarmonyPatch]
	static class SoundOrderFix
	{
		[HarmonyPatch(typeof(SdCharacterUtil), nameof(SdCharacterUtil.CreateSkin))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> SdCharacterUtil_CreateSkin_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilgen)
		{
			var bookClassInfo = PropertyGetter(typeof(BookModel), nameof(BookModel.ClassInfo));
			var motionList = Field(typeof(BookXmlInfo), nameof(BookXmlInfo.motionSoundList));


			var codes = instructions.ToList();

			int startIndex = -1;
			int endIndex = -1;
			for (int i = 0; i < codes.Count; i++)
			{
				if (codes[i].IsLdarg(0) && codes[i + 2].Calls(bookClassInfo) && codes[i + 3].LoadsField(motionList))
				{
					for (int j = i + 4; i < codes.Count; j++)
					{
						if (codes[j].Branches(out var endLabel))
						{
							endIndex = codes.FindIndex(c => c.labels.Contains(endLabel.Value));
							startIndex = i;

							break;
						}
					}
					break;
				}
			}

			if (startIndex >= 0)
			{
				var flagInitIndex = codes.FindIndex(c => c.IsStloc(6));
				if (flagInitIndex >= 0 && flagInitIndex < startIndex - 1)
				{
					var soundInitRange = codes.GetRange(startIndex, endIndex - startIndex);
					codes.RemoveRange(startIndex + 1, endIndex - startIndex - 1);
					codes[startIndex] = new CodeInstruction(Nop).MoveLabelsFrom(soundInitRange[0]);

					var safeInitRangeEnd = new CodeInstruction(Nop);
					soundInitRange.Add(safeInitRangeEnd);

					foreach (var code in soundInitRange)
					{
						if (code.Branches(out var outLabel))
						{
							if (!soundInitRange.Exists(c => c.labels.Contains(outLabel.Value)))
							{
								var replaceSkip = ilgen.DefineLabel();
								code.operand = replaceSkip;
								safeInitRangeEnd.labels.Add(replaceSkip);
							}
						}
					}

					codes.InsertRange(flagInitIndex + 1, soundInitRange);
				}
			}

			return codes;
		}
	}
}
