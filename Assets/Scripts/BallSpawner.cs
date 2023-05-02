using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// TODO -- use a fixed amount of ball pools and just move/reactive them

public class BallSpawner : MonoBehaviour
{
    [SerializeField] private Ball ballPrefab;

    [SerializeField] private float spawningTime = 1f; // How long before ball becomes active
    private float _spawnTimer;
    private Ball _currentBall;

    enum SpawnerState {
        Idle,
        Spawning
    }
    private SpawnerState _state = SpawnerState.Idle;

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull<Ball>(ballPrefab);
        _state = SpawnerState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        // FIX for table resets -- sometimes a table can reset and clear a ball
        // right as spawner still has reference, we reset to Idle in this case
        if(_currentBall == null)
            _state = SpawnerState.Idle;


        if(_state == SpawnerState.Spawning)
        {
            _spawnTimer -= Time.deltaTime;
            if(_spawnTimer <= 0f)
            {
                // Spawning is done make ball active!
                _currentBall.GetComponent<Rigidbody>().isKinematic = false;
                _state = SpawnerState.Idle;
                _currentBall = null;

            }
        }
    }

    public Ball SpawnBall(int type)
    {
        if(_currentBall != null)
        {   
            Debug.Log("Already spawning ball, aborting...");
            return null;
        }

        // We place ball slightly above table
        Vector3 spawnPos = transform.position;
        spawnPos.y += (ballPrefab.GetComponent<SphereCollider>().radius) + 0.01f;
        _currentBall = Instantiate<Ball>(ballPrefab, spawnPos, Quaternion.identity);
        _currentBall.SetType(type);
        _currentBall.GetComponent<Rigidbody>().isKinematic = true;

        _spawnTimer = spawningTime;
        _state = SpawnerState.Spawning;

        return _currentBall;
    }


    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2);
    }


}
