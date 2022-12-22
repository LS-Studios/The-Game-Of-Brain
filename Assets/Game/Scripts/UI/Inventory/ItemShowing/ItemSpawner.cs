using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;
using Zenject;
using TMPro;
using UnityEngine.Events;

public class ItemSpawner : MonoBehaviour
{
    public References references;
    public Information information;
    public ClickEvents clickEvents;

    [Header("Slot Transform")]
    public Vector2 slotScale = new Vector2(1, 1);

    public enum SlotTyp { Prewiew, ChangeableItem, SelectableItem }
    public SlotTyp slotTyp = SlotTyp.Prewiew;

    [HideInInspector]
    public int itemsPerRow = 0;

    private List<ValueTuple<ItemData.ItemCategory, List<GameObject>>> headerGroups = new List<(ItemData.ItemCategory, List<GameObject>)>();

    [HideInInspector]
    public ItemSpawnManage itemSpawnManage;

    public PlayerHandler playerHandler;

    [Serializable]
    public class References
    {
        [HideInInspector]
        public GlobalData globalData;

        //For drag movement
        public GameObject parent;

        //Important if items should be able to get moved
        public List<ItemSpawner> spawnersToWorkWith = new List<ItemSpawner>();
    }

    [Serializable]
    public class Information
    {
        public bool keepSloots = false;

        [ConditionalHide(new string[] { "useInventory", "usePackbackInventory", "useShopInventory" }, true, true, false)]
        public bool useEquipmentSet = false;

        [ConditionalHide(new string[] { "useEquipmentSet", "usePackbackInventory", "useShopInventory" }, true, true, false)]
        public bool useInventory = false;

        [ConditionalHide(new string[] { "useEquipmentSet", "useInventory", "useShopInventory" }, true, true, false)]
        public bool usePackbackInventory = false;

        [ConditionalHide(new string[] { "useEquipmentSet", "useInventory", "usePackbackInventory" }, true, true, false)]
        public bool useShopInventory = false;

        [Space]

        public bool sortByCategory = false;
        public bool fillEmptyGameInventorySlots = false;

        [Space]
        
        public List<GameObject> itemsToSpawn = new List<GameObject>();

        [HideInInspector]
        public List<ItemSlot> spawnedSlots = new List<ItemSlot>();
    }


    [Serializable]
    public class ClickEvents
    {

        [Serializable]
        public class ClickEvent : UnityEvent<ItemSlot>
        {
        }

        public ClickEvent clickEvent;

        [Serializable]
        public class DoubleClickEvent : UnityEvent<ItemSlot>
        {
        }

        public DoubleClickEvent doubleClickAction;
    }

    protected virtual void Awake()
    {
        if (itemSpawnManage == null)
        {
            if (playerHandler.currentItemHandler != null)
                itemSpawnManage = new InventoryManage(playerHandler.currentItemHandler);
            else
                itemSpawnManage = new ItemSpawnManage();
            
            if (references.spawnersToWorkWith.Count > 0)
            {
                references.spawnersToWorkWith.ForEach(spawner => spawner.itemSpawnManage = itemSpawnManage);
            }
        }

        itemSpawnManage.itemSpawner = this;

        references.globalData = GameInstance.instance.referenceValues.globalData;
        RectTransform rectTransform = references.globalData.itemSlot.GetComponent<RectTransform>();
        GridLayoutGroup gridLayoutGroup = GetComponent<GridLayoutGroup>();
        gridLayoutGroup.cellSize = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
        gridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
        itemsPerRow = gridLayoutGroup.constraintCount;

        //Add all item categorys to header group
        foreach (ItemData.ItemCategory itemCategory in Enum.GetValues(typeof(ItemData.ItemCategory)))
        {
            headerGroups.Add((itemCategory, new List<GameObject>()));
        }

        if (playerHandler.currentItemHandler != null && information.useInventory)
            playerHandler.inventory = this;
    }

    private void OnEnable()
    {
        RefreshInventory();
    }

    public void ResetItemsToSpawn()
    {
        information.itemsToSpawn = new List<GameObject>();
    }

