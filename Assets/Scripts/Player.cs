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

    [Header("Effects")]
    [SerializeField] ParticleSystem explosionVFX;
    [SerializeField] AudioSource explosionSFX;
    [SerializeField] AudioSource thrustSFX;

    GameManager gameManager;

    bool isUndestroyable;

    AudioSource fireSFX;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
        colliderYBound = GetComponent<Collider2D>().bounds.extents.y;

        fireSFX = GetComponent<AudioSource>();

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
            ShotIfInInterval();
    }

    void ShotIfInInterval()
    {
        if (shotsMadeInInterval < shotsLimit)
        {
            GameObject b = Pool.singleton.Get("Bullet");
            if (b != null)
            {
                fireSFX.Play();

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

    void PhysicalMove()
    {
        if (verticalInput > 0)
        {
            playerRb.AddForce(transform.up * verticalInput * thrustForce * Time.fixedDeltaTime);
            if (playerRb.velocity.magnitude > maxSpeed)
                playerRb.velocity = Vector2.ClampMagnitude(playerRb.velocity, maxSpeed);
            else
            {
                Instantiate(thrustSFX, transform.position, transform.rotation).Play();
            }
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
        if (!isUndestroyable)
        {
            if (other.CompareTag("Asteroid"))
            {
                Instantiate(explosionVFX, transform.position, transform.rotation).Play();
                Instantiate(explosionSFX, transform.position, transform.rotation).Play();

                gameObject.SetActive(false);

                StopCoroutine("Countdown");
                shotsMadeInInterval = 0;

                if (gameManager.DecreaseLives(this.gameObject) > 0)
                {
                    gameObject.transform.position = gameManager.GenerateRandomPos();
                    gameObject.SetActive(true);
                    StartCoroutine(Spawn());
                }
            }
        }
    }

    public IEnumerator Spawn()
    {
        isUndestroyable = true;
        Component[] childrenRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
        Color color;

        for (int i = 0; i < 6; i++)
        {
            yield return new WaitForSeconds(.25f);
            foreach (Renderer renderer in childrenRenderers)
            {
                color = renderer.material.color;
                color.a = 0f;
                renderer.material.color = color;
            }
            yield return new WaitForSeconds(.25f);
            foreach (Renderer renderer in childrenRenderers)
            {
                color = renderer.material.color;
                color.a = 0.5f;
                renderer.material.color = color;
            }
        }

        isUndestroyable = false;
    }
}
