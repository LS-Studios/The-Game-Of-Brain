using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentSet
{
    public string setName;

    public SetItem[] setItems;

    [Serializable]
    public struct SetItem {
        public GameObject item;
        public ItemData.ItemCategory category;
    }

    public EquipmentSet(string name, SetItem[] items)
    {
        setName = name;
        setItems = items;
    }
}
