using Zenject;
using UnityEngine;
using System.Collections.Generic;

public class MeeleWeapon : WeaponBase
{
    public bool addForceOnHit = false;

    [ConditionalHide("addForceOnHit")]
    public float foceToAdd = 1f;

    public bool alwaysBlock = false;

    [ConditionalHide("alwaysBlock")]
    public bool takeDameOnBlock = true;
    
    private bool canDoDamage = false;

    private List<GameObject> hitObjects = new List<GameObject>();
    protected override void Awake()
    {
        base.Awake();

        base.SetWeaponUp(itemData, transform);
    }

    public override void Attack()
    {
        base.Attack();

        AudioManager.instance.Play(itemData.weaponData.attackSounds[Random.Range(0, itemData.weaponData.attackSounds.Length)], transform);

        animator.SetTrigger("Attack");
    }

    protected override void Update()
    {
        base.Update();

        bool check = false;

        if (animator != null)
        {
            switch (itemData.generalData.meeleWeaponType)
            {
                case ItemData.GeneralData.MeeleWeaponType.Katana:
                    check = animator.GetCurrentAnimatorStateInfo(0).IsName("Swing");
                    break;

                case ItemData.GeneralData.MeeleWeaponType.Knife:
                    check = animator.GetCurrentAnimatorStateInfo(0).IsName("Knife");
                    break;

                case ItemData.GeneralData.MeeleWeaponType.Shield:
                    check = animator.GetCurrentAnimatorStateInfo(0).IsName("Push");
                    break;
            }
        }

        canDoDamage = check;

        if (canDoDamage)
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
            gameObject.layer = 0;
            GetComponent<SpriteRenderer>().sortingOrder = 5;
        }
        else
        {
            GetComponent<BoxCollider2D>().isTrigger = false;
            hitObjects.Clear();
            gameObject.layer = 10;
            GetComponent<SpriteRenderer>().sortingOrder = 6;
        }

        if (alwaysBlock && !takeDameOnBlock)
        {
            gameObject.layer = 0;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (canDoDamage && !hitObjects.Contains(collision.gameObject) &&
            targetTags.Contains(collision.gameObject.tag) && collision.gameObject.GetComponent<IDamageable>() != null
            && itemData.damage > 0)
        {
            GameObject hitEffect = Instantiate(itemData.globalData.hitEffect, collision.bounds.ClosestPoint(transform.position), itemData.globalData.hitEffect.transform.rotation);
            hitEffect.GetComponent<HitEffect>().hitEffectData = collision.transform.GetComponent<EffectAssinger>().hitEffectData;

            GameInstance.instance.referenceValues.globalData.CreateGPAddPoint(collision.bounds.ClosestPoint(transform.position), gameManager.GetComponent<RewardComponent>().GamePointReward);

            collision.gameObject.GetComponent<IDamageable>().TakeDamage(itemData.damage, true);

            hitObjects.Add(collision.gameObject);
        }

        Vector2 dir = transform.position - collision.transform.position;
        dir.Normalize();

        Rigidbody2D hitRB = collision.gameObject.GetComponent<Rigidbody2D>();

        if (hitRB != null)
            hitRB.AddForce(-dir * foceToAdd * hitRB.mass);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canDoDamage && !hitObjects.Contains(collision.gameObject) &&
            targetTags.Contains(collision.gameObject.tag) && collision.gameObject.GetComponent<IDamageable>() != null
            && itemData.damage > 0)
        {
            GameObject hitEffect = Instantiate(itemData.globalData.hitEffect, collision.bounds.ClosestPoint(transform.position), itemData.globalData.hitEffect.transform.rotation);
            hitEffect.GetComponent<HitEffect>().hitEffectData = collision.transform.GetComponent<EffectAssinger>().hitEffectData;

            GameInstance.instance.referenceValues.globalData.CreateGPAddPoint(collision.bounds.ClosestPoint(transform.position), gameManager.GetComponent<RewardComponent>().GamePointReward);

            collision.gameObject.GetComponent<IDamageable>().TakeDamage(itemData.damage, true);

            hitObjects.Add(collision.gameObject);
        }

        Vector2 dir = transform.position - collision.transform.position;
        dir.Normalize();

        Rigidbody2D hitRB = collision.gameObject.GetComponent<Rigidbody2D>();

        if (hitRB != null)
            hitRB.AddForce(-dir * foceToAdd * hitRB.mass);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (hitObjects.Contains(collision.gameObject))
            hitObjects.Remove(collision.gameObject);
    }
}
