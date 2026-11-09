using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static PlayerController;
using static UnityEditor.Progress;


public class SimpleInventory : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private GameObject inventoryPanel;       // 인벤토리 UI 패널
    [SerializeField] private TextMeshProUGUI[] slotText;      // 인벤토리 슬롯에 표시할 텍스트

    private List<InventoryItem> items = new List<InventoryItem>(); // items를 관리하는 List 자료 구조선언

    private PlayerController playerController;

    private PlayerPickup playerPickup;

    private ThirdPersonCamera thirdPersonCamera; //인벤토리가 오픈되었을때 임의로 카메라가 움직이지 않게 하기 위해

    private bool isOpen;


    [SerializeField] private int selectedIndex = -1; //-1 : 선택 없음 . 0 : 첫번째 슬롯, 1: 두번째 슬롯 -> 3

    [SerializeField] private TextMeshProUGUI selectedltemText;

    [SerializeField] private TextMeshProUGUI descriptionText;


    [SerializeField] private Button useButton;

    [SerializeField] private Button dropButton;

    [SerializeField] private PlayerHealth playerHealth;

    [SerializeField] private int healAmount = 30;

    void Start()
    {
        UpdateUI();

    }

    public void SelectSlot(int index)  //슬롯 선택함수
    {
        if (index < 0 || index >= items.Count) return;

        selectedIndex = index;

        UpdateUI();

    }

    public void SelectSlot0() { SelectSlot(0); }
    public void SelectSlot1() { SelectSlot(1); }
    public void SelectSlot2() { SelectSlot(2); }
    public void SelectSlot3() { SelectSlot(3); }

    private bool CanUseltem(string itemName)
    {

        return itemName == "물약" || itemName == "회복약" || itemName == "구급키트";
    }

    public bool RemoveItem(string itemName, int amount)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemName != itemName) continue;  //아이템 이름이 같은지
            if (items[i].amount < amount) return false;   //충분한 수량이 있는지

            items[i].amount -= amount;

            if (items[i].amount <= 0)
            {
                items.RemoveAt(i);
                selectedIndex = -1;
            }
            UpdateUI();
            return true;
        }
        return false;

    }

    public void UseSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Count) return;

        InventoryItem item = items[selectedIndex];

        if (CanUseltem(item.itemName) == false)
        {
            selectedltemText.text = item.itemName + "은(는) 사용 할 수 없습니다."; return;
        }
        playerHealth. Heal(healAmount);
        RemoveItem(item.itemName, 1);
    }

    public void DropSelectedltem()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Count) return;

        string itemName = items[selectedIndex].itemName;
        RemoveItem(itemName, 1);
    }



    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerPickup = GetComponent<PlayerPickup>();

        if (Camera.main != null)
        {
            thirdPersonCamera = Camera.main.GetComponent<ThirdPersonCamera>();
        }

        inventoryPanel.SetActive(false);
    }

    

    

    private void UpdateUI()
    {
        for (int i = 0; i < slotText.Length; i++)
        {
            if (i < items.Count)
            {
                slotText[i].text = items[i].itemName + "\n" + items[i].amount + "개";
            }
            else
            {
                slotText[i].text = "비어 있음";
            }
        }
        if(selectedIndex >= 0 && selectedIndex < items.Count)
        {
            selectedltemText.text = "EH : " + items[selectedIndex].itemName;
            descriptionText.text = items[selectedIndex].description;

            useButton.interactable = CanUseltem(items[selectedIndex].itemName);
            dropButton.interactable = true;
        }
        else
        {
            selectedltemText.text = "아이템을 선택하세요";
            descriptionText.text = "슬롯을 클릭하면 설명이 표시 됩니다.";

            useButton.interactable = false;
            dropButton.interactable = false;
        }



    }

    public int GetltemAmount(string itemName)
    {
        foreach (InventoryItem item in items)
        {
            if (item.itemName == itemName)
            {
                return item.amount;
            }
        }
        return 0;
    }













    private void ToggleInventory()
    {
        if (isOpen == false && playerController.IsNormalState() == false)
        {
            return;
        }

        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            playerController.ChangeState(PlayerState.inventory);
            playerPickup.enabled = false;

            if (thirdPersonCamera != null)
            {
                thirdPersonCamera.enabled = false;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            playerController.ChangeState(PlayerState.Normal);
            playerPickup.enabled = true;

            if (thirdPersonCamera != null)
            {
                thirdPersonCamera.enabled = true;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public bool AddItem(string itemName, int amount , string description)
    {
        foreach (InventoryItem item in items) // Slot을 순환 하면서 아이템 이름이 같은 것이 있을 경우 수량을 추가 해준다.
        {
            if (item.itemName == itemName)
            {
                item.amount += amount;
                UpdateUI();
                return true;
            }
        }

        if (items.Count >= slotText.Length)
        {
            Debug.Log("인벤토리가 가득 찼습니다.");
            return false;
        }

        InventoryItem newItem = new InventoryItem();
        newItem.itemName = itemName;
        newItem.amount = amount;
        newItem.description = description;

        items.Add(newItem);
        UpdateUI();

        return true;
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null) return;

        if (keyboard.iKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }
}