using Zenject;
using UnityEngine;
using System.Collections.Generic;

public class AutomaticWeapon : WeaponBase
{
    protected override void Awake()
    {
        base.Awake();

        base.SetWeaponUp(itemData, transform);

        muzzle = transform.GetChild(0);
    }

    protected override void Update()
    {
        base.Update();

        if (!GetComponent<DropItem>().IsDropped)
        {
            if (IsShoot && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / itemData.weaponData.firerate;

                Shoot();
            }
        }
    }

    public void Shoot()
    {
        if (itemData.weaponData.currentClipAmmo > 0)
        {
            AudioManager.instance.Play(itemData.weaponData.attackSounds[Random.Range(0, itemData.weaponData.attackSounds.Length)], transform);

            animator.SetTrigger("Attack");

            GameObject bullet = Instantiate(itemData.globalData.projectile, muzzle.position, muzzle.rotation);
            bullet.GetComponent<Rigidbody2D>().AddForce(muzzle.up * 20, ForceMode2D.Impulse);
            bullet.GetComponent<Prokectile>().SetWeaponData(itemData, targetTags);

            Camera.main.transform.parent.GetComponent<CameraHandler>().Shake();

            itemData.weaponData.currentClipAmmo -= 1;
        }
        else
        {
            AudioManager.instance.Play("EmptyMag", transform);
        }
    }    
}
