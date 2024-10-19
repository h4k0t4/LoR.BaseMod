using GameSave;
using Mod;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace BaseMod
{
	public class UtilTools
	{
		public static Font _DefFont
		{
			get
			{
				if (DefFont == null)
				{
					DefFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
					DefFontColor = UIColorManager.Manager.GetUIColor(UIColor.Default);
				}
				return DefFont;
			}
		}
		public static Color _DefFontColor
		{
			get
			{
				return DefFontColor;
			}
		}

		public static InputField CreateInputField(Transform parent, string Imagepath, Vector2 position, TextAnchor tanchor, int fsize, Color tcolor, Font font)
		{
			GameObject gameObject = CreateImage(parent, Imagepath, new Vector2(1f, 1f), position).gameObject;
			Text text = CreateText(gameObject.transform, new Vector2(0f, 0f), fsize, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 0f), tanchor, tcolor, font);
			text.text = "";
			InputField inputField = gameObject.AddComponent<InputField>();
			inputField.targetGraphic = gameObject.GetComponent<Image>();
			inputField.textComponent = text;
			return inputField;
		}
		public static void DeepCopyGameObject(Transform original, Transform copyed)
		{
			copyed.localPosition = original.localPosition;
			copyed.localRotation = original.localRotation;
			copyed.localScale = original.localScale;
			copyed.gameObject.layer = original.gameObject.layer;
			for (int i = 0; i < copyed.childCount; i++)
			{
				DeepCopyGameObject(original.GetChild(i), copyed.GetChild(i));
			}
		}
		public static IEnumerator RenderCam_2(int index, UICharacterRenderer renderer)
		{
			return renderer.RenderCam(index);
		}
		public static Button AddButton(Image target)
		{
			Button button = target.gameObject.AddComponent<Button>();
			button.targetGraphic = target;
			return button;
		}
		public static Image CreateImage(Transform parent, string Imagepath, Vector2 scale, Vector2 position)
		{
			GameObject gameObject = new GameObject("Image");
			Image image = gameObject.AddComponent<Image>();
			image.transform.SetParent(parent);
			Texture2D texture2D = new Texture2D(2, 2);
			texture2D.LoadImage(File.ReadAllBytes(Imagepath));
			Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
			image.sprite = sprite;
			image.rectTransform.sizeDelta = new Vector2(texture2D.width, texture2D.height);
			gameObject.SetActive(true);
			gameObject.transform.localScale = scale;
			gameObject.transform.localPosition = position;
			return image;
		}
		public static Image CreateImage(Transform parent, Sprite Image, Vector2 scale, Vector2 position)
		{
			GameObject gameObject = new GameObject("Image");
			Image image = gameObject.AddComponent<Image>();
			image.transform.SetParent(parent);
			image.sprite = Image;
			image.rectTransform.sizeDelta = new Vector2(Image.texture.width, Image.texture.height);
			gameObject.SetActive(true);
			gameObject.transform.localScale = scale;
			gameObject.transform.localPosition = position;
			return image;
		}
		public static Text CreateText(Transform target, Vector2 position, int fsize, Vector2 anchormin, Vector2 anchormax, Vector2 anchorposition, TextAnchor anchor, Color tcolor, Font font)
		{
			GameObject gameObject = new GameObject("Text");
			Text text = gameObject.AddComponent<Text>();
			gameObject.transform.SetParent(target);
			text.rectTransform.sizeDelta = Vector2.zero;
			text.rectTransform.anchorMin = anchormin;
			text.rectTransform.anchorMax = anchormax;
			text.rectTransform.anchoredPosition = anchorposition;
			text.text = " ";
			text.font = font;
			text.fontSize = fsize;
			text.color = tcolor;
			text.alignment = anchor;
			gameObject.transform.localScale = new Vector3(1f, 1f);
			gameObject.transform.localPosition = position;
			gameObject.SetActive(true);
			return text;
		}
		public static TextMeshProUGUI CreateText_TMP(Transform target, Vector2 position, int fsize, Vector2 anchormin, Vector2 anchormax, Vector2 anchorposition, TextAlignmentOptions anchor, Color tcolor, TMP_FontAsset font)
		{
			GameObject gameObject = new GameObject("Text");
			TextMeshProUGUI textMeshProUGUI = gameObject.AddComponent<TextMeshProUGUI>();
			gameObject.transform.SetParent(target);
			textMeshProUGUI.rectTransform.sizeDelta = Vector2.zero;
			textMeshProUGUI.rectTransform.anchorMin = anchormin;
			textMeshProUGUI.rectTransform.anchorMax = anchormax;
			textMeshProUGUI.rectTransform.anchoredPosition = anchorposition;
			textMeshProUGUI.text = " ";
			textMeshProUGUI.font = font;
			textMeshProUGUI.fontSize = fsize;
			textMeshProUGUI.color = tcolor;
			textMeshProUGUI.alignment = anchor;
			gameObject.transform.localScale = new Vector3(1f, 1f);
			gameObject.transform.localPosition = position;
			gameObject.SetActive(true);
			return textMeshProUGUI;
		}
		public static Text CreateText(Transform target)
		{
			return CreateText(target, new Vector2(0f, 0f), 10, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 0f), TextAnchor.UpperLeft, Color.black, DefFont);
		}
		public static Button CreateButton(Transform parent, string Imagepath, Vector2 scale, Vector2 position)
		{
			Image image = CreateImage(parent, Imagepath, scale, position);
			GameObject gameObject = image.gameObject;
			Button button = gameObject.AddComponent<Button>();
			button.targetGraphic = image;
			return button;
		}
		public static Button CreateButton(Transform parent, Sprite Image, Vector2 scale, Vector2 position)
		{
			Image image = CreateImage(parent, Image, scale, position);
			Button button = image.gameObject.AddComponent<Button>();
			button.targetGraphic = image;
			return button;
		}
		public static Button CreateButton(Transform parent, string Imagepath)
		{
			return CreateButton(parent, Imagepath, new Vector2(1f, 1f), new Vector2(0f, 0f));
		}
		public static UIOriginEquipPageSlot DuplicateEquipPageSlot(UIOriginEquipPageSlot origin, UIOriginEquipPageList parent)
		{
			if (origin is UISettingInvenEquipPageSlot slot)
			{
				return UnityEngine.Object.Instantiate(slot, origin.transform.parent);
			}
			else
			{
				return UnityEngine.Object.Instantiate((UIInvenEquipPageSlot)origin, origin.transform.parent);
			}
			//Instantiate copies all relative paths anyway, so there's no need to assign them manually
		}
		public static void LoadFromSaveData_GiftInventory(GiftInventory __instance, SaveData data)
		{
			try
			{
				__instance.LoadFromSaveData(data);
			}
			catch { }
		}
		public static void SpriteTrace(string path, Sprite sprite)
		{
			string text = sprite.name + Environment.NewLine;
			text = text + sprite.rect.ToString() + Environment.NewLine;
			text = text + sprite.border.ToString() + Environment.NewLine;
			text = text + sprite.pivot.ToString() + Environment.NewLine;
			File.WriteAllText(path, text);
		}
		public static DirectoryInfo FindExistDir(string path)
		{
			foreach (ModContent modContent in Harmony_Patch.LoadedModContents)
			{
				DirectoryInfo _dirInfo = modContent._dirInfo;
				if (Directory.Exists(_dirInfo.FullName + "/" + path))
				{
					return new DirectoryInfo(_dirInfo.FullName + "/" + path);
				}
			}
			return null;
		}
		public static void CopyDir(string srcPath, string aimPath)
		{
			try
			{
				if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
				{
					aimPath += Path.DirectorySeparatorChar;
				}
				if (!Directory.Exists(aimPath))
				{
					Directory.CreateDirectory(aimPath);
				}
				string[] fileList = Directory.GetFileSystemEntries(srcPath);
				foreach (string file in fileList)
				{
					if (Directory.Exists(file))
					{
						CopyDir(file, aimPath + Path.GetFileName(file));
					}
					else
					{
						File.Copy(file, aimPath + Path.GetFileName(file), true);
					}
				}
			}
			catch (Exception ex)
			{
				ModContentManager.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
				File.WriteAllText(Application.dataPath + "/Mods/CopyDirerror.log", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		public static Shortcut CreateShortcut(string shortcutDirectory, string shortcutName, string targetPath, string targetDirectory, string description = null, string iconLocation = null)
		{
			if (!Directory.Exists(shortcutDirectory))
			{
				Directory.CreateDirectory(shortcutDirectory);
			}
			string shortcutPath = Path.Combine(shortcutDirectory, string.Format("{0}.lnk", shortcutName));
			Shortcut sc = new Shortcut
			{
				Path = targetPath,
				Arguments = "",
				WorkingDirectory = targetDirectory,
				Description = description,
			};
			sc.Save(shortcutPath);
			return sc;
		}
		public static string GetTransformPath(Transform transform, bool includeSelf = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (includeSelf)
			{
				stringBuilder.Append(transform.name);
			}
			while (transform.parent)
			{
				transform = transform.parent;
				stringBuilder.Insert(0, '/');
				stringBuilder.Insert(0, transform.name);
			}
			return stringBuilder.ToString();
		}
		public static Transform GetTransform(string input)
		{
			Transform result = null;
			if (input.EndsWith("/"))
			{
				input = input.Remove(input.Length - 1);
			}
			GameObject gameObject = GameObject.Find(input);
			if (gameObject != null)
			{
				result = gameObject.transform;
			}
			else
			{
				string b = input.Split(new char[]
				{
					'/'
				}).Last();
				UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(GameObject));
				List<GameObject> list = new List<GameObject>();
				foreach (UnityEngine.Object @object in array)
				{
					if (@object.name == b)
					{
						list.Add((GameObject)@object);
					}
				}
				foreach (GameObject gameObject2 in list)
				{
					string text = GetTransformPath(gameObject2.transform, true);
					if (text.EndsWith("/"))
					{
						text = text.Remove(text.Length - 1);
					}
					if (text == input)
					{
						result = gameObject2.transform;
						break;
					}
				}
			}
			return result;
		}

		public static Font DefFont;

		public static TMP_FontAsset DefFont_TMP;

		public static Color DefFontColor;
	}
}
