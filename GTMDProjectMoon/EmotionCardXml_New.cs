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
		public static XmlSerializer Serializer
		{
			get
			{
				if (_serializer == null)
				{
					var ignore = new XmlAttributes
					{
						XmlIgnore = true
					};
					var overrides = new XmlAttributeOverrides();
					overrides.Add(typeof(EmotionCardXmlInfo), nameof(EmotionCardXmlInfo.Sephirah), ignore);
					_serializer = new XmlSerializer(typeof(EmotionCardXmlRoot_V2), overrides);
				}
				return _serializer;
			}
		}
		static XmlSerializer _serializer;
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
		public static XmlSerializer Serializer
		{
			get
			{
				if (_serializer == null)
				{
					var ignore = new XmlAttributes
					{
						XmlIgnore = true
					};
					var overrides = new XmlAttributeOverrides();
					overrides.Add(typeof(Sephirah), nameof(Sephirah.sephirahType), ignore);
					_serializer = new XmlSerializer(typeof(AbnormalityCardsRoot_V2), overrides);
				}
				return _serializer;
			}
		}
		static XmlSerializer _serializer;
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
