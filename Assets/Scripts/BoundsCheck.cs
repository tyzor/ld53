using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerExit(Collider other)
    {
        // Destroy balls that fall outside the table
        if(other.gameObject.CompareTag("Ball"))
        {
            // TODO -- trigger action here to update interface and failure status etc
            Debug.Log("Ball fell off table!");
            Destroy(other.gameObject);
        }

    }
}
