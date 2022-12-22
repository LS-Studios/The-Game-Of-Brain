using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBosZombie : ZombieEnemy
{
    public ParticleSystem spitPS;

    private bool canSpit = true;

    protected override void Start()
    {
        switch (GameInstance.instance.inGameValues.difficulty)
        {
            case GameInstance.InGameValues.Difficulty.Easy:
                damage = 8;
                attackRate = 0.25f;
                healthComponent.startHealth = 300;
                healthComponent.normalSpeed = healthComponent.currentSpeed = 2;
                break;

            case GameInstance.InGameValues.Difficulty.Medium:
                damage = 10;
                attackRate = 0.3f;
                healthComponent.startHealth = 375;
                healthComponent.normalSpeed = healthComponent.currentSpeed = 2.2f;
                break;

            case GameInstance.InGameValues.Difficulty.Hard:
                damage = 12.5f;
                attackRate = 0.35f;
                healthComponent.startHealth = 425;
                healthComponent.normalSpeed = healthComponent.currentSpeed = 2.4f;
                break;

            case GameInstance.InGameValues.Difficulty.Insane:
                damage = 15;
                attackRate = 0.3f;
                healthComponent.startHealth = 500;
                healthComponent.normalSpeed = healthComponent.currentSpeed = 3;
                break;
        }

        IEnumerator SpitDelay()
        {
            while(true)
            {
                if (target != null && Vector2.Distance(transform.position, target.position) < 4)
                {
                    if (!spitPS.isPlaying) spitPS.Play();

                    yield return new WaitForSeconds(3f);
                }

                yield return null;
            }
        }

        StartCoroutine(SpitDelay());

        base.Start();
    }

    public override void Die()
    {
        Instantiate(GameInstance.instance.referenceValues.globalData.normalZombie, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
