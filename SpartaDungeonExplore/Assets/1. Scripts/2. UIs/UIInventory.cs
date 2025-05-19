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
    private ItemSlot selectedItem;
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

    //  인벤토리 UI 초기화 메서드
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

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

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

    public void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    public void SelectItem(int index)
    {
        if (slots[index].itemData == null)
        {
            return;
        }

        selectedItem = slots[index];
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.itemData.displayName;
        selectedItemDesc.text = selectedItem.itemData.description;

        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        for(int i = 0; i < selectedItem.itemData.consumables.Length; i++)
        {
            selectedItemStatName.text += selectedItem.itemData.consumables[i].type.ToString() + "\n";
            selectedItemStatValue.text += selectedItem.itemData.consumables[i].value.ToString() + "\n";
        }

        useButton.SetActive(selectedItem.itemData.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.itemData.type == ItemType.Equipable && !slots[index].isEquipped);
        unEquipButton.SetActive(selectedItem.itemData.type == ItemType.Equipable && slots[index].isEquipped);
        dropButton.SetActive(true);
    }

    public void OnUseButton()
    {
        if(selectedItem.itemData.type == ItemType.Consumable)
        {
            for(int i = 0; i < selectedItem.itemData.consumables.Length; i++)
            {
                switch(selectedItem.itemData.consumables[i].type)
                {
                    case ConsumableType.Health:
                        playerCondition.Heal(selectedItem.itemData.consumables[i].value);
                        break;
                    case ConsumableType.Stanima:
                        break;
                }
            }

            RemoveSelectedItem();
        }
    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem.itemData);
        RemoveSelectedItem();
    }

    void RemoveSelectedItem()
    {
        selectedItem.quantity--;

        if(selectedItem.quantity <= 0)
        {
            if (slots[selectedItemIndex].isEquipped)
            {
                UnEquip(selectedItemIndex);
            }

            selectedItem.itemData = null;
            ClearSelectedItemWindow();
        }

        UpdateUI();
    }

    public bool HasItem(ItemData item, int quantity)
    {
        return false;
    }

    void UnEquip(int index)
    {
        slots[index].isEquipped = false;
        //CharacterManager.Instance.Player.equip.UnEquip();

        UpdateUI();

        if (selectedItemIndex == index)
        {
            SelectItem(selectedItemIndex);
        }
    }

    public void OnUnEqipButton()
    {
        UnEquip(selectedItemIndex);
    }
}
