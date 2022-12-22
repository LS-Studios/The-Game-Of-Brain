using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;
using System;
using UnityEngine.Events;

public class EquipmentSlot : MonoBehaviour
{
    public ItemData.ItemCategory itemCategory;
    public bool renameSlotTextToCategory = true;

    public PackBagHandler packBagHandler;

    public TextMeshProUGUI slotHeader;

    public ItemSlot itemSlot;

    public ClickEvents clickEvents;

    [Serializable]
    public class ClickEvents
    {

        [Serializable]
        public class ClickEvent : UnityEvent<EquipmentSlot>
        {
        }

        public ClickEvent clickEvent;

        [Serializable]
        public class DoubleClickEvent : UnityEvent<EquipmentSlot>
        {
        }

        public DoubleClickEvent doubleClickAction;
    }

    void Start()
    {
        RefreshSlot();
    }

    public void RefreshSlot()
    {
        if (renameSlotTextToCategory)
            slotHeader.text = LSUtils.SeperateCompundString(itemCategory.ToString());

        itemSlot.RefreshSlot();
        GameInstance.instance.equipmentValues.SetEquipmentSetValues(itemSlot.itemIndex, itemSlot.item, itemCategory);
    }

    public void SetSlotData(GameObject newItem)
    {
        itemSlot.item = newItem;

        if (newItem != null)
            itemSlot.itemData = newItem.GetComponent<ItemInfo>().ItemData;
        else
            itemSlot.itemData = null;

        itemSlot.RefreshSlot();
    }

    public void FilterPackBag()
    {
        packBagHandler.FilterByTyp(itemCategory);
    }
}
