using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using TMPro;

public class GameMenueHandler: MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject pauseMenuePanel;
    public GameObject mapCam;

    public GameObject grenadeButton;

    public GameObject mobileControlls;

    public Image currentWeaponImage;

    public Slider reloadSlider;

    public CurrentItemHandler currentItemHandler;

    void Start()
    {
        inventoryPanel.GetComponent<CanvasGroup>().alpha = 0;
        inventoryPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        inventoryPanel.GetComponent<CanvasGroup>().interactable = false;

        pauseMenuePanel.GetComponent<CanvasGroup>().alpha = 0;
        pauseMenuePanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        pauseMenuePanel.GetComponent<CanvasGroup>().interactable = false;

        if (GameInstance.instance.debugValues.isMobile)
        {
            mobileControlls.SetActive(true);
        }
        else
        {
            mobileControlls.SetActive(false);
        }
    }

    private void Update()
    {
        if (currentItemHandler != null && currentItemHandler.useableItems.Count > 0)
        {
            GameObject[] grenadesInInventory = Array.FindAll(currentItemHandler.useableItems.ToArray(), item => item.GetComponent<Grenade>() != null);

            currentWeaponImage.color = new Color(currentWeaponImage.color.r, currentWeaponImage.color.g, currentWeaponImage.color.b, 1);
            currentWeaponImage.SetNativeSize();

            if (currentItemHandler.CurrentItem != null)
            {
                currentWeaponImage.sprite = currentItemHandler.CurrentItem.GetComponent<ItemInfo>().ItemData.slotData.itemImageSide;
                Vector3 scale = currentItemHandler.CurrentItem.GetComponent<ItemInfo>().ItemData.slotData.slotImageScaleSide;
                currentWeaponImage.transform.localScale = new Vector3(scale.x/4, scale.y/4, 1);
            }
            else
                currentWeaponImage.color = new Color(currentWeaponImage.color.r, currentWeaponImage.color.g, currentWeaponImage.color.b, 0);

            if (grenadesInInventory.Length > 0 && grenadesInInventory[0] != null)
            {
                grenadeButton.SetActive(true);

                grenadeButton.transform.GetChild(0).GetComponent<Image>().sprite = grenadesInInventory[0].GetComponent<ItemInfo>().ItemData.slotData.itemImageSide;
                grenadeButton.transform.GetChild(0).GetComponent<Image>().SetNativeSize();
            }
            else
            {
                grenadeButton.SetActive(false);
            }
        } 
        else
        {
            currentWeaponImage.color = new Color(currentWeaponImage.color.r, currentWeaponImage.color.g, currentWeaponImage.color.b, 0);

            grenadeButton.SetActive(false);
        }

        if (inventoryPanel.GetComponent<CanvasGroup>().alpha == 1 ||
            pauseMenuePanel.GetComponent<CanvasGroup>().alpha == 1)
        {
            GameInstance.instance.canDoItemAction = false;
        }
    }

    public void ToggelPauseMenue()
    {
        Action toggleOnAction = () =>
        {
            Time.timeScale = 0f;
            GameInstance.instance.canDoItemAction = false;
            mapCam.SetActive(true);
        };
        Action toggleOffAction = () =>
        {
            Time.timeScale = 1f;
            GameInstance.instance.canDoItemAction = true;
            mapCam.SetActive(false);
        };

        ToggleScreen(pauseMenuePanel, toggleOnAction, toggleOffAction);
    }

    public void ToggelInventory()
    {
        Action toggleOnAction = () =>
        {
            GameInstance.instance.canDoItemAction = false;

            foreach (ItemSpawner spawner in inventoryPanel.GetComponentsInChildren<ItemSpawner>())
            {
                spawner.RefreshInventory();
            }
        };
        Action toggleOffAction = () =>
        {
            GameInstance.instance.canDoItemAction = true;
        };

        if (!IsToggeled(inventoryPanel) && GameInstance.instance.canDoItemAction)
            ToggleScreen(inventoryPanel, toggleOnAction, toggleOffAction);
        else if (IsToggeled(inventoryPanel))
            ToggleScreen(inventoryPanel, toggleOnAction, toggleOffAction);
    }

    private bool IsToggeled(GameObject screen)
    {
        return screen.GetComponent<CanvasGroup>().alpha == 1;
    }
    private void ToggleScreen(GameObject screen, Action toggleOnAction, Action toggleOffAction)
    {
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();
        if (IsToggeled(screen))
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            toggleOffAction?.Invoke();
        } 
        else if (!IsToggeled(screen))
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            toggleOnAction?.Invoke();
        }
    }

    public void BackToMainMenue()
    {
        ToggelPauseMenue();

        FindObjectOfType<PlayerHandler>().GetComponent<HealtComponent>().currentHealth = -1;
    }
}
