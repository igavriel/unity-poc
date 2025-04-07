using UnityEngine;

public class SeagullFlight : MonoBehaviour
{
    public float speed = 5f;    // Speed of horizontal movement

    public float amplitude = 1f;    // Amplitude of the sine wave (vertical movement)

    public float frequency = 1f;    // Frequency of the sine wave

    private Vector3 startPosition;  // Starting position

    void Start()
    {
        // Save the starting position of the sprite
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate new horizontal position (moving to the right)
        float horizontalPosition = startPosition.x + speed * Time.time;

        // Calculate new vertical position based on sine wave
        float verticalOffset = Mathf.Sin(Time.time * frequency) * amplitude;

        // Update the sprite's position
        transform.position = new Vector3(horizontalPosition, startPosition.y + verticalOffset, startPosition.z);

        // If the sprite moves off-screen to the right, reset to the left side
        if (Camera.main != null)
        {
            float screenRightEdge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
            if (transform.position.x > screenRightEdge)
            {
                float screenLeftEdge = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
                transform.position = new Vector3(screenLeftEdge, transform.position.y, transform.position.z);
            }
        }
    }
}
