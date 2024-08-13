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
        {
            NetworkManager.instance.SendLoginPacket(id, password);
            GameManager.instance.loginUI.transform.GetChild(3).GetComponent<Button>().interactable = false;
        }

    }

    // 계정 생성 버튼
    public void OnRegisterButtonClicked()
    {
        string id = GameManager.instance.registerUI.transform.GetChild(2).GetChild(0).GetComponent<InputField>().text;
        string password = GameManager.instance.registerUI.transform.GetChild(2).GetChild(1).GetComponent<InputField>().text;
        string name = GameManager.instance.registerUI.transform.GetChild(2).GetChild(2).GetComponent<InputField>().text;

        if (id != "" && password != "" && name != "")
        {
            NetworkManager.instance.SendRegisterPacket(id, password, name);
            GameManager.instance.registerUI.transform.GetChild(3).GetComponent<Button>().interactable = false;
        }
    }

    // 케릭터 선택 버튼
    public void OnCharacterChoiceButtonClicked()
    {
        uint characterId = GameManager.instance.characterId;
        Debug.Log(characterId);

        NetworkManager.instance.SendCharacterEarnPacket(characterId);
        NetworkManager.instance.SendJoinLobbyPacket(characterId);
        GameManager.instance.characterChoiceUI.transform.GetChild(1).GetComponent<Button>().interactable = false;
    }

    // 케릭터 고르기 버튼
    public void OnCharacterSelectButtonClicked()
    {
        uint characterId = GameManager.instance.characterId;

        NetworkManager.instance.SendJoinLobbyPacket(characterId);
        GameManager.instance.characterSelectUI.transform.GetChild(1).GetComponent<Button>().interactable = false;
    }

    // 매칭/대결 버튼
    public void OnMatchGameButtonClicked()
    {
        string sessionId = GameManager.instance.sessionId;

        NetworkManager.instance.SendMatchPacket(sessionId);
        GameManager.instance.storeBtn.GetComponent<Button>().interactable = false; ;
        GameManager.instance.matchStartUI.transform.GetChild(0).GetComponent<Button>().interactable = false;
    }

    // 로비 복귀 버튼
    public void OnReturnLobbyButtonClicked()
    {
        NetworkManager.instance.SendReturnLobbyPacket();
        GameManager.instance.ReturnLobby();
        GameManager.instance.gameEndUI.transform.GetChild(3).GetComponent<Button>().interactable = false;
    }

    //게임 종료 버튼
    public void OnExitButtonClicked()
    {
        NetworkManager.instance.SendExitPacket();
        GameManager.instance.exitBtn.transform.GetChild(0).GetComponent<Button>().interactable = false;
    }

    //상점 버튼
    public void OnStoreButtonClicked()
    {
        NetworkManager.instance.SendStoreOpenPacket();
        GameManager.instance.storeBtn.GetComponent<Button>().interactable = false;
        GameManager.instance.storeUI.transform.GetChild(2).GetComponent<Button>().interactable = false;
    }

    //상점 캐릭터 목록 버튼
    public void OnCharacterStoreButtonClicked()
    {
        GameManager.instance.storeUI.transform.GetChild(0).gameObject.SetActive(true);
        GameManager.instance.storeUI.transform.GetChild(1).gameObject.SetActive(false);
        GameManager.instance.storeUI.transform.GetChild(2).GetComponent<Button>().interactable = false;
        GameManager.instance.storeUI.transform.GetChild(3).GetComponent<Button>().interactable = true;
    }

    //상점 장비 목록 버튼
    public void OnEquipmentStoreButtonClicked()
    {
        GameManager.instance.storeUI.transform.GetChild(0).gameObject.SetActive(false);
        GameManager.instance.storeUI.transform.GetChild(1).gameObject.SetActive(true);
        GameManager.instance.storeUI.transform.GetChild(2).GetComponent<Button>().interactable = true;
        GameManager.instance.storeUI.transform.GetChild(3).GetComponent<Button>().interactable = false;
    }

    //상점 캐릭터 선택 버튼
    public void OnSelectCharacterButtonClicked()
    {
        uint characterId = 0;
        Transform group = GameManager.instance.storeUI.transform.GetChild(0);
        int count = group.childCount;

        for (int i = 0; i < count; i++)
        {
            Transform btn = group.GetChild(i);
            uint index = (uint)i; // 로컬 변수로 캡처

            Button buttonComponent = btn.GetComponent<Button>();

            buttonComponent.onClick.RemoveAllListeners();

            buttonComponent.onClick.AddListener(() =>
            {
                characterId = index;
                GameManager.instance.storeUI.SetActive(false);
                GameManager.instance.purchaseCheckUI.SetActive(true);
                GameManager.instance.PurchaseCharacter(characterId);
            });
        }
    }

    //상점 장비 선택 버튼
    public void OnSelectEquipmentButtonClicked()
    { }

    //상점 캐릭터 구입 버튼
    public void OnPurchaseCharacterButtonClicked()
    {
        Text name = GameManager.instance.purchaseCheckUI.transform.GetChild(3).GetComponent<Text>();
        Text price = GameManager.instance.purchaseCheckUI.transform.GetChild(4).GetComponent<Text>();
        NetworkManager.instance.SendPurchaseCharacterPacket(name.text, price.text);
        GameManager.instance.purchaseCheckUI.transform.GetChild(0).GetComponent<Button>().interactable = false;
    }

    //상점 장비 구입 버튼
    public void OnPurchaseEquipmentButtonClicked()
    {
        // GameManager.instance.characterSelectUI.transform.GetChild(1).GetComponent<Button>().interactable = false;
    }

    //상점 구매 취소 버튼
    public void OnPurchaseCancelClicked()
    {
        GameManager.instance.purchaseCheckUI.SetActive(false);
        GameManager.instance.storeUI.SetActive(true);
    }

    //상점 나가기 버튼
    public void OnExitStoreButtonClicked()
    {
        GameManager.instance.chattingUI.SetActive(true);
        GameManager.instance.exitBtn.SetActive(true);
        GameManager.instance.matchStartUI.SetActive(true);
        GameManager.instance.storeBtn.SetActive(true);
        GameManager.instance.storeUI.SetActive(false);
        GameManager.instance.storeBtn.GetComponent<Button>().interactable = true;
    }
}
