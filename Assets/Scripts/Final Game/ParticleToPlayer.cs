using UnityEngine;

public enum Effect { Shield, Heal }

public class ParticleToPlayer : MonoBehaviour 
{
	private ParticleSystem ps;
	private ParticleSystem.Particle[] particles;

    private Player source;
    private Player target;
    private Effect effect;
    private bool effectEnabled;

    [SerializeField] private float delayTime;
    [SerializeField] private float velocity;
    [SerializeField] private float lerpValue;

    private void Start()
	{
        effectEnabled = false;

		InitializeIfNeeded();
        Invoke("InvokeStart", delayTime);
    }

    private void InvokeStart()
    {
        effectEnabled = true;
    }

    private void FixedUpdate()
    {
        if (!effectEnabled) return;

        MoveToTarget();
    }

    private void MoveToTarget()
    {
        int numParticlesAlive = ps.GetParticles(particles);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            Vector3 newDirVec = (target.gameObject.transform.position - particles[i].position).normalized;
            particles[i].velocity = Vector3.Lerp(particles[i].velocity, newDirVec * velocity, lerpValue);

            if (Vector3.Distance(target.gameObject.transform.position, particles[i].position) <= 0.1)
            {
                particles[i].remainingLifetime = 0;
            }
        }

        if (numParticlesAlive <= 0)
        {
            if (effect == Effect.Heal) 
                target.HealOverTime();
            if (effect == Effect.Shield)
                target.CreateShield(source);
                
            Destroy(this.gameObject);
        }

        ps.SetParticles(particles, numParticlesAlive);
    }

    private void InitializeIfNeeded()
	{
		if (ps == null)
            ps = GetComponent<ParticleSystem>();

        if (particles == null || particles.Length < ps.main.maxParticles)
            particles = new ParticleSystem.Particle[ps.main.maxParticles]; 
	}

    public void SetTarget(Player source, Player target, Effect effect)
    {
        this.source = source;
        this.target = target;
        this.effect = effect;
    }
}
