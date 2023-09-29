using GameSave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace LorIdExtensions
{
	public class LorName : IEquatable<LorName>, IComparable<LorName>
	{
		public LorName(string name) : this(GetRealPackageId(name, "", out var innerName), innerName)
		{
		}

		static string GetRealPackageId(string name, string packageId, out string innerName)
		{
			if (name == null || !name.Contains(NAME_SEPARATOR))
			{
				innerName = name;
				return packageId;
			}
			else
			{
				int pos = name.IndexOf(NAME_SEPARATOR);
				innerName = name.Substring(pos + 1);
				return name.Substring(0, pos);
			}
		}

		public LorName(string name, bool isGenericCustom): this(GetRealPackageId(name, isGenericCustom ? WORKSHOP_ID : "", out var innerName), innerName)
		{
		}

		public LorName(string packageId, string name)
		{
			if (name == null || !name.Contains(NAME_SEPARATOR))
			{
				this.packageId = packageId;
				this.name = name;
				if (packageId == null)
				{
					Debug.LogError("LorName: packageId shouldn't be null!");
				}
			}
			else
			{
				int pos = name.IndexOf(NAME_SEPARATOR);
				this.name = name.Substring(pos + 1);
				this.packageId = name.Substring(0, pos);
			}
#pragma warning disable CS0618 // Type or member is obsolete
			if (this.packageId == BASEMOD_ID)
			{
				this.packageId = WORKSHOP_ID;
			}
#pragma warning restore CS0618 // Type or member is obsolete
		}

		public string Compress()
		{
			if (string.IsNullOrWhiteSpace(packageId))
			{
				return name;
			}
			else
			{
				return string.Concat(packageId, NAME_SEPARATOR, name);
			}
		}

		public static bool IsCompressed(string name)
		{
			return (name != null && name.Contains(NAME_SEPARATOR));
		}

		public bool IsBasic()
		{
			return IsBasicId(packageId);
		}

		public bool IsWorkshop()
		{
			return !IsBasicId(packageId);
		}

		public bool IsWorkshopGeneric()
		{
			return IsWorkshopGenericId(packageId);
		}

		public bool IsNone()
		{
			return string.IsNullOrWhiteSpace(name);
		}

		public override bool Equals(object obj)
		{
			LorName other;
			return !((other = (obj as LorName)) is null) && Equals(other);
		}

		public bool Equals(LorName other)
		{
			return name == other.name && packageId == other.packageId;
		}

		public bool Equals(string other)
		{
			return name == other && IsBasic();
		}

		public override int GetHashCode()
		{
			if (packageId == null)
			{
				Debug.LogError("error");
			}
			return name.GetHashCode() + packageId.GetHashCode();
		}

		public static bool operator ==(LorName lhs, string rhs)
		{
			if (lhs is null)
			{
				lhs = None;
			}
			return lhs.Equals(rhs);
		}

		public static bool operator !=(LorName lhs, string rhs)
		{
			return !(lhs == rhs);
		}

		public static bool operator ==(LorName lhs, LorName rhs)
		{
			if (lhs is null)
			{
				lhs = None;
			}
			if (rhs is null)
			{
				rhs = None;
			}
			return lhs.Equals(rhs);
		}

		public static bool operator !=(LorName lhs, LorName rhs)
		{
			return !(lhs == rhs);
		}

		public SaveData GetSaveData()
		{
			SaveData saveData = new SaveData();
			saveData.AddData("name", new SaveData(name));
			int value = SaveManager.Instance.ConvertPackageIdToInt(packageId);
			saveData.AddData("pid", new SaveData(value));
			return saveData;
		}

		public static LorName LoadFromSaveData(SaveData data)
		{
			if (data.GetDataType() == SaveDataType.String)
			{
				return new LorName(data.GetStringSelf());
			}
			SaveData data2 = data.GetData("name");
			SaveData data3 = data.GetData("pid");
			if (data2 == null)
			{
				return None;
			}
			int intSelf = data3.GetIntSelf();
			return new LorName(SaveManager.Instance.ConvertIntToPackageId(intSelf), data2.GetStringSelf());
		}

		public int CompareTo(LorName other)
		{
			int num = packageId.CompareTo(other.packageId);
			if (num == 0)
			{
				return name.CompareTo(other.name);
			}
			return num;
		}

		public static bool IsModId(string packageId)
		{
			return !IsBasicId(packageId);
		}

		public static bool IsWorkshopGenericId(string packageId)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			return packageId == WORKSHOP_ID || packageId == BASEMOD_ID;
#pragma warning restore CS0618 // Type or member is obsolete
		}

		public static bool IsBasicId(string packageId)
		{
			return string.IsNullOrWhiteSpace(packageId);
		}

		public static LorName MakeLorName(LorNameXml src, string defaultPid)
		{
			if (string.IsNullOrWhiteSpace(src.pid))
			{
				return new LorName(defaultPid, src.xmlName);
			}
			if (src.pid == "@origin")
			{
				return new LorName(src.xmlName);
			}
			return new LorName(src.pid, src.xmlName);
		}

		public static LorName MakeLorNameLegacy(LorNameXml src, string defaultPid)
		{
			if (string.IsNullOrWhiteSpace(src.pid) || src.pid == "@origin")
			{
				return new LorName(src.xmlName);
			}
			if (src.pid == "@this")
			{
				return new LorName(defaultPid, src.xmlName);
			}
			return new LorName(src.pid, src.xmlName);
		}

		public static void InitializeLorNames(List<LorNameXml> src, List<LorName> dst, string defaultPid)
		{
			dst.Clear();
			foreach (LorNameXml t in src)
			{
				dst.Add(MakeLorName(t, defaultPid));
			}
		}
		public static void InitializeLorNamesLegacy(List<LorNameXml> src, List<LorName> dst, string defaultPid)
		{
			dst.Clear();
			foreach (LorNameXml t in src)
			{
				dst.Add(MakeLorNameLegacy(t, defaultPid));
			}
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
			"LorName(",
			packageId,
			":",
			name,
			")"
			});
		}

		public readonly string name;

		public readonly string packageId;

		public static readonly LorName None = new LorName("");

		public const char NAME_SEPARATOR = ':';

		[Obsolete("Only kept for backwards compatibility with old BMfW versions; please use WORKSHOP_ID instead", false)]
		static readonly string BASEMOD_ID = "BaseMod";

		static readonly string WORKSHOP_ID = "Workshop";
	}

	public class LorNameXml
	{
		[XmlAttribute("Pid")]
		public string pid
		{
			get; set;
		}

		[XmlText]
		public string xmlName
		{
			get; set;
		}

		public LorNameXml()
		{
		}

		public LorNameXml(string pid, string name)
		{
			this.pid = pid;
			xmlName = name;
		}
	}

	public static class LorIdLegacy
	{
		public static LorId MakeLorIdLegacy(ILorIdXml src, string defaultPid)
		{
			if (string.IsNullOrEmpty(src.pid) || src.pid == "@origin")
			{
				return new LorId(src.xmlId);
			}
			if (src.pid == "@this")
			{
				return new LorId(defaultPid, src.xmlId);
			}
			return new LorId(src.pid, src.xmlId);
		}

		public static void InitializeLorIdsLegacy<T>(List<T> src, List<LorId> dst, string defaultPid) where T : ILorIdXml
		{
			dst.Clear();
			dst.AddRange(src.Select(x => MakeLorIdLegacy(x, defaultPid)));
		}
	}
}