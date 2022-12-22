using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class ItemSlotMovement : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [Inject]
    private MainMenueHandler menueHandler;

    private RectTransform rectTransform;

    private CanvasGroup canvasGroup;

    private ItemSlot itemSlot;

    private Vector3 actualPosition;

    private void Awake()
    {
        itemSlot = GetComponent<ItemSlot>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        itemSlot.transform.parent = itemSlot.myItemSpawner.references.parent.transform;
        rectTransform.anchoredPosition += eventData.delta / menueHandler.GetComponent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        itemSlot.transform.parent = itemSlot.myItemSpawner.transform;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }
}