    public virtual void RefreshInventory()
    {
        if (!information.keepSloots)
        {
            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
            }
        } else
        {
            foreach (Transform t in transform)
            {
                ItemSlot itemSlot = t.GetComponent<ItemSlot>();

                if (itemSlot != null)
                {
                    itemSlot.slotTyp = slotTyp;
                }
            }
        }


        if (information.useInventory)
        {
             ResetItemsToSpawn();
             //currentItemHandler.useableItems.ForEach(item => information.itemsToSpawn.Add(item));

            for (int i = 0; i < 4; i++)
            {
                if (i < playerHandler.currentItemHandler.useableItems.Count)
                    information.itemsToSpawn.Add(playerHandler.currentItemHandler.useableItems[i]);
                else
                    information.itemsToSpawn.Add(null);
            }
        }
        else if (information.usePackbackInventory)
        {
            //Actions are handeled by equipment set slots
        }
        else if (information.useEquipmentSet)
        {
            if (GameInstance.instance.equipmentValues.currentEquipmentSet != null)
            {
                EquipmentSet currentSet = GameInstance.instance.equipmentValues.currentEquipmentSet;

                for (int i = 0; i < transform.childCount; i++)
                {
                    GameObject slotItem = currentSet.setItems[i].item;
                    EquipmentSlot slot = transform.GetChild(i).GetComponent<EquipmentSlot>();

                    slot.SetSlotData(slotItem);
                    slot.itemCategory = currentSet.setItems[i].category;

                    slot.RefreshSlot();
                }
            }
        }
        else if (information.useShopInventory)
        {
            //Items get filtered in the shop handler
        }

        if (information.sortByCategory)
        {
            //Add items to the header groups
            foreach (var headerGroup in headerGroups)
            {
                headerGroup.Item2.Clear();

                List<GameObject> groupItems = information.itemsToSpawn.
                                                FindAll(item => item.GetComponent<ItemInfo>().
                                                ItemData.itemCategory == headerGroup.Item1);

                while (groupItems.Count % itemsPerRow != 0)
                {
                    GameObject emptySlot = references.globalData.itemSlot;
                    emptySlot.name = "PlaceHolder";
                    groupItems.Add(emptySlot);
                }

                groupItems.ForEach(item => headerGroup.Item2.Add(item));
            }
        }

        if (references.globalData != null)
            CreateSlots();

