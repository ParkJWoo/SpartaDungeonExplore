using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICondition : MonoBehaviour
{
    public Condition health;
    public Condition stamina;

    private void Start()
    {
        CharacterManager.Instance.Player.playerCondition.uiCondition = this;
    }
}
