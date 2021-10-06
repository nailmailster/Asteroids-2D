using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    public static float screenHalfHeightInUnits;
    public static float screenHalfWidthInUnits;

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
        {
            PauseGame();
        }
    }

    private void OnGUI()
    {
        livesText.SetText("Попыток: " + lives);
        scoreText.SetText("Наград: " + score);
    }

    void PauseGame()
    {
        resumeGameButton.gameObject.SetActive(true);
        newGameButton.gameObject.SetActive(true);

        // playerVelocity = playerRb.velocity;
        // playerRb.velocity = Vector2.zero;
        // playerAngularVelocity = playerRb.angularVelocity;
        // playerRb.angularVelocity = 0;

        // Pool.singleton.FreezeActiveObjects();

        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        resumeGameButton.gameObject.SetActive(false);
        newGameButton.gameObject.SetActive(false);

        // playerRb.velocity = playerVelocity;
        // playerRb.angularVelocity = playerAngularVelocity;

        // Pool.singleton.UnfreezeActiveObjects();

        Time.timeScale = 1;
    }

    public void StartNewGame()
    {
        isGameOver = false;
        startingAsteroidsAmount = 2;
        lives = 3;
        newGameButton.gameObject.SetActive(false);
        resumeGameButton.gameObject.SetActive(false);
        Pool.singleton.DeactivateAllActiveObjects();
        StartLevel();
    }

    void StartLevel()
    {
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
        Pool.singleton.FreezeActiveObjects();
        newGameButton.gameObject.SetActive(true);
    }

    public void AddScore(int value)
    {
        score += value;
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
}
