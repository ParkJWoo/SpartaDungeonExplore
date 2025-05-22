using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform slotPanel;
    public Transform dropPosition;

    [Header("Selected Item")]
    private ItemData selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDesc;
    public TextMeshProUGUI selectedItemStatName;
    public TextMeshProUGUI selectedItemStatValue;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject dropButton;

    private int curEquipIndex;

    private PlayerController playerController;
    private PlayerCondition playerCondition;

    #region Start 메서드
    private void Start()
    {
        //  예외 처리
        if(CharacterManager.Instance?.Player == null)
        {
            Debug.LogError("CharacterManager.Instance.Player가 null입니다");
            return;
        }

        playerController = CharacterManager.Instance.Player.GetComponent<PlayerController>();
        playerCondition = CharacterManager.Instance.Player.GetComponent<PlayerCondition>();
        dropPosition = CharacterManager.Instance.Player.dropPosition;

        //  Action 호출 시 필요한 함수 등록
        playerController.inventory += Toggle;
        CharacterManager.Instance.Player.addItem += AddItem;

        //  인벤토리 UI 초기화 로직들
        inventoryWindow.SetActive(false);
        slots = new ItemSlot[slotPanel.childCount];

        for(int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
            slots[i].Clear();
        }

        ClearSelectedItemWindow();
    }
    #endregion

    #region 인벤토리 UI 초기화 메서드
    void ClearSelectedItemWindow()
    {
        selectedItem = null;

        selectedItemName.text = string.Empty;
        selectedItemDesc.text = string.Empty;
        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        useButton.SetActive(false);
        equipButton.SetActive(false);
        unEquipButton.SetActive(false);
        dropButton.SetActive(false);
    }
    #endregion

    #region Toggle 메서드
    public void Toggle()
    {
        if(IsOpen())
        {
            inventoryWindow.SetActive(false);
        }

        else
        {
            inventoryWindow.SetActive(true);
        }
    }
    #endregion

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    #region 인벤토리에 들어오는 아이템 처리 메서드
    public void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData;

        if(data.canStack)
        {
            ItemSlot slot = GetItemStack(data);

            if(slot != null)
            {
                slot.quantity++;
                UpdateUI();
                CharacterManager.Instance.Player.itemData = null;
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if(emptySlot != null)
        {
            emptySlot.itemData = data;
            emptySlot.quantity = 1;
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        ThrowItem(data);
        CharacterManager.Instance.Player.itemData = null;
    }
    #endregion

    public void UpdateUI()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemData != null)
            {
                slots[i].Set();
            }

            else
            {
                slots[i].Clear();
            }
        }
    }

    ItemSlot GetItemStack(ItemData data)
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemData == data && slots[i].quantity < data.maxStackAmount)
            {
                return slots[i];
            }
        }

        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemData == null)
            {
                return slots[i];
            }
        }

        return null;
    }

    #region [버리기] 버튼을 누를 시 아이템을 버리는 메서드
    public void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }
    #endregion

    #region 인벤토리 창에서 아이템 선택 처리 메서드
    public void SelectItem(int index)
    {
        if (slots[index].itemData == null)
        {
            return;
        }

        selectedItem = slots[index].itemData;
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.displayName;
        selectedItemDesc.text = selectedItem.description;

        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        for(int i = 0; i < selectedItem.consumables.Length; i++)
        {
            selectedItemStatName.text += selectedItem.consumables[i].type.ToString() + "\n";
            selectedItemStatValue.text += selectedItem.consumables[i].value.ToString() + "\n";
        }

        useButton.SetActive(selectedItem.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.type == ItemType.Equipable && !slots[index].isEquipped);
        unEquipButton.SetActive(selectedItem.type == ItemType.Equipable && slots[index].isEquipped);
        dropButton.SetActive(true);
    }
    #endregion

    #region [사용] 버튼 이벤트 처리 메서드
    public void OnUseButton()
    {
        if(selectedItem.type == ItemType.Consumable)
        {
            foreach(var consumable in selectedItem.consumables)
            {
                switch(consumable.type)
                {
                    case ConsumableType.Health:
                        playerCondition.Heal(consumable.value);
                        break;
                    case ConsumableType.Stanima:
                        playerCondition.StaminaHeal(consumable.value);
                        break;
                    case ConsumableType.SpeedUp:
                        playerCondition.SpeedUp(consumable.value, consumable.duration);
                        break;
                    case ConsumableType.JumpPowerUp:
                        playerCondition.JumpPowerUp(consumable.value, consumable.duration);
                        break;
                }
            }

            RemoveSelectedItem();
        }
    }
    #endregion

    #region [장착] 버튼 이벤트 처리 메서드
    public void OnEquipButton()
    {
        if (slots[curEquipIndex].isEquipped)
        {
            UnEquip(curEquipIndex);
        }

        slots[selectedItemIndex].isEquipped = true;
        curEquipIndex = selectedItemIndex;
        CharacterManager.Instance.Player.playerEquip.EquipNew(selectedItem); 
        UpdateUI();

        SelectItem(selectedItemIndex);
    }
    #endregion

    #region [해제] 버튼 이벤트 처리 메서드
    public void OnUnEqipButton()
    {
        UnEquip(selectedItemIndex);
    }
    #endregion

    #region [버리기] 버튼 이벤트 처리 메서드
    public void OnDropButton()
    {
        ThrowItem(selectedItem);
        RemoveSelectedItem();
    }
    #endregion

    #region 선택한 아이템 제거 메서드
    void RemoveSelectedItem()
    {
        slots[selectedItemIndex].quantity--;

        if (slots[selectedItemIndex].quantity <= 0)
        {
            selectedItem = null;
            slots[selectedItemIndex].itemData = null;
            selectedItemIndex = -1;
            ClearSelectedItemWindow();
        }

        UpdateUI();
    }
    #endregion

    public bool HasItem(ItemData item, int quantity)
    {
        return false;
    }

    #region 아이템 장착 해제 메서드
    void UnEquip(int index)
    {
        slots[index].isEquipped = false;
        CharacterManager.Instance.Player.playerEquip.UnEquip();
        UpdateUI();

        if (selectedItemIndex == index)
        {
            SelectItem(selectedItemIndex);
        }
    }
    #endregion
}
