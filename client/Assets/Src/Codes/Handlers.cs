using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Handlers;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using UnityEngine.UI;

public class Handlers : MonoBehaviour
{
    public static Handlers instance;

    //public InventoryData inventoryData;
    //public event Action OnInventoryDataUpdated;


    public GameObject Player;
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
        PURCHASE_EQUIPMENT = 31,
        OPEN_MAP = 40,
        SKILL = 50,
        REMOVESKILL = 51
    }

    [Serializable]
    public struct CharacterChoice
    {
        public string playerId;
        public string name;
        public int guild;
        public string sessionId;
        public List<PlayerItem> allInventoryItems;
        public List<PlayerItem> allEquippedItems;
        public List<ItemStats> itemStats;
        public int userMoney;

    }

    [Serializable]
    public struct CharacterSelect
    {
        public string playerId;
        public string name;
        public int guild;
        public string sessionId;
        public List<uint> possession;
        public List<PlayerItem> allInventoryItems;
        public List<PlayerItem> allEquippedItems;
        public List<ItemStats> itemStats;
        public int userMoney;

    }

    [Serializable]
    public struct PlayerItem
    {
        public int inventoryId;
        public string playerId;
        public int itemId;
        public string itemSpriteName;
        public int equippedItems;
        public string equipSlot;

        public bool IsEquipped => equippedItems == 1;
    }


    [Serializable]
    public struct ItemStats
    {
        public int itemId;
        public string itemName;
        public string itemEquipSlot;
        public float itemHp;
        public float itemSpeed;
        public float itemAttack;
        public int itemPrice;
    }

    [Serializable]
    public struct UserData
    {
        public string playerId;
        public uint characterId;
        public uint guild;
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

        public characterSkill zSkill;

        public characterSkill xSkill;
        public List<UserData> userDatas;
    }

    [Serializable]
    public struct characterSkill
    {
        public uint skill_id;
        public string skill_name;
        public int skill_type;
        public int character_id;
        public float damage_factor;
        public float cool_time;
        public float range_x;
        public float range_y;
        public int scale;
    }

    [Serializable]
    public struct CharacterDatas
    {
        public List<UserData> userDatas;
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
        public List<ItemStats> allItems;
        public List<ItemStats> equippedItems;
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
    public class UpdatedInventoryData
    {
        public string message;
        public CombinedStats combinedStats;
        public List<ItemStats> equippedItems;
        public List<ItemStats> allItems;

    }


    public struct MapData
    {
        public string mapName;
        public bool isDisputedArea;
        public string ownedBy;
    }

    [Serializable]
    public struct MapDataArrayWrapper
    {
        public MapData[] mapData;
    }

    public void GetCharacterChoice(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);
        CharacterChoice characterChoice = JsonUtility.FromJson<CharacterChoice>(jsonString);


        GameManager.instance.playerId = characterChoice.playerId;
        GameManager.instance.name = characterChoice.name;
        GameManager.instance.player.guild = characterChoice.guild;
        GameManager.instance.sessionId = characterChoice.sessionId;
        GameManager.instance.items = characterChoice.itemStats;
        InventoryManager.instance.inventory = characterChoice.allInventoryItems;
        InventoryManager.instance.equipment = characterChoice.allEquippedItems;


        GameManager.instance.GoCharacterChoice();
        InventoryManager.instance.InitializeItemSpriteMapping();
    }

    public void GetCharacterSelect(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);

        CharacterSelect characterSelect = JsonUtility.FromJson<CharacterSelect>(jsonString);

        GameManager.instance.playerId = characterSelect.playerId;
        GameManager.instance.name = characterSelect.name;
        GameManager.instance.player.guild = characterSelect.guild;
        GameManager.instance.sessionId = characterSelect.sessionId;
        GameManager.instance.possession = characterSelect.possession;
        InventoryManager.instance.inventory = characterSelect.allInventoryItems;
        InventoryManager.instance.equipment = characterSelect.allEquippedItems;
        GameManager.instance.items = characterSelect.itemStats;


        //for(int i = 0;  i< GameManager.instance.Items.Count; i++)
        //{
        //    GameManager.instance.Items[i]=characterSelect.itemStats[i];

        //}
        Debug.Log(GameManager.instance.items[0].itemName);

        Debug.Log($"Received {characterSelect.allEquippedItems.Count} equipped items// {InventoryManager.instance.equipment.Count}");

        GameManager.instance.GoCharacterSelect();
        InventoryManager.instance.InitializeItemSpriteMapping();


    }

    public void SetCharacterStats(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);
        CharacterStats characterStats = JsonUtility.FromJson<CharacterStats>(jsonString);

        Debug.Log("characterZSkill : " + characterStats.zSkill.skill_name);

        GameManager.instance.player.characterId = characterStats.characterId;
        GameManager.instance.player.characterName = characterStats.characterName;
        GameManager.instance.player.hp = characterStats.hp;
        GameManager.instance.player.speed = characterStats.speed;
        GameManager.instance.player.power = characterStats.power;
        GameManager.instance.player.defense = characterStats.defense;
        GameManager.instance.player.critical = characterStats.critical;

        GameManager.instance.player.zSkill = characterStats.zSkill.skill_name;
        GameManager.instance.player.xSkill = characterStats.xSkill.skill_name;
        GameManager.instance.player.zSkill_id = characterStats.zSkill.skill_id;
        GameManager.instance.player.xSkill_id = characterStats.xSkill.skill_id;
        GameManager.instance.player.zSkill_CoolTime = characterStats.zSkill.cool_time;
        GameManager.instance.player.xSkill_CoolTime = characterStats.xSkill.cool_time;
        //�ٸ� �÷��̾� ���� ����
        foreach (var user in characterStats.userDatas)
        {
            CharacterManager.instance.CreateOtherPlayers(user.playerId, user.characterId, user.guild);
        }
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
        GameManager.instance.characterPurchaseCheckUI.transform.GetChild(0).GetComponent<Button>().interactable = true;
        GameManager.instance.equipmentPurchaseCheckUI.transform.GetChild(0).GetComponent<Button>().interactable = true;
        GameManager.instance.mapBtn.SetActive(false);
    }

    public void PurchaseMessage(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);
        PurchaseStateMessage purchaseStateMessage = JsonUtility.FromJson<PurchaseStateMessage>(jsonString);
        Text message = GameManager.instance.purchaseMessageUI.transform.GetChild(1).GetComponent<Text>();
        message.text = purchaseStateMessage.message;
        GameManager.instance.characterPurchaseCheckUI.SetActive(false);
        GameManager.instance.equipmentPurchaseCheckUI.SetActive(false);
        GameManager.instance.purchaseMessageUI.SetActive(true);
    }

    public void OpenMap(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);
        MapDataArrayWrapper mapDataArray = JsonUtility.FromJson<MapDataArrayWrapper>(jsonString);
        for (int i = 0; i < mapDataArray.mapData.Length; i++)
        {
            MapData map = mapDataArray.mapData[i];
            Image mapImage = GameManager.instance.mapUI.transform.GetChild(2).GetChild(i).GetComponent<Image>();
            if (map.isDisputedArea == true)
            {
                mapImage.color = new Color(255 / 255f, 78 / 255f, 64 / 255f);
            }
            if (map.ownedBy == "red")
            {
                mapImage.color = new Color(64 / 255f, 141 / 255f, 255 / 255f);

            }
            if (map.ownedBy == "blue")
            {
                mapImage.color = new Color(79 / 255f, 233 / 255f, 72 / 255f);

            }
        }
    }

    public void ReturnLobbySetting(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);
        CharacterDatas characterDatas = JsonUtility.FromJson<CharacterDatas>(jsonString);
        foreach (var user in characterDatas.userDatas)
        {
            CharacterManager.instance.CreateOtherPlayers(user.playerId, user.characterId, user.guild);
        }

        GameManager.instance.isLive = true;
        GameManager.instance.player.ResetAnimation();
        // GameManager.instance.player.transform.position = new Vector2(0, 0);
        NetworkManager.instance.isLobby = true;

        GameManager.instance.matchStartUI.SetActive(true);
        GameManager.instance.exitBtn.SetActive(true);
        GameManager.instance.mapBtn.SetActive(true);
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

    //public void SetInventoryItemData(byte[] data)
    //{
    //    string jsonString = Encoding.UTF8.GetString(data);

    //    Handlers.instance.inventoryData = JsonUtility.FromJson<InventoryData>(jsonString);
    //    Debug.Log($"Received InventoryData with {inventoryData.allItems.Count} items and {inventoryData.equippedItems.Count} equipped items.");



    //    if (Handlers.instance.inventoryData.allItems == null)
    //        {
    //        Handlers.instance.inventoryData.allItems = new List<Item>();
    //        }

    //        if (Handlers.instance.inventoryData.equippedItems == null)
    //        {
    //        Handlers.instance.inventoryData.equippedItems = new List<Item>();
    //        }

    //    Debug.Log("Invoking OnInventoryDataUpdated event.");

    //    OnInventoryDataUpdated?.Invoke();
    //}


    //public void UpdateEquipItem(byte[] data)
    //{
    //    UpdateInventoryAndStats(data);
    //}

    //public void UpdateUnequipItem(byte[] data)
    //{
    //    UpdateInventoryAndStats(data);
    //}

    //private void UpdateInventoryAndStats(byte[] data)
    //{
    //    // Deserialize the response data into UpdatedStats
    //    string jsonString = Encoding.UTF8.GetString(data);
    //    UpdatedInventoryData updatedInventoryData = JsonUtility.FromJson<UpdatedInventoryData>(jsonString);

    //    // Update the inventory data with the new items and stats
    //    inventoryData.combinedStats = updatedInventoryData.combinedStats;
    //    inventoryData.allItems = updatedInventoryData.allItems;
    //    inventoryData.equippedItems = updatedInventoryData.equippedItems;

    //    UpdateStatsUI(inventoryData.combinedStats);


    //    OnInventoryDataUpdated?.Invoke();
    }
//    public void UpdateStatsUI(CombinedStats combinedStats)
//    {
//        Transform inventoryTransform = GameManager.instance.inventoryUI.transform.GetChild(3);

//        Text userHp = inventoryTransform.GetChild(1).GetComponent<Text>();
//        userHp.text = combinedStats.hp.ToString();

//        Text userSpeed = inventoryTransform.GetChild(3).GetComponent<Text>();
//        userSpeed.text = combinedStats.speed.ToString();

//        Text userPower = inventoryTransform.GetChild(5).GetComponent<Text>();
//        userPower.text = combinedStats.power.ToString();

//        Text userDefense = inventoryTransform.GetChild(7).GetComponent<Text>();
//        userDefense.text = combinedStats.defense.ToString();

//        Text userCritical = inventoryTransform.GetChild(9).GetComponent<Text>();
//        userCritical.text = combinedStats.critical.ToString();
//    }
//}