using UnityEngine;
using System.Collections.Generic;

public class BulletPool : MonoBehaviour
{
    private List<Bullet> blueBullets;
    private List<Bullet> redBullets;

    [SerializeField] private GameObject redBulletPrefab;
    [SerializeField] private GameObject blueBulletPrefab;
    [SerializeField] private int pooledBullets;

    private void Awake()
    {
        blueBullets = new List<Bullet>();
        redBullets = new List<Bullet>();
    }

    private void Start()
    {
        for (int i = 0; i < pooledBullets / 2; i++)
        {
            GameObject go = Instantiate(redBulletPrefab, Vector3.zero, Quaternion.identity);
            go.SetActive(false);

            Bullet bullet = go.GetComponent<Bullet>();
            redBullets.Add(bullet);
        }

        for (int i = 0; i < pooledBullets / 2; i++)
        {
            GameObject go = Instantiate(blueBulletPrefab, Vector3.zero, Quaternion.identity);
            go.SetActive(false);

            Bullet bullet = go.GetComponent<Bullet>();
            blueBullets.Add(bullet);
        }
    }

    public Bullet GetBlueBullet()
    {
        for (int i = 0; i < blueBullets.Count; i++)
        {
            if (!blueBullets[i].gameObject.activeInHierarchy)
                return blueBullets[i];
        }

        return null;
    }

    public Bullet GetRedBullet()
    {
        for (int i = 0; i < redBullets.Count; i++)
        {
            if (!redBullets[i].gameObject.activeInHierarchy)
                return redBullets[i];
        }

        return null;
    }

    public void ResetBullets()
    {
        for (int i = 0; i < blueBullets.Count; i++)
        {
            blueBullets[i].ResetBullet();
        }

        for (int i = 0; i < redBullets.Count; i++)
        {
            redBullets[i].ResetBullet();
        }
    }
}
