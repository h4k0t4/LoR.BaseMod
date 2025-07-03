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

		public static bool IsStloc(this CodeInstruction instruction, int index)
		{
			switch (index)
			{
				case 0:
					if (instruction.opcode == OpCodes.Stloc_0)
					{
						return true;
					}
					break;
				case 1:
					if (instruction.opcode == OpCodes.Stloc_1)
					{
						return true;
					}
					break;
				case 2:
					if (instruction.opcode == OpCodes.Stloc_2)
					{
						return true;
					}
					break;
				case 3:
					if (instruction.opcode == OpCodes.Stloc_3)
					{
						return true;
					}
					break;
			}
			return (instruction.opcode == OpCodes.Stloc || instruction.opcode == OpCodes.Stloc_S) &&
				(instruction.operand is IConvertible i && i.ToInt32(null) == index || instruction.operand is LocalBuilder local && local.LocalIndex == index);
		}

		public static bool IsStloc(this CodeInstruction instruction, out int index)
		{
			if (instruction.opcode == OpCodes.Stloc_0)
			{
				index = 0;
				return true;
			}
			if (instruction.opcode == OpCodes.Stloc_1)
			{
				index = 1;
				return true;
			}
			if (instruction.opcode == OpCodes.Stloc_2)
			{
				index = 2;
				return true;
			}
			if (instruction.opcode == OpCodes.Stloc_3)
			{
				index = 3;
				return true;
			}

			if (instruction.opcode != OpCodes.Stloc && instruction.opcode != OpCodes.Stloc_S) 
			{
				index = -1;
				return false;
			}

			if (instruction.operand is IConvertible i)
			{
				index = i.ToInt32(null);
				return true;
			}
			if (instruction.operand is LocalBuilder local)
			{
				index = local.LocalIndex;
				return true;
			}

			index = -1;
			return false;
		}
	}
}