using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMediator
{
    void Notify(Component sender, string eventCode, object data = null);
}

#region GameEvent 클래스
public static class GameEvent
{
    public const string StaminaKey = "ConsumeStamina";
    public const string SpeedBuffKey = "ApplySpeedBuff";
    public const string JumpBuffKey = "ApplyJumpBuff";
}
#endregion

public class GameMediator : MonoBehaviour, IMediator
{
    public static GameMediator Instance { get; private set; }

    public PlayerController playerController;
    public PlayerCondition playerCondition;
    public Equipment playerEquipment;

    public float duration;

    #region Awake 메서드
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
    #endregion

    #region Notify 메서드
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
        
            default:
                Debug.LogWarning($"[GameMediator] : Unknown Event {eventCode}");
                break;
        }
    }
    #endregion
}
