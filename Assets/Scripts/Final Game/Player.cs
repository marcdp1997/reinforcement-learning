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

    private Shield shieldBH;
    private Vector3 initPosition;
    private Quaternion initRotation;
    private Rigidbody rBody;
    private int currBullets;
    private float currLife;
    private float rateTimer;

    public PlayerUI info;
    public Score score;
    [Space(10)]
    public Team team;
    public Player mate;
    [Space(10)]
    public PlayerStats player;
    public ShootingStats shoot;
    //public ShieldStats shield;
    public HealStats heal;

    private void Awake()
    {
        rBody = GetComponent<Rigidbody>();
        //shieldBH = Instantiate(shield.prefab, this.transform.position, Quaternion.identity).GetComponent<Shield>();

        initPosition = this.transform.position;
        initRotation = this.transform.rotation;
    }

    public override void OnEpisodeBegin()
    {
        ResetPlayer();
        score.ResetScore();
        shoot.pool.ResetBullets();
    }

    private void FixedUpdate()
    {
        UpdateAmmunition();
        //UpdateShieldPosition();
    }

    private void ResetPlayer()
    {
        this.transform.position = initPosition;
        this.transform.rotation = initRotation;
        rBody.velocity = Vector3.zero;

        currBullets = 0;
        rateTimer = 0;
        currLife = player.maxLife;

        info.UpdateLifeUI(currLife, player.maxLife);
        info.UpdateBulletsUI(currBullets, player.maxBullets, rateTimer);
        info.UpdateName(gameObject.name);
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

    public void RecieveDamage(Player source)
    {
        // Hitting enemy team (damage)
        if (source.team == Team.Blue && team == Team.Red ||
            source.team == Team.Red && team == Team.Blue)
        {
            float damage = source.shoot.damage;

            if (currLife - damage <= 0)
            {
                if (source.team == Team.Red) score.AddRedScore();
                if (source.team == Team.Blue) score.AddBlueScore();

                ResetPlayer();
            }
            else currLife -= damage;
        }

        // Hitting teammate(heal)
        else
        {
            float damage = source.heal.damage;

            if (currLife + damage <= player.maxLife)
            {
                currLife += damage;
            }
            else currLife = player.maxLife;
        }

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
        Bullet bullet;

        if (team == Team.Blue) bullet = shoot.pool.GetBlueBullet();
        else bullet = shoot.pool.GetRedBullet();

        if (bullet != null)
        {
            bullet.gameObject.SetActive(true);
            bullet.transform.position = shoot.firePoint.position;
            bullet.Shoot(this, shoot.speed, shoot.firePoint.forward, shoot.timeActive);

            currBullets--;
        }
    }
    #endregion
    // ----------------------------------------------------------------------------------
    #region Shield
    // ----------------------------------------------------------------------------------
    private void UpdateShieldPosition()
    {
        if (shieldBH.gameObject.activeSelf)
            shieldBH.transform.position = transform.position;
    }
    #endregion    
    // ----------------------------------------------------------------------------------
}
