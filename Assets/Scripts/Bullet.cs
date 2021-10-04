using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    private Vector2 startPos;               //  начальное положение пули
    private float maxDistance;              //  расстояние, преодолев которое пуля исчезнет
    
    Rigidbody2D bulletRb;
    [SerializeField] float speed = .01f;

    public void Fire()
    {
        transform.Translate(Vector2.up, Space.Self);                //  сдвинем пулю в направлении выстрела на 1 unit от центра корабля
        startPos = transform.position;                              //  запомним положение пули в момент выстрела

        bulletRb.AddRelativeForce(Vector2.up * speed, ForceMode2D.Impulse);   //  Fire!
    }

    private void OnDisable()
    {
        // transform.position = Vector2.up;
        // transform.rotation = Quaternion.identity;
    }

    private void Awake()
    {
        //  рассчитаем максимальную дальность полета пули
        float height = Camera.main.orthographicSize * 2;            //  высота экрана в unit'ах
        float width = height * Screen.width / Screen.height;        //  ширана экрана в unit'ах
        maxDistance = width;

        bulletRb = GetComponent<Rigidbody2D>();
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
            gameObject.SetActive(false);
        }
    }
}
