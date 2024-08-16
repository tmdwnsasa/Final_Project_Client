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
        public int guild;
        public string sessionId;
    }

    [Serializable]
    public struct CharacterSelect
    {
        public string playerId;
        public string name;
        public int guild;
        public string sessionId;
        public List<uint> possession;
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
        public List<UserData> userDatas;
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

    public void GetCharacterChoice(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);
        CharacterChoice characterChoice = JsonUtility.FromJson<CharacterChoice>(jsonString);

        GameManager.instance.playerId = characterChoice.playerId;
        GameManager.instance.name = characterChoice.name;
        GameManager.instance.player.guild = characterChoice.guild;
        GameManager.instance.sessionId = characterChoice.sessionId;

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
        GameManager.instance.player.hpSlider.gameObject.SetActive(false);
        GameManager.instance.gameEndUI.transform.GetChild(3).GetComponent<Button>().interactable = true;
    }
}