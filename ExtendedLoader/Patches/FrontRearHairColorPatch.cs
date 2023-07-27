using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace ExtendedLoader.Patches
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
