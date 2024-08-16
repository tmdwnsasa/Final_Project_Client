using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPrefab : MonoBehaviour
{
    public float speed = 10f;
    public Vector2 direction;
    public string bulletNum;
    public uint skillType;

    public void Awake()
    {
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        float timescale = 1 / 120f;
        while (true)
        {
            transform.Translate(direction * speed * timescale);
            yield return new WaitForSeconds(timescale);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != gameObject.tag && collision.gameObject.tag != "area" && collision.gameObject.tag != "ground")
        {
            if (collision.gameObject.tag != "red" && collision.gameObject.tag != "blue")
            {
                Debug.Log("skillType은 !!" + skillType);
                NetworkManager.instance.SendRemoveSkillPacket(bulletNum, skillType);
            }
            // 충돌 처리
            Destroy(gameObject);
        }
    }
}
