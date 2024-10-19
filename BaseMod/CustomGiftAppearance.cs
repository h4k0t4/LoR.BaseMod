using HarmonyLib;
using Mod;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using ExtendedLoader;

namespace BaseMod
{
	public class CustomGiftAppearance : GiftAppearance
	{
		public static GiftAppearance CreateCustomGift(string giftName)
		{
			return CreateCustomGift(giftName.Split('_'));
		}
		public static GiftAppearance CreateCustomGift(string[] array2)
		{
			if (array2.Length < 3 || array2[1].ToLower() != "custom")
			{
				return null;
			}
			GiftAppearance result;
			if (CreatedGifts.TryGetValue(array2[2], out var gift))
			{
				result = gift;
			}
			else
			{
				GiftAppearance original = Harmony_Patch.CustomGiftAppearancePrefabObject;
				GiftAppearance giftAppearance = Instantiate(original, XLRoot.persistentRoot.transform).GetComponent<GiftAppearance>();

				SetGiftArtWork(giftAppearance, array2[2]);

				CreatedGifts[array2[2]] = giftAppearance;
				result = giftAppearance;
			}
			return result;
		}
		public static void GetGiftArtWork(bool forcedly = false)
		{
			if (GiftArtWorkLoaded && !forcedly)
			{
				return;
			}
			GiftArtWork = new Dictionary<string, Sprite>();
			foreach (ModContent modContent in Harmony_Patch.LoadedModContents)
			{
				string giftPath = Path.Combine(modContent._dirInfo.FullName, "GiftArtWork");
				if (Directory.Exists(giftPath))
				{
					DirectoryInfo directory = new DirectoryInfo(giftPath);
					foreach (FileInfo fileInfo in directory.GetFiles())
					{
						Texture2D texture2D = new Texture2D(2, 2);
						texture2D.LoadImage(File.ReadAllBytes(fileInfo.FullName));
						Sprite value = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
						string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
						GiftArtWork[fileNameWithoutExtension] = value;
					}
				}
			}
			GiftArtWorkLoaded = true;
		}
		public static void GetGiftArtWork()
		{
			GetGiftArtWork(false);
		}
		public static Sprite GetGiftArtWork(string name)
		{
			GetGiftArtWork();
			return GiftArtWork.GetValueSafe(name);
		}
		public new void Awake()
		{
			if (!inited)
			{
				if (_frontSpriteRenderer == null)
				{
					GameObject gameObject = new GameObject("new");
					gameObject.transform.SetParent(base.gameObject.transform);
					gameObject.transform.localPosition = new Vector2(0f, 0f);
					gameObject.transform.localScale = new Vector2(1f, 1f);
					_frontSpriteRenderer = gameObject.AddComponent<SpriteRenderer>();
				}
				if (_frontBackSpriteRenderer == null)
				{
					GameObject gameObject2 = new GameObject("new");
					gameObject2.transform.SetParent(gameObject.transform);
					gameObject2.transform.localPosition = new Vector2(0f, 0f);
					gameObject2.transform.localScale = new Vector2(1f, 1f);
					_frontBackSpriteRenderer = gameObject2.AddComponent<SpriteRenderer>();
				}
				if (_sideSpriteRenderer == null)
				{
					GameObject gameObject3 = new GameObject("new");
					gameObject3.transform.SetParent(gameObject.transform);
					gameObject3.transform.localPosition = new Vector2(0f, 0f);
					gameObject3.transform.localScale = new Vector2(1f, 1f);
					_sideSpriteRenderer = gameObject3.AddComponent<SpriteRenderer>();
				}
				if (_sideBackSpriteRenderer == null)
				{
					GameObject gameObject4 = new GameObject("new");
					gameObject4.transform.SetParent(gameObject.transform);
					gameObject4.transform.localPosition = new Vector2(0f, 0f);
					gameObject4.transform.localScale = new Vector2(1f, 1f);
					_sideBackSpriteRenderer = gameObject4.AddComponent<SpriteRenderer>();
				}
				inited = true;
			}
			SortingGroup component = GetComponent<SortingGroup>();
			if (component != null)
			{
				Destroy(component);
			}
		}
		public void CustomInit(string name)
		{
			SetGiftArtWork(this, name);
		}

		internal static void SetGiftArtWork(GiftAppearance giftAppearance, string name)
		{
			GetGiftArtWork();

			SetGiftRendererSprite(giftAppearance._frontSpriteRenderer, name + "_front");
			SetGiftRendererSprite(giftAppearance._sideSpriteRenderer, name + "_side");
			SetGiftRendererSprite(giftAppearance._frontBackSpriteRenderer, name + "_frontBack");
			SetGiftRendererSprite(giftAppearance._sideBackSpriteRenderer, name + "_sideBack");
		}

		static void SetGiftRendererSprite(SpriteRenderer renderer, string name)
		{
			if (!renderer)
			{
				return;
			}
			if (GiftArtWork.ContainsKey(name))
			{
				renderer.enabled = true;
				renderer.sprite = GiftArtWork[name];
			}
			else
			{
				renderer.enabled = false;
				renderer.sprite = null;
			}
		}

		public static Dictionary<string, GiftAppearance> CreatedGifts = new Dictionary<string, GiftAppearance>();

		public static Dictionary<string, Sprite> GiftArtWork;

		public bool inited = false;

		public static bool GiftArtWorkLoaded;
	}
}
