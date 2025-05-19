using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition : MonoBehaviour
{
    public UICondition uiCondition;

    Condition health { get { return uiCondition.health; } }
    Condition stamina { get { return uiCondition.stamina; } }

    public event Action onTakeDamage;

    private void Update()
    {
        health.Subtract(health.passiveValue * Time.deltaTime);

        stamina.Add(stamina.passiveValue * Time.deltaTime);

        if(health.curValue < 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Die()
    {
        Debug.Log("[PlayerCondition] 플레이어 사망");
    }
}
