using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;

    public int characterId;
    public string characterName;
    public float hp;
    public float speed;
    public float power;
    public float defense;
    public float critical;

    public string playerId;
    public string name;
    public RuntimeAnimatorController[] animCon;

    SpriteRenderer spriter;
    Animator anim;
    TextMeshPro myText;

    private Vector2 oldInputVec;
    private bool isPlusX;
    private bool isMinusX;
    private bool isPlusY;
    private bool isMinusY;

    public float x = 1, y = 1;
    public Vector2 BoxArea = new Vector2(0.5f, 0);
    public float attackRangeX = 1, attackRangeY = 2;
    void Awake()
    {
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        myText = GetComponentInChildren<TextMeshPro>();

        oldInputVec = new Vector2(0, 0);

        isPlusX = false;
        isMinusX = false;
        isPlusY = false;
        isMinusY = false;
    }

    void OnEnable()
    {
        if (name.Length > 5)
        {
            myText.text = name[..5];
        }
        else
        {
            myText.text = name;
        }
        myText.GetComponent<MeshRenderer>().sortingOrder = 6;

        anim.runtimeAnimatorController = animCon[GameManager.instance.characterId];
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isLive || GameManager.instance.chatting.inputField.isFocused)
        {
            return;
        }
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");

        if ((isPlusX && inputVec.x < 0) || (isMinusX && inputVec.x > 0))
        {
            inputVec.x = 0;
        }
        if ((isPlusY && inputVec.y < 0) || (isMinusY && inputVec.y > 0))
        {
            inputVec.y = 0;
        }

        if (oldInputVec != inputVec)
        {
            // 위치 이동 패킷 전송 -> 서버로
            NetworkManager.instance.SendLocationUpdatePacket(inputVec.x, inputVec.y);
        }

        oldInputVec = inputVec;

        if (inputVec.x != 0 || inputVec.y != 0)
        {
            x = inputVec.x;
            y = inputVec.y;
        }

        if (x < 0)
            BoxArea.x = -0.5f;
        else if (x > 0)
            BoxArea.x = 0.5f;
        else
            BoxArea.x = 0;

        if (y < 0)
            BoxArea.y = -0.5f;
        else if (y > 0)
            BoxArea.y = 0.5f;
        else
            BoxArea.y = 0;

        if (!(inputVec.x != 0))
        {
            //공격
            if (Input.GetKeyDown(KeyCode.Z) && !NetworkManager.instance.isLobby)
            {
                if (x != 0)
                {
                    //send 스킬 패킷을 보내고
                    NetworkManager.instance.SendSkillUpdatePacket(BoxArea.x, BoxArea.y, attackRangeX, attackRangeY);
                }
                else
                {
                    NetworkManager.instance.SendSkillUpdatePacket(BoxArea.x, BoxArea.y, attackRangeY, attackRangeX);
                }
            }
        }
    }

    // Update가 끝난이후 적용
    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
        {
            return;
        }

        anim.SetFloat("Speed", inputVec.magnitude);

        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.instance.isLive)
        {
            return;
        }
        ContactPoint2D contact = collision.contacts[0];
        //법선 벡터
        Vector2 normal = contact.normal;
        //Debug.Log("법선 벡터: " + normal);

        if(normal.x > 0) {
            isPlusX = true;
        }
        if(normal.x < 0) {
            isMinusX = true;
        }

        if(normal.y > 0) {
            isPlusY = true;
        }
        if(normal.y < 0) {
            isMinusY = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (!GameManager.instance.isLive)
        {
            return;
        }

        isPlusX = false;
        isMinusX = false;
        isPlusY = false;
        isMinusY = false;
    }

    public void movePlayer(float x, float y)
    {
        transform.position = new Vector2(x, y);
    }

    public void SetNearSkill(float x, float y, float rangeX, float rangeY)
    {
        transform.GetChild(4).gameObject.SetActive(true);
        transform.GetChild(4).localPosition = new Vector2(x, y);
        transform.GetChild(4).localScale = new Vector3(rangeX, rangeY, 1);
        StartCoroutine(AttackRangeCheck());
    }

    IEnumerator AttackRangeCheck()
    {
        yield return new WaitForSeconds(1.0f);
        transform.GetChild(4).gameObject.SetActive(false);
    }

    public void SetHp(float hp) {
        //hp 설정
        if(hp <= 0) {
            anim.SetBool("Dead", true);
            GameManager.instance.isLive = false;
        }
    }

    public void ResetAnimation() {
        anim.SetBool("Dead", false);
    }
}
