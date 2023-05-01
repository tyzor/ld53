using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionUtilities;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<TableSpawner> spawnPoints_Objects;
    [SerializeField] List<BallSpawner> spawnPoints_Balls;

    private List<Ball> _currentBalls;
    private List<Cat> _currentActiveCats;


    // TODO -- obstacles should be set by level
    private int desiredCats = 3; // Number of cats we try to spawn
    private List<BarrelShooter> _barrelList;
    private int maxBarrels = 2;

    [SerializeField] float maxBallTime = 3f; // Time before spawning another ball
    [SerializeField] int maxBalls = 10; // Max we can have on the field
    private float ballTimer;


    enum GameState {
        Start,
        Play,
        End
    }
    private GameState _state = GameState.Start;

    private void OnEnable()
    {
        Ball.BallDied += OnBallDied;
        Cat.CatFull += OnCatFull;
    }

    private void OnDisable() {
        Ball.BallDied -= OnBallDied;
        Cat.CatFull -= OnCatFull;
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentBalls = new List<Ball>();
        _currentActiveCats = new List<Cat>();
        _barrelList = new List<BarrelShooter>();
        // Initialize the table by spawning everything
        InitTable();
    }

    // Update is called once per frame
    void Update()
    {
        if(_state == GameState.Play)
        {

            if(_currentBalls.Count == 0)
            {
                // We have no ball!
                SpawnBall();   
            }

            ballTimer -= Time.deltaTime;
            if(ballTimer <= 0 && _currentBalls.Count < maxBalls)
            {
                SpawnBall();
            }

            // Win condition!
            if(_currentActiveCats.Count == 0)
            {
                // TODO -- trigger win condition
                _state = GameState.End;
            }

        }
    }

    void SpawnBall()
    {
        int type = GetValidBallType();
        if(type >= 0){
            _currentBalls.Add(spawnPoints_Balls[Random.Range(0,spawnPoints_Balls.Count)].SpawnBall(type));
            ballTimer = maxBallTime;
        }
        else
            Debug.Log("No valid ball types can be spawned!");
    }

    private void OnBallDied(Ball ball, int way)
    {
        _currentBalls.Remove(ball);
    }

    private void OnCatFull(Cat cat)
    {
        _currentActiveCats.Remove(cat);
    }

    // Based on the current cats, determine what ball type to spawn
    int GetValidBallType()
    {
        if(_currentActiveCats.Count == 0)
            return -1;

        // Get random cat
        Debug.Log($"Active cats: {_currentActiveCats.Count}");
        Cat cat = _currentActiveCats[Random.Range(0,_currentActiveCats.Count)];
        return cat.ballType;
    }

    void InitTable()
    {
        // Spawn obstacles and determine cat difficulty etc   

        // Lets shake up the spawners so we can iterate through them first
        spawnPoints_Objects.Shuffle<TableSpawner>();

        foreach(TableSpawner sp in spawnPoints_Objects)
        {
            if(_currentActiveCats.Count < desiredCats && sp.CanSpawn(TableSpawner.ObstacleType.Cat))
            {
                Cat newCat = sp.Spawn<Cat>(TableSpawner.ObstacleType.Cat);
                newCat.HungerLevel = Random.Range(1,4);
                newCat.SetType(BallTypeManager.instance.GetRandomType());
                _currentActiveCats.Add(newCat);
            } else {

                // We try to put another object or empty here
                TableSpawner.ObstacleType type = sp.GetRandomPossibleSpawn();
                if(type == TableSpawner.ObstacleType.Cat)
                    type = TableSpawner.ObstacleType.Empty;

                if(type == TableSpawner.ObstacleType.Barrel && _barrelList.Count < maxBarrels)
                {
                    BarrelShooter barrel = sp.Spawn<BarrelShooter>(TableSpawner.ObstacleType.Barrel);
                    _barrelList.Add(barrel);
                }

                if(type == TableSpawner.ObstacleType.Bumper)
                {
                    Debug.Log("Bumper spawn");
                    // TODO -- add obstacles to list for cleanup?
                    Bumper bumper = sp.Spawn<Bumper>(TableSpawner.ObstacleType.Bumper);
                }

            }
            
        }
        
        // Start the game
        // TODO -- move this to a user input to start
        _state = GameState.Play;
    }
}
