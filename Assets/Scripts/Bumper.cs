using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        transform.LookAt(Vector3.zero);
        _animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        
        if(other.CompareTag("Ball"))
        {
            Vector3 otherPos = other.transform.position;
            otherPos.y = transform.position.y;
            transform.LookAt(otherPos);
            _animator.Play("BumperSwat");
        }

    }
}
