using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;
using UnityEngine.UI;

public class ShopScreenHandler : MonoBehaviour
{
    public TextMeshProUGUI previousBtnText;
    public TextMeshProUGUI nextBtnText;

    public ScreenSwipe screenSwipe;

    public void ChangeBtnNames(int screenNum)
    {
        List<GameObject> itemsToSpawn = new List<GameObject>();

        switch (screenNum)
        {
            //Primary weapon shop
            case 0:
                nextBtnText.text = "Secondary weapons ->";

                itemsToSpawn = GameInstance.instance.referenceValues.allWeapons.FindAll(item =>
                    item.GetComponent<ItemInfo>().ItemData.itemCategory.Equals(ItemData.ItemCategory.PrimaryWeapon));
                break;

            //Secondary weapon shop
            case 1:
                previousBtnText.text = "<- Primary weapons";
                nextBtnText.text = "Traps ->";

                itemsToSpawn = GameInstance.instance.referenceValues.allWeapons.FindAll(item =>
                    item.GetComponent<ItemInfo>().ItemData.itemCategory.Equals(ItemData.ItemCategory.SecondaryWeapon));
                break;

            //Grenade shop
            case 2:
                previousBtnText.text = "<- Secondary weapons";
                nextBtnText.text = "Traps ->";

                itemsToSpawn = GameInstance.instance.referenceValues.allGrenades;
                break;

            //Trap shop
            case 3:
                previousBtnText.text = "<- Grenades";
                nextBtnText.text = "Perks ->";

                itemsToSpawn = GameInstance.instance.referenceValues.allTraps;
                break;

            //Perk shop
            case 4:
                previousBtnText.text = "<- Traps";

                itemsToSpawn = GameInstance.instance.referenceValues.allPerks;
                break;
        }

        ItemSpawner itemSpawner = screenSwipe.screens[screenNum].GetComponentInChildren<ItemSpawner>();

        itemSpawner.ResetItemsToSpawn();

        itemSpawner.information.itemsToSpawn.AddRange(itemsToSpawn);

        itemSpawner.RefreshInventory();
    } 
}
