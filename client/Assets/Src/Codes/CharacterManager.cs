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

    public void Spawn(LocationUpdate data)
    {
        // if (!GameManager.instance.isLive)
        // {
        //     return;
        // }

        HashSet<string> newUsers = new HashSet<string>();

        foreach (LocationUpdate.UserLocation user in data.users)
        {
            if (user.playerId == GameManager.instance.player.name)
            {
                GameManager.instance.player.newPosition = new Vector2(user.x, user.y);
            }
            else
            {
                newUsers.Add(user.playerId);
                GameObject player = GameManager.instance.pool.Get(user.playerId, user.characterId);
                PlayerPrefab playerScript = player.GetComponent<PlayerPrefab>();
                playerScript.UpdatePosition(user.x, user.y, user.direction);
            }
        }

        foreach (string userId in currentUsers)
        {
            if (!newUsers.Contains(userId))
            {
                GameManager.instance.pool.Remove(userId);
            }
        }

        currentUsers = newUsers;
    }

    public void UpdateAttack(SkillUpdate data)
    {
        if (data.playerId == GameManager.instance.player.name)
        {
            GameManager.instance.player.SetNearSkill(data.x, data.y, data.rangeX, data.rangeY, data.skillType);
        }
        else
        {
            GameObject player = GameManager.instance.pool.GetId(data.playerId);
            PlayerPrefab playerScript = player.GetComponent<PlayerPrefab>();
            playerScript.SetNearSkill(data.x, data.y, data.rangeX, data.rangeY, data.skillType);
        }
    }

    public void UpdateCharacterState(AttackedSuccess data)
    {
        if (data.playerId == GameManager.instance.player.name)
        {
            GameManager.instance.player.SetHp(data.hp);
        }
        else
        {
            GameObject player = GameManager.instance.pool.GetId(data.playerId);
            PlayerPrefab playerScript = player.GetComponent<PlayerPrefab>();
            playerScript.SetHp(data.hp);
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