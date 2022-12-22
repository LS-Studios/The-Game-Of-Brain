using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{  
    //Property to acces item data
    public ItemData ItemData { 
        get 
        {
            if (currentItemHandlerData == null)
            {
                CloneData();
            }

            return currentItemHandlerData;
        }

        set
        {
            currentItemHandlerData = value;
        }
    }

    //Preset item data to clone
    [SerializeField] private ItemData itemData;

    //Item data that can also be upgradet
    private ItemData currentItemHandlerData;

    public void CloneData()
    {
        if (itemData != null && currentItemHandlerData == null)
        {
            ItemData newItemData = Instantiate(itemData);
            currentItemHandlerData = newItemData;
            newItemData.itemName = itemData.itemName;

            if (GameInstance.instance.equipmentValues.CurrentSetContaisPerk(ItemData.PerkData.PerkTyp.ExtraAmmo))
            {
                newItemData.weaponData.maxAmmo = Mathf.RoundToInt(newItemData.weaponData.maxAmmo * 1.5f);

                newItemData.weaponData.currentClipAmmo = newItemData.weaponData.maxClipAmmo;
                newItemData.weaponData.currentAmmo = newItemData.weaponData.maxAmmo;
            }
        }
    }
}
