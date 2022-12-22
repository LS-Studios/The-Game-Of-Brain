using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(float damage, bool playSound);

    public void SetBurn(bool shouldBurn);

    public void PlayHitSound();

    public void MakeStunned(float ammount);
}