using System.Collections.Generic;
using System.Xml.Serialization;

namespace GTMDProjectMoon
{
	[XmlType("ToolTipXmlRoot")]
	public class ToolTipXmlRoot_V2
	{
		[XmlElement("ToolTip")]
		public List<ToolTipXmlInfo_V2> toolTipXmlList;

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
					overrides.Add(typeof(ToolTipXmlInfo), nameof(ToolTipXmlInfo.ID), ignore);
					_serializer = new XmlSerializer(typeof(ToolTipXmlRoot_V2), overrides);
				}
				return _serializer;
			}
		}
		static XmlSerializer _serializer;
	}

	public class ToolTipXmlInfo_V2 : ToolTipXmlInfo
	{
		[XmlAttribute("ID")]
		public string IDname;

		public void InitOldFields()
		{
			ID = string.IsNullOrWhiteSpace(IDname) ? ToolTipTarget.None : BaseMod.Tools.MakeEnum<ToolTipTarget>(IDname);
		}
	}
}
