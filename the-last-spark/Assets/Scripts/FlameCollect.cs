using System.Collections;
using UnityEngine;

public class FlameCollect : MonoBehaviour
{
    public float lightIncreaseAmount = 1.0f;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerLight pl = other.GetComponent<PlayerLight>();
            StartCoroutine(DestroyCoroutine(pl));
        }
    }

    private IEnumerator DestroyCoroutine(PlayerLight playerLight)
    {
        playerLight.UpgradeLight(lightIncreaseAmount);
        audioSource.Play();
        gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(audioSource.clip.length);
        Destroy(gameObject);
    }
}
