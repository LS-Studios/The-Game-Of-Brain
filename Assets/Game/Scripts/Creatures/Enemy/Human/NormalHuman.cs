using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalHuman : HumanEnemy
{
    protected override void Start()
    {
        switch (GameInstance.instance.inGameValues.difficulty)
        {
            case GameInstance.InGameValues.Difficulty.Easy:
                damage = 3;
                healthComponent.startHealth = 100;
                break;

            case GameInstance.InGameValues.Difficulty.Medium:
                damage = 3.2f;
                healthComponent.startHealth = 120;
                break;

            case GameInstance.InGameValues.Difficulty.Hard:
                damage = 4f;
                healthComponent.startHealth = 140;
                break;

            case GameInstance.InGameValues.Difficulty.Insane:
                damage = 6;
                healthComponent.startHealth = 160;
                break;
        }

        base.Start();
    }
}
