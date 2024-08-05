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
    public int latency = 2;

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
    public GameObject hud;
    public GameObject LoginUI;
    public GameObject RegisterUI;
    public GameObject ChattingUI;
    public GameObject CharacterChoiceUI;
    public GameObject CharacterSelectUI;
    public GameObject GameEndUI;
    public GameObject MatchStartUI;




    void Awake() {
        instance = this;
        Application.targetFrameRate = targetFrameRate;
        SetBtn();
    }

    public void GameStart() {
        player.gameObject.transform.position = new Vector2(0, 0);
        player.playerId = playerId;
        player.name = name;
        player.gameObject.SetActive(true);
        hud.SetActive(true);
        LoginUI.SetActive(false);
        RegisterUI.SetActive(false);
        CharacterChoiceUI.SetActive(false);
        CharacterSelectUI.SetActive(false);
        ChattingUI.SetActive(true);
        MatchStartUI.SetActive(true);

        isLive = true;

        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GoRegister()
    {
        LoginUI.SetActive(false);
        RegisterUI.SetActive(true);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GoLogin()
    {
        RegisterUI.SetActive(false);
        LoginUI.SetActive(true);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GoCharacterChoice()
    {
        LoginUI.SetActive(false);
        RegisterUI.SetActive(false);
        CharacterChoiceUI.SetActive(true);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GoCharacterSelect()
    {
        LoginUI.SetActive(false);
        RegisterUI.SetActive(false);
        CharacterSelectUI.SetActive(true);
        for(int i = 0; i < possession.Count; i++)
        {
            GameObject.Find("CharacterSelect").transform.GetChild(0).GetChild((int)possession[i]).gameObject.SetActive(true);
        }

        CharacterChange(possession[0]);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void CharacterChange(uint index)
    {
        Debug.Log(index);
        characterId = index;

        if (GameObject.Find("Canvas").transform.Find("CharacterChoice").gameObject.activeSelf)
        {
            Transform ChoiceImage = GameObject.Find("Canvas").transform.GetChild(4).GetChild(2);
            ChoiceImage.GetComponent<Image>().sprite = farmers[(int)index];
        }
        if (GameObject.Find("Canvas").transform.Find("CharacterSelect").gameObject.activeSelf)
        {
            Transform SelectImage = GameObject.Find("Canvas").transform.GetChild(5).GetChild(2);
            SelectImage.GetComponent<Image>().sprite = farmers[(int)index];
        }
    }

    public void SetBtn()
    {
        {
            Transform group = GameObject.Find("Canvas").transform.GetChild(4).GetChild(0);
            int count = group.childCount;
            for (uint i = 0; i < count; i++)
            {
                Transform btn = group.GetChild((int)i);
                uint index = i;
                btn.GetComponent<Button>().onClick.AddListener(() => CharacterChange(index));
            }
        }

        {
            Transform group = GameObject.Find("Canvas").transform.GetChild(5).GetChild(0);
            int count = group.childCount;
            for (uint i = 0; i < count; i++)
            {
                Transform btn = group.GetChild((int)i);
                uint index = i;
                btn.GetComponent<Button>().onClick.AddListener(() => CharacterChange(index));
            }
        }
    }

     public void BattleGameStart(){
        MatchStartUI.SetActive(false);
    }

    public void GameOver() {
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine() {
        isLive = false;
        yield return new WaitForSeconds(0.5f);

        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
    }

    public void GameRetry() {
        SceneManager.LoadScene(0);
    }

    public void GameQuit() {
        Application.Quit();
    }

    void Update()
    {
        if (!isLive) {
            return;
        }
        gameTime += Time.deltaTime;
    }
    private Image victory;
    private Image defeat;
    public void GameEnd(string result, List<GameEndPayload.UserState> users)
    {
        victory = GameEndUI.transform.Find("victory").GetComponent<Image>();
        defeat = GameEndUI.transform.Find("defeat").GetComponent<Image>();
        Text user1Name = GameEndUI.transform.Find("Panel/user1_name").GetComponent<Text>();
        Text user1Kill = GameEndUI.transform.Find("Panel/user1_kill").GetComponent<Text>();
        Text user1Death = GameEndUI.transform.Find("Panel/user1_death").GetComponent<Text>();
        Text user2Name = GameEndUI.transform.Find("Panel/user2_name").GetComponent<Text>();
        Text user2kill = GameEndUI.transform.Find("Panel/user2_kill").GetComponent<Text>();
        Text user2Death = GameEndUI.transform.Find("Panel/user2_death").GetComponent<Text>();
        Text user3Name = GameEndUI.transform.Find("Panel/user3_name").GetComponent<Text>();
        Text user3Kill = GameEndUI.transform.Find("Panel/user3_kill").GetComponent<Text>();
        Text user3Death = GameEndUI.transform.Find("Panel/user3_death").GetComponent<Text>();
        Text user4Name = GameEndUI.transform.Find("Panel/user4_name").GetComponent<Text>();
        Text user4Kill = GameEndUI.transform.Find("Panel/user4_kill").GetComponent<Text>();
        Text user4Death = GameEndUI.transform.Find("Panel/user4_death").GetComponent<Text>();

        GameEndUI.SetActive(true);
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
        user1Name.text = users[0].playerId;
        user1Kill.text = users[0].kill.ToString();
        user1Death.text = users[0].death.ToString();
        user2Name.text = users[1].playerId;
        user2kill.text = users[1].kill.ToString(); 
        user2Death.text = users[1].death.ToString(); 
        user3Name.text = users[2].playerId;
        user3Kill.text = users[2].kill.ToString(); 
        user3Death.text = users[2].death.ToString(); 
        user4Name.text = users[3].playerId;
        user4Kill.text = users[3].kill.ToString(); 
        user4Death.text = users[3].death.ToString(); 


    }

    public void ReturnLobby()
    {
        GameEndUI.SetActive(false);
    }
}
