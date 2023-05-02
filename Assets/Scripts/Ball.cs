using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{


    private Rigidbody _rigidBody;
    [SerializeField] private Vector3 _gravity;

    public static event Action<Ball, int> BallDied;

    public int ballType;

    private static float maxVelocitySqr = 100f*100f;
    private static float highestSeenVelocitySqr = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();   
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate() {

        // Apply constant gravitational force to bottom of table
        _rigidBody.AddForce( _gravity, ForceMode.Acceleration ); 


        /*
        if(_rigidBody.velocity.sqrMagnitude > highestSeenVelocitySqr)
        {
            highestSeenVelocitySqr = _rigidBody.velocity.sqrMagnitude;
            Debug.Log($"Highest seen velocity - {highestSeenVelocitySqr}");
        }
        */


        // Trying out a velocity limiter
        if(_rigidBody.velocity.sqrMagnitude > maxVelocitySqr)
        {
            Debug.Log("Ball exceeded max velocity!");
            // Safest way of doing this is probably applying a negative force (adding drag)
            _rigidBody.drag = 20;
        } else {
            _rigidBody.drag = 0;
        }
    }

    // Remove a ball from the game - the way tells us how to handle it
    // 0 - ball fell off table (through bottom or a glitch?)
    // 1 - ball was eaten (good outcome)
    public void KillBall(int way) {
        // Alert any listeners that the ball died
        BallDied?.Invoke(this, way); 
        Destroy(gameObject);
    }

    public void SetType(int type)
    {
        ballType = type;   
        Material mat = GetComponent<MeshRenderer>().material;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_BaseColor", BallTypeManager.instance.GetColor(ballType));
        mat.SetColor("_EmissionColor",  BallTypeManager.instance.GetColor(ballType) * .5f);
    }

}
