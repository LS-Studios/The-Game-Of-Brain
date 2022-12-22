using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using TMPro;

public class PackBagHandler : MonoBehaviour
{
    public ItemSpawner itemSpawner;
    public TextMeshProUGUI inventoryText;

    private EquipmentSlot itemSlotEquip;

    [Inject]
    private MainMenueHandler menueHandler;

    public void FilterByTyp(ItemData.ItemCategory packBackTyp)
    {
        List<GameObject> spawnList = new List<GameObject>(GameInstance.instance.inventoryValues.packbackInventory);

        itemSpawner.information.itemsToSpawn = spawnList;

        foreach (GameObject item in GameInstance.instance.inventoryValues.packbackInventory)
        {
            if (!item.GetComponent<ItemInfo>().ItemData.itemCategory.Equals(packBackTyp))
            {
                spawnList.Remove(item);
            }
        }

        inventoryText.text = LSUtils.SeperateCompundString(packBackTyp.ToString());

        itemSpawner.RefreshInventory();
    }

    public void PrepareSlotEquip(EquipmentSlot equipmentSlot)
    {
        itemSlotEquip = equipmentSlot;
    }

    public void EquipNewItem(ItemSlot itemSlot)
    {
        if (!GameInstance.instance.equipmentValues.CurrentSetContais(itemSlot.item))
        {
            itemSlotEquip.SetSlotData(itemSlot.item);
            itemSlotEquip.RefreshSlot();

            menueHandler.GoBackTo();
        } else
        {
            if (itemSlotEquip.itemSlot.item == itemSlot.item)
            {
                itemSlotEquip.SetSlotData(null);
                itemSlotEquip.RefreshSlot();

                menueHandler.GoBackTo();
            }
        }
    }
}
