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
    public float speed;
    public string playerId;
    public string name;
    public RuntimeAnimatorController[] animCon;

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
    TextMeshPro myText;

    private Vector2 oldInputVec;
    private bool isPlusX;
    private bool isMinusX;
    private bool isPlusY;
    private bool isMinusY;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        myText = GetComponentInChildren<TextMeshPro>();

        oldInputVec = new Vector2(0, 0);

        isPlusX = false;
        isMinusX = false;
        isPlusY = false;
        isMinusY = false;
    }

    void OnEnable() {

        if (name.Length > 5) {
            myText.text = name[..5];
        } else {
            myText.text = name;
        }
        myText.GetComponent<MeshRenderer>().sortingOrder = 6;
        anim.runtimeAnimatorController = animCon[GameManager.instance.characterId];
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isLive || GameManager.instance.chatting.inputField.isFocused) {
            return;
        }
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");

        if((isPlusX && inputVec.x < 0) || (isMinusX && inputVec.x > 0)) {
            inputVec.x = -inputVec.x;
        }
        if((isPlusY && inputVec.y < 0) || (isMinusY && inputVec.y > 0)) {
            inputVec.y = -inputVec.y;
        }

        if(oldInputVec != inputVec)
        {
            // 위치 이동 패킷 전송 -> 서버로
            NetworkManager.instance.SendLocationUpdatePacket(inputVec.x, inputVec.y);
        }

        oldInputVec = inputVec;
    }


    void FixedUpdate() {
        if (!GameManager.instance.isLive) {
            return;
        }
        // 힘을 준다.
        // rigid.AddForce(inputVec);

        // 속도 제어
        // rigid.velocity = inputVec;

        // 위치 이동
        // Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        // rigid.MovePosition(rigid.position + nextVec);
    }

    // Update가 끝난이후 적용
    void LateUpdate() {
        if (!GameManager.instance.isLive) {
            return;
        }

        anim.SetFloat("Speed", inputVec.magnitude);

        if (inputVec.x != 0) {
            spriter.flipX = inputVec.x < 0;
        }
    }

    void OnCollisionStay2D(Collision2D collision) {
        if (!GameManager.instance.isLive) {
            return;
        }

        ContactPoint2D contact = collision.contacts[0];
        //법선 벡터
        Vector2 normal = contact.normal;
        //Debug.Log("법선 벡터: " + normal);

        if(normal.x > 0) {
            isPlusX = true;
        } else if(normal.x < 0) {
            isMinusX = true;
        }

        if(normal.y > 0) {
            isPlusY = true;
        } else if(normal.y < 0) {
            isMinusY = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision) 
    {
        if (!GameManager.instance.isLive) {
            return;
        }

        isPlusX = false;
        isMinusX = false;
        isPlusY = false;
        isMinusY = false;
    }

    public void movePlayer(float x, float y) {
        rigid.MovePosition(new Vector2(x, y));
    }
}
