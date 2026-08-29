using GTMDProjectMoon;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseMod
{
	class AddTracker<T>
	{
		public T element;
		public bool added;
		public AddTracker(T element)
		{
			this.element = element;
		}
	}

	class TrackerDict<TValue> : Dictionary<LorId, AddTracker<TValue>>
	{
		public IEnumerable<LorId> SortedKeys => Keys.OrderBy(x => x, OptimizedReplacer.packageSortedComp);
	}

	class SplitTrackerDict<TSplitter, TValue> : Dictionary<TSplitter, TrackerDict<TValue>>
	{
		public readonly IComparer<TSplitter> splitterSorter;

		public SplitTrackerDict(Comparison<TSplitter> splitterSorter) : base()
		{
			this.splitterSorter = Comparer<TSplitter>.Create(splitterSorter);
		}

		public SplitTrackerDict(IComparer<TSplitter> splitterSorter) : base()
		{
			this.splitterSorter = splitterSorter;
		}

		public IEnumerable<TSplitter> SortedKeys => Keys.OrderBy(x => x, splitterSorter);
	}

	static class OptimizedReplacer
	{
		public static readonly IComparer<LorId> packageSortedComp = Comparer<LorId>.Create((x, y) =>
		{
			int result = (x.packageId ?? "").CompareTo(y.packageId ?? "");
			if (result != 0)
			{
				return result;
			}
			return x.id - y.id;
		});

		public static readonly IComparer<SephirahType> sephirahComp = Comparer<SephirahType>.Create((x, y) =>
		{
			bool isXOriginal = x >= SephirahType.None && x <= SephirahType.ETC;
			bool isYOriginal = y >= SephirahType.None && y <= SephirahType.ETC;
			if (isXOriginal)
			{
				if (isYOriginal)
				{
					return x.CompareTo(y);
				}
				return -1;
			}
			else
			{
				if (isYOriginal)
				{
					return 1;
				}
				return x.ToString().CompareTo(y.ToString());
			}
		});

		internal static void AddOrReplace<TElement, TBase>(TrackerDict<TElement> dict, List<TBase> originList, Func<TBase, LorId> idSelector, Predicate<TElement> predicate = null) where TElement : TBase
		{
			for (int i = 0; i < originList.Count; i++)
			{
				var id = idSelector(originList[i]);
				if (dict.TryGetValue(id, out var tracker) && (predicate == null || predicate(tracker.element)))
				{
					originList[i] = tracker.element;
					tracker.added = true;
				}
			}
			foreach (var key in dict.SortedKeys)
			{
				var tracker = dict[key];
				if (tracker.added)
				{
					tracker.added = false;
				}
				else if (predicate == null || predicate(tracker.element))
				{
					originList.Add(tracker.element);
				}
			}
		}
		internal static void AddOrReplace<TElement, TBase>(TrackerDict<TElement> dict, List<TBase> originList, Func<TBase, int> idSelector, Predicate<TElement> predicate = null) where TElement : TBase
		{
			for (int i = 0; i < originList.Count; i++)
			{
				var id = new LorId(idSelector(originList[i]));
				if (dict.TryGetValue(id, out var tracker) && (predicate == null || predicate(tracker.element)))
				{
					originList[i] = tracker.element;
					tracker.added = true;
				}
			}
			foreach (var key in dict.SortedKeys)
			{
				var tracker = dict[key];
				if (tracker.added)
				{
					tracker.added = false;
				}
				else if (predicate == null || predicate(tracker.element))
				{
					originList.Add(tracker.element);
				}
			}
		}
		internal static void AddOrReplace<TSplitter, TElement, TBase>(SplitTrackerDict<TSplitter, TElement> splitDict, Dictionary<TSplitter, List<TBase>> originSplitDict, Func<TBase, LorId> idSelector, Predicate<TElement> predicate = null) where TElement : TBase
		{
			foreach (var key in splitDict.SortedKeys)
			{
				if (!originSplitDict.TryGetValue(key, out var originList))
				{
					originSplitDict[key] = originList = new List<TBase>();
				}
				AddOrReplace(splitDict[key], originList, idSelector, predicate);
			}
		}
		internal static void AddOrReplace<TSplitter, TElement, TBase>(SplitTrackerDict<TSplitter, TElement> splitDict, Dictionary<TSplitter, List<TBase>> originSplitDict, Func<TBase, int> idSelector, Predicate<TElement> predicate = null) where TElement : TBase
		{
			foreach (var key in splitDict.SortedKeys)
			{
				if (!originSplitDict.TryGetValue(key, out var originList))
				{
					originSplitDict[key] = originList = new List<TBase>();
				}
				AddOrReplace(splitDict[key], originList, idSelector, predicate);
			}
		}
		internal static void AddOrReplace<TSplitter, TElement, TBase>(SplitTrackerDict<TSplitter, TElement> splitDict, List<TBase> originSplitList, Func<TBase, int> idSelector, Func<TBase, TSplitter> splitSelector, Predicate<TElement> predicate = null) where TElement : TBase
		{
			for (int i = 0; i < originSplitList.Count; i++)
			{
				var id = new LorId(idSelector(originSplitList[i]));
				var splitter = splitSelector(originSplitList[i]);
				if (splitDict.TryGetValue(splitter, out var subDict) && subDict.TryGetValue(id, out var tracker) && (predicate == null || predicate(tracker.element)))
				{
					originSplitList[i] = tracker.element;
					tracker.added = true;
				}
			}
			foreach (var key in splitDict.SortedKeys)
			{
				var dict = splitDict[key];
				foreach (var subkey in dict.SortedKeys)
				{
					var tracker = dict[subkey];
					if (tracker.added)
					{
						tracker.added = false;
					}
					else if (predicate == null || predicate(tracker.element))
					{
						originSplitList.Add(tracker.element);
					}
				}
			}
		}
		internal static void AddOrReplaceWithInject<TSplitter, TElement, TBase>(SplitTrackerDict<TSplitter, TElement> splitDict, List<TBase> originSplitList, Func<TBase, int> idSelector, Func<TBase, TSplitter> splitSelector, Func<TElement, int> injectIdFunc, Action<TElement> addAction = null, Predicate<TElement> predicate = null) where TElement : TBase, IIdInjectable
		{
			for (int i = 0; i < originSplitList.Count; i++)
			{
				var id = new LorId(idSelector(originSplitList[i]));
				var splitter = splitSelector(originSplitList[i]);
				if (splitDict.TryGetValue(splitter, out var subDict) && subDict.TryGetValue(id, out var tracker) && (predicate == null || predicate(tracker.element)))
				{
					originSplitList[i] = tracker.element;
					tracker.added = true;
				}
			}
			foreach (var key in splitDict.SortedKeys)
			{
				var subDict = splitDict[key];
				foreach (var subkey in subDict.SortedKeys)
				{
					var tracker = subDict[subkey];
					if (tracker.added)
					{
						tracker.added = false;
					}
					else
					{
						var element = tracker.element;
						if (predicate == null || predicate(element))
						{
							if (subkey.IsBasic())
							{
								element.InjectId(subkey.id);
								originSplitList.Add(element);
								addAction?.Invoke(element);
							}
							else
							{
								element.InjectId(injectIdFunc(element));
								originSplitList.Add(element);
							}
						}
					}
				}
			}
		}
		internal static void AddOrReplaceWithInject<TElement, TBase>(TrackerDict<TElement> dict, List<TBase> originList, Func<TBase, int> idSelector, Func<TElement, int> injectIdFunc, Action<TElement> addAction = null, Predicate<TElement> predicate = null) where TElement : TBase, IIdInjectable
		{
			for (int i = 0; i < originList.Count; i++)
			{
				var id = new LorId(idSelector(originList[i]));
				if (dict.TryGetValue(id, out var tracker) && (predicate == null || predicate(tracker.element)))
				{
					originList[i] = tracker.element;
					tracker.added = true;
				}
			}
			foreach (var key in dict.SortedKeys)
			{
				var tracker = dict[key];
				if (tracker.added)
				{
					tracker.added = false;
				}
				else
				{
					var element = tracker.element;
					if (predicate == null || predicate(element))
					{
						if (key.IsBasic())
						{
							element.InjectId(key.id);
							originList.Add(element);
							addAction?.Invoke(element);
						}
						else
						{
							element.InjectId(injectIdFunc(element));
							originList.Add(element);
						}
					}
				}
			}
		}
		internal static void AddOrReplace<TElement, TBase>(TrackerDict<TElement> dict, Dictionary<LorId, TBase> originDict, Func<TBase, LorId> idSelector, Predicate<TElement> predicate = null) where TElement : TBase
		{
			foreach (var tracker in dict.Values)
			{
				if (predicate == null || predicate(tracker.element))
				{
					originDict[idSelector(tracker.element)] = tracker.element;
				}
			}
		}
		internal static void AddOrReplace<TElement, TBase>(TrackerDict<TElement> dict, Dictionary<int, TBase> originDict, Func<TBase, int> idSelector, Predicate<TElement> predicate = null) where TElement : TBase
		{
			foreach (var tracker in dict.Values)
			{
				if (predicate == null || predicate(tracker.element))
				{
					originDict[idSelector(tracker.element)] = tracker.element;
				}
			}
		}
	}
}
