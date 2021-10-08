using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] int initLives = 3;
    int lives;
    int score = 0;
    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Button newGameButton;
    [SerializeField] Button resumeGameButton;

    [SerializeField] GameObject player;
    Rigidbody2D playerRb;
    Vector2 playerVelocity;
    float playerAngularVelocity;

    public static float screenHalfHeightInUnits, screenHalfWidthInUnits;    //  границы экрана

    [SerializeField] int startingAsteroidsAmount = 2;
    int asteroidsAmount;

    public bool isGameOver = false;

    [Header("Effects")]
    [SerializeField] ParticleSystem asteroidExplosionVFX;
    [SerializeField] AudioSource smallExplosion;
    [SerializeField] AudioSource mediumExplosion;
    [SerializeField] AudioSource largeExplosion;

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
        asteroidsAmount = startingAsteroidsAmount;
        lives = initLives;
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

        for (int i = 0; i < asteroidsAmount; i++)
            SpawnRandomAsteroid(AsteroidsCalibre.Large);
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

    public void AddScore(GameObject parentAsteroid, Vector2 parentVelocity)
    {
        Instantiate(asteroidExplosionVFX, parentAsteroid.transform.position, parentAsteroid.transform.rotation).Play();

        Asteroid parentScript = parentAsteroid.GetComponent<Asteroid>();
        score += (int)parentScript.calibre;

        if (parentScript.calibre == AsteroidsCalibre.Small)
        {
            Instantiate(smallExplosion, parentAsteroid.transform.position, parentAsteroid.transform.rotation).Play();
            if (Pool.singleton.ActiveObjectsCount("Asteroid") == 0)
            {
                asteroidsAmount++;
                StartLevel();
            }
        }
        else
        {
            if (parentScript.calibre == AsteroidsCalibre.Large)
                Instantiate(largeExplosion, parentAsteroid.transform.position, parentAsteroid.transform.rotation).Play();
            if (parentScript.calibre == AsteroidsCalibre.Medium)
                Instantiate(mediumExplosion, parentAsteroid.transform.position, parentAsteroid.transform.rotation).Play();

            AsteroidsCalibre parentCalibre = parentScript.calibre;
            float speed = Random.Range(1, Asteroid.maxForce);
            SpawnChildAsteroid(parentAsteroid, parentScript, parentVelocity, 45, parentCalibre, speed);
            SpawnChildAsteroid(parentAsteroid, parentScript, parentVelocity, -45, parentCalibre, speed);
        }
    }

    void SpawnChildAsteroid(GameObject parentObject, Asteroid parentScript, Vector2 parentVelocity, float angle, AsteroidsCalibre parentCalibre, float newForce)
    {
        GameObject newObject = Pool.singleton.Get("Asteroid");
        if (newObject != null)
        {
            Asteroid newObjectScript = newObject.GetComponent<Asteroid>();

            if (parentCalibre == AsteroidsCalibre.Large)
            {
                newObjectScript.calibre = AsteroidsCalibre.Medium;
                newObjectScript.transform.localScale = new Vector3(6, 6, 1);
            }
            else if (parentCalibre == AsteroidsCalibre.Medium)
            {
                newObjectScript.calibre = AsteroidsCalibre.Small;
                newObjectScript.transform.localScale = new Vector3(4, 4, 1);
            }

            newObjectScript.initPos = parentObject.transform.position;
            newObject.transform.position = parentObject.transform.position;

            newObjectScript.initForce = newForce;

            newObjectScript.velocity = transform.InverseTransformDirection(parentVelocity);
            newObjectScript.velocity = (Quaternion.Euler(0, 0, angle) * newObjectScript.velocity).normalized;

            newObject.SetActive(true);
        }
    }

    void SpawnRandomAsteroid(AsteroidsCalibre calibre)
    {
        GameObject a = Pool.singleton.Get("Asteroid");
        if (a != null)
        {
            if (calibre == AsteroidsCalibre.Large)
                a.transform.localScale = new Vector3(8, 8, 1);
            else if (calibre == AsteroidsCalibre.Medium)
                a.transform.localScale = new Vector3(6, 6, 1);
            else if (calibre == AsteroidsCalibre.Small)
                a.transform.localScale = new Vector3(4, 4, 1);

            Asteroid newAsteroid = a.GetComponent<Asteroid>();
            newAsteroid.calibre = calibre;
            newAsteroid.initPos = GenerateRandomPos();
            newAsteroid.direction = Random.insideUnitCircle.normalized;
            newAsteroid.initForce = Random.Range(1, Asteroid.maxForce);

            a.SetActive(true);
        }
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
