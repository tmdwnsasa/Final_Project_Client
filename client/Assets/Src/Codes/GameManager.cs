using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using static Handlers;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Sprite[] itemSprites; 
    public Dictionary<int, Sprite> itemSpriteMapping; 
    public List<ItemStats> equipmentStore;


    [Header("# Images")]
    public List<Sprite> farmers;

    [Header("# Game Control")]
    public bool isLive;
    public float gameTime;
    public int targetFrameRate;
    public string version = "1.0.0";
    public bool isMatchging;

    [Header("# Register")]
    public int guild = 0;

    [Header("# Player Info")]
    public uint characterId;
    public string sessionId;
    public string playerId;
    public string password;
    public string name;
    public List<uint> possession;

    [Header("# Item")]
    public List<ItemStats> items;
   
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
    public GameObject matchCancelUI;
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
        GetItemSpriteMapping();
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
        matchCancelUI.SetActive(false);
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
        isMatchging = false;

        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }


    public void InitializeItemSpriteMapping()
    {
        if (itemSpriteMapping == null)
        {
            itemSpriteMapping = new Dictionary<int, Sprite>();

            // Map item IDs to their corresponding sprites
            for (int i = 0; i < itemSprites.Length; i++)
            {
                itemSpriteMapping.Add(i + 1, itemSprites[i]);
            }

            Debug.Log($"itemSpriteMapping initialized with {itemSpriteMapping.Count} entries.");
        }
    }


    public Dictionary<int, Sprite> GetItemSpriteMapping()
    {
        if (itemSpriteMapping == null)
        {
            InitializeItemSpriteMapping();
        }
        return itemSpriteMapping;
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
        //gameTime += Time.deltaTime;
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
        // Debug.Log(equipmentIndex);
        Text equipmentName = storeUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild((int)equipmentIndex).GetChild(2).GetComponent<Text>();
        Text purchaseName = equipmentPurchaseCheckUI.transform.GetChild(3).GetComponent<Text>();
        purchaseName.text = equipmentName.text;
    }

}

