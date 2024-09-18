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
					overrides.Add(typeof(DiceCardXmlInfo), nameof(DiceCardXmlInfo.category), ignore);
					overrides.Add(typeof(DiceCardXmlInfo), nameof(DiceCardXmlInfo.optionList), ignore);
					overrides.Add(typeof(DiceCardXmlInfo), nameof(DiceCardXmlInfo.workshopID), new XmlAttributes { XmlIgnore = false, XmlAttribute = new XmlAttributeAttribute("Pid") });
#pragma warning disable CS0618 // Type or member is obsolete
					var unignore = new XmlAttributes
					{
						XmlIgnore = false
					};
					unignore.XmlElements.Add(new XmlElementAttribute("CustomCategory"));
					overrides.Add(typeof(DiceCardXmlInfo_V2), nameof(DiceCardXmlInfo_V2.customCategoryFallback), unignore);
#pragma warning restore CS0618 // Type or member is obsolete
					_serializer = new XmlSerializer(typeof(DiceCardXmlRoot_V2), overrides);
				}
				return _serializer;
			}
		}
		static XmlSerializer _serializer;
	}

	public class DiceCardXmlInfo_V2 : DiceCardXmlInfo
	{
		[XmlElement("Category")]
		public string customCategory;

		[Obsolete("Just use the Category tag; if multiple entries are desired, use the Option tag instead (for cardOptions)", false)]
		[XmlElement("CustomCategory")]
		public string customCategoryFallback;

		[XmlElement("Option")]
		public List<string> customOptionList = new List<string>();

		public void InitOldFields()
		{
			if (!string.IsNullOrWhiteSpace(customCategory))
			{
				category = BaseMod.Tools.MakeEnum<BookCategory>(customCategory);
			}
#pragma warning disable CS0618 // Type or member is obsolete
			else if (!string.IsNullOrWhiteSpace(customCategoryFallback))
			{
				category = BaseMod.Tools.MakeEnum<BookCategory>(customCategoryFallback);
			}
#pragma warning restore CS0618 // Type or member is obsolete
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