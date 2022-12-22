using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using System.Collections.Generic;

public class ItemInfoAction : MonoBehaviour
{
    public Image topIcon;
    public Image sideIcon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemStets;
    public TextMeshProUGUI itemDescription;
    public GameObject actionButton;

    private ItemSlot itemSlot;

    public void DisplayInfo(ItemSlot itemSlot)
    {
        this.itemSlot = itemSlot;

        if (GameInstance.instance.inventoryValues.packbackInventory.Contains(itemSlot.item))
        {
            actionButton.SetActive(false);
        } else
        {
            actionButton.SetActive(true);
        }

        topIcon.transform.localScale = itemSlot.itemData.slotData.slotImageScaleTop;
        topIcon.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, itemSlot.itemData.slotData.slotImageRotationTop));

        sideIcon.transform.localScale = itemSlot.itemData.slotData.slotImageScaleSide;
        sideIcon.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, itemSlot.itemData.slotData.slotImageRotationSide));

        ItemData itemData = itemSlot.itemData;

        if (itemData != null)
        {
            //Set top icon sprite
            if (itemData.slotData.itemImageTop != null)
                topIcon.sprite = itemData.slotData.itemImageTop;
            else
                topIcon.sprite = itemData.slotData.itemImageSide;
            topIcon.SetNativeSize();

            //Set side icon sprite
            sideIcon.sprite = itemData.slotData.itemImageSide;
            sideIcon.SetNativeSize();

            itemName.text = itemData.itemName;

            itemDescription.text = itemData.slotData.decription;

            string stets = itemData.CreateStetsString();

            stets += "\nCash price:\n" + itemData.cashPrice + "$";
            stets += "\nBraincell price:\n" + itemData.braincellPrice;

            itemStets.text = stets;

        }
    }

    public void BuyItem()
    {
        //Add the item to player inventory
        GameInstance.instance.inventoryValues.packbackInventory.Add(itemSlot.item);

        itemSlot.myItemSpawner.RefreshInventory();

        //Add bought weapon to current equipment set
        InventoryManage inventoryManage = (InventoryManage)itemSlot.myItemSpawner.itemSpawnManage;
        inventoryManage.FillEmptyGameInventorySlots();

        actionButton.GetComponent<GoToButton>().GoTo();
    }
}
