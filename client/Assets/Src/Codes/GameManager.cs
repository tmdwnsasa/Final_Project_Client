using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# Images")]
    public List<Sprite> farmers;

    [Header("# Game Control")]
    public bool isLive;
    public float gameTime;
    public int targetFrameRate;
    public string version = "1.0.0";

    [Header("# Register")]
    public int guild = 0;

    [Header("# Player Info")]
    public uint characterId;
    public string sessionId;
    public string playerId;
    public string password;
    public string name;
    public List<uint> possession;

    [Header("# Game Object")]
    public PoolManager pool;
    public Player player;
    public Chatting chatting;
    // public GameObject hud;
    public GameObject loginUI;
    public GameObject registerUI;
    public GameObject chattingUI;
    public GameObject characterChoiceUI;
    public GameObject characterSelectUI;
    public GameObject gameEndUI;
    public GameObject matchStartUI;
    public GameObject exitBtn;
    public GameObject inventoryButton;
    public GameObject inventoryUI;
    public GameObject equipUnequipItemMessageUI;
    public GameObject storeBtn;
    public GameObject storeUI;
    public GameObject characterPurchaseCheckUI;
    public GameObject equipmentPurchaseCheckUI;
    public GameObject purchaseMessageUI;
    public GameObject mapBtn;
    public GameObject mapUI;
    public GameObject AnnouncementMap;


    void Awake()
    {
        instance = this;
        Application.targetFrameRate = targetFrameRate;
        SetBtn();
    }

    public void GameStart()
    {
        player.gameObject.transform.position = new Vector2(0, 0);
        player.playerId = playerId;
        player.name = name;
        player.gameObject.SetActive(true);
        // hud.SetActive(true);
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        characterChoiceUI.SetActive(false);
        characterSelectUI.SetActive(false);
        chattingUI.SetActive(true);
        matchStartUI.SetActive(true);
        exitBtn.SetActive(true);
        inventoryButton.SetActive(true);
        inventoryUI.SetActive(false);
        equipUnequipItemMessageUI.SetActive(false);
        storeBtn.SetActive(true);
        storeUI.SetActive(false);
        characterPurchaseCheckUI.SetActive(false);
        equipmentPurchaseCheckUI.SetActive(false);
        purchaseMessageUI.SetActive(false);
        mapBtn.SetActive(true);
        mapUI.SetActive(false);
        AnnouncementMap.SetActive(false);

        isLive = true;

        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GoRegister()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(true);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GoLogin()
    {
        registerUI.SetActive(false);
        loginUI.SetActive(true);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GoCharacterChoice()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        characterChoiceUI.SetActive(true);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GoCharacterSelect()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        characterSelectUI.SetActive(true);
        for (int i = 0; i < possession.Count; i++)
        {
            characterSelectUI.transform.GetChild(0).GetChild((int)possession[i]).gameObject.SetActive(true);
        }

        CharacterChange(possession[0]);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void CharacterChange(uint index)
    {
        characterId = index;

        if (characterChoiceUI.gameObject.activeSelf)
        {
            Transform ChoiceImage = characterChoiceUI.transform.GetChild(2);
            ChoiceImage.GetComponent<Image>().sprite = farmers[(int)index];
        }
        if (characterSelectUI.gameObject.activeSelf)
        {
            Transform SelectImage = characterSelectUI.transform.GetChild(2);
            SelectImage.GetComponent<Image>().sprite = farmers[(int)index];
        }
    }

    public void SetBtn()
    {
        {
            Transform group = characterChoiceUI.transform.GetChild(0);
            int count = group.childCount;
            for (uint i = 0; i < count; i++)
            {
                Transform btn = group.GetChild((int)i);
                uint index = i;
                btn.GetComponent<Button>().onClick.AddListener(() => CharacterChange(index));
            }
        }

        {
            Transform group = characterSelectUI.transform.GetChild(0);
            int count = group.childCount;
            for (uint i = 0; i < count; i++)
            {
                Transform btn = group.GetChild((int)i);
                uint index = i;
                btn.GetComponent<Button>().onClick.AddListener(() => CharacterChange(index));
            }
        }
    }

    public void GameOver()
    {
        isLive = false;
        // StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;
        yield return new WaitForSeconds(0.5f);

        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    void Update()
    {
        if (!isLive)
        {
            return;
        }
        gameTime += Time.deltaTime;
    }
    private Image victory;
    private Image defeat;
    public void GameEnd(string result, List<GameEndPayload.UserState> users)
    {
        isLive = false;

        victory = gameEndUI.transform.GetChild(0).GetComponent<Image>();
        defeat = gameEndUI.transform.GetChild(1).GetComponent<Image>();
        for (int i = 0; i < users.Count; i++)
        {
            Text userName = gameEndUI.transform.GetChild(2).GetChild(i * 4 + 4).GetComponent<Text>();
            Text userKill = gameEndUI.transform.GetChild(2).GetChild(i * 4 + 5).GetComponent<Text>();
            Text userDeath = gameEndUI.transform.GetChild(2).GetChild(i * 4 + 6).GetComponent<Text>();
            Text userDamage = gameEndUI.transform.GetChild(2).GetChild(i * 4 + 7).GetComponent<Text>();
            userName.text = users[i].name;
            userKill.text = users[i].kill.ToString();
            userDeath.text = users[i].death.ToString();
            userDamage.text = users[i].damage.ToString();
        }
        // Text user1Name = GameEndUI.transform.GetChild(2).GetChild(4).GetComponent<Text>();
        // Text user1Kill = GameEndUI.transform.GetChild(2).GetChild(5).GetComponent<Text>();
        // Text user1Death = GameEndUI.transform.GetChild(2).GetChild(6).GetComponent<Text>();
        // Text user1Damage = GameEndUI.transform.GetChild(2).GetChild(7).GetComponent<Text>();
        // Text user2Name = GameEndUI.transform.GetChild(2).GetChild(8).GetComponent<Text>();
        // Text user2kill = GameEndUI.transform.GetChild(2).GetChild(9).GetComponent<Text>();
        // Text user2Damage = GameEndUI.transform.GetChild(2).GetChild(10).GetComponent<Text>();
        // Text user2Death = GameEndUI.transform.GetChild(2).GetChild(11).GetComponent<Text>();
        // Text user3Name = GameEndUI.transform.GetChild(2).GetChild(12).GetComponent<Text>();
        // Text user3Kill = GameEndUI.transform.GetChild(2).GetChild(13).GetComponent<Text>();
        // Text user3Death = GameEndUI.transform.GetChild(2).GetChild(14).GetComponent<Text>();
        // Text user3Damage = GameEndUI.transform.GetChild(2).GetChild(15).GetComponent<Text>();
        // Text user4Name = GameEndUI.transform.GetChild(2).GetChild(16).GetComponent<Text>();
        // Text user4Kill = GameEndUI.transform.GetChild(2).GetChild(17).GetComponent<Text>();
        // Text user4Death = GameEndUI.transform.GetChild(2).GetChild(18).GetComponent<Text>();
        // Text user4Damage = GameEndUI.transform.GetChild(2).GetChild(19).GetComponent<Text>();

        GameManager.instance.AnnouncementMap.SetActive(false);
        gameEndUI.SetActive(true);
        if (result == "Win")
        {
            victory.gameObject.SetActive(true);
            defeat.gameObject.SetActive(false);
        }
        else if (result == "Lose")
        {
            victory.gameObject.SetActive(false);
            defeat.gameObject.SetActive(true);
        }
        // user1Name.text = users[0].name;
        // user1Kill.text = users[0].kill.ToString();
        // user1Death.text = users[0].death.ToString();
        // user1Damage.text = users[0].damage.ToString();
        // user2Name.text = users[1].name;
        // user2kill.text = users[1].kill.ToString(); 
        // user2Death.text = users[1].death.ToString(); 
        // user2Damage.text = users[1].damage.ToString(); 
        // user3Name.text = users[2].name;
        // user3Kill.text = users[2].kill.ToString(); 
        // user3Death.text = users[2].death.ToString(); 
        // user3Damage.text = users[2].damage.ToString(); 
        // user4Name.text = users[3].name;
        // user4Kill.text = users[3].kill.ToString(); 
        // user4Death.text = users[3].death.ToString(); 
        // user4Damage.text = users[3].damage.ToString(); 
    }

    public void ReturnLobby()
    {
        gameEndUI.SetActive(false);
        storeBtn.SetActive(true);
        storeBtn.GetComponent<Button>().interactable = true;
    }

    public void PurchaseCharacter(uint characterId){
        // Debug.Log(characterId);
        Text characterName = storeUI.transform.GetChild(0).GetChild((int)characterId).GetChild(1).GetComponent<Text>();
        Text purchaseName = characterPurchaseCheckUI.transform.GetChild(3).GetComponent<Text>();
        purchaseName.text = characterName.text;
    }

    public void PurchaseEquipment(uint equipmentIndex){
        Debug.Log(equipmentIndex);
        Text equipmentName = storeUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild((int)equipmentIndex).GetChild(2).GetComponent<Text>();
        Text purchaseName = equipmentPurchaseCheckUI.transform.GetChild(3).GetComponent<Text>();
        purchaseName.text = equipmentName.text;
    }
}
