using System;
using System.IO;
using UnityEngine;

namespace BaseMod
{
    public class CustomMapManager : CreatureMapManager
    {
        public virtual bool IsMapChangable()
        {
            return true;
        }
        public virtual void CustomInit()
        {
        }
    }
}

namespace BaseMod
{
    public class SimpleMapManager : CustomMapManager
    {
        public bool SimpleInit(string Path, string MapName)
        {
            if (Directory.Exists(Path))
            {
                resourcePath = Path + "/";
                mapName = MapName;
                return transform;
            }
            return false;
        }
        public override bool IsMapChangable()
        {
            return false;
        }
        public override void CustomInit()
        {
            try
            {
                Retextualize();
                AudioClip[] audioClips = new AudioClip[3];
                foreach (FileInfo fileInfo in new DirectoryInfo(resourcePath).GetFiles())
                {
                    if (Path.GetFileNameWithoutExtension(fileInfo.FullName) == "BGM")
                    {
                        audioClips[0] = Tools.GetAudio(fileInfo.FullName, mapName + "BGM");
                        audioClips[1] = Tools.GetAudio(fileInfo.FullName, mapName + "BGM");
                        audioClips[2] = Tools.GetAudio(fileInfo.FullName, mapName + "BGM");
                        break;
                    }
                    if (Path.GetFileNameWithoutExtension(fileInfo.FullName) == "BGM1")
                    {
                        audioClips[0] = Tools.GetAudio(fileInfo.FullName, mapName + "BGM1");
                    }
                    if (Path.GetFileNameWithoutExtension(fileInfo.FullName) == "BGM2")
                    {
                        audioClips[1] = Tools.GetAudio(fileInfo.FullName, mapName + "BGM2");
                    }
                    if (Path.GetFileNameWithoutExtension(fileInfo.FullName) == "BGM3")
                    {
                        audioClips[2] = Tools.GetAudio(fileInfo.FullName, mapName + "BGM3");
                    }
                }
                mapBgm = audioClips;
                mapSize = MapSize.L;
                _bMapInitialized = true;
                Singleton<StageController>.Instance.GetCurrentWaveModel().team.emotionTotalBonus = 100;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/Mods/SimpleMaperror.log", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public override void EnableMap(bool b)
        {
            isEnabled = b;
            gameObject.SetActive(b);
        }

        public override void ResetMap()
        {
            StageWaveModel currentWaveModel = Singleton<StageController>.Instance.GetCurrentWaveModel();
            if (currentWaveModel != null)
            {
                FormationModel formation = currentWaveModel.GetFormation();
                if (formation != null)
                {
                    formation.PostionList.ForEach(delegate (FormationPosition x)
                    {
                        x.ChangePosToDefault();
                    });
                }
            }
            StageLibraryFloorModel currentStageFloorModel = Singleton<StageController>.Instance.GetCurrentStageFloorModel();
            if (currentStageFloorModel != null)
            {
                FormationModel formation2 = currentStageFloorModel.GetFormation();
                if (formation2 != null)
                {
                    formation2.PostionList.ForEach(delegate (FormationPosition x)
                    {
                        x.ChangePosToDefault();
                    });
                }
            }
        }

        public override void OnRoundStart()
        {
            base.OnRoundStart();
            SingletonBehavior<BattleSoundManager>.Instance.SetEnemyTheme(mapBgm);
            int emotionTotalCoinNumber = Singleton<StageController>.Instance.GetCurrentStageFloorModel().team.emotionTotalCoinNumber;
            Singleton<StageController>.Instance.GetCurrentWaveModel().team.emotionTotalBonus = emotionTotalCoinNumber + 100;
        }

        public override GameObject GetWallCrater()
        {
            return null;
        }

        public override GameObject GetScratch(int lv, Transform parent)
        {
            return null;
        }

        private void Retextualize()
        {
            base.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            base.gameObject.transform.GetChild(0).localScale = new Vector3(6.5f, 6.8f, 1f);
            GameObject gameObject = base.gameObject.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
            DuplicateSprite(gameObject, TextureBackground, 0.5f);
            for (int i = 1; i < 6; i++)
            {
                base.gameObject.transform.GetChild(1).GetChild(0).GetChild(i).gameObject.SetActive(false);
            }
            GameObject gameObject2 = base.gameObject.transform.GetChild(2).GetChild(0).gameObject;
            DuplicateSprite(gameObject2, TextureFloor, 0.5f);
            GameObject gameObject3 = base.gameObject.transform.GetChild(2).GetChild(1).gameObject;
            DuplicateSprite(gameObject3, TextureFloorUnder, 0.5f);
            base.gameObject.transform.GetChild(2).GetChild(2).gameObject.SetActive(false);
            base.gameObject.transform.GetChild(2).GetChild(3).gameObject.SetActive(false);
            base.gameObject.transform.GetChild(3).gameObject.SetActive(false);
            base.gameObject.transform.GetChild(4).gameObject.SetActive(false);
            base.gameObject.transform.GetChild(5).gameObject.SetActive(false);
        }

        private void DuplicateSprite(GameObject obj, string path, float YMiddle = 0.5f)
        {
            string filePath = resourcePath + path + ".png";
            if (!File.Exists(filePath))
            {
                obj.SetActive(false);
                return;
            }
            Texture2D texture2D = new Texture2D(1, 1);
            texture2D.LoadImage(File.ReadAllBytes(filePath));
            Sprite sprite = obj.GetComponent<SpriteRenderer>().sprite;
            obj.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, YMiddle), sprite.pixelsPerUnit, 0U, SpriteMeshType.FullRect);
            obj.transform.localPosition = Vector3.zero;
            obj.SetActive(true);
        }

        private readonly string TextureBackground = "BackGround";

        private readonly string TextureFloor = "Floor";

        private readonly string TextureFloorUnder = "FloorUnder";

        private readonly string Bgm = "BGM";

        private readonly int Ydeficit;

        private string resourcePath = "";

        private string mapName = "";
    }
}