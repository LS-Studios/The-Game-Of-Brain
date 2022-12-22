using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Zenject;

public abstract class DestructibleObject : MonoBehaviour
{
    [Header("Destroy State Info [Should start with MaxLife and end with 0]")]
    public float[] destructiveStateSteps;
    private List<Action> destructiveStates = new List<Action>();

    protected HealtComponent healthComponent;

    protected GlobalData globalData;

    [Inject]
    protected GameManager gameManager;

    protected virtual void Start()
    {
        if (GameInstance.instance == null && GetComponent<ZenAutoInjecter>() == null)
            gameObject.AddComponent<ZenAutoInjecter>().ContainerSource = ZenAutoInjecter.ContainerSources.SceneContext;

        healthComponent = GetComponent<HealtComponent>();

        healthComponent.onDie = Die;

        globalData = GameInstance.instance.referenceValues.globalData;

        for (int i = 0; i < destructiveStateSteps.Length; i++)
        {
            destructiveStates.Add((Action)GetType().GetMethod("DestroyState" + i).CreateDelegate(typeof(Action), this));
        }
    }

    protected virtual void Update()
    {
        for (int i = 0; i < destructiveStateSteps.Length; i++)
        {
            if ((i + 1) < destructiveStateSteps.Length)
            {
                if (healthComponent.currentHealth <= destructiveStateSteps[i] && healthComponent.currentHealth > destructiveStateSteps[i + 1])
                {
                    destructiveStates[i].Invoke();
                }
            }
        }
    }

    public void Die()
    {
        destructiveStates[destructiveStateSteps.Length - 1].Invoke();

        StartCoroutine(Destroy());
    }

    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(0.25f);

        RewardComponent rewardComponent = GetComponent<RewardComponent>();
        if (rewardComponent != null)
            rewardComponent.GiveRewardToPlayer();

        OnDestroyObject();

        Destroy(gameObject);

        gameManager.Scan();
    }

    public virtual void OnDestroyObject()
    {

    }

    public virtual void DestroyState0()
    {

    }
    public virtual void DestroyState1()
    {

    }
    public virtual void DestroyState2()
    {

    }
    public virtual void DestroyState3()
    {

    }
    public virtual void DestroyState4()
    {

    }
}
