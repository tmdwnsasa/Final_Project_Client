using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Handlers : MonoBehaviour
{
    public static Handlers instance;
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
        EXIT = 20,
        OPEN_STORE = 29,
        PURCHASE_CHARACTER = 30,
        OPEN_MAP = 40,
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
        GameManager.instance.mapBtn.SetActive(false);
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
                mapImage.color = new Color(255/255f, 78/255f, 64/255f);
            }
            if (map.ownedBy == "red")
            {
                mapImage.color = new Color(64/255f, 141/255f, 255/255f);

            }
            if (map.ownedBy == "blue")
            {
                mapImage.color = new Color(79/255f, 233/255f, 72/255f);

            }
        }
    }

    public void ReturnLobbySetting()
    {
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
}