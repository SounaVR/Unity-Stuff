using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<AllItems> inventoryItems = new();

    private void Awake()
    {
        Instance = this;
    }

    public void AddItem(AllItems item)
    {
        inventoryItems.Add(item);
    }

    public void RemoveItem(AllItems item)
    {
        if (inventoryItems.Contains(item))
        {
            inventoryItems.Remove(item);
        }
    }

    public enum AllItems
    {
        Key,
        Banana
    }
}
