﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using LorIdExtensions;

//EmotionEgoXml_New Custom Ego 
namespace GTMDProjectMoon
{
	[XmlType("EmotionEgoXmlRoot")]
	public class EmotionEgoXmlRoot_V2 : XmlRoot
	{
		[XmlElement("EmotionEgo")]
		public List<EmotionEgoXmlInfo_V2> egoXmlList;

		[XmlIgnore]
		public static XmlAttributeOverrides Overrides
		{
			get
			{
				if (_overrides == null)
				{
					var ignore = new XmlAttributes
					{
						XmlIgnore = true
					};
					_overrides = new XmlAttributeOverrides();
					_overrides.Add(typeof(EmotionEgoXmlInfo), nameof(EmotionEgoXmlInfo.Sephirah), ignore);
					_overrides.Add(typeof(EmotionEgoXmlInfo), nameof(EmotionEgoXmlInfo._CardId), ignore);
					_overrides.Add(typeof(EmotionEgoXmlInfo), nameof(EmotionEgoXmlInfo.id), ignore);
				}
				return _overrides;
			}
		}
		static XmlAttributeOverrides _overrides;
	}
	public class EmotionEgoXmlInfo_V2 : EmotionEgoXmlInfo, IIdInjectable
	{
		public void InjectId(int injectedId)
		{
			if (id == 0)
			{
				id = injectedId;
				OrcTools.EgoDic[this] = lorCardId;
			}
		}

		[XmlElement("ID")]
		public LorIdXml lorIdXml;

		[XmlIgnore]
		public LorId lorId;

		[XmlElement("Sephirah")]
		public string SephirahName;

		[XmlElement("Card")]
		public LorIdXml lorCardIdXml;

		[XmlIgnore]
		public LorId lorCardId;

		public void InitOldFields(string packageId)
		{
			Sephirah = string.IsNullOrWhiteSpace(SephirahName) ? SephirahType.None : BaseMod.Tools.MakeEnum<SephirahType>(SephirahName);
			lorId = LorIdLegacy.MakeLorIdLegacy(lorIdXml, packageId);

			lorCardId = LorId.MakeLorId(lorCardIdXml, packageId);
		}
	}

	[Obsolete]
	public class EgoCardDetail
	{
		[XmlElement("ID")]
		public int id;

		[XmlElement("Sephirah")]
		public SephirahType Sephirah;

		[XmlElement("Card")]
		public LorIdXml _CardId;

		[XmlElement("LockInBattle")]
		public bool isLock;
	}
	[Obsolete]
	public class EmotionEgoXmlRoot
	{
		[XmlElement("EmotionEgo")]
		public List<EmotionEgoXmlInfo_New> egoXmlList;
	}
	[Obsolete]
	public class EmotionEgoXmlInfo_New
	{
		[XmlElement("ID")]
		public int id;

		[XmlElement("Sephirah")]
		public SephirahType Sephirah;

		[XmlElement("Card")]
		public LorIdXml _CardId;

		[XmlElement("LockInBattle")]
		public bool isLock;
	}
}
