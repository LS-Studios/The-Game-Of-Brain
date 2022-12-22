using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Zenject;

public class MainMenueHandler : MonoBehaviour
{
    public bool directStartGame = false;

    public RectTransform screenMove;
    public float screenMoveSpeed = 0.1f;

    public SlideButton addSetButton;

    public GameObject preScreen;

    public Animation animations;
    private Vector2 lastPos;

    public ItemInfoAction itemInfoAction;

    private void Awake()
    {
        if (directStartGame)
            StartGame();

        itemInfoAction = FindObjectOfType<ItemInfoAction>();

        if (!GameInstance.instance.gameHasStartenAlready)
        {
            preScreen.SetActive(true);

            GameInstance.instance.gameHasStartenAlready = true;
        }
        else
        {
            preScreen.SetActive(false);

            StartGameSlide();
        }
    }

    public void StartGameSlide()
    {
        animations.Play("StartGame");
    }

    public void GoTo(Vector2 direction)
    {
        StartCoroutine(SlideScreen(new Vector2(-direction.x * screenMove.rect.width, -direction.y * screenMove.rect.height)));

        lastPos = screenMove.anchoredPosition - direction;
    }

    public void GoBackTo()
    {
        StartCoroutine(SlideScreen(lastPos));
    }

    IEnumerator SlideScreen(Vector2 moveTo)
    {
        float value = 0f;

        Vector2 startPos = screenMove.anchoredPosition;

        while (value < 1.2f)
        {
            screenMove.anchoredPosition = Vector2.Lerp(startPos, new Vector2(0, 0) + moveTo, value);

            value += Time.deltaTime * screenMoveSpeed;

            yield return null;
        }

        screenMove.anchoredPosition = moveTo;
    }

    public void ShowItemInfo(ItemSlot itemsSlot)
    {
        itemInfoAction.DisplayInfo(itemsSlot);
        GoTo(new Vector2(0, -2));
    }
    
    public void AddEquipmentSet()
    {
        if (GameInstance.instance.equipmentValues.equipmentSets.Count + 1 < 6)
            GameInstance.instance.equipmentValues.AddEquipmentSet();
        else if (GameInstance.instance.equipmentValues.equipmentSets.Count + 1 >= 6)
        {
            GameInstance.instance.equipmentValues.AddEquipmentSet();
            addSetButton.GetComponentInChildren<TextMeshProUGUI>().text = "6 sets is maximum";
            addSetButton.CanSlide = false;
        }

    }

    public void StartGame()
    {
        List<GameObject> itemList = new List<GameObject>();

        foreach (EquipmentSet.SetItem setItem in GameInstance.instance.equipmentValues.currentEquipmentSet.setItems)
        {
            if (setItem.category != ItemData.ItemCategory.Perk) 
            {
                itemList.Add(setItem.item);
            }
        }

        GameInstance.instance.inventoryValues.inventory = itemList;
        SceneManager.LoadSceneAsync(GameInstance.instance.missionValues.missionsSceneName);
    }
}
