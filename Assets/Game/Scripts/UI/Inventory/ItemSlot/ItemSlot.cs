using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using UnityEngine.Events;

public class ItemSlot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] 
    private TextMeshProUGUI ammountText;

    [SerializeField]
    private TextMeshProUGUI levelText;

    [SerializeField]
    private TextMeshProUGUI slotNameText;

    [SerializeField] 
    private Button button;

    [SerializeField] 
    private Image slotImage;

    public ItemSpawner myItemSpawner;

    public ItemSpawner.SlotTyp slotTyp;

    [HideInInspector]
    public GameObject item;

    public int itemIndex = 0;

    public ItemData itemData;

    void Start()
    {
        gameObject.AddComponent<ZenAutoInjecter>().ContainerSource = ZenAutoInjecter.ContainerSources.SceneContext;

        if (myItemSpawner == null)
            myItemSpawner = transform.parent.GetComponent<ItemSpawner>();

        slotTyp = myItemSpawner.slotTyp;

        //Scale Slot
        transform.localScale = new Vector3(myItemSpawner.slotScale.x, myItemSpawner.slotScale.y, transform.localScale.z);

        RefreshSlot();

        button.onClick.AddListener(SetButtonEvent);
    }

    public void RefreshSlot()
    {
        slotNameText.gameObject.SetActive(true);
        slotImage.gameObject.SetActive(true);
        GetComponent<CanvasGroup>().alpha = 1;

        if (item != null)
        {
            itemData = item.GetComponent<ItemInfo>().ItemData;
        }

        if (itemData != null)
        {
            gameObject.name = itemData.itemName;

            slotImage.sprite = itemData.slotData.itemImageSide;
            slotImage.SetNativeSize();
            slotImage.transform.localScale = itemData.slotData.slotImageScaleSide;
            slotImage.transform.rotation = Quaternion.Euler(new Vector3(0, 0, itemData.slotData.slotImageRotationSide));

            levelText.fontMaterial.EnableKeyword("GLOW_ON");

            //Set Slot description
            slotNameText.text = itemData.itemName;
        }
        else
        {
            ammountText.gameObject.SetActive(false);
            levelText.gameObject.SetActive(false);
            slotNameText.gameObject.SetActive(false);
            slotImage.gameObject.SetActive(false);
            GetComponent<CanvasGroup>().alpha = 0.2f;
        }

        SetSlotStets();
    }

    private void Update()
    {
        RefreshSlot();
    }

    private void SetSlotStets()
    {
        if (itemData != null)
        {
            if (itemData.currentAmmount > 1)
            {
                ammountText.text = "" + itemData.currentAmmount;
                ammountText.gameObject.SetActive(true);
            }
            else
                ammountText.gameObject.SetActive(false);

            if (itemData.currentLevel > 0)
            {
                levelText.gameObject.SetActive(true);
                levelText.text = "" + itemData.currentLevel;

                var baseMaterial = levelText.fontSharedMaterial;
                var glowMaterial = new Material(baseMaterial);
                glowMaterial.SetColor("_GlowColor", GameInstance.instance.inGameValues.GetRarity(itemData.currentLevel).rarityColor);
                levelText.fontSharedMaterial = glowMaterial;
                
            }
            else
                levelText.gameObject.SetActive(false);
        }
    }

    public void RemoveItem()
    {
        myItemSpawner.playerHandler.currentItemHandler.DropItem(item);

        myItemSpawner.RefreshInventory();
    }

    private void SetButtonEvent()
    {
        myItemSpawner.clickEvents.clickEvent?.Invoke(this);
        
        if (myItemSpawner.information.useEquipmentSet)
        {
            EquipmentSlot equipmentSlot = myItemSpawner.transform.GetChild(itemIndex).GetComponent<EquipmentSlot>();
            equipmentSlot.clickEvents.clickEvent?.Invoke(equipmentSlot);
        }

        switch (slotTyp)
        {
            case ItemSpawner.SlotTyp.ChangeableItem:
                myItemSpawner.itemSpawnManage.ItemSlotChange(this);
                break;

            case ItemSpawner.SlotTyp.SelectableItem:
                myItemSpawner.itemSpawnManage.SelectSlot(this);
                break;

            case ItemSpawner.SlotTyp.Prewiew:
                //Do nothing on click
                break;
        }
    }
}
