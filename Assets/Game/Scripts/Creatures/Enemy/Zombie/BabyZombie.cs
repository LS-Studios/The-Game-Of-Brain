using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyZombie: ZombieEnemy
{
    protected override void Start()
    {
        switch (GameInstance.instance.inGameValues.difficulty)
        {
            case GameInstance.InGameValues.Difficulty.Easy:
                damage = 1.5f;
                attackRate = 0.275f;
                healthComponent.startHealth += 25;
                healthComponent.normalSpeed = healthComponent.currentSpeed = 2;
                break;

            case GameInstance.InGameValues.Difficulty.Medium:
                damage = 1.6f;
                attackRate = 0.25f;
                healthComponent.startHealth += 30;
                healthComponent.normalSpeed = healthComponent.currentSpeed = 2.25f;
                break;

            case GameInstance.InGameValues.Difficulty.Hard:
                damage = 1.8f;
                attackRate = 0.15f;
                healthComponent.startHealth += 60;
                healthComponent.normalSpeed = healthComponent.currentSpeed = 2.75f;
                break;

            case GameInstance.InGameValues.Difficulty.Insane:
                damage = 2.25f;
                attackRate = 0.125f;
                healthComponent.startHealth += 80;
                healthComponent.normalSpeed = healthComponent.currentSpeed = 3.5f;
                break;
        }

        base.Start();
    }
}
