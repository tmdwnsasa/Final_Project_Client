using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("#BGM")]
    public AudioClip[] bgmClip;
    public float bgmVolume;
    AudioSource[] bgmPlayer;
    AudioHighPassFilter bgmEffect;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;

    [Header("#LoopedSFX")]
    public AudioClip[] LoopedSfxClips;
    public float LoopedSfxVolume;
    public int LoopedChannels;
    AudioSource[] LoopedSfxPlayers;
    int LoopedChannelIndex;

    public enum BGM : int
    { Lobby = 0, Game }
    public enum SFX { Dead, Hit, LevelUp = 3, Lose, Melee, Range = 7, Select, Win }


    void Awake()
    {
        instance = this;
        Init();
    }

    void Init()
    {
        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;

        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            bgmPlayer[i] = bgmObject.AddComponent<AudioSource>();
            bgmPlayer[i].playOnAwake = false;
            bgmPlayer[i].loop = true;
            bgmPlayer[i].volume = bgmVolume;
            bgmPlayer[i].clip = bgmClip[i];
        }
        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].bypassListenerEffects = true;
            sfxPlayers[i].volume = sfxVolume;
        }

        // 반복 효과음 플레이어 초기화
        GameObject loopedSfxObject = new GameObject("LoopedSfxPlayer");
        loopedSfxObject.transform.parent = transform;
        LoopedSfxPlayers = new AudioSource[channels];

        for (int i = 0; i < LoopedSfxPlayers.Length; i++)
        {
            LoopedSfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            LoopedSfxPlayers[i].playOnAwake = false;
            LoopedSfxPlayers[i].loop = true;
            LoopedSfxPlayers[i].bypassListenerEffects = true;
            LoopedSfxPlayers[i].volume = sfxVolume;
        }
    }

    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
        {
            if(NetworkManager.instance.isLobby == true)
                bgmPlayer[(int)BGM.Lobby].Play();
            else if (NetworkManager.instance.isLobby == false)
                bgmPlayer[(int)BGM.Game].Play();
            //bgmPlayer.Play();
            //테스트할때 잠시 브금 꺼둠! 이부분 꼭 머지하기전에 고치기
        }
        else
        {
            if (NetworkManager.instance.isLobby == true)
                bgmPlayer[(int)BGM.Lobby].Stop();
            else if (NetworkManager.instance.isLobby == false)
                bgmPlayer[(int)BGM.Game].Stop();
        }
    }

    public void EffectBgm(bool isPlay)
    {
        bgmEffect.enabled = isPlay;
    }

    public void PlaySfx(SFX sfx)
    {

        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            int loopIndex = (i + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
            {
                continue;
            }

            int ranIndex = 0;
            if (sfx == SFX.Hit || sfx == SFX.Melee)
            {
                ranIndex = Random.Range(0, 2);
            }

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx + ranIndex];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
}
