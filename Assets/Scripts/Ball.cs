using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    private Rigidbody _rigidBody;

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
        _rigidBody.AddForce( new Vector3(0,0,-9f), ForceMode.Acceleration ); 

    }
}
