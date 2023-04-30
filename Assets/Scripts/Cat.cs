using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : MonoBehaviour
{
    private enum CatState {
        Hungry,
        Eating,
        Shooting
    }
    private CatState _state = CatState.Hungry;
    [SerializeField] private float animationSpeed = 2f;
    [SerializeField] private float eatingTimerDuration = 1f;
    [SerializeField] private float shootingTimerDuration = 1f;
    [SerializeField] private float shootingSpeed = 20f;

    private float _eatingTimer;
    private float _shootingTimer;


    private GameObject _eatenBall;

    // Start is called before the first frame update
    void Start()
    {
        _state = CatState.Hungry;
    }

    // Update is called once per frame
    void Update()
    {
        if(_state == CatState.Hungry)
        {
            transform.LookAt(Vector3.zero);
            // Open mouth
            if(transform.position.y < .5f)
            {
                transform.Translate(transform.up * animationSpeed * Time.deltaTime);
            }
        }

        if(_state == CatState.Eating)
        {
            _eatingTimer -= Time.deltaTime;
            Vector3 endPos = transform.position;
            endPos.y = 0;
            transform.position = Vector3.Lerp(transform.position, endPos, (eatingTimerDuration - _eatingTimer)/eatingTimerDuration );

            // TODO -- check if the ball was the right kind of food for this cat
            if(_eatingTimer <= 0)
            {
                _shootingTimer = shootingTimerDuration;
                _state = CatState.Shooting;
            }
        }

        if(_state == CatState.Shooting)
        {
            _shootingTimer -= Time.deltaTime;
            Vector3 endPos = transform.position;
            endPos.y = 0.5f;
            transform.position = Vector3.Lerp(transform.position, endPos, (eatingTimerDuration - _eatingTimer)*2f/eatingTimerDuration );
            
            if(_eatenBall && transform.position.y == endPos.y)
                ShootBall();

            if(_shootingTimer <= 0)
                _state = CatState.Hungry;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(_state == CatState.Hungry && other.CompareTag("Ball"))
        {
            // Cat eats ball
            EatBall(other.gameObject);
            _state = CatState.Eating;
            _eatingTimer = eatingTimerDuration;
        }
    }

    void EatBall(GameObject ball) 
    {
        _eatenBall = ball;
        Vector3 pos = transform.position;
        pos.y = ball.transform.position.y;
        ball.transform.position = pos;
        ball.SetActive(false);
        Debug.Log("Cat ate ball");

    }

    void ShootBall()
    {
        // Pick a random direction
        Vector2 new2DDir = Random.insideUnitCircle;
        Vector3 dir = new Vector3(new2DDir.x, _eatenBall.transform.position.y, new2DDir.y);
        Rigidbody body = _eatenBall.GetComponent<Rigidbody>();
        body.angularVelocity = Vector3.zero;
        body.velocity = dir * shootingSpeed;
        _eatenBall.SetActive(true);

        _eatenBall = null;


    }

}
