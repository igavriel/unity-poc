using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public KeyCode actionKey = KeyCode.Space;
    public float actionDuration = 1.0f;

    [Header("Jump")]
    public float jumpForce = 20f; // Force applied when jumping
    public Transform groundCheck; // Position to check for the ground
    public float groundCheckRadius = 0.2f; // Radius of the ground check
    public LayerMask groundLayer; // Layer that represents the ground

    [Header("Colliders and Transform")]
    public Transform ballTarget;
    public Transform head;
    public Transform leftFoot;
    public Transform rightFoot;
    public Collider2D headCollider;
    public Collider2D leftFootCollider;
    public Collider2D rightFootCollider;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        headCollider.enabled = false;
        leftFootCollider.enabled = false;
        rightFootCollider.enabled = false;
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveX * speed, rb.linearVelocity.y);

        // Check if the player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Handle Kick if action pressed
        bool actionPressed = Input.GetKey(actionKey);
        if (actionPressed)
        {
            StartCoroutine(HandleKick());
        }
    }

    private System.Collections.IEnumerator HandleKick()
    {
        Collider2D activeCollider = null;
        // Determine appropriate collider based on ball position
        bool ballIsAbove = ballTarget.position.y > transform.position.y;
        if(ballIsAbove)
        {
            //Debug.Log($"ball is Above");
            activeCollider = headCollider;
             // Jump if grounded
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }
        else
        {
            float leftDist = Math.Abs(ballTarget.position.x - leftFootCollider.transform.position.x);
            float rightDist = Math.Abs(ballTarget.position.x - rightFootCollider.transform.position.x);
            if (leftDist < rightDist)
            {
                //Debug.Log($"Left Kick");
                activeCollider = leftFootCollider;
            }
            else
            {
                //Debug.Log($"Right Kick");
                activeCollider = rightFootCollider;
            }
        }
        activeCollider.enabled = true;
        yield return new WaitForSeconds(actionDuration);
        activeCollider.enabled = false;
    }

    void Kick(Vector2 footPosition, Vector2 direction)
    {
        //Debug.Log($"Kick check foot={footPosition.x},{footPosition.y}, dir={direction.x},{direction.y}");
        Collider2D ball = Physics2D.OverlapCircle(footPosition, 0.5f);
        if (ball && ball.CompareTag("Ball"))
        {
            //Debug.Log($"Kick ball");
            BallController ballController = ball.GetComponent<BallController>();
            if (ballController)
            {
                //Debug.Log($"Kick direction {direction.x}, {direction.y}");
                ballController.Kick(direction);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log($"OnCollisionEnter2D with {collision.GetType()} - name {collision.gameObject.name}");
        if (collision.gameObject.CompareTag("Ball"))
        {
            BallController ball = ballTarget.GetComponent<BallController>();
            // Bounce up if above head
            if (ball && collision.transform.position.y > head.position.y)
            {
                //Debug.Log("Bounce up");
                ball.BounceUp();
            }
            else
            {   // kick left or right
                if(leftFootCollider.enabled)
                {
                    //Debug.Log("Collider ball left");
                    Collider2D ballNearLeft = Physics2D.OverlapCircle(leftFoot.position, 0.5f);
                    if (ballNearLeft && ballNearLeft.CompareTag("Ball"))
                    {
                        //Debug.Log("Kick left");
                        Kick(leftFoot.position, new Vector2(-0.7f, 1f));
                    }
                }
                else if(rightFootCollider.enabled)
                {
                    Collider2D ballNearRight = Physics2D.OverlapCircle(rightFoot.position, 0.5f);
                    if (ballNearRight && ballNearRight.CompareTag("Ball"))
                    {
                        //Debug.Log("Kick right");
                        Kick(rightFoot.position, new Vector2(0.7f, 1f));
                    }
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a circle in the editor to visualize the ground check area
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
