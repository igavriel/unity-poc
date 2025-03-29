using UnityEngine;

public class BallController : MonoBehaviour
{
    public float bounceForce = 10f;
    public float kickForce = 12f;
    private Rigidbody2D rb;
    private CircleCollider2D collider2D;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<CircleCollider2D>();
    }

    public void BounceUp()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
    }

    public void Kick(Vector2 direction)
    {
        rb.linearVelocity = direction * kickForce;
    }
}