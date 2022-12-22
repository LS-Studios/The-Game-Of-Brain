using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class RewardComponent : MonoBehaviour
{
    [Header("Game Reward")]
    [SerializeField]
    private int brainCellReward = 4;
    [SerializeField]
    private int cashReward = 2;
    [SerializeField]
    private int gamePointReward = 5;

    public int BrainCellReward
    {
        set
        {
            if (GameInstance.instance.equipmentValues.CurrentSetContaisPerk(ItemData.PerkData.PerkTyp.ExtraReward))
            {
                brainCellReward = Mathf.RoundToInt(value * 1.5f);
            }
            else
            {
                brainCellReward = value;
            }
        }

        get
        {
            return brainCellReward;
        }
    }
    public int CashReward
    {
        set
        {
            if (GameInstance.instance.equipmentValues.CurrentSetContaisPerk(ItemData.PerkData.PerkTyp.ExtraReward))
            {
                cashReward = Mathf.RoundToInt(value * 1.5f);
            }
            else
            {
                cashReward = value;
            }
        }

        get
        {
            return cashReward;
        }
    }
    public int GamePointReward {
        set
        {
            if (GameInstance.instance.equipmentValues.CurrentSetContaisPerk(ItemData.PerkData.PerkTyp.ExtraGP))
            {
                gamePointReward = Mathf.RoundToInt(value * 1.5f);
            } else
            {
                gamePointReward = value;
            }
        }

        get
        {
            return gamePointReward;
        }
    }

    private void Start()
    {
        if (GameInstance.instance.equipmentValues.CurrentSetContaisPerk(ItemData.PerkData.PerkTyp.ExtraGP))
        {
            gamePointReward = Mathf.RoundToInt(gamePointReward * 1.5f);
        }
    }

    public void GiveRewardToPlayer()
    {
        PlayerHandler playerHandler = FindObjectOfType<PlayerHandler>();

        if (playerHandler != null)
        {
            if (GameInstance.instance.equipmentValues.CurrentSetContaisPerk(ItemData.PerkData.PerkTyp.ExtraReward))
            {
                playerHandler.earnedBrainCells += Mathf.RoundToInt(brainCellReward * 1.5f);
                playerHandler.earnedCash += Mathf.RoundToInt(cashReward * 1.5f);
            } else
            {
                playerHandler.earnedBrainCells += brainCellReward;
                playerHandler.earnedCash += cashReward;
            }

            if (gamePointReward > 0)
                GameInstance.instance.referenceValues.globalData.CreateGPAddPoint(transform.position, gamePointReward);
        }
    }
}
