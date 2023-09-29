using HarmonyLib;
using System;
using System.IO;
using UnityEngine;
using Workshop;
using LorIdExtensions;

namespace ExtendedLoader
{
	public static class SkinExtensions
	{
		public class ModSkinInfo : BattleUnitView.SkinInfo
		{
			public string packageId;
			public LorName lorName
			{
				get
				{
					return new LorName(packageId, skinName);
				}
				set
				{
					if (value is null)
					{
						packageId = "";
						skinName = "";
					}
					else
					{
						packageId = value.packageId;
						skinName = value.name;
					}
				}
			}

			public BattleUnitView.SkinInfo Compress()
			{
				return new BattleUnitView.SkinInfo()
				{
					state = state,
					skinName = lorName.Compress()
				};
			}
		}

		public static void ChangeSkin(this BattleUnitView view, LorName charName)
		{
			view.ChangeSkin(charName.Compress());
		}
		public static void ChangeEgoSkin(this BattleUnitView view, LorName egoName, bool bookNameChange = true)
		{
			view.ChangeEgoSkin(egoName.Compress(), bookNameChange);
		}
		public static void ChangeCreatureSkin(this BattleUnitView view, LorName creatureName)
		{
			view.ChangeCreatureSkin(creatureName.Compress());
		}
		public static void ChangeSkinByModSkinInfo(this BattleUnitView view, ModSkinInfo info, bool forceChange = false)
		{
			if (info == null)
			{
				return;
			}
			switch (info.state)
			{
				case BattleUnitView.SkinState.Default:
					if (forceChange)
					{
						view.ChangeSkin(info.lorName);
					}
					else
					{
						view.CreateSkin();
					}
					return;
				case BattleUnitView.SkinState.Changed:
					view.ChangeSkin(info.lorName);
					return;
				case BattleUnitView.SkinState.Creature:
					view.ChangeCreatureSkin(info.lorName);
					return;
				case BattleUnitView.SkinState.EGO:
					view.ChangeEgoSkin(info.lorName, true);
					return;
			}
		}
		public static ModSkinInfo GetCurrentModSkinInfo(this BattleUnitView view)
		{
			var skinInfo = view.GetCurrentSkinInfo();
			if (skinInfo.state == BattleUnitView.SkinState.Default)
			{
				var bookName = view.model.GetOriginalSkin();
				return new ModSkinInfo()
				{
					state = BattleUnitView.SkinState.Default,
					lorName = bookName
				};
			}
			else
			{
				var modSkinInfo = new ModSkinInfo() { state = skinInfo.state };
				var unitdata = view.model.UnitData.unitData;
				var skinName = skinInfo.skinName;
				var bookId = unitdata.bookItem.ClassInfo.workshopID;
				if (!string.IsNullOrWhiteSpace(bookId) && SkinTools.TryGetBookSkinData(bookId, skinName, out _))
				{
					modSkinInfo.lorName = new LorName(bookId, skinName);
				}
				else
				{
					var customBookId = unitdata.CustomBookItem.ClassInfo.workshopID;
					if (customBookId != bookId && !string.IsNullOrWhiteSpace(customBookId) &&
						SkinTools.TryGetBookSkinData(customBookId, skinName, out _))
					{
						modSkinInfo.lorName = new LorName(customBookId, skinName);
					}
					else
					{
						if (CustomizingResourceLoader.Instance.IsExistWorkshopSkinData(skinName))
						{
							modSkinInfo.lorName = new LorName(skinName, true);
						}
						else
						{
							modSkinInfo.lorName = new LorName(skinName);
						}
					}
				}
				return modSkinInfo;
			}
		}
		public static void ChangeSkinWorkshop(this BattleUnitView view, string skinName)
		{
			view.ChangeSkin(new LorName(skinName, true));
		}
		public static void ChangeEgoSkinWorkshop(this BattleUnitView view, string egoName, bool bookNameChange = true)
		{
			view.ChangeEgoSkin(new LorName(egoName, true), bookNameChange);
		}
		public static void ChangeCreatureSkinWorkshop(this BattleUnitView view, string creatureName)
		{
			view.ChangeCreatureSkin(new LorName(creatureName, true));
		}
		public static void SetAltSkin(this BattleUnitView view, LorName skinName)
		{
			view.SetAltSkin(skinName.Compress());
		}
		public static void SetAltSkinWorkshop(this BattleUnitView view, string skinName)
		{
			view.SetAltSkin(new LorName(skinName, true).Compress());
		}
		public static void ResetAltSkin(this BattleUnitView view)
		{
			view._altSkinInfo = null;
		}
		public static void ResetAnySkin(this BattleUnitView view)
		{
			view._altSkinInfo = null;
			view.ResetSkin();
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
					if (defaultData.sprite != null)
					{
						DirectoryInfo directoryInfo = new DirectoryInfo(defaultData.spritePath);
						string thumbPath = directoryInfo.Parent.Parent.FullName + "/Thumb.png";
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
		public static LorName GetOriginalSkin(this BattleUnitModel model)
		{
			string workshopSkin = model._unitData.unitData.workshopSkin;
			if (!string.IsNullOrWhiteSpace(workshopSkin))
			{
				return new LorName(workshopSkin, true);
			}
			string packageId = "";
			string originalName = model.customBook.GetOriginalCharcterName();

			if (model.customBook._classInfo.skinType == "Custom")
			{
				var bookId = model.customBook._classInfo.workshopID;
				if (SkinTools.GetWorkshopBookSkinData(bookId, originalName, "") != null)
				{
					packageId = bookId;
				}
			}
			return new LorName(packageId, originalName);
		}
		public static void PreloadSkinSprites(this WorkshopSkinData data)
		{
			SkinTools.PreloadSkinSprites(data);
		}
	}

	public static class SkinTools
	{
		public static WorkshopSkinData GetWorkshopBookSkinData(string id, string name, string gender)
		{
			WorkshopSkinData result = null;
			if (!LorName.IsWorkshopGenericId(id))
			{
				if (!TryGetBookSkinData(id, name + gender, out result))
				{
					if (gender == "_N" || !TryGetBookSkinData(id, name + "_N", out result))
					{
						if (!string.IsNullOrEmpty(gender))
						{
							TryGetBookSkinData(id, name, out result);
						}
					}
				}
			}
			if (result == null)
			{
				result = CustomizingResourceLoader.Instance.GetWorkshopSkinData(name);
			}
			return result;
		}
		public static WorkshopSkinData GetWorkshopBookSkinData(LorName name, string gender)
		{
			return GetWorkshopBookSkinData(name.packageId, name.name, gender);
		}
		internal static bool TryGetBookSkinData(string packageId, string name, out WorkshopSkinData skinData)
		{
			skinData = CustomizingBookSkinLoader.Instance.GetWorkshopBookSkinData(packageId, name);
			return skinData != null;
		}

		public static void PreloadSkinSprites(string id, string name)
		{
			PreloadSkinSprites(CustomizingBookSkinLoader.Instance.GetWorkshopBookSkinData(id, name));
		}
		public static void PreloadSkinSprites(string name)
		{
			PreloadSkinSprites(CustomizingResourceLoader.Instance.GetWorkshopSkinData(name));
		}
		public static void PreloadSkinSprites(WorkshopSkinData data)
		{
			try
			{
				if (data != null && data.dic != null)
				{
					foreach (var motion in data.dic.Values)
					{
						if (motion is ExtendedClothCustomizeData extendedMotion)
						{
							extendedMotion.LoadAllSprites();
							continue;
						}
						else if (motion.GetType() != typeof(ClothCustomizeData))
						{
							try
							{
								var method = Traverse.Create(motion).Method("LoadAllSprites", Array.Empty<object>());
								if (method.MethodExists())
								{
									method.GetValue();
									continue;
								}
							}
							catch (Exception ex)
							{
								Debug.LogException(ex);
							}
						}
						_ = motion.sprite;
						_ = motion.frontSprite;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
	}
}