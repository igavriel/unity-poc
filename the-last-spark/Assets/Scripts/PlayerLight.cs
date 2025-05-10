using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLight : MonoBehaviour
{
    [SerializeField]
    private int flameCount = 0; // Number of flames collected

    [SerializeField]
    private int hitCount = 0; // Number of water hits taken

    private Light2D lanternLight;
    private float maxRadius = 8f;

    void Start()
    {
        lanternLight = GetComponentInChildren<Light2D>();
    }

    // Returns the current light radius of the lantern.
    public float GetCurrentLightRadius()
    {
        return lanternLight.pointLightOuterRadius;
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
    public void DecreaseLight(float amount)
    {
        // Increase the hit count and update the light radius.
        hitCount++;
        // Decrease the light radius and clamp it to a minimum value.
        lanternLight.pointLightOuterRadius = Mathf.Max(
            lanternLight.pointLightOuterRadius - amount,
            0f
        );
    }
}
