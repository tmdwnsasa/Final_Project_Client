using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Handlers : MonoBehaviour
{
    public static Handlers instance;

    //public InventoryData inventoryData;
    //public event Action OnInventoryDataUpdated;

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
        MATCHINGCANCEL = 12,
        RESELECTCHARACTER = 13,
        GAME_END = 15,
        RETURN_LOBBY = 16,
        INVENTORY = 17,
        EQUIP_ITEM = 18,
        UNEQUIP_ITEM = 19,
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
        public List<ItemStats> allItems;
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
        public List<ItemStats> allItems;
        public int userMoney;

    }

    [Serializable]
    public struct PlayerItem
    {
        public int inventoryId;
        public string playerId;
        public int itemId;
        // public string itemSpriteName;
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
        public CombinedStats updatedStats;
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
        public List<PlayerItem> allInventoryItems;
        public List<PlayerItem> allEquippedItems;
        public int remainMoney;
        public string message;
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
    public struct UpdatedInventoryData
    {
        public CombinedStats updatedStats;
        public List<PlayerItem> allInventoryItems;
        public List<PlayerItem> allEquippedItems;
        public int money;
        public string message;
    }
    
    [Serializable]
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
        GameManager.instance.items = characterChoice.allItems;
        InventoryManager.instance.inventory = characterChoice.allInventoryItems;
        InventoryManager.instance.equipment = characterChoice.allEquippedItems;
        InventoryManager.instance.money = characterChoice.userMoney;

        GameManager.instance.equipmentStore = characterChoice.allItems;
        
        Debug.Log(GameManager.instance.equipmentStore.Count);

        for (int i = 0; i < GameManager.instance.equipmentStore.Count; i++)
        {

            //GameManager.instance.storeUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Text>().text = GameManager.instance.equipmentStore[i].item;
            GameManager.instance.storeUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Text>().text = GameManager.instance.equipmentStore[i].itemName;
            GameManager.instance.storeUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(i).GetChild(3).GetComponent<Text>().text = GameManager.instance.equipmentStore[i].itemPrice.ToString();

        }

        InventoryManager.instance.ShowInventoryItems();
        InventoryManager.instance.ShowEquippedItems();
        InventoryManager.instance.UpdateInventoryCombinedStats();
        GameManager.instance.GoCharacterChoice();
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
        GameManager.instance.items = characterSelect.allItems;
        InventoryManager.instance.money = characterSelect.userMoney;

        GameManager.instance.equipmentStore = characterSelect.allItems;

        for (int i = 0; i < GameManager.instance.equipmentStore.Count; i++)
        {

            //GameManager.instance.storeUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(i).GetChild(1).GetComponent<Text>().text = GameManager.instance.equipmentStore[i].item;
            GameManager.instance.storeUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(i).GetChild(2).GetComponent<Text>().text = GameManager.instance.equipmentStore[i].itemName;
            GameManager.instance.storeUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(i).GetChild(3).GetComponent<Text>().text = GameManager.instance.equipmentStore[i].itemPrice.ToString();

        }

        InventoryManager.instance.ShowInventoryItems();
        InventoryManager.instance.ShowEquippedItems();
        InventoryManager.instance.UpdateInventoryCombinedStats();
        GameManager.instance.GoCharacterSelect();
    }

    public void SetCharacterStats(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);
        CharacterStats characterStats = JsonUtility.FromJson<CharacterStats>(jsonString);

        GameManager.instance.player.characterId = characterStats.characterId;
        GameManager.instance.player.characterName = characterStats.characterName;
        GameManager.instance.player.hp = characterStats.updatedStats.hp;
        GameManager.instance.player.speed = characterStats.updatedStats.speed;
        GameManager.instance.player.power = characterStats.updatedStats.power;
        GameManager.instance.player.defense = characterStats.updatedStats.defense;
        GameManager.instance.player.critical = characterStats.updatedStats.critical;

        GameManager.instance.player.zSkill = characterStats.zSkill.skill_name;
        GameManager.instance.player.xSkill = characterStats.xSkill.skill_name;
        GameManager.instance.player.zSkill_id = characterStats.zSkill.skill_id;
        GameManager.instance.player.xSkill_id = characterStats.xSkill.skill_id;
        GameManager.instance.player.zSkill_CoolTime = characterStats.zSkill.cool_time;
        GameManager.instance.player.xSkill_CoolTime = characterStats.xSkill.cool_time;

        InventoryManager.instance.UpdateInventoryCombinedStats();

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
        GameManager.instance.matchCancelUI.SetActive(false);
        GameManager.instance.storeBtn.SetActive(false);
        GameManager.instance.storeUI.SetActive(true);
        GameManager.instance.purchaseMessageUI.SetActive(false);
        GameManager.instance.reselectCharacterBtn.SetActive(false);
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

        if (purchaseStateMessage.remainMoney == 0)
            return;

        // InventoryManager.instance.inventory = purchaseStateMessage.allInventoryItems;
        // InventoryManager.instance.equipment = purchaseStateMessage.allEquippedItems;
        // InventoryManager.instance.money = purchaseStateMessage.remainMoney;

        InventoryManager.instance.ShowEquippedItems();
        InventoryManager.instance.ShowInventoryItems();
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
            if (map.ownedBy == "blue")
            {
                mapImage.color = new Color(64 / 255f, 141 / 255f, 255 / 255f);

            }
            if (map.ownedBy == "green")
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
        GameManager.instance.reselectCharacterBtn.transform.GetComponent<Button>().interactable = true;
        GameManager.instance.inventoryButton.SetActive(true);
        GameManager.instance.exitBtn.SetActive(true);
        GameManager.instance.mapBtn.SetActive(true);
        GameManager.instance.reselectCharacterBtn.SetActive(true);
        GameManager.instance.player.hpSlider.gameObject.SetActive(false);
        GameManager.instance.gameEndUI.transform.GetChild(3).GetComponent<Button>().interactable = true;

        GameManager.instance.player.isZSkill = true;
        GameManager.instance.player.isXSkill = true;

        GameManager.instance.isMatching = false;
    }

 


    public void UpdateInventoryAndStats(byte[] data)
    {
        // Deserialize the response data into UpdatedStats
        string jsonString = Encoding.UTF8.GetString(data);
        UpdatedInventoryData updatedInventoryData = JsonUtility.FromJson<UpdatedInventoryData>(jsonString);

        if(updatedInventoryData.updatedStats.hp == 0)
        {
            return;
        }

        // Update the inventory data with the new items and stats
        GameManager.instance.player.GetComponent<Player>().SetStats(updatedInventoryData.updatedStats);

        InventoryManager.instance.inventory = updatedInventoryData.allInventoryItems;
        InventoryManager.instance.equipment = updatedInventoryData.allEquippedItems;

        if(updatedInventoryData.money != 0)
        {
            InventoryManager.instance.money = updatedInventoryData.money;
        }

        InventoryManager.instance.UpdateInventoryCombinedStats();
        InventoryManager.instance.ShowInventoryItems();
        InventoryManager.instance.ShowEquippedItems();
    }
}