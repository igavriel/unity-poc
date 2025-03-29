using UnityEngine;

public class BallController : MonoBehaviour
{
    public float bounceForce = 10f;
    public float kickForce = 12f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
