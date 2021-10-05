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

    public static float screenHalfHeightInUnits;
    public static float screenHalfWidthInUnits;

    private void Awake()
    {
        screenHalfHeightInUnits = Camera.main.orthographicSize;
        screenHalfWidthInUnits = screenHalfHeightInUnits * Screen.width / Screen.height;
    }

    private void OnGUI()
    {
        livesText.SetText("Lives: " + lives);
        scoreText.SetText("Score: " + score);
    }

    public void DecreaseLives()
    {
        if (lives > 0)
            lives--;
    }

    public void AddScore(int value)
    {
        score += value;
    }
}
