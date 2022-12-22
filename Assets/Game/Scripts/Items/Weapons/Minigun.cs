using Zenject;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Minigun : WeaponBase
{
    private bool hasLoadedUp = false;

    private bool isLoadingUp = false;
    private AudioSource loadUpSource;

    private AudioSource loadDownSource;

    private List<Coroutine> animationCoroutines = new List<Coroutine>();

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
            //Play start and end shooot sound of minigun
            if (IsShoot && !hasLoadedUp && !isLoadingUp)
            {
                if (loadDownSource != null)
                    loadDownSource.Stop();

                isLoadingUp = true;
                loadUpSource = AudioManager.instance.Play("MiniGunLoadUp", transform).source;

                StartCoroutine(LoadUpDelay());

                IEnumerator LoadUpDelay()
                {
                    while (loadUpSource != null)
                    {
                        yield return null;
                    }

                    isLoadingUp = false;
                    hasLoadedUp = true;
                }
            } 
            else if (!IsShoot && hasLoadedUp)
            {
                if (loadUpSource != null)
                    loadUpSource.Stop();

                if (loadDownSource == null)
                    loadDownSource = AudioManager.instance.Play("MiniGunLoadDown", transform).source;

                hasLoadedUp = false;

                if (AudioManager.instance.GetPlayingSound("MiniGunSpinnLoop") != null)
                    AudioManager.instance.GetPlayingSound("MiniGunSpinnLoop").source.Stop();

                if (animationCoroutines.Count > 0)
                {
                    StopCoroutine(animationCoroutines[0]);
                    animationCoroutines.Clear();
                }
            }

            //Play loop animation
            if (IsShoot)
            {
                if (animationCoroutines.Count <= 0)
                    animationCoroutines.Add(StartCoroutine(LoopAnimation()));
            }

            //Play spinn loop sound
            if (hasLoadedUp && IsShoot)
            {
                if (AudioManager.instance.GetPlayingSound("MiniGunSpinnLoop") == null)
                    AudioManager.instance.Play("MiniGunSpinnLoop", transform);
            }

            //Do the shooting
            if (hasLoadedUp && IsShoot && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / itemData.weaponData.firerate;

                Shoot();
            }
        }
    }

    private IEnumerator LoopAnimation()
    {
        SpriteRenderer spriteRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        while (true)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;

            yield return new WaitForSeconds(0.1f);
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
