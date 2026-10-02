using UnityEngine;

public class EngineVFX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private ParticleSystem engineParticles;
    [SerializeField] private ParticleSystem boostParticles;
    [SerializeField] private Transform enginePoint;

    [Header("Normal Engine")]
    [SerializeField] private float normalEmission = 20f;
    [SerializeField] private float movingEmission = 45f;

    [Header("Boost")]
    [SerializeField] private float boostEmission = 100f;

    [Header("Size")]
    [SerializeField] private float normalLength = 0.7f;
    [SerializeField] private float movingLength = 1.2f;
    [SerializeField] private float boostLength = 2.2f;

    private ParticleSystem.EmissionModule engineEmission;
    private ParticleSystem.MainModule engineMain;

    private ParticleSystem.EmissionModule boostEmissionModule;
    private ParticleSystem.MainModule boostMain;

    private void Awake()
    {
        if (engineParticles != null)
        {
            engineEmission =
                engineParticles.emission;

            engineMain =
                engineParticles.main;
        }

        if (boostParticles != null)
        {
            boostEmissionModule =
                boostParticles.emission;

            boostMain =
                boostParticles.main;
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        float speed =
            player.Velocity.magnitude;

        float maxSpeed =
            7f;

        if (PlayerStats.Instance != null)
        {
            maxSpeed =
                PlayerStats.Instance.MoveSpeed;
        }

        float speedPercent =
            Mathf.Clamp01(
                speed / maxSpeed
            );

        bool boosting =
            player.IsBoosting;

        // ==================================
        // NORMAL ENGINE
        // ==================================

        if (engineParticles != null)
        {
            float emission =
                Mathf.Lerp(
                    normalEmission,
                    movingEmission,
                    speedPercent
                );

            engineEmission.rateOverTime =
                emission;

            float length =
                Mathf.Lerp(
                    normalLength,
                    movingLength,
                    speedPercent
                );

            engineMain.startLifetime =
                Mathf.Lerp(
                    0.08f,
                    0.18f,
                    speedPercent
                );

            engineMain.startSpeed =
                Mathf.Lerp(
                    0.5f,
                    1.5f,
                    speedPercent
                );
        }

        // ==================================
        // BOOST
        // ==================================

        if (boostParticles != null)
        {
            boostEmissionModule.rateOverTime =
                boosting
                    ? boostEmission
                    : 0f;

            boostMain.startLifetime =
                boosting
                    ? 0.25f
                    : 0.1f;

            boostMain.startSpeed =
                boosting
                    ? 3f
                    : 0f;
        }
    }
}