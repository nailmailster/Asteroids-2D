using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D playerRb;
    public static float colliderYBound;

    [SerializeField] float thrustForce = 150;
    [SerializeField] float maxSpeed = 5;
    [SerializeField] float torqueForce = 50;

    float horizontalInput, verticalInput;

    int shotsMadeInInterval = 0;
    [SerializeField] float shotsInterval = 1;
    [SerializeField] int shotsLimit = 3;

    [SerializeField] ParticleSystem burstVFX;

    GameManager gameManager;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
        colliderYBound = GetComponent<Collider2D>().bounds.extents.y;

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
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

    IEnumerator Countdown(float seconds)
    {
        while (shotsMadeInInterval > 0)
        {
            yield return new WaitForSeconds(seconds);
            shotsMadeInInterval = 0;
        }
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        if (verticalInput < 0)
            verticalInput = 0;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (shotsMadeInInterval < shotsLimit)
            {
                GameObject b = Pool.singleton.Get("Bullet");
                if (b != null)
                {
                    b.transform.position = transform.position;
                    b.transform.rotation = transform.rotation;
                    b.SetActive(true);
                    b.GetComponent<Bullet>().Fire();

                    shotsMadeInInterval++;
                    if (shotsMadeInInterval == 1)
                        StartCoroutine(Countdown(shotsInterval));
                }
            }
        }
    }

    void PhysicalMove()
    {
        if (verticalInput > 0)
        {
            playerRb.AddForce(transform.up * verticalInput * thrustForce * Time.fixedDeltaTime);
            if (playerRb.velocity.magnitude > maxSpeed)
                playerRb.velocity = Vector2.ClampMagnitude(playerRb.velocity, maxSpeed);
        }

        playerRb.AddTorque(horizontalInput * -torqueForce * Time.fixedDeltaTime);
    }

    void CheckVisibility()
    {
        if (transform.position.y > (GameManager.screenHalfHeightInUnits + 1/2))
            transform.position = new Vector2(transform.position.x, -GameManager.screenHalfHeightInUnits);
        else if (transform.position.y < -(GameManager.screenHalfHeightInUnits + 1))
            transform.position = new Vector2(transform.position.x, GameManager.screenHalfHeightInUnits);

        if (transform.position.x > (GameManager.screenHalfWidthInUnits + 1))
            transform.position = new Vector2(-GameManager.screenHalfWidthInUnits, transform.position.y);
        else if (transform.position.x < -(GameManager.screenHalfWidthInUnits + 1))
            transform.position = new Vector2(GameManager.screenHalfWidthInUnits, transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Asteroid"))
        {
            burstVFX.transform.position = transform.position;
            burstVFX.Play();

            gameObject.SetActive(false);

            gameManager.DecreaseLives();
        }
    }
}
