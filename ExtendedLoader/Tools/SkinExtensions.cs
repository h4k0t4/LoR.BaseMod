using HarmonyLib;
using System;
using System.IO;
using UnityEngine;
using Workshop;
using LorIdExtensions;

namespace ExtendedLoader
{
	/// <summary>
	/// The class providing extension methods for handling skins.
	/// </summary>
	public static class SkinExtensions
	{
		/// <summary>
		/// A class for handling skin information with mod specification attached.
		/// Mostly intended to be used for transferring skins between units or over time.
		/// </summary>
		public class ModSkinInfo : BattleUnitView.SkinInfo
		{
			/// <summary>
			/// The mod id the skin belongs to (empty if the skin is not external).
			/// </summary>
			public string packageId;

			/// <summary>
			/// The full identifier of the skin.
			/// Can be used to change to the skin using ChangeSkin.
			/// </summary>
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

			/// <summary>
			/// "Compresses" the skin LorName to fit into a regular SkinInfo (see <see cref="LorName.Compress"/>).
			/// Note that if the state is Default, changing other units' skins by compressed info will not work!
			/// </summary>
			/// <returns></returns>
			public BattleUnitView.SkinInfo Compress()
			{
				return new BattleUnitView.SkinInfo()
				{
					state = state,
					skinName = lorName.Compress()
				};
			}
		}

		/// <summary>
		/// A LorName equivalent of <see cref="BattleUnitView.ChangeSkin(string)"/>.
		/// </summary>
		public static void ChangeSkin(this BattleUnitView view, LorName charName)
		{
			view.ChangeSkin(charName.Compress());
		}
		/// <summary>
		/// A LorName equivalent of <see cref="BattleUnitView.ChangeEgoSkin(string, bool)"/>.
		/// </summary>
		public static void ChangeEgoSkin(this BattleUnitView view, LorName egoName, bool bookNameChange = true)
		{
			view.ChangeEgoSkin(egoName.Compress(), bookNameChange);
		}
		/// <summary>
		/// A LorName equivalent of <see cref="BattleUnitView.ChangeCreatureSkin(string)"/>.
		/// </summary>
		public static void ChangeCreatureSkin(this BattleUnitView view, LorName creatureName)
		{
			view.ChangeCreatureSkin(creatureName.Compress());
		}
		/// <summary>
		/// Mostly a ModSkinInfo equivalent of <see cref="BattleUnitView.ChangeSkinBySkinInfo(BattleUnitView.SkinInfo)"/>.
		/// The additional parameter <paramref name="forceChange"/> is used to force the change to the given skin if state is <see cref="BattleUnitView.SkinState.Default"/>, instead of resetting to the unit's own default skin (useful for copying skins between different units).
		/// </summary>
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
		/// <summary>
		/// A ModSkinInfo equivalent of <see cref="BattleUnitView.GetCurrentSkinInfo"/>.
		/// </summary>
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
		/// <summary>
		/// An equivalent of <see cref="BattleUnitView.ChangeSkin(string)"/>
		/// </summary>
		public static void ChangeSkinWorkshop(this BattleUnitView view, string skinName)
		{
			view.ChangeSkin(new LorName(skinName, true));
		}
		/// <summary>
		/// An equivalent of <see cref="BattleUnitView.ChangeEgoSkin(string, bool)"/> for standalone external skins.
		/// </summary>
		public static void ChangeEgoSkinWorkshop(this BattleUnitView view, string egoName, bool bookNameChange = true)
		{
			view.ChangeEgoSkin(new LorName(egoName, true), bookNameChange);
		}
		/// <summary>
		/// An equivalent of <see cref="BattleUnitView.ChangeCreatureSkin(string)"/> for standalone external skins.
		/// </summary>
		public static void ChangeCreatureSkinWorkshop(this BattleUnitView view, string creatureName)
		{
			view.ChangeCreatureSkin(new LorName(creatureName, true));
		}
		/// <summary>
		/// A LorName equivalent of <see cref="BattleUnitView.SetAltSkin(string)"/>.
		/// </summary>
		public static void SetAltSkin(this BattleUnitView view, LorName skinName)
		{
			view.SetAltSkin(skinName.Compress());
		}
		/// <summary>
		/// An equivalent of <see cref="BattleUnitView.SetAltSkin(string)"/> for standalone external skins.
		/// </summary>
		public static void SetAltSkinWorkshop(this BattleUnitView view, string skinName)
		{
			view.SetAltSkin(new LorName(skinName, true).Compress());
		}
		/// <summary>
		/// Discards the currently set AltSkin (note that this WILL NOT cause a skin change by itself).
		/// </summary>
		public static void ResetAltSkin(this BattleUnitView view)
		{
			view._altSkinInfo = null;
		}
		/// <summary>
		/// Fully resets the current view skin, also discarding AltSkin.
		/// </summary>
		public static void ResetAnySkin(this BattleUnitView view)
		{
			view._altSkinInfo = null;
			view.ResetSkin();
		}
		/// <summary>
		/// Tries to get a thumbnail sprite for external skin data.
		/// </summary>
		/// <returns>A thumbnail sprite, or <see langword="null"/> if retrieval/generation did not succeed.</returns>
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

