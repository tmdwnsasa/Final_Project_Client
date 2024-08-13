using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    //로그인 버튼
    public void OnLoginButtonClicked()
    {
        string id = GameManager.instance.loginUI.transform.GetChild(2).GetChild(0).GetComponent<InputField>().text;
        string password = GameManager.instance.loginUI.transform.GetChild(2).GetChild(1).GetComponent<InputField>().text;

        if (id != "" && password != "" && name != "")
            NetworkManager.instance.SendLoginPacket(id, password);
    }

    // 계정 생성 버튼
    public void OnRegisterButtonClicked()
    {
        string id = GameManager.instance.registerUI.transform.GetChild(2).GetChild(0).GetComponent<InputField>().text;
        string password = GameManager.instance.registerUI.transform.GetChild(2).GetChild(1).GetComponent<InputField>().text;
        string name = GameManager.instance.registerUI.transform.GetChild(2).GetChild(2).GetComponent<InputField>().text;

        if (id != "" && password != "" && name != "")
            NetworkManager.instance.SendRegisterPacket(id, password, name);
    }

    // 케릭터 선택 버튼
    public void OnCharacterChoiceButtonClicked()
    {
        uint characterId = GameManager.instance.characterId;

        NetworkManager.instance.SendCharacterEarnPacket(characterId);
        NetworkManager.instance.SendJoinLobbyPacket(characterId);
    }

    // 케릭터 고르기 버튼
    public void OnCharacterSelectButtonClicked()
    {
        uint characterId = GameManager.instance.characterId;

        NetworkManager.instance.SendJoinLobbyPacket(characterId);
    }

    // 매칭/대결 버튼
    public void OnMatchGameButtonClicked()
    {
        string sessionId = GameManager.instance.sessionId;

        NetworkManager.instance.SendMatchPacket(sessionId);
    }

    public void OnReturnLobbyButtonClicked()
    {
        NetworkManager.instance.SendReturnLobbyPacket();
        GameManager.instance.ReturnLobby();
    }

    public void OnExitButtonClicked()
    {
        NetworkManager.instance.SendExitPacket();
    }

    public void OnPoliceButtonClicked()
    {
        GameManager.instance.isPolice = 1;
    }
    public void OnFarmerButtonClicked()
    {
        GameManager.instance.isPolice = 2;
    }
}
