using System.Collections.Generic;
using HarmonyLib;

namespace ExtendedLoader
{
	internal class SephirahHeadPatch
	{
		public static void FixSpecialCustomAll()
		{
			foreach (var value in CustomizingResourceLoader.Instance._specialCustomPrefabDic.Values)
			{
				FixSpecialCustom(value);
			}
		}

		public static void FixSpecialCustom(SpecialCustomizedAppearance specialAppearance)
		{
			if (specialAppearance.list != null)
			{
				var index = specialAppearance.list.FindIndex(head => head.detail == ActionDetail.Default);
				if (index > 0)
				{
					var x = specialAppearance.list[index];
					specialAppearance.list.RemoveAt(index);
					specialAppearance.list.Insert(0, x);
				}
				switch (specialAppearance.name)
				{
					case "Customized_Gebura":
						FixHeadRedirect(specialAppearance.list, ActionDetail.Penetrate, ActionDetail.Slash);
						return;
					case "Customized_Binah":
						FixHeadRedirect(specialAppearance.list, ActionDetail.Slash, ActionDetail.Hit);
						FixHeadRedirect(specialAppearance.list, ActionDetail.Penetrate, ActionDetail.Hit);
						FixHeadRedirect(specialAppearance.list, ActionDetail.Default, ActionDetail.Guard);
						FixHeadRedirect(specialAppearance.list, ActionDetail.Evade, ActionDetail.Guard);
						return;
					case "Customized_Angela":
						FixHeadRedirect(specialAppearance.list, ActionDetail.Slash, ActionDetail.Hit);
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
