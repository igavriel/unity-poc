using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // The player
    public float followSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 0f, -10f); // Keep camera behind player in 2D

    void Start()
    {
        // Find the player if not assigned
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void LateUpdate()
    {
        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
