using UnityEngine;

public class WaterEnemy : MonoBehaviour
{
    public Transform player;
    public float speed = 1.0f;
    public float avoidDistance = 3.0f;
    private Vector2 currentDirection;

    void Start()
    {
        // Find the player if not assigned
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        currentDirection = Random.insideUnitCircle.normalized;
    }

    void Update()
    {
        if (player == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < avoidDistance)
        {
            // Chase the player
            Vector2 dir = (player.position - transform.position).normalized;
            transform.position += (Vector3)(dir * speed * Time.deltaTime);
        }
        else
        {
            // Move in a straight line
            transform.position += (Vector3)(currentDirection * speed * Time.deltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            if (collision.contacts.Length > 0)
            {
                Vector2 normal = collision.contacts[0].normal;
                currentDirection = Vector2.Reflect(currentDirection, normal).normalized;
            }
            else
            {
                currentDirection = Random.insideUnitCircle.normalized; // Fallback
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerLight playerLight = other.GetComponent<PlayerLight>();
            if (playerLight != null)
            {
                playerLight.DecreaseLight(1.0f); // Adjust damage value as needed
                if (playerLight.GetCurrentLightRadius() <= 0f)
                {
                    // Game over logic here
                    Debug.Log("Game Over: Light extinguished.");
                }
            }

            Destroy(gameObject);

            GameManager.Instance.SpawnEnemy(); // Call the spawn method from GameManager
        }
    }
}
