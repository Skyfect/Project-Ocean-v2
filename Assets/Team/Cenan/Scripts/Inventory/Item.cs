using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int maxStack = 1;
    public GameObject prefab; 

    public virtual void Use()
    {
        Debug.Log($"Using {itemName}");
    }
}
