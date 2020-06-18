using System;
using UnityEngine;

[Serializable]
public struct PlayerStats
{
    public float speed;
    public float damage;
    public float maxLife;
}

[Serializable]
public struct ShootingStats
{
    public GameObject prefab;
    public Transform firePoint;
    public float rateRecover;
    public float speed;
    public float timeActive;
}

[Serializable]
public struct ShieldStats
{
    public GameObject particles;
    public GameObject prefab;
    public float cooldown;
    public float timeActive;
}

[Serializable]
public struct HealStats
{
    public GameObject particles;
    public ParticleSystem effect;
    public float cooldown;
    public float timeActive;
    public float healSecond;
}

