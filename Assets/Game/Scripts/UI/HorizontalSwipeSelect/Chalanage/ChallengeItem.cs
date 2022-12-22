using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class ChallengeItem : MonoBehaviour
{
    public bool inGameChalange;
    public bool chalangeCount;

    public int chalangeIndex;

    public Challenge chalange;

    public TextMeshProUGUI chalangeDescription;
    public TextMeshProUGUI cashReward;
    public TextMeshProUGUI brainCellReward;
    public Slider progressSlider;
    public TextMeshProUGUI progressText;

    private void Update()
    {
        chalange = GameInstance.instance.chalangeValues.chalanges[chalangeIndex];

        chalangeDescription.text = chalange.description;

        cashReward.text = chalange.cashReward.ToString();
        brainCellReward.text = chalange.brainCellReward.ToString();

        progressSlider.maxValue = chalange.progressToReach;

        if (!chalangeCount)
        {
            progressSlider.value = chalange.currentProgress + chalange.currentIngameProgress;
            progressText.text = chalange.currentProgress + chalange.currentIngameProgress + "/" + chalange.progressToReach;

            if (chalange.currentProgress + chalange.currentIngameProgress >= chalange.progressToReach)
            {
                chalange.isCompleted = true;

                if (!inGameChalange)
                    GameInstance.instance.chalangeValues.CompleteChalange(chalange);
            }
        } else
        {
            progressSlider.value = chalange.currentProgress;
            progressText.text = chalange.currentProgress + "/" + chalange.progressToReach;

            if (chalange.currentProgress >= chalange.progressToReach)
            {
                chalange.isCompleted = true;

                if (!inGameChalange)
                    GameInstance.instance.chalangeValues.CompleteChalange(chalange);
            }
        }
    }
}

[System.Serializable]
public class Challenge
{
    public string description;

    public bool isCompleted;

    public enum ChalangeTyp { Kill, Collect, Survive }
    public ChalangeTyp chalangeTyp;

    [ConditionalHide("chalangeTyp", ChalangeTyp.Kill)]
    public Enemy.EnemyTyp enemyTyp;

    [ConditionalHide("chalangeTyp", ChalangeTyp.Collect)]
    public Collectable.CollectableTyp collectableTyp;

    public int currentIngameProgress;
    public int currentProgress;

    public int progressToReach;

    public int cashReward;
    public int brainCellReward;
}
