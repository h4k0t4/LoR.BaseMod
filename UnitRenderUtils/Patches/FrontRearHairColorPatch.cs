using HarmonyLib;

namespace ExtendedLoader
{
	[HarmonyPatch]
	class FrontRearHairColorPatch
	{
		[HarmonyPatch(typeof(CustomizedAppearance), nameof(CustomizedAppearance.InitCustomData))]
		[HarmonyPostfix]
		[HarmonyPriority(Priority.High)]
		static void CustomizedAppearance_InitCustomData_Postfix(CustomizedAppearance __instance, UnitCustomizingData data)
		{
			if (__instance.backHair.Length > 2)
			{
				__instance.backHair[2].color = data.hairColor;
			}
		}
	}
}
