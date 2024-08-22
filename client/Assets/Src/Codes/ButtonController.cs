using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);

    }

    // 계정 생성 버튼
    public void OnRegisterButtonClicked()
    {
        string id = GameManager.instance.registerUI.transform.GetChild(2).GetChild(0).GetComponent<InputField>().text;
        string password = GameManager.instance.registerUI.transform.GetChild(2).GetChild(1).GetComponent<InputField>().text;
        string name = GameManager.instance.registerUI.transform.GetChild(2).GetChild(2).GetComponent<InputField>().text;
        int guild = GameManager.instance.guild;

        if (id != "" && password != "" && name != "")
        {
            NetworkManager.instance.SendRegisterPacket(id, password, name, guild);
            GameManager.instance.registerUI.transform.GetChild(3).GetComponent<Button>().interactable = false;
        }
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    // 케릭터 선택 버튼
    public void OnCharacterChoiceButtonClicked()
    {
        uint characterId = GameManager.instance.characterId;

        NetworkManager.instance.SendCharacterEarnPacket(characterId);
        NetworkManager.instance.SendJoinLobbyPacket(characterId);
        GameManager.instance.characterChoiceUI.transform.GetChild(1).GetComponent<Button>().interactable = false;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    // 케릭터 고르기 버튼
    public void OnCharacterSelectButtonClicked()
    {
        uint characterId = GameManager.instance.characterId;

        NetworkManager.instance.SendJoinLobbyPacket(characterId);
        GameManager.instance.characterSelectUI.transform.GetChild(1).GetComponent<Button>().interactable = false;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    // 매칭/대결 버튼
    public void OnMatchGameButtonClicked()
    {
        string sessionId = GameManager.instance.sessionId;

        NetworkManager.instance.SendMatchPacket(sessionId, (uint)Handlers.HandlerIds.MATCHMAKING);
        GameManager.instance.matchStartUI.transform.GetChild(0).GetComponent<Button>().interactable = false;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    // 매칭/대결 취소 버튼
    public void OnMatchCancelButtonClicked()
    {
        string sessionId = GameManager.instance.sessionId;

        NetworkManager.instance.SendMatchPacket(sessionId, (uint)Handlers.HandlerIds.MATCHINGCANCEL);
        GameManager.instance.matchCancelUI.transform.GetChild(0).GetComponent<Button>().interactable = false;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    // 로비 복귀 버튼
    public void OnReturnLobbyButtonClicked()
    {
        NetworkManager.instance.SendReturnLobbyPacket();
        GameManager.instance.ReturnLobby();
        GameManager.instance.gameEndUI.transform.GetChild(3).GetComponent<Button>().interactable = false;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    //게임 종료 버튼
    public void OnExitButtonClicked()
    {
        NetworkManager.instance.SendExitPacket();
        GameManager.instance.exitBtn.transform.GetChild(0).GetComponent<Button>().interactable = false;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    //인벤토리 버튼
    public void OnInventoryButtonClicked()
    {
        string sessionId = GameManager.instance.sessionId;

        //GameManager.instance.inventoryButton.transform.GetChild(0).GetComponent<Button>().interactable = false;
        GameManager.instance.inventoryUI.SetActive(!GameManager.instance.inventoryUI.activeSelf);
        InventoryManager.instance.ShowInventoryItems();
        InventoryManager.instance.ShowEquippedItems();

        GameManager.instance.storeBtn.SetActive(false);
        GameManager.instance.mapBtn.SetActive(false);
        GameManager.instance.matchStartUI.SetActive(false);
        GameManager.instance.exitBtn.SetActive(false);
        GameManager.instance.inventoryButton.SetActive(false);
        GameManager.instance.chattingUI.SetActive(false);
    }

    public void OnInventoryItemSlotButtonClicked()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject);
        if (EventSystem.current.currentSelectedGameObject.transform.GetComponent<InventorySlot>().item.itemName == null)
  
        {
           Debug.Log($"Empty Inventory Slot");
           return;
        }
        string itemId = EventSystem.current.currentSelectedGameObject.transform.GetComponent<InventorySlot>().item.itemId.ToString();
        NetworkManager.instance.SendEquipItemPacket(itemId);

        for (int i = 0; i < 3; i++)
        {
            //Debug.Log(GameManager.instance.inventoryUI.transform.GetChild(5).GetChild(i).GetComponent<InventorySlot>().item.itemEquipSlot);
            if (GameManager.instance.inventoryUI.transform.GetChild(5).GetChild(i).GetComponent<InventorySlot>().item.itemEquipSlot ==
                EventSystem.current.currentSelectedGameObject.transform.GetComponent<InventorySlot>().item.itemEquipSlot)
            {
                Debug.Log($"Equip Error");
                return;
            }
        }

        for (int i = 0; i < 3; i++)
        {
            if (GameManager.instance.inventoryUI.transform.GetChild(5).GetChild(i).GetComponent<InventorySlot>().item.itemName == null)
            {
                Handlers.PlayerItem item = InventoryManager.instance.inventory.Find(item => item.itemId == EventSystem.current.currentSelectedGameObject.transform.GetComponent<InventorySlot>().item.itemId);
                int num = InventoryManager.instance.inventory.RemoveAll(item => item.itemId == EventSystem.current.currentSelectedGameObject.transform.GetComponent<InventorySlot>().item.itemId);

                GameManager.instance.inventoryUI.transform.GetChild(5).GetChild(i).GetComponent<InventorySlot>().item = EventSystem.current.currentSelectedGameObject.transform.GetComponent<InventorySlot>().item;
                EventSystem.current.currentSelectedGameObject.transform.GetComponent<InventorySlot>().item = new Handlers.ItemStats();

                InventoryManager.instance.ShowEquippedItems();
                InventoryManager.instance.ShowInventoryItems();

                break;
            }
        }
    }


    public void OnEquipmentItemSlotButtonClicked()
    {
        if (EventSystem.current.currentSelectedGameObject.transform.GetComponent<InventorySlot>().item.itemName == null)
        {
            Debug.Log($"Empty Equipment Slot");
            return;
        }

        for (int i = 0; i < 10; i++)
        {
            if (GameManager.instance.inventoryUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(i).GetComponent<InventorySlot>().item.itemName == null)
            {

                Handlers.PlayerItem item = InventoryManager.instance.equipment.Find(item => item.itemId == EventSystem.current.currentSelectedGameObject.transform.GetComponent<InventorySlot>().item.itemId);
                int unequip = InventoryManager.instance.equipment.RemoveAll(item => item.itemId == EventSystem.current.currentSelectedGameObject.transform.GetComponent<InventorySlot>().item.itemId);

                string itemId = EventSystem.current.currentSelectedGameObject.transform.GetComponent<InventorySlot>().item.itemId.ToString();
                NetworkManager.instance.SendUnequipItemPacket(itemId);

                GameManager.instance.inventoryUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(i).GetComponent<InventorySlot>().item = EventSystem.current.currentSelectedGameObject.transform.GetComponent<InventorySlot>().item;
                EventSystem.current.currentSelectedGameObject.transform.GetComponent<InventorySlot>().item = new Handlers.ItemStats();

                InventoryManager.instance.ShowEquippedItems();
                InventoryManager.instance.ShowInventoryItems();

                break;
            }
        }

    }


    //인벤토리 닫기 버튼
    public void OnInventoryCloseButtonClicked()
    {
        GameManager.instance.inventoryUI.SetActive(false);
        GameManager.instance.storeBtn.SetActive(true);
        GameManager.instance.mapBtn.SetActive(true);
        GameManager.instance.matchStartUI.SetActive(true);
        GameManager.instance.exitBtn.SetActive(true);
        GameManager.instance.inventoryButton.SetActive(true);
        GameManager.instance.chattingUI.SetActive(true);
    }

    //상점 버튼
    public void OnStoreButtonClicked()
    {
        string sessionId = GameManager.instance.sessionId;
        NetworkManager.instance.SendStoreOpenPacket(sessionId);
        GameManager.instance.storeBtn.GetComponent<Button>().interactable = false;
        GameManager.instance.inventoryButton.SetActive(false);
        // GameManager.instance.storeUI.transform.GetChild(2).GetComponent<Button>().interactable = false;

        // 캐릭터 버튼 세팅
        uint characterId = 0;
        Transform characterGroup = GameManager.instance.storeUI.transform.GetChild(0);
        int characterCount = characterGroup.childCount;

        for (int i = 0; i < characterCount; i++)
        {
            Transform btn = characterGroup.GetChild(i);
            uint index = (uint)i; // 로컬 변수로 캡처

            Button buttonComponent = btn.GetComponent<Button>();

            buttonComponent.onClick.RemoveAllListeners();

            buttonComponent.onClick.AddListener(() =>
            {
                characterId = index;
                GameManager.instance.storeUI.SetActive(false);
                GameManager.instance.characterPurchaseCheckUI.SetActive(true);
                GameManager.instance.PurchaseCharacter(characterId);
            });
        }

        // 장비 버튼 세팅
        uint equipmentIndex = 0;
        Transform equipmentGroup = GameManager.instance.storeUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0);
        int equipmentCount = equipmentGroup.childCount;

        for (int i = 0; i < equipmentCount; i++)
        {
            Transform btn = equipmentGroup.GetChild(i);
            uint index = (uint)i; // 로컬 변수로 캡처

            Button buttonComponent = btn.GetComponent<Button>();

            buttonComponent.onClick.RemoveAllListeners();

            buttonComponent.onClick.AddListener(() =>
            {
                equipmentIndex = index;
                GameManager.instance.storeUI.SetActive(false);
                GameManager.instance.equipmentPurchaseCheckUI.SetActive(true);
                GameManager.instance.PurchaseEquipment(equipmentIndex);
            });
        }
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    //상점 캐릭터 목록 버튼
    public void OnCharacterStoreButtonClicked()
    {
        GameManager.instance.storeUI.transform.GetChild(0).gameObject.SetActive(true);
        GameManager.instance.storeUI.transform.GetChild(1).gameObject.SetActive(false);
        GameManager.instance.storeUI.transform.GetChild(2).GetComponent<Button>().interactable = false;
        GameManager.instance.storeUI.transform.GetChild(3).GetComponent<Button>().interactable = true;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    //상점 장비 목록 버튼
    public void OnEquipmentStoreButtonClicked()
    {
        GameManager.instance.storeUI.transform.GetChild(0).gameObject.SetActive(false);
        GameManager.instance.storeUI.transform.GetChild(1).gameObject.SetActive(true);
        GameManager.instance.storeUI.transform.GetChild(2).GetComponent<Button>().interactable = true;
        GameManager.instance.storeUI.transform.GetChild(3).GetComponent<Button>().interactable = false;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    //상점 캐릭터 구입 버튼
    public void OnPurchaseCharacterButtonClicked()
    {
        string sessionId = GameManager.instance.sessionId;
        Text name = GameManager.instance.characterPurchaseCheckUI.transform.GetChild(3).GetComponent<Text>();
        Text price = GameManager.instance.characterPurchaseCheckUI.transform.GetChild(4).GetComponent<Text>();
        NetworkManager.instance.SendPurchaseCharacterPacket(name.text, price.text, sessionId);
        GameManager.instance.characterPurchaseCheckUI.transform.GetChild(0).GetComponent<Button>().interactable = false;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    //상점 장비 구입 버튼
    public void OnPurchaseEquipmentButtonClicked()
    {
        string sessionId = GameManager.instance.sessionId;
        Text name = GameManager.instance.equipmentPurchaseCheckUI.transform.GetChild(3).GetComponent<Text>();
        Text price = GameManager.instance.equipmentPurchaseCheckUI.transform.GetChild(4).GetComponent<Text>();
        NetworkManager.instance.SendPurchaseEquipmentPacket(name.text, price.text, sessionId);
        GameManager.instance.equipmentPurchaseCheckUI.transform.GetChild(0).GetComponent<Button>().interactable = false;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    //상점 구매 취소 버튼
    public void OnPurchaseCancelClicked()
    {
        GameManager.instance.characterPurchaseCheckUI.SetActive(false);
        GameManager.instance.equipmentPurchaseCheckUI.SetActive(false);
        GameManager.instance.storeUI.SetActive(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    //상점 나가기 버튼
    public void OnExitStoreButtonClicked()
    {
        GameManager.instance.chattingUI.SetActive(true);
        GameManager.instance.exitBtn.SetActive(true);
        if(GameManager.instance.isMatchging) {
            GameManager.instance.matchCancelUI.SetActive(true);
        } else {
            GameManager.instance.matchStartUI.SetActive(true);
        }
        GameManager.instance.storeBtn.SetActive(true);
        GameManager.instance.storeUI.SetActive(false);
        GameManager.instance.mapBtn.SetActive(true);
        GameManager.instance.inventoryButton.SetActive(true);
        GameManager.instance.storeBtn.GetComponent<Button>().interactable = true;
        GameManager.instance.storeUI.transform.GetChild(0).gameObject.SetActive(true);
        GameManager.instance.storeUI.transform.GetChild(1).gameObject.SetActive(false);
        GameManager.instance.storeUI.transform.GetChild(2).GetComponent<Button>().interactable = true;
        GameManager.instance.storeUI.transform.GetChild(3).GetComponent<Button>().interactable = true;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void OnPoliceButtonClicked()
    {
        GameManager.instance.guild = 1;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void OnFarmerButtonClicked()
    {
        GameManager.instance.guild = 2;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    //진영별 점령 땅 확인 버튼
    public void OnCheckMapButtonClicked()
    {
        NetworkManager.instance.SendOpenMapPacket();
        GameManager.instance.chattingUI.SetActive(false);
        GameManager.instance.exitBtn.SetActive(false);
        GameManager.instance.matchStartUI.SetActive(false);
        GameManager.instance.matchCancelUI.SetActive(false);
        GameManager.instance.storeBtn.SetActive(false);
        GameManager.instance.mapBtn.SetActive(false);
        GameManager.instance.mapUI.SetActive(true);
        GameManager.instance.inventoryButton.SetActive(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    // 맵 나가기 버튼
    public void OnExitMapButtonClicked()
    {
        GameManager.instance.chattingUI.SetActive(true);
        GameManager.instance.exitBtn.SetActive(true);
        if(GameManager.instance.isMatchging) {
            GameManager.instance.matchCancelUI.SetActive(true);
        } else {
            GameManager.instance.matchStartUI.SetActive(true);
        }
        GameManager.instance.storeBtn.SetActive(true);
        GameManager.instance.mapBtn.SetActive(true);
        GameManager.instance.mapUI.SetActive(false);
        GameManager.instance.inventoryButton.SetActive(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);

    }
}
