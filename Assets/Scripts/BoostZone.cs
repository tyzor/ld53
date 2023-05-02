using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostZone : MonoBehaviour
{
    [SerializeField] float boostPower = 50f;

    private void OnTriggerStay(Collider other) {

        if(other.CompareTag("Ball"))
        {
            other.attachedRigidbody.AddForce(transform.forward * boostPower, ForceMode.Acceleration);
        }

    }

 
}
