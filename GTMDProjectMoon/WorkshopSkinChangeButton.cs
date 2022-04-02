using System.Collections.Generic;
using UnityEngine;

namespace GTMDProjectMoon
{
    public class WorkshopSkinChangeButton : MonoBehaviour
    {
        private void Update()
        {
            if (!(BaseMod.Harmony_Patch.uiEquipPageCustomizePanel == null) && BaseMod.Harmony_Patch.uiEquipPageCustomizePanel.isActiveAndEnabled && !BaseMod.Harmony_Patch.uiEquipPageCustomizePanel.isWorkshop && Input.GetKeyDown(KeyCode.Tab))
            {
                BaseMod.Harmony_Patch.isModWorkshopSkin = !BaseMod.Harmony_Patch.isModWorkshopSkin;
                if (!BaseMod.Harmony_Patch.isModWorkshopSkin)
                {
                    BaseMod.Harmony_Patch.uiEquipPageCustomizePanel.Init(Singleton<CustomCoreBookInventoryModel>.Instance.GetBookIdList_CustomCoreBook(BaseMod.Harmony_Patch.uiEquipPageCustomizePanel.panel.Parent.SelectedUnit.OwnerSephirah, BaseMod.Harmony_Patch.uiEquipPageCustomizePanel.panel.Parent.SelectedUnit.isSephirah), false);
                    BaseMod.Harmony_Patch.uiEquipPageCustomizePanel.UpdateList();
                }
                else
                {
                    BaseMod.Harmony_Patch.ModWorkshopBookIndex = new Dictionary<int, LorId>();
                    int num = 0;
                    List<int> list = new List<int>();
                    foreach (LorId lorId in Singleton<BookInventoryModel>.Instance.GetIdList_noDuplicate())
                    {
                        BookXmlInfo info = Singleton<BookXmlList>.Instance.GetData(lorId);
                        if (lorId.IsWorkshop() && !info.isError && !info.canNotEquip)
                        {
                            BaseMod.Harmony_Patch.ModWorkshopBookIndex.Add(num, lorId);
                            list.Add(num);
                            num++;
                        }
                    }
                    BaseMod.Harmony_Patch.uiEquipPageCustomizePanel.Init(list, false);
                    BaseMod.Harmony_Patch.uiEquipPageCustomizePanel.UpdateList();
                }
            }
        }
    }
}
