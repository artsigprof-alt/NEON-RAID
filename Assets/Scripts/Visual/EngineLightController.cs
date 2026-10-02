using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EngineLightController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Light2D engineLight;

    [SerializeField] private float normalIntensity = 0.5f;
    [SerializeField] private float boostIntensity = 2f;

    private void Update()
    {
        if (player == null ||
            engineLight == null)
            return;

        float target =
            player.IsBoosting
                ? boostIntensity
                : normalIntensity;

        engineLight.intensity =
            Mathf.Lerp(
                engineLight.intensity,
                target,
                12f * Time.deltaTime
            );
    }
}