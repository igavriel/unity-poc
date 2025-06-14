using UnityEngine;

public class ObstacleBehavior : MonoBehaviour, IObstacle
{
    public bool CollisionDetected(Collision2D collision)
    {
        // This method can be used to handle specific collision logic if needed
        Debug.Log("Collision detected with obstacle: " + gameObject.name);
        if (collision.gameObject.CompareTag("Floor"))
        {
            Destroy(gameObject);
        }
        return true; // Indicating a collision has occurred
    }
}
