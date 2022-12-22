using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterColorHandler;
using static UnityEditor.Progress;

public class HumanEnemy : Enemy
{
    public CurrentItemHandler currentItemHandler;

    [Header("Shoot values")]
    public float shootRange = 5f;
    public float runRange = 8f;
    public float damageMultiply = 0.5f;
    public List<GameObject> items = new List<GameObject>();
    public bool randomItems = false;

    public bool dropItemOnDeath = false;

    private int lastItemCount = 0;

    protected override void Awake()
    {
        base.Awake();

        if (randomItems)
        {
            items.Clear();

            List<GameObject> allWeapons = new List<GameObject>(GameInstance.instance.referenceValues.allWeapons);

            List<GameObject> allPrimaryWeapons = allWeapons.FindAll(item => item.GetComponent<ItemInfo>().ItemData.itemCategory == ItemData.ItemCategory.PrimaryWeapon);
            GameObject randomPrimary = allPrimaryWeapons[Random.Range(0, allPrimaryWeapons.Count - 1)];
            items.Add(randomPrimary);

            List<GameObject> allSecondaryWeapons = allWeapons.FindAll(item => item.GetComponent<ItemInfo>().ItemData.itemCategory == ItemData.ItemCategory.SecondaryWeapon);
            GameObject randomSecondary = allSecondaryWeapons[Random.Range(0, allSecondaryWeapons.Count-1)];
            items.Add(randomSecondary);
        }

        currentItemHandler.damageMultiply = damageMultiply;
        currentItemHandler.useableItems = items;
    }

    protected override void Start()
    {
        base.Start();

        currentItemHandler.targetTags = targetTags;
        currentItemHandler.CreateItemList(items);

        ItemCountChanged();
    }

    protected override void Update()
    {
        base.Update();

        if (!healthComponent.isDead && target != null && aIPathScript.reachedEndOfPath)
            StartCoroutine(LookTo());       

        if (currentItemHandler.useableItems.Count != lastItemCount)
        {
            ItemCountChanged();
        }

        if (target != null && Vector2.Distance(transform.position, target.position) > runRange && readyToAttack)
        {
            healthComponent.currentSpeed = healthComponent.runSpeed;
        }
        else
        {
            healthComponent.currentSpeed = healthComponent.normalSpeed;
        }

        //With weapon
        if (currentItemHandler.CurrentItem != null)
        {
            WeaponBase currentWeapon = currentItemHandler.CurrentItem.GetComponent<WeaponBase>();

            if (currentWeapon != null)
            {
                //Reload
                if (currentWeapon.itemData.weaponData.currentClipAmmo <= 0 && currentWeapon.itemData.weaponData.weaponTyp != ItemData.WeaponData.WeaponTyp.NoAmmu
                    && currentWeapon.itemData.itemCategory != ItemData.ItemCategory.Grenade)
                {
                    currentWeapon.IsShoot = false;
                    currentWeapon.Reload();
                }

                if (currentWeapon.isReload)
                {
                    currentWeapon.IsShoot = false;
                }

                SetItemTypeDistance();

                //Change attack type if no ammo
                if (currentWeapon.itemData.weaponData.weaponTyp != ItemData.WeaponData.WeaponTyp.NoAmmu &&
                    currentWeapon.itemData.dataTyp == ItemData.DataTyp.Weapondata &&
                    currentWeapon.itemData.weaponData.currentClipAmmo <= 0 &&
                    currentWeapon.itemData.weaponData.currentAmmo <= 0
                    && readyToAttack)
                {
                    if (currentItemHandler.useableItems.Count > 1 &&
                        (currentItemHandler.useableItems.Find(item => item.GetComponent<WeaponBase>().haveAmmo()) != null || currentItemHandler.useableItems.Find(item => item.GetComponent<WeaponBase>().itemData.generalData.meeleWeaponType == ItemData.GeneralData.MeeleWeaponType.Shield) != null ||
                        currentItemHandler.useableItems.Find(item => item.GetComponent<WeaponBase>().itemData.currentAmmount > 0 && item.GetComponent<WeaponBase>().itemData.dataTyp == ItemData.DataTyp.Grenadedata) != null))
                    {
                        currentItemHandler.SwitchWeapon(1);
                    }

                    aIPathScript.whenCloseToDestination = Pathfinding.CloseToDestinationMode.ContinueToExactDestination;
                }
                else if (currentWeapon.itemData.weaponData.weaponTyp == ItemData.WeaponData.WeaponTyp.NoAmmu ||
                    currentWeapon.itemData.itemCategory == ItemData.ItemCategory.Grenade || currentWeapon.itemData.weaponData.currentClipAmmo > 0
                    && readyToAttack)
                {
                    aIPathScript.whenCloseToDestination = Pathfinding.CloseToDestinationMode.Stop;

                    //Attack
                    if (aIPathScript.reachedEndOfPath && target != null && target.GetComponent<HealtComponent>().currentHealth > 0)
                    {
                        IEnumerator DoDamage()
                        {
                            doDamageDelay = true;
                            yield return new WaitForSeconds(attackRate);
                            currentWeapon.Attack();

                            doDamageDelay = false;
                        }

                        currentWeapon.IsShoot = true;

                        if (!doDamageDelay)
                            StartCoroutine(DoDamage());
                    }
                    else
                    {
                        currentWeapon.IsShoot = false;
                    }
                }
            }
        }
    }

