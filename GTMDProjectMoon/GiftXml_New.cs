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
					_overrides.Add(typeof(GiftXmlInfo), nameof(GiftXmlInfo.ScriptList), ignore);
				}
				return _overrides;
			}
		}
		static XmlAttributeOverrides _overrides;
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
