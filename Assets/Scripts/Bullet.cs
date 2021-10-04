using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    //  в этом скрипте нам нужно лишь вернуть объект в пул - pool.Release() в исходном состоянии - OnDisable()
    public IObjectPool<Bullet> Pool { get; set; }

    private Vector2 startPos;               //  начальное положение пули
    private float maxDistance;              //  расстояние, преодолев которое пуля исчезнет
    
    Rigidbody2D bulletRb;
    [SerializeField] float speed = .01f;

    [SerializeField] GameObject player;

    private void OnEnable()
    {
        //  это работает, но можно ли избавиться от player
        // transform.position = player.transform.position;
        // transform.rotation = player.transform.rotation;


        // transform.Translate(transform.forward, Space.Self);      //  WORKS!
        // transform.Translate(transform.up, Space.Self);           //  DOESN'T WORK
        transform.Translate(Vector2.up, Space.Self);                //  сдвинем пулю в направлении выстрела на 1 unit от центра корабля
        startPos = transform.position;                              //  запомним положение пули в момент выстрела

        bulletRb.AddRelativeForce(player.transform.up * speed, ForceMode2D.Impulse);   //  Fire!
        // bulletRb.AddRelativeForce(transform.up * speed, ForceMode2D.Impulse);   //  Fire!
    }

    private void OnDisable()
    {
        transform.position = Vector2.up;
        transform.rotation = Quaternion.identity;
    }

    private void Awake()
    {
        //  рассчитаем максимальную дальность полета пули
        float height = Camera.main.orthographicSize * 2;            //  высота экрана в unit'ах
        float width = height * Screen.width / Screen.height;        //  ширана экрана в unit'ах
        maxDistance = width;

        bulletRb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Ship");
    }

    void Update()
    {
        CheckDistance();
    }

    void CheckDistance()
    {
        float distance = Vector2.Distance(startPos, transform.position);
        if (distance > maxDistance)
        {
            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        Pool.Release(this);
    }
}
