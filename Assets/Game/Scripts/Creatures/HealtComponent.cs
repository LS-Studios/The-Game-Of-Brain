using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class HealtComponent : MonoBehaviour, IDamageable
{
    public float startHealth = 100;

    [HideInInspector]
    public float startHealthAddValue = 0;

    [HideInInspector]
    public float maxHealth;

    [HideInInspector]
    public float currentHealth;

    public bool isDead = false;

    public float normalSpeed = 2;
    public float runSpeed = 6f;
    [HideInInspector] public float currentSpeed;

    bool isAlreadyStunned = false;

    public ParticleSystem burnPS;

    public bool isBurn = false;

    public string hitSound = "";

    public CurrentItemHandler currentItemHandler;

    public UnityAction onDie;

    void Start()
    {
        startHealth += startHealthAddValue;

        maxHealth = startHealth;

        currentHealth = maxHealth;

        currentSpeed = normalSpeed;

        if (burnPS != null && !burnPS.isStopped)
            burnPS.Stop();
    }

    void Update()
    {
        if (isBurn)
        {
            if (burnPS != null && !burnPS.isPlaying)
                burnPS.Play();

            TakeDamage(12f * Time.deltaTime, false);

            if (AudioManager.instance.GetAudioSourceFromTransform("Burn", transform) == null)
                AudioManager.instance.Play("Burn", transform);
        }
        else
        {
            if (burnPS != null && !burnPS.isStopped)
                burnPS.Stop();

            if (AudioManager.instance.GetAudioSourceFromTransform("Burn", transform) != null)
                AudioManager.instance.GetAudioSourceFromTransform("Burn", transform).Stop();

        }

        if (currentHealth <= 0)
        {
            isDead = true;
            if (onDie != null)
                onDie.Invoke();
        }
    }

    public void AddHealth(float healthToAdd)
    {
        if (currentHealth + healthToAdd > maxHealth)
            currentHealth = 100;
        else
            currentHealth += healthToAdd;
    }

    public void AddFullHealth()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage, bool playSound)
    {
        if (playSound)
            AudioManager.instance.Play(hitSound, transform);

        if (GameInstance.instance.equipmentValues.CurrentSetContaisPerk(ItemData.PerkData.PerkTyp.ExtraDamage) &&
            GetComponent<PlayerHandler>() == null)
        {
            currentHealth -= damage * 1.5f;
        } 
        else
        {
            currentHealth -= damage;
        }
        
    }

    public void TakeDamageToDie(bool playSound)
    {
        if (playSound)
            AudioManager.instance.Play(hitSound, transform);

        currentHealth = 0;
    }

    public void MakeStunned(float ammount)
    {
        if (!isAlreadyStunned)
        {
            currentSpeed /= ammount;

            if (currentItemHandler != null)
                currentItemHandler.canDoAction = false;

            isAlreadyStunned = true;
        }

        StartCoroutine(StunnDelay(ammount));
    }

    IEnumerator StunnDelay(float time)
    {
        yield return new WaitForSeconds(time/2);

        currentSpeed = normalSpeed;
        if (currentItemHandler != null)
            currentItemHandler.canDoAction = true;

        isAlreadyStunned = false;
    }

    public void SetBurn(bool shouldBurn)
    {
        isBurn = shouldBurn;

        if (BurnDelay() != null)
            StopCoroutine(BurnDelay());

        StartCoroutine(BurnDelay());
    }

    public void PlayHitSound()
    {
        AudioManager.instance.Play(hitSound, transform);
    }

    private IEnumerator BurnDelay()
    {
        yield return new WaitForSeconds(6f);

        isBurn = false;
        if (burnPS != null && !burnPS.isStopped)
            burnPS.Stop();
    }
}
