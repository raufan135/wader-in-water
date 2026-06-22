using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Swim")]
    [SerializeField] private float swimSpeed = 3f;
    [SerializeField] private float bobAmplitude = 0.5f;
    [SerializeField] private float bobFrequency = 1.5f;
    [SerializeField] private bool autoSwim = true;
    [SerializeField] private bool allowPlayerInput = false;

    [Header("Patrol")]
    [SerializeField] private bool patrolEnabled = false;
    [SerializeField] private float leftBound = -5f;
    [SerializeField] private float rightBound = 5f;

    [Header("Turning")]
    [SerializeField] private bool flipOnCollision = true;
    [SerializeField] private bool randomTurn = false;
    [SerializeField] private float minTurnInterval = 2f;
    [SerializeField] private float maxTurnInterval = 5f;

    private Rigidbody2D body;
    private float swimDirection = 1f;
    private float nextTurnTime;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.gravityScale = 0f;
            body.freezeRotation = true;
        }
    }

    private void Start()
    {
        nextTurnTime = Time.time + Random.Range(minTurnInterval, maxTurnInterval);
    }

    private void FixedUpdate()
    {
        float horizontal = autoSwim ? swimDirection : (allowPlayerInput ? Input.GetAxis("Horizontal") : 0f);
        float vertical = bobAmplitude * Mathf.Sin(Time.time * bobFrequency);

        if (body != null)
        {
            body.linearVelocity = new Vector2(horizontal * swimSpeed, vertical);
        }
        else
        {
            transform.position += new Vector3(horizontal * swimSpeed, vertical, 0f) * Time.fixedDeltaTime;
        }

        if (patrolEnabled)
        {
            if (transform.position.x < leftBound && swimDirection < 0f) FlipDirection();
            if (transform.position.x > rightBound && swimDirection > 0f) FlipDirection();
        }

        if (randomTurn && Time.time >= nextTurnTime)
        {
            FlipDirection();
            nextTurnTime = Time.time + Random.Range(minTurnInterval, maxTurnInterval);
        }
    }

    private void Update()
    {
        // Keep sprite facing movement direction
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Sign(swimDirection) * Mathf.Abs(scale.x == 0f ? 1f : scale.x);
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (flipOnCollision) FlipDirection();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (flipOnCollision) FlipDirection();
    }

    private void FlipDirection()
    {
        swimDirection *= -1f;
    }
}