		/// <summary>
		/// Tries to get a conclusive identifier of a unit's "original" skin (that is, the skin that the unit would start combat with).
		/// This identifier can then be used to make other units "copy" the skin using ChangeSkin.
		/// </summary>
		/// <returns>The skin identifier.</returns>
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

		/// <summary>
		/// Tries to preload all the sprites in an external skin.
		/// </summary>
		public static void PreloadSkinSprites(this WorkshopSkinData data)
		{
			SkinTools.PreloadSkinSprites(data);
		}
	}

	/// <summary>
	/// The class providing tools for handling skins (that are not extension methods).
	/// </summary>
	public static class SkinTools
	{
		/// <summary>
		/// Tries to find external skin data by mod id and skin name, specifying gender if possible.
		/// Also tries to check standalone external skins with the given name if the given mod id does not contain the given skin name.
		/// </summary>
		/// <param name="id">The mod id.</param>
		/// <param name="name">The skin name.</param>
		/// <param name="gender">Gender specifier (will be added to the skin name if possible).</param>
		/// <returns>The found skin data, or <see langword="null"/> if nothing was found.</returns>
		public static WorkshopSkinData GetWorkshopBookSkinData(string id, string name, string gender = "")
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
		/// <summary>
		/// Tries to find external skin data by mod id and skin name, specifying gender if possible.
		/// Also tries to check standalone external skins with the given name if the given mod id does not contain the given skin name.
		/// </summary>
		/// <param name="name">The full skin name, including mod id and internal name.</param>
		/// <param name="gender">Gender specifier (will be added to the skin name if possible).</param>
		/// <returns>The found skin data, or <see langword="null"/> if nothing was found.</returns>
		public static WorkshopSkinData GetWorkshopBookSkinData(LorName name, string gender)
		{
			return GetWorkshopBookSkinData(name.packageId, name.name, gender);
		}
		internal static bool TryGetBookSkinData(string packageId, string name, out WorkshopSkinData skinData)
		{
			skinData = CustomizingBookSkinLoader.Instance.GetWorkshopBookSkinData(packageId, name);
			return skinData != null;
		}

		/// <summary>
		/// Tries to preload all the sprites in a given external skin from a given mod.
		/// </summary>
		/// <param name="id">The mod id.</param>
		/// <param name="name">The mod-internal skin name.</param>
		public static void PreloadSkinSprites(string id, string name)
		{
			PreloadSkinSprites(CustomizingBookSkinLoader.Instance.GetWorkshopBookSkinData(id, name));
		}
		/// <summary>
		/// Tries to preload all the sprites in a given standalone external skin (that is, a skin that is just ModInfo and not part of a StageModInfo mod).
		/// </summary>
		/// <param name="name">The skin name.</param>
		public static void PreloadSkinSprites(string name)
		{
			PreloadSkinSprites(CustomizingResourceLoader.Instance.GetWorkshopSkinData(name));
		}
		/// <summary>
		/// Tries to preload all the sprites in an external skin given its data.
		/// </summary>
		/// <param name="data">The skin data.</param>
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