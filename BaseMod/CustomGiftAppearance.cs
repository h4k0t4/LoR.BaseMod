using HarmonyLib;
using Mod;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace BaseMod
{
    public class CustomGiftAppearance : GiftAppearance
    {
        public static GiftAppearance CreateCustomGift(string[] array2)
        {
            GiftAppearance result;
            if (CreatedGifts.ContainsKey(array2[2]))
            {
                result = CreatedGifts[array2[2]];
            }
            else
            {
                GameObject original = Resources.Load<GameObject>("Prefabs/Gifts/Gifts_NeedRename/Gift_Challenger");
                GiftAppearance component = Instantiate(original).GetComponent<GiftAppearance>();
                SpriteRenderer spriteRenderer = component._frontSpriteRenderer;
                SpriteRenderer spriteRenderer2 = component._sideSpriteRenderer;
                SpriteRenderer spriteRenderer3 = component._frontBackSpriteRenderer;
                SpriteRenderer spriteRenderer4 = component._sideBackSpriteRenderer;
                spriteRenderer.gameObject.transform.localScale = new Vector2(1f, 1f);
                if (GiftArtWork.ContainsKey(array2[2] + "_front"))
                {
                    spriteRenderer.sprite = GiftArtWork[array2[2] + "_front"];
                }
                else
                {
                    spriteRenderer.gameObject.SetActive(false);
                    component.GetType().GetField("_frontSpriteRenderer", AccessTools.all).SetValue(component, null);
                }
                spriteRenderer2.gameObject.transform.localScale = new Vector2(1f, 1f);
                if (GiftArtWork.ContainsKey(array2[2] + "_side"))
                {
                    spriteRenderer2.sprite = GiftArtWork[array2[2] + "_side"];
                }
                else
                {
                    spriteRenderer2.gameObject.SetActive(false);
                    component.GetType().GetField("_sideSpriteRenderer", AccessTools.all).SetValue(component, null);
                }
                spriteRenderer3.gameObject.transform.localScale = new Vector2(1f, 1f);
                if (GiftArtWork.ContainsKey(array2[2] + "_frontBack"))
                {
                    spriteRenderer3.sprite = GiftArtWork[array2[2] + "_frontBack"];
                }
                else
                {
                    spriteRenderer3.gameObject.SetActive(false);
                    component.GetType().GetField("_frontBackSpriteRenderer", AccessTools.all).SetValue(component, null);
                }
                spriteRenderer4.gameObject.transform.localScale = new Vector2(1f, 1f);
                if (GiftArtWork.ContainsKey(array2[2] + "_sideBack"))
                {
                    spriteRenderer4.sprite = GiftArtWork[array2[2] + "_sideBack"];
                }
                else
                {
                    spriteRenderer4.gameObject.SetActive(false);
                    component.GetType().GetField("_sideBackSpriteRenderer", AccessTools.all).SetValue(component, null);
                }
                CreatedGifts[array2[2]] = component;
                result = component;
            }
            return result;
        }
        public static void GetGiftArtWork()
        {
            GiftArtWork = new Dictionary<string, Sprite>();
            foreach (ModContent modContent in Harmony_Patch.LoadedModContents)
            {
                DirectoryInfo _dirInfo = modContent.GetType().GetField("_dirInfo", AccessTools.all).GetValue(modContent) as DirectoryInfo;
                if (Directory.Exists(_dirInfo.FullName + "/GiftArtWork"))
                {
                    DirectoryInfo directoryInfo2 = new DirectoryInfo(_dirInfo.FullName + "/GiftArtWork");
                    foreach (FileInfo fileInfo in directoryInfo2.GetFiles())
                    {
                        Texture2D texture2D = new Texture2D(2, 2);
                        texture2D.LoadImage(File.ReadAllBytes(fileInfo.FullName));
                        Sprite value = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                        GiftArtWork[fileNameWithoutExtension] = value;
                    }
                }
            }
        }
        public static Sprite GetGiftArtWork(string name)
        {
            bool flag = GiftArtWork.ContainsKey(name);
            Sprite result;
            if (flag)
            {
                result = GiftArtWork[name];
            }
            else
            {
                result = null;
            }
            return result;
        }
        public new void Awake()
        {
            bool flag = inited;
            if (!flag)
            {
                bool flag2 = _frontSpriteRenderer == null;
                if (flag2)
                {
                    GameObject gameObject = new GameObject("new");
                    gameObject.transform.SetParent(base.gameObject.transform);
                    gameObject.transform.localPosition = new Vector2(0f, 0f);
                    gameObject.transform.localScale = new Vector2(1f, 1f);
                    _frontSpriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                }
                bool flag3 = _frontBackSpriteRenderer == null;
                if (flag3)
                {
                    GameObject gameObject2 = new GameObject("new");
                    gameObject2.transform.SetParent(gameObject.transform);
                    gameObject2.transform.localPosition = new Vector2(0f, 0f);
                    gameObject2.transform.localScale = new Vector2(1f, 1f);
                    _frontBackSpriteRenderer = gameObject2.AddComponent<SpriteRenderer>();
                }
                bool flag4 = _sideSpriteRenderer == null;
                if (flag4)
                {
                    GameObject gameObject3 = new GameObject("new");
                    gameObject3.transform.SetParent(gameObject.transform);
                    gameObject3.transform.localPosition = new Vector2(0f, 0f);
                    gameObject3.transform.localScale = new Vector2(1f, 1f);
                    _sideSpriteRenderer = gameObject3.AddComponent<SpriteRenderer>();
                }
                bool flag5 = _sideBackSpriteRenderer == null;
                if (flag5)
                {
                    GameObject gameObject4 = new GameObject("new");
                    gameObject4.transform.SetParent(gameObject.transform);
                    gameObject4.transform.localPosition = new Vector2(0f, 0f);
                    gameObject4.transform.localScale = new Vector2(1f, 1f);
                    _sideBackSpriteRenderer = gameObject4.AddComponent<SpriteRenderer>();
                }
            }
            SortingGroup component = base.GetComponent<SortingGroup>();
            if (component != null)
            {
                UnityEngine.Object.Destroy(component);
            }
        }
        public void CustomInit(string name)
        {
            File.WriteAllText(Application.dataPath + "/Mods/" + name, "");
            Sprite giftArtWork = GetGiftArtWork(name + "_front");
            Sprite giftArtWork2 = GetGiftArtWork(name + "_frontBack");
            Sprite giftArtWork3 = GetGiftArtWork(name + "_side");
            Sprite giftArtWork4 = GetGiftArtWork(name + "_sideBack");
            if (giftArtWork != null)
            {
                _frontSpriteRenderer.sprite = giftArtWork;
            }
            else
            {
                _frontSpriteRenderer = null;
            }
            if (giftArtWork2 != null)
            {
                _frontSpriteRenderer.sprite = giftArtWork2;
            }
            else
            {
                _frontBackSpriteRenderer = null;
            }
            if (giftArtWork3 != null)
            {
                _sideSpriteRenderer.sprite = giftArtWork3;
            }
            else
            {
                _sideSpriteRenderer = null;
            }
            if (giftArtWork4 != null)
            {
                _sideBackSpriteRenderer.sprite = giftArtWork4;
            }
            else
            {
                _sideBackSpriteRenderer = null;
            }
        }
        public CustomGiftAppearance()
        {
            inited = false;
        }
        static CustomGiftAppearance()
        {
            CreatedGifts = new Dictionary<string, GiftAppearance>();
        }

        public static Dictionary<string, GiftAppearance> CreatedGifts;

        public static Dictionary<string, Sprite> GiftArtWork;

        public bool inited;
    }
}
