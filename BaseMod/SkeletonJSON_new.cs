using HarmonyLib;
using Spine;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BaseMod
{
	public class SkeletonJSON_new
	{
		public static void ReadAnimation_new(SkeletonJson __instance, Dictionary<string, object> map, string name, SkeletonData skeletonData)
		{
			float num = 1f;
			ExposedList<Timeline> exposedList = new ExposedList<Timeline>();
			float num2 = 0f;
			bool flag = map.ContainsKey("slots");
			if (flag)
			{
				foreach (KeyValuePair<string, object> keyValuePair in ((Dictionary<string, object>)map["slots"]))
				{
					string key = keyValuePair.Key;
					int slotIndex = skeletonData.FindSlotIndex(key);
					foreach (KeyValuePair<string, object> keyValuePair2 in ((Dictionary<string, object>)keyValuePair.Value))
					{
						List<object> list = (List<object>)keyValuePair2.Value;
						string key2 = keyValuePair2.Key;
						if (key2 == "attachment")
						{
							AttachmentTimeline attachmentTimeline = new AttachmentTimeline(list.Count)
							{
								SlotIndex = slotIndex
							};
							int num3 = 0;
							foreach (object obj in list)
							{
								Dictionary<string, object> dictionary = (Dictionary<string, object>)obj;
								float time = (float)dictionary["time"];
								attachmentTimeline.SetFrame(num3++, time, (string)dictionary["name"]);
							}
							exposedList.Add(attachmentTimeline);
							num2 = Math.Max(num2, attachmentTimeline.Frames[attachmentTimeline.FrameCount - 1]);
						}
						else
						{
							if (key2 == "color")
							{
								ColorTimeline colorTimeline = new ColorTimeline(list.Count)
								{
									SlotIndex = slotIndex
								};
								int num4 = 0;
								foreach (object obj2 in list)
								{
									Dictionary<string, object> dictionary2 = (Dictionary<string, object>)obj2;
									float time2 = (float)dictionary2["time"];
									string hexString = (string)dictionary2["color"];
									colorTimeline.SetFrame(num4, time2, ToColor(hexString, 0, 8), ToColor(hexString, 1, 8), ToColor(hexString, 2, 8), ToColor(hexString, 3, 8));
									ReadCurve(dictionary2, colorTimeline, num4);
									num4++;
								}
								exposedList.Add(colorTimeline);
								num2 = Math.Max(num2, colorTimeline.Frames[(colorTimeline.FrameCount - 1) * 5]);
							}
							else
							{
								if (!(key2 == "twoColor"))
								{
									throw new Exception(string.Concat(new string[]
									{
										"Invalid timeline type for a slot: ",
										key2,
										" (",
										key,
										")"
									}));
								}
								TwoColorTimeline twoColorTimeline = new TwoColorTimeline(list.Count)
								{
									SlotIndex = slotIndex
								};
								int num5 = 0;
								foreach (object obj3 in list)
								{
									Dictionary<string, object> dictionary3 = (Dictionary<string, object>)obj3;
									float time3 = (float)dictionary3["time"];
									string hexString2 = (string)dictionary3["light"];
									string hexString3 = (string)dictionary3["dark"];
									twoColorTimeline.SetFrame(num5, time3, ToColor(hexString2, 0, 8), ToColor(hexString2, 1, 8), ToColor(hexString2, 2, 8), ToColor(hexString2, 3, 8), ToColor(hexString3, 0, 6), ToColor(hexString3, 1, 6), ToColor(hexString3, 2, 6));
									ReadCurve(dictionary3, twoColorTimeline, num5);
									num5++;
								}
								exposedList.Add(twoColorTimeline);
								num2 = Math.Max(num2, twoColorTimeline.Frames[(twoColorTimeline.FrameCount - 1) * 8]);
							}
						}
					}
				}
			}
			if (map.ContainsKey("bones"))
			{
				foreach (KeyValuePair<string, object> keyValuePair3 in ((Dictionary<string, object>)map["bones"]))
				{
					string key3 = keyValuePair3.Key;
					int num6 = skeletonData.FindBoneIndex(key3);
					if (num6 == -1)
					{
						throw new Exception("Bone not found: " + key3);
					}
					foreach (KeyValuePair<string, object> keyValuePair4 in ((Dictionary<string, object>)keyValuePair3.Value))
					{
						List<object> list2 = (List<object>)keyValuePair4.Value;
						string key4 = keyValuePair4.Key;
						if (key4 == "rotate")
						{
							RotateTimeline rotateTimeline = new RotateTimeline(list2.Count)
							{
								BoneIndex = num6
							};
							int num7 = 0;
							foreach (object obj4 in list2)
							{
								Dictionary<string, object> dictionary4 = (Dictionary<string, object>)obj4;
								rotateTimeline.SetFrame(num7, (float)dictionary4["time"], (float)dictionary4["angle"]);
								ReadCurve(dictionary4, rotateTimeline, num7);
								num7++;
							}
							exposedList.Add(rotateTimeline);
							num2 = Math.Max(num2, rotateTimeline.Frames[(rotateTimeline.FrameCount - 1) * 2]);
						}
						else
						{
							if (!(key4 == "translate") && !(key4 == "scale") && !(key4 == "shear"))
							{
								throw new Exception(string.Concat(new string[]
								{
									"Invalid timeline type for a bone: ",
									key4,
									" (",
									key3,
									")"
								}));
							}
							float num8 = 1f;
							bool flag9 = key4 == "scale";
							TranslateTimeline translateTimeline;
							if (flag9)
							{
								translateTimeline = new ScaleTimeline(list2.Count);
							}
							else
							{
								if (key4 == "shear")
								{
									translateTimeline = new ShearTimeline(list2.Count);
								}
								else
								{
									translateTimeline = new TranslateTimeline(list2.Count);
									num8 = num;
								}
							}
							translateTimeline.BoneIndex = num6;
							int num9 = 0;
							foreach (object obj5 in list2)
							{
								Dictionary<string, object> dictionary5 = (Dictionary<string, object>)obj5;
								float time4 = (float)dictionary5["time"];
								float @float = GetFloat(dictionary5, "x", 0f);
								float float2 = GetFloat(dictionary5, "y", 0f);
								translateTimeline.SetFrame(num9, time4, @float * num8, float2 * num8);
								ReadCurve(dictionary5, translateTimeline, num9);
								num9++;
							}
							exposedList.Add(translateTimeline);
							num2 = Math.Max(num2, translateTimeline.Frames[(translateTimeline.FrameCount - 1) * 3]);
						}
					}
				}
			}
			if (map.ContainsKey("ik"))
			{
				foreach (KeyValuePair<string, object> keyValuePair5 in ((Dictionary<string, object>)map["ik"]))
				{
					IkConstraintData item = skeletonData.FindIkConstraint(keyValuePair5.Key);
					List<object> list3 = (List<object>)keyValuePair5.Value;
					IkConstraintTimeline ikConstraintTimeline = new IkConstraintTimeline(list3.Count)
					{
						IkConstraintIndex = skeletonData.IkConstraints.IndexOf(item)
					};
					int num10 = 0;
					foreach (object obj6 in list3)
					{
						Dictionary<string, object> dictionary6 = (Dictionary<string, object>)obj6;
						float time5 = (float)dictionary6["time"];
						float float3 = GetFloat(dictionary6, "mix", 1f);
						bool boolean = GetBoolean(dictionary6, "bendPositive", true);
						ikConstraintTimeline.SetFrame(num10, time5, float3, 0f, (!boolean) ? -1 : 1, false, false);
						ReadCurve(dictionary6, ikConstraintTimeline, num10);
						num10++;
					}
					exposedList.Add(ikConstraintTimeline);
					num2 = Math.Max(num2, ikConstraintTimeline.Frames[(ikConstraintTimeline.FrameCount - 1) * 3]);
				}
			}
			if (map.ContainsKey("transform"))
			{
				foreach (KeyValuePair<string, object> keyValuePair6 in ((Dictionary<string, object>)map["transform"]))
				{
					TransformConstraintData item2 = skeletonData.FindTransformConstraint(keyValuePair6.Key);
					List<object> list4 = (List<object>)keyValuePair6.Value;
					TransformConstraintTimeline transformConstraintTimeline = new TransformConstraintTimeline(list4.Count)
					{
						TransformConstraintIndex = skeletonData.TransformConstraints.IndexOf(item2)
					};
					int num11 = 0;
					foreach (object obj7 in list4)
					{
						Dictionary<string, object> dictionary7 = (Dictionary<string, object>)obj7;
						float time6 = (float)dictionary7["time"];
						float float4 = GetFloat(dictionary7, "rotateMix", 1f);
						float float5 = GetFloat(dictionary7, "translateMix", 1f);
						float float6 = GetFloat(dictionary7, "scaleMix", 1f);
						float float7 = GetFloat(dictionary7, "shearMix", 1f);
						transformConstraintTimeline.SetFrame(num11, time6, float4, float5, float6, float7);
						ReadCurve(dictionary7, transformConstraintTimeline, num11);
						num11++;
					}
					exposedList.Add(transformConstraintTimeline);
					num2 = Math.Max(num2, transformConstraintTimeline.Frames[(transformConstraintTimeline.FrameCount - 1) * 5]);
				}
			}
			if (map.ContainsKey("paths"))
			{
				foreach (KeyValuePair<string, object> keyValuePair7 in ((Dictionary<string, object>)map["paths"]))
				{
					int num12 = skeletonData.FindPathConstraintIndex(keyValuePair7.Key);
					if (num12 == -1)
					{
						throw new Exception("Path constraint not found: " + keyValuePair7.Key);
					}
					PathConstraintData pathConstraintData = skeletonData.PathConstraints.Items[num12];
					foreach (KeyValuePair<string, object> keyValuePair8 in ((Dictionary<string, object>)keyValuePair7.Value))
					{
						List<object> list5 = (List<object>)keyValuePair8.Value;
						string key5 = keyValuePair8.Key;
						if (key5 == "position" || key5 == "spacing")
						{
							float num13 = 1f;
							PathConstraintPositionTimeline pathConstraintPositionTimeline;
							if (key5 == "spacing")
							{
								pathConstraintPositionTimeline = new PathConstraintSpacingTimeline(list5.Count);
								if (pathConstraintData.SpacingMode == SpacingMode.Length || pathConstraintData.SpacingMode == SpacingMode.Fixed)
								{
									num13 = num;
								}
							}
							else
							{
								pathConstraintPositionTimeline = new PathConstraintPositionTimeline(list5.Count);
								if (pathConstraintData.PositionMode == PositionMode.Fixed)
								{
									num13 = num;
								}
							}
							pathConstraintPositionTimeline.PathConstraintIndex = num12;
							int num14 = 0;
							foreach (object obj8 in list5)
							{
								Dictionary<string, object> dictionary8 = (Dictionary<string, object>)obj8;
								pathConstraintPositionTimeline.SetFrame(num14, (float)dictionary8["time"], GetFloat(dictionary8, key5, 0f) * num13);
								ReadCurve(dictionary8, pathConstraintPositionTimeline, num14);
								num14++;
							}
							exposedList.Add(pathConstraintPositionTimeline);
							num2 = Math.Max(num2, pathConstraintPositionTimeline.Frames[(pathConstraintPositionTimeline.FrameCount - 1) * 2]);
						}
						else
						{
							if (key5 == "mix")
							{
								PathConstraintMixTimeline pathConstraintMixTimeline = new PathConstraintMixTimeline(list5.Count)
								{
									PathConstraintIndex = num12
								};
								int num15 = 0;
								foreach (object obj9 in list5)
								{
									Dictionary<string, object> dictionary9 = (Dictionary<string, object>)obj9;
									pathConstraintMixTimeline.SetFrame(num15, (float)dictionary9["time"], GetFloat(dictionary9, "rotateMix", 1f), GetFloat(dictionary9, "translateMix", 1f));
									ReadCurve(dictionary9, pathConstraintMixTimeline, num15);
									num15++;
								}
								exposedList.Add(pathConstraintMixTimeline);
								num2 = Math.Max(num2, pathConstraintMixTimeline.Frames[(pathConstraintMixTimeline.FrameCount - 1) * 3]);
							}
						}
					}
				}
			}
			if (map.ContainsKey("deform"))
			{
				foreach (KeyValuePair<string, object> keyValuePair9 in ((Dictionary<string, object>)map["deform"]))
				{
					Skin skin = skeletonData.FindSkin(keyValuePair9.Key);
					foreach (KeyValuePair<string, object> keyValuePair10 in ((Dictionary<string, object>)keyValuePair9.Value))
					{
						int num16 = skeletonData.FindSlotIndex(keyValuePair10.Key);
						if (num16 == -1)
						{
							throw new Exception("Slot not found: " + keyValuePair10.Key);
						}
						foreach (KeyValuePair<string, object> keyValuePair11 in ((Dictionary<string, object>)keyValuePair10.Value))
						{
							List<object> list6 = (List<object>)keyValuePair11.Value;
							VertexAttachment vertexAttachment = (VertexAttachment)skin.GetAttachment(num16, keyValuePair11.Key);
							if (vertexAttachment == null)
							{
								throw new Exception("Deform attachment not found: " + keyValuePair11.Key);
							}
							bool flag23 = vertexAttachment.Bones != null;
							float[] vertices = vertexAttachment.Vertices;
							int num17 = (!flag23) ? vertices.Length : (vertices.Length / 3 * 2);
							DeformTimeline deformTimeline = new DeformTimeline(list6.Count)
							{
								SlotIndex = num16,
								Attachment = vertexAttachment
							};
							int num18 = 0;
							foreach (object obj10 in list6)
							{
								Dictionary<string, object> dictionary10 = (Dictionary<string, object>)obj10;
								bool flag24 = !dictionary10.ContainsKey("vertices");
								float[] array;
								if (flag24)
								{
									array = ((!flag23) ? vertices : new float[num17]);
								}
								else
								{
									array = new float[num17];
									int @int = GetInt(dictionary10, "offset", 0);
									float[] floatArray = GetFloatArray(dictionary10, "vertices", 1f);
									Array.Copy(floatArray, 0, array, @int, floatArray.Length);
									if (num != 1f)
									{
										int i = @int;
										int num19 = i + floatArray.Length;
										while (i < num19)
										{
											array[i] *= num;
											i++;
										}
									}
									if (!flag23)
									{
										for (int j = 0; j < num17; j++)
										{
											array[j] += vertices[j];
										}
									}
								}
								deformTimeline.SetFrame(num18, (float)dictionary10["time"], array);
								ReadCurve(dictionary10, deformTimeline, num18);
								num18++;
							}
							exposedList.Add(deformTimeline);
							num2 = Math.Max(num2, deformTimeline.Frames[deformTimeline.FrameCount - 1]);
						}
					}
				}
			}
			if (map.ContainsKey("drawOrder") || map.ContainsKey("draworder"))
			{
				List<object> list7 = (List<object>)map[(!map.ContainsKey("drawOrder")) ? "draworder" : "drawOrder"];
				DrawOrderTimeline drawOrderTimeline = new DrawOrderTimeline(list7.Count);
				int count = skeletonData.Slots.Count;
				int num20 = 0;
				foreach (object obj11 in list7)
				{
					Dictionary<string, object> dictionary11 = (Dictionary<string, object>)obj11;
					int[] array2 = null;
					if (dictionary11.ContainsKey("offsets"))
					{
						array2 = new int[count];
						for (int k = count - 1; k >= 0; k--)
						{
							array2[k] = -1;
						}
						List<object> list8 = (List<object>)dictionary11["offsets"];
						int[] array3 = new int[count - list8.Count];
						int l = 0;
						int num21 = 0;
						foreach (object obj12 in list8)
						{
							Dictionary<string, object> dictionary12 = (Dictionary<string, object>)obj12;
							int num22 = skeletonData.FindSlotIndex((string)dictionary12["slot"]);
							if (num22 == -1)
							{
								string str = "Slot not found: ";
								object obj13 = dictionary12["slot"];
								throw new Exception(str + (obj13?.ToString()));
							}
							while (l != num22)
							{
								array3[num21++] = l++;
							}
							int num23 = l + (int)((float)dictionary12["offset"]);
							array2[num23] = l++;
						}
						while (l < count)
						{
							array3[num21++] = l++;
						}
						for (int m = count - 1; m >= 0; m--)
						{
							if (array2[m] == -1)
							{
								array2[m] = array3[--num21];
							}
						}
					}
					drawOrderTimeline.SetFrame(num20++, (float)dictionary11["time"], array2);
				}
				exposedList.Add(drawOrderTimeline);
				num2 = Math.Max(num2, drawOrderTimeline.Frames[drawOrderTimeline.FrameCount - 1]);
			}
			if (map.ContainsKey("events"))
			{
				List<object> list9 = (List<object>)map["events"];
				EventTimeline eventTimeline = new EventTimeline(list9.Count);
				int num24 = 0;
				foreach (object obj14 in list9)
				{
					Dictionary<string, object> dictionary13 = (Dictionary<string, object>)obj14;
					EventData eventData = skeletonData.FindEvent((string)dictionary13["name"]);
					bool flag32 = eventData == null;
					if (flag32)
					{
						string str2 = "Event not found: ";
						object obj15 = dictionary13["name"];
						throw new Exception(str2 + (obj15?.ToString()));
					}
					Event @event = new Event((float)dictionary13["time"], eventData)
					{
						Int = GetInt(dictionary13, "int", eventData.Int),
						Float = GetFloat(dictionary13, "float", eventData.Float),
						String = GetString(dictionary13, "string", eventData.String)
					};
					eventTimeline.SetFrame(num24++, @event);
				}
				exposedList.Add(eventTimeline);
				num2 = Math.Max(num2, eventTimeline.Frames[eventTimeline.FrameCount - 1]);
			}
			exposedList.TrimExcess();
			skeletonData.Animations.Add(new Spine.Animation(name, exposedList, num2));
		}
		public static void ReadAnimation(SkeletonJson __instance, Dictionary<string, object> map, string name, SkeletonData skeletonData)
		{
			float scale = __instance.Scale;
			ExposedList<Timeline> exposedList = new ExposedList<Timeline>();
			float num = 0f;
			if (map.ContainsKey("slots"))
			{
				foreach (KeyValuePair<string, object> keyValuePair in ((Dictionary<string, object>)map["slots"]))
				{
					string key = keyValuePair.Key;
					int slotIndex = skeletonData.FindSlotIndex(key);
					foreach (KeyValuePair<string, object> keyValuePair2 in ((Dictionary<string, object>)keyValuePair.Value))
					{
						List<object> list = (List<object>)keyValuePair2.Value;
						string key2 = keyValuePair2.Key;
						if (key2 == "attachment")
						{
							AttachmentTimeline attachmentTimeline = new AttachmentTimeline(list.Count)
							{
								SlotIndex = slotIndex
							};
							int num2 = 0;
							foreach (object obj in list)
							{
								Dictionary<string, object> dictionary = (Dictionary<string, object>)obj;
								float @float = GetFloat(dictionary, "time", 0f);
								attachmentTimeline.SetFrame(num2++, @float, (string)dictionary["name"]);
							}
							exposedList.Add(attachmentTimeline);
							num = Math.Max(num, attachmentTimeline.Frames[attachmentTimeline.FrameCount - 1]);
						}
						else
						{
							if (key2 == "color")
							{
								ColorTimeline colorTimeline = new ColorTimeline(list.Count)
								{
									SlotIndex = slotIndex
								};
								int num3 = 0;
								foreach (object obj2 in list)
								{
									Dictionary<string, object> dictionary2 = (Dictionary<string, object>)obj2;
									float float2 = GetFloat(dictionary2, "time", 0f);
									string hexString = (string)dictionary2["color"];
									colorTimeline.SetFrame(num3, float2, ToColor(hexString, 0, 8), ToColor(hexString, 1, 8), ToColor(hexString, 2, 8), ToColor(hexString, 3, 8));
									ReadCurve(dictionary2, colorTimeline, num3);
									num3++;
								}
								exposedList.Add(colorTimeline);
								num = Math.Max(num, colorTimeline.Frames[(colorTimeline.FrameCount - 1) * 5]);
							}
							else
							{
								if (!(key2 == "twoColor"))
								{
									throw new Exception(string.Concat(new string[]
									{
										"Invalid timeline type for a slot: ",
										key2,
										" (",
										key,
										")"
									}));
								}
								TwoColorTimeline twoColorTimeline = new TwoColorTimeline(list.Count)
								{
									SlotIndex = slotIndex
								};
								int num4 = 0;
								foreach (object obj3 in list)
								{
									Dictionary<string, object> dictionary3 = (Dictionary<string, object>)obj3;
									float float3 = GetFloat(dictionary3, "time", 0f);
									string hexString2 = (string)dictionary3["light"];
									string hexString3 = (string)dictionary3["dark"];
									twoColorTimeline.SetFrame(num4, float3, ToColor(hexString2, 0, 8), ToColor(hexString2, 1, 8), ToColor(hexString2, 2, 8), ToColor(hexString2, 3, 8), ToColor(hexString3, 0, 6), ToColor(hexString3, 1, 6), ToColor(hexString3, 2, 6));
									ReadCurve(dictionary3, twoColorTimeline, num4);
									num4++;
								}
								exposedList.Add(twoColorTimeline);
								num = Math.Max(num, twoColorTimeline.Frames[(twoColorTimeline.FrameCount - 1) * 8]);
							}
						}
					}
				}
			}
			if (map.ContainsKey("bones"))
			{
				foreach (KeyValuePair<string, object> keyValuePair3 in ((Dictionary<string, object>)map["bones"]))
				{
					string key3 = keyValuePair3.Key;
					int num5 = skeletonData.FindBoneIndex(key3);
					bool flag6 = num5 == -1;
					if (num5 == -1)
					{
						throw new Exception("Bone not found: " + key3);
					}
					foreach (KeyValuePair<string, object> keyValuePair4 in ((Dictionary<string, object>)keyValuePair3.Value))
					{
						List<object> list2 = (List<object>)keyValuePair4.Value;
						string key4 = keyValuePair4.Key;
						bool flag7 = key4 == "rotate";
						if (key4 == "rotate")
						{
							RotateTimeline rotateTimeline = new RotateTimeline(list2.Count)
							{
								BoneIndex = num5
							};
							int num6 = 0;
							foreach (object obj4 in list2)
							{
								Dictionary<string, object> dictionary4 = (Dictionary<string, object>)obj4;
								rotateTimeline.SetFrame(num6, GetFloat(dictionary4, "time", 0f), GetFloat(dictionary4, "angle", 0f));
								ReadCurve(dictionary4, rotateTimeline, num6);
								num6++;
							}
							exposedList.Add(rotateTimeline);
							num = Math.Max(num, rotateTimeline.Frames[(rotateTimeline.FrameCount - 1) * 2]);
						}
						else
						{
							bool flag8 = !(key4 == "translate") && !(key4 == "scale") && !(key4 == "shear");
							if (!(key4 == "translate") && !(key4 == "scale") && !(key4 == "shear"))
							{
								throw new Exception(string.Concat(new string[]
								{
									"Invalid timeline type for a bone: ",
									key4,
									" (",
									key3,
									")"
								}));
							}
							float num7 = 1f;
							float defaultValue = 0f;
							bool flag9 = key4 == "scale";
							TranslateTimeline translateTimeline;
							if (key4 == "scale")
							{
								translateTimeline = new ScaleTimeline(list2.Count);
								defaultValue = 1f;
							}
							else
							{
								bool flag10 = key4 == "shear";
								if (key4 == "shear")
								{
									translateTimeline = new ShearTimeline(list2.Count);
								}
								else
								{
									translateTimeline = new TranslateTimeline(list2.Count);
									num7 = scale;
								}
							}
							translateTimeline.BoneIndex = num5;
							int num8 = 0;
							foreach (object obj5 in list2)
							{
								Dictionary<string, object> dictionary5 = (Dictionary<string, object>)obj5;
								float float4 = GetFloat(dictionary5, "time", 0f);
								float float5 = GetFloat(dictionary5, "x", defaultValue);
								float float6 = GetFloat(dictionary5, "y", defaultValue);
								translateTimeline.SetFrame(num8, float4, float5 * num7, float6 * num7);
								ReadCurve(dictionary5, translateTimeline, num8);
								num8++;
							}
							exposedList.Add(translateTimeline);
							num = Math.Max(num, translateTimeline.Frames[(translateTimeline.FrameCount - 1) * 3]);
						}
					}
				}
			}
			if (map.ContainsKey("ik"))
			{
				foreach (KeyValuePair<string, object> keyValuePair5 in ((Dictionary<string, object>)map["ik"]))
				{
					IkConstraintData item = skeletonData.FindIkConstraint(keyValuePair5.Key);
					List<object> list3 = (List<object>)keyValuePair5.Value;
					IkConstraintTimeline ikConstraintTimeline = new IkConstraintTimeline(list3.Count)
					{
						IkConstraintIndex = skeletonData.IkConstraints.IndexOf(item)
					};
					int num9 = 0;
					foreach (object obj6 in list3)
					{
						Dictionary<string, object> dictionary6 = (Dictionary<string, object>)obj6;
						ikConstraintTimeline.SetFrame(num9, GetFloat(dictionary6, "time", 0f), GetFloat(dictionary6, "mix", 1f), GetFloat(dictionary6, "softness", 0f) * scale, GetBoolean(dictionary6, "bendPositive", true) ? 1 : -1, GetBoolean(dictionary6, "compress", false), GetBoolean(dictionary6, "stretch", false));
						ReadCurve(dictionary6, ikConstraintTimeline, num9);
						num9++;
					}
					exposedList.Add(ikConstraintTimeline);
					num = Math.Max(num, ikConstraintTimeline.Frames[(ikConstraintTimeline.FrameCount - 1) * 6]);
				}
			}
			if (map.ContainsKey("transform"))
			{
				foreach (KeyValuePair<string, object> keyValuePair6 in ((Dictionary<string, object>)map["transform"]))
				{
					TransformConstraintData item2 = skeletonData.FindTransformConstraint(keyValuePair6.Key);
					List<object> list4 = (List<object>)keyValuePair6.Value;
					TransformConstraintTimeline transformConstraintTimeline = new TransformConstraintTimeline(list4.Count)
					{
						TransformConstraintIndex = skeletonData.TransformConstraints.IndexOf(item2)
					};
					int num10 = 0;
					foreach (object obj7 in list4)
					{
						Dictionary<string, object> dictionary7 = (Dictionary<string, object>)obj7;
						transformConstraintTimeline.SetFrame(num10, GetFloat(dictionary7, "time", 0f), GetFloat(dictionary7, "rotateMix", 1f), GetFloat(dictionary7, "translateMix", 1f), GetFloat(dictionary7, "scaleMix", 1f), GetFloat(dictionary7, "shearMix", 1f));
						ReadCurve(dictionary7, transformConstraintTimeline, num10);
						num10++;
					}
					exposedList.Add(transformConstraintTimeline);
					num = Math.Max(num, transformConstraintTimeline.Frames[(transformConstraintTimeline.FrameCount - 1) * 5]);
				}
			}
			if (map.ContainsKey("path"))
			{
				foreach (KeyValuePair<string, object> keyValuePair7 in ((Dictionary<string, object>)map["path"]))
				{
					int num11 = skeletonData.FindPathConstraintIndex(keyValuePair7.Key);
					bool flag14 = num11 == -1;
					if (num11 == -1)
					{
						throw new Exception("Path constraint not found: " + keyValuePair7.Key);
					}
					PathConstraintData pathConstraintData = skeletonData.PathConstraints.Items[num11];
					foreach (KeyValuePair<string, object> keyValuePair8 in ((Dictionary<string, object>)keyValuePair7.Value))
					{
						List<object> list5 = (List<object>)keyValuePair8.Value;
						string key5 = keyValuePair8.Key;
						bool flag15 = key5 == "position" || key5 == "spacing";
						if (key5 == "position" || key5 == "spacing")
						{
							float num12 = 1f;
							bool flag16 = key5 == "spacing";
							PathConstraintPositionTimeline pathConstraintPositionTimeline;
							if (key5 == "spacing")
							{
								pathConstraintPositionTimeline = new PathConstraintSpacingTimeline(list5.Count);
								bool flag17 = pathConstraintData.SpacingMode == SpacingMode.Length || pathConstraintData.SpacingMode == SpacingMode.Fixed;
								if (pathConstraintData.SpacingMode == SpacingMode.Length || pathConstraintData.SpacingMode == SpacingMode.Fixed)
								{
									num12 = scale;
								}
							}
							else
							{
								pathConstraintPositionTimeline = new PathConstraintPositionTimeline(list5.Count);
								bool flag18 = pathConstraintData.PositionMode == PositionMode.Fixed;
								if (pathConstraintData.PositionMode == PositionMode.Fixed)
								{
									num12 = scale;
								}
							}
							pathConstraintPositionTimeline.PathConstraintIndex = num11;
							int num13 = 0;
							foreach (object obj8 in list5)
							{
								Dictionary<string, object> dictionary8 = (Dictionary<string, object>)obj8;
								pathConstraintPositionTimeline.SetFrame(num13, GetFloat(dictionary8, "time", 0f), GetFloat(dictionary8, key5, 0f) * num12);
								ReadCurve(dictionary8, pathConstraintPositionTimeline, num13);
								num13++;
							}
							exposedList.Add(pathConstraintPositionTimeline);
							num = Math.Max(num, pathConstraintPositionTimeline.Frames[(pathConstraintPositionTimeline.FrameCount - 1) * 2]);
						}
						else
						{
							bool flag19 = key5 == "mix";
							if (key5 == "mix")
							{
								PathConstraintMixTimeline pathConstraintMixTimeline = new PathConstraintMixTimeline(list5.Count)
								{
									PathConstraintIndex = num11
								};
								int num14 = 0;
								foreach (object obj9 in list5)
								{
									Dictionary<string, object> dictionary9 = (Dictionary<string, object>)obj9;
									pathConstraintMixTimeline.SetFrame(num14, GetFloat(dictionary9, "time", 0f), GetFloat(dictionary9, "rotateMix", 1f), GetFloat(dictionary9, "translateMix", 1f));
									ReadCurve(dictionary9, pathConstraintMixTimeline, num14);
									num14++;
								}
								exposedList.Add(pathConstraintMixTimeline);
								num = Math.Max(num, pathConstraintMixTimeline.Frames[(pathConstraintMixTimeline.FrameCount - 1) * 3]);
							}
						}
					}
				}
			}
			if (map.ContainsKey("deform"))
			{
				foreach (KeyValuePair<string, object> keyValuePair9 in ((Dictionary<string, object>)map["deform"]))
				{
					Skin skin = skeletonData.FindSkin(keyValuePair9.Key);
					foreach (KeyValuePair<string, object> keyValuePair10 in ((Dictionary<string, object>)keyValuePair9.Value))
					{
						int num15 = skeletonData.FindSlotIndex(keyValuePair10.Key);
						bool flag21 = num15 == -1;
						if (num15 == -1)
						{
							throw new Exception("Slot not found: " + keyValuePair10.Key);
						}
						foreach (KeyValuePair<string, object> keyValuePair11 in ((Dictionary<string, object>)keyValuePair10.Value))
						{
							List<object> list6 = (List<object>)keyValuePair11.Value;
							VertexAttachment vertexAttachment = (VertexAttachment)skin.GetAttachment(num15, keyValuePair11.Key);
							bool flag22 = vertexAttachment == null;
							if (vertexAttachment == null)
							{
								throw new Exception("Deform attachment not found: " + keyValuePair11.Key);
							}
							bool flag23 = vertexAttachment.Bones != null;
							float[] vertices = vertexAttachment.Vertices;
							int num16 = flag23 ? (vertices.Length / 3 * 2) : vertices.Length;
							DeformTimeline deformTimeline = new DeformTimeline(list6.Count)
							{
								SlotIndex = num15,
								Attachment = vertexAttachment
							};
							int num17 = 0;
							foreach (object obj10 in list6)
							{
								Dictionary<string, object> dictionary10 = (Dictionary<string, object>)obj10;
								bool flag24 = !dictionary10.ContainsKey("vertices");
								float[] array;
								if (flag24)
								{
									array = (flag23 ? new float[num16] : vertices);
								}
								else
								{
									array = new float[num16];
									int @int = GetInt(dictionary10, "offset", 0);
									float[] floatArray = GetFloatArray(dictionary10, "vertices", 1f);
									Array.Copy(floatArray, 0, array, @int, floatArray.Length);
									bool flag25 = scale != 1f;
									if (scale != 1f)
									{
										int i = @int;
										int num18 = i + floatArray.Length;
										while (i < num18)
										{
											array[i] *= scale;
											i++;
										}
									}
									bool flag26 = !flag23;
									if (!flag23)
									{
										for (int j = 0; j < num16; j++)
										{
											array[j] += vertices[j];
										}
									}
								}
								deformTimeline.SetFrame(num17, GetFloat(dictionary10, "time", 0f), array);
								ReadCurve(dictionary10, deformTimeline, num17);
								num17++;
							}
							exposedList.Add(deformTimeline);
							num = Math.Max(num, deformTimeline.Frames[deformTimeline.FrameCount - 1]);
						}
					}
				}
			}
			if (map.ContainsKey("drawOrder") || map.ContainsKey("draworder"))
			{
				List<object> list7 = (List<object>)map[map.ContainsKey("drawOrder") ? "drawOrder" : "draworder"];
				DrawOrderTimeline drawOrderTimeline = new DrawOrderTimeline(list7.Count);
				int count = skeletonData.Slots.Count;
				int num19 = 0;
				foreach (object obj11 in list7)
				{
					Dictionary<string, object> dictionary11 = (Dictionary<string, object>)obj11;
					int[] array2 = null;
					bool flag28 = dictionary11.ContainsKey("offsets");
					if (dictionary11.ContainsKey("offsets"))
					{
						array2 = new int[count];
						for (int k = count - 1; k >= 0; k--)
						{
							array2[k] = -1;
						}
						List<object> list8 = (List<object>)dictionary11["offsets"];
						int[] array3 = new int[count - list8.Count];
						int num20 = 0;
						int num21 = 0;
						foreach (object obj12 in list8)
						{
							Dictionary<string, object> dictionary12 = (Dictionary<string, object>)obj12;
							int num22 = skeletonData.FindSlotIndex((string)dictionary12["slot"]);
							bool flag29 = num22 == -1;
							if (num22 == -1)
							{
								string str = "Slot not found: ";
								object obj13 = dictionary12["slot"];
								throw new Exception(str + (obj13?.ToString()));
							}
							while (num20 != num22)
							{
								array3[num21++] = num20++;
							}
							int num23 = num20 + (int)((float)dictionary12["offset"]);
							array2[num23] = num20++;
						}
						for (; ; )
						{
							bool flag30 = num20 >= count;
							if (num20 >= count)
							{
								break;
							}
							array3[num21++] = num20++;
						}
						for (int l = count - 1; l >= 0; l--)
						{
							bool flag31 = array2[l] == -1;
							if (array2[l] == -1)
							{
								array2[l] = array3[--num21];
							}
						}
					}
					drawOrderTimeline.SetFrame(num19++, GetFloat(dictionary11, "time", 0f), array2);
				}
				exposedList.Add(drawOrderTimeline);
				num = Math.Max(num, drawOrderTimeline.Frames[drawOrderTimeline.FrameCount - 1]);
			}
			if (map.ContainsKey("events"))
			{
				List<object> list9 = (List<object>)map["events"];
				EventTimeline eventTimeline = new EventTimeline(list9.Count);
				int num24 = 0;
				foreach (object obj14 in list9)
				{
					Dictionary<string, object> dictionary13 = (Dictionary<string, object>)obj14;
					EventData eventData = skeletonData.FindEvent((string)dictionary13["name"]);
					bool flag33 = eventData == null;
					if (eventData == null)
					{
						string str2 = "Event not found: ";
						object obj15 = dictionary13["name"];
						throw new Exception(str2 + (obj15?.ToString()));
					}
					Event @event = new Event(GetFloat(dictionary13, "time", 0f), eventData)
					{
						Int = GetInt(dictionary13, "int", eventData.Int),
						Float = GetFloat(dictionary13, "float", eventData.Float),
						String = GetString(dictionary13, "string", eventData.String)
					};
					bool flag34 = @event.Data.AudioPath != null;
					if (@event.Data.AudioPath != null)
					{
						@event.Volume = GetFloat(dictionary13, "volume", eventData.Volume);
						@event.Balance = GetFloat(dictionary13, "balance", eventData.Balance);
					}
					eventTimeline.SetFrame(num24++, @event);
				}
				exposedList.Add(eventTimeline);
				num = Math.Max(num, eventTimeline.Frames[eventTimeline.FrameCount - 1]);
			}
			exposedList.TrimExcess();
			skeletonData.Animations.Add(new Spine.Animation(name, exposedList, num));
		}
		public static void ReadVertices(SkeletonJson __instance, Dictionary<string, object> map, VertexAttachment attachment, int verticesLength)
		{
			attachment.WorldVerticesLength = verticesLength;
			float[] floatArray = GetFloatArray(map, "vertices", 1f);
			float scale = __instance.Scale;
			if (verticesLength == floatArray.Length)
			{
				if (scale != 1f)
				{
					for (int i = 0; i < floatArray.Length; i++)
					{
						floatArray[i] *= scale;
					}
				}
				attachment.Vertices = floatArray;
			}
			else
			{
				ExposedList<float> exposedList = new ExposedList<float>(verticesLength * 3 * 3);
				ExposedList<int> exposedList2 = new ExposedList<int>(verticesLength * 3);
				int j = 0;
				int num = floatArray.Length;
				while (j < num)
				{
					int num2 = (int)floatArray[j++];
					exposedList2.Add(num2);
					int num3 = j + num2 * 4;
					while (j < num3)
					{
						exposedList2.Add((int)floatArray[j]);
						exposedList.Add(floatArray[j + 1] * __instance.Scale);
						exposedList.Add(floatArray[j + 2] * __instance.Scale);
						exposedList.Add(floatArray[j + 3]);
						j += 4;
					}
				}
				attachment.Bones = exposedList2.ToArray();
				attachment.Vertices = exposedList.ToArray();
			}
		}
		public static Attachment ReadAttachment(SkeletonJson __instance, Dictionary<string, object> map, Skin skin, int slotIndex, string name, SkeletonData skeletonData)
		{
			float scale = __instance.Scale;
			AttachmentLoader attachmentLoader;
			try
			{
				attachmentLoader = (AttachmentLoader)__instance.GetType().GetField("attachmentLoader", AccessTools.all).GetValue(__instance);
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/BaseMods/attachmenterror.log", ex.Message + Environment.NewLine + ex.StackTrace);
				return null;
			}
			name = GetString(map, "name", name);
			string @string = GetString(map, "type", "region");
			AttachmentType attachmentType = (AttachmentType)Enum.Parse(typeof(AttachmentType), @string, true);
			string string2 = GetString(map, "path", name);
			Attachment result;
			switch (attachmentType)
			{
				case AttachmentType.Region:
					{
						RegionAttachment regionAttachment = attachmentLoader.NewRegionAttachment(skin, name, string2);
						if (regionAttachment == null)
						{
							result = null;
						}
						else
						{
							regionAttachment.Path = string2;
							regionAttachment.X = GetFloat(map, "x", 0f) * scale;
							regionAttachment.Y = GetFloat(map, "y", 0f) * scale;
							regionAttachment.ScaleX = GetFloat(map, "scaleX", 1f);
							regionAttachment.ScaleY = GetFloat(map, "scaleY", 1f);
							regionAttachment.Rotation = GetFloat(map, "rotation", 0f);
							regionAttachment.Width = GetFloat(map, "width", 32f) * scale;
							regionAttachment.Height = GetFloat(map, "height", 32f) * scale;
							if (map.ContainsKey("color"))
							{
								string hexString = (string)map["color"];
								regionAttachment.R = ToColor(hexString, 0, 8);
								regionAttachment.G = ToColor(hexString, 1, 8);
								regionAttachment.B = ToColor(hexString, 2, 8);
								regionAttachment.A = ToColor(hexString, 3, 8);
							}
							regionAttachment.UpdateOffset();
							result = regionAttachment;
						}
						break;
					}
				case AttachmentType.Boundingbox:
					{
						BoundingBoxAttachment boundingBoxAttachment = attachmentLoader.NewBoundingBoxAttachment(skin, name);
						if (boundingBoxAttachment == null)
						{
							result = null;
						}
						else
						{
							ReadVertices(__instance, map, boundingBoxAttachment, GetInt(map, "vertexCount", 0) << 1);
							result = boundingBoxAttachment;
						}
						break;
					}
				case AttachmentType.Mesh:
				case AttachmentType.Linkedmesh:
					{
						MeshAttachment meshAttachment = attachmentLoader.NewMeshAttachment(skin, name, string2);
						if (meshAttachment == null)
						{
							result = null;
						}
						else
						{
							meshAttachment.Path = string2;
							if (map.ContainsKey("color"))
							{
								string hexString2 = (string)map["color"];
								meshAttachment.R = ToColor(hexString2, 0, 8);
								meshAttachment.G = ToColor(hexString2, 1, 8);
								meshAttachment.B = ToColor(hexString2, 2, 8);
								meshAttachment.A = ToColor(hexString2, 3, 8);
							}
							meshAttachment.Width = GetFloat(map, "width", 0f) * scale;
							meshAttachment.Height = GetFloat(map, "height", 0f) * scale;
							string string3 = GetString(map, "parent", null);
							if (string3 != null)
							{
								linkedMeshes.Add(new LinkedMesh(meshAttachment, GetString(map, "skin", null), slotIndex, string3, GetBoolean(map, "deform", true)));
								result = meshAttachment;
							}
							else
							{
								float[] floatArray = GetFloatArray(map, "uvs", 1f);
								ReadVertices(__instance, map, meshAttachment, floatArray.Length);
								meshAttachment.Triangles = GetIntArray(map, "triangles");
								meshAttachment.RegionUVs = floatArray;
								meshAttachment.UpdateUVs();
								if (map.ContainsKey("hull"))
								{
									meshAttachment.HullLength = GetInt(map, "hull", 0) * 2;
								}
								if (map.ContainsKey("edges"))
								{
									meshAttachment.Edges = GetIntArray(map, "edges");
								}
								result = meshAttachment;
							}
						}
						break;
					}
				case AttachmentType.Path:
					{
						PathAttachment pathAttachment = attachmentLoader.NewPathAttachment(skin, name);
						if (pathAttachment == null)
						{
							result = null;
						}
						else
						{
							pathAttachment.Closed = GetBoolean(map, "closed", false);
							pathAttachment.ConstantSpeed = GetBoolean(map, "constantSpeed", true);
							int @int = GetInt(map, "vertexCount", 0);
							ReadVertices(__instance, map, pathAttachment, @int << 1);
							pathAttachment.Lengths = GetFloatArray(map, "lengths", scale);
							result = pathAttachment;
						}
						break;
					}
				case AttachmentType.Point:
					{
						PointAttachment pointAttachment = attachmentLoader.NewPointAttachment(skin, name);
						if (pointAttachment == null)
						{
							result = null;
						}
						else
						{
							pointAttachment.X = GetFloat(map, "x", 0f) * scale;
							pointAttachment.Y = GetFloat(map, "y", 0f) * scale;
							pointAttachment.Rotation = GetFloat(map, "rotation", 0f);
							result = pointAttachment;
						}
						break;
					}
				case AttachmentType.Clipping:
					{
						ClippingAttachment clippingAttachment = attachmentLoader.NewClippingAttachment(skin, name);
						if (clippingAttachment == null)
						{
							result = null;
						}
						else
						{
							string string4 = GetString(map, "end", null);
							if (string4 != null)
							{
								SlotData slotData = skeletonData.FindSlot(string4);
								clippingAttachment.EndSlot = slotData ?? throw new Exception("Clipping end slot not found: " + string4);
							}
							ReadVertices(__instance, map, clippingAttachment, GetInt(map, "vertexCount", 0) << 1);
							result = clippingAttachment;
						}
						break;
					}
				default:
					result = null;
					break;
			}
			return result;
		}
		public static Attachment ReadAttachment_new(SkeletonJson __instance, Dictionary<string, object> map, Skin skin, int slotIndex, string name, SkeletonData skeletonData)
		{
			AttachmentLoader attachmentLoader = (AttachmentLoader)__instance.GetType().GetField("attachmentLoader", AccessTools.all).GetValue(__instance);
			float scale = __instance.Scale;
			name = GetString(map, "name", name);
			string @string = GetString(map, "type", "region");
			AttachmentType attachmentType = (AttachmentType)Enum.Parse(typeof(AttachmentType), @string, true);
			string string2 = GetString(map, "path", name);
			Attachment result;
			switch (attachmentType)
			{
				case AttachmentType.Region:
					{
						RegionAttachment regionAttachment = attachmentLoader.NewRegionAttachment(skin, name, string2);
						if (regionAttachment == null)
						{
							result = null;
						}
						else
						{
							regionAttachment.Path = string2;
							regionAttachment.X = GetFloat(map, "x", 0f) * scale;
							regionAttachment.Y = GetFloat(map, "y", 0f) * scale;
							regionAttachment.ScaleX = GetFloat(map, "scaleX", 1f);
							regionAttachment.ScaleY = GetFloat(map, "scaleY", 1f);
							regionAttachment.Rotation = GetFloat(map, "rotation", 0f);
							regionAttachment.Width = GetFloat(map, "width", 32f) * scale;
							regionAttachment.Height = GetFloat(map, "height", 32f) * scale;
							if (map.ContainsKey("color"))
							{
								string hexString = (string)map["color"];
								regionAttachment.R = ToColor(hexString, 0, 8);
								regionAttachment.G = ToColor(hexString, 1, 8);
								regionAttachment.B = ToColor(hexString, 2, 8);
								regionAttachment.A = ToColor(hexString, 3, 8);
							}
							regionAttachment.UpdateOffset();
							result = regionAttachment;
						}
						break;
					}
				case AttachmentType.Boundingbox:
					{
						BoundingBoxAttachment boundingBoxAttachment = attachmentLoader.NewBoundingBoxAttachment(skin, name);
						if (boundingBoxAttachment == null)
						{
							result = null;
						}
						else
						{
							ReadVertices(__instance, map, boundingBoxAttachment, GetInt(map, "vertexCount", 0) << 1);
							result = boundingBoxAttachment;
						}
						break;
					}
				case AttachmentType.Mesh:
				case AttachmentType.Linkedmesh:
					{
						MeshAttachment meshAttachment = attachmentLoader.NewMeshAttachment(skin, name, string2);
						if (meshAttachment == null)
						{
							result = null;
						}
						else
						{
							meshAttachment.Path = string2;
							if (map.ContainsKey("color"))
							{
								string hexString2 = (string)map["color"];
								meshAttachment.R = ToColor(hexString2, 0, 8);
								meshAttachment.G = ToColor(hexString2, 1, 8);
								meshAttachment.B = ToColor(hexString2, 2, 8);
								meshAttachment.A = ToColor(hexString2, 3, 8);
							}
							meshAttachment.Width = GetFloat(map, "width", 0f) * scale;
							meshAttachment.Height = GetFloat(map, "height", 0f) * scale;
							string string3 = GetString(map, "parent", null);
							if (string3 != null)
							{
								linkedMeshes.Add(new LinkedMesh(meshAttachment, GetString(map, "skin", null), slotIndex, string3, true));
								result = meshAttachment;
							}
							else
							{
								float[] floatArray = GetFloatArray(map, "uvs", 1f);
								ReadVertices(__instance, map, meshAttachment, floatArray.Length);
								meshAttachment.Triangles = GetIntArray(map, "triangles");
								meshAttachment.RegionUVs = floatArray;
								meshAttachment.UpdateUVs();
								if (map.ContainsKey("hull"))
								{
									meshAttachment.HullLength = GetInt(map, "hull", 0) * 2;
								}
								if (map.ContainsKey("edges"))
								{
									meshAttachment.Edges = GetIntArray(map, "edges");
								}
								result = meshAttachment;
							}
						}
						break;
					}
				case AttachmentType.Path:
					{
						PathAttachment pathAttachment = attachmentLoader.NewPathAttachment(skin, name);
						if (pathAttachment == null)
						{
							result = null;
						}
						else
						{
							pathAttachment.Closed = GetBoolean(map, "closed", false);
							pathAttachment.ConstantSpeed = GetBoolean(map, "constantSpeed", true);
							int @int = GetInt(map, "vertexCount", 0);
							ReadVertices(__instance, map, pathAttachment, @int << 1);
							pathAttachment.Lengths = GetFloatArray(map, "lengths", scale);
							result = pathAttachment;
						}
						break;
					}
				case AttachmentType.Point:
					{
						PointAttachment pointAttachment = attachmentLoader.NewPointAttachment(skin, name);
						if (pointAttachment == null)
						{
							result = null;
						}
						else
						{
							pointAttachment.X = GetFloat(map, "x", 0f) * scale;
							pointAttachment.Y = GetFloat(map, "y", 0f) * scale;
							pointAttachment.Rotation = GetFloat(map, "rotation", 0f);
							result = pointAttachment;
						}
						break;
					}
				case AttachmentType.Clipping:
					{
						ClippingAttachment clippingAttachment = attachmentLoader.NewClippingAttachment(skin, name);
						if (clippingAttachment == null)
						{
							result = null;
						}
						else
						{
							string string4 = GetString(map, "end", null);
							if (string4 != null)
							{
								SlotData slotData = skeletonData.FindSlot(string4);
								clippingAttachment.EndSlot = slotData ?? throw new Exception("Clipping end slot not found: " + string4);
							}
							ReadVertices(__instance, map, clippingAttachment, GetInt(map, "vertexCount", 0) << 1);
							result = clippingAttachment;
						}
						break;
					}
				default:
					result = null;
					break;
			}
			return result;
		}
		public static float GetFloat(Dictionary<string, object> map, string name, float defaultValue)
		{
			float result;
			if (!map.ContainsKey(name))
			{
				result = defaultValue;
			}
			else
			{
				result = (float)map[name];
			}
			return result;
		}
		public static int GetInt(Dictionary<string, object> map, string name, int defaultValue)
		{
			int result;
			if (!map.ContainsKey(name))
			{
				result = defaultValue;
			}
			else
			{
				result = (int)((float)map[name]);
			}
			return result;
		}
		public static bool GetBoolean(Dictionary<string, object> map, string name, bool defaultValue)
		{
			bool result;
			if (!map.ContainsKey(name))
			{
				result = defaultValue;
			}
			else
			{
				result = (bool)map[name];
			}
			return result;
		}
		public static string GetString(Dictionary<string, object> map, string name, string defaultValue)
		{
			string result;
			if (!map.ContainsKey(name))
			{
				result = defaultValue;
			}
			else
			{
				result = (string)map[name];
			}
			return result;
		}
		public static float[] GetFloatArray(Dictionary<string, object> map, string name, float scale)
		{
			List<object> list = (List<object>)map[name];
			float[] array = new float[list.Count];
			if (scale == 1f)
			{
				int i = 0;
				int count = list.Count;
				while (i < count)
				{
					array[i] = (float)list[i];
					i++;
				}
			}
			else
			{
				int j = 0;
				int count2 = list.Count;
				while (j < count2)
				{
					array[j] = (float)list[j] * scale;
					j++;
				}
			}
			return array;
		}
		public static int[] GetIntArray(Dictionary<string, object> map, string name)
		{
			List<object> list = (List<object>)map[name];
			int[] array = new int[list.Count];
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				array[i] = (int)((float)list[i]);
				i++;
			}
			return array;
		}
		public static void ReadCurve(Dictionary<string, object> valueMap, CurveTimeline timeline, int frameIndex)
		{
			if (valueMap.ContainsKey("curve"))
			{
				object obj = valueMap["curve"];
				if (obj is string)
				{
					timeline.SetStepped(frameIndex);
				}
				else
				{
					timeline.SetCurve(frameIndex, (float)obj, GetFloat(valueMap, "c2", 0f), GetFloat(valueMap, "c3", 1f), GetFloat(valueMap, "c4", 1f));
				}
			}
		}
		public static float ToColor(string hexString, int colorIndex, int expectedLength = 8)
		{
			if (hexString.Length != expectedLength)
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"Color hexidecimal length must be ",
					expectedLength,
					", recieved: ",
					hexString
				}), "hexString");
			}
			return Convert.ToInt32(hexString.Substring(colorIndex * 2, 2), 16) / 255f;
		}
		static SkeletonJSON_new()
		{
			linkedMeshes = new List<LinkedMesh>();
		}
		public static List<LinkedMesh> linkedMeshes;

		public class LinkedMesh
		{
			public LinkedMesh(MeshAttachment mesh, string skin, int slotIndex, string parent, bool inheritDeform)
			{
				this.mesh = mesh;
				this.skin = skin;
				this.slotIndex = slotIndex;
				this.parent = parent;
				this.inheritDeform = inheritDeform;
			}

			internal string parent;

			internal string skin;

			internal int slotIndex;

			internal MeshAttachment mesh;

			internal bool inheritDeform;
		}
	}
}
