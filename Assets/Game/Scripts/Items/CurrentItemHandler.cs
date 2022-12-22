using ModestTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class CurrentItemHandler : MonoBehaviour
{
    [Header("Values")]
    private string currentItemName;

    [SerializeField]
    private GameObject currentItem;

    public GameObject CurrentItem
    {
        get
        {
            return currentItem;
        }

        set
        {
            currentItem = value;

            if (currentItem != null)
                currentItemName = currentItem.GetComponent<ItemInfo>().ItemData.itemName;
        }
    }

    private int currentWeaponIndex = 0;
    private int maxWeaponNumber;
    public List<GameObject> useableItems = new List<GameObject>();

    public List<string> targetTags = new List<string>();

    public float damageMultiply = 1f;

    [HideInInspector]
    public bool canDoAction = true;

    [Header("References")]
    public Animator animator;

    public UnityAction hitAction;
    public UnityAction reloadAction;

    public SpriteRenderer sight;
    public Sprite spreatSight;
    public Sprite steightSight;
    private Color sightColor;

    public GameObject backItems;

    [HideInInspector]
    public UnityEvent addEvent;

    private void Start()
    {
        sightColor = sight.color;

        SetAvailableItem();
        
        UpdateCurrentItem();
    }

    private void Update()
    {
        if (CurrentItem != null)
        {
            foreach (Transform g in transform.GetChild(0).transform)
            {
                if (g.gameObject.activeSelf)
                {
                    CurrentItem = g.gameObject;

                    ItemData itemData = CurrentItem.GetComponent<ItemInfo>().ItemData;

                    switch (CurrentItem.GetComponent<ItemInfo>().ItemData.generalData.sightTyp)
                    {
                        case ItemData.GeneralData.SightTyp.Streight:
                            sight.color = sightColor;
                            sight.sprite = steightSight;
                            break;

                        case ItemData.GeneralData.SightTyp.Spreat:
                            sight.color = sightColor;
                            sight.sprite = spreatSight;
                            break;

                        case ItemData.GeneralData.SightTyp.Place:
                            sight.color = sightColor;
                            sight.sprite = itemData.slotData.itemImageTop;
                            break;

                        case ItemData.GeneralData.SightTyp.NoSight:
                            sight.color = new Color(0, 0, 0, 0);
                            break;
                    }

                    sight.transform.localScale = new Vector3(itemData.generalData.placeSightSize, itemData.generalData.placeSightSize, sight.transform.localScale.z);
                }
            }

            if (CurrentItem.GetComponent<ItemInfo>().ItemData.currentAmmount <= 0)
            {
                RemoveItem(CurrentItem);

                SetAvailableItem();
            }

            //Activate back objects that are in the inventory
            foreach (Transform backItem in backItems.transform)
            {
                ItemData itemData = backItem.GetComponent<ItemInfo>().ItemData;
                
                if (useableItems.Find(item => item.GetComponent<ItemInfo>().ItemData.itemName == itemData.itemName)
                    != null)
                {
                    if (CurrentItem.GetComponent<ItemInfo>().ItemData.itemName == itemData.itemName)
                    {
                        backItem.gameObject.SetActive(false);
                    } 
                    else
                    {
                        backItem.gameObject.SetActive(true);
                        break;
                    }
                } 
                else
                {
                    backItem.gameObject.SetActive(false);
                }
            }
        } 
        else
        {
            sight.color = new Color(0, 0, 0, 0);

            foreach (Transform transform in transform.GetChild(0))
            {
                if (transform.gameObject.activeSelf)
                {
                    CurrentItem = transform.gameObject;
                }
            }
        }
    }

    public GameObject DropItem(GameObject itemToDrop)
    {
        if (itemToDrop.GetComponent<ItemInfo>().ItemData.dataTyp == ItemData.DataTyp.TrapData)
        {
            itemToDrop.SetActive(true);
            GameObject lmg = itemToDrop.GetComponent<LMG>().Place(transform.position + transform.up * 2);
            lmg.SetActive(true);
            return lmg;
        } 
        else
        {
            if (itemToDrop.GetComponent<ItemInfo>().ItemData.currentAmmount <= 1)
            {
                if (itemToDrop.GetComponent<WeaponBase>() != null)
                    itemToDrop.GetComponent<WeaponBase>().IsShoot = false;

                itemToDrop.transform.parent = transform.root.parent;

                itemToDrop.transform.position = transform.position + transform.up * 2;

                itemToDrop.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360));

                itemToDrop.SetActive(true);

                DropItem dropItem = itemToDrop.GetComponent<DropItem>();
                dropItem.enabled = true;
                dropItem.IsDropped = true;

                if (CurrentItem == itemToDrop)
                {
                    CurrentItem = null;
                    useableItems.Remove(itemToDrop);
                    SetAvailableItem();
                }
                else
                    useableItems.Remove(itemToDrop);

                UpdateCurrentItem();

                maxWeaponNumber = useableItems.Count - 1;

                return itemToDrop;
            }
            else
            {
                GameObject dropItemPrefab = Array.Find(GameInstance.instance.referenceValues.allUseables.ToArray(),
                item => item.GetComponent<ItemInfo>().ItemData.itemName.
                Equals(itemToDrop.GetComponent<ItemInfo>().ItemData.itemName));

                GameObject dropItem = Instantiate(dropItemPrefab);

                dropItem.transform.position = transform.position + transform.up * 2;

                dropItem.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360));

                dropItem.SetActive(true);

                DropItem dropItemScript = dropItem.GetComponent<DropItem>();
                dropItemScript.enabled = true;
                dropItemScript.IsDropped = true;

                dropItem.GetComponent<ItemInfo>().ItemData.currentAmmount = 1;
                itemToDrop.GetComponent<ItemInfo>().ItemData.currentAmmount--;

                UpdateCurrentItem();

                maxWeaponNumber = useableItems.Count - 1;

                return dropItem;
            }
        }
    }

    public void DropAllItems(Vector3 posToDrop)
    {
        while (useableItems.Count > 0)
            DropItem(useableItems[0]).transform.position = posToDrop;
    }

    public void DropAllItems()
    {
        while (useableItems.Count > 0)
            DropItem(useableItems[0]);
    }

    public void RemoveItem(GameObject itemToRemove)
    {
        if (CurrentItem == itemToRemove)
        {
            useableItems.Remove(itemToRemove);
            Destroy(itemToRemove);
            CurrentItem = null;
            SetAvailableItem();
        }
        else
        {
            useableItems.Remove(itemToRemove);
            Destroy(itemToRemove);
        }

        maxWeaponNumber = useableItems.Count - 1;
    }

    public void AddItem(GameObject itemToAdd)
    {
        GameObject newItem = Array.Find(GameInstance.instance.referenceValues.allUseables.ToArray(), 
            item => item.GetComponent<ItemInfo>().ItemData.itemName.
            Equals(itemToAdd.GetComponent<ItemInfo>().ItemData.itemName));

        newItem = CreateItem(newItem);

        //Only set the item data if its no prefab
        if (itemToAdd.scene.IsValid())
            newItem.GetComponent<ItemInfo>().ItemData = itemToAdd.GetComponent<ItemInfo>().ItemData;

        if (useableItems.Count == 1)
            SetAvailableItem();

        addEvent.Invoke();

        maxWeaponNumber = useableItems.Count - 1;
    }

    //Create all items of inventory
    public void CreateItemList(List<GameObject> inventory)
    {
        useableItems = new List<GameObject>();
        
        for (int i = 0; i < inventory.Count; i++)
        {
            GameObject newItem = inventory[i];

            CreateItem(newItem);
        }

        SetAvailableItem();
        maxWeaponNumber = useableItems.Count - 1;
    }

    //Create an item an add it to the player
    private GameObject CreateItem(GameObject newItem)
    {
        if (newItem != null) {
            GameObject item = Instantiate(newItem, transform.GetChild(0));
            item.SetActive(true);
            WeaponBase weaponBase = item.GetComponent<WeaponBase>();
            weaponBase.currentItemHandler = this;
            weaponBase.itemData.damage *= damageMultiply;
            weaponBase.targetTags = targetTags;
            hitAction += weaponBase.CallPunchWithWeapon;

            if (item.GetComponent<WeaponBase>() != null)
                weaponBase.SetAnimator(animator);

            item.SetActive(false);

            useableItems.Add(item);

            return item;
        }

        return null;
    }

    public void SetAvailableItem()
    {
        if (useableItems.Count > 0)
        {
            CurrentItem = useableItems[0];
        }

        UpdateCurrentItem();
    }

    public void UpdateCurrentItem()
    {
        if (CurrentItem != null)
        {
            if (CurrentItem.GetComponent<WeaponBase>() != null)
                CurrentItem.GetComponent<WeaponBase>().StopReload();

            foreach (GameObject item in useableItems)
            {
                item.SetActive(false);
            }

            foreach (GameObject item in useableItems)
            {
                if (item.GetComponent<ItemInfo>().ItemData.itemName == currentItemName)
                {
                    item.SetActive(true);
                }
            }

            ItemData itemData = CurrentItem.GetComponent<ItemInfo>().ItemData;

            animator.SetInteger("HoldingTyp", (int)itemData.generalData.holdingType+1);

            switch (itemData.generalData.holdingType)
            {
                case ItemData.GeneralData.HoldingType.HoldingMeeleWeapon:
                    animator.SetInteger("WeaponTyp", (int)itemData.generalData.meeleWeaponType);
                    break;

                case ItemData.GeneralData.HoldingType.HoldingOneHandedWeapon:
                    animator.SetInteger("WeaponTyp", (int)itemData.generalData.oneHandetWeaponTyp);
                    break;

                case ItemData.GeneralData.HoldingType.HoldingTwoHandedWeapon:
                    animator.SetInteger("WeaponTyp", (int)itemData.generalData.twoHandetWeaponTyp);
                    break;

                case ItemData.GeneralData.HoldingType.HoldinObjectInHands:
                    animator.SetInteger("WeaponTyp", (int)itemData.generalData.objectTyp);
                    break;
            }
        } else
        {
            animator.SetInteger("HoldingTyp", 0);
        }
    }

    private void UpdateEnumViaNumber()
    {
        if (useableItems.Count > 0)
        {
            CurrentItem = useableItems[currentWeaponIndex];
        }
    }

    public void SwitchWeapon(float dir)
    {
        if (dir > 0)
        {
            NextItem();
        }
        else if (dir < 0)
        {
            LastItem();
        }
        UpdateCurrentItem();
    }

    private void NextItem()
    {
        if (CurrentItem.GetComponent<WeaponBase>() != null)
            CurrentItem.GetComponent<WeaponBase>().StopReload();

        currentWeaponIndex += 1;

        if (currentWeaponIndex > maxWeaponNumber)
        {
            currentWeaponIndex = 0;
        }
        UpdateEnumViaNumber();
        UpdateCurrentItem();
    }

    private void LastItem()
    {
        if (CurrentItem.GetComponent<WeaponBase>() != null)
            CurrentItem.GetComponent<WeaponBase>().StopReload();

        currentWeaponIndex -= 1;

        if (currentWeaponIndex < 0)
        {
            currentWeaponIndex = maxWeaponNumber;
        }

        UpdateEnumViaNumber();
        UpdateCurrentItem();
    }

    public void ChangeItem(GameObject newItem)
    {
        if (CurrentItem != null && CurrentItem.GetComponent<WeaponBase>() != null)
            CurrentItem.GetComponent<WeaponBase>().StopReload();

        CurrentItem = newItem;

        UpdateCurrentItem();
    }

    public void PrepareAttack()
    {
        CallCurrentItemAction();
    }

    public void CallCurrentItemAction()
    {
        if (canDoAction)
        {
            if (CurrentItem == null)
            {
                Punch();
            }
            else
            {
                if (CurrentItem.GetComponent<WeaponBase>() != null)
                {
                    if (CurrentItem.GetComponent<ItemInfo>().ItemData.weaponData.currentClipAmmo > 0)
                        CurrentItem.GetComponent<WeaponBase>().StopReload();
                    CurrentItem.GetComponent<WeaponBase>().CallAttack();
                }
                else if (CurrentItem.GetComponent<LMG>() != null)
                {
                    CurrentItem.GetComponent<LMG>().Place(sight.transform.position + transform.up * 1.25f).SetActive(true);
                }
            }
        }
    }

    public void Punch()
    {
        if (GameInstance.instance.canDoItemAction)
        {
            hitAction.Invoke();

            if (useableItems.Count <= 0)
            {
                animator.SetInteger("HoldingTyp", 0);
                int random = UnityEngine.Random.Range(0, 3);

                animator.SetInteger("Side", random);
            }

            animator.SetTrigger("Attack");
            AudioManager.instance.Play("Punch", transform);
        }
    }
}
