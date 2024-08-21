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

    public Vector2 newPosition;

    public int characterId;
    public string characterName;

    public float nowHp;
    public float hp = 0;
    public int guild = 0;

    public Slider hpSlider;

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

    //총알 관련 텍스트
    public GameObject projectilePrefab;
    public Transform firePoint;
    public GameObject bulletManager;

    private Vector2 oldInputVec;
    private bool isPlusX;
    private bool isMinusX;
    private bool isPlusY;
    private bool isMinusY;

    public string zSkill;
    public float zSkill_CoolTime;
    public uint zSkill_id;
    public string xSkill;
    public float xSkill_CoolTime;
    public uint xSkill_id;

    public bool directionX;

    private bool isZSkill;
    private bool isXSkill;

    public SpriteRenderer gunSprite;

    public float x = 1, y = 1;
    public Vector2 BoxArea = new Vector2(0.5f, 0);
    void Awake()
    {
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        myText = GetComponentInChildren<TextMeshPro>();

        hpSlider.gameObject.SetActive(false);

        oldInputVec = new Vector2(0, 0);

        isPlusX = false;
        isMinusX = false;
        isPlusY = false;
        isMinusY = false;
        isZSkill = true;
        isXSkill = true;
        directionX = true;
        BoxArea.x = 0.5f;
        BoxArea.y = 0f;
        hpSlider.value = 1;

        if(guild == 1)
        {
            myText.color = Color.blue;
        }
        else if (guild == 2)
        {
            myText.color = Color.green;
        }
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
        Debug.Log(GameManager.instance.characterId);
        anim.runtimeAnimatorController = animCon[GameManager.instance.characterId];
    }

    // Update is called once per frame
    void Update()
    {
        movePlayer();
        if (!GameManager.instance.isLive || GameManager.instance.chatting.inputField.isFocused)
        {
            inputVec = new Vector2(0, 0);
            if (oldInputVec != inputVec)
            {
                // 위치 이동 패킷 전송 -> 서버로
                NetworkManager.instance.SendLocationUpdatePacket(inputVec.x, inputVec.y);
            }

            oldInputVec = inputVec;
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

        if (!(inputVec.x != 0 && inputVec.y != 0))
        {
            //공격
            if (Input.GetKeyDown(KeyCode.Z) && !NetworkManager.instance.isLobby && isZSkill)
            {
                if (x != 0)
                {
                    directionX = true;
                    //send 스킬 패킷을 보내고
                    NetworkManager.instance.SendSkillUpdatePacket(BoxArea.x, BoxArea.y, directionX, zSkill_id);
                }
                else
                {
                    directionX = false;
                    NetworkManager.instance.SendSkillUpdatePacket(BoxArea.x, BoxArea.y, directionX, zSkill_id);
                }
                isZSkill = false;

                StartCoroutine(CoolTimeCheck("zSkill"));

            }
            if (Input.GetKeyDown(KeyCode.X) && !NetworkManager.instance.isLobby && isXSkill)
            {
                if (x != 0)
                {
                    directionX = true;
                    //send 스킬 패킷을 보내고
                    NetworkManager.instance.SendSkillUpdatePacket(BoxArea.x, BoxArea.y, directionX, xSkill_id);
                }
                else
                {
                    directionX = false;
                    NetworkManager.instance.SendSkillUpdatePacket(BoxArea.x, BoxArea.y, directionX, xSkill_id);
                }
                isXSkill = false;

                StartCoroutine(CoolTimeCheck("xSkill"));

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

        if (normal.x > 0)
        {
            isPlusX = true;
        }
        if (normal.x < 0)
        {
            isMinusX = true;
        }

        if (normal.y > 0)
        {
            isPlusY = true;
        }
        if (normal.y < 0)
        {
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

    public void movePlayer()
    {
        transform.position = Vector2.Lerp(transform.position, newPosition, 0.2f);
    }

    public void SetSkill(float x, float y, float rangeX, float rangeY, uint skillType, string prefabNum)
    {
        switch (skillType)
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
                projScript.skillType = skillType;

                if (x > 0)
                {
                    StartCoroutine(SetActiveGunSprite());
                    projScript.direction = Vector2.right;
                }
                else if (y > 0)
                {
                    StartCoroutine(SetActiveGunSprite(90f));
                    projScript.direction = Vector2.up;
                }
                else if (y < 0)
                {
                    StartCoroutine(SetActiveGunSprite(-90f));
                    projScript.direction = Vector2.down;
                }
                else if (x < 0)
                {
                    StartCoroutine(SetActiveGunSprite());
                    projScript.direction = Vector2.left;
                }
                break;
            case 4:
                if (xSkill_id == 2)
                {
                    StartCoroutine(ChangeColorByBuff("green", 5));
                }
                break;
            default:
                break;
        }
    }

    IEnumerator SetActiveGunSprite(float z = 0)
    {
        gunSprite.gameObject.SetActive(true);
        gunSprite.flipX = spriter.flipX;
        z = gunSprite.flipX ? z * -1 : z;
        gunSprite.transform.rotation = Quaternion.Euler(0, 0, z);
        yield return new WaitForSeconds(0.5f);
        gunSprite.gameObject.SetActive(false);
    }

    IEnumerator AttackRangeCheck()
    {
        yield return new WaitForSeconds(1.0f);
        transform.GetChild(4).gameObject.SetActive(false);
    }

    IEnumerator CoolTimeCheck(string Skill)
    {
        if ("zSkill" == Skill)
        {
            yield return new WaitForSeconds(zSkill_CoolTime);
            isZSkill = true;
        }

        else
        {
            yield return new WaitForSeconds(xSkill_CoolTime);
            isXSkill = true;
        }
    }


    public void SetHp(float hp)
    {
        StartCoroutine(AttackedCharacter());
        nowHp = hp;
        hpSlider.value = nowHp / this.hp;
        //hp 설정
        if (hp <= 0)
        {
            StopAllCoroutines();

            if(gunSprite.gameObject.activeSelf) {
                gunSprite.gameObject.SetActive(false);
            } 
            GetComponent<SpriteRenderer>().color = Color.white;

            hpSlider.gameObject.SetActive(false);
            anim.SetBool("Dead", true);
            GameManager.instance.isLive = false;
            gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        }
    }

    IEnumerator AttackedCharacter()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        sprite.color = Color.white;
    }

    IEnumerator ChangeColorByBuff(string color, float duration)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Color newColor;
        ColorUtility.TryParseHtmlString(color, out newColor);
        sprite.color = newColor;
        yield return new WaitForSeconds(duration);
        sprite.color = Color.white;
    }

    public void SetSkill(string isSkill)
    {
        if ("isSkillZ" == isSkill)
        {
            this.isZSkill = true;
        }
        else
        {
            this.isXSkill = true;
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
    public void ResetAnimation()
    {
        anim.SetBool("Dead", false);
        gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
    }
}
