using Zenject;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public TopDownActions controlls;

    [HideInInspector]
    public float mouseScrollY;
    [HideInInspector]
    public bool isAim;

    public int survivedWaves = 0;

    public SlideButton skipButton;
    public WaveSpawner waveSpawner;
    public TextMeshProUGUI waveCountDownText;
    public TextMeshProUGUI survivedWavesText;

    private Movement movement;

    public PlayerHandler playerHandler;

    [Inject]
    private GameMenueHandler gameMenueHandler;

    void Awake()
    {
        controlls = new TopDownActions();

        movement = FindObjectOfType<Movement>(); 
            
        skipButton.manualAction.AddListener(() => waveSpawner.WaveCountdown = 0);
        waveSpawner.enabled = GameInstance.instance.debugValues.activateWaveSpawner;
    }

    private void Update()
    {
        if (!GameInstance.instance.debugValues.isMobile && playerHandler.currentItemHandler != null)
        {
            isAim = Mouse.current.rightButton.isPressed;
            movement.SetRun(Keyboard.current.shiftKey.isPressed);

            mouseScrollY = Mouse.current.scroll.ReadValue().y;

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
                gameMenueHandler.ToggelPauseMenue();

            if (Keyboard.current.bKey.wasPressedThisFrame)
                gameMenueHandler.ToggelInventory();

            if (Keyboard.current.eKey.wasPressedThisFrame)
                ThrowGrenade();

            if (Mouse.current.leftButton.wasPressedThisFrame)
                playerHandler.currentItemHandler.CallCurrentItemAction();
        }

        if (waveSpawner.enabled && waveSpawner.State == WaveSpawner.SpawnState.COUNTING)
        {
            waveCountDownText.text = waveSpawner.WaveCountdown.ToString("F1");
            skipButton.gameObject.SetActive(true);
        } else if (waveSpawner.enabled)
        {
            waveCountDownText.text = "";
            skipButton.gameObject.SetActive(false);
            GameInstance.instance.canDoItemAction = true;
        }

        if (survivedWaves > 0)
            survivedWavesText.text = survivedWaves.ToString();
        else
            survivedWavesText.text = "";

        if (!waveSpawner.enabled)
        {
            survivedWavesText.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(false);
            waveCountDownText.gameObject.SetActive(false);
        }
    }

    public void Reload()
    {
        if (playerHandler.currentItemHandler.CurrentItem.GetComponent<WeaponBase>() != null)
            playerHandler.currentItemHandler.CurrentItem.GetComponent<WeaponBase>().Reload();
    }

    public void ThrowGrenade()
    {
        movement.AimToClosestTarget();

        GameObject lastItem = playerHandler.currentItemHandler.CurrentItem;

        //Get array of grenades in inventory
        GameObject[] grenadesInInventory = Array.FindAll(playerHandler.currentItemHandler.useableItems.ToArray(), item => item.GetComponent<Grenade>() != null);

        //Throw if there are grenades
        if (grenadesInInventory.Length > 0)
        {
            GameObject grenade = grenadesInInventory[0];
            //Equip grenade
            playerHandler.currentItemHandler.ChangeItem(grenade);

            playerHandler.currentItemHandler.CallCurrentItemAction();

            if (grenade.GetComponent<WeaponBase>().itemData.currentAmmount <= 0)
            {
                playerHandler.currentItemHandler.RemoveItem(grenade);
            }

            playerHandler.currentItemHandler.ChangeItem(lastItem);
        }
    }

    #region Enable/Disable InputSystem
    private void OnEnable()
    {
        controlls.Enable();
    }
    private void OnDisable()
    {
        controlls.Disable();
    }
    #endregion

    #region selectWehele
    //private void EnableItemWheel()
    //{
    //    weaponWheel.SetActive(true);
    //}
    //private void DisableItemWheel()
    //{
    //    CurrentWeapon currentWeapon = GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetChild(1).GetComponent<CurrentWeapon>();
    //    currentWeapon.UpdateEnumViaNumber();
    //    weaponWheel.SetActive(false);
    //} 
    #endregion

    public void Scan()
    {
        AstarPath.active.Scan();
    }
}
