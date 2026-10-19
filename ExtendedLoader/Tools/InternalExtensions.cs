using HarmonyLib;
using System;
using System.IO;
using System.Reflection.Emit;
using UnityEngine;
using Workshop;
using static System.Reflection.Emit.OpCodes;

namespace ExtendedLoader
{
	static class InternalExtensions
	{
		public static bool IsLdloc(this CodeInstruction instruction, int index)
		{
			switch (index)
			{
				case 0:
					if (instruction.opcode == Ldloc_0)
					{
						return true;
					}
					break;
				case 1:
					if (instruction.opcode == Ldloc_1)
					{
						return true;
					}
					break;
				case 2:
					if (instruction.opcode == Ldloc_2)
					{
						return true;
					}
					break;
				case 3:
					if (instruction.opcode == Ldloc_3)
					{
						return true;
					}
					break;
			}
			return (instruction.opcode == Ldloc || instruction.opcode == Ldloc_S) &&
				(instruction.operand is IConvertible i && i.ToInt32(null) == index || instruction.operand is LocalBuilder local && local.LocalIndex == index);
		}

		public static bool IsStloc(this CodeInstruction instruction, int index)
		{
			switch (index)
			{
				case 0:
					if (instruction.opcode == Stloc_0)
					{
						return true;
					}
					break;
				case 1:
					if (instruction.opcode == Stloc_1)
					{
						return true;
					}
					break;
				case 2:
					if (instruction.opcode == Stloc_2)
					{
						return true;
					}
					break;
				case 3:
					if (instruction.opcode == Stloc_3)
					{
						return true;
					}
					break;
			}
			return (instruction.opcode == Stloc || instruction.opcode == Stloc_S) &&
				(instruction.operand is IConvertible i && i.ToInt32(null) == index || instruction.operand is LocalBuilder local && local.LocalIndex == index);
		}

		public static Sprite GetThumbSprite(this WorkshopSkinData data)
		{
			int skinId = data.id;
			if (XLRoot.SkinThumb.TryGetValue(skinId, out var thumb))
			{
				return thumb;
			}
			ClothCustomizeData defaultData = data.dic.GetValueSafe(ActionDetail.Default);
			if (defaultData != null)
			{
				try
				{
					if (defaultData.sprite != null && File.Exists(defaultData.spritePath))
					{
						DirectoryInfo spriteDir = new DirectoryInfo(defaultData.spritePath);
						string thumbPath = Path.Combine(spriteDir.Parent.Parent.FullName, "Thumb.png");
						var sprite = SpriteUtilExtension.LoadSpriteCompressed(thumbPath, new Vector2(0.5f, 0.5f));
						if (sprite)
						{
							XLRoot.SkinThumb[skinId] = sprite;
							return sprite;
						}
						XLRoot.MakeThumbnail(data.dic[ActionDetail.Default]);
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}
			return null;
		}
	}
}