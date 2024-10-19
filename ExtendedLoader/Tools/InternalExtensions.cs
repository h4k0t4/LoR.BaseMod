using HarmonyLib;
using System;
using System.IO;
using System.Reflection.Emit;
using UnityEngine;
using Workshop;

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
						if (File.Exists(thumbPath))
						{
							Texture2D texture2D = new Texture2D(2, 2);
							texture2D.LoadImage(File.ReadAllBytes(thumbPath));
							Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
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
