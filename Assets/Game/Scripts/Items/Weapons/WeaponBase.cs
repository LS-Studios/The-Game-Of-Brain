using System.Collections;
using Zenject;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(ItemInfo), typeof(DropItem))]
public abstract class WeaponBase : MonoBehaviour
{
    [HideInInspector]
    public ItemData itemData;

    protected Transform muzzle;

    protected float nextTimeToFire = 0f;

    protected Animator animator;

    private bool isShoot;

    public bool IsShoot
    {
        get
        {
            if (currentItemHandler.canDoAction)
                return isShoot;
            else
                return false;
        }

        set
        {
            isShoot = value;
        }
    }

    protected bool hasReloaded = true;

    [HideInInspector]
    public Transform weaponHolder;

    [HideInInspector]
    public bool isReload = false;

    [HideInInspector]
    public CurrentItemHandler currentItemHandler;

    protected GameManager gameManager;

    protected GameMenueHandler gameMenueHandler;

    public List<string> targetTags = new List<string>();

    [Inject]
    private void Contruct(GameManager gameManager, GameMenueHandler gameMenueHandler)
    {
        this.gameManager = gameManager;
        this.gameMenueHandler = gameMenueHandler;
    }

    protected virtual void Awake()
    {
        itemData = GetComponent<ItemInfo>().ItemData;
    }

    protected virtual void Start()
    {
        if (GetComponent<ZenAutoInjecter>() == null)
            gameObject.AddComponent<ZenAutoInjecter>().ContainerSource = ZenAutoInjecter.ContainerSources.SceneContext;
    }        

    protected virtual void Update()
    {
        itemData = GetComponent<ItemInfo>().ItemData;

        if (!Mathf.Approximately(itemData.weaponData.currentClipAmmo, Mathf.RoundToInt(itemData.weaponData.currentClipAmmo)))
        {
            itemData.weaponData.currentClipAmmo = Mathf.Round(itemData.weaponData.currentClipAmmo * 100) / 100;
        }

        if (!Mathf.Approximately(itemData.weaponData.currentAmmo, Mathf.RoundToInt(itemData.weaponData.currentAmmo)))
        {
            itemData.weaponData.currentAmmo = Mathf.Round(itemData.weaponData.currentAmmo * 100) / 100;
        }
    }

    public void SetWeaponUp(ItemData itemData, Transform myTransform)
    {
        weaponHolder = myTransform.parent;
    }

    public void CallAttack()
    {
        if (currentItemHandler.canDoAction)
            Attack();
    }

    protected virtual void OnEnable()
    {
        
    }

    public void CallPunchWithWeapon()
    {
        if (GetComponent<MeeleWeapon>() == null && currentItemHandler.canDoAction)
        {
            if (!GetComponent<DropItem>().IsDropped && gameObject.activeSelf)
                StartCoroutine(PunchWithWeapon());
        } else
        {
            CallAttack();
        }
    }

    private IEnumerator PunchWithWeapon()
    {
        yield return new WaitForSeconds(0.35f);

        Collider2D collider2D = GetComponent<BoxCollider2D>();

        if (collider2D != null)
        {
            List<Collider2D> hitCollider = new List<Collider2D>();

            collider2D.OverlapCollider(new ContactFilter2D(), hitCollider);

            hitCollider.ForEach(collider => {
                if (collider.GetComponent<IDamageable>() != null && targetTags.Contains(collider.tag))
                {
                    GameObject hitEffect = Instantiate(itemData.globalData.hitEffect, transform.position + transform.up, Quaternion.identity);
                    hitEffect.GetComponent<HitEffect>().hitEffectData = collider.transform.GetComponent<EffectAssinger>().hitEffectData;

                    GameInstance.instance.referenceValues.globalData.
                    CreateGPAddPoint(collider.bounds.ClosestPoint(transform.position),
                    gameManager.GetComponent<RewardComponent>().GamePointReward);

                    collider.GetComponent<IDamageable>().TakeDamage(15*currentItemHandler.damageMultiply, true);
                }
            });
        }
    }

    public virtual void Attack()
    {

    }

