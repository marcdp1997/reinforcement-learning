using System.Collections;
using Unity.MLAgents;
using UnityEngine;

public class Player : Agent
{
    private Rigidbody rBody;
    private int currBullets;
    private float currLife;
    private bool healing;

    private float rateTimer;
    private float shieldCdTimer;
    private float healCdTimer;

    public PlayerUI info;
    public Transform rotationRoot;
    public Player mate;
    public PlayerStats player;
    public ShootingStats shoot;
    public ShieldStats shield;
    public HealStats heal;

    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        currBullets = 3;
        rateTimer = 0;
        currLife = player.maxLife;
        shieldCdTimer = 0;
        healing = false;

        info.UpdateLifeUI(currLife, player.maxLife);
        info.UpdateBulletsUI(currBullets, rateTimer);
        info.UpdateName(gameObject.name);
    }

    private void Update()
    {
        UpdateAmmunition();
        UpdateAbilities();
        RequestDecision();
    }

    public virtual void Rotate(Vector3 movement)
    {
        if (movement != Vector3.zero)
        {
            rotationRoot.transform.rotation = Quaternion.Lerp(rotationRoot.transform.rotation, Quaternion.LookRotation(movement), 10 * Time.deltaTime);
        }
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        // Movement
        var x = vectorAction[0];
        var z = vectorAction[1];
        Vector3 movement = new Vector3(x * player.speed, 0, z * player.speed);
        rBody.AddForce(movement);
        Rotate(movement);

        // Shoot
        if (vectorAction[2] == 1.0f && CanShoot()) Shoot();

        // Shield
        if (vectorAction[3] == 1.0f && CanShield()) Shield(this);
        if (vectorAction[4] == 1.0f && CanShield()) Shield(mate);

        // Heal
        if (vectorAction[5] == 1.0f && CanHeal()) Heal(this);
        if (vectorAction[6] == 1.0f && CanHeal()) Heal(mate);
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
        actionsOut[2] = Input.GetMouseButtonDown(0) ? 1.0f : 0.0f;
        actionsOut[3] = Input.GetKeyDown(KeyCode.O) ? 1.0f : 0.0f;
        actionsOut[4] = Input.GetKeyDown(KeyCode.P) ? 1.0f : 0.0f;
        actionsOut[5] = Input.GetKeyDown(KeyCode.K) ? 1.0f : 0.0f;
        actionsOut[6] = Input.GetKeyDown(KeyCode.L) ? 1.0f : 0.0f;
    }

    private void UpdateAmmunition()
    {
        if (currBullets < 3)
        {
            rateTimer += Time.deltaTime / shoot.rateRecover;
            info.UpdateBulletsUI(currBullets, rateTimer);

            if (rateTimer >= 1.0f)
            {
                currBullets++;
                rateTimer = 0;
            }
        }
    }

    private void UpdateAbilities()
    {
        // Shield
        shieldCdTimer -= Time.deltaTime;
        if (shieldCdTimer < 0) shieldCdTimer = 0;
        else info.UpdateShieldUI(shieldCdTimer, shield.cooldown);

        // Heal
        if (!healing) healCdTimer -= Time.deltaTime;
        if (healCdTimer < 0) healCdTimer = 0;
        else info.UpdateHealUI(healCdTimer, heal.cooldown);
    }

    private bool CanShield()
    {
        return shieldCdTimer == 0 ? true : false;
    }

    private bool CanHeal()
    {
        return healCdTimer == 0 ? true : false;
    }

    private bool CanShoot()
    {
        return currBullets > 0 ? true : false;
    }

    private void Shoot()
    {
        GameObject go = Instantiate(shoot.prefab, shoot.firePoint.position, Quaternion.identity);
        go.GetComponent<Bullet>().Shoot(shoot.speed, shoot.firePoint.forward, shoot.timeActive, player.damage);
        currBullets--;
    }

    private void Shield(Player target)
    {
        GameObject go = Instantiate(shield.prefab, target.gameObject.transform.position, Quaternion.identity);
        go.GetComponent<Shield>().Use(shield.timeActive);
        shieldCdTimer = shield.cooldown;
    }

    private void Heal(Player target)
    {
        StartCoroutine(HealOverTime(heal.timeActive, heal.healSecond));
        ParticleManager.Instance.CreateHealPS(transform.position, target.gameObject.transform);
        healCdTimer = heal.cooldown;
    }

    private IEnumerator HealOverTime(float timeActive, float healPerSecond)
    {
        float secondsPassed = 0;
        healing = true;

        while (healing && secondsPassed < timeActive)
        {
            yield return new WaitForSeconds(1.0f);
            secondsPassed += 1.0f;

            if (currLife + healPerSecond <= player.maxLife) currLife += healPerSecond;
            else currLife = player.maxLife;

            info.UpdateLifeUI(currLife, player.maxLife);
        }

        healing = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bullet Red") ||
            other.gameObject.layer == LayerMask.NameToLayer("Bullet Blue"))
        {
            healing = false;
            Bullet bullet = other.gameObject.GetComponent<Bullet>();

            if (currLife - bullet.damage >= 0) currLife -= bullet.damage;
            else currLife = 0;

            info.UpdateLifeUI(currLife, player.maxLife);
            Destroy(other.gameObject);
        }
    }
}
