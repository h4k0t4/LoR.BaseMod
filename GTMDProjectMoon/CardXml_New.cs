using LOR_DiceSystem;
using LOR_XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace GTMDProjectMoon
{
	[XmlType("DiceCardXmlRoot")]
	public class DiceCardXmlRoot_V2 : XmlRoot
	{
		[XmlElement("Card")]
		public List<DiceCardXmlInfo_V2> cardXmlList = new List<DiceCardXmlInfo_V2>();

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
					_overrides.Add(typeof(DiceCardXmlInfo), nameof(DiceCardXmlInfo.category), ignore);
					_overrides.Add(typeof(DiceCardXmlInfo), nameof(DiceCardXmlInfo.optionList), ignore);
					_overrides.Add(typeof(DiceCardXmlInfo), nameof(DiceCardXmlInfo.workshopID), new XmlAttributes { XmlIgnore = false, XmlAttribute = new XmlAttributeAttribute("Pid") });
				}
				return _overrides;
			}
		}
		static XmlAttributeOverrides _overrides;
	}

	public class DiceCardXmlInfo_V2 : DiceCardXmlInfo
	{
		[XmlElement("Category")]
		public string customCategory;

		[Obsolete]
		[XmlElement("CustomCategory")]
		public string customCategoryFallback;

		[XmlElement("Option")]
		public List<string> customOptionList = new List<string>();

		public void InitOldFields()
		{
#pragma warning disable CS0612 // Type or member is obsolete
			if (!string.IsNullOrWhiteSpace(customCategory))
			{
				category = BaseMod.Tools.MakeEnum<BookCategory>(customCategory);
			}
			else if (!string.IsNullOrWhiteSpace(customCategoryFallback))
			{
				category = BaseMod.Tools.MakeEnum<BookCategory>(customCategoryFallback);
			}
#pragma warning restore CS0612 // Type or member is obsolete
			optionList = customOptionList.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => BaseMod.Tools.MakeEnum<CardOption>(x)).ToList();
		}
	}

	[XmlType("BattleCardDescRoot")]
	public class BattleCardDescRoot_V2 : XmlRoot
	{
		[XmlArrayItem("BattleCardDesc")]
		public List<BattleCardDesc_V2> cardDescList;
	}

	public class BattleCardDesc_V2 : BattleCardDesc
	{
		[XmlAttribute("Pid")]
		public string workshopId;

		[XmlIgnore]
		public LorId lorId => new LorId(workshopId, cardID);
	}



	[Obsolete]
	public class DiceCardXmlRoot
	{
		[XmlElement("Card")]
		public List<DiceCardXmlInfo_New> cardXmlList;
	}
	[Obsolete]
	public class DiceCardXmlInfo_New : DiceCardXmlInfo
	{
		[XmlElement("CustomCategory")]
		public string customCategory = "";
	}
}