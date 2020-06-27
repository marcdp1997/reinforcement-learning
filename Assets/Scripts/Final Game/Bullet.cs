﻿using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rBody;
    private Player source;

    private void Awake()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public void Shoot(Player source, float bulletSpeed, Vector3 direction, float timeActive, float damage)
    {
        this.damage = damage;
        this.source = source;

        rBody.velocity = new Vector3(direction.x, 0.0f, direction.z).normalized * bulletSpeed;
        StartCoroutine(CheckTimeActive(timeActive));
    }

    private IEnumerator CheckTimeActive(float timeActive)
    {
        yield return new WaitForSeconds(timeActive);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Team Red") ||
            other.gameObject.layer == LayerMask.NameToLayer("Team Blue") ||
            other.gameObject.layer == LayerMask.NameToLayer("Invisible Red") ||
            other.gameObject.layer == LayerMask.NameToLayer("Invisible Blue"))
        {
            other.GetComponentInParent<Player>().RecieveDamage(source, damage);
            Destroy(this.gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Wall") ||
            other.gameObject.layer == LayerMask.NameToLayer("Shield Red") ||
            other.gameObject.layer == LayerMask.NameToLayer("Shield Blue"))
            Destroy(this.gameObject);
    }

    public float damage { get; set; }
}
