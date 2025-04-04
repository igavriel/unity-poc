using UnityEngine;

public class BallController : MonoBehaviour
{
    public float bounceForce = 10f;
    public float kickForce = 12f;
    private Rigidbody2D rb;

    private int kickCount = 0;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    public void BounceUp()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
        AddKickCount();
    }

    public void Kick(Vector2 direction)
    {
        rb.linearVelocity = direction * kickForce;
        AddKickCount();
    }

    private void AddKickCount()
    {
        if(audioSource != null)
        {
            audioSource.Play();
        }
        // Increment the kick count
        kickCount++;
        //Debug.Log($"Ball kicked {kickCount} times");
    }

    public void ResetKickCount()
    {
        // Reset the kick count
        Debug.Log($"Ball kicked {kickCount} times before hitting the ground");
        kickCount = 0;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the ball collides with the ground
        Debug.Log($"Ball collided with {collision.collider.name}");
        if (collision.collider.name == "Ground")
        {
            ResetKickCount();
        }
    }
}
