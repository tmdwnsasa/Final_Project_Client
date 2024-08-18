using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using static Handlers;

public class InventoryManager : MonoBehaviour
{
    public SlotPrefabs slotPrefab; // Reference to prefab
    public Transform slotsParent; // Parent transform for slots
    public Sprite[] weaponSprites; // Array of weapon sprites

    private Dictionary<int, Sprite> itemSpriteMapping; // Dictionary to map item ID to sprites

    void Start()
    {
        // Initialize itemSpriteMapping dictionary
        InitializeItemSpriteMapping();

        // Start the coroutine to wait for Handlers initialization
        StartCoroutine(WaitForHandlersInitialization());
    }

    private void InitializeItemSpriteMapping()
    {
        if (itemSpriteMapping == null)
        {
            itemSpriteMapping = new Dictionary<int, Sprite>();

            // Map item IDs to their corresponding sprites
            for (int i = 0; i < weaponSprites.Length; i++)
            {
                itemSpriteMapping.Add(i + 1, weaponSprites[i]);
            }

            Debug.Log($"itemSpriteMapping initialized with {itemSpriteMapping.Count} entries.");
        }
    }

    private IEnumerator WaitForHandlersInitialization()
    {
        if (Handlers.instance == null)
        {
            Debug.Log("Handlers.instance is null, initializing Handlers.");
            GameObject handlersObject = new GameObject("Handlers");
            handlersObject.AddComponent<Handlers>();
        }

        // Wait until Handlers.instance is initialized
        while (Handlers.instance == null)
        {
            Debug.Log("Waiting for Handlers.instance to be initialized in InventoryManager.");
            yield return null; // Wait for the next frame
        }

        Handlers.instance.OnInventoryDataUpdated += HandleInventoryDataUpdated;
    }

    public Dictionary<int, Sprite> GetItemSpriteMapping()
    {
        if (itemSpriteMapping == null)
        {
            InitializeItemSpriteMapping();
        }
        return itemSpriteMapping;
    }

    private void HandleInventoryDataUpdated()
    {
        // Access InventoryData from Handlers
        InventoryData inventoryData = Handlers.instance.inventoryData;

        // Ensure inventoryData has initialized lists
        if (inventoryData.allItems == null)
        {
            Debug.LogWarning("allItems was null, initializing an empty list.");
            inventoryData.allItems = new List<Item>();
        }

        DisplayInventoryItems(inventoryData);
    }

    void DisplayInventoryItems(InventoryData inventoryData)
    {
        if (inventoryData.allItems.Count > 0)
        {
            int slotIndex = 0;

            foreach (Item item in inventoryData.allItems)
            {
                if (slotIndex < slotsParent.childCount) // Check if there are available slots
                {
                    SlotPrefabs existingSlot = slotsParent.GetChild(slotIndex).GetComponent<SlotPrefabs>();
                    if (existingSlot != null)
                    {
                        if (itemSpriteMapping.TryGetValue(item.itemId, out Sprite itemSprite))
                        {
                            existingSlot.SetSlotImage(itemSprite); // Set image on the existing slot
                        }
                        else
                        {
                            Debug.LogError($"No sprite found for item ID: {item.itemId}");
                        }
                    }
                    else
                    {
                        Debug.LogError("SlotPrefabs component is missing on the slot.");
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
