using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

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
        SKILL = 50
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
}