using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Handlers : MonoBehaviour
{
    public static Handlers instance;

    public InventoryData inventoryData;
    public event Action OnInventoryDataUpdated;


    public enum HandlerIds
    {
        LOGIN = 0,
        REGISTER = 1,
        UPDATE_LOCACTION = 2,
        CREATE_GAME = 4,
        JOIN_GAME = 5,
        JOIN_LOBBY = 6,
        CHOICE_CHARACTER = 7,
        SELECT_CHARACTER = 8,
        GIVE_CHARACTER = 9,
        CHATTING = 10,
        MATCHMAKING = 11,
        GAME_END = 15,
        RETURN_LOBBY = 16,
        INVENTORY = 17,
        EQUIP_ITEM=18,
        UNEQUIP_ITEM=19,
        EXIT = 20,
        OPEN_STORE = 29,
        PURCHASE_CHARACTER = 30,
        SKILL = 50,
    }

    [Serializable]
    public struct CharacterChoice
    {
        public string playerId;
        public string name;
        public string sessionId;
    }

    [Serializable]
    public struct CharacterSelect
    {
        public string playerId;
        public string name;
        public string sessionId;
        public List<uint> possession;
    }

    [Serializable]
    public struct CharacterStats
    {
        public int characterId;
        public string characterName;
        public float hp;
        public float speed;
        public float power;
        public float defense;
        public float critical;
        public int price;
    }

    [Serializable]
    public struct UserMoney
    {
        public int money;
    }

    [Serializable]
    public struct PurchaseStateMessage
    {
        public string message;
    }

    [Serializable]
    public class InventoryData 
    {
        public UserMoney userMoney;
        public List<Item> allItems;
        public List<Item> equippedItems;
        public CombinedStats combinedStats;
    }

    [Serializable]
    public struct CombinedStats
    {
        public float hp;
        public float speed;
        public float power;
        public float defense;
        public float critical;
    }

    [Serializable]
    public struct Item
    {
        public int inventoryId;
        public string playerId;
        public int itemId;
        public int equippedItems;
        public string equipSlot;
        public string itemSpriteName;

        public bool IsEquipped => equippedItems == 1;
    }

    [Serializable]
    public class UpdatedInventoryData
    {
        public string message;
        public CombinedStats combinedStats;
        public List<Item> equippedItems;
        public List<Item> allItems;

    }



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void GetCharacterChoice(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);
        CharacterChoice characterChoice = JsonUtility.FromJson<CharacterChoice>(jsonString);

        GameManager.instance.playerId = characterChoice.playerId;
        GameManager.instance.name = characterChoice.name;
        GameManager.instance.sessionId = characterChoice.sessionId;

        GameManager.instance.GoCharacterChoice();
    }

    public void GetCharacterSelect(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);
        CharacterSelect characterSelect = JsonUtility.FromJson<CharacterSelect>(jsonString);

        GameManager.instance.playerId = characterSelect.playerId;
        GameManager.instance.name = characterSelect.name;
        GameManager.instance.sessionId = characterSelect.sessionId;
        GameManager.instance.possession = characterSelect.possession;

        GameManager.instance.GoCharacterSelect();
    }

    public void SetCharacterStats(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);
        CharacterStats characterStats = JsonUtility.FromJson<CharacterStats>(jsonString);

        GameManager.instance.player.characterId = characterStats.characterId;
        GameManager.instance.player.characterName = characterStats.characterName;
        GameManager.instance.player.hp = characterStats.hp;
        GameManager.instance.player.speed = characterStats.speed;
        GameManager.instance.player.power = characterStats.power;
        GameManager.instance.player.defense = characterStats.defense;
        GameManager.instance.player.critical = characterStats.critical;
    }

    public void StoreOpen(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);
        UserMoney userMoney = JsonUtility.FromJson<UserMoney>(jsonString);
        Text userGold = GameManager.instance.storeUI.transform.GetChild(5).GetChild(0).GetComponent<Text>();
        userGold.text = userMoney.money.ToString();
        GameManager.instance.chattingUI.SetActive(false);
        GameManager.instance.exitBtn.SetActive(false);
        GameManager.instance.matchStartUI.SetActive(false);
        GameManager.instance.storeBtn.SetActive(false);
        GameManager.instance.storeUI.SetActive(true);
        GameManager.instance.purchaseMessageUI.SetActive(false);
        GameManager.instance.purchaseCheckUI.transform.GetChild(0).GetComponent<Button>().interactable = true;
    }

    public void PurchaseMessage(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);
        PurchaseStateMessage purchaseStateMessage = JsonUtility.FromJson<PurchaseStateMessage>(jsonString);
        Text message = GameManager.instance.purchaseMessageUI.transform.GetChild(1).GetComponent<Text>();
        message.text = purchaseStateMessage.message;
        GameManager.instance.purchaseCheckUI.SetActive(false);
        GameManager.instance.purchaseMessageUI.SetActive(true);
    }
    public void ReturnLobbySetting()
    {
        GameManager.instance.isLive = true;
        GameManager.instance.player.ResetAnimation();
        // GameManager.instance.player.transform.position = new Vector2(0, 0);
        NetworkManager.instance.isLobby = true;

        GameManager.instance.matchStartUI.SetActive(true);
        GameManager.instance.exitBtn.SetActive(true);
        GameManager.instance.player.hpSlider.gameObject.SetActive(false);
        GameManager.instance.gameEndUI.transform.GetChild(3).GetComponent<Button>().interactable = true;
    }

    public void SetCharactersCombinedStats(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);

        InventoryData inventoryData = JsonUtility.FromJson<InventoryData>(jsonString);
        CombinedStats combinedStats = JsonUtility.FromJson<CombinedStats>(jsonString);

        Debug.Log($"Parsed CombinedStats: HP={inventoryData.combinedStats.hp}, Speed={inventoryData.combinedStats.speed}, Power={inventoryData.combinedStats.power}, Defense={inventoryData.combinedStats.defense}, Critical={inventoryData.combinedStats.critical}");

        Transform inventoryTransform = GameManager.instance.inventoryUI.transform.GetChild(3);

        Text userHp = inventoryTransform.GetChild(1).GetComponent<Text>();
        userHp.text = inventoryData.combinedStats.hp.ToString();

        Text userSpeed = inventoryTransform.GetChild(3).GetComponent<Text>();
        userSpeed.text = inventoryData.combinedStats.speed.ToString();

        Text userPower = inventoryTransform.GetChild(5).GetComponent<Text>();
        userPower.text = inventoryData.combinedStats.power.ToString();

        Text userDefense = inventoryTransform.GetChild(7).GetComponent<Text>();
        userDefense.text = inventoryData.combinedStats.defense.ToString();

        Text userCritical = inventoryTransform.GetChild(9).GetComponent<Text>();
        userCritical.text = inventoryData.combinedStats.critical.ToString();
    }


    public void SetUserMoney(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);

        InventoryData inventoryData = JsonUtility.FromJson<InventoryData>(jsonString);
        UserMoney userMoney = JsonUtility.FromJson<UserMoney>(jsonString);

        Debug.Log($"Parsed money: {inventoryData.userMoney.money}");


        Text userCoin = GameManager.instance.inventoryUI.transform.GetChild(2).GetChild(1).GetComponent<Text>();
        userCoin.text = inventoryData.userMoney.money.ToString();
    }

    public void SetInventoryItemData(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);

        Handlers.instance.inventoryData = JsonUtility.FromJson<InventoryData>(jsonString);
        Debug.Log($"Received updatedInventoryData with {inventoryData.allItems.Count} items and {inventoryData.equippedItems.Count} equipped items.");



        if (Handlers.instance.inventoryData.allItems == null)
            {
            Handlers.instance.inventoryData.allItems = new List<Item>();
            }

            if (Handlers.instance.inventoryData.equippedItems == null)
            {
            Handlers.instance.inventoryData.equippedItems = new List<Item>();
            }
            OnInventoryDataUpdated?.Invoke();
    }


    public void UpdateEquipItem(byte[] data)
    {
        UpdateInventoryAndStats(data);
    }

    public void UpdateUnequipItem(byte[] data)
    {
        UpdateInventoryAndStats(data);
    }

    private void UpdateInventoryAndStats(byte[] data)
    {
        // Deserialize the response data into UpdatedStats
        string jsonString = Encoding.UTF8.GetString(data);
        UpdatedInventoryData updatedInventoryData = JsonUtility.FromJson<UpdatedInventoryData>(jsonString);

        // Update the inventory data with the new items and stats
        inventoryData.combinedStats = updatedInventoryData.combinedStats;
        inventoryData.allItems = updatedInventoryData.allItems;
        inventoryData.equippedItems = updatedInventoryData.equippedItems;

        UpdateStatsUI(inventoryData.combinedStats);


        OnInventoryDataUpdated?.Invoke();
    }
    public void UpdateStatsUI(CombinedStats combinedStats)
    {
        Transform inventoryTransform = GameManager.instance.inventoryUI.transform.GetChild(3);

        Text userHp = inventoryTransform.GetChild(1).GetComponent<Text>();
        userHp.text = combinedStats.hp.ToString();

        Text userSpeed = inventoryTransform.GetChild(3).GetComponent<Text>();
        userSpeed.text = combinedStats.speed.ToString();

        Text userPower = inventoryTransform.GetChild(5).GetComponent<Text>();
        userPower.text = combinedStats.power.ToString();

        Text userDefense = inventoryTransform.GetChild(7).GetComponent<Text>();
        userDefense.text = combinedStats.defense.ToString();

        Text userCritical = inventoryTransform.GetChild(9).GetComponent<Text>();
        userCritical.text = combinedStats.critical.ToString();
    }
}