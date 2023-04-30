using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipper : MonoBehaviour
{
    [SerializeField] private float _velocity = 1000f;
    [SerializeField] private float _force = 1000f;
    [SerializeField] private float _reverseForce = 300f; // speed at which flipper will reset
    [SerializeField] private bool _clockwise = false;
    private float _targetVel => _clockwise ? _velocity : _velocity * -1f;

    private HingeJoint _hinge;

    // Start is called before the first frame update
    void Start()
    {
        _hinge = GetComponent<HingeJoint>();
        JointMotor motor = _hinge.motor;
        motor.force = _force;
        motor.freeSpin = true;
        _hinge.motor = motor;
        _hinge.useMotor = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        bool bIsFlipping = _clockwise ? Input.GetKey(KeyCode.RightControl) : Input.GetKey(KeyCode.LeftControl);
        var motor = _hinge.motor;
        if(bIsFlipping)
        {            
            motor.targetVelocity = _targetVel;
            motor.force = _force;
        } else {
            motor.targetVelocity = _targetVel * -1f;
            motor.force = _reverseForce; // half force when returning
        }
        _hinge.motor = motor;
    }
}