    public virtual void Reload() 
    {
        if (itemData.weaponData.maxAmmo > 0 && itemData.weaponData.currentAmmo > 0 && currentItemHandler.canDoAction)
        {
            if (itemData.weaponData.currentAmmo > 0 && itemData.weaponData.currentClipAmmo != itemData.weaponData.maxClipAmmo && !isShoot)
            {
                if (hasReloaded)
                {
                    if (!itemData.weaponData.isSingleReloader)
                    {
                        AudioManager.instance.Play(itemData.weaponData.reloadSounds[0], transform);
                        itemData.weaponData.currentReloadSound = itemData.weaponData.reloadSounds[0];

                        currentItemHandler.reloadAction?.Invoke();
                    }
                }

                if (!isReload)
                    StartCoroutine(StartReloadSlider(itemData));
            }
        }
    }

    public void SetCurrentAmmo(float clipToAdd, float ammoToAdd)
    {
        if (itemData != null)
        {
            itemData.weaponData.currentClipAmmo = clipToAdd;
            itemData.weaponData.currentAmmo = ammoToAdd;
        }
        else
        {
            itemData.currentAmmount = (int)ammoToAdd;
        }
    }

    public IEnumerator StartReloadSlider(ItemData itemData)
    {
        hasReloaded = false;
        isReload = true;

        //Play one animation or single bullet reload animation
        if (!itemData.weaponData.isSingleReloader)
        {
            AudioSource reloadSound = AudioManager.instance.GetAudioSourceFromTransform(itemData.weaponData.currentReloadSound, transform);
            reloadSound.pitch *= itemData.weaponData.reloadTimeMultiply;

            float speed = (1 / reloadSound.clip.length) * reloadSound.pitch * itemData.weaponData.reloadTimeMultiply;

            if (GameInstance.instance.equipmentValues.CurrentSetContaisPerk(ItemData.PerkData.PerkTyp.FasterReload) 
                && currentItemHandler.GetComponentInParent<PlayerHandler>() != null)
                reloadSound.pitch *= 1.6f;

            animator.SetTrigger("Reload");
            animator.speed = speed;

            float startTime = Time.time;
            float slideTime = Time.time + (1 / speed);

            while (Time.time < slideTime)
            {
                if (currentItemHandler.GetComponentInParent<PlayerHandler>() != null)
                    gameMenueHandler.reloadSlider.value = Mathf.InverseLerp(startTime, slideTime, Time.time);

                yield return null;
            }

            if (currentItemHandler.GetComponentInParent<PlayerHandler>() != null)
                gameMenueHandler.reloadSlider.value = 0;

            hasReloaded = true;
            isReload = false;

            animator.speed = 1;

            ReloadWeaponAmmo();
        }
        else
        {
            IEnumerator AddSingleLoop()
            {
                int addedAmmo = 0;

                int ammountToAdd = 0;
                if (itemData.weaponData.currentClipAmmo > 0 && (itemData.weaponData.maxClipAmmo - itemData.weaponData.currentClipAmmo) <= itemData.weaponData.currentAmmo)
                {
                    ammountToAdd = (int)(itemData.weaponData.maxClipAmmo - itemData.weaponData.currentClipAmmo);
                }
                else if (itemData.weaponData.currentClipAmmo > 0 && (itemData.weaponData.maxClipAmmo - itemData.weaponData.currentClipAmmo) >= itemData.weaponData.currentAmmo)
                {
                    ammountToAdd = (int)itemData.weaponData.currentAmmo;
                }
                else if (itemData.weaponData.currentClipAmmo <= 0 && itemData.weaponData.currentAmmo <= itemData.weaponData.maxClipAmmo)
                {
                    ammountToAdd = (int)itemData.weaponData.currentAmmo;
                }
                else if (itemData.weaponData.currentClipAmmo <= 0 && itemData.weaponData.currentAmmo > itemData.weaponData.maxClipAmmo)
                {
                    ammountToAdd = (int)itemData.weaponData.maxClipAmmo;
                }

                AudioSource reloadSound;

                while (addedAmmo < ammountToAdd)
                {
                    addedAmmo++;
                    itemData.weaponData.currentClipAmmo += 1;
                    itemData.weaponData.currentAmmo -= 1;

                    //Play single bullet sound
                    itemData.weaponData.currentReloadSound = itemData.weaponData.reloadSounds[0];
                    reloadSound = AudioManager.instance.Play(itemData.weaponData.reloadSounds[0], transform).source;

                    //Play single bullet animation
                    animator.Play(itemData.weaponData.reloadAnimations[0]);

                    if (GameInstance.instance.equipmentValues.CurrentSetContaisPerk(ItemData.PerkData.PerkTyp.FasterReload) && currentItemHandler.GetComponentInParent<PlayerHandler>() != null)
                    {
                        reloadSound.pitch *= 1.6f;

                        yield return new WaitForSeconds(itemData.weaponData.singleReloadTime * itemData.weaponData.reloadTimeMultiply / 1.6f);
                    }
                    else 
                        yield return new WaitForSeconds(itemData.weaponData.singleReloadTime * itemData.weaponData.reloadTimeMultiply);
                }

                //Play end reload sound
                itemData.weaponData.currentReloadSound = itemData.weaponData.reloadSounds[1];
                reloadSound = AudioManager.instance.Play(itemData.weaponData.reloadSounds[1], transform).source;

                if (GameInstance.instance.equipmentValues.CurrentSetContaisPerk(ItemData.PerkData.PerkTyp.FasterReload) && currentItemHandler.GetComponentInParent<PlayerHandler>() != null)
                {
                    animator.speed *= 1.6f;
                    reloadSound.pitch *= 1.6f;
                }

                //Play end reload animation
                animator.Play(itemData.weaponData.reloadAnimations[1]);

                yield return new WaitForSeconds(reloadSound.clip.length * reloadSound.pitch * itemData.weaponData.reloadTimeMultiply);

                hasReloaded = true;
                isReload = false;

                animator.speed = 1;
            }

            StartCoroutine(AddSingleLoop());
        }          
    }

