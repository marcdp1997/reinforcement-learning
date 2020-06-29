using System;
using UnityEngine;

[Serializable]
public struct PlayerStats
{
    public Collider hitbox;
    public float speed;
    public float maxLife;
    public int maxBullets;
}

[Serializable]
public struct ShootingStats
{
    public BulletPool pool;
    public Transform firePoint;
    public float damage;
    public float rateRecover;
    public float speed;
    public float timeActive;
}

[Serializable]
public struct ShieldStats
{
    public GameObject prefab;
    public float cooldown;
    public float timeActive;
}

[Serializable]
public struct HealStats
{
    public ParticleSystem effect;
    public float damage;
}

