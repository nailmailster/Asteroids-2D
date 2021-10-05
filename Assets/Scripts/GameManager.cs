using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] int lives = 3;
    int score = 0;
    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI gameOverText;

    public static float screenHalfHeightInUnits;
    public static float screenHalfWidthInUnits;

    int startingAsteroidsAmount = 2;

    public bool isGameOver = false;

    private void Awake()
    {
        screenHalfHeightInUnits = Camera.main.orthographicSize;
        screenHalfWidthInUnits = screenHalfHeightInUnits * Screen.width / Screen.height;
    }

    private void Start()
    {
        StartLevel();
    }

    private void Update()
    {
        
    }

    private void OnGUI()
    {
        livesText.SetText("Lives: " + lives);
        scoreText.SetText("Score: " + score);

        if (GUILayout.Button("Start"))
        {
            SpawnAsteroid();
        }

        if (isGameOver)
        {
            gameOverText.gameObject.SetActive(true);
        }
    }

    void StartLevel()
    {
        for (int i = 0; i < startingAsteroidsAmount; i++)
        {
            Invoke("SpawnAsteroid", .5f);
        }
    }

    public int DecreaseLives(GameObject player)
    {
        if (lives > 0)
        {
            lives--;
        }
        else
        {
            isGameOver = true;
        }

        return lives;
    }

    public void AddScore(int value)
    {
        score += value;

        if (Pool.singleton.CountActiveObjects("Asteroid") == 0)
            Debug.Log("New level");
    }

    void SpawnAsteroid()
    {
        GameObject a = Pool.singleton.Get("Asteroid");
        if (a != null)
        {
            a.SetActive(true);
        }
    }
}
