using GameSave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace LorIdExtensions
{
	/// <summary>
	/// An equivalent of <see cref="LorId"/> for string names instead of int ids.
	/// </summary>
	public class LorName : IEquatable<LorName>, IComparable<LorName>
	{
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
		/// <summary>
		/// Creates a LorName given either a simple name (in which case the package id will be empty), or a compressed form of a LorName (in which case the package id will be recovered).
		/// </summary>
		/// <param name="name">The name to construct a LorName by.</param>
		public LorName(string name) : this(GetRealPackageId(name, "", out var innerName), innerName)
		{
		}

		/// <summary>
		/// Creates a "generic custom" LorName for addressing modded entities that nevertheless do not belong to any mod that has a package id.
		/// (most notably, standalone external skins)
		/// </summary>
		/// <param name="name">The name to construct a LorName by.</param>
		/// <param name="isGenericCustom"><see langword="true"/> to construct a "generic custom" LorName (if <see langword="false"/>, the default constructor is used).</param>
		public LorName(string name, bool isGenericCustom): this(GetRealPackageId(name, isGenericCustom ? WORKSHOP_ID : "", out var innerName), innerName)
		{
		}

		/// <summary>
		/// Creates a LorName given a package id and an internal name.
		/// NOTE: if the given name is already a compressed form of a LorName, its package id will be recovered and used instead!
		/// </summary>
		/// <param name="packageId">The package id.</param>
		/// <param name="name">The internal name.</param>
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

		/// <summary>
		/// Compresses a LorName into a single string of the form "packageId:internalName", or just the internal name if package id is empty.
		/// </summary>
		/// <returns>The compressed form of the LorName.</returns>
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

		/// <summary>
		/// Checks whether a given string is a valid compressed form of a LorName.
		/// That is, whether it is not <see langword="null"/> and contains the separator character ":".
		/// </summary>
		/// <param name="name">The name to be checked.</param>
		/// <returns><see langword="true"/> if the given name is a valid compressed form of a LorName, otherwise <see langword="false"/>.</returns>
		public static bool IsCompressed(string name)
		{
			return (name != null && name.Contains(NAME_SEPARATOR));
		}

		/// <summary>
		/// Tests whether the LorName is a "generic custom" identifier (see <see cref="LorName(string, bool)"/>).
		/// </summary>
		/// <returns><see langword="true"/> if this LorName is a "generic custom" LorName, otherwise <see langword="false"/>.</returns>
		public bool IsWorkshopGeneric()
		{
			return IsWorkshopGenericId(packageId);
		}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
		public bool IsNone()
		{
			return string.IsNullOrWhiteSpace(name);
		}

		public bool IsBasic()
		{
			return IsBasicId(packageId);
		}

		public bool IsWorkshop()
		{
			return !IsBasicId(packageId);
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

		public static bool IsBasicId(string packageId)
		{
			return string.IsNullOrWhiteSpace(packageId);
		}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

		/// <summary>
		/// Tests whether the given package id corresponds to a "generic custom" identifier (see <see cref="LorName(string, bool)"/>).
		/// </summary>
		/// <returns><see langword="true"/> if the given package id corresponds to a "generic custom" identifier, otherwise <see langword="false"/>.</returns>
		public static bool IsWorkshopGenericId(string packageId)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			return packageId == WORKSHOP_ID || packageId == BASEMOD_ID;
#pragma warning restore CS0618 // Type or member is obsolete
		}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
		public static void InitializeLorNames(List<LorNameXml> src, List<LorName> dst, string defaultPid)
		{
			dst.Clear();
			foreach (LorNameXml t in src)
			{
				dst.Add(MakeLorName(t, defaultPid));
			}
		}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

		/// <summary>
		/// Creates a LorName for "legacy" applications that need to default to empty package id on empty (instead of using the default id in that case).
		/// Thus, instead of substituting the default package id for empty/missing xml package id (as <see cref="LorId.MakeLorId(ILorIdXml, string)"/>/<see cref="LorName.MakeLorName(LorNameXml, string)"/> do),
		/// it is only substituted if the xml package id is "@this".
		/// (see also: <seealso cref="LorIdLegacy.MakeLorIdLegacy(ILorIdXml, string)"/>)
		/// </summary>
		/// <param name="src">The LorName xml from which to create a legacy LorName.</param>
		/// <param name="defaultPid">The "default" package id.</param>
		/// <returns>The created LorName.</returns>
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
		/// <summary>
		/// Initializes a list of LorName for "legacy" applications that need to default to empty package id on empty (instead of using the default id in that case).
		/// Thus, instead of substituting the default package id for empty/missing xml package id (as <see cref="LorId.MakeLorId(ILorIdXml, string)"/>/<see cref="LorName.MakeLorName(LorNameXml, string)"/> do),
		/// it is only substituted if the xml package id is "@this".
		/// (see also: <seealso cref="LorIdLegacy.InitializeLorIdsLegacy{T}(List{T}, List{LorId}, string)"/>)
		/// </summary>
		/// <param name="src">The list of LorName xmls from which to create legacy LorName values.</param>
		/// <param name="dst">The list of LorName into which the results should be placed.</param>
		/// <param name="defaultPid">The "default" package id.</param>
		public static void InitializeLorNamesLegacy(List<LorNameXml> src, List<LorName> dst, string defaultPid)
		{
			dst.Clear();
			foreach (LorNameXml t in src)
			{
				dst.Add(MakeLorNameLegacy(t, defaultPid));
			}
		}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

		[Obsolete("Only kept for backwards compatibility with old BMfW versions; please use WORKSHOP_ID instead", false)]
		static readonly string BASEMOD_ID = "BaseMod";

		static readonly string WORKSHOP_ID = "Workshop";
	}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

	/// <summary>
	/// The class for handling "legacy" LorId xml fields that preserve empty package id.
	/// </summary>
	public static class LorIdLegacy
	{
		/// <summary>
		/// Creates a LorId for "legacy" applications that need to default to empty package id on empty (instead of using the default id in that case).
		/// Thus, instead of substituting the default package id for empty/missing xml package id (as <see cref="LorId.MakeLorId(ILorIdXml, string)"/> does),
		/// it is only substituted if the xml package id is "@this".
		/// </summary>
		/// <param name="src">The LorId xml from which to create a legacy LorId.</param>
		/// <param name="defaultPid">The "default" package id.</param>
		/// <returns>The created LorId.</returns>
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

		/// <summary>
		/// Initializes a list of LorId for "legacy" applications that need to default to empty package id on empty (instead of using the default id in that case).
		/// Thus, instead of substituting the default package id for empty/missing xml package id (as <see cref="LorId.MakeLorId(ILorIdXml, string)"/> does),
		/// it is only substituted if the xml package id is "@this".
		/// </summary>
		/// <param name="src">The list of LorId xmls from which to create legacy LorId values.</param>
		/// <param name="dst">The list of LorId into which the results should be placed.</param>
		/// <param name="defaultPid">The "default" package id.</param>
		public static void InitializeLorIdsLegacy<T>(List<T> src, List<LorId> dst, string defaultPid) where T : ILorIdXml
		{
			dst.Clear();
			dst.AddRange(src.Select(x => MakeLorIdLegacy(x, defaultPid)));
		}
	}
}