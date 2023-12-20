using UnityEngine;

public class LockedDoorBehaviour : MonoBehaviour
{
    [SerializeField] public bool IsLocked = true;
    [SerializeField] public InventoryManager.AllItems requiredItem;
    [SerializeField] public AudioSource unlockSFX;
    
    public bool GetLockedStatus()
    {
        return IsLocked;
    }

    public bool HasRequiredItem(InventoryManager.AllItems itemRequired)
    {
        if (InventoryManager.Instance.inventoryItems.Contains(itemRequired))
        {
            return true;
        } else return false;
    }
}
