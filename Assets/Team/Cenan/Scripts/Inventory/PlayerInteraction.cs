using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange;
    public InventorySystem inventorySystem;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Pickup item if holding nothing
            if (!inventorySystem.IsHoldingItem() || inventorySystem.IsHoldingItem())
            {
                Ray ray = new Ray(transform.position, transform.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, interactionRange))
                {
                    var interactable = hit.collider.GetComponent<Interactable>();
                    if (interactable != null)
                    {
                        Debug.Log($"Interacting with {interactable.item.itemName}.");
                        interactable.Interact(inventorySystem); // Add item to inventory
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (inventorySystem.IsHoldingItem())
            {
                inventorySystem.DropItem(inventorySystem.selectedSlotIndex);
            }
        }
    }






}