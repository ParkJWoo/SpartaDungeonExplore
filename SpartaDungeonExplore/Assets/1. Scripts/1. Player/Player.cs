using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController playerController;
    public PlayerCondition playerCondition;
    public Equipment playerEquip;

    public ItemData itemData;
    public Action addItem;

    public Transform dropPosition;

    private void Awake()
    {
        CharacterManager.Instance.Player = this;

        playerController = GetComponent<PlayerController>();
        playerCondition = GetComponent<PlayerCondition>();
        playerEquip = GetComponent<Equipment>();

        dropPosition = GetComponent<Transform>();
    }
}