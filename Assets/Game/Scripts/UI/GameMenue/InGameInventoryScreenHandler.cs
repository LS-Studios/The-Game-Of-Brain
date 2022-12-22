using Zenject;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InGameInventoryScreenHandler : MonoBehaviour
{
    public TextMeshProUGUI gamePointText;

    public UnityEvent dropOneItemAction;
    public UnityEvent dropAllItemsAction;

    public SlideButton upgradeButton;
    public SlideButton dropButton;
    public ItemSpawner itemSpawner;
    public TextMeshProUGUI stetText;

    public CurrentItemHandler currentItemHandler;

    [Inject]
    private GameManager myGameManager;

    void Update()
    {
        if (GameInstance.instance.inGameValues.GamePoints > 99999999999999999)
        {
            gamePointText.text = 99999999999999999.ToString();
            GameInstance.instance.inGameValues.GamePoints = 99999999999999999;
        }
        else
        {
            gamePointText.text = GameInstance.instance.inGameValues.GamePoints.ToString();
        }

        var selectedSlot = itemSpawner.itemSpawnManage.currentSelectedSlot;

        if (selectedSlot != null)
        {
            ItemData itemData = selectedSlot.itemData;

            if (itemData != null)
            {
                upgradeButton.GetComponent<EventTrigger>().enabled = true;
                upgradeButton.CanSlide = true;
                upgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Upgrade";
                upgradeButton.GetComponentsInChildren<TextMeshProUGUI>()[1].text = itemData.UpgradePrice.ToString();

                dropButton.CanSlide = true;
                dropButton.GetComponentInChildren<TextMeshProUGUI>().text = "Drop";
                dropButton.manualAction = dropOneItemAction;

                if (!itemData.canGetLeveled)
                {
                    upgradeButton.GetComponent<EventTrigger>().enabled = false;
                    upgradeButton.CanSlide = false;

                    upgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "No upgrade";
                    upgradeButton.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "-/-";
                }
                else if (itemData.currentLevel >= itemData.maxLevel)
                {
                    upgradeButton.GetComponent<EventTrigger>().enabled = false;
                    upgradeButton.CanSlide = false;

                    upgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Max level";
                    upgradeButton.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "-/-";
                }
                else if (GameInstance.instance.inGameValues.GamePoints < itemData.UpgradePrice)
                {
                    upgradeButton.GetComponent<EventTrigger>().enabled = false;
                    upgradeButton.CanSlide = false;
                }
            }
        } 
        else
        {
            upgradeButton.GetComponent<EventTrigger>().enabled = false;
            upgradeButton.CanSlide = false;
            upgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Upgrade";
            upgradeButton.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "-/-";

            upgradeButton.CanSlide = false;
            dropButton.GetComponentInChildren<TextMeshProUGUI>().text = "Drop all";
            dropButton.manualAction = dropAllItemsAction;

            if (currentItemHandler.useableItems.Count > 0)
                dropButton.CanSlide = true;
            else
                dropButton.CanSlide = false;
        }
    }
}
