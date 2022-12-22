using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;
using UnityEngine.InputSystem;

public class PlayerHandler : MonoBehaviour
{
    public GameObject hitEffectPrefab;

    [HideInInspector]
    public HealtComponent healthComponent;

    public InfoBar infoBarScript;

    public Animator bodyAimator;

    public GameObject deadBody;

    public int earnedCash = 0;
    public int earnedBrainCells = 0;

    private float step = 0f;

    private bool usedExtraLife = false;

    [HideInInspector]
    public ItemSpawner inventory;

    public CurrentItemHandler currentItemHandler;

    [HideInInspector]
    public bool holdingShootButton;

    [Inject]
    private GameManager gameManager;

    private void Awake()
    {
        healthComponent = GetComponent<HealtComponent>();

        if (GameInstance.instance.equipmentValues.CurrentSetContaisPerk(ItemData.PerkData.PerkTyp.ExtraSpeed))
        {
            healthComponent.normalSpeed = 3;
            healthComponent.runSpeed = 4;
        }

        if (GameInstance.instance.equipmentValues.CurrentSetContaisPerk(ItemData.PerkData.PerkTyp.ExtraHealth))
        {
            healthComponent.startHealth = 125;
        }
    }

    void Start()
    {
        GetComponent<Movement>().enabled = false;

        GameInstance.instance.canDoItemAction = true;

        deadBody.SetActive(false);

        healthComponent.onDie = Die;

        currentItemHandler.addEvent.AddListener(() =>
        {
            if (inventory != null)
            {
                inventory.RefreshInventory();
            }
        });

        currentItemHandler.CreateItemList(GameInstance.instance.inventoryValues.inventory);

        infoBarScript.SetHealtValue(healthComponent.currentHealth);

        StartCoroutine(StartDelay());
    }

    private void Update()
    {
        infoBarScript.SetHealtValue(healthComponent.currentHealth);

        if (currentItemHandler.CurrentItem != null && GameInstance.instance.canDoItemAction && !GameInstance.instance.debugValues.isMobile && Mouse.current.scroll.IsActuated())
        {
            currentItemHandler.SwitchWeapon(gameManager.mouseScrollY);
        }

        if (GetComponent<Movement>() != null && currentItemHandler.CurrentItem != null 
            && currentItemHandler.CurrentItem.GetComponent<WeaponBase>() != null)
        {
            WeaponBase weaponBase = currentItemHandler.CurrentItem.GetComponent<WeaponBase>();

            if (!GameInstance.instance.debugValues.isMobile)
            {
                weaponBase.IsShoot = Mouse.current.leftButton.IsPressed();

                if (Keyboard.current.rKey.wasPressedThisFrame)
                    weaponBase.Reload();

                if (Keyboard.current.cKey.wasPressedThisFrame)
                    currentItemHandler.Punch();
            }
        }
    }

    public void SetHoldingShotButton(bool newBool)
    {
        holdingShootButton = newBool;
    }

    public void Die()
    {
        if (!usedExtraLife && 
            GameInstance.instance.equipmentValues.CurrentSetContaisPerk(ItemData.PerkData.PerkTyp.ExtraLife))
        {
            healthComponent.AddFullHealth();

            usedExtraLife = true;
            return;
        }

        currentItemHandler.transform.parent.GetComponent<Animator>().Play("DeadIdle");

        FindObjectOfType<GameMenueHandler>().inventoryPanel.SetActive(false);

        GameInstance.instance.canDoItemAction = false;

        deadBody.SetActive(true);

        Destroy(transform.GetComponent<Movement>());
        Destroy(transform.GetComponent<Rigidbody2D>());
        Destroy(transform.GetComponent<BoxCollider2D>());

        currentItemHandler.DropAllItems(transform.position);

        if (currentItemHandler != null)
        {
            foreach (SpriteRenderer renderer in currentItemHandler.transform.parent.parent.GetComponentsInChildren<SpriteRenderer>())
            {
                if (renderer.GetComponent<ItemInfo>() == null)
                {
                    renderer.color = new Color(0, 0, 0, 0);
                }
            }
        }

        StartCoroutine(DisapearDelay());
    }

    IEnumerator StartDelay()
    {
        Array.ForEach(GetComponentsInChildren<SpriteRenderer>(), renderer => renderer.material.SetFloat("_Fade", 0));

        yield return new WaitForSeconds(1f);

        Array.ForEach(GetComponentsInChildren<SpriteRenderer>(), renderer => StartCoroutine(Apear(renderer)));
    }

    IEnumerator DisapearDelay()
    {
        yield return new WaitForSeconds(1f);

        Array.ForEach(GetComponentsInChildren<SpriteRenderer>(), renderer => StartCoroutine(Disapear(renderer.material)));
    }

    private IEnumerator Apear(SpriteRenderer renderer)
    {
        renderer.material.SetColor("_Color", new Color(0, 11.3474016f, 18.7643986f, 1));
        renderer.material.SetFloat("_Fade", 0);

        renderer.gameObject.SetActive(true);

        while (step < 1)
        {
            step += Time.deltaTime * 0.08f;

            renderer.material.SetFloat("_Fade", step);

            yield return null;
        }

        renderer.material.SetFloat("_Fade", 1);

        if (GetComponent<Movement>() != null) 
            GetComponent<Movement>().enabled = true;
    }

    private IEnumerator Disapear(Material material)
    {
        material.SetFloat("_Fade", 1);

        while (step > 0)
        {
            step -= Time.deltaTime * 0.0006f;

            material.SetFloat("_Fade", step);

            yield return null;
        }

        material.SetFloat("_Fade", 0);

        yield return new WaitForSeconds(0.25f);

        GameObject deathScreen = Resources.FindObjectsOfTypeAll<CountUpManage>()[0].gameObject;
        deathScreen.SetActive(true);
        //Cash
        GameInstance.instance.playerValues.cash += earnedCash;
        deathScreen.GetComponent<CountUpManage>().values[0] = earnedCash;
        //BrainCells
        GameInstance.instance.playerValues.brainCells += earnedBrainCells;
        deathScreen.GetComponent<CountUpManage>().values[1] = earnedBrainCells;

        Destroy(gameObject);
    }
}
