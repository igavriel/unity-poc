using UnityEngine;

public class FlameCollect : MonoBehaviour
{
    public float lightIncreaseAmount = 1.0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerLight pl = other.GetComponent<PlayerLight>();
            pl.UpgradeLight(lightIncreaseAmount);
            Destroy(gameObject);
        }
    }
}
