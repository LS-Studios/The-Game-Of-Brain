using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using System.Linq;
using System.Reflection;

public class InventoryManage : ItemSpawnManage
{
    private CurrentItemHandler currentItemHandler;

    public InventoryManage(CurrentItemHandler currentItemHandler)
    {
        this.currentItemHandler = currentItemHandler;
    }

    public void FillEmptyGameInventorySlots()
    {
        EquipmentSet selectedEquipmentSet = GameInstance.instance.equipmentValues.currentEquipmentSet;
        List<GameObject> packbackInventory = GameInstance.instance.inventoryValues.packbackInventory;

        //Fill empty slots
        if (packbackInventory.Count > 0)
        {
            List<GameObject> GetItemsByCategory(ItemData.ItemCategory category)
            {
                List<GameObject> categoryItems = new List<GameObject>();
                foreach (GameObject item in packbackInventory)
                {
                    if (item.GetComponent<ItemInfo>().ItemData.itemCategory.Equals(category))
                    {
                        categoryItems.Add(item);
                    }
                }

                return categoryItems;
            }

            for (int i = 0; i < selectedEquipmentSet.setItems.Length; i++)
            {
                if (selectedEquipmentSet.setItems[i].item == null)
                {
                    foreach (GameObject item in GetItemsByCategory(selectedEquipmentSet.setItems[i].category))
                    {
                        if (!GameInstance.instance.equipmentValues.CurrentSetContais(item))
                        {
                            GameInstance.instance.equipmentValues.SetEquipmentSetValues(i, item, selectedEquipmentSet.setItems[i].category);
                            return;
                        }
                    }
                }
            }
        }
    }

    public bool AddEquipmentSetItem(GameObject itemToAdd)
    {
        EquipmentSet selectedEquipmentSet = GameInstance.instance.equipmentValues.currentEquipmentSet;

        ItemData.ItemCategory itemCategory = itemToAdd.GetComponent<ItemInfo>().ItemData.itemCategory;

        for (int i = 0; i <= 3; i++)
        {
            if (selectedEquipmentSet.setItems[i].item == null && selectedEquipmentSet.setItems[i].category.Equals(itemCategory))
            {
                selectedEquipmentSet.setItems[i].item = itemToAdd;

                return true;
            }
        }

        return false;
    }

    public void DropSelectedItem()
    {
        currentItemHandler.DropItem(currentSelectedSlot.item);
    }

    public void DropAllItems()
    {
        currentItemHandler.DropAllItems();
    }
}
