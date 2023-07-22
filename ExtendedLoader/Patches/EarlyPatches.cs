using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using System.Reflection.Emit;
using UnityEngine;
using System.Xml;
using Workshop;
using System.Globalization;
using System.IO;
using LOR_DiceSystem;
using System.Drawing;

namespace ExtendedLoader
{
	internal class EarlyPatches
	{
		[HarmonyPatch(typeof(XmlDocument), nameof(XmlDocument.LoadXml))]
		[HarmonyPostfix]
		static void XmlDocument_LoadXml_Postfix(XmlDocument __instance)
		{
			try
			{
				var clothNode = __instance.SelectSingleNode(".//ClothInfo");
				if (clothNode != null)
				{
					var headNodes = clothNode.SelectNodes(".//Head");
					foreach (XmlNode headNode in headNodes)
					{
						var attributes = headNode.Attributes;
						if (attributes.GetNamedItem("head_x") is XmlAttribute headX)
						{
							headX.InnerText = SanitizeDecimals(headX.InnerText);
						}
						if (attributes.GetNamedItem("head_y") is XmlAttribute headY)
						{
							headY.InnerText = SanitizeDecimals(headY.InnerText);
						}
						if (attributes.GetNamedItem("rotation") is XmlAttribute headR)
						{
							headR.InnerText = SanitizeDecimals(headR.InnerText);
						}
					}
					var pivotNodes = clothNode.SelectNodes(".//Pivot");
					foreach (XmlNode pivotNode in pivotNodes)
					{
						var attributes = pivotNode.Attributes;
						if (attributes.GetNamedItem("pivot_x") is XmlAttribute pivotX)
						{
							pivotX.InnerText = SanitizeDecimals(pivotX.InnerText);
						}
						if (attributes.GetNamedItem("pivot_y") is XmlAttribute pivotY)
						{
							pivotY.InnerText = SanitizeDecimals(pivotY.InnerText);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		static string SanitizeDecimals(string source)
		{
			return ParseFloatSafe(source).ToString();
		}

		public static float ParseFloatSafe(string s)
		{
			try
			{
				return float.Parse(s.Replace(',', '.').Replace('/', '.'), invariant);
			}
			catch
			{
				Debug.Log("ExtendedLoader: could not parse float (" + s + "), using 0 as fallback");
				return 0;
			}
		}
		readonly static NumberFormatInfo invariant = CultureInfo.InvariantCulture.NumberFormat;

		[HarmonyPatch(typeof(WorkshopAppearanceItemLoader), nameof(WorkshopAppearanceItemLoader.LoadCustomAppearanceInfo))]
		[HarmonyFinalizer]
		static Exception WorkshopAppearanceItemLoader_LoadCustomAppearanceInfo_Finalizer(string rootPath, Exception __exception)
		{
			if (__exception != null)
			{
				Debug.LogError("ExtendedLoader: error loading skin from " + rootPath);
				Debug.LogException(__exception);
			}
			return null;
		}

		[HarmonyPatch(typeof(Workshop.WorkshopAppearanceItemLoader), nameof(WorkshopAppearanceItemLoader.LoadCustomAppearanceInfo))]
		[HarmonyPrefix]
		static void WorkshopAppearanceItemLoader_LoadCustomAppearanceInfo_Prefix(ref bool isBookSkin)
		{
			isBookSkin = false;
		}

		static readonly Vector2 midPivot = new Vector2(0.5f, 0.5f);

		[HarmonyPatch(typeof(Workshop.WorkshopAppearanceItemLoader), nameof(WorkshopAppearanceItemLoader.LoadCustomAppearanceInfo))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> list = new List<CodeInstruction>(instructions);
			int num = 0;
			var spriteMethod = AccessTools.Method(typeof(SpriteUtil), nameof(SpriteUtil.LoadSprite512));
			for (int l = num; l < list.Count; l++)
			{
				if (list[l].Is(OpCodes.Call, spriteMethod))
				{
					list.Insert(l, new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(EarlyPatches), nameof(midPivot))));
					list[l + 1].operand = AccessTools.Method(typeof(SpriteUtil), nameof(SpriteUtil.LoadSprite));
					Debug.Log("ExtendedLoader: Full Size Face Loading transpiler Successful");
					num = l + 2;
					break;
				}
			}
			for (int i = num; i < list.Count; i++)
			{
				if (list[i].opcode == OpCodes.Ldloc_S
					&& list[i + 1].opcode == OpCodes.Brtrue
					&& list[i + 2].opcode == OpCodes.Ldstr
					&& list[i + 2].operand as string == "Workshop :: ")
				{
					for (int j = i + 3; j < list.Count; j++)
					{
						if (list[j].opcode == OpCodes.Brfalse)
						{
							list.RemoveRange(i, j - i - 1);
							Debug.Log("ExtendedLoader: Optimization transpiler Successful");
							num = i;
							break;
						}
					}
					break;
				}
			}
			sbyte b = (sbyte)(Enum.GetValues(typeof(ActionDetail)).Length - 1);
			for (int k = num; k < list.Count; k++)
			{
				if (list[k].opcode == OpCodes.Ldc_I4_S && list[k].LoadsConstant(11L))
				{
					list[k].operand = b;
					Debug.Log("ExtendedLoader: Entry Expansion transpiler Successful");
					break;
				}
			}
			return list;
		}

		[HarmonyPatch(typeof(Workshop.WorkshopAppearanceItemLoader), nameof(WorkshopAppearanceItemLoader.LoadCustomAppearanceInfo))]
		[HarmonyPostfix]
		static void LoadCustomAppearanceInfo(string rootPath, string xml, WorkshopAppearanceInfo __result)
		{
			if (__result == null)
			{
				return;
			}
			StreamReader streamReader = new StreamReader(xml);
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.LoadXml(streamReader.ReadToEnd());
				XmlNode rootNode = xmlDocument.SelectSingleNode("ModInfo");
				bool isExtended = false;
				XmlNode extendedXml = rootNode.Attributes.GetNamedItem("extended") ?? rootNode.Attributes.GetNamedItem("Extended");
				if (extendedXml != null)
				{
					bool.TryParse(extendedXml.InnerText, out isExtended);
				}
				if (!isExtended)
				{
					return;
				}
				XmlNode versionXml = rootNode.Attributes.GetNamedItem("version") ?? rootNode.Attributes.GetNamedItem("Version");
				FaceData faceData = null;
				XmlNode faceNode = rootNode.SelectSingleNode("FaceInfo");
				if (faceNode != null && __result.faceCustomInfo != null)
				{
					XmlNode extendedFaceNode = rootNode.SelectSingleNode("ExtendedFaceInfo");
					if (extendedFaceNode != null)
					{
						faceData = new FaceData(__result.faceCustomInfo);
						for (int i = 0; i < 6; i++)
						{
							CustomizeType key = (CustomizeType)i;
							string xpath = key.ToString();
							XmlNode customNameNode = extendedFaceNode.SelectSingleNode(xpath);
							if (customNameNode != null)
							{
								string name = customNameNode.InnerText;
								if (!string.IsNullOrWhiteSpace(name))
								{
									if (int.TryParse(name, out var index))
									{
										faceData.customIds[key] = index;
									}
									else
									{
										faceData.customNames[key] = name;
									}
								}
							}
						}
						for (int i = 0; i < 3; i++)
						{
							CustomizeColor key = (CustomizeColor)i;
							string xpath = key.ToString();
							XmlNode customColorNode = extendedFaceNode.SelectSingleNode(xpath);
							if (customColorNode != null)
							{
								if (ColorUtility.TryParseHtmlString(customColorNode.InnerText, out var color))
								{
									faceData.customColors[key] = color;
								}
							}
						}
						XLRoot.LoadFaceCustom(__result.faceCustomInfo, XLRoot.GetCustomLocationKeyByFolder(rootPath));
					}
				}
				XmlNode clothNode = rootNode.SelectSingleNode("ClothInfo");
				if (clothNode != null)
				{
					bool isDynamicRes = false;
					if (clothNode.Attributes.GetNamedItem("size_dynamic") is XmlNode dynamicNode && bool.TryParse(dynamicNode.InnerText, out var dynamic))
					{
						isDynamicRes = dynamic;
					}
					float defaultRes = 50f;
					if (clothNode.Attributes.GetNamedItem("quality") is XmlNode qualityNode)
					{
						float quality = ParseFloatSafe(qualityNode.InnerText);
						if (quality > 12f)
						{
							defaultRes = quality;
						}
					}
					if (__result.clothCustomInfo == null)
					{
						__result.clothCustomInfo = new Dictionary<ActionDetail, ClothCustomizeData>();
					}
					var skinDic = __result.clothCustomInfo;
					var skinData = new SkinData(skinDic);
					skinData.faceData = faceData;
					string folderPath = rootPath + "/ClothCustom/";
					if (Directory.Exists(folderPath))
					{
						HashSet<string> files = new HashSet<string>(new DirectoryInfo(folderPath).EnumerateFiles().Select(file => file.Name));
						for (int j = 0; j < 31; j++)
						{
							ActionDetail actionDetail = (ActionDetail)j;
							if (actionDetail == ActionDetail.Standing || actionDetail == ActionDetail.NONE)
							{
								continue;
							}
							string actionName = actionDetail.ToString();
							try
							{
								XmlNode actionNode = clothNode.SelectSingleNode(actionName);
								if (actionNode == null)
								{
									continue;
								}

								bool hasSpriteFile = false;
								string spritePath = actionName + "_mid.png";
								bool hasSkinSprite = false;
								string skinSpritePath = actionName + "_mid_skin.png";
								bool hasBackSprite = false;
								string backSpritePath = actionName + "_back.png";
								bool hasBackSkinSprite = false;
								string backSkinSpritePath = actionName + "_back_skin.png";
								string pathForSize = null;
								if (files.Contains(spritePath))
								{
									hasSpriteFile = true;
									pathForSize = spritePath;
									if (files.Contains(skinSpritePath))
									{
										hasSkinSprite = true;
									}
									if (!files.Contains(backSpritePath))
									{
										backSpritePath = actionName + ".png";
										backSkinSpritePath = actionName + "_skin.png";
									}
								}
								else
								{
									spritePath = actionName + ".png";
									skinSpritePath = actionName + "_skin.png";
									if (files.Contains(spritePath))
									{
										hasSpriteFile = true;
										var largeSpritePath = actionName + "_large.png";
										if (files.Contains(largeSpritePath))
										{
											spritePath = largeSpritePath;
										}
										pathForSize = spritePath;
										if (files.Contains(skinSpritePath))
										{
											hasSkinSprite = true;
										}
									}
								}
								if (files.Contains(backSpritePath))
								{
									hasBackSprite = true;
									if (files.Contains(backSkinSpritePath))
									{
										hasBackSkinSprite = true;
									}
									pathForSize = pathForSize ?? backSpritePath;
								}
								bool hasFrontSprite = false;
								bool hasFrontSpriteFile = false;
								string frontSpritePath = actionName + "_front.png";
								bool hasFrontSkinSprite = false;
								string frontSkinSpritePath = frontSkinSpritePath = actionName + "_front_skin.png";
								if (files.Contains(frontSpritePath))
								{
									hasFrontSprite = true;
									hasFrontSpriteFile = true;
									var largeSpritePath = actionName + "_front_large.png";
									if (files.Contains(largeSpritePath))
									{
										frontSpritePath = largeSpritePath;
									}
									pathForSize = pathForSize ?? frontSpritePath;
									if (files.Contains(frontSkinSpritePath))
									{
										hasFrontSkinSprite = true;
									}
								}
								bool hasEffectSprite = false;
								string effectSpritePath = actionName + "_effect.png";
								if (files.Contains(effectSpritePath))
								{
									hasEffectSprite = true;
									pathForSize = pathForSize ?? effectSpritePath;
								}

								isDynamicRes &= pathForSize != null;
								Vector2Int size = isDynamicRes ? GetImageNativeSize(folderPath + pathForSize) : new Vector2Int(512, 512);
								float res = defaultRes;
								XmlNode sizeX = actionNode.Attributes.GetNamedItem("size_x");
								if (sizeX != null)
								{
									size.x = int.Parse(sizeX.InnerText);
								}
								XmlNode sizeY = actionNode.Attributes.GetNamedItem("size_y");
								if (sizeY != null)
								{
									size.y = int.Parse(sizeY.InnerText);
								}
								XmlNode resXml = actionNode.Attributes.GetNamedItem("quality");
								if (resXml != null)
								{
									res = ParseFloatSafe(resXml.InnerText);
								}

								XmlNode spritePivotNode = actionNode.SelectSingleNode("Pivot");
								XmlNode spritePivotXXml = spritePivotNode.Attributes.GetNamedItem("pivot_x_custom");
								XmlNode spritePivotYXml = spritePivotNode.Attributes.GetNamedItem("pivot_y_custom");
								if (spritePivotXXml == null)
								{
									spritePivotXXml = spritePivotNode.Attributes.GetNamedItem("pivot_x");
								}
								if (spritePivotYXml == null)
								{
									spritePivotYXml = spritePivotNode.Attributes.GetNamedItem("pivot_y");
								}
								float spritePivotX = ParseFloatSafe(spritePivotXXml.InnerText);
								float spritePivotY = ParseFloatSafe(spritePivotYXml.InnerText);
								Vector2 pivotPos = new Vector2(spritePivotX / (2 * size.x) + 0.5f, spritePivotY / (2 * size.y) + 0.5f);

								XmlNode headNode = actionNode.SelectSingleNode("Head");
								bool headEnabled = true;
								FaceOverride faceOverride = FaceOverride.None;
								EffectPivot headPivot = GetEffectPivot(headNode);
								if (headPivot == null)
								{
									headPivot = new EffectPivot();
									headEnabled = false;
								}
								else
								{
									XmlNode headEnabledXml = headNode.Attributes.GetNamedItem("head_enable");
									if (headEnabledXml != null)
									{
										bool.TryParse(headEnabledXml.InnerText, out headEnabled);
									}
									XmlNode faceXml = headNode.Attributes.GetNamedItem("face");
									if (Enum.TryParse(faceXml?.InnerText, true, out FaceOverride faceOverride1))
									{
										faceOverride = faceOverride1;
									}
								}
								float headRotation = headPivot.localEulerAngles.z;
								Vector2 headPos = new Vector2(headPivot.localPosition.x, headPivot.localPosition.y);

								XmlNode directionNode = actionNode.SelectSingleNode("Direction");
								CharacterMotion.MotionDirection direction = CharacterMotion.MotionDirection.FrontView;
								if (directionNode.InnerText == "Side")
								{
									direction = CharacterMotion.MotionDirection.SideView;
								}

								List<EffectPivot> additionalPivotCoords = new List<EffectPivot>();
								XmlNodeList additionalPivotNodes = actionNode.SelectNodes("AdditionalPivot");
								if (additionalPivotNodes != null)
								{
									foreach (XmlNode additionalPivot in additionalPivotNodes)
									{
										additionalPivotCoords.Add(GetEffectPivot(additionalPivot));
									}
								}

								if (hasSpriteFile || hasFrontSprite || hasBackSprite || hasEffectSprite)
								{
									ClothCustomizeData value = new ExtendedClothCustomizeData
									{
										hasSpriteFile = hasSpriteFile,
										spritePath = folderPath + spritePath,
										hasFrontSprite = hasFrontSprite,
										hasFrontSpriteFile = hasFrontSpriteFile,
										frontSpritePath = folderPath + frontSpritePath,
										pivotPos = pivotPos,
										headPos = headPos,
										headRotation = headRotation,
										direction = direction,
										headEnabled = headEnabled,
										size = size,
										resolution = res,
										skinSpritePath = hasSkinSprite ? folderPath + skinSpritePath : "",
										frontSkinSpritePath = hasFrontSkinSprite ? folderPath + frontSkinSpritePath : "",
										backSpritePath = hasBackSprite ? folderPath + backSpritePath : "",
										backSkinSpritePath = hasBackSkinSprite ? folderPath + backSkinSpritePath : "",
										effectSpritePath = hasEffectSprite ? folderPath + effectSpritePath : "",
										additionalPivots = additionalPivotCoords,
										headPivot = headPivot,
										faceOverride = faceOverride
									};
									skinDic[actionDetail] = value;
								}
							}
							catch (Exception ex)
							{
								Debug.LogException(ex);
							}
						}
					}
					XmlNode soundNode = clothNode.SelectSingleNode("SoundList");
					if (soundNode != null)
					{
						List<CustomInvitation.BookSoundInfo> motionSoundList = skinData.motionSoundList;
						XmlNodeList motionSoundsXml = soundNode.SelectNodes("SoundInfo");
						if (motionSoundsXml != null)
						{
							foreach (XmlNode motionSound in motionSoundsXml)
							{
								XmlAttributeCollection attr = motionSound.Attributes;
								XmlNode motion = attr.GetNamedItem("Motion");
								XmlNode winExternal = attr.GetNamedItem("WinExternal");
								XmlNode winPath = attr.GetNamedItem("Win");
								XmlNode loseExternal = attr.GetNamedItem("LoseExternal");
								XmlNode losePath = attr.GetNamedItem("Lose");
								if (Enum.TryParse(motion.InnerText, true, out MotionDetail motionDetail))
								{
									motionSoundList.Add(new CustomInvitation.BookSoundInfo()
									{
										motion = motionDetail,
										isWinExternal = winExternal != null && bool.Parse(winExternal.InnerText),
										winSound = winPath?.InnerText ?? "",
										isLoseExternal = loseExternal != null && bool.Parse(loseExternal.InnerText),
										loseSound = losePath?.InnerText ?? ""
									});
								}
							}
						}
					}
					XmlNode effectNode = clothNode.SelectSingleNode("AtkEffectPivotInfo");
					if (effectNode != null)
					{
						Dictionary<string, EffectPivot> atkEffectPivotDic = skinData.atkEffectPivotDic;
						AddPivot(atkEffectPivotDic, effectNode, "atkEffectRoot");
						AddPivot(atkEffectPivotDic, effectNode, "atkEffectPivot_H");
						AddPivot(atkEffectPivotDic, effectNode, "atkEffectPivot_J");
						AddPivot(atkEffectPivotDic, effectNode, "atkEffectPivot_Z");
						AddPivot(atkEffectPivotDic, effectNode, "atkEffectPivot_G");
						AddPivot(atkEffectPivotDic, effectNode, "atkEffectPivot_E");
						AddPivot(atkEffectPivotDic, effectNode, "atkEffectPivot_S");
						AddPivot(atkEffectPivotDic, effectNode, "atkEffectPivot_F");
					}
					XmlNode specialNode = clothNode.SelectSingleNode("SpecialMotionPivotInfo");
					if (specialNode != null)
					{
						Dictionary<ActionDetail, EffectPivot> specialMotionPivotDic = skinData.specialMotionPivotDic;
						for (int j = 0; j < 31; j++)
						{
							ActionDetail actionDetail = (ActionDetail)j;
							string text = actionDetail.ToString();
							XmlNode effectPivotNode = specialNode.SelectSingleNode(text);
							if (effectPivotNode != null)
							{
								specialMotionPivotDic[actionDetail] = GetEffectPivot(effectPivotNode);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		static Vector2Int GetImageNativeSize(string path)
		{
			try
			{
				var img = Image.FromStream(File.OpenRead(path), false, false);
				return new Vector2Int(img.Width, img.Height);
			}
			catch
			{
				return new Vector2Int(512, 512);
			}
		}
		static void AddPivot(Dictionary<string, EffectPivot> atkEffectPivotDic, XmlNode effectNode, string pivotNode)
		{
			XmlNode effectPivotNode = effectNode.SelectSingleNode(pivotNode);
			if (effectPivotNode != null)
			{
				atkEffectPivotDic[pivotNode] = GetEffectPivot(effectPivotNode);
			}
		}
		static EffectPivot GetEffectPivot(XmlNode effectPivotNode)
		{
			if (effectPivotNode == null)
			{
				return null;
			}
			var pivot = new EffectPivot();
			var attr = effectPivotNode.Attributes;
			if ((attr.GetNamedItem("x") ?? attr.GetNamedItem("pivot_x") ?? attr.GetNamedItem("head_x_custom") ?? attr.GetNamedItem("head_x")) is XmlNode xnode)
			{
				pivot.localPosition.x = ParseFloatSafe(xnode.InnerText) / 100;
			}
			if ((attr.GetNamedItem("y") ?? attr.GetNamedItem("pivot_y") ?? attr.GetNamedItem("head_y_custom") ?? attr.GetNamedItem("head_y")) is XmlNode ynode)
			{
				pivot.localPosition.y = ParseFloatSafe(ynode.InnerText) / 100;
			}
			if (attr.GetNamedItem("rotation") is XmlNode rnode)
			{
				pivot.localEulerAngles.z = ParseFloatSafe(rnode.InnerText);
			}
			if ((attr.GetNamedItem("nested") ?? attr.GetNamedItem("Nested")) is XmlNode nnode)
			{
				bool.TryParse(nnode.InnerText, out pivot.isNested);
			}
			SetPivotCoords(pivot, effectPivotNode);
			return pivot;
		}
		static void SetPivotCoords(EffectPivot pivot, XmlNode pivotNode)
		{
			pivot.localPosition = SetVectorCoords(pivotNode.SelectSingleNode("localPosition"), pivot.localPosition);
			pivot.localScale = SetVectorCoords(pivotNode.SelectSingleNode("localScale"), pivot.localScale);
			pivot.localEulerAngles = SetVectorCoords(pivotNode.SelectSingleNode("localEulerAngles"), pivot.localEulerAngles);
		}
		static Vector3 SetVectorCoords(XmlNode coordsNode, Vector3 coords)
		{
			if (coordsNode != null)
			{
				if (coordsNode.Attributes.GetNamedItem("x") is XmlNode xnode)
				{
					coords.x = ParseFloatSafe(xnode.InnerText);
				}
				if (coordsNode.Attributes.GetNamedItem("y") is XmlNode ynode)
				{
					coords.y = ParseFloatSafe(ynode.InnerText);
				}
				if (coordsNode.Attributes.GetNamedItem("z") is XmlNode znode)
				{
					coords.z = ParseFloatSafe(znode.InnerText);
				}
			}
			return coords;
		}
	}
}
