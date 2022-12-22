using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieEnemy : Enemy
{
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.tag != "Enemy" && collision.collider.tag != "NoDamage")
        {
            IDamageable iDamageable = collision.transform.GetComponent<IDamageable>();
            if (iDamageable != null)
            {
                if (spawnedEffects <= 0)
                {
                    GameObject hitEffect = Instantiate(GameInstance.instance.referenceValues.globalData.hitEffect, collision.transform.position, collision.transform.rotation);

                    spawnedEffects++;

                    if (collision.transform.GetComponent<EffectAssinger>() != null)
                        hitEffect.GetComponent<HitEffect>().hitEffectData = collision.transform.GetComponent<EffectAssinger>().hitEffectData;
                    else
                        hitEffect.GetComponent<HitEffect>().hitEffectData = new HitEffect.HitEffectData(true);

                    hitEffect.GetComponent<HitEffect>().OnDestroyAction += () => spawnedEffects--;
                }

                IEnumerator DoDamage()
                {
                    doDamageDelay = true;
                    yield return new WaitForSeconds(attackRate);
                    animator.SetTrigger("Hit");
                    iDamageable.TakeDamage(damage, true);
                    doDamageDelay = false;
                }

                if (!doDamageDelay)
                    StartCoroutine(DoDamage());
            }
        }
    }
}
