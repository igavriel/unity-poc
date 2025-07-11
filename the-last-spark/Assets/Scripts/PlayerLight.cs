using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLight : MonoBehaviour
{
    [Header("Score")]
    [SerializeField]
    private int flameCount = 0; // Number of flames collected

    [SerializeField]
    private int hitCount = 0; // Number of water hits taken

    [Header("Light Settings")]
    public float maxRadius = 20.0f;
    public float initialMaxRadius = 15.0f; // Initial light radius after new game
    public float initialStartRadius = 2.0f; // Initial light radius after new game


    private Light2D lanternLight;

    void Start()
    {
        lanternLight = GetComponentInChildren<Light2D>();
        NewGame();
    }

    public void NewGame()
    {
        // Reset the flame and hit counts
        flameCount = 0;
        hitCount = 0;
        StartCoroutine(HandleNewGameLightReduce());
    }

    private IEnumerator HandleNewGameLightReduce()
    {
        // Gradually reduce the light radius to the initial value over 2 seconds
        lanternLight.pointLightOuterRadius = initialMaxRadius;
        float duration = 2f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            lanternLight.pointLightOuterRadius = Mathf.Lerp(
                initialMaxRadius,
                initialStartRadius,
                t);
            yield return null;
        }
    }

    // Returns the current light radius of the lantern.
    public float GetCurrentLightRadius()
    {
        return lanternLight.pointLightOuterRadius;
    }

    // Returns the current flame count.
    public int GetFlameCount()
    {
        return flameCount;
    }

    // Returns the current hit count.
    public int GetHitCount()
    {
        return hitCount;
    }

    // Upgrades the light radius of the lantern.
    public void UpgradeLight(float amount)
    {
        // Increase the flame count and update the light radius.
        flameCount++;
        lanternLight.pointLightOuterRadius = Mathf.Min(
            lanternLight.pointLightOuterRadius + amount,
            maxRadius
        );
    }

    // Decreases the light radius of the lantern.
    public void DecreaseLight(float amount, bool isHit)
    {
        if (isHit)
        { // Increase the hit count and update the light radius.
            hitCount++;
        }
        // Decrease the light radius and clamp it to a minimum value.
        lanternLight.pointLightOuterRadius = Mathf.Max(
            lanternLight.pointLightOuterRadius - amount,
            0f
        );
    }
}
