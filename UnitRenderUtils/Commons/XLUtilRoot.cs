using System.Collections.Generic;
using UnityEngine;

namespace ExtendedLoader
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	public class XLUtilRoot : SingletonBehavior<XLUtilRoot>
	{
		static GameObject _persistentRoot;
		public static GameObject persistentRoot
		{
			get
			{
				if (_persistentRoot == null)
				{
					_persistentRoot = CreatePersistentRoot();
				}
				return _persistentRoot;
			}
		}

		internal static readonly Dictionary<string, int> coreThumbDic = new Dictionary<string, int>();
		internal static Dictionary<string, Sprite> extendedCoreThumbDic = new Dictionary<string, Sprite>();

		static GameObject CreatePersistentRoot()
		{
			GameObject root = new GameObject("ExtendedLoader_CoreUtils_PersistentRoot");
			DontDestroyOnLoad(root);
			root.AddComponent<XLUtilRoot>();
			return root;
		}

		void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
				return;
			}
		}
	}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
