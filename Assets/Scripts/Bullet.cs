using UnityEngine;

public class Bullet : MonoBehaviour
{
    float maxDistance;              //  расстояние, преодолев которое пуля станет неактивной
    float totalDistance = 0;        //  пройденная дистанция
    Vector2 controlPoint;           //  стартовый вектор для вычисления дистанции
    
    Rigidbody2D bulletRb;
    [SerializeField] float speed = .01f;

    GameManager gameManager;

    public void Fire()
    {
        transform.Translate(Vector2.up * Player.colliderYBound, Space.Self);    //  сдвинем пулю в направлении выстрела за границу коллайдера корабля
        controlPoint = transform.position;                                      //  запомним положение пули в момент выстрела

        bulletRb.AddRelativeForce(Vector2.up * speed, ForceMode2D.Impulse);     //  Bang!
    }

    private void Awake()
    {
        bulletRb = GetComponent<Rigidbody2D>();

        maxDistance = GameManager.screenHalfWidthInUnits * 2;    //  максимальная дальность по условию = ширине экрана

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    void Update()
    {
        CheckDistance();
    }

    void CheckDistance()
    {
        float distance = Vector2.Distance(controlPoint, transform.position);

        if (totalDistance + distance >= maxDistance)
            gameObject.SetActive(false);
    }

    private void OnBecameInvisible()
    {
        float distance = Vector2.Distance(controlPoint, transform.position);
        totalDistance += distance;

        gameManager.InvisibilityHandling(transform);

        controlPoint = transform.position;
    }

    private void OnDisable()
    {
        controlPoint = Vector2.zero;
        totalDistance = 0;
    }
}
