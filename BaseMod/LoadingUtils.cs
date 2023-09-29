using GTMDProjectMoon;
using System;
using System.Collections.Generic;

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

	}

	class SplitTrackerDict<TSplitter, TValue> : Dictionary<TSplitter, TrackerDict<TValue>>
	{

	}

	static class OptimizedReplacer
	{
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
			foreach (var tracker in dict.Values)
			{
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
			foreach (var tracker in dict.Values)
			{
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
			foreach (var kvp in splitDict)
			{
				if (!originSplitDict.TryGetValue(kvp.Key, out var originList))
				{
					originSplitDict[kvp.Key] = originList = new List<TBase>();
				}
				AddOrReplace(kvp.Value, originList, idSelector, predicate);
			}
		}
		internal static void AddOrReplace<TSplitter, TElement, TBase>(SplitTrackerDict<TSplitter, TElement> splitDict, Dictionary<TSplitter, List<TBase>> originSplitDict, Func<TBase, int> idSelector, Predicate<TElement> predicate = null) where TElement : TBase
		{
			foreach (var kvp in splitDict)
			{
				if (!originSplitDict.TryGetValue(kvp.Key, out var originList))
				{
					originSplitDict[kvp.Key] = originList = new List<TBase>();
				}
				AddOrReplace(kvp.Value, originList, idSelector, predicate);
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
			foreach (var kvp in splitDict)
			{
				foreach (var tracker in kvp.Value.Values)
				{
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
			foreach (var subDict in splitDict.Values)
			{
				foreach (var kvp in subDict)
				{
					if (!kvp.Value.added)
					{
						if (kvp.Key.IsBasic())
						{
							var element = kvp.Value.element;
							if (predicate == null || predicate(element))
							{
								element.InjectId(kvp.Key.id);
								originSplitList.Add(element);
								addAction?.Invoke(element);
							}
						}
					}
				}
				foreach (var kvp in subDict)
				{
					if (kvp.Value.added)
					{
						kvp.Value.added = false;
					}
					else
					{
						if (kvp.Key.IsWorkshop())
						{
							var element = kvp.Value.element;
							if (predicate == null || predicate(element))
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
			foreach (var kvp in dict)
			{
				if (!kvp.Value.added)
				{
					if (kvp.Key.IsBasic())
					{
						var element = kvp.Value.element;
						if (predicate == null || predicate(element))
						{
							element.InjectId(kvp.Key.id);
							originList.Add(element);
							addAction?.Invoke(element);
						}
					}
				}
			}
			foreach (var kvp in dict)
			{
				if (kvp.Value.added)
				{
					kvp.Value.added = false;
				}
				else
				{
					if (kvp.Key.IsWorkshop())
					{
						var element = kvp.Value.element;
						if (predicate == null || predicate(element))
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
