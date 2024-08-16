using UnityEngine;
using System.Collections.Generic;
using static Handlers;

public class EquipManager : MonoBehaviour
{
    public EquipPrefabs equipPrefab; // Reference to prefab
    public Transform slotsParent; // Parent transform for slots
    public Sprite[] weaponSprites; // Array of weapon sprites


    private Dictionary<int, Sprite> itemSpriteMapping; // Dictionary to map item ID to sprites

    void Start()
    {
        // Log the size of the weaponSprites array
        Debug.Log("weaponSprites array size: " + weaponSprites.Length);

        // Initialize itemSpriteMapping dictionary
        itemSpriteMapping = new Dictionary<int, Sprite>();

        // Map item IDs to their corresponding sprites
        for (int i = 0; i < weaponSprites.Length; i++)
        {
            Debug.Log($"Mapping weapon{i} to sprite: {weaponSprites[i]?.name}");
            itemSpriteMapping.Add(i + 1, weaponSprites[i]);
        }

        Handlers.instance.OnInventoryDataUpdated += HandleInventoryDataUpdated;
    }

    private void HandleInventoryDataUpdated()
    {
        // Access InventoryData from Handlers
        InventoryData inventoryData = Handlers.instance.inventoryData;

        // Ensure inventoryData has initialized lists
        if (inventoryData.allItems == null)
        {
            inventoryData.allItems = new List<Item>();
        }

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
                            Debug.Log($"Slot for item {item.itemId} reused and image set: {itemSprite.name}");
                        }
                        else
                        {
                            Debug.LogError($"No sprite found for item ID: {item.itemId}");
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
            Debug.LogError("No items in inventoryData.");
        }
    }
}

