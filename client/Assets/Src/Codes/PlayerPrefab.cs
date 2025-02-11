using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefab : MonoBehaviour
{
    public RuntimeAnimatorController[] animCon;
    private Animator anim;//inventory inven
    private SpriteRenderer spriter;
    public Vector2 newPosition;
    public uint guild;

    private Vector3 lastPosition;
    private Vector3 currentPosition;
    private uint characterId;//
    TextMeshPro myText;//

    public float nowHp;
    public float hp;
    public Slider hpSlider;

    public float direction;

    public GameObject sickleRange;
    public GameObject shovelRange;

    public SpriteRenderer gunSprite;

    //총알 관련 텍스트
    public GameObject projectilePrefab;

    public GameObject blueHealPrefab;
    public GameObject greenHealPrefab;

    //불장판 관련 텍스트
    public GameObject AoePrefab;

    public TextMeshPro buffText;

    void Awake()
    {
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshPro>();

        if (guild == 1)
        {
            myText.color = Color.blue;
        }
        else if (guild == 2)
        {
            myText.color = Color.green;
        }
    }

    private void Update()
    {
        UpdatePosition();
    }

    public void Init(string playerId, uint characterId, uint guild)
    {
        gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
        anim.runtimeAnimatorController = animCon[characterId];
        lastPosition = Vector3.zero;
        currentPosition = Vector3.zero;
        direction = 0;
        hpSlider.gameObject.SetActive(false);
        transform.GetChild(4).gameObject.SetActive(false);

        this.characterId = characterId;

        if (playerId.Length > 6)
        {
            myText.text = playerId[..6];
        }
        else
        {
            myText.text = playerId;
        }
        this.guild = guild;
        if (guild == 1)
            myText.color = Color.blue;
        else if (guild == 2)
            myText.color = Color.green;
        myText.GetComponent<MeshRenderer>().sortingOrder = 6;
    }

    void OnEnable()
    {
        anim.runtimeAnimatorController = animCon[characterId];
    }

    // 서버로부터 위치 업데이트를 수신할 때 호출될 메서드
    public void UpdatePosition()
    {
        lastPosition = transform.position;
        currentPosition = newPosition;
        transform.position = Vector3.Lerp(transform.position, newPosition, 0.2f);

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

    public void SetSkill(float x, float y, float rangeX, float rangeY, uint skill_type, string prefabNum, float speed, float duration)
    {
        switch (skill_type)
        {
            case 7:
            case 1:
                if(characterId == 0) {
                    sickleRange.SetActive(true);
                    sickleRange.transform.localPosition = new Vector2(x, y);
                    SpriteRenderer nearSkillRender = sickleRange.GetComponent<SpriteRenderer>();
                    nearSkillRender.flipX = spriter.flipX;
                    if (x > 0)
                    {
                        sickleRange.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    else if (y > 0)
                    {
                        sickleRange.transform.rotation = Quaternion.Euler(0, 0, nearSkillRender.flipX ? 90 * -1 : 90);
                    }
                    else if (y < 0)
                    {
                        sickleRange.transform.rotation = Quaternion.Euler(0, 0, nearSkillRender.flipX ? 90 : 90 * -1);
                    }
                    else if (x < 0)
                    {
                        sickleRange.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    StartCoroutine(AttackRangeCheck(sickleRange));
                }
                else if(characterId == 2) {
                    shovelRange.SetActive(true);
                    shovelRange.transform.localPosition = new Vector2(x, y);
                    SpriteRenderer nearSkillRender = shovelRange.GetComponent<SpriteRenderer>();
                    nearSkillRender.flipX = spriter.flipX;
                    if (x > 0)
                    {
                        shovelRange.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    else if (y > 0)
                    {
                        shovelRange.transform.rotation = Quaternion.Euler(0, 0, nearSkillRender.flipX ? 90 * -1 : 90);
                    }
                    else if (y < 0)
                    {
                        shovelRange.transform.rotation = Quaternion.Euler(0, 0, nearSkillRender.flipX ? 90 : 90 * -1);
                    }
                    else if (x < 0)
                    {
                        shovelRange.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    StartCoroutine(AttackRangeCheck(shovelRange));
                }
                if (skill_type == 1)
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Swing);
                if (skill_type == 7)
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Stun);
                break;
            case 2:
                GameObject projectile = Instantiate(projectilePrefab, transform.position + new Vector3(x, y), Quaternion.identity);
                if(characterId == 1) {
                    projectile.GetComponent<SpriteRenderer>().color = Color.white;
                } else if (characterId == 3) {
                    projectile.GetComponent<SpriteRenderer>().color = Color.green;
                }
                BulletPrefab projScript = projectile.GetComponent<BulletPrefab>();
                projectile.gameObject.tag = gameObject.tag;
                projScript.bulletNum = prefabNum;
                projScript.skillType = skill_type;
                projScript.speed = speed;
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
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Shot);
                break;
            case 4:
                if (characterId == 0)
                {
                    StartCoroutine(StartBuff("나 화났어!", duration));
                }
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Berserk);
                break;
            case 5:
                if(guild == 1) {
                    GameObject blueHeal = Instantiate(blueHealPrefab, transform.position + new Vector3(x, y), Quaternion.identity);
                    HealPrefab blueHealScript = blueHeal.GetComponent<HealPrefab>();
                    blueHealScript.duration = duration;
                }
                else if(guild == 2) {
                    GameObject greenHeal = Instantiate(greenHealPrefab, transform.position + new Vector3(x, y), Quaternion.identity);
                    HealPrefab greenHealScript = greenHeal.GetComponent<HealPrefab>();
                    greenHealScript.duration = duration;
                }                
                break;
            case 8:
                if(characterId == 1)
                {
                    GameObject fireAoe = Instantiate(AoePrefab, transform.position, Quaternion.identity);
                    fireAoe.transform.localScale = new Vector3(rangeX + 5, rangeY + 5, 1);
                    AoePrefab fireAoeScript = fireAoe.GetComponent<AoePrefab>();
                    fireAoeScript.duration = duration;
                }
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Fire);
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

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.instance.isLive)
        {
            return;
        }
    }

    IEnumerator AttackRangeCheck(GameObject obj)
    {
        yield return new WaitForSeconds(0.5f);
        obj.SetActive(false);
    }

    public void SetHp(float hp, bool isHeal)
    {
        if(isHeal) {
            StartCoroutine(AttackedCharacter(Color.green));
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Heal);
        } else {
            StartCoroutine(AttackedCharacter(Color.red));
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hurt);
        }

        nowHp = hp;
        hpSlider.value = nowHp / this.hp;
        //hp 설정
        if (hp <= 0)
        {
            StopAllCoroutines();

            if(gunSprite.gameObject.activeSelf) {
                gunSprite.gameObject.SetActive(false);
            }
            if(sickleRange.gameObject.activeSelf) {
                sickleRange.gameObject.SetActive(false);
            } 
            if(shovelRange.gameObject.activeSelf) {
                shovelRange.gameObject.SetActive(false);
            } 
            if(buffText.gameObject.activeSelf) {
                buffText.gameObject.SetActive(false);
            } 
            GetComponent<SpriteRenderer>().color = Color.white;

            hpSlider.gameObject.SetActive(false);
            anim.SetBool("Dead", true);
            gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
        }
    }

    IEnumerator AttackedCharacter(Color color)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = color;
        yield return new WaitForSeconds(0.3f);
        sprite.color = Color.white;
    }

    IEnumerator StartBuff(string text, float duration)
    {
        buffText.gameObject.SetActive(true);
        buffText.text = text;
        yield return new WaitForSeconds(duration);
        buffText.gameObject.SetActive(false);
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