    public void StopReload()
    {
        if (isReload)
        {
            StopAllCoroutines();
            if (AudioManager.instance.GetAudioSourceFromTransform(itemData.weaponData.currentReloadSound, transform) != null)
            {
                AudioManager.instance.GetAudioSourceFromTransform(itemData.weaponData.currentReloadSound, transform).Stop();
            }
            animator.SetTrigger("StopReload");
            hasReloaded = true;
            isReload = false;

            if (currentItemHandler.GetComponentInParent<PlayerHandler>() != null)
                gameMenueHandler.reloadSlider.value = 0;
        }
    }

    private void ReloadWeaponAmmo()
    {
        if (itemData.weaponData.currentClipAmmo != itemData.weaponData.maxClipAmmo)
        {
            if (itemData.weaponData.currentClipAmmo > 0 && (itemData.weaponData.maxClipAmmo - itemData.weaponData.currentClipAmmo) <= itemData.weaponData.currentAmmo)
            {
                itemData.weaponData.currentAmmo -= (itemData.weaponData.maxClipAmmo - itemData.weaponData.currentClipAmmo);
                itemData.weaponData.currentClipAmmo = itemData.weaponData.maxClipAmmo;
            }
            else if (itemData.weaponData.currentClipAmmo > 0 && (itemData.weaponData.maxClipAmmo - itemData.weaponData.currentClipAmmo) >= itemData.weaponData.currentAmmo)
            {
                itemData.weaponData.currentClipAmmo += itemData.weaponData.currentAmmo;
                itemData.weaponData.currentAmmo = 0;
            }
            else if (itemData.weaponData.currentClipAmmo <= 0 && itemData.weaponData.currentAmmo <= itemData.weaponData.maxClipAmmo)
            {
                itemData.weaponData.currentClipAmmo = itemData.weaponData.currentAmmo;
                itemData.weaponData.currentAmmo = 0;
            }
            else if (itemData.weaponData.currentClipAmmo <= 0 && itemData.weaponData.currentAmmo > itemData.weaponData.maxClipAmmo)
            {
                itemData.weaponData.currentAmmo -= itemData.weaponData.maxClipAmmo;
                itemData.weaponData.currentClipAmmo = itemData.weaponData.maxClipAmmo;
            }
        }
    }

    public bool NeedAmmo()
    {
        bool needAmmo = false;

        if (itemData.weaponData.currentAmmo < itemData.weaponData.maxAmmo
            || itemData.weaponData.currentClipAmmo < itemData.weaponData.maxClipAmmo)
        {
            needAmmo = true;
        }

        return needAmmo;
    }

    public bool haveAmmo()
    {
        return itemData.weaponData.currentAmmo > 0 && itemData.weaponData.currentClipAmmo > 0;
    }

    public void SetAnimator(Animator animator)
    {
        this.animator = animator;
    }

    protected virtual void OnDestroy()
    {
        if (currentItemHandler != null)
            currentItemHandler.hitAction -= CallPunchWithWeapon;
    }
}
