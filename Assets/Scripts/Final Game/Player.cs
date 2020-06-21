using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public enum Team { Blue, Red };

public class Player : Agent
{
    public float reward;

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
        RequestDecision();
    }

    private void ResetPlayer()
    {
        this.transform.position = initPosition;
        this.transform.rotation = initRotation;

        currBullets = 3;
        rateTimer = 0;
        currLife = player.maxLife;
        shieldCdTimer = 0;
        damaged = false;

        info.UpdateLifeUI(currLife, player.maxLife);
        info.UpdateBulletsUI(currBullets, rateTimer);
        info.UpdateName(gameObject.name);
    }

    private void Rotate(Vector3 movement)
    {
        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(movement), 5 * Time.deltaTime);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observations
        sensor.AddObservation(this.transform.position);
        sensor.AddObservation(currLife);
        sensor.AddObservation(currBullets);
        sensor.AddObservation(healCdTimer);
        sensor.AddObservation(shieldCdTimer);
        sensor.AddObservation(rBody.velocity);

        if (mate != null)
        {
            sensor.AddObservation(mate.transform.position);
            sensor.AddObservation(mate.currLife);
            sensor.AddObservation(mate.currBullets);
            sensor.AddObservation(mate.healCdTimer);
            sensor.AddObservation(mate.shieldCdTimer);
        }
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        // Movement
        Vector3 movement = new Vector3(vectorAction[0], 0, vectorAction[1]);
        movement = movement.normalized * player.speed;
        rBody.velocity = movement;
        Rotate(movement);

        // Shoot
        if (vectorAction[2] == 1.0f && CanShoot()) Shoot();

        // Shield
        if (vectorAction[3] == 1.0f && CanShield()) Shield(this);
        if (vectorAction[4] == 1.0f && CanShield() && mate != null) Shield(mate);

        // Heal
        if (vectorAction[5] == 1.0f && CanHeal()) Heal(this);
        if (vectorAction[6] == 1.0f && CanHeal() && mate != null) Heal(mate);     
    }

    public override void Heuristic(float[] actionsOut)
    {
        // Forward, backwards, no action
        actionsOut[0] = Input.GetAxis("Horizontal");

        // Left, Right, no action
        actionsOut[1] = Input.GetAxis("Vertical");

        // Shoot, no action
        actionsOut[2] = Input.GetMouseButtonDown(0) ? 1.0f : 0.0f;

        // Use ability, no action (x4)
        actionsOut[3] = Input.GetKeyDown(KeyCode.O) ? 1.0f : 0.0f;
        actionsOut[4] = Input.GetKeyDown(KeyCode.P) ? 1.0f : 0.0f;
        actionsOut[5] = Input.GetKeyDown(KeyCode.K) ? 1.0f : 0.0f;
        actionsOut[6] = Input.GetKeyDown(KeyCode.L) ? 1.0f : 0.0f;
    }

    public void RecieveDamage(float damage)
    {
        StartCoroutine(VisibleDueToDamage());

        if (currLife - damage <= 0)
        {
            this.SetReward(-10.0f);
            mate.SetReward(-10.0f);

            if (team == Team.Blue) score.AddRedScore();
            if (team == Team.Red) score.AddBlueScore();

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
