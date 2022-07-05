using System.Collections.Generic;
using System.IO;
using System.Linq;
using UI;
using UnityEngine;
using Workshop;

namespace ExtendedLoader
{
    public class XLRoot
    {
        public const int THUMB_LAYER = 11;
        public const int THUMB_MASK = 1 << THUMB_LAYER;
        public static readonly GameObject persistentRoot = CreatePersistentRoot();
        public static readonly GameObject UICustomAppearancePrefab = CreateUIAppearancePrefab();
        public static readonly GameObject CustomAppearancePrefab = CreateCustomAppearancePrefab();
        public static readonly Dictionary<int, Sprite> SkinThumb = new Dictionary<int, Sprite>();
        public static readonly SkinPartRenderer thumbRenderers = new SkinPartRenderer();
        static Camera thumbCamera = null;
        public static Camera ThumbCamera
        {
            get
            {
                if (thumbCamera == null)
                {
                    UICharacterRenderer uiRenderer = SingletonBehavior<UICharacterRenderer>.Instance;
                    if (uiRenderer != null)
                    {
                        Camera camera = UnityEngine.Object.Instantiate(uiRenderer.cameraList[0], persistentRoot.transform);
                        camera.name = "ThumbCamera";
                        camera.targetTexture = UnityEngine.Object.Instantiate(uiRenderer.cameraList[0].targetTexture);
                        UnityEngine.Object.DontDestroyOnLoad(camera.targetTexture);
                        camera.targetTexture.name = "ThumbTargetTexture";
                        camera.transform.position = new Vector3(-2000f, 0f, 0f);
                        camera.cullingMask = THUMB_MASK;
                        thumbCamera = camera;
                        SpriteRenderer original = UICustomAppearancePrefab.GetComponent<CharacterAppearance>()._motionList[0].motionSpriteSet[0].sprRenderer;

                        thumbRenderers.front = UnityEngine.Object.Instantiate(original, camera.transform);
                        original = thumbRenderers.front;
                        original.name = "ThumbRenderer_Front";
                        original.gameObject.layer = THUMB_LAYER;
                        original.sortingOrder = 2;
                        original.transform.localPosition = new Vector3(0f, -2f, 0f);
                        original.gameObject.SetActive(false);

                        thumbRenderers.rear = UnityEngine.Object.Instantiate(original, camera.transform);
                        thumbRenderers.rear.name = "ThumbRenderer_Mid";
                        thumbRenderers.rear.sortingOrder = 1;

                        thumbRenderers.rearest = UnityEngine.Object.Instantiate(original, camera.transform);
                        thumbRenderers.rearest.name = "ThumbRenderer_Back";
                        thumbRenderers.rearest.sortingOrder = 0;
                    }
                }
                return thumbCamera;
            }
        }

        public static Sprite MakeThumbnail(ClothCustomizeData data)
        {
            if (ThumbCamera == null)
                return null;
            thumbRenderers.front.sprite = data.frontSprite;
            thumbRenderers.rear.sprite = data.sprite;
            thumbRenderers.rearest.sprite = (data as ExtendedClothCustomizeData)?.backSprite;
            foreach (object obj in thumbCamera.transform)
            {
                SpriteRenderer renderer = (obj as Transform)?.GetComponent<SpriteRenderer>();
                if (renderer != null)
                    renderer.gameObject.SetActive(renderer.sprite != null);
            }
            RenderTexture rt = thumbCamera.targetTexture;
            RenderTexture backup = RenderTexture.active;
            RenderTexture.active = rt;
            rt.Release();
            thumbCamera.Render();
            Texture2D tex = new Texture2D(256, 512);
            tex.ReadPixels(new Rect(0, 0, 256, 512), 0, 0);
            foreach (object obj in thumbCamera.transform)
            {
                SpriteRenderer renderer = (obj as Transform)?.GetComponent<SpriteRenderer>();
                if (renderer != null)
                    renderer.gameObject.SetActive(false);
            }
            RenderTexture.active = backup;
            byte[] bytes = tex.EncodeToPNG();
            string text = new DirectoryInfo(data.spritePath).Parent.Parent.FullName + "/Thumb.png";
            File.WriteAllBytes(text, bytes);
            Debug.Log("Saved baked thumbnail to " + text);
            return Sprite.Create(tex, new Rect(0f, 0f, 256f, 512f), new Vector2(0.5f, 0.5f));
        }

        static GameObject CreatePersistentRoot()
        {
            GameObject root = new GameObject("ExtendedLoader_PersistentRoot");
            UnityEngine.Object.DontDestroyOnLoad(root);
            return root;
        }

