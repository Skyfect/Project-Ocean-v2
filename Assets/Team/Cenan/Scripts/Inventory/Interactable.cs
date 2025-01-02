using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Item item;

    public void Interact(InventorySystem inventorySystem)
    {
        if (inventorySystem.CanAddItem(item, 1))
        {
            inventorySystem.AddItem(item, 1, gameObject);
            gameObject.SetActive(false);//Hide the object if the item is received
        }
        else
        {
            Debug.LogWarning("Cannot pick up item: Stack limit reached or inventory is full.");
        }
    }
}
