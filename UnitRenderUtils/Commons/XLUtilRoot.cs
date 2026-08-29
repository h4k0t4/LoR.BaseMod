using System;
using System.Collections.Generic;
using System.Linq;
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

		public static event Func<UnitDataModel, IEnumerable<LorId>> CustomWorkshopBooksForUnit = null;
		public static event Func<IEnumerable<LorId>> CustomWorkshopBooksForAll = null;


		static List<LorId> GetCustomWorkshopBooksForUnit(UnitDataModel unit)
		{
			var result = new List<LorId>();
			if (CustomWorkshopBooksForUnit == null)
			{
				return result;
			}
			foreach (var del in CustomWorkshopBooksForUnit.GetInvocationList())
			{
				try
				{
					result.AddRange(((Func<UnitDataModel, IEnumerable<LorId>>)del)(unit));
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}
			return result;
		}

		static List<LorId> GetCustomWorkshopBooksForAll()
		{
			var result = new List<LorId>();
			if (CustomWorkshopBooksForAll == null)
			{
				return result;
			}
			foreach (var del in CustomWorkshopBooksForAll.GetInvocationList())
			{
				try
				{
					result.AddRange(((Func<IEnumerable<LorId>>)del)());
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}
			return result;
		}

		public static List<LorId> GetAllCustomWorkshopBooks(UnitDataModel currentUnit)
		{
			var unitBooks = GetCustomWorkshopBooksForUnit(currentUnit).Distinct().OrderBy(id => id.packageId).ThenBy(id => id.id).ToList();
			var unitBookSet = new HashSet<LorId>(unitBooks);

			var allBooks = BookInventoryModel.Instance.GetIdList_noDuplicate().Concat(GetCustomWorkshopBooksForAll()).Where(x => unitBookSet.Add(x)).OrderBy(id => id.packageId).ThenBy(id => id.id);

			return unitBooks.Concat(allBooks).ToList();
		}
	}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}