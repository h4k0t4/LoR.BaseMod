using System.Collections.Generic;
using HarmonyLib;

namespace ExtendedLoader
{
	[HarmonyPatch]
	internal class SephirahHeadPatch
	{
		[HarmonyPatch(typeof(CustomizingResourceLoader), nameof(CustomizingResourceLoader.CreateCustomizedAppearance))]
		[HarmonyPostfix]
		[HarmonyPriority(Priority.Low)]
		static void CustomizingResourceLoader_CreateCustomizedAppearance_Postfix(CustomizedAppearance __result)
		{
			if (__result is SpecialCustomizedAppearance special && special.list != null)
			{
				var index = special.list.FindIndex(head => head.detail == ActionDetail.Default);
				if (index > 0)
				{
					var x = special.list[index];
					special.list.RemoveAt(index);
					special.list.Insert(0, x);
				}
				switch (special.name)
				{
					case "Customized_Gebura(Clone)":
						FixHeadRedirect(special.list, ActionDetail.Penetrate, ActionDetail.Slash);
						return;
					case "Customized_Binah(Clone)":
						FixHeadRedirect(special.list, ActionDetail.Slash, ActionDetail.Hit);
						FixHeadRedirect(special.list, ActionDetail.Penetrate, ActionDetail.Hit);
						FixHeadRedirect(special.list, ActionDetail.Default, ActionDetail.Guard);
						FixHeadRedirect(special.list, ActionDetail.Evade, ActionDetail.Guard);
						return;
					case "Customized_Angela(Clone)":
						FixHeadRedirect(special.list, ActionDetail.Slash, ActionDetail.Hit);
						return;
				}
			}
		}

		static void FixHeadRedirect(List<SpecialCustomHead> headList, ActionDetail source, ActionDetail target)
		{
			var sourceHead = headList.Find(x => x.detail == source);
			if (sourceHead == null)
			{
				return;
			}
			if (sourceHead.replaceHead && headList.Exists(x => x.rootObject.name == sourceHead.replaceHead.name))
			{
				return;
			}
			var targetHead = headList.Find(x => x.detail == target);
			if (targetHead != null)
			{
				sourceHead.replaceHead = targetHead.rootObject;
			}
		}
	}
}
