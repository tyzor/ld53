using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelShooter : MonoBehaviour
{
    enum BarrelState{
        Idle,
        Loaded
    }

    private BarrelState _state = BarrelState.Idle;
    [SerializeField] float spinSpeedIdle = 10f;
    [SerializeField] float spinSpeedLoaded = 20f;


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
        } else if(_state == BarrelState.Loaded)
        {
            // Still rotate but faster
            transform.Rotate ( Vector3.up * ( spinSpeedLoaded * 2f * Time.deltaTime ) );

            // Check if we are pushing both buttons to launch
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
}
