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
					_overrides.Add(typeof(ToolTipXmlInfo), nameof(ToolTipXmlInfo.ID), ignore);
				}
				return _overrides;
			}
		}
		static XmlAttributeOverrides _overrides;
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
