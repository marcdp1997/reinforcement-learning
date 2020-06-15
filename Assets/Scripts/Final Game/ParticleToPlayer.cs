using UnityEngine;

public class ParticleToPlayer : MonoBehaviour 
{
	private ParticleSystem ps;
	private ParticleSystem.Particle[] particles;

    private Transform target;
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
            Vector3 newDirVec = (target.position - particles[i].position).normalized;
            particles[i].velocity = Vector3.Lerp(particles[i].velocity, newDirVec * velocity, lerpValue);

            if ( Vector3.Distance(target.position, particles[i].position) <= 0.1)
            {
                particles[i].remainingLifetime = 0;
                //ParticleManager.Instance.PlayDNAPickUp();
            }
        }

        if (numParticlesAlive <= 0)
        {
            Destroy(this.gameObject);
        }

        // Apply the particle changes to the particle system
        ps.SetParticles(particles, numParticlesAlive);
    }

    private void InitializeIfNeeded()
	{
		if (ps == null)
            ps = GetComponent<ParticleSystem>();

        if (particles == null || particles.Length < ps.main.maxParticles)
            particles = new ParticleSystem.Particle[ps.main.maxParticles]; 
	}

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
