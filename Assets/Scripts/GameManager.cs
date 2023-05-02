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
    private List<GameObject> _allTableObjects; // For cleaning up -- we maintain a list of everything

    private List<BarrelShooter> _barrelList;
    private List<Bumper> _bumperList;

    [SerializeField] float maxBallTime = 5f; // Time before spawning another ball
    private int maxBalls = 1; // Max we can have on the field
    private float ballTimer;


    enum GameState {
        Start,
        Play,
        Pause,
        End
    }
    private GameState _state = GameState.Start;

    public static event System.Action<int> LevelChange;
    public static event System.Action<int> LivesChange;

    public int currentLevel;
    public int playerLives = 3;

    private UIManager _uiManager;

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
        _uiManager = FindObjectOfType<UIManager>();

        _currentBalls = new List<Ball>();
        _currentActiveCats = new List<Cat>();
        _barrelList = new List<BarrelShooter>();
        _bumperList = new List<Bumper>();
        _allTableObjects = new List<GameObject>();
        
        StartNewGame();
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
                _state = GameState.Pause;
                StartCoroutine(NextLevel());
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

        if(way == 0)
        {
            ChangeLives(playerLives-1);
            if(playerLives <= 0)
            {   
                // Game Over
                _state = GameState.End;
                StartCoroutine(GameOver());
            }
        }
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

    void InitTable(int level)
    {
        // Determine table values
        int desiredCats = Mathf.Max(1,level / 3);
        maxBalls = desiredCats;
        int maxBarrels = 2;
        int maxBumpers = level / 2;

        // Spawn objects

        // Lets shake up the spawners so we can iterate through them first
        spawnPoints_Objects.Shuffle<TableSpawner>();

        foreach(TableSpawner sp in spawnPoints_Objects)
        {
            if(_currentActiveCats.Count < desiredCats && sp.CanSpawn(TableSpawner.ObstacleType.Cat))
            {
                Cat newCat = sp.Spawn<Cat>(TableSpawner.ObstacleType.Cat);
                newCat.HungerLevel = 1; //Random.Range(1,4);
                newCat.SetType(BallTypeManager.instance.GetRandomType());
                _currentActiveCats.Add(newCat);
                _allTableObjects.Add(newCat.gameObject);
            } else {

                // We try to put another object or empty here
                TableSpawner.ObstacleType type = sp.GetRandomPossibleSpawn();
                if(type == TableSpawner.ObstacleType.Cat)
                    type = TableSpawner.ObstacleType.Empty;

                if(type == TableSpawner.ObstacleType.Barrel && _barrelList.Count < maxBarrels)
                {
                    BarrelShooter barrel = sp.Spawn<BarrelShooter>(TableSpawner.ObstacleType.Barrel);
                    _barrelList.Add(barrel);
                    _allTableObjects.Add(barrel.gameObject);
                }

                if(type == TableSpawner.ObstacleType.Bumper && _bumperList.Count < maxBumpers)
                {
                    Debug.Log("Bumper spawn");
                    // TODO -- add obstacles to list for cleanup?
                    Bumper bumper = sp.Spawn<Bumper>(TableSpawner.ObstacleType.Bumper);
                    _allTableObjects.Add(bumper.gameObject);
                }

            }
            
        }
        
    }

    // Starting a fresh run
    public void StartNewGame()
    {
        ChangeLives(3);
        ChangeLevel(1);
        StartNewLevel(1);
    }

    // Reset table elements for new game
    void CleanUpTable()
    {
        foreach(GameObject obj in _allTableObjects)
        {
            Destroy(obj);
        }

        foreach(Ball ball in _currentBalls)
        {
            Destroy(ball.gameObject);
        }

        _allTableObjects.Clear();
        _barrelList.Clear();
        _bumperList.Clear();
        _currentActiveCats.Clear();
        _currentBalls.Clear();   
    }

    void StartNewLevel(int level = 1)
    {
        CleanUpTable();
        InitTable(level);   

        // Start the game
        // TODO -- move this to a user input to start
        _state = GameState.Play;
        Time.timeScale = 1;
    }

    private IEnumerator NextLevel()
    {
        Debug.Log("NextLevel called");
        Time.timeScale = 0; // Pause time
        ChangeLevel(currentLevel+1);
        
        _uiManager.FadeOut();

        yield return new WaitForSecondsRealtime(2f);

        _uiManager.FadeIn();

        Time.timeScale = 1;
        StartNewLevel(currentLevel);
    }

    private IEnumerator GameOver()
    {
        Debug.Log("GameOver called");
        Time.timeScale = 0; // Pause time
    
        // Show game over screen with restart button
        _uiManager.SetGameOver(true);

        yield break;
    }

    private void ChangeLevel(int newLevel)
    {
        currentLevel = newLevel;
        LevelChange?.Invoke(newLevel);
    }

    private void ChangeLives(int newLives)
    {
        playerLives = newLives;
        LivesChange?.Invoke(newLives);
    }

}

