using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoePrefab : MonoBehaviour
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
}
