﻿using System.Collections;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public void Use(float timeActive)
    {
        StartCoroutine(CheckTimeActive(timeActive));
    }

    private IEnumerator CheckTimeActive(float timeActive)
    {
        yield return new WaitForSeconds(timeActive);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bullet Red") ||
            other.gameObject.layer == LayerMask.NameToLayer("Bullet Blue"))
        {
            Destroy(other.gameObject);
        }
    }
}
