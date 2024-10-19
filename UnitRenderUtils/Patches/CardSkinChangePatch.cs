using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using static System.Reflection.Emit.OpCodes;
using static HarmonyLib.AccessTools;

namespace ExtendedLoader
{
	[HarmonyPatch]
	class CardSkinChangePatch
	{
		[HarmonyPatch(typeof(RencounterManager), nameof(RencounterManager.StartRencounter))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> RencounterManager_StartRencounter_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = instructions.ToList();
			var infoGetter = Method(typeof(BattleUnitView), nameof(BattleUnitView.GetCurrentSkinInfo));
			var modelField = Field(typeof(BattleUnitView), nameof(BattleUnitView.model));
			var helper = Method(typeof(CardSkinChangePatch), nameof(CardSkinChangePatch.TryFixCardSkinByView));
			bool foundLibrarian = false;
			bool foundEnemy = false;
			for (int i = 0; i < codes.Count - 3; i++)
			{
				if (codes[i + 1].Calls(infoGetter))
				{
					if (codes[i].opcode == Ldarg_2)
					{
						if (!foundLibrarian)
						{
							foundLibrarian = true;
							i++;
							for (; i < codes.Count - 1; i++)
							{
								if (codes[i].opcode == Ldarg_2 && codes[i + 1].LoadsField(modelField))
								{
									codes.InsertRange(i, new CodeInstruction[]
									{
										new CodeInstruction(Ldarg_2).MoveLabelsFrom(codes[i]),
										new CodeInstruction(Ldc_I4_0),
										new CodeInstruction(Call, helper)
									});
									i += 3;
									break;
								}
							}
							if (foundEnemy)
							{
								break;
							}
						}
					}
					else if (codes[i].opcode == Ldarg_1)
					{
						if (!foundEnemy)
						{
							foundEnemy = true;
							i++;
							for (; i < codes.Count - 1; i++)
							{
								if (codes[i].opcode == Ldarg_1 && codes[i + 1].LoadsField(modelField))
								{
									codes.InsertRange(i, new CodeInstruction[]
									{
										new CodeInstruction(Ldarg_1).MoveLabelsFrom(codes[i]),
										new CodeInstruction(Ldc_I4_1),
										new CodeInstruction(Call, helper)
									});
									i += 3;
									break;
								}
							}
							if (foundLibrarian)
							{
								break;
							}
						}
					}
				}
			}
			return codes;
		}

		[HarmonyPatch(typeof(BattleFarAreaPlayManager), nameof(BattleFarAreaPlayManager.StartFarAreaPlay))]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> BattleFarAreaPlayManager_StartFarAreaPlay_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = instructions.ToList();
			var cardField = Field(typeof(BattlePlayingCardDataInUnitModel), nameof(BattlePlayingCardDataInUnitModel.card));
			var heightGetter = Method(typeof(BattleDiceCardModel), nameof(BattleDiceCardModel.GetSkinHeight));
			var helper = Method(typeof(CardSkinChangePatch), nameof(CardSkinChangePatch.TryFixCardSkinByCard));
			for (int i = 0; i < codes.Count - 2; i++)
			{
				if (codes[i].opcode == Ldarg_1 && codes[i +1].LoadsField(cardField) && codes[i + 2].Calls(heightGetter))
				{
					codes.InsertRange(i, new CodeInstruction[]
					{
						new CodeInstruction(Ldarg_1).MoveLabelsFrom(codes[i]),
						new CodeInstruction(Call, helper)
					});
					break;
				}
			}
			return codes;
		}

		static void TryFixCardSkinByView(BattleUnitView unitView, bool fixNormal)
		{
			TryFixCardSkin(unitView, unitView.model.battleCardResultLog.usedCard, fixNormal);
		}

		static void TryFixCardSkinByCard(BattlePlayingCardDataInUnitModel card)
		{
			TryFixCardSkin(card.owner.view, card.card, false);
		}

		static void TryFixCardSkin(BattleUnitView view, BattleDiceCardModel card, bool fixNormal)
		{
			if (card.GetSkinType() != LOR_DiceSystem.CardSkinType.Normal)
			{
				return;
			}
			var skin = card.GetSkin();
			if (skin.StartsWith("Creature:"))
			{
				view.ChangeCreatureSkin(skin.Substring("Creature:".Length));
			}
			else if (fixNormal)
			{
				view.ChangeSkin(skin);
			}
		}
	}
}
