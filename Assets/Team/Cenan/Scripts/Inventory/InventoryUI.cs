using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;             
    public Transform inventoryPanel;         
    public InventorySystem inventorySystem;   
    private List<GameObject> slotObjects = new List<GameObject>(); 
    private int maxSlots;                     
    private int selectedSlotIndex = 0;        
    public Color selectedColor = Color.yellow; 
    public Color defaultColor = Color.white;  

    public void Start()
    {
       
        if (inventorySystem != null)
        {
            inventorySystem.InventoryChanged += UpdateUI;
        }
        maxSlots = inventorySystem.maxSlots;
        CreateSlots();
        UpdateUI();
        UpdateSlotSelection();
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0)
        {
            selectedSlotIndex = (selectedSlotIndex + 1) % maxSlots;
            inventorySystem.SelectNextSlot();
        }
        else if (scroll < 0)
        {
            selectedSlotIndex = (selectedSlotIndex - 1 + maxSlots) % maxSlots;
            inventorySystem.SelectPreviousSlot();
        }
    }

    public void CreateSlots()
    {
        foreach (Transform child in inventoryPanel)
        {
            Destroy(child.gameObject);
        }

        slotObjects.Clear();

        for (int i = 0; i < maxSlots; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, inventoryPanel);
            slotObjects.Add(newSlot);
        }
    }


    public void UpdateUI()
    {
        List<InventorySlot> inventory = inventorySystem.GetInventory();

        for (int i = 0; i < slotObjects.Count; i++)
        {
            GameObject slot = slotObjects[i];
            TextMeshProUGUI slotText = slot.GetComponentInChildren<TextMeshProUGUI>();
            Image slotImage = slot.transform.Find("Image").GetComponent<Image>();

            if (i < inventory.Count && inventory[i].item != null)
            {
                InventorySlot itemSlot = inventory[i];

                // Slot is filled
                if (slotText != null)
                    slotText.text = $"{itemSlot.item.itemName} x{itemSlot.quantity}";

                if (slotImage != null)
                {
                    slotImage.sprite = itemSlot.item.icon;
                    slotImage.enabled = true;
                }
            }
            else
            {
                if (slotText != null)
                    slotText.text = "Empty Slot";

                if (slotImage != null)
                {
                    slotImage.sprite = null;
                    slotImage.enabled = false;
                }
            }
        }

        UpdateSlotSelection();
    }






    private void UpdateSlotSelection()
    {
        for (int i = 0; i < slotObjects.Count; i++)
        {
            Image slotBackground = slotObjects[i].transform.Find("Backgroundd").GetComponent<Image>();

            if (slotBackground != null)
            {
                slotBackground.color = i == selectedSlotIndex ? selectedColor : defaultColor;
            }
        }
        Debug.Log($"Selected Slot Index: {selectedSlotIndex}");
    }
}
