using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.Events;
using System;

public class ChallengeComponent : MonoBehaviour
{
    public Challenge.ChalangeTyp chalangeTyp;

    [ConditionalHide("chalangeTyp", Challenge.ChalangeTyp.Kill)]
    public Enemy.EnemyTyp enemyTyp;

    [ConditionalHide("chalangeTyp", Challenge.ChalangeTyp.Collect)]
    public Collectable.CollectableTyp collectableTyp;

    public void AddChalangeProgress()
    {
        foreach (Challenge chalange in GetChalangesOfTyp(chalangeTyp))
        {
            bool addReward = false;

            switch (chalangeTyp)
            {
                case Challenge.ChalangeTyp.Kill:
                    if (chalange.enemyTyp == enemyTyp)
                        addReward = true;
                    break;

                case Challenge.ChalangeTyp.Collect:
                    if (chalange.collectableTyp == collectableTyp)
                        addReward = true;
                    break;

                case Challenge.ChalangeTyp.Survive:
                    addReward = true;
                    break;
            }

            if (addReward)
            {
                if (chalange.currentProgress + chalange.currentIngameProgress + 1 < chalange.progressToReach)
                    chalange.currentIngameProgress += 1;
                else if (chalange.currentProgress + chalange.currentIngameProgress + 1 >= chalange.progressToReach)
                {
                    chalange.isCompleted = true;
                    chalange.currentIngameProgress = chalange.progressToReach - chalange.currentProgress;
                }
            }
        }
    }

    public void AddChalangeProgress(int progress)
    {
        foreach (Challenge chalange in GetChalangesOfTyp(chalangeTyp))
        {
            bool addReward = false;

            switch (chalangeTyp)
            {
                case Challenge.ChalangeTyp.Kill:
                    if (chalange.enemyTyp == enemyTyp)
                        addReward = true;
                    break;

                case Challenge.ChalangeTyp.Collect:
                    if (chalange.collectableTyp == collectableTyp)
                        addReward = true;
                    break;
            }

            if (addReward)
            {
                if (chalange.currentProgress + chalange.currentIngameProgress + progress < chalange.progressToReach)
                    chalange.currentIngameProgress += progress;
                else if (chalange.currentProgress + chalange.currentIngameProgress + progress >= chalange.progressToReach)
                {
                    chalange.isCompleted = true;
                    chalange.currentIngameProgress = chalange.progressToReach - chalange.currentProgress;
                }
            }
        }
    }

    private List<Challenge> GetChalangesOfTyp(Challenge.ChalangeTyp chalangeTyp)
    {
        return GameInstance.instance.chalangeValues.chalanges.FindAll(chalange => chalange.chalangeTyp == chalangeTyp);
    }

    private void OnDestroy()
    {
        AddChalangeProgress();
    }
}