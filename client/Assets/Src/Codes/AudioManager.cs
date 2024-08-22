using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("#BGM")]
    public AudioClip[] bgmClip;
    public float bgmVolume;
    public int bgmChannels;
    AudioSource[] bgmPlayer;
    AudioHighPassFilter bgmEffect;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int sfxChannels;
    AudioSource[] sfxPlayers;
    int channelIndex;

    public enum Bgm { Lobby, Game }
    public enum Sfx { Dead, Shot, Swing, LevelUp, Hurt, Heal, Walk, Select, Win, Lose, Stun, Berserk, Berserk2, Fire }


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
        bgmPlayer = new AudioSource[bgmChannels];

        for (int i = 0; i < bgmPlayer.Length; i++)
        {
            bgmPlayer[i] = bgmObject.AddComponent<AudioSource>();
            bgmPlayer[i].playOnAwake = false;
            bgmPlayer[i].loop = true;
            //작은 소리 ||로 추가
            bgmPlayer[i].volume = bgmVolume;
            bgmPlayer[i].clip = bgmClip[i];
        }
        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[sfxChannels];

        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            //루프 해야하는 소리 ||로 추가
            sfxPlayers[i].bypassListenerEffects = true;
            sfxPlayers[i].volume = sfxVolume;
        }
    }

    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
        {
            //bgmPlayer.Play();
            //테스트할때 잠시 브금 꺼둠! 이부분 꼭 머지하기전에 고치기
            if(NetworkManager.instance.isLobby == true)
            {
                bgmPlayer[(int)Bgm.Lobby].Play();
            }
            else if(NetworkManager.instance.isLobby == false)
            {
                bgmPlayer[(int)Bgm.Game].Play();
            }
        }
        else
        {
            if(NetworkManager.instance.isLobby == true)
            {
                bgmPlayer[(int)Bgm.Lobby].Stop();
            }
            else if(NetworkManager.instance.isLobby == false)
            {
                bgmPlayer[(int)Bgm.Game].Stop();
            }
        }
    }

    public void EffectBgm(bool isPlay)
    {
        bgmEffect.enabled = isPlay;
    }

    public void PlaySfx(Sfx sfx)
    {
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            int loopIndex = (i + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
            {
                continue;
            }

            int ranIndex = 0;
            if(sfx == Sfx.Walk)
            {
                sfxPlayers[(int)Sfx.Walk].loop = true;
            }

            //랜덤으로 소리 나야할 때
            if (sfx == Sfx.Berserk)
            {
                ranIndex = Random.Range(0, 1);
            }

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx + ranIndex];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
}
