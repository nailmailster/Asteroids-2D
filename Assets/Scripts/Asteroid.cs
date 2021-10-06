using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    Rigidbody2D asteroidRb;
    float screenHalfWidthInUnits, screenHalfHeightInUnits;  //  границы видимости
    [SerializeField] float maxForce = 7;

    GameManager gameManager;

    private void Awake()
    {
        asteroidRb = GetComponent<Rigidbody2D>();

        screenHalfWidthInUnits = GameManager.screenHalfWidthInUnits;
        screenHalfHeightInUnits = GameManager.screenHalfHeightInUnits;

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void OnEnable()
    {
        Vector2 randomPos = gameManager.GenerateRandomPos();
        transform.position = randomPos;
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomSpeed = Random.Range(1, maxForce);
        asteroidRb.AddForce(randomDirection * randomSpeed, ForceMode2D.Impulse);
    }

    private void OnBecameInvisible()
    {
        if (transform.position.y > screenHalfHeightInUnits)
            transform.position = new Vector2(transform.position.x, -screenHalfHeightInUnits);
        else if (transform.position.y < -screenHalfHeightInUnits)
            transform.position = new Vector2(transform.position.x, screenHalfHeightInUnits);

        if (transform.position.x > screenHalfWidthInUnits)
            transform.position = new Vector2(-screenHalfWidthInUnits, transform.position.y);
        else if (transform.position.x < -screenHalfWidthInUnits)
            transform.position = new Vector2(screenHalfWidthInUnits, transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            gameObject.SetActive(false);
            other.gameObject.SetActive(false);

            gameManager.AddScore(20);
        }
    }
}
