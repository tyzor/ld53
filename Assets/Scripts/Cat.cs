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
    private Animator _animator;
    [SerializeField] private float eatingTimerDuration = 1f;
    [SerializeField] private float shootingTimerDuration = 1f;
    [SerializeField] private float shootingSpeed = 20f;
    [SerializeField] private float shotCooldownTimerDuration = 1f;

    [SerializeField] Bouncer _bouncer;

    public int ballType;
    private Ball _eatenBall;

    public int HungerLevel { get; set; } // Inverse of health 0 means it is full

    public static event Action<Cat> CatFull;

    private Coroutine _facingRoutine;

    [SerializeField] MeshRenderer catBodyRenderer;
    

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

    }

    private void OnTriggerEnter(Collider other)
    {
        if(_state == CatState.Hungry && other.CompareTag("Ball") && _eatenBall == null)
        {
            // Cat eats ball
            EatBall(other.GetComponent<Ball>());
        }
    }

    private IEnumerator RunEatingState()
    {
        _state = CatState.Eating;
        EnableBouncer();

        yield return new WaitForSeconds(eatingTimerDuration);
        // If the ball was not the right kind we shoot it out
        if(_eatenBall.ballType != ballType)
        {
            StartCoroutine(RunShootingState());
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
                SetType(-1);
                _state = CatState.Full;
            }
            else
            {
                DisableBouncer();
                FacePoint(Vector3.zero);
                _state = CatState.Hungry;
            }
        }
    }

    private IEnumerator RunShootingState() {
        
        // Make sure we have no collision
        DisableBouncer();

        // Pick a random direction
        Vector2 new2DDir = UnityEngine.Random.insideUnitCircle;
        Vector3 dir = new Vector3(new2DDir.x, _eatenBall.transform.position.y, new2DDir.y);
        FacePoint(dir * 100f);

        // Set ball up but don't shoot yet
        Rigidbody body = _eatenBall.GetComponent<Rigidbody>();
        body.angularVelocity = Vector3.zero;
        body.velocity = dir.normalized * shootingSpeed;

        _state = CatState.Shooting;

        yield return new WaitForSeconds(shootingTimerDuration);
        
        if( _eatenBall != null )
        {
            // release ball
            _eatenBall.gameObject.SetActive(true);
            _eatenBall = null;

            // give ball enough time so it's not re-eaten
            yield return new WaitForSeconds(shotCooldownTimerDuration);

            FacePoint(Vector3.zero);
            _state = CatState.Hungry;
            
        }
    }


    void EatBall(Ball ball) 
    {
        _eatenBall = ball;
        Vector3 pos = transform.position;
        pos.y = ball.transform.position.y;
        ball.transform.position = pos;
        ball.gameObject.SetActive(false);

        StartCoroutine(RunEatingState());

    }

    void ShootBall()
    {
        StartCoroutine(RunShootingState());    
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
        Material mat = catBodyRenderer.material;
        Debug.Log(mat.name);
        if(mat == null)
            return;

        Color color;
        if(type >= 0)
        {
            color = BallTypeManager.instance.GetColor(ballType);
            //mat.EnableKeyword("_EMISSION");
            mat.SetColor("_BaseColor", color);
            //mat.SetColor("_EmissionColor", color);
        }else {
            color = Color.HSVToRGB(0,0,0.26f); // Cat is full we make them non-highlighted
            mat.DisableKeyword("_EMISSION");
            mat.SetColor("_BaseColor", color);
            mat.SetColor("_EmissionColor", color);
        }

    }

        // Turn the cat in a direction on the table, only rotate on y-axis
    void FacePoint(Vector3 target, float duration = 1f)
    {
        if(_facingRoutine != null)
        {
            StopCoroutine(_facingRoutine);
        }

        _facingRoutine = StartCoroutine(ChangeFacing(GetFacingRotation(target), _state, duration));
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

    void OnDrawGizmos()
    {
        Color color;
        switch(_state)
        {
            case CatState.Hungry:
                color = Color.green;
                break;
            case CatState.Eating:
                color = Color.red;
                break;
            case CatState.Shooting:
                color = Color.blue;
                break;
            case CatState.Full:
                color = Color.black;
                break;
            default:
                color = Color.white;
                break;
        }
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, 4);
    }

}
