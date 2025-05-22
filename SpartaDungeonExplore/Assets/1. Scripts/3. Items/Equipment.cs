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

    #region 장비 장착 메서드
    public void EquipNew(ItemData itemData)
    {
        UnEquip();
        curEquip = Instantiate(itemData.equipPrefab, equipParent).GetComponent<Equip>();

        //  장비 효과 적용
        EquipTool equipTool = curEquip as EquipTool;

        if(equipTool != null)
        {
            playerController.moveSpeed += equipTool.addMoveSpeed;
            playerController.jumpPower += equipTool.addjumpPower;
        }
    }
    #endregion

    #region 장비 장착 해제 메서드
    public void UnEquip()
    {
        //  적용된 장비 효과도 같이 해제!

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
