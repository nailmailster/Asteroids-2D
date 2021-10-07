using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public static class Vector2Extension
{
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        // float tx = v.x;
        // float ty = v.y;
        // v.x = (cos * tx) - (sin * ty);
        // v.y = (sin * tx) + (cos * ty);

        // return v;

        float tx = v.x;
        float ty = v.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);    }
}
 
public class GameManager : MonoBehaviour
{
    [SerializeField] int initLives = 3;
    int lives;
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

    public static float screenHalfHeightInUnits, screenHalfWidthInUnits;    //  границы экрана

    [SerializeField] int startingAsteroidsAmount = 2;
    int asteroidsAmount;

    public bool isGameOver = false;

    [SerializeField] ParticleSystem burstVFX;

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
        burstVFX.transform.position = parentAsteroid.transform.position;
        burstVFX.Play();

        Asteroid parentScript = parentAsteroid.GetComponent<Asteroid>();
        score += (int)parentScript.calibre;

        if (parentScript.calibre == AsteroidsCalibre.Small)
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
            float speed = Random.Range(1, Asteroid.maxForce);
            SpawnFractionAsteroid(parentAsteroid, parentScript, parentVelocity, 45, parentCalibre, speed);
            SpawnFractionAsteroid(parentAsteroid, parentScript, parentVelocity, -45, parentCalibre, speed);
        }

        #region comment 1
        // else if (parentScript.calibre == AsteroidsCalibre.Large)
        // {
        //     // SpawnAsteroid(AsteroidsCalibre.Medium, initSpeed, parentScript, false, false, -Asteroid.deflectionAngle);
        //     // SpawnAsteroid(AsteroidsCalibre.Medium, initSpeed, parentScript, false, false, Asteroid.deflectionAngle * 2);
        //     // // SpawnAsteroid(AsteroidSize.Medium, initSpeed, parentAsteroid, false, false, -parentAsteroid.deflectionAngle);


        //     float initSpeed = Random.Range(1, Asteroid.maxForce);
        //     SpawnAsteroid(AsteroidsCalibre.Medium, initSpeed, parentScript, false, false, Asteroid.deflectionAngle);
        //     SpawnAsteroid(AsteroidsCalibre.Medium, initSpeed, parentScript, false, false, -Asteroid.deflectionAngle * 2);
        // }
        // else if (parentScript.calibre == AsteroidsCalibre.Medium)
        // {
        //     // SpawnAsteroid(AsteroidsCalibre.Small, initSpeed, parentScript, false, false, -Asteroid.deflectionAngle);
        //     // SpawnAsteroid(AsteroidsCalibre.Small, initSpeed, parentScript, false, false, Asteroid.deflectionAngle * 2);
        //     // // SpawnAsteroid(AsteroidSize.Small, initSpeed, parentAsteroid, false, false, -parentAsteroid.deflectionAngle);


        //     float initSpeed = Random.Range(1, Asteroid.maxForce);
        //     SpawnAsteroid(AsteroidsCalibre.Small, initSpeed, parentScript, false, false, Asteroid.deflectionAngle);
        //     SpawnAsteroid(AsteroidsCalibre.Small, initSpeed, parentScript, false, false, -Asteroid.deflectionAngle * 2);
        // }
        #endregion
    }

    void SpawnFractionAsteroid(GameObject parentObject, Asteroid parentScript, Vector2 parentVelocity, float angle, AsteroidsCalibre parentCalibre, float newForce)
    {
        GameObject newObject = Pool.singleton.Get("Asteroid");
        if (newObject != null)
        {
            Asteroid newObjectScript = newObject.GetComponent<Asteroid>();

            Debug.Log(parentCalibre);
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

    #region comment2
    // void SpawnAsteroid(AsteroidsCalibre size, float newForce = 0, Asteroid parentScript = null, bool randomPos = true, bool randomDir = true, float angle = 0)
    // {
    //     GameObject a = Pool.singleton.Get("Asteroid");
    //     if (a != null)
    //     {
    //         Asteroid newAsteroid = a.GetComponent<Asteroid>();

    //         if (size == AsteroidsCalibre.Large)
    //             a.transform.localScale = new Vector3(8, 8, 1);
    //         else if (size == AsteroidsCalibre.Medium)
    //             a.transform.localScale = new Vector3(6, 6, 1);
    //         else if (size == AsteroidsCalibre.Small)
    //             a.transform.localScale = new Vector3(4, 4, 1);

    //         newAsteroid.calibre = size;

    //         if (randomPos)
    //         {
    //             newAsteroid.initPos = GenerateRandomPos();
    //         }
    //         else
    //         {
    //             newAsteroid.initPos = parentScript.transform.position;

    //             a.transform.position = parentScript.transform.position;
    //             a.transform.rotation = parentScript.transform.rotation;
    //         }

    //         if (randomDir)
    //             newAsteroid.direction = Random.insideUnitCircle.normalized;
    //         else
    //         {
    //             Debug.Log("parentAsteroid before = " + parentScript.direction);
    //             Debug.Log("newAsteroid before = " + newAsteroid.direction);

    //             Vector3 parentDir = new Vector3(parentScript.direction.x, parentScript.direction.y, parentScript.direction.z);
    //             // newAsteroid.initDir = ((Vector2)parentDir).Rotate(angle).normalized;
    //             // newAsteroid.initDir = ((Vector2)parentAsteroid.initDir).Rotate(angle).normalized;
    //             // newAsteroid.initDir = (Quaternion.Euler(0, 0, angle) * parentAsteroid.initDir).normalized;
    //             newAsteroid.direction = (Quaternion.Euler(0, 0, angle) * parentDir).normalized;

    //             Debug.Log("parentAsteroid after = " + parentScript.direction);
    //             Debug.Log("newAsteroid after = " + newAsteroid.direction);
    //         }
            
    //         if (newForce > 0)
    //             newAsteroid.initForce = newForce;
    //         else
    //             newAsteroid.initForce = Random.Range(1, Asteroid.maxForce);

    //         a.SetActive(true);
    //     }
    // }
    #endregion

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
