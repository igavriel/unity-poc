using UnityEngine;
using UnityEngine.SceneManagement;

public class BallBoundsController : MonoBehaviour
{
    public Transform boundsArea; // Drag your invisible bounds object here

    private float minX, maxX, minY, maxY;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Calculate bounds based on object's position and scale
        Vector2 center = boundsArea.position;
        Vector2 size = boundsArea.localScale;

        float halfWidth = size.x / 2f;
        float halfHeight = size.y / 2f;

        minX = center.x - halfWidth;
        maxX = center.x + halfWidth;
        minY = center.y - halfHeight;
        maxY = center.y + halfHeight;
    }

    void FixedUpdate()
    {
        Vector2 pos = transform.position;

        // Game Over if ball falls below the region
        if (pos.y < minY)
        {
            Debug.Log("Game Over: Ball fell below bounds.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        float clampedX = Mathf.Clamp(pos.x, minX, maxX);
        float clampedY = pos.y;

        if (pos.x < minX || pos.x > maxX || pos.y < minY || pos.y > maxY)
        {
            rb.position = new Vector2(clampedX, clampedY);
            rb.linearVelocity = Vector2.zero;
        }
    }

    void OnDrawGizmos()
    {
        if (boundsArea != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(boundsArea.position, boundsArea.localScale);
        }
    }
}
