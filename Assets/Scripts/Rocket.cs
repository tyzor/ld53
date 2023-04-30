using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    private bool _isLaunching = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_isLaunching)
        {
            // Go off the screen!
            transform.Translate(transform.up * 10f * Time.deltaTime);

        }
    }

    private void OnTriggerEnter(Collider other) {
        
        Debug.Log("Rocket has triggered!");

        if(other.gameObject.CompareTag("Ball"))
        {
            // TODO -- Check if ball has same goal as this target
            
            // Launch rocket!
            _isLaunching = true;

            // TODO -- animation for loading ball
            



        }

    }
}
