using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D playerRb;
    public static float colliderYBound;

    [SerializeField] float thrustForce = 150;
    [SerializeField] float maxSpeed = 5;
    [SerializeField] float torqueForce = 50;

    float horizontalInput, verticalInput;

    public static float screenHalfHeightInUnits;
    public static float screenHalfWidthInUnits;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
        colliderYBound = GetComponent<Collider2D>().bounds.extents.y;

        screenHalfHeightInUnits = Camera.main.orthographicSize;
        screenHalfWidthInUnits = screenHalfHeightInUnits * Screen.width / Screen.height;
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
            verticalInput = 0;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject b = Pool.singleton.Get("Bullet");
            if (b != null)
            {
                b.transform.position = transform.position;
                b.transform.rotation = transform.rotation;
                b.SetActive(true);
                b.GetComponent<Bullet>().Fire();
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
        if (transform.position.y > (screenHalfHeightInUnits + 1/2))
            transform.position = new Vector2(transform.position.x, -screenHalfHeightInUnits);
        if (transform.position.y < -(screenHalfHeightInUnits + 1))
            transform.position = new Vector2(transform.position.x, screenHalfHeightInUnits);

        if (transform.position.x > (screenHalfWidthInUnits + 1))
            transform.position = new Vector2(-screenHalfWidthInUnits, transform.position.y);
        if (transform.position.x < -(screenHalfWidthInUnits + 1))
            transform.position = new Vector2(screenHalfWidthInUnits, transform.position.y);
    }
}
