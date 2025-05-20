using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    None,
    Equipable,
    Consumable
}

public enum ConsumableType
{
    None, 
    Health,
    Stanima, 
    SpeedUp,
    JumpPowerUp
}

[System.Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;

    //  ���� �ð� ������ ȿ���� �����ϴ� �������� ���� ���� �ð� ���� �߰�.
    public float duration;
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public ItemType type;
    public Sprite icon;
    public GameObject dropPrefab;

    [Header("Stacking")]
    public bool canStack;
    public int maxStackAmount;

    [Header("Consumable")]
    public ItemDataConsumable[] consumables;

    [Header("Equip")]
    public GameObject equipPrefab;
}
