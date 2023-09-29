using System.Collections.Generic;
using System.Xml.Serialization;

namespace GTMDProjectMoon
{
	[XmlType("TitleXmlRoot")]
	public class TitleXmlRoot_V2 : XmlRoot
	{
		[XmlElement("Prefix")]
		public TitleListRoot_V2 prefixXmlList;

		[XmlElement("Postfix")]
		public TitleListRoot_V2 postfixXmlList;
	}
	public class TitleListRoot_V2
	{
		[XmlElement("Title")]
		public List<TitleXmlInfo_V2> List;
	}

	public class TitleXmlInfo_V2 : TitleXmlInfo
	{
		[XmlIgnore]
		public LorId lorId
		{
			get
			{
				return new LorId(WorkshopId, originalId ?? ID);
			}
		}

		[XmlIgnore]
		int? originalId = null;

		public void InjectId(int injectedId)
		{
			if (originalId == null)
			{
				originalId = ID;
				ID = injectedId;
				OrcTools.GiftAndTitleDic[injectedId] = lorId;
			}
		}

		[XmlAttribute("Pid")]
		public string WorkshopId = "";
	}
}
