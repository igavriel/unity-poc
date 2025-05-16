using System.Collections;
using UnityEngine;

public class WaterEnemy : MonoBehaviour
{
    public float enemyDecreaseAmount = 1.5f;
    public float avoidDistance = 5.0f;

    private float normalSpeed = 1.0f;
    private float chaseSpeed = 1.5f;
    private float speedOscillationTime;

    private float chaseTimer = 0f;
    private float pauseTimer = 0f;
    private bool isPausing = false;
    private float chaseDuration = 5f;
    private float pauseDuration = 0.5f;

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
        normalSpeed = playerMovement.GetSpeed() + Random.Range(-0.5f, -0.1f);
        chaseSpeed = normalSpeed + Random.Range(0.1f, 0.3f);

        audioSource = GetComponent<AudioSource>();

        currentDirection = Random.insideUnitCircle.normalized;
        speedOscillationTime = Random.Range(0f, Mathf.PI * 2); // Random phase offset
    }

    void Update()
    {
        if (player == null)
            return;

        speedOscillationTime += Time.deltaTime;
        float speedMultiplier = 0.5f * Mathf.Sin(speedOscillationTime); // Oscillates between -0.5 and 0.5

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < avoidDistance)
        {
            if (isPausing)
            {
                pauseTimer += Time.deltaTime;
                if (pauseTimer >= pauseDuration)
                {
                    isPausing = false;
                    pauseTimer = 0f;
                    chaseTimer = 0f;
                }
            }
            else
            {
                chaseTimer += Time.deltaTime;
                if (chaseTimer >= chaseDuration)
                {
                    isPausing = true;
                    chaseTimer = 0f;
                }
                else
                {
                    float currentSpeed = Mathf.Lerp(normalSpeed, chaseSpeed, speedMultiplier);
                    // Move towards the player
                    Vector2 dir = (player.position - transform.position).normalized;
                    transform.position += (Vector3)(dir * currentSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            transform.position += (Vector3)(currentDirection * normalSpeed * Time.deltaTime);
            chaseTimer = 0f;
            pauseTimer = 0f;
            isPausing = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            StartCoroutine(HandleEnemyDestroy());
        }
        //if (collision.collider.CompareTag("Wall"))
        else
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

    private IEnumerator HandleEnemyDestroy()
    {
        audioSource.Play();
        playerLight.DecreaseLight(enemyDecreaseAmount, true); // Adjust damage value as needed
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
