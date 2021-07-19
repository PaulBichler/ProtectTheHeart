using GMTK;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    public Vector2 _spawnFieldYZ;
    public float _minSpawnInterval;
    public float _maxSpawnInterval;
    private GameObject _projectile;
    private float currentInterval;

    private float currentTimer;


    private void Awake()
    {
        _projectile = Resources.Load<GameObject>("Prefabs/Projectiles/Purple");
    }

    private void Reset()
    {
        currentInterval = Mathf.Lerp(_minSpawnInterval, _maxSpawnInterval, Random.value);
        currentTimer = 0;
    }

    private void Update()
    {
        currentTimer += Time.deltaTime;

        if (currentTimer >= currentInterval)
        {
            Spawn();
            Reset();
        }
    }

    private void Spawn()
    {
        GameObject instantiate = Instantiate(_projectile, transform);
        instantiate.transform.localPosition = new Vector3(0,
            Mathf.Lerp(-_spawnFieldYZ.x, _spawnFieldYZ.x, Random.value),
            Mathf.Lerp(-_spawnFieldYZ.y, _spawnFieldYZ.y, Random.value));
        instantiate.GetComponent<Collider>().enabled = false;
        instantiate.GetComponent<ProjectileMove>().AddForce(instantiate.transform.forward);
    }
}