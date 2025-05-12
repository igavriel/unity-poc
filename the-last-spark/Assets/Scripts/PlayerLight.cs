using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLight : MonoBehaviour
{
    [SerializeField]
    private int flameCount = 0; // Number of flames collected

    [SerializeField]
    private int hitCount = 0; // Number of water hits taken

    public float maxRadius = 20.0f;

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
