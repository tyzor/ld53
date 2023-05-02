using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableSpawner : MonoBehaviour
{
    public enum ObstacleType {
        Empty,
        Cat,
        Bumper,
        Barrel
        // TODO -- add more types!
    }

    // List of what can be spawned at this point
    public List<ObstacleType> spawnableTypes;

    // Prefab links
    [SerializeField] private Cat catPrefab;
    [SerializeField] private BarrelShooter barrelPrefab;
    [SerializeField] private Bumper bumperPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure we have at least a default
        if(spawnableTypes.Count == 0)
        {
            spawnableTypes.Add(ObstacleType.Empty);
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanSpawn(ObstacleType type)
    {
        return spawnableTypes.Contains(type);
    }

    public ObstacleType GetRandomPossibleSpawn()
    {
        return spawnableTypes[Random.Range(0,spawnableTypes.Count)];
    }

    public T Spawn<T>(ObstacleType type) where T : class
    {
        if(type == ObstacleType.Cat)
        {
            return Instantiate<Cat>(catPrefab, transform.position, Quaternion.identity) as T;
        }
        else if (type == ObstacleType.Barrel)
        {
            return Instantiate<BarrelShooter>(barrelPrefab, transform.position, Quaternion.identity) as T;
        }
        else if (type == ObstacleType.Bumper)
        {
            return Instantiate<Bumper>(bumperPrefab, transform.position, Quaternion.identity) as T;
        } else {
            // unknown or empty -- do nothing
            return default;
        }
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 3);
    }

}
