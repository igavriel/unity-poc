using System.Collections;
using UnityEngine;

public class WaterEnemy : MonoBehaviour
{
    public float enemyDecreaseAmount = 1.5f;

    private float avoidDistance = 3.0f;
    private float normalSpeed = 1.0f;
    private float chaseSpeed = 1.5f;
    private Transform player;
    private Vector2 currentDirection;
    private AudioSource audioSource;
    private PlayerLight playerLight;

    void Start()
    {
        // Find the player if not assigned
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        playerLight = player.GetComponent<PlayerLight>();
        avoidDistance = playerLight.GetCurrentLightRadius() - 0.5f;
        // Ensure avoidDistance is not too small
        if (avoidDistance < 5.0f)
            avoidDistance = 5.0f;

        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        normalSpeed = playerMovement.GetSpeed() + Random.Range(-0.5f, 0.5f);
        chaseSpeed = normalSpeed + Random.Range(0.1f, 0.8f);

        audioSource = GetComponent<AudioSource>();

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
            transform.position += (Vector3)(dir * chaseSpeed * Time.deltaTime);
        }
        else
        {
            // Move in a straight line
            transform.position += (Vector3)(currentDirection * normalSpeed * Time.deltaTime);
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
        else if (collision.collider.CompareTag("Player"))
        {
            StartCoroutine(HandleEnemyDestroy());
        }
    }

    private IEnumerator HandleEnemyDestroy()
    {
        audioSource.Play();
        playerLight.DecreaseLight(enemyDecreaseAmount); // Adjust damage value as needed
        if (playerLight.GetCurrentLightRadius() <= 0f)
        {
            // Game over logic here
            Debug.Log("Game Over: Light extinguished.");
            GameManager.Instance.GameOver("Light extinguished.");
        }
        else
        {
            // stop collision with player
            gameObject.GetComponent<Collider2D>().enabled = false;
            // hide sprite
            gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;

            yield return new WaitForSeconds(audioSource.clip.length / 2);
            GameManager.Instance.SpawnEnemy(); // Call the spawn method from GameManager
            Destroy(gameObject);
        }
    }
}
