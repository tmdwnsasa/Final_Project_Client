using UnityEngine;
using System.Collections.Generic;
using static Handlers;

public class EquipManager : MonoBehaviour
{
    public EquipPrefabs equipPrefab; // Reference to prefab
    public Transform slotsParent; // Parent transform for slots
    private Dictionary<int, Sprite> itemSpriteMapping; // Dictionary to map item ID to sprites
    private InventoryManager inventoryManager;


    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager != null)
        {
            itemSpriteMapping = inventoryManager.GetItemSpriteMapping();
        }
        else
        {
            Debug.LogError("InventoryManager not found in the scene.");
        }

        Handlers.instance.OnInventoryDataUpdated += HandleInventoryDataUpdated;
    }

    private void HandleInventoryDataUpdated()
    {
        // Access InventoryData from Handlers
        InventoryData inventoryData = Handlers.instance.inventoryData;

        if (inventoryData.equippedItems == null)
        {
            inventoryData.equippedItems = new List<Item>();
        }

        DisplayEquippedItems(inventoryData);

    }

    void DisplayEquippedItems(InventoryData inventoryData)
    {
        if (inventoryData.equippedItems.Count > 0)
        {
            int slotIndex = 0;

            foreach (Item item in inventoryData.equippedItems)
            {
                if (slotIndex < slotsParent.childCount) // Check if there are available slots
                {
                    EquipPrefabs existingSlot = slotsParent.GetChild(slotIndex).GetComponent<EquipPrefabs>();
                    if (existingSlot != null)
                    {
                        if (itemSpriteMapping.TryGetValue(item.itemId, out Sprite itemSprite))
                        {
                            existingSlot.SetSlotImage(itemSprite); // Set image on the existing slot
                            Debug.Log($"Slot for equipped item {item.itemId} reused and image set: {itemSprite.name}");
                        }
                        else
                        {
                            Debug.LogError($"No sprite found for equipped item ID: {item.itemId}");
                        }
                    }
                    else
                    {
                        Debug.LogError("EquipPrefabs component is missing on the slot.");
                    }
                    slotIndex++;
                }
                else
                {
                    Debug.LogError("No more slots available in the slotsParent.");
                }
            }
        }
        else
        {
            Debug.LogError("No items in inventoryData (equipped).");
        }
    }
}

