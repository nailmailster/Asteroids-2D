using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO : MonoBehaviour
{
    public static float colliderYBound;

    List<Vector2> path = new List<Vector2>();
    int pathIndex = 0;
    [SerializeField] float speed;
    [SerializeField] AudioSource sirenSFX;
    [SerializeField] AudioSource fireSFX;

    GameManager gameManager;

    GameObject player;

    private void Awake()
    {
        colliderYBound = GetComponent<Collider2D>().bounds.extents.y;

        GenerateRandomPath();
        speed = CalculateSpeed();

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        player = GameObject.Find("Ship");
    }

    void Start()
    {
        transform.position = path[pathIndex];
        pathIndex++;
    }

    IEnumerator ShotRoutine()
    {
        while (!gameManager.isGameOver)
        {
            yield return new WaitForSeconds(Random.Range(2, 6));
            Fire();
        }
    }

    void Fire()
    {
        GameObject b = Pool.singleton.Get("Bullet");
        if (b != null)
        {
            fireSFX.Play();

            b.tag = "Enemy Bullet";
            b.transform.position = transform.position;
            b.GetComponent<SpriteRenderer>().color = Color.red;
            b.SetActive(true);
            b.GetComponent<Bullet>().FireUFO(player.transform.position);
        }
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        if (pathIndex <= path.Count - 1)
        {
            Vector2 targetPos = path[pathIndex];
            var deltaMove = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPos, deltaMove);

            if (transform.position == (Vector3)targetPos)
                pathIndex++;
        }
        else
            Destroy(gameObject);
    }

    void GenerateRandomPath()
    {
        float randomX, randomY;
        float maxX = GameManager.screenHalfWidthInUnits;
        float maxY = GameManager.screenHalfHeightInUnits;

        //  начальная точка
        randomX = Random.Range(0, 100) <= 50 ? -(maxX + 2) : maxX + 2;
        randomY = Mathf.Clamp(Random.Range(-maxY, maxY), -maxY * .6f, maxY * .6f);
        path.Add(new Vector2(randomX, randomY));

        //  вторая точка
        if (path[0].x < 0)  //  слева направо
            randomX = Random.Range(-maxX + 2, 0);
        else                //  справа налево
            randomX = Random.Range(maxX - 2, 0);
        if (path[0].y < 0)  //  сверху вниз
            randomY = Random.Range(0, maxY * .8f);
        else                //  снизу вверх
            randomY = Random.Range(0, -maxY * .8f);
        path.Add(new Vector2(randomX, randomY));

        //  третья точка
        if (path[0].x < 0)  //  слева направо
            randomX = Random.Range(0, maxX *.6f);
        else                //  справа налево
            randomX = Random.Range(0, -maxX * .6f);
        randomY = path[1].y;
        path.Add(new Vector2(randomX, randomY));

        //  конечная точка
        if (path[0].x < 0)  //  слева направо
            randomX = maxX + 2;
        else                //  справа налево
            randomX = -(maxX + 2);
        if (path[0].y < 0)  //  если на второй точке сверху вниз, то сейчас снизу вверх
            randomY = Random.Range(0, -maxY);
        else                //  если на второй точке снизу вверх, то сейчас сверху вниз
            randomY = Random.Range(0, maxY);
        path.Add(new Vector2(randomX, randomY));
    }

    float CalculateSpeed()
    {
        float distance = 0;

        for (int i = 0; i < path.Count - 1; i++)
            distance += Vector2.Distance(path[i], path[i + 1]);
        
        return distance / 10;
    }

    private void OnBecameVisible()
    {
        StartCoroutine(ShotRoutine());
    }

    private void OnBecameInvisible()
    {
        StopCoroutine(ShotRoutine());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(gameObject);
            other.gameObject.SetActive(false);
            gameManager.AddScoreForUFO();
        }
    }
}
