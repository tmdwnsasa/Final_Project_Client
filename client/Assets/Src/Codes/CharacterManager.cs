using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;
    private HashSet<string> currentUsers = new HashSet<string>();

    void Awake()
    {
        instance = this;
    }

    public void CreateOtherPlayers(CreateUser data)
    {
        HashSet<string> newUsers = new HashSet<string>();
        GameObject player = GameManager.instance.pool.Init(data.name, data.characterId, data.guild);
        newUsers.Add(data.name);
        currentUsers = newUsers;
    }

    public void CreateOtherPlayers(string name, uint characterId, uint guild)
    {
        HashSet<string> newUsers = new HashSet<string>();

        GameObject player = GameManager.instance.pool.Init(name, characterId, guild);
        newUsers.Add(name);
        currentUsers = newUsers;
    }

    public void RemoveOtherPlayers(RemoveUser data)
    {
        HashSet<string> newUsers = new HashSet<string>();

        GameManager.instance.pool.Remove(data.name);

        currentUsers.Remove(data.name);
    }

    public void MoveAllPlayers(LocationUpdate data)
    {
        foreach (LocationUpdate.UserLocation user in data.users)
        {
            if (user.playerId == GameManager.instance.player.name)
            {
                GameManager.instance.player.movePlayer(user.x, user.y);
            }
            else
            {
                GameObject player = GameManager.instance.pool.Get(user.playerId, user.characterId);
                PlayerPrefab playerScript = player.GetComponent<PlayerPrefab>();
                playerScript.newPosition = new Vector2(user.x, user.y);
            }
        }
    }

    public void UpdateAttack(SkillUpdate data)
    {
        if (data.playerId == GameManager.instance.player.name)
        {
            GameManager.instance.player.SetNearSkill(data.x, data.y, data.rangeX, data.rangeY);
        }
        else
        {
            GameObject player = GameManager.instance.pool.GetId(data.playerId);
            PlayerPrefab playerScript = player.GetComponent<PlayerPrefab>();
            playerScript.SetNearSkill(data.x, data.y, data.rangeX, data.rangeY);
        }
    }

    public void UpdateCharacterState(AttackedSuccess data)
    {
        foreach (AttackedSuccess.UserAttackState user in data.users)
        {
            if (user.playerId == GameManager.instance.player.name)
            {
                Debug.Log("Player Damaged");
                GameManager.instance.player.SetHp(user.hp);
            }
            else
            {
                Debug.Log("Player Damaged");
                GameObject player = GameManager.instance.pool.GetId(user.playerId);
                PlayerPrefab playerScript = player.GetComponent<PlayerPrefab>();
                playerScript.SetHp(user.hp);
            }
        }
    }

    public void SetCharacterHp(BattleStart data)
    {
        foreach (BattleStart.UserTeam user in data.users)
        {
            if (user.playerId != GameManager.instance.player.name)
            {
                GameObject player = GameManager.instance.pool.GetId(user.playerId);
                PlayerPrefab playerScript = player.GetComponent<PlayerPrefab>();
                playerScript.startSetHp(user.hp);
            }

            else if (user.playerId == GameManager.instance.player.name)
            {
                GameManager.instance.player.startSetHp(user.hp);
            }
        }
    }
}