using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMediator
{
    void Notify(Component sender, string eventCode, object data = null);
}

public static class GameEvent
{
    public const string StaminaKey = "ConsumeStamina";
    public const string SpeedBuffKey = "ApplySpeedBuff";
    public const string JumpBuffKey = "ApplyJumpBuff";
    public const string EquipKey = "EquipItem";
    public const string UnEquipKey = "UnEquip";
}

public class GameMediator : MonoBehaviour, IMediator
{
    public static GameMediator Instance { get; private set; }

    public PlayerController playerController;
    public PlayerCondition playerCondition;
    public Equipment playerEquipment;

    public float duration;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerController = FindObjectOfType<PlayerController>();
        playerCondition = FindObjectOfType<PlayerCondition>();
        playerEquipment = FindObjectOfType<Equipment>();
    }

    public void Notify(Component sender, string eventCode, object data = null)
    {
        switch(eventCode)
        {
            case GameEvent.StaminaKey:
                if(data is float cost && playerCondition.CanUseStamina(cost))
                {
                    playerCondition.ConsumeStamina(cost);
                }
                break;

            case GameEvent.SpeedBuffKey:
                if(data is float speedBuff)
                {
                    playerController.ApplySpeedUp(speedBuff, duration);
                }
                break;

            case GameEvent.JumpBuffKey:
                if(data is float jumpBuff)
                {
                    playerController.ApplyJumpPowerUp(jumpBuff, duration);
                }
                break;

            case GameEvent.EquipKey:
                if(data is ItemData itemData)
                {
                    playerEquipment.EquipNew(itemData);
                }
                break;

            case GameEvent.UnEquipKey:
                playerEquipment.UnEquip();
                break;

            default:
                Debug.LogWarning($"[GameMediator] : Unknown Event {eventCode}");
                break;
        }
    }
}
