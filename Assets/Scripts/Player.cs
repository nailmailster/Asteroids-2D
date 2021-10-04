using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D playerRb;

    [SerializeField] float thrustForce = 150;
    [SerializeField] float maxSpeed = 5;
    [SerializeField] float torqueForce = 50;

    float horizontalInput, verticalInput;

    // [SerializeField] GameObject bullet;
    float height;
    float width;

    private BulletObjectPool _pool; //  значением будет добавленный компонент BulletObjectPool, будем использовать его Spawn() при выстрелах

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();

        height = Camera.main.orthographicSize;
        width = height * Screen.width / Screen.height;
    }

    void Start()
    {
        _pool = gameObject.AddComponent<BulletObjectPool>();
    }

    private void Update()
    {
        GetInput();
    }

    void FixedUpdate()
    {
        PhysicalMove();
    }

    private void LateUpdate()
    {
        CheckVisibility();
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        if (verticalInput < 0)
        {
            verticalInput = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _pool.Spawn(transform.position, transform.rotation, transform);
        }
    }

    void PhysicalMove()
    {
        if (verticalInput > 0)
        {
            playerRb.AddForce(transform.up * verticalInput * thrustForce * Time.fixedDeltaTime);
            if (playerRb.velocity.magnitude > maxSpeed)
            {
                playerRb.velocity = Vector2.ClampMagnitude(playerRb.velocity, maxSpeed);
            }
        }

        playerRb.AddTorque(horizontalInput * -torqueForce * Time.fixedDeltaTime);
    }

    void CheckVisibility()
    {
        if (transform.position.y > (height + .5f))
        {
            transform.position = new Vector2(transform.position.x, -height);
        }
        if (transform.position.y < -(height + 1))
        {
            transform.position = new Vector2(transform.position.x, height);
        }

        if (transform.position.x > (width + 1))
        {
            transform.position = new Vector2(-width, transform.position.y);
        }
        if (transform.position.x < -(width + 1))
        {
            transform.position = new Vector2(width, transform.position.y);
        }
    }
}
