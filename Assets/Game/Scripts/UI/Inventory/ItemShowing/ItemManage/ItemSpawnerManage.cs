using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using System.Linq;
using System.Reflection;

public class ItemSpawnManage
{
    public ItemSlot currentSelectedSlot;

    internal ItemSpawner itemSpawner;

    public enum SortTyp { Name, Damage, CurrentAmmo, MaxAmmo }
    public enum SortDirection { Up, Down }

    public void ItemSlotChange(ItemSlot selectedSlot)
    {
        //Change item       
        if (currentSelectedSlot != null && currentSelectedSlot != selectedSlot)
        {
            ChangeItem(currentSelectedSlot, selectedSlot);
        }

        //Select new slot
        else
        {
            SelectSlot(selectedSlot);
        }
    }

    public void SelectSlot(ItemSlot selectedSlot)
    {
        //Double click
        if (currentSelectedSlot != null && selectedSlot.itemIndex == currentSelectedSlot.itemIndex && selectedSlot.myItemSpawner == currentSelectedSlot.myItemSpawner)
        {
            selectedSlot.myItemSpawner.clickEvents.doubleClickAction.Invoke(selectedSlot);

            if (itemSpawner.information.useEquipmentSet)
            {
                EquipmentSlot equipmentSlot = itemSpawner.transform.GetChild(selectedSlot.itemIndex).GetComponent<EquipmentSlot>();
                equipmentSlot.clickEvents.doubleClickAction?.Invoke(equipmentSlot);
            }

            DeSelectSlot();

            return;
        }
        else if (currentSelectedSlot != null)
            currentSelectedSlot.GetComponentInChildren<Image>().color = selectedSlot.myItemSpawner.references.globalData.itemSlot.GetComponentInChildren<Image>().color;

        currentSelectedSlot = selectedSlot;
        currentSelectedSlot.GetComponentInChildren<Image>().color = new Color(255, 255, 255);
    }

    public void ReSelectSlot(ItemSlot selectedSlot)
    {
        currentSelectedSlot = selectedSlot;
        currentSelectedSlot.GetComponentInChildren<Image>().color = new Color(255, 255, 255);
    }

    public void DeSelectSlot()
    {
        if (currentSelectedSlot != null)
        {
            currentSelectedSlot.GetComponentInChildren<Image>().color = currentSelectedSlot.myItemSpawner.references.globalData.itemSlot.GetComponentInChildren<Image>().color;

            currentSelectedSlot = null;
        }
    }

    public void UpgradeSelectedItem()
    {
        currentSelectedSlot.itemData.UpgradeItem();
    }

    public void ChangeItem(ItemSlot currentItemSlot, ItemSlot changeSlot)
    {
        ItemSpawner currentSpawner = currentItemSlot.myItemSpawner;
        ItemSpawner changeSpawner = changeSlot.myItemSpawner;

        //Current selected item index
        int currentSelectedItemIndex = currentItemSlot.itemIndex;

        //Now selected item index
        int nowSelectedItemIndex = changeSlot.itemIndex;

        //Replace current item
        currentSpawner.information.itemsToSpawn[currentSelectedItemIndex] = changeSlot.item;

        //Replace selected item
        changeSpawner.information.itemsToSpawn[nowSelectedItemIndex] = currentItemSlot.item;

        //Refresh
        currentItemSlot.myItemSpawner.RefreshInventory();
        changeSlot.myItemSpawner.RefreshInventory();
    }

    public void SortItems(bool sortEquipment, string variableToSort, ValueTuple<SortTyp, SortDirection> sortProperty)
    {
        List<GameObject> sorted = new List<GameObject>();

        switch (sortProperty)
        {
            case (SortTyp.Damage, SortDirection.Down):
                sorted = itemSpawner.information.itemsToSpawn.OrderBy(w => w.GetComponent<ItemInfo>().ItemData.damage).ToList();
                sorted.Reverse();
                break;
            case (SortTyp.Damage, SortDirection.Up):
                sorted = itemSpawner.information.itemsToSpawn.OrderBy(w => w.GetComponent<ItemInfo>().ItemData.damage).ToList();
                break;

            case (SortTyp.CurrentAmmo, SortDirection.Down):
                sorted = itemSpawner.information.itemsToSpawn.OrderBy(w => w.GetComponent<ItemInfo>().ItemData.weaponData.currentAmmo).ToList();
                sorted.Reverse();
                break;
            case (SortTyp.CurrentAmmo, SortDirection.Up):
                sorted = itemSpawner.information.itemsToSpawn.OrderBy(w => w.GetComponent<ItemInfo>().ItemData.weaponData.currentAmmo).ToList();
                break;

            case (SortTyp.MaxAmmo, SortDirection.Down):
                sorted = itemSpawner.information.itemsToSpawn.OrderBy(w => w.GetComponent<ItemInfo>().ItemData.weaponData.maxAmmo).ToList();
                sorted.Reverse();
                break;
            case (SortTyp.MaxAmmo, SortDirection.Up):
                sorted = itemSpawner.information.itemsToSpawn.OrderBy(w => w.GetComponent<ItemInfo>().ItemData.weaponData.maxAmmo).ToList();
                break;

            case (SortTyp.Name, SortDirection.Down):
                sorted = itemSpawner.information.itemsToSpawn.OrderBy(w => w.GetComponent<ItemInfo>().ItemData.name).ToList();
                sorted.Reverse();
                break;
            case (SortTyp.Name, SortDirection.Up):
                sorted = itemSpawner.information.itemsToSpawn.OrderBy(w => w.GetComponent<ItemInfo>().ItemData.name).ToList();
                break;
        }

        ValueTuple<object, FieldInfo, object> value = (null, null, null);

        if (sortEquipment)
            value = LSUtils.GetClassFieldProperties(new List<object> { GameInstance.instance.equipmentValues.currentEquipmentSet }, variableToSort);
        else
            value = LSUtils.GetClassFieldProperties(new List<object> { itemSpawner.information }, "itemsToSpawn");

        value.Item2.SetValue(value.Item1, sorted);

        itemSpawner.RefreshInventory();
    }
}
