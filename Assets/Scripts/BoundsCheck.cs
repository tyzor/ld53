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

            // Unity is stupid and fires triggers when kinematic mode is toggled
            // so we need to double check that the object is actually outside the bounds
            if(BoundsContains(other.GetComponent<MeshRenderer>().bounds))
            {
                Debug.Log("Detected false trigger exit!");
                return;               
            }

            // TODO -- trigger action here to update interface and failure status etc
            Debug.Log("Ball fell off table!");
            other.GetComponent<Ball>().KillBall(0);
        }

    }

    bool BoundsContains(Bounds bounds1)
    {
        Bounds bounds2 = GetComponent<BoxCollider>().bounds;

        // Check if all eight corners of bounds1 are inside bounds2
        if (bounds2.Contains(bounds1.min) &&
            bounds2.Contains(bounds1.max) &&
            bounds2.Contains(new Vector3(bounds1.min.x, bounds1.min.y, bounds1.max.z)) &&
            bounds2.Contains(new Vector3(bounds1.min.x, bounds1.max.y, bounds1.min.z)) &&
            bounds2.Contains(new Vector3(bounds1.min.x, bounds1.max.y, bounds1.max.z)) &&
            bounds2.Contains(new Vector3(bounds1.max.x, bounds1.min.y, bounds1.min.z)) &&
            bounds2.Contains(new Vector3(bounds1.max.x, bounds1.min.y, bounds1.max.z)) &&
            bounds2.Contains(new Vector3(bounds1.max.x, bounds1.max.y, bounds1.min.z)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
