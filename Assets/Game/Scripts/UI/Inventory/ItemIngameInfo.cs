using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemIngameInfo : MonoBehaviour
{
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemContent;

    public ItemSpawner spawner;

    private void OnEnable()
    {
        ResetItemData();
    }

    public void UpdateItemData(ItemSlot itemSlot)
    {
        if (itemSlot.itemData != null)
        {
            itemName.text = itemSlot.itemData.itemName;

            switch (itemSlot.itemData.dataTyp)
            {
                case ItemData.DataTyp.ExtraData:
                    itemContent.text = itemSlot.itemData.slotData.decription;
                    break;

                default:
                    itemContent.text = itemSlot.itemData.CreateStetsString();
                    break;
            }
        }
    }

    public void ShowCurrentItemStetData()
    {
        if (spawner.itemSpawnManage.currentSelectedSlot != null)
        {
            itemName.text = spawner.itemSpawnManage.currentSelectedSlot.itemData.itemName;

            switch (spawner.itemSpawnManage.currentSelectedSlot.itemData.dataTyp)
            {
                case ItemData.DataTyp.ExtraData:

                    itemContent.text = spawner.itemSpawnManage.currentSelectedSlot.itemData.slotData.decription;
                    break;

                default:

                    itemContent.text = spawner.itemSpawnManage.currentSelectedSlot.itemData.CreateStetsString();
                    break;
            }
        } else
        {
            ResetItemData();
        }
    }

    public void UpgradeItemData()
    {
        if (spawner.itemSpawnManage.currentSelectedSlot != null)
        {
            itemName.text = spawner.itemSpawnManage.currentSelectedSlot.itemData.itemName;
            itemContent.text = spawner.itemSpawnManage.currentSelectedSlot.itemData.CreateUpgradeStetsString();
        }
    }

    public void ResetItemData()
    {
        itemName.text = "";
        itemContent.text = "";
    }
}
