using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO : MonoBehaviour
{
    List<Vector2> path = new List<Vector2>();
    int pathIndex;
    [SerializeField] float speed = 4;

    private void Awake()
    {
        GenerateRandomPath();
    }

    void Start()
    {
        transform.position = path[pathIndex];
        pathIndex++;
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
        randomY = Mathf.Clamp(Random.Range(-maxY, maxY), -maxY * 2 * .2f, maxY * 2 * .2f);
        path.Add(new Vector2(randomX, randomY));

        //  вторая точка
        if (path[0].x < 0)  //  слева направо
            randomX = Random.Range(-maxX, 0);
        else                //  справа налево
            randomX = Random.Range(maxX, 0);
        if (path[0].y < 0)  //  сверху вниз
            randomY = Random.Range(path[0].y, maxY);
        else                //  снизу вверх
            randomY = Random.Range(path[0].y, -maxY);
        path.Add(new Vector2(randomX, randomY));

        //  третья точка
        if (path[0].x < 0)  //  слева направо
            randomX = Random.Range(path[1].x, maxX);
        else                //  справа налево
            randomX = Random.Range(path[1].x, -maxX);
        randomY = path[1].y;
        path.Add(new Vector2(randomX, randomY));

        //  конечная точка
        if (path[0].x < 0)  //  слева направо
            randomX = maxX + 2;
        else                //  справа налево
            randomX = -(maxX + 2);
        if (path[0].y < 0)  //  сверху вниз
            randomY = Random.Range(path[2].y, -maxY);
        else                //  снизу вверх
            randomY = Random.Range(path[2].y, maxY);
        path.Add(new Vector2(randomX, randomY));
    }
}
