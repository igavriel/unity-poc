using UnityEngine;

public class SeagullFlight : MonoBehaviour
{
    public float speed = 5f;        // Speed of horizontal movement
    public float amplitude = 1f;    // Amplitude of the sine wave (vertical movement)
    public float frequency = 1f;    // Frequency of the sine wave

    [Header("Colliders")]
    public Collider2D edgeCollider;    // Reference to the specific box collider for collision detection
    public Collider2D ballCollider;    // Reference to the ball collider

    // Starting position
    private Vector3 startPosition;

    void Start()
    {
        // Save the starting position of the sprite
        startPosition = transform.position;
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
            // You can add your logic here, e.g., destroy the seagull or play a sound
            Destroy(gameObject);
        }
    }

    private System.Collections.IEnumerator HandleResetToRightPosition()
    {
        int wait = UnityEngine.Random.Range(5, 10); //1,2);
        Debug.Log($"Next seagull in {wait} seconds");

        yield return new WaitForSeconds(wait);
        // Resetting to right position
        transform.position = startPosition;
    }
}
