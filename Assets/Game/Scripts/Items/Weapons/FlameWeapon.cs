using Zenject;
using UnityEngine;
using System.Collections.Generic;

public class FlameWeapon : WeaponBase
{
    private ParticleSystem fire;
    protected override void Awake()
    {
        base.Awake();

        base.SetWeaponUp(itemData, transform);

        muzzle = transform.GetChild(0);

        fire = transform.GetChild(0).GetChild(0).transform.GetComponent<ParticleSystem>();
    }

    protected override void Update()
    {
        base.Update();

        if (!GetComponent<DropItem>().IsDropped)
        {
            string randomSound = itemData.weaponData.attackSounds[Random.Range(0, itemData.weaponData.attackSounds.Length)];

            if (IsShoot && itemData.weaponData.currentClipAmmo > 0)
            {
                if (!fire.isPlaying) fire.Play();
                fire.GetComponent<Prokectile>().SetWeaponData(itemData, targetTags);
                itemData.weaponData.currentClipAmmo -= Time.deltaTime * 12;
                

                if (AudioManager.instance.GetPlayingSound(randomSound) == null)
                    AudioManager.instance.Play(randomSound, transform);
            }
            else
            {
                if (fire.isPlaying) fire.Stop();

                if (AudioManager.instance.GetPlayingSound(randomSound) != null)
                    AudioManager.instance.GetPlayingSound(randomSound).source.Stop();
            }

            if (itemData.weaponData.currentClipAmmo < 0)
                itemData.weaponData.currentClipAmmo = 0;
        } else
        {
            if (fire.isPlaying) fire.Stop();
        }
    }
}