        static GameObject CreateUIAppearancePrefab()
        {
            GameObject proto = (GameObject)Resources.Load("Prefabs/Characters/[Prefab]Appearance_Custom");
            GameObject gameObject = UnityEngine.Object.Instantiate(proto, persistentRoot.transform);
            CharacterAppearance appearance = gameObject.GetComponent<CharacterAppearance>();
            foreach (CharacterMotion motion in appearance._motionList.ToList())
            {
                if (motion.actionDetail != ActionDetail.Default && motion.actionDetail != ActionDetail.Standing)
                {
                    appearance._motionList.Remove(motion);
                    UnityEngine.Object.Destroy(motion.gameObject);
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
                    ExtendedCharacterMotion newMotion = CopyCharacterMotion(motion);
                    appearance._motionList.Remove(motion);
                    UnityEngine.Object.Destroy(motion);
                    appearance._motionList.Add(newMotion);
                }
            }
            UnityEngine.Object.Destroy(gameObject.GetComponent<WorkshopSkinDataSetter>());
            gameObject.AddComponent<UIWorkshopSkinDataSetter>();
            gameObject.name = "Appearance_Custom_Extended_UI";
            return gameObject;
        }

        static GameObject CreateCustomAppearancePrefab()
        {
            GameObject proto = (GameObject)Resources.Load("Prefabs/Characters/[Prefab]Appearance_Custom");
            GameObject gameObject = UnityEngine.Object.Instantiate(proto, persistentRoot.transform);
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
                ExtendedCharacterMotion newMotion = CopyCharacterMotion(oldMotion);
                appearance._motionList.Remove(oldMotion);
                UnityEngine.Object.Destroy(oldMotion);
                appearance._motionList.Add(newMotion);
            }
            CharacterMotion masterMotion = appearance._motionList[0];
            Transform masterTransform = masterMotion.transform.parent;
            ActionDetail action = (ActionDetail)9;
            CharacterMotion addedMotion = UnityEngine.Object.Instantiate(masterMotion, masterTransform);
            addedMotion.transform.position = masterMotion.transform.position;
            addedMotion.transform.localPosition = masterMotion.transform.localPosition;
            addedMotion.transform.localScale = masterMotion.transform.localScale;
            addedMotion.actionDetail = action;
            addedMotion.transform.name = "Custom_" + action.ToString();
            appearance._motionList.Add(addedMotion);
            for (int i = 12; i < 31; i++)
            {
                action = (ActionDetail)i;
                addedMotion = UnityEngine.Object.Instantiate(masterMotion, masterTransform);
                addedMotion.transform.position = masterMotion.transform.position;
                addedMotion.transform.localPosition = masterMotion.transform.localPosition;
                addedMotion.transform.localScale = masterMotion.transform.localScale;
                addedMotion.actionDetail = action;
                addedMotion.transform.name = "Custom_" + action.ToString();
                appearance._motionList.Add(addedMotion);
            }
            gameObject.name = "Appearance_Custom_Extended";
            return gameObject;
        }

        static void AddSkinRenderers(CharacterMotion motion, Transform transform)
        {
            motion.motionSpriteSet.Clear();

            Transform transform1 = UnityEngine.Object.Instantiate(transform, transform.parent);
            transform1.gameObject.name = "Customize_Renderer_Back";
            motion.motionSpriteSet.Add(new SpriteSet(transform1.GetComponent<SpriteRenderer>(), CharacterAppearanceType.Body));
            transform1 = UnityEngine.Object.Instantiate(transform, transform.parent);
            transform1.gameObject.name = "Customize_Renderer_Back_Skin";
            motion.motionSpriteSet.Add(new SpriteSet(transform1.GetComponent<SpriteRenderer>(), CharacterAppearanceType.Skin));
            transform1 = UnityEngine.Object.Instantiate(transform, transform.parent);
            transform1.gameObject.name = "Customize_Renderer_Skin";
            motion.motionSpriteSet.Add(new SpriteSet(transform1.GetComponent<SpriteRenderer>(), CharacterAppearanceType.Skin));
            transform1 = UnityEngine.Object.Instantiate(transform, transform.parent);
            transform1.gameObject.name = "Customize_Renderer_Front_Skin";
            motion.motionSpriteSet.Add(new SpriteSet(transform1.GetComponent<SpriteRenderer>(), CharacterAppearanceType.Skin));

            motion.motionSpriteSet.Add(new SpriteSet(motion.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>(), CharacterAppearanceType.Body));
            motion.motionSpriteSet.Add(new SpriteSet(motion.transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>(), CharacterAppearanceType.Head));
            motion.motionSpriteSet.Add(new SpriteSet(motion.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>(), CharacterAppearanceType.Body));
        }

        static ExtendedCharacterMotion CopyCharacterMotion(CharacterMotion motion)
        {
            ExtendedCharacterMotion motion1 = motion.gameObject.AddComponent<ExtendedCharacterMotion>();
            motion1.actionDetail = motion.actionDetail;
            motion1.additionalPivotList = motion.additionalPivotList;
            motion1.customPivot = motion.customPivot;
            motion1.motionSpriteSet = motion.motionSpriteSet;
            motion1.giftPivotList = motion.giftPivotList;
            return motion1;
        }
    }
}
