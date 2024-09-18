using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using UI;
using UnityEngine;
using Workshop;
using Mod;

namespace ExtendedLoader
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	public class XLRoot : SingletonBehavior<XLRoot>
	{
		public const int THUMB_LAYER = 23;
		public const int THUMB_MASK = 1 << THUMB_LAYER;
		static GameObject _persistentRoot;
		public static GameObject persistentRoot
		{
			get
			{
				if (_persistentRoot == null)
				{
					_persistentRoot = CreatePersistentRoot();
				}
				return _persistentRoot;
			}
		}
		static GameObject _UICustomAppearancePrefab = null;
		public static GameObject UICustomAppearancePrefab
		{
			get
			{
				if (_UICustomAppearancePrefab == null)
				{
					_UICustomAppearancePrefab = CreateUIAppearancePrefab();
				}
				return _UICustomAppearancePrefab;
			}
		}
		static GameObject _CustomAppearancePrefab = null;
		public static GameObject CustomAppearancePrefab
		{
			get
			{
				if (_CustomAppearancePrefab == null)
				{
					_CustomAppearancePrefab = CreateCustomAppearancePrefab();
				}
				return _CustomAppearancePrefab;
			}
		}
		public static readonly Dictionary<int, Sprite> SkinThumb = new Dictionary<int, Sprite>();
		static readonly SkinPartRenderer thumbRenderers = new SkinPartRenderer();
		static CustomizedAppearance thumbHead;
		static Camera thumbCamera = null;
		static readonly Queue<ClothCustomizeData> thumbQueue = new Queue<ClothCustomizeData>();
		public static Camera ThumbCamera
		{
			get
			{
				if (thumbCamera == null)
				{
					UICharacterRenderer uiRenderer = UICharacterRenderer.Instance;
					if (uiRenderer != null)
					{
						Camera camera = Instantiate(uiRenderer.cameraList[0], persistentRoot.transform);
						camera.name = "ThumbCamera";
						camera.targetTexture = Instantiate(uiRenderer.cameraList[0].targetTexture);
						DontDestroyOnLoad(camera.targetTexture);
						camera.targetTexture.name = "ThumbTargetTexture";
						camera.transform.position = new Vector3(-2000f, 0f, 0f);
						camera.cullingMask = THUMB_MASK;
						thumbCamera = camera;

						SpriteRenderer original = UICustomAppearancePrefab.GetComponent<CharacterAppearance>()._motionList[0].motionSpriteSet[0].sprRenderer;
						thumbRenderers.effect = Instantiate(original, camera.transform);
						original = thumbRenderers.effect;
						original.name = "ThumbRenderer_Effect";
						original.gameObject.layer = THUMB_LAYER;
						original.transform.localPosition = new Vector3(0f, -2f, 10f);
						original.gameObject.SetActive(false);
						original.sortingOrder = 4;

						thumbRenderers.front = Instantiate(original, camera.transform);
						thumbRenderers.front.name = "ThumbRenderer_Front";
						thumbRenderers.front.sortingOrder = 3;

						var headRoot = new GameObject("ThumbRenderer_Head");
						headRoot.transform.parent = camera.transform;
						headRoot.transform.localPosition = new Vector3(0f, -2f, 10f);
						headRoot.transform.localEulerAngles = Vector3.zero;
						headRoot.transform.localScale = Vector3.one;
						thumbHead = Instantiate(CustomizingResourceLoader.Instance.customAppearancePrefab, headRoot.transform);
						for (int i = 0; i < 2; i++)
						{
							thumbHead.head[i].sprite = CustomizingResourceLoader.Instance.GetHeadSprite(i);
							thumbHead.head[i].sortingOrder = 2;
						}
						thumbHead.ChangeLayer(LayerMask.LayerToName(THUMB_LAYER));
						for (int i = 0; i < thumbHead.face.Length; i++)
						{
							thumbHead.face[i].gameObject.SetActive(false);
						}
						for (int i = 0; i < thumbHead.brow.Length; i++)
						{
							thumbHead.brow[i].gameObject.SetActive(false);
						}
						for (int i = 0; i < thumbHead.mouth.Length; i++)
						{
							thumbHead.mouth[i].gameObject.SetActive(false);
						}
						for (int i = 0; i < thumbHead.frontHair.Length; i++)
						{
							thumbHead.frontHair[i].gameObject.SetActive(false);
						}
						for (int i = 0; i < thumbHead.backHair.Length; i++)
						{
							thumbHead.backHair[i].gameObject.SetActive(false);
						}
						thumbHead.gameObject.SetActive(false);

						thumbRenderers.rear = Instantiate(original, camera.transform);
						thumbRenderers.rear.name = "ThumbRenderer_Mid";
						thumbRenderers.rear.sortingOrder = 1;

						thumbRenderers.rearest = Instantiate(original, camera.transform);
						thumbRenderers.rearest.name = "ThumbRenderer_Back";
						thumbRenderers.rearest.sortingOrder = 0;
					}
				}
				return thumbCamera;
			}
		}

		public static Sprite MakeThumbnail(ClothCustomizeData data)
		{
			if (data != null && (data.GetType() == typeof(ClothCustomizeData) || data is ExtendedClothCustomizeData))
			{
				thumbQueue.Enqueue(data);
			}
			return null;
		}

		public static IEnumerator MakeThumbnailCoroutine()
		{
			while (true)
			{
				yield return YieldCache.waitFrame;
				if (ThumbCamera == null) continue;
				if (thumbQueue.Count == 0) continue;
				ClothCustomizeData nextInQueue = thumbQueue.Dequeue();
				thumbRenderers.front.sprite = nextInQueue.frontSprite;
				thumbRenderers.front.gameObject.SetActive(nextInQueue.frontSprite);
				thumbRenderers.rear.sprite = nextInQueue.sprite;
				thumbRenderers.rear.gameObject.SetActive(nextInQueue.sprite);
				if (nextInQueue is ExtendedClothCustomizeData extend)
				{
					thumbRenderers.effect.sprite = extend.effectSprite;
					thumbRenderers.effect.gameObject.SetActive(extend.effectSprite);
					thumbRenderers.rearest.sprite = extend.backSprite;
					thumbRenderers.rearest.gameObject.SetActive(extend.backSprite);
				}
				else
				{
					thumbRenderers.effect.sprite = null;
					thumbRenderers.effect.gameObject.SetActive(false);
					thumbRenderers.rearest.sprite = null;
					thumbRenderers.rearest.gameObject.SetActive(false);
				}
				if (nextInQueue.headEnabled)
				{
					thumbHead.gameObject.SetActive(true);
					if (nextInQueue is ExtendedClothCustomizeData extendedData)
					{
						thumbHead.transform.localPosition = extendedData.headPivot.localPosition;
						thumbHead.transform.localEulerAngles = extendedData.headPivot.localEulerAngles;
						thumbHead.transform.localScale = extendedData.headPivot.localScale;
					}
					else
					{
						thumbHead.transform.localPosition = nextInQueue.headPos;
						thumbHead.transform.localRotation = Quaternion.Euler(0f, 0f, nextInQueue.headRotation);
						thumbHead.transform.localScale = Vector3.one;
					}
					var isFront = nextInQueue.direction == CharacterMotion.MotionDirection.FrontView;
					thumbHead.head[0].gameObject.SetActive(isFront);
					thumbHead.head[1].gameObject.SetActive(!isFront);
				}
				else
				{
					thumbHead.gameObject.SetActive(false);
				}
				yield return YieldCache.waitFrame;
				RenderTexture rt = thumbCamera.targetTexture;
				RenderTexture backup = RenderTexture.active;
				rt.Release();
				thumbCamera.Render();
				RenderTexture.active = rt;
				Texture2D tex = new Texture2D(256, 512);
				tex.ReadPixels(new Rect(0, 0, 256, 512), 0, 0);
				/*
				thumbRenderers.effect.gameObject.SetActive(false);
				thumbRenderers.front.gameObject.SetActive(false);
				thumbHead.gameObject.SetActive(false);
				thumbRenderers.rear.gameObject.SetActive(false);
				thumbRenderers.rearest.gameObject.SetActive(false);
				*/
				RenderTexture.active = backup;
				byte[] bytes = tex.EncodeToPNG();
				string text = new DirectoryInfo(nextInQueue.spritePath).Parent.Parent.FullName + "/Thumb.png";
				File.WriteAllBytes(text, bytes);
				Debug.Log("Saved baked thumbnail to " + text);
			}
		}

		public static Dictionary<string, int> CoreThumbDic = new Dictionary<string, int>();
		public static Dictionary<LorId, Sprite> BookThumb = new Dictionary<LorId, Sprite>();

		static GameObject CreatePersistentRoot()
		{
			GameObject root = new GameObject("ExtendedLoader_PersistentRoot");
			DontDestroyOnLoad(root);
			root.AddComponent<XLRoot>();
			return root;
		}

		internal static void EnsureInit()
		{
			_ = UICustomAppearancePrefab;
			_ = CustomAppearancePrefab;
		}

		void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
				return;
			}
			else
			{
				StartCoroutine(MakeThumbnailCoroutine());
			}
		}

		static GameObject CreateUIAppearancePrefab()
		{
			GameObject proto = (GameObject)Resources.Load("Prefabs/Characters/[Prefab]Appearance_Custom");
			GameObject gameObject = Instantiate(proto, persistentRoot.transform);
			CharacterAppearance appearance = gameObject.GetComponent<CharacterAppearance>();
			foreach (CharacterMotion motion in appearance._motionList.ToList())
			{
				if (motion.actionDetail != ActionDetail.Default && motion.actionDetail != ActionDetail.Standing)
				{
					appearance._motionList.Remove(motion);
					Destroy(motion.gameObject);
				}
				else
				{
					foreach (object obj in motion.transform)
					{
						Transform transform = (Transform)obj;
						if (transform.gameObject.name == "Customize_Renderer")
						{
							AddSkinRenderers(motion, transform);
						}
					}
					motion.transform.name = $"Custom_Extended_{motion.actionDetail}";
				}
			}
			Destroy(gameObject.GetComponent<WorkshopSkinDataSetter>());
			gameObject.AddComponent<UIWorkshopSkinDataSetter>();
			gameObject.name = "Appearance_Custom_Extended_UI";
			return gameObject;
		}

		static GameObject CreateCustomAppearancePrefab()
		{
			GameObject proto = (GameObject)Resources.Load("Prefabs/Characters/[Prefab]Appearance_Custom");
			GameObject gameObject = Instantiate(proto, persistentRoot.transform);
			CharacterAppearance appearance = gameObject.GetComponent<CharacterAppearance>();
			for (int i = 0; i < 12; i++)
			{
				if (i == 9)
				{
					i++;
				}
				ActionDetail actionDetail = (ActionDetail)i;
				CharacterMotion oldMotion = appearance._motionList.Find((CharacterMotion motion) => motion.actionDetail == actionDetail);
				foreach (object obj in oldMotion.transform)
				{
					Transform transform = (Transform)obj;
					if (transform.gameObject.name == "Customize_Renderer")
					{
						AddSkinRenderers(oldMotion, transform);
					}
				}
				oldMotion.transform.name = $"Custom_Extended_{actionDetail}";
			}
			CharacterMotion masterMotion = appearance._motionList[0];
			Transform masterTransform = masterMotion.transform.parent;
			ActionDetail action = (ActionDetail)9;
			CharacterMotion addedMotion = Instantiate(masterMotion, masterTransform);
			addedMotion.transform.position = masterMotion.transform.position;
			addedMotion.transform.localPosition = masterMotion.transform.localPosition;
			addedMotion.transform.localScale = masterMotion.transform.localScale;
			addedMotion.actionDetail = action;
			addedMotion.transform.name = $"Custom_Extended_{action}";
			appearance._motionList.Add(addedMotion);
			for (int i = 12; i < 31; i++)
			{
				action = (ActionDetail)i;
				addedMotion = Instantiate(masterMotion, masterTransform);
				addedMotion.transform.position = masterMotion.transform.position;
				addedMotion.transform.localPosition = masterMotion.transform.localPosition;
				addedMotion.transform.localScale = masterMotion.transform.localScale;
				addedMotion.actionDetail = action;
				addedMotion.transform.name = $"Custom_Extended_{action}";
				appearance._motionList.Add(addedMotion);
			}
			gameObject.name = "Appearance_Custom_Extended";
			return gameObject;
		}

		static void AddSkinRenderers(CharacterMotion motion, Transform transform)
		{
			motion.motionSpriteSet.Clear();
			SpriteRenderer midRenderer = motion.transform.Find("Customize_Renderer").gameObject.GetComponent<SpriteRenderer>();
			midRenderer.sortingOrder = 4;
			motion.motionSpriteSet.Add(new SpriteSet(midRenderer, CharacterAppearanceType.Body));
			motion.motionSpriteSet.Add(new SpriteSet(motion.transform.Find("CustomizePivot/DummyHead").gameObject.GetComponent<SpriteRenderer>(), CharacterAppearanceType.Head));
			motion.motionSpriteSet.Add(new SpriteSet(motion.transform.Find("Customize_Renderer_Front").gameObject.GetComponent<SpriteRenderer>(), CharacterAppearanceType.Body));
			SpriteRenderer baseRenderer = transform.GetComponent<SpriteRenderer>();

			void CreateSkinRenderer(string name, CharacterAppearanceType type, int sortingOrder)
			{
				SpriteRenderer newRenderer = Instantiate(baseRenderer, transform.parent);
				newRenderer.gameObject.name = name;
				newRenderer.sortingOrder = sortingOrder;
				newRenderer.gameObject.SetActive(false);
				motion.motionSpriteSet.Add(new SpriteSet(newRenderer, type));
			}

			CreateSkinRenderer("Customize_Renderer_Back", CharacterAppearanceType.Body, -5);
			CreateSkinRenderer("Customize_Renderer_Back_Skin", CharacterAppearanceType.Skin, -4);
			CreateSkinRenderer("Customize_Renderer_Skin", CharacterAppearanceType.Skin, 5);
			CreateSkinRenderer("Customize_Renderer_Front_Skin", CharacterAppearanceType.Skin, 21);
			CreateSkinRenderer("Customize_Renderer_Effect", CharacterAppearanceType.Effect, 50);
		}


		public static void LoadFaceCustom(Dictionary<Workshop.FaceCustomType, Sprite> faceCustomInfo, string location)
		{
			FaceResourceSet eyeResourceSet = new FaceResourceSet();
			FaceResourceSet browResourceSet = new FaceResourceSet();
			FaceResourceSet mouthResourceSet = new FaceResourceSet();
			HairResourceSet frontHairResourceSet = new HairResourceSet();
			HairResourceSet rearHairResourceSet = new HairResourceSet();
			var loader = CustomizingResourceLoader.Instance;
			foreach (KeyValuePair<Workshop.FaceCustomType, Sprite> keyValuePair in faceCustomInfo)
			{
				Workshop.FaceCustomType key = keyValuePair.Key;
				Sprite value = keyValuePair.Value;
				switch (key)
				{
					case Workshop.FaceCustomType.Front_RearHair:
						rearHairResourceSet.Default = value;
						break;
					case Workshop.FaceCustomType.Front_FrontHair:
						frontHairResourceSet.Default = value;
						break;
					case Workshop.FaceCustomType.Front_Eye:
						eyeResourceSet.normal = value;
						break;
					case Workshop.FaceCustomType.Front_Brow_Normal:
						browResourceSet.normal = value;
						break;
					case Workshop.FaceCustomType.Front_Brow_Attack:
						browResourceSet.atk = value;
						break;
					case Workshop.FaceCustomType.Front_Brow_Hit:
						browResourceSet.hit = value;
						break;
					case Workshop.FaceCustomType.Front_Mouth_Normal:
						mouthResourceSet.normal = value;
						break;
					case Workshop.FaceCustomType.Front_Mouth_Attack:
						mouthResourceSet.atk = value;
						break;
					case Workshop.FaceCustomType.Front_Mouth_Hit:
						mouthResourceSet.hit = value;
						break;
					case Workshop.FaceCustomType.Side_RearHair_Rear:
						rearHairResourceSet.Side_Back = value;
						break;
					case Workshop.FaceCustomType.Side_FrontHair:
						frontHairResourceSet.Side_Front = value;
						break;
					case Workshop.FaceCustomType.Side_RearHair_Front:
						rearHairResourceSet.Side_Front = value;
						break;
					case Workshop.FaceCustomType.Side_Mouth:
						mouthResourceSet.atk_side = value;
						break;
					case Workshop.FaceCustomType.Side_Brow:
						browResourceSet.atk_side = value;
						break;
					case Workshop.FaceCustomType.Side_Eye:
						eyeResourceSet.atk_side = value;
						break;
				}
			}
			eyeResourceSet.FillSprite();
			browResourceSet.FillSprite();
			mouthResourceSet.FillSprite();
			FaceData data = FaceData.GetExtraData(faceCustomInfo);
			if (faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_Eye) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Side_Eye))
			{
				if (XLInitializer.originalEyeIndex >= 0 && XLInitializer.originalEyeIndex < loader._eyeResources.Count)
				{
					loader._eyeResources[XLInitializer.originalEyeIndex] = eyeResourceSet;
					XLInitializer.originalEyeIndex++;
				}
				else
				{
					XLInitializer.originalEyeIndex = -1;
					loader._eyeResources.Add(eyeResourceSet);
				}
				TrySetIndex(data, location, CustomizeType.Eye, XLInitializer.originalEyeIndex, loader._eyeResources);
			}
			if (faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_Brow_Attack) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_Brow_Hit) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_Brow_Normal) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Side_Brow))
			{
				if (XLInitializer.originalBrowIndex >= 0 && XLInitializer.originalBrowIndex < loader._browResources.Count)
				{
					loader._browResources[XLInitializer.originalBrowIndex] = browResourceSet;
					XLInitializer.originalBrowIndex++;
				}
				else
				{
					XLInitializer.originalBrowIndex = -1;
					loader._browResources.Add(browResourceSet);
				}
				TrySetIndex(data, location, CustomizeType.Brow, XLInitializer.originalBrowIndex, loader._browResources);
			}
			if (faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_Mouth_Attack) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_Mouth_Hit) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_Mouth_Normal) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Side_Mouth))
			{
				if (XLInitializer.originalMouthIndex >= 0 && XLInitializer.originalMouthIndex < loader._mouthResources.Count)
				{
					loader._mouthResources[XLInitializer.originalMouthIndex] = mouthResourceSet;
					XLInitializer.originalMouthIndex++;
				}
				else
				{
					XLInitializer.originalMouthIndex = -1;
					loader._mouthResources.Add(mouthResourceSet);
				}
				TrySetIndex(data, location, CustomizeType.Mouth, XLInitializer.originalMouthIndex, loader._mouthResources);
			}
			if (faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_FrontHair) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Side_FrontHair))
			{
				if (XLInitializer.originalFrontHairIndex >= 0 && XLInitializer.originalFrontHairIndex < loader._frontHairResources.Count)
				{
					loader._frontHairResources[XLInitializer.originalFrontHairIndex] = frontHairResourceSet;
					XLInitializer.originalFrontHairIndex++;
				}
				else
				{
					XLInitializer.originalFrontHairIndex = -1;
					loader._frontHairResources.Add(frontHairResourceSet);
				}
				TrySetIndex(data, location, CustomizeType.FrontHair, XLInitializer.originalFrontHairIndex, loader._frontHairResources);
			}
			if (faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Front_RearHair) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Side_RearHair_Front) || faceCustomInfo.ContainsKey(Workshop.FaceCustomType.Side_RearHair_Rear))
			{
				if (XLInitializer.originalRearHairIndex >= 0 && XLInitializer.originalRearHairIndex < loader._rearHairResources.Count)
				{
					loader._rearHairResources[XLInitializer.originalRearHairIndex] = rearHairResourceSet;
					XLInitializer.originalRearHairIndex++;
				}
				else
				{
					XLInitializer.originalRearHairIndex = -1;
					loader._rearHairResources.Add(rearHairResourceSet);
				}
				TrySetIndex(data, location, CustomizeType.RearHair, XLInitializer.originalRearHairIndex, loader._rearHairResources);
			}
		}
		internal static void TrySetIndex(FaceData data, string location, CustomizeType type, int originalIndex, ICollection originalList)
		{
			int index = (originalIndex > 0 ? originalIndex : originalList.Count) - 1;
			if (location != null)
			{
				locationsToIndexes[type][location] = index;
				indexesToLocations[type][index] = location;
			}
			if (data != null)
			{
				if (data.customNames.TryGetValue(type, out string name))
				{
					if (FaceData.idsByNames[type].TryGetValue(name, out var index1))
					{
						index = index1;
					}
					else
					{
						FaceData.idsByNames[type][name] = index;
					}
				}
				data.customIds[type] = index;
			}
		}

		internal void OnClickWorkshopBook()
		{
			CustomBookUIPatch.OnClickWorkshopBook();
		}

		internal static Dictionary<CustomizeType, Dictionary<string, int>> locationsToIndexes = new Dictionary<CustomizeType, Dictionary<string, int>>
		{
			[CustomizeType.FrontHair] = new Dictionary<string, int>(),
			[CustomizeType.RearHair] = new Dictionary<string, int>(),
			[CustomizeType.Eye] = new Dictionary<string, int>(),
			[CustomizeType.Brow] = new Dictionary<string, int>(),
			[CustomizeType.Mouth] = new Dictionary<string, int>(),
		};
		internal static Dictionary<CustomizeType, Dictionary<int, string>> indexesToLocations = new Dictionary<CustomizeType, Dictionary<int, string>>
		{
			[CustomizeType.FrontHair] = new Dictionary<int, string>(),
			[CustomizeType.RearHair] = new Dictionary<int, string>(),
			[CustomizeType.Eye] = new Dictionary<int, string>(),
			[CustomizeType.Brow] = new Dictionary<int, string>(),
			[CustomizeType.Mouth] = new Dictionary<int, string>(),
		};

		static readonly string localModsFolder = Path.Combine(Application.dataPath, "Mods"); 
		static readonly string workshopModsFolder = PlatformManager.Instance.GetWorkshopDirPath();
		static readonly bool hasWorkshop = !string.IsNullOrWhiteSpace(workshopModsFolder);
		static readonly Dictionary<string, string> localModsByFolder = new Dictionary<string, string>();
		static readonly Dictionary<string, string> workshopModsByFolder = new Dictionary<string, string>();
		static readonly char[] pathSeparators = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

		internal static void LoadModFolders()
		{
			foreach (var mod in ModContentManager.Instance._allMods)
			{
				var path = mod.dirInfo.FullName;
				if (path.StartsWith(localModsFolder))
				{
					var modOwnFolder = path.Substring(localModsFolder.Length).Split(pathSeparators, System.StringSplitOptions.RemoveEmptyEntries)[0];
					localModsByFolder[modOwnFolder] = mod.invInfo.workshopInfo.uniqueId;
				}
				else if (hasWorkshop && path.StartsWith(workshopModsFolder))
				{
					var modOwnFolder = path.Substring(workshopModsFolder.Length).Split(pathSeparators, System.StringSplitOptions.RemoveEmptyEntries)[0];
					workshopModsByFolder[modOwnFolder] = mod.invInfo.workshopInfo.uniqueId;
				}
			}
		}

		public static string GetCustomLocationKeyByFolder(string path)
		{
			var parent = Path.GetDirectoryName(path);
			if (parent == localModsFolder || parent == workshopModsFolder)
			{
				return $"skin_{Path.GetFileName(path)}";
			}
			if (path.StartsWith(localModsFolder))
			{
				var modOwnFolder = path.Substring(localModsFolder.Length).Split(pathSeparators, System.StringSplitOptions.RemoveEmptyEntries)[0];
				if (localModsByFolder.TryGetValue(modOwnFolder, out var result)) 
				{
					return $"mod_{result}:{Path.GetFileName(path)}";
				}
			} 
			else if (hasWorkshop && path.StartsWith(workshopModsFolder))
			{
				var modOwnFolder = path.Substring(workshopModsFolder.Length).Split(pathSeparators, System.StringSplitOptions.RemoveEmptyEntries)[0];
				if (workshopModsByFolder.TryGetValue(modOwnFolder, out var result))
				{
					return $"mod_{result}:{Path.GetFileName(path)}";
				}
			}
			return null;
		}
	}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
