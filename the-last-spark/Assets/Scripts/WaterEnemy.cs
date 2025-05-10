using System.Collections;
using UnityEngine;

public class WaterEnemy : MonoBehaviour
{
    public Transform player;
    public float speed = 1.0f;
    public float avoidDistance = 3.0f;
    public float enemyDecreaseAmount = 1.0f;

    private Vector2 currentDirection;
    private AudioSource audioSource;

    void Start()
    {
        // Find the player if not assigned
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

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
        else if (collision.collider.CompareTag("Player"))
        {
            PlayerLight pl = collision.collider.GetComponent<PlayerLight>();
            StartCoroutine(DestroyCoroutine(pl));
        }
    }

    private IEnumerator DestroyCoroutine(PlayerLight playerLight)
    {
        playerLight.DecreaseLight(enemyDecreaseAmount); // Adjust damage value as needed
        if (playerLight.GetCurrentLightRadius() <= 0f)
        {
            // Game over logic here
            Debug.Log("Game Over: Light extinguished.");
        }
        audioSource.Play();
        gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(audioSource.clip.length);
        Destroy(gameObject);

        yield return new WaitForSeconds(1f);
        GameManager.Instance.SpawnEnemy(); // Call the spawn method from GameManager
    }
}
