using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPrefab : MonoBehaviour
{
    public float duration;

    public void Awake()
    {
        StartCoroutine(removePrefab());
    }

    IEnumerator removePrefab()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "area" && collision.gameObject.tag != "ground" && collision.gameObject.layer != 8)
        {
            // 충돌 처리
            Destroy(gameObject);
        }
    }
}
