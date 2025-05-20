using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public Equip curEquip;
    public Transform equipParent;

    private PlayerController playerController;
    private PlayerCondition playerCondition;

    private void Start()
    {
        playerController = CharacterManager.Instance.Player.playerController;
        playerCondition = CharacterManager.Instance.Player.playerCondition;
    }

    public void EquipNew(ItemData itemData)
    {
        UnEquip();
        curEquip = Instantiate(itemData.equipPrefab, equipParent).GetComponent<Equip>();
    }

    public void UnEquip()
    {
        if(curEquip != null)
        {
            Destroy(curEquip.gameObject);
            curEquip = null;
        }
    }
}
