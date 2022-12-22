using Zenject;
using UnityEngine;
using System.Collections.Generic;

public class RocketWeapon : WeaponBase
{
    public Sprite noRocketState;

    protected override void Awake()
    {
        base.Awake();

        base.SetWeaponUp(itemData, transform);

        muzzle = transform.GetChild(0);
    }

    protected override void Update()
    {
        base.Update();

        if (itemData.weaponData.currentClipAmmo <= 0)
        {
            GetComponent<SpriteRenderer>().sprite = noRocketState;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = itemData.slotData.itemImageTop;
        }
    }


    public override void Attack()
    {
        base.Attack();

        if (itemData.weaponData.currentClipAmmo > 0)
        {
            AudioManager.instance.Play(itemData.weaponData.attackSounds[Random.Range(0, itemData.weaponData.attackSounds.Length)], transform);

            animator.SetTrigger("Attack");

            GameObject rocket = Instantiate(itemData.globalData.projectile, muzzle.position, muzzle.rotation);
            rocket.GetComponent<Rigidbody2D>().AddForce(muzzle.up * 20, ForceMode2D.Impulse);
            rocket.GetComponent<Prokectile>().SetWeaponData(itemData, targetTags);

            Camera.main.transform.parent.GetComponent<CameraHandler>().Shake();

            itemData.weaponData.currentClipAmmo -= 1;
        }
        else
        {
            AudioManager.instance.Play("EmptyMag", transform);
        }
    }
}
