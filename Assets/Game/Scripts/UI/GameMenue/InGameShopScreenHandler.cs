using Zenject;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InGameShopScreenHandler : MonoBehaviour
{
    public TextMeshProUGUI gamPointText;

    public SlideButton buyButton;
    public GameObject extraInventory;

    public List<ScrollRect> scrollRects;

    private PlayerHandler playerHandler;

    private GameObject stackItem;

    public CurrentItemHandler currentItemHandler;

    private void OnEnable()
    {
        scrollRects.ForEach(scroll => scroll.normalizedPosition = new Vector2(0, 0));
    }

    private void Start()
    {
        playerHandler = FindObjectOfType<PlayerHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameInstance.instance.inGameValues.GamePoints > 99999999999999999)
        {
            gamPointText.text = 99999999999999999.ToString();
            GameInstance.instance.inGameValues.GamePoints = 99999999999999999;
        }
        else
        {
            gamPointText.text = GameInstance.instance.inGameValues.GamePoints.ToString();
        }

        var currentSelectedSlot = extraInventory.GetComponentInChildren<ItemSpawner>().itemSpawnManage.currentSelectedSlot;

        if (currentSelectedSlot != null)
        {
            buyButton.CanSlide = true;
            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buy";
            buyButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 7.5f;

            ItemData selectedItemData = currentSelectedSlot.itemData;

            if (selectedItemData != null)
            {
                if (selectedItemData.gamePointPrice > GameInstance.instance.inGameValues.GamePoints)
                {
                    buyButton.CanSlide = false;

                    buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Not enought GP";

                    buyButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 5.3f;
                }
                else
                {
                    //Is no extra for example trap or grenade
                    //if (selectedItemData.dataTyp != ItemData.DataTyp.Extra)
                    //{
                    //    stackItem = currentItemHandler.useableItems.Find(
                    //                        item => item.GetComponent<ItemInfo>().ItemData.ID == selectedItemData.ID &&
                    //                        item.GetComponent<ItemInfo>().ItemData.currentAmmount < item.GetComponent<ItemInfo>().ItemData.maxAmmount);

                    //    if (stackItem != null)
                    //    {
                    //        ItemData stackItemData = stackItem.GetComponent<ItemInfo>().ItemData;

                    //        if (stackItemData.currentAmmount + 1 > stackItemData.maxAmmount)
                    //        {
                    //            buyButton.CanSlide = false;

                    //            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "No space";
                    //        }
                    //    }
                    //    else if (stackItem == null && currentItemHandler.useableItems.Count == 4)
                    //    {
                    //        buyButton.CanSlide = false;

                    //        buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "No space";
                    //    }
                    //}

                    //Is Extra
                    if (selectedItemData.dataTyp == ItemData.DataTyp.ExtraData)
                    {
                        WeaponBase currentWeaponBase = null;

                        switch (selectedItemData.extraData.extraTyp)
                        {
                            case ItemData.ExtraData.ExtraTyp.AllAmmo:
                                bool anyWeaponNeedAmmo = false;
                                foreach (GameObject item in currentItemHandler.useableItems)
                                {
                                    currentWeaponBase = item.GetComponent<WeaponBase>();
                                    if (currentWeaponBase != null && currentWeaponBase.itemData != null && currentWeaponBase.NeedAmmo())
                                    {
                                        anyWeaponNeedAmmo = true;
                                        break;
                                    }
                                }

                                if (!anyWeaponNeedAmmo)
                                {
                                    buyButton.CanSlide = false;

                                    buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Full";
                                }
                                break;

                            case ItemData.ExtraData.ExtraTyp.CurrentAmmo:
                                currentWeaponBase = null;

                                if (currentItemHandler.CurrentItem != null)
                                    currentWeaponBase = currentItemHandler.CurrentItem.GetComponent<WeaponBase>();

                                if (currentWeaponBase != null)
                                {
                                    if (!currentWeaponBase.NeedAmmo())
                                    {
                                        buyButton.CanSlide = false;

                                        buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Full";
                                    }
                                } 
                                else
                                {
                                    buyButton.CanSlide = false;

                                    buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "No Item";
                                }
                                break;

                            case ItemData.ExtraData.ExtraTyp.AllHealth:
                                if (playerHandler.healthComponent.currentHealth >= playerHandler.healthComponent.maxHealth)
                                {
                                    buyButton.CanSlide = false;

                                    buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Full";
                                }
                                break;

                            case ItemData.ExtraData.ExtraTyp.MiddleHealth:
                                if (playerHandler.healthComponent.currentHealth >= playerHandler.healthComponent.maxHealth)
                                {
                                    buyButton.CanSlide = false;

                                    buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Full";
                                }
                                break;

                            case ItemData.ExtraData.ExtraTyp.SmallHealth:
                                if (playerHandler.healthComponent.currentHealth >= playerHandler.healthComponent.maxHealth)
                                {
                                    buyButton.CanSlide = false;

                                    buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Full";
                                }
                                break;

                            case ItemData.ExtraData.ExtraTyp.NewGrenades:
                                bool haveGrenadeInSet = false;

                                foreach (EquipmentSet.SetItem setItem in GameInstance.instance.equipmentValues.currentEquipmentSet.setItems)
                                {
                                    if (setItem.category.Equals(ItemData.ItemCategory.Grenade) && setItem.item != null)
                                    {
                                        haveGrenadeInSet = true;
                                    }
                                }

                                bool anyGrenadeSlotFree = true;

                                if (haveGrenadeInSet)
                                {
                                    foreach (GameObject item in currentItemHandler.useableItems)
                                    {
                                        ItemData itemData = item.GetComponent<ItemInfo>().ItemData;
                                        if (itemData.itemCategory.Equals(ItemData.ItemCategory.Grenade) &&
                                            itemData.currentAmmount >= itemData.maxAmmount)
                                        {
                                            anyGrenadeSlotFree = false;
                                        }
                                    }
                                }

                                if (!haveGrenadeInSet)
                                {
                                    buyButton.CanSlide = false;

                                    buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "No grenade in equipment set";

                                    buyButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 4.5f;
                                }
                                else if (!anyGrenadeSlotFree)
                                {
                                    buyButton.CanSlide = false;

                                    buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "No Space";
                                }
                                break;

                            case ItemData.ExtraData.ExtraTyp.NewTraps:
                                bool haveTrapInSet = false;

                                foreach (EquipmentSet.SetItem setItem in GameInstance.instance.equipmentValues.currentEquipmentSet.setItems)
                                {
                                    if (setItem.category.Equals(ItemData.ItemCategory.Trap) && setItem.item != null)
                                    {
                                        haveTrapInSet = true;
                                    }
                                }

                                bool anyTrapSlotFree = true;

                                foreach (GameObject item in currentItemHandler.useableItems)
                                {
                                    ItemData itemData = item.GetComponent<ItemInfo>().ItemData;
                                    if (itemData.itemCategory.Equals(ItemData.ItemCategory.Trap) &&
                                        itemData.currentAmmount >= itemData.maxAmmount)
                                    {
                                        anyTrapSlotFree = false;
                                    }
                                }

                                if (!haveTrapInSet)
                                {
                                    buyButton.CanSlide = false;

                                    buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "No trap in equipment set";

                                    buyButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 4.5f;
                                }
                                else if (!anyTrapSlotFree)
                                {
                                    buyButton.CanSlide = false;

                                    buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "No Space";
                                }
                                break;
                        }
                    }
                }
            }
        } else
        {
            buyButton.CanSlide = false;

            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
            buyButton.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "";
        }
    }

    public void SetExtraBuyUp(ItemSlot itemSlot)
    {
        buyButton.GetComponentsInChildren<TextMeshProUGUI>()[1].text = itemSlot.itemData.gamePointPrice + " GP";

        void AddExtra()
        {
            GameInstance.instance.inGameValues.GamePoints -= itemSlot.itemData.gamePointPrice;

            switch (itemSlot.itemData.extraData.extraTyp)
            {
                case ItemData.ExtraData.ExtraTyp.AllAmmo:
                    AddAllWepaonAmmo();
                    break;

                case ItemData.ExtraData.ExtraTyp.CurrentAmmo:
                    AddCurrentWeaonAmmo();
                    break;

                case ItemData.ExtraData.ExtraTyp.AllHealth:
                    playerHandler.healthComponent.AddFullHealth();
                    break;

                case ItemData.ExtraData.ExtraTyp.MiddleHealth:
                    playerHandler.healthComponent.AddHealth(50);
                    break;

                case ItemData.ExtraData.ExtraTyp.SmallHealth:
                    playerHandler.healthComponent.AddHealth(25);
                    break;

                case ItemData.ExtraData.ExtraTyp.NewGrenades:
                    foreach(GameObject item in GameInstance.instance.inventoryValues.inventory)
                    {
                        if (item.GetComponent<ItemInfo>().ItemData.itemCategory.Equals(ItemData.ItemCategory.Grenade))
                        {
                            currentItemHandler.AddItem(item);
                        }
                    }
                    break;

                case ItemData.ExtraData.ExtraTyp.NewTraps:
                    foreach (GameObject item in GameInstance.instance.inventoryValues.inventory)
                    {
                        if (item.GetComponent<ItemInfo>().ItemData.itemCategory.Equals(ItemData.ItemCategory.Trap))
                        {
                            currentItemHandler.AddItem(item);
                        }
                    }
                    break;
            }
        }

        buyButton.manualAction.RemoveAllListeners();
        buyButton.manualAction.AddListener(AddExtra);
    }

    public void AddAllWepaonAmmo()
    {
        if (currentItemHandler.useableItems.Count > 0)
        {
            foreach (GameObject g in currentItemHandler.useableItems)
            {
                WeaponBase weaponBase = g.GetComponent<WeaponBase>();

                if (weaponBase != null)
                {
                    g.SetActive(true);

                    weaponBase.SetCurrentAmmo(weaponBase.itemData.weaponData.maxClipAmmo, weaponBase.itemData.weaponData.maxAmmo);

                    g.SetActive(false);

                    currentItemHandler.UpdateCurrentItem();
                }
            }
        }
    }

    public void AddCurrentWeaonAmmo()
    {
        if (currentItemHandler.CurrentItem != null)
        {
            WeaponBase weaponBase = currentItemHandler.CurrentItem.GetComponent<WeaponBase>();

            if (weaponBase != null)
            {
                weaponBase.SetCurrentAmmo(weaponBase.itemData.weaponData.maxClipAmmo, weaponBase.itemData.weaponData.maxAmmo);
            }
        }
    }

    public void BuyItem()
    {
        var currentSelectedSlot = extraInventory.GetComponentInChildren<ItemSpawner>().itemSpawnManage.currentSelectedSlot;

        ItemData selectedItemData = currentSelectedSlot.itemData;

        if (selectedItemData != null)
        {
            stackItem = currentItemHandler.useableItems.Find(
                                        item => item.GetComponent<ItemInfo>().ItemData.itemName == selectedItemData.itemName &&
                                        item.GetComponent<ItemInfo>().ItemData.currentAmmount < item.GetComponent<ItemInfo>().ItemData.maxAmmount);

            if (stackItem != null)
            {
                ItemData stackItemData = stackItem.GetComponent<ItemInfo>().ItemData;

                if (stackItemData.currentAmmount + selectedItemData.currentAmmount <= stackItemData.maxAmmount)
                    stackItemData.currentAmmount++;
            }
            else
                currentItemHandler.AddItem(extraInventory.GetComponentInChildren<ItemSpawner>().itemSpawnManage.currentSelectedSlot.item);
        }
    }
}
