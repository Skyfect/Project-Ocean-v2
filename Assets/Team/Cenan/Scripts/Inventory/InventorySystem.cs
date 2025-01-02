using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventorySystem : MonoBehaviour
{
    public int maxSlots;
    private List<InventorySlot> inventory = new List<InventorySlot>();
    private GameObject heldItem;
    public int selectedSlotIndex = 0; 
    public delegate void InventoryChangedHandler();
    public event InventoryChangedHandler InventoryChanged;
    public TextMeshProUGUI raycastText;
    public void AddItem(Item item, int quantity, GameObject obj)
    {
        // Check if the selected slot has the same item and there is space in the stack
        if (selectedSlotIndex < inventory.Count && inventory[selectedSlotIndex].item == item)
        {
            var selectedSlot = inventory[selectedSlotIndex];
            if (selectedSlot.quantity + quantity <= item.maxStack)
            {
                selectedSlot.quantity += quantity;
                InventoryChanged?.Invoke();
                return;
            }
        }

        // Try to find an empty slot or a slot that can fit the item stack
        for (int i = 0; i < maxSlots; i++)
        {
            // If the slot is empty or can hold more of the same item
            if (i >= inventory.Count || inventory[i].item == null || (inventory[i].item == item && inventory[i].quantity + quantity <= item.maxStack))
            {
                if (i >= inventory.Count)
                {
                    inventory.Add(new InventorySlot(item, quantity)); // Add a new slot if necessary
                }
                else
                {
                    inventory[i] = new InventorySlot(item, quantity); // Replace with the new item if the slot was empty
                }

                if (i == selectedSlotIndex)
                {
                    HoldItem(item.prefab); // Hold the item in hand if it's selected
                }

                InventoryChanged?.Invoke();
                return;
            }
        }

        // If no space is available in the inventory
        raycastText.text = "Inventory is full!";
        Debug.LogWarning("Inventory is full!");
    }




    public void HoldItem(GameObject prefab)
    {
        if (heldItem == null)
        {
            Destroy(heldItem); 
        }

        heldItem = Instantiate(prefab, transform);
        heldItem.transform.localPosition = new Vector3(0, 0, 1); 
        heldItem.transform.localRotation = Quaternion.identity;
    }

    public void DropItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventory.Count || inventory[slotIndex].item == null)
        {
            raycastText.text = "Invalid slot index or no item to drop.";
            Debug.LogWarning("Invalid slot index or no item to drop.");
            return;
        }

        InventorySlot slot = inventory[slotIndex];
        GameObject droppedItem = Instantiate(slot.item.prefab, transform.position + transform.forward * 1.5f, Quaternion.identity);

        Rigidbody rb = droppedItem.GetComponent<Rigidbody>();
        if (rb == null) rb = droppedItem.AddComponent<Rigidbody>();
        rb.isKinematic = false;

        Collider col = droppedItem.GetComponent<Collider>();
        if (col == null) col = droppedItem.AddComponent<BoxCollider>();

        if (slot.quantity > 1)
        {
            slot.quantity--; 
        }
        else
        {
            inventory[slotIndex] = new InventorySlot(null, 0); //Clear the slot
            Destroy(heldItem);
        }

        InventoryChanged?.Invoke();


        Debug.Log($"Dropped item: {slot.item.itemName}. Remaining quantity: {slot.quantity}");
    }



    public bool HasAnyItemAtSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventory.Count) return false;
        return inventory[slotIndex].item != null && inventory[slotIndex].quantity > 0;
    }


    public void RemoveItemAt(int slotIndex)
    {
        if (slotIndex < inventory.Count)
        {
            inventory[slotIndex] = new InventorySlot(null, 0);
        }
    }

    public void SelectNextSlot()
    {
        selectedSlotIndex = (selectedSlotIndex + 1) % maxSlots;
        UpdateHeldItem();
    }

    public void SelectPreviousSlot()
    {
        selectedSlotIndex = (selectedSlotIndex - 1 + maxSlots) % maxSlots;
        UpdateHeldItem();
    }

    private void UpdateHeldItem()
    {
        
        if (selectedSlotIndex < inventory.Count && inventory[selectedSlotIndex].item != null)
        {
          
            if (heldItem != null)
            {
                Destroy(heldItem);
            }

            HoldItem(inventory[selectedSlotIndex].item.prefab);
        }
        else
        {
            if (heldItem != null)
            {
                Destroy(heldItem);
                heldItem = null; 
            }
        }

        InventoryChanged?.Invoke(); 
    }


    public bool HasAnyItem()
    {
        foreach (var slot in inventory)
        {
            if (slot.item != null && slot.quantity > 0)
                return true;
        }
        return false;
    }

    public bool IsHoldingItem()
    {
        return selectedSlotIndex >= 0 && selectedSlotIndex < inventory.Count && inventory[selectedSlotIndex].item != null;
    }


    public List<InventorySlot> GetInventory()
    {
        return inventory;
    }

    public bool CanAddItem(Item item, int quantity)
    {
        // Check if the selected slot has space for the item stack
        if (selectedSlotIndex < inventory.Count && inventory[selectedSlotIndex].item == item)
        {
            return inventory[selectedSlotIndex].quantity + quantity <= item.maxStack;
        }

        // Check if there is space in any of the inventory slots
        foreach (var slot in inventory)
        {
            if (slot.item == item && slot.quantity + quantity <= item.maxStack)
            {
                return true;
            }
        }

        // If there is any empty slot available
        for (int i = 0; i < maxSlots; i++)
        {
            if (i >= inventory.Count || inventory[i].item == null)
            {
                return true;
            }
        }

        return false; // If no space is available
    }


}
