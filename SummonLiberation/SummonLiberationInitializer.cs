using BattleCharacterProfile;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using UI;
using UnityEngine;
using System.Linq;
using System.Reflection;

namespace SummonLiberation
{
	public class Harmony_Patch
	{
		//Enlarge Librarian's formation
		[HarmonyPatch(typeof(LibraryFloorModel), nameof(LibraryFloorModel.Init))]
		[HarmonyPostfix]
		static void LibraryFloorModel_Init_Post(LibraryFloorModel __instance)
		{
			try
			{
				AddIndexes(__instance._formationIndex, 99);
				AddFormationPosition(__instance._defaultFormation, 99);
				if (__instance._formation == null)
				{
					__instance._formation = __instance._defaultFormation;
				}
				else
				{
					AddFormationPosition(__instance._formation, 99);
				}
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/LFIerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		//remove error logs about saved formations being too big
		[HarmonyPatch(typeof(LibraryFloorModel), nameof(LibraryFloorModel.LoadFromSaveData))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> LibraryFloorModel_LoadFromSaveData_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var method = AccessTools.Method(typeof(Debug), nameof(Debug.LogError), new Type[] {typeof(object)});
			var codes = instructions.ToList();
			for (int i = 0; i < codes.Count - 2; i++)
			{
				if (codes[i].Is(OpCodes.Ldstr, "formation index length is too high") && codes[i + 1].Is(OpCodes.Call, method))
				{
					codes.RemoveRange(i, 2);
					break;
				}
			}
			return codes;
		}
		//Enlarge Enemy's formation
		[HarmonyPatch(typeof(StageWaveModel), nameof(StageWaveModel.Init))]
		[HarmonyPostfix]
		static void StageWaveModel_Init_Post(StageWaveModel __instance)
		{
			try
			{
				AddIndexes(__instance._formationIndex, 100);
				AddFormationPositionForEnemy(__instance._formation, 100);
			}
			catch (Exception ex)
			{
				File.WriteAllText(Application.dataPath + "/Mods/SWMIerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
			}
		}
		static void AddIndexes(List<int> indexes, int targetCount)
		{
			List<int> sortedIndexes = new List<int>(indexes);
			sortedIndexes.Sort();
			int i = 0;
			for (int j = 0; indexes.Count < targetCount; j++)
			{
				if (i < sortedIndexes.Count && j == sortedIndexes[i])
				{
					i++;
				}
				else
				{
					indexes.Add(j);
				}
			}
		}
		static void AddFormationPosition(FormationModel Formation, int targetCount)
		{
			List<FormationPosition> _postionList = Formation._postionList;
			for (int i = _postionList.Count; i < targetCount; i++)
			{
				FormationPositionXmlData data = new FormationPositionXmlData()
				{
					name = "E" + i.ToString(),
					vector = new XmlVector2()
					{
						x = GetVector2X(i - 4),
						y = GetVector2Y(i - 4),
					},
					eventList = new List<FormationPositionEventXmlData>()
				};
				FormationPosition formationPosition = new FormationPosition(data)
				{
					eventList = new List<FormationPositionEvent>(),
					index = i
				};
				_postionList.Add(formationPosition);
			}
		}
		static int GetVector2X(int i)
		{
			switch (i)
			{
				case 1:
					return 12;
				case 2:
					return 12;
				case 3:
					return 9;
				case 4:
					return 9;
				case 5:
					return 8;
				case 6:
					return 8;
				case 7:
					return 21;
				case 8:
					return 21;
				case 9:
					return 20;
				case 10:
					return 20;
				case 11:
					return 2;
				case 12:
					return 2;
				case 13:
					return 22;
				case 14:
					return 22;
				case 15:
					return 22;
				default:
					return 12;
			}
		}
		static int GetVector2Y(int i)
		{
			switch (i)
			{
				case 1:
					return 7;
				case 2:
					return -9;
				case 3:
					return -5;
				case 4:
					return -15;
				case 5:
					return 19;
				case 6:
					return 9;
				case 7:
					return 19;
				case 8:
					return 9;
				case 9:
					return -5;
				case 10:
					return -15;
				case 11:
					return -14;
				case 12:
					return 14;
				case 13:
					return -16;
				case 14:
					return 0;
				case 15:
					return 16;
				default:
					return 0;
			}
		}
		static void AddFormationPositionForEnemy(FormationModel Formation, int targetCount)
		{
			List<FormationPosition> _postionList = Formation._postionList;
			int x = -23;
			int y = 18;
			for (int i = _postionList.Count; i < targetCount; i++)
			{
				FormationPositionXmlData data = new FormationPositionXmlData()
				{
					name = "E" + i.ToString(),
					vector = new XmlVector2()
					{
						x = x,
						y = y
					},
					eventList = null
				};
				x += 5;
				if (x > -3)
				{
					y -= 7;
					x = -23;
				}
				if (y < -17)
				{
					x = -12;
					y = 0;
				}
				FormationPosition formationPosition = new FormationPosition(data)
				{
					eventList = new List<FormationPositionEvent>(),
					index = i
				};
				_postionList.Add(formationPosition);
			}
		}
		public static BattleUnitModel SummonUnit(Faction Faction, LorId EnemyUnitID, LorId BookID, int Index = -1, string PlayerUnitName = "Null", bool ForcedlySummon = false)
		{
			try
			{
				if (EnemyUnitID == null)
				{
					EnemyUnitID = LorId.None;
				}
				BattleUnitModel battleUnitModel = null;
				if (BattleObjectManager.instance.ExistsUnit((BattleUnitModel x) => x.faction == Faction && x.index == Index) && !ForcedlySummon)
				{
					return battleUnitModel;
				}
				if (Faction == Faction.Enemy)
				{
					StageModel _stageModel = StageController.Instance._stageModel;
					BattleTeamModel _enemyTeam = StageController.Instance._enemyTeam;

					UnitBattleDataModel EnemyUnitBattleDataModel = UnitBattleDataModel.CreateUnitBattleDataByEnemyUnitId(_stageModel, EnemyUnitID);
					BattleObjectManager.instance.UnregisterUnitByIndex(Faction, Index);

					StageWaveModel currentWaveModel = StageController.Instance.GetCurrentWaveModel();
					UnitDataModel unitData = EnemyUnitBattleDataModel.unitData;
					battleUnitModel = BattleObjectManager.CreateDefaultUnit(Faction.Enemy);

					if (Index == -1)
					{
						battleUnitModel.index = -1;
						foreach (BattleUnitModel unit in BattleObjectManager.instance.GetAliveList(Faction))
						{
							if (unit.index >= battleUnitModel.index)
							{
								battleUnitModel.index = unit.index + 1;
							}
						}
					}
					else
					{
						battleUnitModel.index = Index;
					}

					battleUnitModel.formation = currentWaveModel.GetFormationPosition(battleUnitModel.index);
					if (EnemyUnitBattleDataModel.isDead)
					{
						return battleUnitModel;
					}
					battleUnitModel.grade = unitData.grade;
					battleUnitModel.SetUnitData(EnemyUnitBattleDataModel);
					battleUnitModel.OnCreated();
					_enemyTeam.AddUnit(battleUnitModel);
				}
				else
				{

					StageLibraryFloorModel currentStageFloorModel = StageController.Instance.GetCurrentStageFloorModel();
					BattleTeamModel _librarianTeam = StageController.Instance._librarianTeam;

					UnitDataModel unitDataModel = new UnitDataModel(BookID, currentStageFloorModel.Sephirah, false);
					UnitBattleDataModel unitBattleDataModel = new UnitBattleDataModel(StageController.Instance.GetStageModel(), unitDataModel);
					BattleObjectManager.instance.UnregisterUnitByIndex(Faction, Index);

					if (EnemyUnitClassInfoList.Instance.GetData(EnemyUnitID) != null)
					{
						unitDataModel.SetByEnemyUnitClassInfo(EnemyUnitClassInfoList.Instance.GetData(EnemyUnitID));
					}
					else
					{
						unitDataModel.SetTemporaryPlayerUnitByBook(BookID);
						unitDataModel.isSephirah = false;
						unitDataModel.customizeData.height = 175;
						unitDataModel.gender = Gender.N;
						unitDataModel.appearanceType = unitDataModel.gender;
						unitDataModel.SetCustomName(PlayerUnitName);
						unitDataModel.forceItemChangeLock = true;
					}
					if (PlayerUnitName != "Null")
					{
						unitDataModel.SetTempName(PlayerUnitName);
					}
					unitBattleDataModel.Init();

					battleUnitModel = BattleObjectManager.CreateDefaultUnit(Faction.Player);

					if (Index == -1)
					{
						battleUnitModel.index = -1;
						foreach (BattleUnitModel unit in BattleObjectManager.instance.GetAliveList(Faction))
						{
							if (unit.index >= battleUnitModel.index)
							{
								battleUnitModel.index = unit.index + 1;
							}
						}
					}
					else
					{
						battleUnitModel.index = Index;
					}

					battleUnitModel.grade = unitDataModel.grade;
					battleUnitModel.formation = currentStageFloorModel.GetFormationPosition(battleUnitModel.index);
					battleUnitModel.SetUnitData(unitBattleDataModel);
					unitDataModel._enemyUnitId = EnemyUnitID;
					battleUnitModel.OnCreated();
					_librarianTeam.AddUnit(battleUnitModel);
				}
				BattleObjectManager.instance.RegisterUnit(battleUnitModel);
				battleUnitModel.passiveDetail.OnUnitCreated();
				if (battleUnitModel.allyCardDetail.GetAllDeck().Count > 0)
				{
					battleUnitModel.allyCardDetail.ReturnAllToDeck();
					battleUnitModel.allyCardDetail.DrawCards(battleUnitModel.UnitData.unitData.GetStartDraw());
				}
				if (StageController.Instance.Phase <= StageController.StagePhase.ApplyLibrarianCardPhase)
				{
					battleUnitModel.OnRoundStartOnlyUI();
					battleUnitModel.RollSpeedDice();
				}
				BattleManagerUI.Instance.ui_unitListInfoSummary.UpdateCharacterProfileAll();
				int num2 = 0;
				foreach (BattleUnitModel battleUnitModel2 in BattleObjectManager.instance.GetList())
				{
					UICharacterRenderer.Instance.SetCharacter(battleUnitModel2.UnitData.unitData, num2++, true, false);
				}
				BattleObjectManager.instance.InitUI();
				return battleUnitModel;
			}
			catch (Exception ex)
			{
				Debug.LogError("召唤错误" + ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return null;
		}
		public static BattleUnitModel SummonUnitByUnitBattleData(Faction Faction, UnitBattleDataModel unitBattleData, LorId EnemyUnitID, int Index = -1, string PlayerUnitName = "Null", bool ForcedlySummon = false)
		{
			try
			{
				if (EnemyUnitID == null)
				{
					EnemyUnitID = LorId.None;
				}
				BattleUnitModel battleUnitModel = null;
				if (BattleObjectManager.instance.ExistsUnit((BattleUnitModel x) => x.faction == Faction && x.index == Index) && !ForcedlySummon)
				{
					return battleUnitModel;
				}
				if (Faction == Faction.Enemy)
				{
					StageModel _stageModel = StageController.Instance._stageModel;
					BattleTeamModel _enemyTeam = StageController.Instance._enemyTeam;


					BattleObjectManager.instance.UnregisterUnitByIndex(Faction, Index);
					StageWaveModel currentWaveModel = StageController.Instance.GetCurrentWaveModel();
					UnitDataModel unitData = unitBattleData.unitData;
					battleUnitModel = BattleObjectManager.CreateDefaultUnit(Faction.Enemy);

					if (Index == -1)
					{
						battleUnitModel.index = -1;
						foreach (BattleUnitModel unit in BattleObjectManager.instance.GetAliveList(Faction))
						{
							if (unit.index >= battleUnitModel.index)
							{
								battleUnitModel.index = unit.index + 1;
							}
						}
					}
					else
					{
						battleUnitModel.index = Index;
					}

					battleUnitModel.formation = currentWaveModel.GetFormationPosition(battleUnitModel.index);
					if (unitBattleData.isDead)
					{
						return battleUnitModel;
					}
					battleUnitModel.grade = unitData.grade;
					battleUnitModel.SetUnitData(unitBattleData);
					battleUnitModel.OnCreated();
					_enemyTeam.AddUnit(battleUnitModel);
				}
				else
				{

					StageLibraryFloorModel currentStageFloorModel = StageController.Instance.GetCurrentStageFloorModel();
					BattleTeamModel _librarianTeam = StageController.Instance._librarianTeam;

					BattleObjectManager.instance.UnregisterUnitByIndex(Faction, Index);


					if (PlayerUnitName != "Null")
					{
						unitBattleData.unitData.SetTempName(PlayerUnitName);
					}
					unitBattleData.Init();
					battleUnitModel = BattleObjectManager.CreateDefaultUnit(Faction.Player);

					if (Index == -1)
					{
						battleUnitModel.index = -1;
						foreach (BattleUnitModel unit in BattleObjectManager.instance.GetAliveList(Faction))
						{
							if (unit.index >= battleUnitModel.index)
							{
								battleUnitModel.index = unit.index + 1;
							}
						}
					}
					else
					{
						battleUnitModel.index = Index;
					}

					battleUnitModel.grade = unitBattleData.unitData.grade;
					battleUnitModel.formation = currentStageFloorModel.GetFormationPosition(battleUnitModel.index);
					battleUnitModel.SetUnitData(unitBattleData);
					unitBattleData.unitData._enemyUnitId = EnemyUnitID;
					battleUnitModel.OnCreated();
					_librarianTeam.AddUnit(battleUnitModel);
				}
				BattleObjectManager.instance.RegisterUnit(battleUnitModel);
				battleUnitModel.passiveDetail.OnUnitCreated();
				if (battleUnitModel.allyCardDetail.GetAllDeck().Count > 0)
				{
					battleUnitModel.allyCardDetail.ReturnAllToDeck();
					battleUnitModel.allyCardDetail.DrawCards(battleUnitModel.UnitData.unitData.GetStartDraw());
				}
				if (StageController.Instance.Phase <= StageController.StagePhase.ApplyLibrarianCardPhase)
				{
					battleUnitModel.OnRoundStartOnlyUI();
					battleUnitModel.RollSpeedDice();
				}
				BattleManagerUI.Instance.ui_unitListInfoSummary.UpdateCharacterProfileAll();
				int num2 = 0;
				foreach (BattleUnitModel battleUnitModel2 in BattleObjectManager.instance.GetList())
				{
					UICharacterRenderer.Instance.SetCharacter(battleUnitModel2.UnitData.unitData, num2++, true, false);
				}
				BattleObjectManager.instance.InitUI();
				return battleUnitModel;
			}
			catch (Exception ex)
			{
				Debug.LogError("召唤错误" + ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return null;
		}
		public static BattleUnitModel SummonUnitByUnitData(Faction Faction, UnitDataModel unitData, LorId EnemyUnitID, LorId BookID, int Index = -1, string PlayerUnitName = "Null", bool ForcedlySummon = false)
		{
			try
			{
				if (EnemyUnitID == null)
				{
					EnemyUnitID = LorId.None;
				}
				BattleUnitModel battleUnitModel = null;
				if (BattleObjectManager.instance.ExistsUnit((BattleUnitModel x) => x.faction == Faction && x.index == Index) && !ForcedlySummon)
				{
					return battleUnitModel;
				}
				if (Faction == Faction.Enemy)
				{
					StageModel _stageModel = StageController.Instance._stageModel;
					BattleTeamModel _enemyTeam = StageController.Instance._enemyTeam;

					UnitBattleDataModel EnemyUnitBattleDataModel = new UnitBattleDataModel(_stageModel, unitData);
					EnemyUnitBattleDataModel.Init();

					BattleObjectManager.instance.UnregisterUnitByIndex(Faction, Index);

					StageWaveModel currentWaveModel = StageController.Instance.GetCurrentWaveModel();

					battleUnitModel = BattleObjectManager.CreateDefaultUnit(Faction.Enemy);

					if (Index == -1)
					{
						battleUnitModel.index = -1;
						foreach (BattleUnitModel unit in BattleObjectManager.instance.GetAliveList(Faction))
						{
							if (unit.index >= battleUnitModel.index)
							{
								battleUnitModel.index = unit.index + 1;
							}
						}
					}
					else
					{
						battleUnitModel.index = Index;
					}

					battleUnitModel.formation = currentWaveModel.GetFormationPosition(battleUnitModel.index);
					if (EnemyUnitBattleDataModel.isDead)
					{
						return battleUnitModel;
					}
					battleUnitModel.grade = unitData.grade;
					battleUnitModel.SetUnitData(EnemyUnitBattleDataModel);
					battleUnitModel.OnCreated();
					_enemyTeam.AddUnit(battleUnitModel);
				}
				else
				{
					StageLibraryFloorModel currentStageFloorModel = StageController.Instance.GetCurrentStageFloorModel();
					BattleTeamModel _librarianTeam = StageController.Instance._librarianTeam;

					UnitBattleDataModel unitBattleDataModel = new UnitBattleDataModel(StageController.Instance.GetStageModel(), unitData);
					BattleObjectManager.instance.UnregisterUnitByIndex(Faction, Index);

					if (EnemyUnitClassInfoList.Instance.GetData(EnemyUnitID) != null)
					{
						unitData.SetByEnemyUnitClassInfo(EnemyUnitClassInfoList.Instance.GetData(EnemyUnitID));
					}

					if (PlayerUnitName != "Null")
					{
						unitData.SetTempName(PlayerUnitName);
					}

					unitBattleDataModel.Init();
					battleUnitModel = BattleObjectManager.CreateDefaultUnit(Faction.Player);

					if (Index == -1)
					{
						battleUnitModel.index = -1;
						foreach (BattleUnitModel unit in BattleObjectManager.instance.GetAliveList(Faction))
						{
							if (unit.index >= battleUnitModel.index)
							{
								battleUnitModel.index = unit.index + 1;
							}
						}
					}
					else
					{
						battleUnitModel.index = Index;
					}

					battleUnitModel.grade = unitData.grade;
					battleUnitModel.formation = currentStageFloorModel.GetFormationPosition(battleUnitModel.index);
					battleUnitModel.SetUnitData(unitBattleDataModel);
					unitData._enemyUnitId = EnemyUnitID;
					battleUnitModel.OnCreated();
					_librarianTeam.AddUnit(battleUnitModel);
				}
				BattleObjectManager.instance.RegisterUnit(battleUnitModel);
				battleUnitModel.passiveDetail.OnUnitCreated();
				if (battleUnitModel.allyCardDetail.GetAllDeck().Count > 0)
				{
					battleUnitModel.allyCardDetail.ReturnAllToDeck();
					battleUnitModel.allyCardDetail.DrawCards(battleUnitModel.UnitData.unitData.GetStartDraw());
				}
				if (StageController.Instance.Phase <= StageController.StagePhase.ApplyLibrarianCardPhase)
				{
					battleUnitModel.OnRoundStartOnlyUI();
					battleUnitModel.RollSpeedDice();
				}
				BattleManagerUI.Instance.ui_unitListInfoSummary.UpdateCharacterProfileAll();
				int num2 = 0;
				foreach (BattleUnitModel battleUnitModel2 in BattleObjectManager.instance.GetList())
				{
					UICharacterRenderer.Instance.SetCharacter(battleUnitModel2.UnitData.unitData, num2++, true, false);
				}
				BattleObjectManager.instance.InitUI();
				return battleUnitModel;
			}
			catch (Exception ex)
			{
				Debug.LogError("召唤错误" + ex.Message + Environment.NewLine + ex.StackTrace);
			}
			return null;
		}
	}
}