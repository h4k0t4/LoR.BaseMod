using LOR_XML;
using System.Collections.Generic;
using System.Xml.Serialization;

//NewXml
namespace GTMDProjectMoon
{
	public class GiftXmlRoot
	{
		[XmlElement("Pid")]
		public string workshopID;

		[XmlElement("Gift")]
		public new List<GiftXmlInfo_New> giftXmlList;
	}
	public class GiftXmlInfo_New : GiftXmlInfo
	{
		[XmlIgnore]
		public LorId lorId
		{
			get
			{
				return new LorId(WorkshopId, id);
			}
		}

		[XmlAttribute("Pid")]
		public string WorkshopId = "";
	}
}
