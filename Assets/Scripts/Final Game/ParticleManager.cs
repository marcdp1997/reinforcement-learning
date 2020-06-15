using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private GameObject healPS;

    #region Singleton
    private static ParticleManager _Instance;
    public static ParticleManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                Debug.LogError("Attempting to access Particle Manager before it's been created.");
            }

            return _Instance;
        }
    }
    #endregion

    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(gameObject);
        }

        _Instance = this;
    }

    public void CreateHealPS(Vector3 initPos, Transform target)
    {
        GameObject heal = Instantiate(healPS, initPos, Quaternion.identity);
        heal.GetComponent<ParticleToPlayer>().SetTarget(target);
    }
}
