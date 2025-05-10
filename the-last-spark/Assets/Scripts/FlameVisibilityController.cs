using System.Collections;
using UnityEngine;

public class FlameVisibilityController : MonoBehaviour
{
    public Transform player;
    public float lightTriggerRadius = 3.0f;

    private bool isInLight = false;
    private ParticleSystem flameParticles;
    private Coroutine flickerRoutine;

    void Start()
    {
        // Find the player if not assigned
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        flameParticles = GetComponentInChildren<ParticleSystem>();

        flickerRoutine = StartCoroutine(RandomFlicker());
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        bool currentlyInLight = distance < lightTriggerRadius;

        if (currentlyInLight != isInLight)
        {
            isInLight = currentlyInLight;

            if (isInLight)
            { // Always on when in light
                if (!flameParticles.isPlaying)
                    flameParticles.Play();

                if (flickerRoutine != null)
                    StopCoroutine(flickerRoutine);
            }
            else
            { // Back to flicker logic
                flickerRoutine = StartCoroutine(RandomFlicker());
            }
        }
    }

    IEnumerator RandomFlicker()
    {
        while (!isInLight)
        { // Randomly show particles for a short time
            flameParticles.Play();
            yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));

            flameParticles.Stop();
            yield return new WaitForSeconds(Random.Range(2.5f, 5.0f));
        }
    }
}
