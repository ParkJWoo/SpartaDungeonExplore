using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition : MonoBehaviour
{
    public UICondition uiCondition;

    Condition health { get { return uiCondition.health; } }
    Condition stamina { get { return uiCondition.stamina; } }
    float moveSpeed { get { return CharacterManager.Instance.Player.playerController.moveSpeed; } }


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

    public void StaminaHeal(float amount)
    {
        stamina.Add(amount);
    }

    public void SpeedUp(float amount, float duration)
    {
        CharacterManager.Instance.Player.playerController.ApplySpeedUp(amount, duration);
    }

    public void JumpPowerUp(float amount, float duration)
    {
        CharacterManager.Instance.Player.playerController.ApplyJumpPowerUp(amount, duration);
    }

    public void ConsumeStamina(float amount)
    {
        stamina.Subtract(amount);
    }

    public bool CanUseStamina(float amount)
    {
        return stamina.curValue >= amount;
    }


    public void Die()
    {
        Debug.Log("[PlayerCondition] 플레이어 사망");
    }
}
