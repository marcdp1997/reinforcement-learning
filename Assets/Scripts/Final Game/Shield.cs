﻿using System.Collections;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public void Use(float timeActive)
    {
        gameObject.SetActive(true);
        StartCoroutine(CheckTimeActive(timeActive));
    }

    private IEnumerator CheckTimeActive(float timeActive)
    {
        yield return new WaitForSeconds(timeActive);
        gameObject.SetActive(false);
    }
}
