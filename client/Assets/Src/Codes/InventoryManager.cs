using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using static Handlers;
using UnityEngine.UI;
using System;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public SlotPrefabs slotPrefab; // Reference to prefab
    public Transform slotsParent; // Parent transform for slots
    public Sprite[] itemSprites; // Array of weapon sprites
    public List<PlayerItem> inventory;
    public List<PlayerItem> equipment;



    private Dictionary<int, Sprite> itemSpriteMapping; // Dictionary to map item ID to sprites

    void Start()
    {
        instance = this;
    }


    public void InitializeItemSpriteMapping()
    {
        if (itemSpriteMapping == null)
        {
            itemSpriteMapping = new Dictionary<int, Sprite>();

            // Map item IDs to their corresponding sprites
            for (int i = 0; i < itemSprites.Length; i++)
            {
                itemSpriteMapping.Add(i + 1, itemSprites[i]);
            }

            Debug.Log($"itemSpriteMapping initialized with {itemSpriteMapping.Count} entries.");
        }



    }

    public Dictionary<int, Sprite> GetItemSpriteMapping()
    {
        if (itemSpriteMapping == null)
        {
            InitializeItemSpriteMapping();
        }
        return itemSpriteMapping;
    }

    public void ShowInventoryItems()
    {
        int index = 0;
        for(int i = 0; i <10; i++)
        {
            GameManager.instance.inventoryUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Image>().sprite = null;
            GameManager.instance.inventoryUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<Text>().text = "";
            GameManager.instance.inventoryUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(i).GetComponent<InventorySlot>().item = new ItemStats();

        }
        foreach (PlayerItem playerItem in inventory)
        {
            Debug.Log($"inventory item at index {index} with itemId {playerItem.itemId}");
            GameManager.instance.inventoryUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(index).GetChild(2).GetComponent<Image>().sprite = itemSpriteMapping[playerItem.itemId];
            GameManager.instance.inventoryUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(index).GetChild(0).GetComponent<Text>().text = playerItem.equipSlot;
            GameManager.instance.inventoryUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(index).GetComponent<InventorySlot>().item = GameManager.instance.items.Find(item => item.itemId == playerItem.itemId);

            index++;

        }
    }

    public void ShowEquippedItems()
    {

        Debug.Log($"Equipment count{equipment.Count}");
        int index = 0;

        foreach (PlayerItem playerItem in equipment)
        {
            Debug.Log(playerItem.itemId);
            GameManager.instance.inventoryUI.transform.GetChild(5).GetChild(index).GetChild(2).GetComponent<Image>().sprite = itemSpriteMapping[playerItem.itemId];
            GameManager.instance.inventoryUI.transform.GetChild(5).GetChild(index).GetChild(0).GetComponent<Text>().text = playerItem.equipSlot;
            GameManager.instance.inventoryUI.transform.GetChild(5).GetChild(index).GetComponent<InventorySlot>().item = GameManager.instance.items.Find(item => (item.itemId == playerItem.itemId));

            
            index++;
        }
    }


}
