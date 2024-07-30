using System;
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
        CHARACTER_CHOICE = 7,
        CHARACTER_SELECT = 8,
        CHATTING = 10,
    }

    [Serializable]
    public struct CharacterChoice
    {
        public string playerId;

        public string sessionId;
    }

    public void GetCharacterChoice(byte[] data)
    {
        string jsonString = Encoding.UTF8.GetString(data);
        CharacterChoice characterChoice = JsonUtility.FromJson<CharacterChoice>(jsonString);

        GameManager.instance.sessionId = characterChoice.sessionId;
        GameManager.instance.playerId = characterChoice.playerId;

        Debug.Log(GameManager.instance.sessionId);
        Debug.Log(GameManager.instance.playerId);
    }

    public void GetCharacterSelect(byte[] data)
    {

    }
}