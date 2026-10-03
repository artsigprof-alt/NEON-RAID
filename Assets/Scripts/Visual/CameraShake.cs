using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("General")]
    [SerializeField] private float positionMultiplier = 1f;
    [SerializeField] private float maxShakeDistance = 0.15f;

    [Header("Player Shot")]
    [SerializeField] private float playerShotDuration = 0.04f;
    [SerializeField] private float playerShotStrength = 0.003f;

    [Header("Boss Shot")]
    [SerializeField] private float bossShotDuration = 0.06f;
    [SerializeField] private float bossShotStrength = 0.01f;

    [Header("Boss Explosion")]
    [SerializeField] private float bossExplosionDuration = 0.45f;
    [SerializeField] private float bossExplosionStrength = 0.12f;

    public Vector3 ShakeOffset { get; private set; }

    private float shakeTimer;
    private float shakeDuration;
    private float shakeStrength;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void LateUpdate()
    {
        UpdateShake();
    }

    public void PlayerShot()
    {
        StartShake(
            playerShotDuration,
            playerShotStrength
        );
    }

    public void BossShot()
    {
        StartShake(
            bossShotDuration,
            bossShotStrength
        );
    }

    public void BossExplosion()
    {
        StartShake(
            bossExplosionDuration,
            bossExplosionStrength
        );
    }

    public void Shake(float duration, float strength)
    {
        StartShake(duration, strength);
    }

    private void StartShake(
        float duration,
        float strength)
    {
        strength *= positionMultiplier;

        // Берём максимальное значение,
        // чтобы новый shake не обнулял старый.
        if (duration > shakeTimer)
            shakeTimer = duration;

        shakeDuration = Mathf.Max(
            shakeDuration,
            duration
        );

        shakeStrength = Mathf.Max(
            shakeStrength,
            strength
        );
    }

    private void UpdateShake()
    {
        if (shakeTimer <= 0f)
        {
            ShakeOffset =
                Vector3.Lerp(
                    ShakeOffset,
                    Vector3.zero,
                    20f * Time.deltaTime
                );

            return;
        }

        shakeTimer -= Time.deltaTime;

        float progress =
            shakeDuration > 0f
                ? Mathf.Clamp01(
                    shakeTimer / shakeDuration
                )
                : 0f;

        float currentStrength =
            shakeStrength * progress;

        Vector2 randomOffset =
            Random.insideUnitCircle *
            currentStrength;

        randomOffset =
            Vector2.ClampMagnitude(
                randomOffset,
                maxShakeDistance
            );

        ShakeOffset =
            new Vector3(
                randomOffset.x,
                randomOffset.y,
                0f
            );

        if (shakeTimer <= 0f)
        {
            shakeTimer = 0f;
            shakeStrength = 0f;
            shakeDuration = 0f;
        }
    }
}