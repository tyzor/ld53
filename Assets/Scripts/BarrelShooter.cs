using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelShooter : MonoBehaviour
{
    enum BarrelState{
        Idle,
        Loaded,
        Cooldown
    }

    private BarrelState _state = BarrelState.Idle;
    [SerializeField] private float spinSpeedIdle = 10f;
    [SerializeField] private float spinSpeedLoaded = 20f;
    [SerializeField] private float firePower = 100f; // Force ball is shot at
    [SerializeField] private float cooldown = 1f; // Number of seconds before barrel can be loaded again
    private float _currentCooldown = 0f;


    private GameObject _loadedBall;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Slowly spin when idle
        if(_state == BarrelState.Idle)
        {
             transform.Rotate ( Vector3.up * ( spinSpeedIdle * Time.deltaTime ) );
        }  
        
        if(_state == BarrelState.Loaded)
        {
            // Still rotate but faster
            transform.Rotate ( Vector3.up * ( spinSpeedLoaded * 2f * Time.deltaTime ) );

            // Check if we are pushing both buttons to launch
            if( (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.L) )
                || (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.RightControl) ) )
            {
                LaunchBall();
            }
        }

        if(_state == BarrelState.Cooldown)
        {
            _currentCooldown -= Time.deltaTime;
            if(_currentCooldown <= 0)
                _state = BarrelState.Idle;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_state == BarrelState.Idle)
        {
            if(other.gameObject.CompareTag("Ball"))
            {
                // Load the ball in this barrel
                _loadedBall = other.gameObject;

                // Set position and disable the object for now
                _loadedBall.transform.position = new Vector3(transform.position.x, _loadedBall.transform.position.y, transform.position.z);
                _loadedBall.SetActive(false);
                _state = BarrelState.Loaded;
            }
        }
    }

    void LaunchBall() {
        if(_loadedBall)
        {
            _loadedBall.SetActive(true);
            _loadedBall.GetComponent<Rigidbody>().velocity = transform.forward * firePower;
            AudioManager.instance.PlaySound(0);
        }

        _currentCooldown = cooldown;
        _state = BarrelState.Cooldown;


    }
}
