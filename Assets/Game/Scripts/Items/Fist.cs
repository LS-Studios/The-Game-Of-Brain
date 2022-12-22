using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Fist : MonoBehaviour
{
    public CurrentItemHandler currentItemHandler;

    [Inject]
    private GameManager gameManager;

    private void Start()
    {
        currentItemHandler.hitAction = () =>
        {
            if (currentItemHandler != null && currentItemHandler.useableItems.Count <= 0)
                StartCoroutine(Punch());
        };
    }

    private IEnumerator Punch()
    {
        yield return new WaitForSeconds(0.35f);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.6f, 0.5f);

        foreach (Collider2D nearObject in colliders)
        {
            if (nearObject.GetComponent<IDamageable>() != null && currentItemHandler.targetTags.Contains(nearObject.gameObject.tag))
            {
                GameObject hitEffect = Instantiate(GameInstance.instance.referenceValues.globalData.hitEffect, 
                                                   transform.position + transform.up, Quaternion.identity);
                hitEffect.GetComponent<HitEffect>().hitEffectData = nearObject.transform.GetComponent<EffectAssinger>().hitEffectData;

                GameInstance.instance.referenceValues.globalData.
                    CreateGPAddPoint(nearObject.bounds.ClosestPoint(transform.position),
                    gameManager.GetComponent<RewardComponent>().GamePointReward);

                nearObject.GetComponent<IDamageable>().TakeDamage(15*currentItemHandler.damageMultiply, true);
            }
        }
    }
}
