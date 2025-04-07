using UnityEngine;

public class BallController : MonoBehaviour
{
    public float bounceForce = 10f;
    public float kickForce = 12f;
    public GameManager gameManager;
    private Rigidbody2D rb;
    private int kickCount = 0;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    public void BounceUp(string trick)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
        AddKickCount(trick, 20);
    }

    public void Kick(Vector2 direction, string trick)
    {
        rb.linearVelocity = direction * kickForce;
        AddKickCount(trick, 10);
    }

    private void AddKickCount(string trick, int score)
    {
        if(audioSource != null)
        {
            audioSource.Play();
        }
        // Increment the kick count
        kickCount++;
        //Debug.Log($"Ball kicked {kickCount} times");
        gameManager.UpdateTrickText(trick, score);
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
