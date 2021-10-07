using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum AsteroidSize
{
    Large = 20,
    Medium = 50,
    Small = 100
}

public class GameManager : MonoBehaviour
{
    [SerializeField] int lives = 3;
    int score = 0;
    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] Button newGameButton;
    [SerializeField] Button resumeGameButton;

    [SerializeField] GameObject player;
    Rigidbody2D playerRb;
    Vector2 playerVelocity;
    float playerAngularVelocity;

    public static float screenHalfHeightInUnits, screenHalfWidthInUnits;    //  границы экрана

    int startingAsteroidsAmount = 2;

    public bool isGameOver = false;

    private void Awake()
    {
        playerRb = player.GetComponent<Rigidbody2D>();

        screenHalfHeightInUnits = Camera.main.orthographicSize;
        screenHalfWidthInUnits = screenHalfHeightInUnits * Screen.width / Screen.height;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
            PauseGame();
    }

    private void OnGUI()
    {
        livesText.SetText("Попыток: " + lives);
        scoreText.SetText("Баллов: " + score);
    }

    void PauseGame()
    {
        resumeGameButton.gameObject.SetActive(true);
        newGameButton.gameObject.SetActive(true);

        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        resumeGameButton.gameObject.SetActive(false);
        newGameButton.gameObject.SetActive(false);

        Time.timeScale = 1;
    }

    public void StartNewGame()
    {
        isGameOver = false;
        startingAsteroidsAmount = 2;
        lives = 3;
        score = 0;
        newGameButton.gameObject.SetActive(false);
        resumeGameButton.gameObject.SetActive(false);
        Pool.singleton.DeactivateAllActiveObjects();
        StartLevel();
    }

    void StartLevel()
    {
        player.gameObject.transform.position = GenerateRandomPos();
        playerRb.velocity = Vector2.zero;
        player.gameObject.SetActive(true);
        StartCoroutine(player.GetComponent<Player>().Spawn());

        for (int i = 0; i < startingAsteroidsAmount; i++)
            Invoke("SpawnAsteroid", 0);
    }

    public int DecreaseLives(GameObject player)
    {
        lives--;
        if (lives == 0)
            GameOver();
        return lives;
    }

    void GameOver()
    {
        isGameOver = true;
        newGameButton.gameObject.SetActive(true);
    }

    public void AddScore(int value)
    {
        score += value;
        if (Pool.singleton.ActiveObjectsCount("Asteroid") == 0)
        {
            startingAsteroidsAmount++;
            StartLevel();
        }
    }

    void SpawnAsteroid()
    {
        GameObject a = Pool.singleton.Get("Asteroid");
        if (a != null)
            a.SetActive(true);
    }

    public Vector2 GenerateRandomPos()
    {
        float randomXPos = Random.Range(-screenHalfWidthInUnits, screenHalfWidthInUnits);
        float randomYPos = Random.Range(-screenHalfHeightInUnits, screenHalfHeightInUnits);

        return new Vector2(randomXPos, randomYPos);
    }

    public void InvisibilityHandling(Transform _transform)
    {
        if (_transform.position.y > screenHalfHeightInUnits)
            _transform.position = new Vector2(_transform.position.x, -screenHalfHeightInUnits);
        else if (_transform.position.y < -screenHalfHeightInUnits)
            _transform.position = new Vector2(_transform.position.x, screenHalfHeightInUnits);

        if (_transform.position.x > screenHalfWidthInUnits)
            _transform.position = new Vector2(-screenHalfWidthInUnits, _transform.position.y);
        else if (_transform.position.x < -screenHalfWidthInUnits)
            _transform.position = new Vector2(screenHalfWidthInUnits, _transform.position.y);
    }
}
