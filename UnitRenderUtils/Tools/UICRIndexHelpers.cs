using UI;

namespace ExtendedLoader
{
	internal class UICRIndexHelpers
	{
		public static int GetMaxWithoutSkip(UICharacterRenderer renderer)
		{
			int count = renderer.characterList.Count;
			if (count >= 11 && SkipCustomizationIndex())
			{
				count--;
			}
			return count;
		}

		public static int GetIndexWithSkip(int index)
		{
			if (index >= 10 && SkipCustomizationIndex())
			{
				index++;
			}
			return index;
		}

		public static bool SkipCustomizationIndex()
		{
			return StageController.Instance.State == StageController.StageState.Battle && GameSceneManager.Instance.battleScene.gameObject.activeSelf;
		}
	}
}
