using HarmonyLib;
using UI;

namespace ExtendedLoader
{
	[HarmonyPatch]
	class DestroyCharactersPatch
	{
		[HarmonyPatch(typeof(UICharacterRenderer), nameof(UICharacterRenderer.DestroyCharacters))]
		[HarmonyPostfix]
		static void UICharacterRenderer_DestroyCharacters_Postfix(UICharacterRenderer __instance)
		{
			foreach (var character in __instance.characterList)
			{
				if (!character.unitAppearance)
				{
					character.unitModel = null;
					character.resName = "";
				}
			}
		}
	}
}
