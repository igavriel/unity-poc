using UnityEngine;

public class ObstacleBehavior : MonoBehaviour, IObstacle
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            Destroy(gameObject);
        }
    }

    public bool CollisionDetected()
    {
        // This method can be used to handle specific collision logic if needed
        Debug.Log("Collision detected with obstacle: " + gameObject.name);
        return true; // Indicating a collision has occurred
    }
}
