using GameSave;
using HarmonyLib;
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
            yield return YieldCache.waitFrame;
            renderer.cameraList[index].targetTexture.Release();
            renderer.cameraList[index].Render();
            yield break;
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
            new Texture2D(2, 2);
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
            UIOriginEquipPageSlot uioriginEquipPageSlot;
            if (origin is UISettingInvenEquipPageSlot slot)
            {
                uioriginEquipPageSlot = UnityEngine.Object.Instantiate(slot, origin.transform.parent);
            }
            else
            {
                uioriginEquipPageSlot = UnityEngine.Object.Instantiate((UIInvenEquipPageSlot)origin, origin.transform.parent);
            }
            Type typeFromHandle = typeof(UIOriginEquipPageSlot);
            RectTransform value = (RectTransform)uioriginEquipPageSlot.transform.GetChild(0);
            typeFromHandle.GetField("Pivot", AccessTools.all).SetValue(uioriginEquipPageSlot, value);
            CanvasGroup component = uioriginEquipPageSlot.GetComponent<CanvasGroup>();
            typeFromHandle.GetField("cg", AccessTools.all).SetValue(uioriginEquipPageSlot, component);
            if (uioriginEquipPageSlot is UISettingInvenEquipPageSlot)
            {
                Image component2 = uioriginEquipPageSlot.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(1).gameObject.GetComponent<Image>();
                uioriginEquipPageSlot.GetType().GetField("Frame", AccessTools.all).SetValue(uioriginEquipPageSlot, component2);
            }
            else
            {
                Image component2 = uioriginEquipPageSlot.transform.GetChild(0).GetChild(0).GetChild(2).GetChild(1).gameObject.GetComponent<Image>();
                uioriginEquipPageSlot.GetType().GetField("Frame", AccessTools.all).SetValue(uioriginEquipPageSlot, component2);
            }
            if (uioriginEquipPageSlot is UISettingInvenEquipPageSlot)
            {
                Image component3 = uioriginEquipPageSlot.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<Image>();
                uioriginEquipPageSlot.GetType().GetField("FrameGlow", AccessTools.all).SetValue(uioriginEquipPageSlot, component3);
            }
            else
            {
                Image component3 = uioriginEquipPageSlot.transform.GetChild(0).GetChild(0).GetChild(2).GetChild(0).gameObject.GetComponent<Image>();
                uioriginEquipPageSlot.GetType().GetField("FrameGlow", AccessTools.all).SetValue(uioriginEquipPageSlot, component3);
            }
            Image component4 = uioriginEquipPageSlot.transform.GetChild(0).GetChild(2).GetChild(1).gameObject.GetComponent<Image>();
            uioriginEquipPageSlot.GetType().GetField("Icon", AccessTools.all).SetValue(uioriginEquipPageSlot, component4);
            Image component5 = uioriginEquipPageSlot.transform.GetChild(0).GetChild(2).GetChild(0).gameObject.GetComponent<Image>();
            uioriginEquipPageSlot.GetType().GetField("IconGlow", AccessTools.all).SetValue(uioriginEquipPageSlot, component5);
            TextMeshProUGUI component6 = uioriginEquipPageSlot.transform.GetChild(0).GetChild(3).gameObject.GetComponent<TextMeshProUGUI>();
            uioriginEquipPageSlot.GetType().GetField("BookName", AccessTools.all).SetValue(uioriginEquipPageSlot, component6);
            TextMeshProMaterialSetter component7 = uioriginEquipPageSlot.transform.GetChild(0).GetChild(3).gameObject.GetComponent<TextMeshProMaterialSetter>();
            uioriginEquipPageSlot.GetType().GetField("setter_BookName", AccessTools.all).SetValue(uioriginEquipPageSlot, component7);
            if (uioriginEquipPageSlot is UISettingInvenEquipPageSlot)
            {
                UISettingInvenEquipPageSlot uisettingInvenEquipPageSlot = uioriginEquipPageSlot as UISettingInvenEquipPageSlot;
                Button component8 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(4).GetChild(2).GetChild(0).GetChild(0).gameObject.GetComponent<UIEquipPageOperatingButton>();
                uisettingInvenEquipPageSlot.GetType().GetField("button_BookMark", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component8);
                Button component9 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(4).GetChild(2).GetChild(4).GetChild(0).gameObject.GetComponent<UIEquipPageOperatingButton>();
                uisettingInvenEquipPageSlot.GetType().GetField("button_EmptyDeck", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component9);
                Button component10 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(4).GetChild(2).GetChild(2).GetChild(0).gameObject.GetComponent<UIEquipPageOperatingButton>();
                uisettingInvenEquipPageSlot.GetType().GetField("button_Equip", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component10);
                CanvasGroup component11 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(1).gameObject.GetComponent<CanvasGroup>();
                uisettingInvenEquipPageSlot.GetType().GetField("cg_equiproot", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component11);
                CanvasGroup component12 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(4).gameObject.GetComponent<CanvasGroup>();
                uisettingInvenEquipPageSlot.GetType().GetField("cg_operatingPanel", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component12);
                FaceEditor component13 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<FaceEditor>();
                uisettingInvenEquipPageSlot.GetType().GetField("faceEditor", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component13);
                Image component14 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(4).GetChild(2).GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Image>();
                uisettingInvenEquipPageSlot.GetType().GetField("img_bookmarkbuttonIcon", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component14);
                Image component15 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(4).GetChild(2).GetChild(2).GetChild(0).GetChild(0).gameObject.GetComponent<Image>();
                uisettingInvenEquipPageSlot.GetType().GetField("img_equipbuttonIcon", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component15);
                Image component16 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<Image>();
                uisettingInvenEquipPageSlot.GetType().GetField("img_equipFrame", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component16);
                UISettingEquipPageScrollList value2 = (UISettingEquipPageScrollList)origin.GetType().GetField("listRoot", AccessTools.all).GetValue(origin);
                uisettingInvenEquipPageSlot.GetType().GetField("listRoot", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, value2);
                GameObject gameObject = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(1).gameObject;
                uisettingInvenEquipPageSlot.GetType().GetField("ob_equipRoot", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, gameObject);
                GameObject gameObject2 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(4).gameObject;
                uisettingInvenEquipPageSlot.GetType().GetField("ob_OperatingPanel", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, gameObject2);
                TextMeshProUGUI component17 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(4).GetChild(2).GetChild(0).GetChild(0).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
                uisettingInvenEquipPageSlot.GetType().GetField("txt_bookmarkButton", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component17);
                TextMeshProUGUI component18 = uisettingInvenEquipPageSlot.transform.GetChild(0).GetChild(4).GetChild(2).GetChild(2).GetChild(0).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
                uisettingInvenEquipPageSlot.GetType().GetField("txt_equipButton", AccessTools.all).SetValue(uisettingInvenEquipPageSlot, component18);
            }
            else
            {
                UIInvenEquipPageSlot uiinvenEquipPageSlot = uioriginEquipPageSlot as UIInvenEquipPageSlot;
                object component19 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).GetChild(2).GetChild(0).GetChild(0).gameObject.GetComponent<UICustomGraphicObject>();
                uiinvenEquipPageSlot.GetType().GetField("button_BookMark", AccessTools.all).SetValue(uiinvenEquipPageSlot, component19);
                object component20 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).GetChild(2).GetChild(4).GetChild(0).gameObject.GetComponent<UICustomGraphicObject>();
                uiinvenEquipPageSlot.GetType().GetField("button_EmptyDeck", AccessTools.all).SetValue(uiinvenEquipPageSlot, component20);
                object component21 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).GetChild(2).GetChild(2).GetChild(0).gameObject.GetComponent<UICustomGraphicObject>();
                uiinvenEquipPageSlot.GetType().GetField("button_Equip", AccessTools.all).SetValue(uiinvenEquipPageSlot, component21);
                object component22 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).GetChild(2).GetChild(1).GetChild(0).gameObject.GetComponent<UICustomGraphicObject>();
                uiinvenEquipPageSlot.GetType().GetField("button_PassiveSuccession", AccessTools.all).SetValue(uiinvenEquipPageSlot, component22);
                object component23 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).GetChild(2).GetChild(3).GetChild(0).gameObject.GetComponent<UICustomGraphicObject>();
                uiinvenEquipPageSlot.GetType().GetField("button_ReleaseButton", AccessTools.all).SetValue(uiinvenEquipPageSlot, component23);
                CanvasGroup component24 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(1).gameObject.GetComponent<CanvasGroup>();
                uiinvenEquipPageSlot.GetType().GetField("cg_equiproot", AccessTools.all).SetValue(uiinvenEquipPageSlot, component24);
                CanvasGroup component25 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).gameObject.GetComponent<CanvasGroup>();
                uiinvenEquipPageSlot.GetType().GetField("cg_operatingPanel", AccessTools.all).SetValue(uiinvenEquipPageSlot, component25);
                FaceEditor component26 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<FaceEditor>();
                uiinvenEquipPageSlot.GetType().GetField("faceEditor", AccessTools.all).SetValue(uiinvenEquipPageSlot, component26);
                Image component27 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).GetChild(2).GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<Image>();
                uiinvenEquipPageSlot.GetType().GetField("img_bookmarkbuttonIcon", AccessTools.all).SetValue(uiinvenEquipPageSlot, component27);
                Image component28 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).GetChild(2).GetChild(2).GetChild(0).GetChild(1).gameObject.GetComponent<Image>();
                uiinvenEquipPageSlot.GetType().GetField("img_equipbuttonIcon", AccessTools.all).SetValue(uiinvenEquipPageSlot, component28);
                Image component29 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<Image>();
                uiinvenEquipPageSlot.GetType().GetField("img_equipFrame", AccessTools.all).SetValue(uiinvenEquipPageSlot, component29);
                UIEquipPageScrollList value3 = (UIEquipPageScrollList)origin.GetType().GetField("listRoot", AccessTools.all).GetValue(origin);
                uiinvenEquipPageSlot.GetType().GetField("listRoot", AccessTools.all).SetValue(uiinvenEquipPageSlot, value3);
                GameObject gameObject3 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(1).gameObject;
                uiinvenEquipPageSlot.GetType().GetField("ob_equipRoot", AccessTools.all).SetValue(uiinvenEquipPageSlot, gameObject3);
                GameObject gameObject4 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).gameObject;
                uiinvenEquipPageSlot.GetType().GetField("ob_OperatingPanel", AccessTools.all).SetValue(uiinvenEquipPageSlot, gameObject4);
                TextMeshProUGUI component30 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).GetChild(2).GetChild(0).GetChild(0).GetChild(3).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
                uiinvenEquipPageSlot.GetType().GetField("txt_bookmarkButton", AccessTools.all).SetValue(uiinvenEquipPageSlot, component30);
                TextMeshProUGUI component31 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).GetChild(2).GetChild(2).GetChild(0).GetChild(3).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
                uiinvenEquipPageSlot.GetType().GetField("txt_equipButton", AccessTools.all).SetValue(uiinvenEquipPageSlot, component31);
                TextMeshProUGUI component32 = uiinvenEquipPageSlot.transform.GetChild(0).GetChild(5).GetChild(2).GetChild(3).GetChild(0).GetChild(3).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
                uiinvenEquipPageSlot.GetType().GetField("txt_releaseButton", AccessTools.all).SetValue(uiinvenEquipPageSlot, component32);
            }
            return uioriginEquipPageSlot;
        }
        public static void LoadFromSaveData_GiftInventory(GiftInventory __instance, SaveData data)
        {
            SaveData data2 = data.GetData("equipList");
            SaveData data3 = data.GetData("unequipList");
            SaveData data4 = data.GetData("offList");
            foreach (SaveData saveData in data2)
            {
                if (!(Singleton<GiftXmlList>.Instance.GetData(saveData.GetIntSelf()) == null))
                {
                    GiftModel giftModel = null;
                    try
                    {
                        giftModel = new GiftModel(Singleton<GiftXmlList>.Instance.GetData(saveData.GetIntSelf()));
                    }
                    catch
                    {
                        giftModel = null;
                    }
                    if (giftModel != null)
                    {
                        __instance.AddGift(giftModel);
                        __instance.Equip(giftModel);
                    }
                }
            }
            foreach (SaveData saveData2 in data3)
            {
                if (!(Singleton<GiftXmlList>.Instance.GetData(saveData2.GetIntSelf()) == null))
                {
                    GiftModel giftModel2 = null;
                    try
                    {
                        giftModel2 = new GiftModel(Singleton<GiftXmlList>.Instance.GetData(saveData2.GetIntSelf()));
                    }
                    catch
                    {
                        giftModel2 = null;
                    }
                    if (giftModel2 != null)
                    {
                        __instance.AddGift(giftModel2);
                    }
                }
            }
            if (data4 != null)
            {
                foreach (SaveData idData in data4)
                {
                    if (__instance.GetEquippedList().Find((GiftModel x) => x.ClassInfo.id == idData.GetIntSelf()) != null)
                    {
                        __instance.GetEquippedList().Find((GiftModel x) => x.ClassInfo.id == idData.GetIntSelf()).isShowEquipGift = false;
                    }
                }
            }
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
                Singleton<ModContentManager>.Instance.AddErrorLog(ex.Message + Environment.NewLine + ex.StackTrace);
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
