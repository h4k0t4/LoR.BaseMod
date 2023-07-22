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
		public virtual bool IsMapChangableByAssimilation()
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
		public override bool IsMapChangableByAssimilation()
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
				StageController.Instance.GetCurrentWaveModel().team.emotionTotalBonus = 100;
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
			StageWaveModel currentWaveModel = StageController.Instance.GetCurrentWaveModel();
			if (currentWaveModel != null)
			{
				FormationModel formation = currentWaveModel.GetFormation();
				formation?.PostionList.ForEach(delegate (FormationPosition x)
				{
					x.ChangePosToDefault();
				});
			}
			StageLibraryFloorModel currentStageFloorModel = StageController.Instance.GetCurrentStageFloorModel();
			if (currentStageFloorModel != null)
			{
				FormationModel formation2 = currentStageFloorModel.GetFormation();
				formation2?.PostionList.ForEach(delegate (FormationPosition x)
				{
					x.ChangePosToDefault();
				});
			}
		}

		public override void OnRoundStart()
		{
			base.OnRoundStart();
			BattleSoundManager.Instance.SetEnemyTheme(mapBgm);
			int emotionTotalCoinNumber = StageController.Instance.GetCurrentStageFloorModel().team.emotionTotalCoinNumber;
			StageController.Instance.GetCurrentWaveModel().team.emotionTotalBonus = emotionTotalCoinNumber + 100;
		}

		public override GameObject GetWallCrater()
		{
			return null;
		}

		public override GameObject GetScratch(int lv, Transform parent)
		{
			return null;
		}

		void Retextualize()
		{
			Transform foregroundRoot = transform.Find("[Sprite]Foregrounds_BlackFrames_Act5 (1)");
			foregroundRoot.gameObject.SetActive(true);
			foregroundRoot.localScale = new Vector3(6.5f, 6.8f, 1f);
			Transform backgroundRoot = transform.Find("[Transform]BackgroundRootTransform (1)");
			Transform backgroundContainer = backgroundRoot.Find("GameObject (1)");
			Transform background = backgroundContainer.Find("BG (1)");
			DuplicateSprite(background, TextureBackground, 0.5f);
			foreach (Transform transform in backgroundContainer)
			{
				if (transform != background)
				{
					transform.gameObject.SetActive(false);
				}
			}
			foreach (Transform transform in backgroundRoot)
			{
				if (transform != backgroundContainer)
				{
					transform.gameObject.SetActive(false);
				}
			}
			Transform groundRoot = transform.Find("[Transform]GroundSprites (1)");
			Transform road = groundRoot.Find("Road");
			DuplicateSprite(road, TextureFloor, 0.5f);
			Transform roadUnder = groundRoot.Find("RoadUnder");
			DuplicateSprite(roadUnder, TextureFloorUnder, 0.5f);
			foreach (Transform transform in groundRoot)
			{
				if (transform != road && transform != roadUnder)
				{
					transform.gameObject.SetActive(false);
				}
			}
			foreach (Transform transform in transform)
			{
				if (transform != foregroundRoot && transform != backgroundRoot && transform != groundRoot)
				{
					transform.gameObject.SetActive(false);
				}
			}
		}

		void DuplicateSprite(Transform transform, string path, float YMiddle = 0.5f)
		{
			string filePath = resourcePath + path + ".png";
			if (!File.Exists(filePath))
			{
				transform.gameObject.SetActive(false);
				return;
			}
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.LoadImage(File.ReadAllBytes(filePath));
			Sprite sprite = transform.GetComponent<SpriteRenderer>().sprite;
			transform.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, YMiddle), sprite.pixelsPerUnit, 0U, SpriteMeshType.FullRect);
			transform.transform.localPosition = Vector3.zero;
			transform.gameObject.SetActive(true);
		}

		readonly string TextureBackground = "BackGround";

		readonly string TextureFloor = "Floor";

		readonly string TextureFloorUnder = "FloorUnder";

		string resourcePath = "";

		string mapName = "";
	}
}