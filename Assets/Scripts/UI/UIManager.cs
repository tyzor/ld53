using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject faderScreen;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI livesText;

    private GameManager gameManager;

    private void OnEnable()
    {
        GameManager.LevelChange += OnLevelChange;    
        GameManager.LivesChange += OnLivesChange;    
    }
    private void OnDisable() {
        GameManager.LevelChange -= OnLevelChange;    
        GameManager.LivesChange -= OnLivesChange;    
    }

    private void Awake() {
        gameManager = FindObjectOfType<GameManager>();    
        SetGameOver(false);
        SetFadeScreen(false);

        gameOverScreen.gameObject.GetComponentInChildren<Button>().onClick.AddListener(OnRestartClick);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnLevelChange(int newLevel)
    {
        levelText.SetText($"LEVEL  {newLevel}");
    }
    private void OnLivesChange(int newLives)
    {
        livesText.SetText($"{newLives} LIVES");
    }

    public void SetGameOver(bool show)
    {
        gameOverScreen.SetActive(show);
    }

    public void SetFadeScreen(bool show)
    {
        faderScreen.SetActive(show);
    }

    void OnRestartClick()
    {
        SetGameOver(false);
        gameManager.StartNewGame();
    }

    public void FadeOut() {
        StartCoroutine(DoFade(false));
    }
    public void FadeIn() {
        StartCoroutine(DoFade(true));
    }


    private IEnumerator DoFade(bool FadeIn)
    {
        faderScreen.SetActive(true);

        float alphaValue = FadeIn ? 1f : 0;
        
        Image faderImg = faderScreen.GetComponentInChildren<Image>();
        Color color = faderImg.color;
        color.a = alphaValue;
        faderImg.color = color;

        float timer = 1f;
        while(timer > 0)
        {
            timer -= Time.unscaledDeltaTime;
            
            if(FadeIn)
            {
                color.a = timer;
            } else {
                color.a = 1f - timer;
            }
            faderImg.color = color;

            yield return null;
        }           


    }
}
