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
    public List<PlayerItem> inventory;
    public List<PlayerItem> equipment;


    void Start()
    {
        instance = this;
    }

    public void ShowInventoryItems()
    {
        int index = 0;
        for(int i = 0; i <10; i++)
        {
            GameManager.instance.inventoryUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Image>().sprite = null;
            GameManager.instance.inventoryUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<Text>().text = "";  // ""-> null로 바꿀수도 있음"
            GameManager.instance.inventoryUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(i).GetComponent<InventorySlot>().item = new ItemStats();

        }
        foreach (PlayerItem playerItem in inventory)
        {
            Debug.Log($"inventory item at index {index} with itemId {playerItem.itemId}");
            GameManager.instance.inventoryUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(index).GetChild(2).GetComponent<Image>().sprite = GameManager.instance.itemSpriteMapping[playerItem.itemId];
            GameManager.instance.inventoryUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(index).GetChild(0).GetComponent<Text>().text = playerItem.equipSlot;
            GameManager.instance.inventoryUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(index).GetComponent<InventorySlot>().item = GameManager.instance.items.Find(item => item.itemId == playerItem.itemId);

            index++;

        }
    }

    public void ShowEquippedItems()
    {
        int index = 0;

        for(int i = 0; i<3; i++)
        {
            GameManager.instance.inventoryUI.transform.GetChild(5).GetChild(i).GetChild(2).GetComponent<Image>().sprite = null;
            GameManager.instance.inventoryUI.transform.GetChild(5).GetChild(i).GetChild(0).GetComponent<Text>().text = "";
            GameManager.instance.inventoryUI.transform.GetChild(5).GetChild(i).GetComponent<InventorySlot>().item = new ItemStats();
        }

        foreach (PlayerItem playerItem in equipment)
        {
            GameManager.instance.inventoryUI.transform.GetChild(5).GetChild(index).GetChild(2).GetComponent<Image>().sprite = GameManager.instance.itemSpriteMapping[playerItem.itemId];
            GameManager.instance.inventoryUI.transform.GetChild(5).GetChild(index).GetChild(0).GetComponent<Text>().text = playerItem.equipSlot;
            GameManager.instance.inventoryUI.transform.GetChild(5).GetChild(index).GetComponent<InventorySlot>().item = GameManager.instance.items.Find(item => item.itemId == playerItem.itemId);

            
            index++;
        }
    }

    public void UpdateInventoryCombinedStats()
    {

        Transform inventoryTransform = GameManager.instance.inventoryUI.transform.GetChild(3);

        Text userHp = inventoryTransform.GetChild(1).GetComponent<Text>();
        userHp.text = GameManager.instance.player.hp.ToString();

        Text userSpeed = inventoryTransform.GetChild(3).GetComponent<Text>();
        userSpeed.text = GameManager.instance.player.speed.ToString();

        Text userPower = inventoryTransform.GetChild(5).GetComponent<Text>();
        userPower.text = GameManager.instance.player.power.ToString();

        Text userDefense = inventoryTransform.GetChild(7).GetComponent<Text>();
        userDefense.text = GameManager.instance.player.defense.ToString();

        Text userCritical = inventoryTransform.GetChild(9).GetComponent<Text>();
        userCritical.text = GameManager.instance.player.critical.ToString();
    }

}
