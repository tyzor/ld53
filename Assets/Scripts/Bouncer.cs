using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    [SerializeField] private float _bounceForce = 100f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other) {
        
        if(other.gameObject.CompareTag("Ball"))
        {
            // Lets take the contact normal of the ball and reverse it to apply a force
            other.rigidbody.AddForce(other.contacts[0].normal * -_bounceForce, ForceMode.Impulse);

            AudioManager.instance.PlaySound(1);

            // Print how many points are colliding with this transform
            Debug.Log("Points colliding: " + other.contacts.Length);
            // Print the normal of the first point in the collision.
            Debug.Log("Normal of the first point: " + other.contacts[0].normal);
            // Draw a different colored ray for every normal in the collision
            foreach (var item in other.contacts)
            {
                Debug.DrawRay(item.point, -item.normal * 100, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
            }
        }
    }

}
