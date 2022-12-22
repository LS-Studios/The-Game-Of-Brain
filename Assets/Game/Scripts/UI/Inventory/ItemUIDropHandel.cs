using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class ItemUIDropHandel : MonoBehaviour, IDropHandler
{
    public ItemSpawner itemSpawner;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            itemSpawner.information.itemsToSpawn.Add(eventData.pointerDrag.GetComponent<ItemSlot>().item);
            eventData.pointerDrag.GetComponent<ItemSlot>().myItemSpawner.information.itemsToSpawn.Remove(eventData.pointerDrag.GetComponent<ItemSlot>().item);
            eventData.pointerDrag.GetComponent<ItemSlot>().myItemSpawner.RefreshInventory();
            itemSpawner.RefreshInventory();
        }
    }
}
