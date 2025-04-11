using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region public properties
    [SerializeField] private KeyCode actionKey = KeyCode.Space;
    [SerializeField] private float actionDuration = 1.0f;
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private Animator animator; // Animator component for player animations

    [Header("Jump")]
    [SerializeField] private float jumpForce = 20f; // Force applied when jumping
    [SerializeField] private float groundCheckRadius = 0.2f; // Radius of the ground check
    [SerializeField] private LayerMask groundLayer; // Layer that represents the ground
    [SerializeField] private Transform groundCheckTarget; // Position to check for the ground

    [Header("Ball")]
    [SerializeField] private float footCheckRadius = 0.5f; // Radius of the foot check
    [SerializeField] private LayerMask ballLayer; // Layer that represents the ball
    [SerializeField] private Transform ballTarget;

    [Header("Colliders and Transform - Player Internal")]
    [SerializeField] private Transform head;
    [SerializeField] private Transform leftFoot;
    [SerializeField] private Transform rightFoot;
    [SerializeField] private Collider2D headCollider;
    [SerializeField] private Collider2D leftFootCollider;
    [SerializeField] private Collider2D rightFootCollider;
    #endregion

    #region private properties
    private Rigidbody2D rb;
    private bool isGrounded;
    #endregion

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
        // Update animator parameters
        animator.SetFloat("Speed", Mathf.Abs(moveX));

        rb.linearVelocity = new Vector2(moveX * walkSpeed, rb.linearVelocity.y);

        // Check if the player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheckTarget.position, groundCheckRadius, groundLayer);

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
        if (ballIsAbove)
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
                rightFootCollider.enabled = false;
                animator.SetTrigger("KickLeft");
            }
            else
            {
                //Debug.Log($"Right Kick");
                activeCollider = rightFootCollider;
                leftFootCollider.enabled = false;
                animator.SetTrigger("KickRight");
            }
        }
        activeCollider.enabled = true;
        yield return new WaitForSeconds(actionDuration);
        activeCollider.enabled = false;
    }

    void Kick(Vector2 footPosition, Vector2 direction, string trick)
    {
        //Debug.Log($"Kick check foot={footPosition.x},{footPosition.y}, dir={direction.x},{direction.y}");
        Collider2D ball = Physics2D.OverlapCircle(footPosition, footCheckRadius, ballLayer);

        if (ball && ball.CompareTag("Ball"))
        {
            //Debug.Log($"Kick ball");
            BallController ballController = ball.GetComponent<BallController>();
            if (ballController)
            {
                //Debug.Log($"Kick direction {direction.x}, {direction.y}");
                ballController.Kick(direction, trick);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log($"OnCollisionEnter2D with {collision.GetType()} - name {collision.gameObject.name}");
        if (collision.gameObject.CompareTag("Ball"))
        {
            //Debug.Log($"OnCollisionEnter2D this is the Ball with {collision.GetType()} - name {collision.gameObject.name}");
            BallController ball = ballTarget.GetComponent<BallController>();
            // Bounce up if above head
            if (ball && collision.transform.position.y > head.position.y)
            {
                //Debug.Log("OnCollisionEnter2D Bounce up");
                ball.BounceUp("Bounce");
            }
            else
            {
                // Debug.Log($"OnCollisionEnter2D kick left or right with {collision.GetType()} - name {collision.gameObject.name}");
                // kick left or right
                if (leftFootCollider.enabled)
                {
                    //Debug.Log("Kick left");
                    float angle = UnityEngine.Random.Range(-90f, -70f);
                    Vector2 direction = DegreeToVector2(angle);
                    Kick(leftFoot.position, direction, $"Left Kick");
                }
                else if (rightFootCollider.enabled)
                {
                    //Debug.Log("Kick right");
                    float angle = UnityEngine.Random.Range(70f, 90f);
                    Vector2 direction = DegreeToVector2(angle);
                    Kick(rightFoot.position, direction, $"Right Kick");
                }
                // else
                // {
                //     Debug.Log("NO FOOT COLLIDER ENABLED");
                // }
            }
        }

        Vector2 DegreeToVector2(float angleInDegrees)
        {
            float rad = angleInDegrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckTarget.position, groundCheckRadius);
        Gizmos.DrawWireSphere(ballTarget.position, footCheckRadius);
        Gizmos.DrawWireSphere(leftFoot.position, footCheckRadius);
        Gizmos.DrawWireSphere(rightFoot.position, footCheckRadius);
    }
}
