using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class Player : Agent
{
    private Rigidbody rBody;
    private int currBullets;
    private float currLife;

    private float rateTimer;
    private float shieldCooldownTimer;
    private float healCooldownTimer;

    public Transform mate;
    public PlayerStats player;
    public ShootingStats shoot;
    public ShieldStats shield;
    public HealStats heal;

    [Header("User Interface")]
    public Image lifeBar;
    public Image healBar;
    public Image shieldBar;
    public Image bulletsBar;
    public Text lifeText;

    private void Awake()
    {
        rBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        currBullets = 3;
        rateTimer = 0;
        currLife = player.maxLife;
        shieldCooldownTimer = 0;

        UpdateLifeUI();
        UpdateBulletsUI();
    }

    private void Update()
    {
        UpdateAmmunition();
        UpdateAbilities();
        RequestDecision();
    }

    public override void AgentAction(float[] vectorAction)
    {
        // Movement
        var x = vectorAction[0];
        var z = vectorAction[1];
        rBody.AddForce(new Vector3(x * player.speed, 0, z * player.speed));

        // Shooting
        var shootAttempt = vectorAction[2];
        if (shootAttempt == 1.0f && CanShoot()) Shoot();

        // Abilities
        var useShieldOnHimself = vectorAction[3];
        if (useShieldOnHimself == 1.0f && CanUseShield()) UseShield(transform.position);

        var useShieldOnMate = vectorAction[4];
        if (useShieldOnMate == 1.0f && CanUseShield()) UseShield(mate.position);
    }

    public override float[] Heuristic()
    {
        var action = new float[5];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        action[2] = Input.GetMouseButtonDown(0) ? 1.0f : 0.0f;
        action[3] = Input.GetKeyDown(KeyCode.O) ? 1.0f : 0.0f;
        action[4] = Input.GetKeyDown(KeyCode.P) ? 1.0f : 0.0f;

        return action;
    }

    private void UpdateAmmunition()
    {
        if (currBullets < 3)
        {
            rateTimer += Time.deltaTime / shoot.rateRecover;
            UpdateBulletsUI();

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
        shieldCooldownTimer -= Time.deltaTime;
        if (shieldCooldownTimer < 0) shieldCooldownTimer = 0;
        else UpdateShieldUI();

        // Heal
        healCooldownTimer -= Time.deltaTime;
        if (healCooldownTimer < 0) healCooldownTimer = 0;
        else UpdateHealUI();
    }

    private bool CanUseShield()
    {
        return shieldCooldownTimer == 0 ? true : false;
    }

    private bool CanShoot()
    {
        return currBullets > 0 ? true : false;
    }

    private void Shoot()
    {
        Bullet temp = Instantiate(shoot.prefab, shoot.firePoint.position, Quaternion.identity).GetComponent<Bullet>();
        temp.Shoot(shoot.speed, transform.forward, shoot.timeActive, player.damage);
        currBullets--;
    }

    private void UseShield(Vector3 position)
    {
        Shield temp = Instantiate(shield.prefab, position, Quaternion.identity).GetComponent<Shield>();
        temp.Use(shield.timeActive);
        shieldCooldownTimer = shield.cooldown;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bullet Red") ||
            other.gameObject.layer == LayerMask.NameToLayer("Bullet Blue"))
        {
            Bullet bullet = other.gameObject.GetComponent<Bullet>();

            if (currLife - bullet.damage >= 0) currLife -= bullet.damage;
            else currLife = 0;

            UpdateLifeUI();
            Destroy(other.gameObject);
        }
    }

    private void UpdateLifeUI()
    {
        lifeText.text = currLife.ToString();
        lifeBar.fillAmount = currLife / player.maxLife;
    }

    private void UpdateBulletsUI()
    {
        bulletsBar.fillAmount = (currBullets + rateTimer) / 3;
    }

    private void UpdateShieldUI()
    {
        shieldBar.fillAmount = 1 - shieldCooldownTimer / shield.cooldown;
    }

    private void UpdateHealUI()
    {
        healBar.fillAmount = 1 - healCooldownTimer / heal.cooldown;
    }
}
