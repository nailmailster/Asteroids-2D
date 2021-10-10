using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum AsteroidsCalibre
{
    Large = 20,
    Medium = 50,
    Small = 100
}

public class Asteroid : MonoBehaviour
{
    Rigidbody2D asteroidRb;
    [HideInInspector] public AsteroidsCalibre calibre;

    [HideInInspector] public Vector3 initPos;
    [HideInInspector] public Vector3 direction;
    [HideInInspector] public float initForce;

    [HideInInspector] public Vector2 velocity;

    [HideInInspector] GameManager gameManager;

    private void Awake()
    {
        asteroidRb = GetComponent<Rigidbody2D>();

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void OnEnable()
    {
        transform.position = initPos;
        if (calibre == AsteroidsCalibre.Large)
            asteroidRb.AddForce(direction * initForce, ForceMode2D.Impulse);
        else
            asteroidRb.velocity = velocity * initForce;
    }

    private void OnBecameInvisible()
    {
        gameManager.InvisibilityHandling(transform);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet") || other.CompareTag("Enemy Bullet"))
        {
            other.gameObject.SetActive(false);

            Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
            gameObject.SetActive(false);

            gameManager.AddScore(gameObject, velocity);
        }
        else if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }
}
