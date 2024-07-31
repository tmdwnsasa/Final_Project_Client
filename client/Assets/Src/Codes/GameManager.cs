using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        for(int i = 0; i < GameManager.instance.possession.Count; i++)
        {
            GameObject.Find("CharacterSelect").transform.GetChild(0).GetChild((int)GameManager.instance.possession[i]).gameObject.SetActive(true);
        }

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
}
