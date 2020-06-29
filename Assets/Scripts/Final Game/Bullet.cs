using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rBody;
    private Player source;

    private void Awake()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public void Shoot(Player source, float bulletSpeed, Vector3 direction, float timeActive)
    {
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
            other.gameObject.layer == LayerMask.NameToLayer("Team Blue"))
        {
            if (other != source.player.hitbox)
            {
                other.GetComponentInParent<Player>().RecieveDamage(source);
                Destroy(this.gameObject);
            }
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Wall") ||
            other.gameObject.layer == LayerMask.NameToLayer("Shield Red") ||
            other.gameObject.layer == LayerMask.NameToLayer("Shield Blue"))
            Destroy(this.gameObject);
    }
}
