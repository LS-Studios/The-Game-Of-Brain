using Zenject;
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

//Is used because the player can hold more than one grenade
//Its the throw prefab, so that the actual grenade dot need to get removed
public class GrenadeProjectile : MonoBehaviour
{
    [HideInInspector]
    public ItemData itemData;

    public Transform spawn;
    public List<String> targetTags = new List<String>();
    private GlobalData globalData;

    private Rigidbody2D rb;
    private float rotation = 0;

    void Start()
    {
        gameObject.AddComponent<ZenAutoInjecter>().ContainerSource = ZenAutoInjecter.ContainerSources.SceneContext;

        globalData = GameInstance.instance.referenceValues.globalData;

        GetComponent<SpriteRenderer>().sprite = itemData.grenadeData.sprite;

        rb = GetComponent<Rigidbody2D>();

        Destroy(gameObject, 0.4f);
    }

    private void Update()
    {
        rotation += 500 * Time.deltaTime;
        rb.MoveRotation(rotation);
    }

    private void OnDestroy()
    {
        ParticleSystem explosion = Instantiate(globalData.explosion, spawn.position, spawn.rotation);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 3);

        foreach (Collider2D nearObject in colliders)
        {
            if (nearObject.GetComponent<IDamageable>() != null && targetTags.Contains(nearObject.gameObject.tag))
            {
                nearObject.GetComponent<IDamageable>().TakeDamage(itemData.damage, true);
                nearObject.GetComponent<IDamageable>().MakeStunned(itemData.grenadeData.effectValue);
            }
        }

        switch (itemData.grenadeData.grenadeTyp)
        {
            case ItemData.GrenadeData.GrenadeTyp.Normal:
                AudioManager.instance.PlaySoundOnThempTransform("Explosion", transform.position);
                break;
            case ItemData.GrenadeData.GrenadeTyp.Flashbang:
                AudioManager.instance.PlaySoundOnThempTransform("Flash", transform.position);
                break;
        }

        Camera.main.transform.parent.GetComponent<CameraHandler>().Shake();
    }
}
