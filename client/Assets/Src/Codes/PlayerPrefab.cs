using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefab : MonoBehaviour
{
    public RuntimeAnimatorController[] animCon;
    private Animator anim;
    private SpriteRenderer spriter;
    private Vector3 lastPosition;
    private Vector3 currentPosition;
    private uint characterId;
    TextMeshPro myText;

    public float nowHp;
    public float hp;
    public Slider hpSlider;

    void Awake()
    {
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshPro>();
    }

    public void Init(uint characterId, string playerId)
    {
        anim.runtimeAnimatorController = animCon[characterId];
        lastPosition = Vector3.zero;
        currentPosition = Vector3.zero;
        hpSlider.gameObject.SetActive(false);

        this.characterId = characterId;

        if (playerId.Length > 5)
        {
            myText.text = playerId[..5];
        }
        else
        {
            myText.text = playerId;
        }
        myText.GetComponent<MeshRenderer>().sortingOrder = 6;
    }

    void OnEnable()
    {
        anim.runtimeAnimatorController = animCon[characterId];
    }

    // 서버로부터 위치 업데이트를 수신할 때 호출될 메서드
    public void UpdatePosition(float x, float y)
    {
        lastPosition = currentPosition;
        currentPosition = new Vector3(x, y);
        transform.position = currentPosition;

        UpdateAnimation();
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
        {
            return;
        }

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        // 현재 위치와 이전 위치를 비교하여 이동 벡터 계산
        Vector2 inputVec = currentPosition - lastPosition;

        anim.SetFloat("Speed", inputVec.magnitude);

        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0;
        }
    }

    public void SetNearSkill(float x, float y, float rangeX, float rangeY)
    {
        transform.GetChild(4).gameObject.SetActive(true);
        transform.GetChild(4).localPosition = new Vector2(x, y);
        transform.GetChild(4).localScale = new Vector3(rangeX, rangeY, 1);
        StartCoroutine(AttackRangeCheck());
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.instance.isLive)
        {
            return;
        }
    }
    IEnumerator AttackRangeCheck()
    {
        yield return new WaitForSeconds(1.0f);
        transform.GetChild(4).gameObject.SetActive(false);
    }

    public void SetHp(float hp)
    {
        nowHp = hp;
        hpSlider.value = nowHp / this.hp;
        //hp 설정
        if (hp <= 0)
        {
            hpSlider.gameObject.SetActive(false);
            anim.SetBool("Dead", true);
            gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        }
    }

    public void startSetHp(float hp)
    {
        hpSlider.gameObject.SetActive(true);
        //hp 설정
        this.hp = hp;
        nowHp = hp;
        hpSlider.value = nowHp / hp;
    }
}
