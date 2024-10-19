using HarmonyLib;
using System;
using System.Reflection.Emit;

namespace ExtendedLoader
{
	static class InternalExtensions
	{
		public static bool IsLdloc(this CodeInstruction instruction, int index)
		{
			switch (index)
			{
				case 0:
					if (instruction.opcode == OpCodes.Ldloc_0)
					{
						return true;
					}
					break;
				case 1:
					if (instruction.opcode == OpCodes.Ldloc_1)
					{
						return true;
					}
					break;
				case 2:
					if (instruction.opcode == OpCodes.Ldloc_2)
					{
						return true;
					}
					break;
				case 3:
					if (instruction.opcode == OpCodes.Ldloc_3)
					{
						return true;
					}
					break;
			}
			return (instruction.opcode == OpCodes.Ldloc || instruction.opcode == OpCodes.Ldloc_S) &&
				(instruction.operand is IConvertible i && i.ToInt32(null) == index || instruction.operand is LocalBuilder local && local.LocalIndex == index);
		}
	}
}