    public override void Die()
    {
        base.Die();

        if (dropItemOnDeath)
            currentItemHandler.DropAllItems(transform.position);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.tag != "Enemy" && collision.collider.tag != "NoDamage")
        {
            IDamageable iDamageable = collision.transform.GetComponent<IDamageable>();
            if (iDamageable != null)
            {
                if (spawnedEffects <= 0)
                {
                    GameObject hitEffect = Instantiate(GameInstance.instance.referenceValues.globalData.hitEffect, collision.transform.position, collision.transform.rotation);

                    spawnedEffects++;

                    if (collision.transform.GetComponent<EffectAssinger>() != null)
                        hitEffect.GetComponent<HitEffect>().hitEffectData = collision.transform.GetComponent<EffectAssinger>().hitEffectData;
                    else
                        hitEffect.GetComponent<HitEffect>().hitEffectData = new HitEffect.HitEffectData(true);

                    hitEffect.GetComponent<HitEffect>().OnDestroyAction += () => spawnedEffects--;
                }

                IEnumerator DoDamage()
                {
                    doDamageDelay = true;
                    yield return new WaitForSeconds(attackRate);
                    currentItemHandler.Punch();

                    if (currentItemHandler.useableItems.Count > 0)
                        iDamageable.TakeDamage(damage, true);

                    doDamageDelay = false;
                }

                if (!doDamageDelay)
                    StartCoroutine(DoDamage());
            }
        }
    }

    private IEnumerator LookTo()
    {
        float val = 0;

        while (val < 1)
        {
            if (target != null)
                transform.rotation = Quaternion.Lerp(transform.rotation, LSUtils.GetRotationTowardsObject(gameObject, target.gameObject), val);

            val += Time.deltaTime;

            yield return null;
        }
    }

    private void ItemCountChanged()
    {
        lastItemCount = currentItemHandler.useableItems.Count;

        if (currentItemHandler.useableItems.Count > 0)
        {
            aIPathScript.whenCloseToDestination = Pathfinding.CloseToDestinationMode.Stop;
            SetItemTypeDistance();
        }
        else
        {
            aIPathScript.whenCloseToDestination = Pathfinding.CloseToDestinationMode.ContinueToExactDestination;
            aIPathScript.endReachedDistance = 0;
        }
    } 

    private void SetItemTypeDistance()
    {
        WeaponBase currentWeapon = currentItemHandler.CurrentItem.GetComponent<WeaponBase>();

        if (currentItemHandler.CurrentItem != null && currentWeapon.itemData.dataTyp == ItemData.DataTyp.Weapondata 
            && currentWeapon.itemData.weaponData.weaponTyp == ItemData.WeaponData.WeaponTyp.NoAmmu)
            aIPathScript.endReachedDistance = 1.5f;
        else
            aIPathScript.endReachedDistance = shootRange;
    }
}
