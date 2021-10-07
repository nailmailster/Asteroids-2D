using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    Rigidbody2D asteroidRb;
    [SerializeField] float maxForce = 7;
    public int rewardValue = 20;

    GameManager gameManager;

    private void Awake()
    {
        asteroidRb = GetComponent<Rigidbody2D>();

        // if (rewardValue == 20)
        //     transform.localScale = new Vector3(8, 8, 1);
        // else if (rewardValue == 50)
        //     transform.localScale = new Vector3(4, 4, 1);
        // else if (rewardValue == 100)
        //     transform.localScale = new Vector3(2, 2, 1);

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
        gameManager.InvisibilityHandling(transform);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            gameObject.SetActive(false);
            other.gameObject.SetActive(false);

            gameManager.AddScore(rewardValue);
        }
    }
}
