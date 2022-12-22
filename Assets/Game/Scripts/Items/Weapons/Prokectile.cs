using Zenject;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(RewardComponent))]
public class Prokectile : MonoBehaviour
{
    [HideInInspector]
    public ItemData itemData;

    public enum ProjectileTyp { Bullet, Rocket, Arrow }
    public ProjectileTyp projectileTyp;

    public enum ProjectileDamageTyp { Normal, Explosive, Fire }
    public ProjectileDamageTyp damageTyp = ProjectileDamageTyp.Normal;

    [ConditionalHide("bulletTyp", ProjectileDamageTyp.Fire)]
    public ParticleSystem burning;

    public int penetratingPower = 1;

    public List<string> targetTags = new List<string>();

    private GlobalData globalData;

    private void OnDestroy()
    {
       switch (damageTyp)
        {
            case ProjectileDamageTyp.Explosive:
                ParticleSystem explosion = Instantiate(globalData.explosion, transform.position, globalData.explosion.transform.rotation);
                AudioManager.instance.Play("Explosion", explosion.transform);
                Camera.main.transform.parent.GetComponent<CameraHandler>().Shake();
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject hitEffect = Instantiate(globalData.hitEffect, transform.position, transform.rotation);

        switch (damageTyp)
        {
            case ProjectileDamageTyp.Normal:

                if (targetTags.Contains(collision.transform.tag) && collision.transform.GetComponent<IDamageable>() != null)
                {
                    if (targetTags.Contains("Enemy"))
                        globalData.CreateGPAddPoint(transform.position, GetComponent<RewardComponent>().GamePointReward);

                    if (itemData.weaponData.weaponTyp.Equals(ItemData.WeaponData.WeaponTyp.Particel))
                        collision.transform.GetComponent<IDamageable>().TakeDamage(itemData.damage, false);          
                    else
                        collision.transform.GetComponent<IDamageable>().TakeDamage(itemData.damage, true);
                }

                if (collision.transform.GetComponent<EffectAssinger>() != null)
                    hitEffect.GetComponent<HitEffect>().hitEffectData = collision.transform.GetComponent<EffectAssinger>().hitEffectData;
                else
                    hitEffect.GetComponent<HitEffect>().hitEffectData = new HitEffect.HitEffectData(true);
                
                penetratingPower--;

                if (penetratingPower <= 0)
                    Destroy(gameObject);

                break;

            case ProjectileDamageTyp.Explosive:

                if (targetTags.Contains("Enemy"))
                    globalData.CreateGPAddPoint(transform.position, GetComponent<RewardComponent>().GamePointReward);

                globalData.CreateExposion(transform.position, itemData.damage);

                penetratingPower--;

                if (penetratingPower <= 0)
                    Destroy(gameObject);

                break;

            case ProjectileDamageTyp.Fire:
                if (targetTags.Contains(collision.transform.tag) && collision.transform.GetComponent<IDamageable>() != null)
                {
                    if (targetTags.Contains("Enemy"))
                        globalData.CreateGPAddPoint(transform.position, GetComponent<RewardComponent>().GamePointReward);

                    if (itemData.weaponData.weaponTyp.Equals(ItemData.WeaponData.WeaponTyp.Particel))
                        collision.transform.GetComponent<IDamageable>().TakeDamage(itemData.damage, false);
                    else
                        collision.transform.GetComponent<IDamageable>().TakeDamage(itemData.damage, true);

                    collision.transform.GetComponent<IDamageable>().MakeStunned(4f);
                    collision.transform.GetComponent<IDamageable>().SetBurn(true);
                }

                if (collision.transform.GetComponent<EffectAssinger>() != null)
                    hitEffect.GetComponent<HitEffect>().hitEffectData = collision.transform.GetComponent<EffectAssinger>().hitEffectData;
                else
                    hitEffect.GetComponent<HitEffect>().hitEffectData = new HitEffect.HitEffectData(true);
                
                penetratingPower--;

                if (penetratingPower <= 0)
                    Destroy(gameObject);

                break;
        }
    }

    //For Fire
    private void OnParticleCollision(GameObject other)
    {
        if (damageTyp == ProjectileDamageTyp.Fire)
        {
            var colEvents = new List<ParticleCollisionEvent>();

            GetComponent<ParticleSystem>().GetCollisionEvents(other, colEvents);

            if (targetTags.Contains(other.tag) && other.GetComponent<IDamageable>() != null)
            {
                int createHitPoint = Random.Range(0, 9);

                if (createHitPoint == 1 && colEvents.Count > 0 && targetTags.Contains("Enemy"))
                    globalData.CreateGPAddPoint(colEvents[Random.Range(0, colEvents.Count)].intersection, GetComponent<RewardComponent>().GamePointReward); 

                if (itemData.weaponData.weaponTyp.Equals(ItemData.WeaponData.WeaponTyp.Particel))
                {
                    other.GetComponent<IDamageable>().TakeDamage(itemData.damage * 0.06f, false);
                }
                else
                {
                    other.GetComponent<IDamageable>().TakeDamage(itemData.damage * 0.06f, true);
                }
                other.GetComponent<IDamageable>().MakeStunned(4f);
                other.GetComponent<IDamageable>().SetBurn(true);
            }
        }
    }

    public void SetWeaponData(ItemData itemData, List<string> targetTags)
    {
        this.itemData = itemData;
        this.targetTags = targetTags;
        this.penetratingPower = itemData.weaponData.penetratingPower;

        damageTyp = itemData.weaponData.damageTyp;
        globalData = GameInstance.instance.referenceValues.globalData;

        if (burning != false && damageTyp == ProjectileDamageTyp.Fire && !burning.isPlaying)
        {
            burning.Play();
        }

        projectileTyp = itemData.weaponData.projectileTyp;

        if (itemData.weaponData.weaponTyp == ItemData.WeaponData.WeaponTyp.Projectile)
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            switch (projectileTyp)
            {
                case ProjectileTyp.Bullet:
                    transform.localScale = new Vector3(0.5f, 0.5f, 1);
                    renderer.sprite = itemData.globalData.bulletSprite;
                    break;

                case ProjectileTyp.Rocket:
                    transform.localScale = new Vector3(2f, 2f, 1);
                    renderer.sprite = itemData.globalData.rocketSprite;
                    break;

                case ProjectileTyp.Arrow:
                    transform.localScale = new Vector3(1.75f, 1.75f, 1);
                    renderer.sprite = itemData.globalData.arrowSprite;
                    break;
            }
        }
    }
}