        //Color already bought slots
        if (information.useShopInventory)
        {
            foreach (Transform itemSlot in transform)
            {
                if (GameInstance.instance.inventoryValues.packbackInventory.Contains(itemSlot.GetComponent<ItemSlot>().item))
                {
                    itemSlot.GetComponentInChildren<Image>().color = new Color32(0x7A, 0xBF, 0x7D, 255);
                }
            }
        } else if (information.usePackbackInventory) {
            foreach (Transform itemSlot in transform)
            {
                if (GameInstance.instance.equipmentValues.CurrentSetContais(itemSlot.GetComponent<ItemSlot>().item))
                {
                    itemSlot.GetComponentInChildren<Image>().color = new Color32(0xD7, 0x7A, 0x74, 255);
                }
            }
        }
    }

    public virtual void SortByCategory(ValueTuple<InventoryManage.SortTyp, InventoryManage.SortDirection> sortProperty, bool sortEquipment, string variableToSort)
    {
        itemSpawnManage.SortItems(sortEquipment, variableToSort, sortProperty);
    }

    public void CreateSlots()
    {
        #region Herader seperation
        if (information.sortByCategory)
        {
            for (int i = 0; i < headerGroups.Count; i++)
            {
                if (headerGroups[i].Item2.Count > 0)
                {
                    for (int s = 0; s < itemsPerRow; s++)
                    {
                        bool placeCheck;
                        //Set neccecary pivot position based in items per row
                        if (itemsPerRow % 2 == 0)
                            placeCheck = (s == Math.Ceiling((float)itemsPerRow / 2));
                        else
                            placeCheck = (s + 1 == Math.Ceiling((float)itemsPerRow / 2));

                        //Spawn the header
                        if (s == 0)
                        {
                            GameObject itemHeaderInst = Instantiate(references.globalData.ItemHeader, transform);
                            itemHeaderInst.GetComponentInChildren<TextMeshProUGUI>().text = LSUtils.SeperateCompundString(headerGroups[i].Item1.ToString()) + "s";

                            //Set neccecary pivot position based in items per row
                            //if (itemsPerRow % 2 == 0)
                            //    itemHeaderInst.transform.GetChild(0).GetComponent<RectTransform>().pivot = new Vector2(1f, 0.5f);
                            //else
                            //    itemHeaderInst.transform.GetChild(0).GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                        }
                        //Add Title space to center the header
                        else
                        {
                            GameObject itemSlotInst = Instantiate(references.globalData.placeHolder, transform);
                        }
                    }

                    //Spawn items to group
                    for (int r = 0; r < headerGroups[i].Item2.Count; r++)
                    {
                        if (headerGroups[i].Item2[r].name != "PlaceHolder")
                        {
                            GameObject itemSlotInst = Instantiate(references.globalData.itemSlot, transform);
                            ItemSlot itemSlotScript = itemSlotInst.GetComponent<ItemSlot>();
                            itemSlotScript.slotTyp = slotTyp;
                            itemSlotScript.myItemSpawner = GetComponent<ItemSpawner>();
                            itemSlotScript.item = headerGroups[i].Item2[r];

                            //Assingn right Index by taking last item of previous header group
                            int index = 0;
                            //Look if there is a last header group and if it contains any items
                            if (i - 1 >= 0 && headerGroups[i - 1].Item2.Count > 0)
                            {
                                List<ItemSlot> itemSlots = new List<ItemSlot>();
                                foreach (GameObject g in headerGroups[i - 1].Item2)
                                {
                                    if (g.GetComponent<ItemSlot>() != null)
                                    {
                                        itemSlots.Add(g.GetComponent<ItemSlot>());
                                    }
                                }

                                index = r + itemSlots[^1].itemIndex + 1;
                            }
                            else
                                index = r;

                            itemSlotScript.itemIndex = index;
                            information.spawnedSlots.Add(itemSlotScript);

                            //Assign instanciated object to header group to acces index
                            headerGroups[i].Item2[r] = itemSlotInst;
                        }
                        else
                        {
                            GameObject itemSlotInst = Instantiate(references.globalData.placeHolder, transform);
                        }
                    }
                }
            }
        }
        #endregion
        else
        {
            for (int i = 0; i < information.itemsToSpawn.Count; i++)
            {
                GameObject itemSlotInst = Instantiate(references.globalData.itemSlot, transform);
                ItemSlot itemSlotScript = itemSlotInst.GetComponent<ItemSlot>();
                itemSlotScript.item = information.itemsToSpawn[i];
                itemSlotScript.itemIndex = i;
                itemSlotScript.slotTyp = slotTyp;
                itemSlotScript.myItemSpawner = GetComponent<ItemSpawner>();

                information.spawnedSlots.Add(itemSlotScript);
            }
        }
    }

    public virtual void UpgradeSelectedItem()
    {
        itemSpawnManage.UpgradeSelectedItem();
    }

    public void DropSelectedItem()
    {
        InventoryManage inventoryManage = (InventoryManage) itemSpawnManage;
        inventoryManage.DropSelectedItem();

        RefreshInventory();
        playerHandler.currentItemHandler.UpdateCurrentItem();
    }

    public void DropAllItems()
    {
        InventoryManage inventoryManage = (InventoryManage)itemSpawnManage;
        inventoryManage.DropAllItems();

        RefreshInventory();
        playerHandler.currentItemHandler.UpdateCurrentItem();
    }

    public void DeselectSlots()
    {
        InventoryManage inventoryManage = (InventoryManage)itemSpawnManage;
        inventoryManage.DeSelectSlot();
    }

    public void EquipNewItem(ItemSlot itemSlot)
    {
        if (playerHandler.currentItemHandler != null && itemSlot.item != null)
        {
            playerHandler.currentItemHandler.ChangeItem(itemSlot.item);

            GameMenueHandler gameMenueHandler = FindObjectOfType<GameMenueHandler>();
            if (gameMenueHandler != null)
                gameMenueHandler.ToggelInventory();
        }
    }
}
