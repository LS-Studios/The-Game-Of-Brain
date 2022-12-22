using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DropItem : MonoBehaviour
{
    public Material outlineMaterial;
    public Material normalMaterial;

    private bool isDroped = false;
    public bool IsDropped
    {
        get
        {
            return isDroped;
        }

        set 
        {
            isDroped = value;

            if (isDroped)
            {
                gameObject.layer = 6;
                GetComponent<SpriteRenderer>().sortingOrder = 2;
            }
            else
            {
                gameObject.layer = 10;
                GetComponent<SpriteRenderer>().sortingOrder = normalSortOrder;
            }
        }
    }

    public int normalSortOrder = 6;

    private bool alreadySpreaded = false;

    private void Awake()
    {
        if (transform.root == transform)
            isDroped = true;
    }

    [ContextMenu("Update")]
    private void Update()
    {
        if (isDroped)
        {
            ItemData currentItemData = GetComponent<ItemInfo>().ItemData;

            GetComponent<ItemInfo>().ItemData.currentAmmount = 1;

            GetComponent<SpriteRenderer>().material = outlineMaterial;
            Color currentLevelColor = GameInstance.instance.inGameValues.GetRarity(currentItemData.currentLevel).rarityColor;
            GetComponent<SpriteRenderer>().material.SetColor("_Color", currentLevelColor);

            if (GetComponent<WeaponBase>() != null)
            {
                GetComponent<WeaponBase>().enabled = false;
            }

            if (!alreadySpreaded)
            {
                IEnumerator SpreadDeleay()
                {
                    yield return new WaitForSeconds(0.5f);

                    GetComponent<BoxCollider2D>().isTrigger = true;
                    Destroy(GetComponent<Rigidbody2D>());
                }


                StartCoroutine(SpreadDeleay());

                Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0;
                rb.mass = 25;

                GetComponent<BoxCollider2D>().isTrigger = false;
                alreadySpreaded = true;
            }
        } else
        {
            alreadySpreaded = false;

            if (GetComponent<WeaponBase>() != null)
            {
                GetComponent<WeaponBase>().enabled = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            CurrentItemHandler currentItemHandler = collision.gameObject.GetComponentInChildren<CurrentItemHandler>();

            if (isDroped)
            {
                //Add to stack if there is one
                foreach (GameObject item in currentItemHandler.useableItems)
                {
                    var currentitemData = GetComponent<ItemInfo>().ItemData;
                    var inventoryItemData = item.GetComponent<ItemInfo>().ItemData;
                    if (currentitemData.itemName.Equals(inventoryItemData.itemName) &&
                        inventoryItemData.currentAmmount < inventoryItemData.maxAmmount)
                    {
                        isDroped = false;

                        inventoryItemData.currentAmmount += currentitemData.currentAmmount;

                        Destroy(gameObject);

                        return;
                    }
                }

                //Add if there is no stackable in inventory
                if (currentItemHandler.useableItems.Count < 4 &&
                    currentItemHandler.useableItems.
                    Find(item => item.GetComponent<ItemInfo>().ItemData.itemName
                    == GetComponent<ItemInfo>().ItemData.itemName) == null)
                {
                    isDroped = false;
                    currentItemHandler.AddItem(gameObject);
                    Destroy(gameObject);
                }
            }
        }
    }
}
