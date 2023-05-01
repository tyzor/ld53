using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : MonoBehaviour
{
    private enum CatState {
        Hungry,
        Eating,
        Shooting,
        Full
    }
    private CatState _state = CatState.Hungry;
    [SerializeField] private float animationSpeed = 2f;
    private Animator _animator;
    [SerializeField] private float eatingTimerDuration = 1f;
    [SerializeField] private float shootingTimerDuration = 1f;
    [SerializeField] private float shootingSpeed = 20f;

    private float _eatingTimer;
    private float _shootingTimer;

    [SerializeField] Bouncer _bouncer;

    public int ballType;
    private Ball _eatenBall;

    public int HungerLevel { get; set; } // Inverse of health 0 means it is full

    public static event Action<Cat> CatFull;

    // Start is called before the first frame update
    void Start()
    {
        _state = CatState.Hungry;
        FacePoint(Vector3.zero);
        _animator = GetComponentInChildren<Animator>();
        DisableBouncer();        
    }
    // Update is called once per frame
    void Update()
    {
        // TODO -- maybe use a switch statement to ensure single state call each frame

        _animator.SetInteger("State", (int)_state);

        switch(_state)
        {
            case CatState.Hungry:

                break;

            case CatState.Eating:
            
                _eatingTimer -= Time.deltaTime;
                if(_eatingTimer <= 0)
                {
                    // If the ball was not the right kind we shoot it out
                    if(_eatenBall.ballType != ballType)
                    {
                        ShootBall();
                    // Correct ball type - cat will eat it
                    } else {
                        _eatenBall.KillBall(1);
                        _eatenBall = null;

                        HungerLevel -= 1;
                        if(HungerLevel <= 0)
                        {
                            // A full cat will be a permanent reflector
                            // TODO -- maybe cat should leave field?
                            EnableBouncer();
                            CatFull?.Invoke(this);
                            _state = CatState.Full;
                        }
                        else
                        {
                            DisableBouncer();
                            _state = CatState.Hungry;
                        }
                    }
                }

                break;
            case CatState.Shooting:
                _shootingTimer -= Time.deltaTime;
                if(_shootingTimer <= 0 || _eatenBall == null)
                {
                    FacePoint(Vector3.zero);
                    _state = CatState.Hungry;
                }
                break;
            case CatState.Full:

                break;
            default:
                break;
        }

        if(_state == CatState.Shooting)
        {
            _shootingTimer -= Time.deltaTime;
            Vector3 endPos = transform.position;
            endPos.y = 0.5f;
            //transform.position = Vector3.Lerp(transform.position, endPos, (eatingTimerDuration - _eatingTimer)*2f/eatingTimerDuration );
            //StartCoroutine(LerpRotation(Quaternion.LookRotation(Vector3.zero) * Quaternion.Euler(60,0,0), _state, animationSpeed));

            if(_shootingTimer <= 0 || _eatenBall == null)
                _state = CatState.Hungry;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(_state == CatState.Hungry && other.CompareTag("Ball"))
        {
            // Cat eats ball
            EatBall(other.GetComponent<Ball>());
        }
    }

    void EatBall(Ball ball) 
    {
        _eatenBall = ball;
        Vector3 pos = transform.position;
        pos.y = ball.transform.position.y;
        ball.transform.position = pos;
        ball.gameObject.SetActive(false);

        _state = CatState.Eating;
        _eatingTimer = eatingTimerDuration;
        EnableBouncer();

        Debug.Log("Cat ate ball");

    }

    void ShootBall()
    {
        _shootingTimer = shootingTimerDuration;        
        
        // Make sure we have no collision
        DisableBouncer();

        // Pick a random direction
        Vector2 new2DDir = UnityEngine.Random.insideUnitCircle;
        Vector3 dir = new Vector3(new2DDir.x, _eatenBall.transform.position.y, new2DDir.y);
        FacePoint(dir * 100f, shootingTimerDuration);
        Rigidbody body = _eatenBall.GetComponent<Rigidbody>();
        body.angularVelocity = Vector3.zero;
        body.velocity = dir * shootingSpeed;
        _eatenBall.gameObject.SetActive(true);
        _eatenBall = null;
        _state = CatState.Shooting;


    }

    void EnableBouncer()
    {
        if(_bouncer != null)
        {
            _bouncer.gameObject.SetActive(true);
        }

    }
    void DisableBouncer()
    {
        if(_bouncer)
        {
            _bouncer.gameObject.SetActive(false);
        }
    }

    public void SetType(int type)
    {
        ballType = type;   
        Material mat = GetComponentInChildren<MeshRenderer>().material;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_Color", BallTypeManager.instance.GetColor(ballType));
        mat.SetColor("_EmissionColor",  BallTypeManager.instance.GetColor(ballType));
    }

        // Turn the cat in a direction on the table, only rotate on y-axis
    void FacePoint(Vector3 target, float duration = 1f)
    {
        transform.rotation = GetFacingRotation(target);
        StartCoroutine(ChangeFacing(GetFacingRotation(target), _state, duration));
        /*
        transform.LookAt(target);
        Quaternion newRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        transform.rotation = newRotation;
        */
    }

    Quaternion GetFacingRotation(Vector3 target)
    {
        Quaternion lookRot = Quaternion.LookRotation(target - transform.position);
        Quaternion newRotation = Quaternion.Euler(0, lookRot.eulerAngles.y, 0);
        return newRotation;
    }

    private IEnumerator ChangeFacing(Quaternion endValue, CatState startState, float duration)
    {
        if(_state != startState)
            yield break;

        float time = 0;
        Quaternion startValue = transform.rotation;
        while(time < duration)
        {
            transform.rotation = Quaternion.Slerp(startValue, endValue, time/duration );
            time += Time.deltaTime;
            yield return null;
        }
        transform.rotation = endValue;
    }

}
