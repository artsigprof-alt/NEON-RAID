using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 360f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 20;

    [Header("Damage")]
    [SerializeField] private int contactDamage = 10;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Reward")]
    [SerializeField] private int scrapReward = 10;

    [Header("Visual")]
    [SerializeField] private float spriteRotationOffset = -90f;

    private int currentHealth;

    private float scaledMoveSpeed;
    private float scaledContactDamage;
    private int scaledMaxHealth;

    private Transform player;
    private PlayerHealth playerHealth;

    private float nextAttackTime;
    private bool dead;

    private Rigidbody2D rb;

    private void Awake()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
            return;

        player =
            playerObject.transform;

        playerHealth =
            playerObject.GetComponent<PlayerHealth>();

        int upgrades = 0;

        if (PlayerStats.Instance != null)
            upgrades =
                PlayerStats.Instance.TotalUpgradeLevels;

        float healthMultiplier =
            EnemyDifficulty.GetHealthMultiplier(
                upgrades
            );

        float damageMultiplier =
            EnemyDifficulty.GetDamageMultiplier(
                upgrades
            );

        float speedMultiplier =
            EnemyDifficulty.GetSpeedMultiplier(
                upgrades
            );

        // =====================================================
        // NORMAL SCALING
        // =====================================================

        scaledMaxHealth =
            Mathf.RoundToInt(
                maxHealth *
                healthMultiplier
            );

        scaledContactDamage =
            contactDamage *
            damageMultiplier;

        scaledMoveSpeed =
            moveSpeed *
            speedMultiplier;

        // =====================================================
        // HARD MODE SCALING
        // =====================================================

        if (HardModeController.IsActive)
        {
            scaledMaxHealth =
                Mathf.Max(
                    1,
                    Mathf.RoundToInt(
                        scaledMaxHealth *
                        HardModeController.EnemyHealthMultiplier
                    )
                );

            scaledContactDamage *=
                HardModeController.EnemyDamageMultiplier;

            scaledMoveSpeed *=
                HardModeController.EnemySpeedMultiplier;
        }

        currentHealth =
            scaledMaxHealth;
    }

    private void Update()
    {
        if (dead || player == null)
            return;

        Vector2 toPlayer =
            (Vector2)player.position -
            (Vector2)transform.position;

        if (toPlayer.sqrMagnitude < 0.001f)
            return;

        Vector2 direction =
            toPlayer.normalized;

        RotateTowards(direction);
        MoveTowards(direction);
    }

    private void RotateTowards(
        Vector2 direction
    )
    {
        float targetAngle =
            Mathf.Atan2(
                direction.y,
                direction.x
            ) *
            Mathf.Rad2Deg +
            spriteRotationOffset;

        float currentAngle =
            transform.eulerAngles.z;

        float newAngle =
            Mathf.MoveTowardsAngle(
                currentAngle,
                targetAngle,
                rotationSpeed *
                Time.deltaTime
            );

        transform.rotation =
            Quaternion.Euler(
                0f,
                0f,
                newAngle
            );
    }

    private void MoveTowards(
        Vector2 direction
    )
    {
        Vector3 movement =
            (Vector3)(
                direction *
                scaledMoveSpeed *
                Time.deltaTime
            );

        if (rb != null &&
            rb.bodyType ==
            RigidbodyType2D.Kinematic)
        {
            rb.MovePosition(
                rb.position +
                (Vector2)movement
            );
        }
        else
        {
            transform.position +=
                movement;
        }
    }

    private void OnCollisionStay2D(
        Collision2D collision
    )
    {
        if (dead)
            return;

        if (!collision.gameObject.CompareTag(
                "Player"))
            return;

        if (Time.time < nextAttackTime)
            return;

        if (playerHealth != null)
        {
            int damageToDeal;

            if (HardModeController.IsActive)
            {
                damageToDeal =
                    Mathf.RoundToInt(
                        scaledContactDamage
                    );
            }
            else
            {
                damageToDeal =
                    contactDamage;
            }

            playerHealth.TakeDamage(
                damageToDeal
            );
        }

        nextAttackTime =
            Time.time +
            attackCooldown;
    }

    public void TakeDamage(int amount)
    {
        if (dead)
            return;

        currentHealth -=
            Mathf.Max(
                0,
                amount
            );

        CombatVFX.SpawnHit(
            transform.position
        );

        if (ProceduralAudioManager.Instance != null)
        {
            ProceduralAudioManager.Instance.PlaySfx(
                AudioSfxType.Hit,
                0.45f
            );
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (dead)
            return;

        dead = true;

        CombatVFX.SpawnExplosion(
            transform.position,
            0.7f
        );

        if (ProceduralAudioManager.Instance != null)
        {
            ProceduralAudioManager.Instance.PlaySfx(
                AudioSfxType.EnemyDeath
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

        GameManager.Instance?.EnemyKilled();

        Destroy(gameObject);
    }
}