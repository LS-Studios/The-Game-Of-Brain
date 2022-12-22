using Zenject;
using UnityEngine;
using System.Collections.Generic;

public class GrenadeLauncher : WeaponBase
{
    public ItemInfo grenadeToSpawnData;
    protected override void Awake()
    {
        base.Awake();

        base.SetWeaponUp(itemData, transform);

        muzzle = transform.GetChild(0);
    }

    public override void Attack()
    {
        base.Attack();

        if (itemData.weaponData.currentClipAmmo > 0)
        {
            AudioManager.instance.Play(itemData.weaponData.attackSounds[Random.Range(0, itemData.weaponData.attackSounds.Length)], transform);

            animator.SetTrigger("Attack");

            GameObject grenade = Instantiate(itemData.globalData.grenadePrefab, muzzle.position, muzzle.rotation);
            grenade.GetComponent<Rigidbody2D>().AddForce(muzzle.up * 8, ForceMode2D.Impulse);
            grenade.GetComponent<GrenadeProjectile>().itemData = grenadeToSpawnData.ItemData;
            itemData.weaponData.currentClipAmmo -= 1;
        }
        else
        {
            AudioManager.instance.Play("EmptyMag", transform);
        }
    }
}
