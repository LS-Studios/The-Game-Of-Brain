using Zenject;
using UnityEngine;
using UnityEditor.Build.Content;

public class Grenade : WeaponBase
{
    protected override void Awake()
    {
        base.Awake();

        SetWeaponUp(itemData, transform);
    }

    protected override void Start()
    {
        base.Start();

        if (GameInstance.instance.equipmentValues.CurrentSetContaisPerk(ItemData.PerkData.PerkTyp.ExtraGrenade))
        {
            itemData.currentAmmount = 6;
            itemData.maxAmmount = 6;
        }
    }

    protected override void Update()
    {
        base.Update();

        GetComponent<SpriteRenderer>().sprite = itemData.grenadeData.sprite;
    }

    public override void Attack()
    {
        base.Attack();

        if (itemData.currentAmmount > 0)
        {
            AudioManager.instance.PlaySoundOnThempTransform(itemData.grenadeData.throwSound, transform.position);

            animator.SetTrigger("Attack");

            GameObject grenade = Instantiate(itemData.globalData.grenadePrefab, transform.position, transform.rotation);
            grenade.GetComponent<Rigidbody2D>().AddForce(transform.up * 8, ForceMode2D.Impulse);
            grenade.GetComponent<GrenadeProjectile>().itemData = itemData;
            grenade.GetComponent<GrenadeProjectile>().targetTags = targetTags;
            itemData.currentAmmount -= 1;
        }
    }
}
