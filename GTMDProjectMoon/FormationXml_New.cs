using System.Collections.Generic;
using System.Xml.Serialization;

//NewXml
namespace GTMDProjectMoon
{
	[XmlType("FormationXmlRoot")]
	public class FormationXmlRoot_V2 : XmlRoot
	{
		[XmlElement("Formation")]
		public List<FormationXmlInfo_V2> list;
	}

	public class FormationXmlInfo_V2 : FormationXmlInfo, IIdInjectable
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
			}
		}

		[XmlAttribute("Pid")]
		public string WorkshopId = "";
	}
}
