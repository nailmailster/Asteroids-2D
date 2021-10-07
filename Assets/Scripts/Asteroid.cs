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
    public static float maxForce = 7;
    public AsteroidsCalibre calibre;
    public static float deflectionAngle = 45;

    public Vector3 initPos;
    public Vector3 direction;
    public float initForce;

    GameManager gameManager;

    private void Awake()
    {
        asteroidRb = GetComponent<Rigidbody2D>();

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void OnEnable()
    {
        transform.position = initPos;
        asteroidRb.AddRelativeForce(direction * initForce, ForceMode2D.Impulse);
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

            gameManager.AddScore(gameObject);
        }
    }
}
