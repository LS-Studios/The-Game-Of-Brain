using System;
using UnityEngine;
using static DestructibleRewardObject;

public class DestructibleRewardObject : DestructibleObject
{
    [Header("Destroy Info")]
    public SpriteRenderer spriteRenderer;
    public Sprite[] destroySprites;

    [Header("Reward")]
    public Reward[] rewardsToDrop;
    public bool chooseRandomFromList;

    [Serializable]
    public struct Reward {
        public GameObject rewardObject;

        public int ammount;
    }

    public override void OnDestroyObject()
    {
        if (chooseRandomFromList)
        {
            Reward reward = rewardsToDrop[UnityEngine.Random.Range(0, rewardsToDrop.Length-1)];

            for (int i = 0; i < reward.ammount; i++)
            {
                Vector2 currentPosition = transform.position;
                Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * 1f;
                Vector2 spawnPosition = currentPosition + randomOffset;
                GameObject droppedObject = Instantiate(reward.rewardObject, spawnPosition, Quaternion.identity);
            }

            return;
        }

        foreach (Reward reward in rewardsToDrop)
        {
            for (int i = 0; i < reward.ammount; i++)
            {
                Vector2 currentPosition = transform.position;
                Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * 1f;
                Vector2 spawnPosition = currentPosition + randomOffset;
                GameObject droppedObject = Instantiate(reward.rewardObject, spawnPosition, Quaternion.identity);
            }
        }
    }
    public override void DestroyState0()
    {
        spriteRenderer.sprite = destroySprites[0];
    }
    public override void DestroyState1()
    {
        spriteRenderer.sprite = destroySprites[1];
    }
    public override void DestroyState2()
    {
        spriteRenderer.sprite = destroySprites[2];
    }
    public override void DestroyState3()
    {
        spriteRenderer.sprite = destroySprites[3];
    }
    public override void DestroyState4()
    {
        spriteRenderer.sprite = destroySprites[4];
    }
}
