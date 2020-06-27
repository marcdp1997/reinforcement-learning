using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

// Note that that the detectable tags are different for the blue and red teams. 
// The order is:
// * wall
// * own teammate
// * opposing player
// * opposing bullet

public enum Team { Blue, Red };

public class Player : Agent
{
    public float reward = 0;

    private Rigidbody rBody;
    private List<MeshRenderer> rendererList;
    private Vector3 initPosition;
    private Quaternion initRotation;
    private int currBullets;
    private float currLife;
    private bool damaged;
    private float rateTimer;
    private float shieldCdTimer;
    private float healCdTimer;

    private float existencialPenalty;
    [HideInInspector] public float timePenalty;

    public PlayerUI info;
    public Score score;
    [Space(10)]
    public Team team;
    public Player mate;
    [Space(10)]
    public Collider hitbox;
    public PlayerStats player;
    public ShootingStats shoot;
    public ShieldStats shield;
    public HealStats heal;

    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
        rendererList = new List<MeshRenderer>();
        GetRenderers();

        initPosition = this.transform.position;
        initRotation = this.transform.rotation;

        existencialPenalty = 1f / MaxStep;
    }

    public override void OnEpisodeBegin()
    {
        ResetPlayer();
        score.ResetScore();
    }

    private void Update()
    {
        reward = GetCumulativeReward();
        UpdateAmmunition();
        UpdateAbilities();
    }

    private void ResetPlayer()
    {
        this.transform.position = initPosition;
        this.transform.rotation = initRotation;
        rBody.velocity = Vector3.zero;

        currBullets = 0;
        rateTimer = 0;
        currLife = player.maxLife;
        shieldCdTimer = 0;
        damaged = false;
        timePenalty = 0;

        info.UpdateLifeUI(currLife, player.maxLife);
        info.UpdateBulletsUI(currBullets, player.maxBullets, rateTimer);
        info.UpdateName(gameObject.name);
    }

    public override void CollectObservations(VectorSensor sensor)
    {

    }

    public override void OnActionReceived(float[] vectorAction)
    {
        // Movement
        float forward = 0.0f, right = 0.0f;
        Vector3 rotateDir = Vector3.zero;

        switch (vectorAction[0])
        {
            case 1:
                forward = 1;
                break;
            case 2:
                forward = -1;
                break;
        }

        switch (vectorAction[1])
        {
            case 1:
                right = -1;
                break;
            case 2:
                right = 1;
                break;
        }

        // Rotation
        switch (vectorAction[2])
        {
            case 1:
                rotateDir = transform.up * -1f;
                break;
            case 2:
                rotateDir = transform.up * 1f;
                break;
        }

        Vector3 movement = new Vector3(right, 0, forward);
        movement = movement.normalized * player.speed;
        rBody.velocity = movement;
        Rotate(rotateDir);

        // Shoot
        if (vectorAction[3] == 1.0f && CanShoot())
            Shoot();
    }

    public override void Heuristic(float[] actionsOut)
    {
        // Forward, backwards, no action
        if (Input.GetKey(KeyCode.W))
        {
            actionsOut[0] = 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            actionsOut[0] = 2f;
        }

        // Left, Right, no action
        if (Input.GetKey(KeyCode.A))
        {
            actionsOut[1] = 1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            actionsOut[1] = 2f;
        }

        // Rotate right, left, no action
        if (Input.GetKey(KeyCode.Q))
        {
            actionsOut[2] = 1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            actionsOut[2] = 2f;
        }

        // Shoot, no action
        actionsOut[3] = Input.GetMouseButtonDown(0) ? 1.0f : 0.0f;
    }

    private void Rotate(Vector3 dirToLook)
    {
        if (dirToLook != Vector3.zero)
        {
            transform.Rotate(dirToLook, Time.deltaTime * 100f);
        }
    }

    public void RecieveDamage(Player source, float damage)
    {
        StartCoroutine(VisibleDueToDamage());

        if (currLife - damage <= 0)
        {
            if (source.team == Team.Red) score.AddRedScore();
            if (source.team == Team.Blue) score.AddBlueScore();

            ResetPlayer();
        }
        else currLife -= damage;

        info.UpdateLifeUI(currLife, player.maxLife);
    }

    // ----------------------------------------------------------------------------------
    #region Shooting
    // ----------------------------------------------------------------------------------
    private void UpdateAmmunition()
    {
        if (currBullets < player.maxBullets)
        {
            rateTimer += Time.deltaTime / shoot.rateRecover;
            info.UpdateBulletsUI(currBullets, player.maxBullets, rateTimer);

            if (rateTimer >= 1.0f)
            {
                currBullets++;
                rateTimer = 0;
            }
        }
    }

    private bool CanShoot()
    {
        return currBullets > 0 ? true : false;
    }

    private void Shoot()
    {
        GameObject go = Instantiate(shoot.prefab, shoot.firePoint.position, Quaternion.identity);
        go.GetComponent<Bullet>().Shoot(this, shoot.speed, shoot.firePoint.forward, shoot.timeActive, player.damage);
        currBullets--;
    }
    #endregion
    // ----------------------------------------------------------------------------------
    #region Abilities
    // ----------------------------------------------------------------------------------
    private void UpdateAbilities()
    {
        // Shield
        shieldCdTimer -= Time.deltaTime;
        if (shieldCdTimer < 0) shieldCdTimer = 0;
        else info.UpdateShieldUI(shieldCdTimer, shield.cooldown + shield.timeActive);

        // Heal
        healCdTimer -= Time.deltaTime;
        if (healCdTimer < 0) healCdTimer = 0;
        else info.UpdateHealUI(healCdTimer, heal.cooldown + heal.timeActive);
    }

    private bool CanShield()
    {
        return shieldCdTimer == 0 ? true : false;
    }

    private bool CanHeal()
    {
        return healCdTimer == 0 ? true : false;
    }

    private void Shield(Player target)
    {
        CreateParticles(shield.particles, transform.position, target, Effect.Shield);
        shieldCdTimer = shield.cooldown + shield.timeActive;
    }

    private void Heal(Player target)
    {
        CreateParticles(heal.particles, transform.position, target, Effect.Heal);
        healCdTimer = heal.cooldown + heal.timeActive;
    }

    public void CreateShield(Player source)
    {
        GameObject go = Instantiate(source.shield.prefab, this.transform.position, Quaternion.identity);
        go.GetComponent<Shield>().Use(source.shield.timeActive);
    }

    public void HealOverTime()
    {
        StartCoroutine(HealOverTime(heal.timeActive, heal.healSecond));
    }

    private IEnumerator HealOverTime(float timeActive, float healPerSecond)
    {
        float secondsPassed = 0;
        heal.effect.Play();

        while (secondsPassed < timeActive)
        {
            yield return new WaitForSeconds(1.0f);
            secondsPassed += 1.0f;

            if (currLife + healPerSecond <= player.maxLife) currLife += healPerSecond;
            else currLife = player.maxLife;

            info.UpdateLifeUI(currLife, player.maxLife);
        }

        heal.effect.Stop();
    }

    private void CreateParticles(GameObject prefab, Vector3 initPos, Player target, Effect effect)
    {
        GameObject go = Instantiate(prefab, initPos, Quaternion.identity);
        go.GetComponent<ParticleToPlayer>().SetTarget(this, target, effect);
    }
    #endregion    
    // ----------------------------------------------------------------------------------
    #region Grass
    // ----------------------------------------------------------------------------------
    private void Invisible()
    {
        if (team == Team.Blue) hitbox.gameObject.layer = LayerMask.NameToLayer("Invisible Blue");

        if (team == Team.Red)
        {
            hitbox.gameObject.layer = LayerMask.NameToLayer("Invisible Red");
            SetAllRenderersEnabled(false);
        }
    }

    private void Visible()
    {
        if (team == Team.Blue) hitbox.gameObject.layer = LayerMask.NameToLayer("Team Blue");

        if (team == Team.Red)
        {
            hitbox.gameObject.layer = LayerMask.NameToLayer("Team Red");
            SetAllRenderersEnabled(true);
        }
    }

    private IEnumerator VisibleDueToDamage()
    {
        damaged = true;
        Visible();

        yield return new WaitForSeconds(1.0f);

        damaged = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!damaged && other.gameObject.layer == LayerMask.NameToLayer("Grass"))
        {
            Invisible();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Grass"))
        {
            Visible();
        }
    }
    #endregion
    // ----------------------------------------------------------------------------------
    #region Utils
    // ----------------------------------------------------------------------------------
    private void GetRenderers()
    {
        foreach (MeshRenderer objectRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            rendererList.Add(objectRenderer);
        }
    }

    private void SetAllRenderersEnabled(bool enabled)
    {
        for (int i = 0; i < rendererList.Count; i++)
        {
            rendererList[i].enabled = enabled;
        }
    }
    #endregion
    // ----------------------------------------------------------------------------------
}
