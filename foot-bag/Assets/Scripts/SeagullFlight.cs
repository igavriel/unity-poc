using UnityEditor;
using UnityEngine;

public class SeagullFlight : MonoBehaviour
{
    [SerializeField] private float speed = 5f;        // Speed of horizontal movement
    [SerializeField] private float amplitude = 1f;    // Amplitude of the sine wave (vertical movement)
    [SerializeField] private float frequency = 1f;    // Frequency of the sine wave

    [Header("Colliders")]
    [SerializeField] private Collider2D edgeCollider;    // Reference to the specific box collider for collision detection
    [SerializeField] private Collider2D ballCollider;    // Reference to the ball collider

    [Header("Physics Settings")]
    [SerializeField] private float rotationTorqueMultiplier = 2.5f; // Adjust for more/less spin
    [SerializeField] private float upwardForce = 5f; // Adds liftoff on hit
    [SerializeField] private float destroyTime = 2f; // Time before the seagull is destroyed

    [Header("Audio Clips")]
    [SerializeField] private AudioClip tweetSound; // Assign in Inspector
    [SerializeField] private AudioClip hitSound;   // Assign in Inspector

    // Starting position
    private Vector3 startPosition;
    private AudioSource audioSource;
    private Rigidbody2D rb;

    void Start()
    {
        // Save the starting position of the sprite
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(tweetSound);
    }

    void Update()
    {
        // Calculate new horizontal position (moving to the right)
        float horizontalPosition = transform.position.x + speed * Time.deltaTime;

        // Calculate new vertical position based on sine wave
        float verticalOffset = Mathf.Sin(Time.time * frequency) * amplitude;

        // Update the sprite's position
        transform.position = new Vector3(horizontalPosition, startPosition.y + verticalOffset, startPosition.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log($"OnTriggerEnter2D Collision detected with: {collision.name}");
        // Check if the object collided with the specific target collider
        if (collision == edgeCollider)
        {
            StartCoroutine(HandleResetToRightPosition());
        }
        else if (collision == ballCollider)
        {
            // Handle collision with the ball
            StartCoroutine(HandleCollisionWithBall(collision.transform));
        }
    }

    private System.Collections.IEnumerator HandleResetToRightPosition()
    {
        audioSource.Stop();
        int wait = UnityEngine.Random.Range(5, 10); //1,2);
        Debug.Log($"Next seagull in {wait} seconds");

        yield return new WaitForSeconds(wait);

        // Resetting to right position
        transform.position = startPosition;
        // Restart the audio
        audioSource.PlayOneShot(tweetSound);
    }

    private System.Collections.IEnumerator HandleCollisionWithBall(Transform hitPosition)
    {
        audioSource.PlayOneShot(hitSound);
        // Calculate hit direction and apply forces
        Vector2 hitDirection = (transform.position - hitPosition.transform.position).normalized;

        // Add spin based on hit angle (cross product for torque)
        float spinDirection = Vector2.Dot(hitDirection, Vector2.up) > 0 ? -1f : 1f;
        rb.AddTorque(spinDirection * rotationTorqueMultiplier, ForceMode2D.Impulse);
        rb.AddForce(hitDirection * upwardForce + Vector2.up * upwardForce, ForceMode2D.Impulse);

        // Optionally destroy the seagull after a short delay
        yield return new WaitForSeconds(destroyTime);

        // Reset the seagull's position and state
        rb.AddTorque(0, ForceMode2D.Impulse); // Stop the spin
        rb.linearVelocity = Vector2.zero; // Stop the movement
        rb.angularVelocity = 0; // Stop the rotation
        rb.rotation = 0; // Reset rotation

        transform.position = startPosition; // Reset position
        // Restart the audio
        audioSource.PlayOneShot(tweetSound);
    }
}
