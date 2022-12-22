using Zenject;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ShotGun : WeaponBase
{
    public Transform[] muzzles;

    public bool doReloadStep = false;

    protected override void Awake()
    {
        base.Awake();

        base.SetWeaponUp(itemData, transform);
    }

    public override void Attack()
    {
        base.Attack();

        if (itemData.weaponData.currentClipAmmo > 0 && !doReloadStep)
        {
            AudioManager.instance.Play(itemData.weaponData.attackSounds[Random.Range(0, itemData.weaponData.attackSounds.Length)], transform);

            animator.SetTrigger("Attack");

            for (int i = 0; i < muzzles.Length; i++)
            {
                GameObject bullet = Instantiate(itemData.globalData.projectile, muzzles[i].position, muzzles[i].rotation);
                bullet.GetComponent<Rigidbody2D>().AddForce(bullet.transform.up * 20, ForceMode2D.Impulse);
                bullet.GetComponent<Prokectile>().SetWeaponData(itemData, targetTags);
            }

            Camera.main.transform.parent.GetComponent<CameraHandler>().Shake();

            itemData.weaponData.currentClipAmmo -= 1;

            IEnumerator ReloadStepDelay()
            {
                doReloadStep = true;

                yield return new WaitForSeconds(0.3f);

                Sound reloadSound = AudioManager.instance.GetSoundFromList(itemData.weaponData.reloadSounds[1]);

                //Play end reload sound
                AudioSource currentReloadSound = AudioManager.instance.Play(reloadSound, transform).source;

                if (GameInstance.instance.equipmentValues.CurrentSetContaisPerk(ItemData.PerkData.PerkTyp.FasterReload))
                {
                    animator.speed *= 1.6f;
                    currentReloadSound.pitch *= 1.6f;
                }

                //Play end reload animation
                animator.Play(itemData.weaponData.reloadAnimations[1]);

                yield return new WaitForSeconds((currentReloadSound.clip.length * (1/ currentReloadSound.pitch) * itemData.weaponData.reloadTimeMultiply)/2);
                
                doReloadStep = false;
                animator.speed = 1;
            }

            if (itemData.weaponData.isSingleReloader)
                StartCoroutine(ReloadStepDelay());

        } else
            AudioManager.instance.Play("EmptyMag", transform);
    }
}
