using System.Collections.Generic;
using System.Xml.Serialization;

//NewXml
namespace GTMDProjectMoon
{
	[XmlType("GiftXmlRoot")]
	public class GiftXmlRoot_V2 : XmlRoot
	{
		[XmlElement("Gift")]
		public List<GiftXmlInfo_V2> giftXmlList;

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
					overrides.Add(typeof(GiftXmlInfo), nameof(GiftXmlInfo.ScriptList), ignore);
					_serializer = new XmlSerializer(typeof(GiftXmlRoot_V2), overrides);
				}
				return _serializer;
			}
		}
		static XmlSerializer _serializer;
	}
	public class GiftXmlInfo_V2 : GiftXmlInfo
	{
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
				OrcTools.GiftAndTitleDic[injectedId] = lorId;
			}
		}

		[XmlAttribute("Pid")]
		public string WorkshopId = "";

		[XmlElement("Passive")]
		public List<string> CustomScriptList = new List<string>();

		[XmlElement("PriorityOrder")]
		public GiftPriorityOrder priority = GiftPriorityOrder.Guest;

		[XmlIgnore]
		internal bool dontRemove;
	}

	public enum GiftPriorityOrder
	{
		Sephirah,
		Librarian,
		Guest,
		Creature
	}
}
