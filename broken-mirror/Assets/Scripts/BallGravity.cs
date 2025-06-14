using UnityEngine;

public class BallGravity : MonoBehaviour
{
    public float dragSpeed = 1f; // How fast it falls when dragged
    public float recoverSpeed = 1f; // How fast it rises back
    public float targetY = 0f; // Y-position to return to (mid screen)

    private bool isDragging = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetY = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)).y; // middle of screen
    }

    void Update()
    {
        if (!isDragging)
        {
            // Slowly rise to the middle Y position
            if (transform.position.y < targetY)
            {
                Vector3 pos = transform.position;
                pos.y += recoverSpeed * Time.deltaTime;
                pos.y = Mathf.Min(pos.y, targetY); // clamp to not go past target
                transform.position = pos;
            }
        }
    }

    void FixedUpdate()
    {
        if (isDragging)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -dragSpeed); // drag down
        }
        else
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // stop dragging
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            Debug.Log("Game Over!");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        IObstacle obstacle = collision.gameObject.GetComponent<IObstacle>();
        if (obstacle != null) // Check if the collided object implements IObstacle
        {
            isDragging = obstacle.CollisionDetected(collision); // Call the interface method
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        IObstacle obstacle = collision.gameObject.GetComponent<IObstacle>();
        if (obstacle != null) // Check if the collided object implements IObstacle
        {
            isDragging = false;
        }
    }
}
