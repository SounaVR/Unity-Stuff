using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    private GameObject currentTeleporter;

    [SerializeField] private Transform player;
    [SerializeField] private AudioSource teleportationSFX;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) {
            if (currentTeleporter != null) {
                var LockedDoor = currentTeleporter.GetComponent<LockedDoorBehaviour>();
                
                if (!LockedDoor.GetLockedStatus()) {
                    teleportationSFX.Play();
                    player.transform.position = currentTeleporter.GetComponent<Teleporter>().GetDestination().position;

                } else {
                    var requiredItem = LockedDoor.requiredItem;

                    if (LockedDoor.HasRequiredItem(requiredItem)) {
                        LockedDoor.unlockSFX.Play();
                        InventoryManager.Instance.RemoveItem(requiredItem);
                        Collectibles.Instance.DestroyFollowingKey();
                        Destroy(currentTeleporter.transform.GetChild(0).gameObject);
                        LockedDoor.IsLocked = false;
                    }
                }
            } 
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Teleporter")) {
            currentTeleporter = collision.gameObject;
            TooltipManager.Instance.ShowTooltip();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Teleporter")) {
            if (collision.gameObject == currentTeleporter) {
                currentTeleporter = null;
                TooltipManager.Instance.HideTooltip();
            }
        }
    }
}
