using Zenject;
using UnityEngine;
using System.Collections.Generic;

public class Crossbow : WeaponBase
{
    public Animator crossbowAnimator;

    protected override void Awake()
    {
        base.Awake();

        base.SetWeaponUp(itemData, transform);

        muzzle = transform.GetChild(0);
    }

    protected override void Start()
    {
        base.Start();

        currentItemHandler.reloadAction += PlayReloadAnimation;

        Sound reloadSound = AudioManager.instance.GetSoundFromList(itemData.weaponData.attackSounds[0]);
        reloadSound.pitch *= itemData.weaponData.reloadTimeMultiply;

        float speed = (1 / reloadSound.clip.length) * reloadSound.pitch * itemData.weaponData.reloadTimeMultiply;

        crossbowAnimator.SetFloat("ReloadSpeed", speed);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (itemData.weaponData.currentAmmo > 0)
            crossbowAnimator.Play("CrossbowAmmo");
        else
            crossbowAnimator.Play("CrossbowNoAmmo");
    }

    public override void Attack()
    {
        base.Attack();

        if (itemData.weaponData.currentClipAmmo > 0)
        {
            AudioManager.instance.Play(itemData.weaponData.attackSounds[Random.Range(0, itemData.weaponData.attackSounds.Length)], transform);

            animator.SetTrigger("Attack");

            crossbowAnimator.Play("CrossbowShoot");

            GameObject arrow = Instantiate(itemData.globalData.projectile, muzzle.position, muzzle.rotation);
            arrow.GetComponent<Rigidbody2D>().AddForce(muzzle.up * 20, ForceMode2D.Impulse);
            arrow.GetComponent<Prokectile>().SetWeaponData(itemData, targetTags);

            Camera.main.transform.parent.GetComponent<CameraHandler>().Shake();

            itemData.weaponData.currentClipAmmo -= 1;
        }
        else
        {
            AudioManager.instance.Play("EmptyMag", transform);
        }
    }

    private void PlayReloadAnimation()
    {
        if (gameObject.activeSelf)
            crossbowAnimator.Play("CrossbowReload");
    }

    protected override void OnDestroy()
    {
        currentItemHandler.reloadAction -= PlayReloadAnimation;
    }
}
