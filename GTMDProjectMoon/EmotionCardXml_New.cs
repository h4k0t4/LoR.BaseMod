using LOR_XML;
using System.Collections.Generic;
using System.Xml.Serialization;

//NewXml
namespace GTMDProjectMoon
{
	[XmlType("EmotionCardXmlRoot")]
	public class EmotionCardXmlRoot_V2 : XmlRoot
	{
		[XmlElement("EmotionCard")]
		public List<EmotionCardXmlInfo_V2> emotionCardXmlList;

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
					_overrides.Add(typeof(EmotionCardXmlInfo), nameof(EmotionCardXmlInfo.Sephirah), ignore);
				}
				return _overrides;
			}
		}
		static XmlAttributeOverrides _overrides;
	}

	public class EmotionCardXmlInfo_V2 : EmotionCardXmlInfo, IIdInjectable
	{
		[XmlElement("Sephirah")]
		public string SephirahName;

		[XmlIgnore]
		public LorId lorId
		{
			get
			{
				return new LorId(WorkshopId, originalId ?? id);
			}
		}

		[XmlIgnore]
		int? originalId = null;

		public void InjectId(int injectedId)
		{
			if (originalId == null)
			{
				originalId = id;
				id = injectedId;
			}
		}

		[XmlAttribute("Pid")]
		public string WorkshopId = "";

		public void InitOldFields()
		{
			Sephirah = string.IsNullOrWhiteSpace(SephirahName) ? SephirahType.None : BaseMod.Tools.MakeEnum<SephirahType>(SephirahName);
		}
	}

	[XmlType("AbnormalityCardsRoot")]
	public class AbnormalityCardsRoot_V2
	{
		[XmlElement("Sephirah")]
		public List<Sephirah_V2> sephirahList;

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
					_overrides.Add(typeof(Sephirah), nameof(Sephirah.sephirahType), ignore);
				}
				return _overrides;
			}
		}
		static XmlAttributeOverrides _overrides;
	}

	public class Sephirah_V2 : Sephirah
	{
		[XmlAttribute("SephirahType")]
		public string sephirahName;

		public void InitOldFields()
		{
			sephirahType = string.IsNullOrWhiteSpace(sephirahName) ? SephirahType.None : BaseMod.Tools.MakeEnum<SephirahType>(sephirahName);
		}
	}
}
