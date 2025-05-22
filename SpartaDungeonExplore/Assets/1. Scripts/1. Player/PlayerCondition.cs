using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition : MonoBehaviour
{
    public UICondition uiCondition;

    Condition health { get { return uiCondition.health; } }
    Condition stamina
    {
        get { return uiCondition.stamina; }
        set { uiCondition.stamina = value; }
    }

    public event Action onTakeDamage;

    private void Update()
    {
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

    #region 스태미너 회복 메서드
    public void StaminaHeal(float amount)
    {
        stamina.Add(amount);
    }
    #endregion

    #region 아이템 효과 적용을 위한 메서드 추가
    public void SpeedUp(float amount, float duration)
    {
        GameMediator.Instance.Notify(this, GameEvent.SpeedBuffKey, duration);
    }

    public void JumpPowerUp(float amount, float duration)
    {
        GameMediator.Instance.Notify(this, GameEvent.JumpBuffKey, duration);
    }

    #endregion

    #region 스태미너 사용 관련 메서드 묶음
    public void ConsumeStamina(float amount)
    {
        stamina.Subtract(amount);
    }

    public bool CanUseStamina(float amount)
    {
        return stamina.curValue >= amount;
    }
    #endregion

    public void Die()
    {
        Debug.Log("[PlayerCondition] 플레이어 사망");
    }
}
