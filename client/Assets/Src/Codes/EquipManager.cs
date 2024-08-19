//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using static Handlers;
//using System;

//public class EquipManager : MonoBehaviour
//{
//    public EquipPrefabs equipPrefab; // Reference to prefab
//    public Transform slotsParent; // Parent transform for slots
//    private Dictionary<int, Sprite> itemSpriteMapping; 
//    private InventoryManager inventoryManager;

//    void Start()
//    {
//        inventoryManager = FindObjectOfType<InventoryManager>();
//        if (inventoryManager == null)
//        {
//            Debug.LogError("InventoryManager not found in the scene.");
//            return;
//        }
//        else
//        {
//            itemSpriteMapping = inventoryManager.GetItemSpriteMapping();

//            // Fallback to re-initialize itemSpriteMapping if it's still null
//            if (itemSpriteMapping == null)
//            {
//                Debug.LogWarning("itemSpriteMapping is null. Attempting to reinitialize.");
//                inventoryManager.GetItemSpriteMapping(); // Force reinitialization
//                itemSpriteMapping = inventoryManager.GetItemSpriteMapping();

//                if (itemSpriteMapping == null)
//                {
//                    Debug.LogError("Failed to initialize itemSpriteMapping.");
//                    return;
//                }
//            }

//            Debug.Log($"Retrieved itemSpriteMapping with {itemSpriteMapping.Count} entries.");
//        }

//        StartCoroutine(WaitForHandlersInitialization());
//    }

//    private IEnumerator WaitForHandlersInitialization()
//    {
//        while (Handlers.instance == null)
//        {
//            yield return null;
//        }

//        Handlers.instance.OnInventoryDataUpdated += HandleInventoryDataUpdated;
//    }

//    private void HandleInventoryDataUpdated()
//    {
//        InventoryData inventoryData = Handlers.instance.inventoryData;

//        if (inventoryData.equippedItems == null)
//        {
//            inventoryData.equippedItems = new List<Item>();
//        }

//        StartCoroutine(DisplayEquippedItemsCoroutine(inventoryData));
//    }

//    IEnumerator DisplayEquippedItemsCoroutine(InventoryData inventoryData)
//    {
//        if (inventoryData.equippedItems.Count > 0)
//        {
//            int slotIndex = 0;

//            foreach (Item item in inventoryData.equippedItems)
//            {
//                EquipPrefabs existingSlot = null;

//                Debug.Log($"Processing equipped item {item.itemId} at slotIndex {slotIndex} with name: {item.itemSpriteName}");

//                if (string.IsNullOrEmpty(item.itemSpriteName))
//                {
//                    Debug.LogError($"Item {item.itemId} has a null or empty sprite name.");
//                    continue;
//                }

//                if (!itemSpriteMapping.ContainsKey(item.itemId))
//                {
//                    Debug.LogError($"itemSpriteMapping does not contain itemId: {item.itemId}");
//                    continue;
//                }

//                if (slotIndex < slotsParent.childCount)
//                {
//                    Transform slotTransform = slotsParent.GetChild(slotIndex);
//                    Debug.Log($"Slot at index {slotIndex} found: {slotTransform.name}");

//                    existingSlot = slotTransform.GetComponent<EquipPrefabs>();
//                    if (existingSlot != null)
//                    {
//                        Debug.Log($"EquipPrefabs component found on slot at index {slotIndex}.");

//                        // Additional null checks
//                        if (existingSlot.slotImage == null)
//                        {
//                            Debug.LogError("slotImage is null in EquipPrefabs.");
//                        }
//                        if (existingSlot.itemNameText == null)
//                        {
//                            Debug.LogError("itemNameText is null in EquipPrefabs.");
//                        }

//                        try
//                        {
//                            if (itemSpriteMapping.TryGetValue(item.itemId, out Sprite itemSprite))
//                            {
//                                Debug.Log($"itemSprite found for item {item.itemId}: {(itemSprite != null ? itemSprite.name : "null")}");

//                                if (existingSlot.slotImage != null && existingSlot.itemNameText != null)
//                                {
//                                    Debug.Log($"Setting slot image and name for item {item.itemId}.");
//                                    existingSlot.SetSlotImage(itemSprite);
//                                    existingSlot.SetItemName(item.itemSpriteName);

//                                    Debug.Log($"Slot for equipped item {item.itemId} reused and image set: {itemSprite.name}");
//                                }
//                                else
//                                {
//                                    Debug.LogError("slotImage or itemNameText is not assigned in EquipPrefabs.");
//                                }
//                            }
//                            else
//                            {
//                                Debug.LogError($"No sprite found for equipped item ID: {item.itemId}");
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Debug.LogError($"Exception occurred while processing item {item.itemId} at slotIndex {slotIndex}: {ex.Message}");
//                            Debug.LogError($"State at error: Slot Image: {existingSlot?.slotImage?.sprite?.name}, Item Name Text: {existingSlot?.itemNameText?.text}");

//                            Debug.LogError($"existingSlot: {existingSlot != null}, slotImage: {existingSlot?.slotImage}, itemNameText: {existingSlot?.itemNameText}");
//                        }
//                    }
//                    else
//                    {
//                        Debug.LogError("EquipPrefabs component is missing on the slot.");
//                    }
//                }
//                else
//                {
//                    Debug.LogError("No more slots available in the slotsParent.");
//                }

//                slotIndex++;

//                // Small delay after each item processing (if needed)
//                yield return new WaitForSeconds(0.1f);
//            }
//        }
//        else
//        {
//            Debug.LogError("No items in inventoryData (equipped).");
//        }

//        yield break;
//    }
//}
