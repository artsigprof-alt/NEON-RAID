using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.2f;
    [SerializeField] private float preferredDistance = 5f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 1000;
    [SerializeField] private int scrapReward = 500;

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private float fireInterval = 1.6f;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private int projectileDamage = 20;

    [Header("Scaling")]
    [SerializeField] private bool scaleWithPlayerUpgrades = true;

    public int CurrentHealth { get; private set; }

    public int MaxHealth =>
        maxHealth;

    private Transform player;

    private float nextFireTime;
    private bool dead;

    private float scaledMoveSpeed;
    private int scaledProjectileDamage;
    private float scaledProjectileSpeed;
    private float scaledFireInterval;

    private void Awake()
    {
        ProceduralAudioManager.Instance?.PlaySfx(
            AudioSfxType.BossEngine,
            0.8f
        );

        // =====================================================
        // BASE
        // =====================================================

        scaledMoveSpeed =
            moveSpeed;

        scaledProjectileDamage =
            projectileDamage;

        scaledProjectileSpeed =
            projectileSpeed;

        scaledFireInterval =
            fireInterval;

        // =====================================================
        // PLAYER UPGRADE SCALING
        // =====================================================

        if (scaleWithPlayerUpgrades &&
            PlayerStats.Instance != null)
        {
            int upgrades =
                PlayerStats.Instance.TotalUpgradeLevels;

            float healthMultiplier =
                EnemyDifficulty.GetBossHealthMultiplier(
                    upgrades
                );

            float damageMultiplier =
                EnemyDifficulty.GetBossDamageMultiplier(
                    upgrades
                );

            float speedMultiplier =
                EnemyDifficulty.GetBossSpeedMultiplier(
                    upgrades
                );

            maxHealth =
                Mathf.RoundToInt(
                    maxHealth *
                    healthMultiplier
                );

            scaledProjectileDamage =
                Mathf.RoundToInt(
                    projectileDamage *
                    damageMultiplier
                );

            scaledMoveSpeed =
                moveSpeed *
                speedMultiplier;
        }

        // =====================================================
        // HARD MODE
        // =====================================================

        if (HardModeController.IsActive)
        {
            maxHealth =
                Mathf.Max(
                    1,
                    Mathf.RoundToInt(
                        maxHealth *
                        HardModeController.BossHealthMultiplier
                    )
                );

            scaledProjectileDamage =
                Mathf.Max(
                    1,
                    Mathf.RoundToInt(
                        scaledProjectileDamage *
                        HardModeController.BossDamageMultiplier
                    )
                );

            scaledMoveSpeed *=
                HardModeController.BossSpeedMultiplier;

            scaledProjectileSpeed *=
                HardModeController.BossProjectileSpeedMultiplier;

            scaledFireInterval =
                Mathf.Max(
                    0.1f,
                    scaledFireInterval *
                    HardModeController.BossFireIntervalMultiplier
                );
        }

        CurrentHealth =
            maxHealth;
    }

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag(
                "Player"
            );

        if (playerObject != null)
        {
            player =
                playerObject.transform;
        }
    }

    private void Update()
    {
        if (dead || player == null)
            return;

        Move();

        if (Time.time >= nextFireTime)
        {
            FirePattern();

            nextFireTime =
                Time.time +
                scaledFireInterval;
        }
    }

    private void Move()
    {
        Vector2 toPlayer =
            player.position -
            transform.position;

        float distance =
            toPlayer.magnitude;

        if (distance > preferredDistance)
        {
            transform.position =
                Vector2.MoveTowards(
                    transform.position,
                    player.position,
                    scaledMoveSpeed *
                    Time.deltaTime
                );
        }
        else if (
            distance <
            preferredDistance * 0.7f
        )
        {
            Vector2 direction =
                -toPlayer.normalized;

            transform.position +=
                (Vector3)(
                    direction *
                    scaledMoveSpeed *
                    Time.deltaTime
                );
        }

        Vector2 aim =
            toPlayer.normalized;

        if (aim.sqrMagnitude > 0.01f)
        {
            float angle =
                Mathf.Atan2(
                    aim.y,
                    aim.x
                ) *
                Mathf.Rad2Deg;

            transform.rotation =
                Quaternion.Lerp(
                    transform.rotation,
                    Quaternion.Euler(
                        0f,
                        0f,
                        angle
                    ),
                    4f *
                    Time.deltaTime
                );
        }
    }

    private void FirePattern()
    {
        if (projectilePrefab == null)
            return;

        if (firePoints == null ||
            firePoints.Length == 0)
        {
            FireAtDirection(
                (
                    player.position -
                    transform.position
                ).normalized,
                transform.position
            );

            PlayBossShotSound();

            return;
        }

        foreach (Transform point in firePoints)
        {
            if (point == null)
                continue;

            Vector2 direction =
                (
                    player.position -
                    point.position
                ).normalized;

            FireAtDirection(
                direction,
                point.position
            );
        }

        PlayBossShotSound();
    }

    private void PlayBossShotSound()
    {
        if (ProceduralAudioManager.Instance != null)
        {
            ProceduralAudioManager.Instance.PlaySfx(
                AudioSfxType.BossShot
            );
        }
    }

    private void FireAtDirection(
        Vector2 direction,
        Vector2 position
    )
    {
        GameObject projectile =
            Instantiate(
                projectilePrefab,
                position,
                Quaternion.identity
            );

        BossProjectile projectileComponent =
            projectile.GetComponent<BossProjectile>();

        if (projectileComponent != null)
        {
            projectileComponent.Initialize(
                direction,
                scaledProjectileSpeed,
                scaledProjectileDamage
            );
        }
    }

    public void TakeDamage(int amount)
    {
        if (dead)
            return;

        CurrentHealth -=
            Mathf.Max(
                0,
                amount
            );

        CombatVFX.SpawnHit(
            transform.position
        );

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (dead)
            return;

        dead = true;

        if (ProceduralAudioManager.Instance != null)
        {
            ProceduralAudioManager.Instance.PlaySfx(
                AudioSfxType.BossDeath
            );
        }

        if (CurrencyManager.Instance != null)
        {
            int finalReward =
                scrapReward;

            if (SkrReactorManager.Instance != null)
            {
                finalReward =
                    SkrReactorManager.Instance.GetScrapReward(
                        scrapReward
                    );
            }

            CurrencyManager.Instance.AddScrap(
                finalReward
            );
        }

        GameManager.Instance?.BossDefeated();

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.BossExplosion();
        }

        CombatVFX.SpawnExplosion(
            transform.position,
            3f
        );

        Destroy(gameObject);
    }
}