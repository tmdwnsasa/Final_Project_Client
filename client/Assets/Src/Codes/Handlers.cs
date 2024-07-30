using System;
using System.Text;
using UnityEngine;

public class Handlers : MonoBehaviour
{
    public static Handlers instance;
    public enum HandlerIds
    {
        Login = 0,
        Register = 1,
        LocationUpdate = 2,
        CreateGame = 4,
        JoinGame = 5,
        JoinLobby = 6,
        CharacterChoice = 7,
        CharacterSelect = 8,
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