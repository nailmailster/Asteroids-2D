using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] int initLives = 3;
    [SerializeField] int startingAsteroidsAmount = 2;
    public static float screenHalfHeightInUnits, screenHalfWidthInUnits;    //  границы экрана
    [HideInInspector] public bool isGameOver = false;
    [HideInInspector] public bool isGamePaused = false;

    int lives;
    int score = 0;
    int asteroidsAmount;

    [Header("Player Settings")]
    [SerializeField] GameObject player;

    Rigidbody2D playerRb;
    Vector2 playerVelocity;
    float playerAngularVelocity;

    [Header("Asteroids Settings")]
    [SerializeField] float asteroidMinForce = 1;
    [SerializeField] float asteroidMaxForce = 7;
    [SerializeField] float fragmentsDeflectionAngle = 45;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI scoreText;
    [Space(5)]
    [SerializeField] Button newGameButton;
    [SerializeField] Button resumeGameButton;

    [Header("Effects")]
    [SerializeField] ParticleSystem asteroidExplosionVFX;
    [Space(5)]
    [SerializeField] AudioSource smallExplosionSFX;
    [SerializeField] AudioSource mediumExplosionSFX;
    [SerializeField] AudioSource largeExplosionSFX;

    [Header("UFO")]
    [SerializeField] GameObject enemy;

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

        isGamePaused = true;

        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        resumeGameButton.gameObject.SetActive(false);
        newGameButton.gameObject.SetActive(false);

        isGamePaused = false;

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

        isGamePaused = false;

        Time.timeScale = 1;

        StartCoroutine(SpawnEnemyRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (!isGameOver)
        {
            yield return new WaitForSeconds(Random.Range(10, 20));
            Instantiate(enemy);
        }
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

    public void AddScore(GameObject parentAsteroid, Vector2 parentVelocity, bool collisionWithPlayer = false)
    {
        Asteroid parentScript = parentAsteroid.GetComponent<Asteroid>();
        score += (int)parentScript.calibre;

        PlayAsteroidExplosionVFX(parentAsteroid.transform.position, parentAsteroid.transform.rotation);
        PlayAsteroidExplosionSFX(parentScript.calibre);

        if (parentScript.calibre == AsteroidsCalibre.Small || collisionWithPlayer)
        {
            if (Pool.singleton.ActiveObjectsCount("Asteroid") == 0)
            {
                asteroidsAmount++;
                StartLevel();
            }
        }
        else
        {
            AsteroidsCalibre parentCalibre = parentScript.calibre;
            float speed = Random.Range(asteroidMinForce, asteroidMaxForce);
            SpawnChildAsteroid(parentAsteroid, parentScript, parentVelocity, fragmentsDeflectionAngle, parentCalibre, speed);
            SpawnChildAsteroid(parentAsteroid, parentScript, parentVelocity, -fragmentsDeflectionAngle, parentCalibre, speed);
        }
    }

    public void PlayAsteroidExplosionVFX(Vector3 position, Quaternion rotation)
    {
        ParticleSystem vfx = Instantiate(asteroidExplosionVFX, position, rotation);
        vfx.Play();
        Destroy(vfx.gameObject, vfx.main.duration);
    }

    public void PlayAsteroidExplosionSFX(AsteroidsCalibre calibre)
    {
        AudioSource sfx = null;

        if (calibre == AsteroidsCalibre.Large)
            sfx = Instantiate(largeExplosionSFX);
        else if (calibre == AsteroidsCalibre.Medium)
            sfx = Instantiate(mediumExplosionSFX);
        else if (calibre == AsteroidsCalibre.Small)
            sfx = Instantiate(smallExplosionSFX);

        if (sfx != null)
        {
            sfx.Play();
            Destroy(sfx.gameObject, sfx.clip.length);
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
            newAsteroid.initForce = Random.Range(asteroidMinForce, asteroidMaxForce);

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
