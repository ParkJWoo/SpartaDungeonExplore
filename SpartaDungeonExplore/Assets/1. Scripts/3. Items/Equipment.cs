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

    #region ��� ���� �޼���
    public void EquipNew(ItemData itemData)
    {
        UnEquip();
        curEquip = Instantiate(itemData.equipPrefab, equipParent).GetComponent<Equip>();

        //  ��� ȿ�� ����
        EquipTool equipTool = curEquip as EquipTool;

        if(equipTool != null)
        {
            playerController.moveSpeed += equipTool.addMoveSpeed;
            playerController.jumpPower += equipTool.addjumpPower;
        }
    }
    #endregion

    #region ��� ���� ���� �޼���
    public void UnEquip()
    {
        //  ����� ��� ȿ���� ���� ����!

        if (curEquip != null)
        {
            EquipTool equipTool = curEquip as EquipTool;

            if(equipTool != null)
            {
                playerController.moveSpeed -= equipTool.addMoveSpeed;
                playerController.jumpPower -= equipTool.addjumpPower;
            }

            Destroy(curEquip.gameObject);
            curEquip = null;
        }
    }
    #endregion
}
