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

    private float direction;

    //총알 관련 텍스트
    public GameObject projectilePrefab;
    public Transform firePoint;
    public GameObject bulletManager;

    void Awake()
    {
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshPro>();
    }

    public void Init(uint characterId, string playerId)
    {
        gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
        anim.runtimeAnimatorController = animCon[characterId];
        lastPosition = Vector3.zero;
        currentPosition = Vector3.zero;
        direction = 0;
        hpSlider.gameObject.SetActive(false);

        this.characterId = characterId;

        if (playerId.Length > 6)
        {
            myText.text = playerId[..6];
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
    public void UpdatePosition(float x, float y, float dir)
    {
        lastPosition = currentPosition;
        currentPosition = new Vector3(x, y);
        transform.position = currentPosition;
        direction = dir;

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

        if (direction != 0)
        {
            spriter.flipX = direction < 0;
        }
    }

    public void SetSkill(float x, float y, float rangeX, float rangeY, uint skill_type, string prefabNum)
    {
        switch (skill_type)
        {
            case 1:
                transform.GetChild(4).gameObject.SetActive(true);
                transform.GetChild(4).localPosition = new Vector2(x, y);
                transform.GetChild(4).localScale = new Vector3(rangeX, rangeY, 1);
                StartCoroutine(AttackRangeCheck());
                break;
            case 2:
                GameObject projectile = Instantiate(projectilePrefab, transform.position + new Vector3(x, y), Quaternion.identity, bulletManager.transform);
                BulletPrefab projScript = projectile.GetComponent<BulletPrefab>();
                projectile.gameObject.tag = gameObject.tag;
                projScript.bulletNum = prefabNum;
                projScript.skillType = skill_type;
                if (x > 0)
                {
                    projScript.direction = Vector2.right;
                }

                else if (y > 0)
                {
                    projScript.direction = Vector2.up;
                }

                else if (y < 0)
                {
                    projScript.direction = Vector2.down;
                }
                else if (x < 0)
                {
                    projScript.direction = Vector2.left;
                }
                break;
            default:
                break;
        }
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
