using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] int lives = 3;
    int points = 0;
    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI pointsText;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnGUI()
    {
        livesText.SetText("Lives: " + lives);
        pointsText.SetText("Points: " + points);
    }
}
