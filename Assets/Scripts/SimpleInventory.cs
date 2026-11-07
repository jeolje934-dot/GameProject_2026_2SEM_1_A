using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerController;

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

    void Start()
    {
        UpdateUI();
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

    public bool AddItem(string itemName, int amount)
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