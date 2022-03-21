using GameSave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace ExtendedLoader
{
    public class LorName : IEquatable<LorName>, IComparable<LorName>
    {
        public LorName(string name)
        {
            if (name == null || !name.Contains(NAME_SEPARATOR))
            {
                this.name = name;
                packageId = "";
            }
            else
            {
                int pos = name.IndexOf(NAME_SEPARATOR);
                this.name = name.Substring(pos + 1);
                packageId = name.Substring(0, pos);
            }
        }

        public LorName(string name, bool isGenericCustom)
        {
            if (name == null || !name.Contains(NAME_SEPARATOR))
            {
                this.name = name;
                if (isGenericCustom)
                {
                    packageId = BASEMOD_ID;
                }
                else
                {
                    packageId = "";
                }
            }
            else
            {
                int pos = name.IndexOf(NAME_SEPARATOR);
                this.name = name.Substring(pos + 1);
                packageId = name.Substring(0, pos);
            }
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
        }

        public string Compress()
        {
            if (string.IsNullOrEmpty(packageId))
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
            return !IsWorkshop();
        }

        public bool IsWorkshop()
        {
            return !IsBasicId(packageId);
        }

        public bool IsNone()
        {
            return string.IsNullOrEmpty(name);
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
            int value = Singleton<SaveManager>.Instance.ConvertPackageIdToInt(packageId);
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
            return new LorName(Singleton<SaveManager>.Instance.ConvertIntToPackageId(intSelf), data2.GetStringSelf());
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
            return string.IsNullOrEmpty(packageId);
        }

        public static LorName MakeLorName(LorNameXml src, string defaultPid)
        {
            if (string.IsNullOrEmpty(src.pid))
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

        public override string ToString()
        {
            return string.Concat(new string[]
            {
            "LorName(",
            packageId.ToString(),
            ":",
            name.ToString(),
            ")"
            });
        }

        public readonly string name;

        public readonly string packageId;

        public static readonly LorName None = new LorName("");

        public const char NAME_SEPARATOR = ':';

        public const string BASEMOD_ID = "BaseMod";
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
}
