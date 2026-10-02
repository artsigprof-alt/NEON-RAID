
public static class EnemyDifficulty
{
    // =========================================================
    // NORMAL ENEMIES
    // =========================================================

    private const float HealthPerUpgrade = 0.06f;
    private const float DamagePerUpgrade = 0.04f;
    private const float SpeedPerUpgrade = 0.02f;

    // =========================================================
    // BOSS
    // =========================================================

    private const float BossHealthPerUpgrade = 0.30f;
    private const float BossDamagePerUpgrade = 0.20f;
    private const float BossSpeedPerUpgrade = 0.10f;

    // =========================================================
    // NORMAL
    // =========================================================

    public static float GetHealthMultiplier(
        int upgrades
    )
    {
        float multiplier =
            1f +
            upgrades *
            HealthPerUpgrade;

        // SKR Reactor adds additional enemy HP.
        if (SkrReactorManager.Instance != null &&
            SkrReactorManager.Instance.IsActive)
        {
            multiplier *=
                SkrReactorManager.Instance
                    .EnemyHealthMultiplier;
        }

        return multiplier;
    }

    public static float GetDamageMultiplier(
        int upgrades
    )
    {
        return
            1f +
            upgrades *
            DamagePerUpgrade;
    }

    public static float GetSpeedMultiplier(
        int upgrades
    )
    {
        return
            1f +
            upgrades *
            SpeedPerUpgrade;
    }

    // =========================================================
    // BOSS
    // =========================================================

    public static float GetBossHealthMultiplier(
        int upgrades
    )
    {
        float multiplier =
            1f +
            upgrades *
            BossHealthPerUpgrade;

        // SKR Reactor also affects Boss HP.
        if (SkrReactorManager.Instance != null &&
            SkrReactorManager.Instance.IsActive)
        {
            multiplier *=
                SkrReactorManager.Instance
                    .EnemyHealthMultiplier;
        }

        return multiplier;
    }

    public static float GetBossDamageMultiplier(
        int upgrades
    )
    {
        return
            1f +
            upgrades *
            BossDamagePerUpgrade;
    }

    public static float GetBossSpeedMultiplier(
        int upgrades
    )
    {
        return
            1f +
            upgrades *
            BossSpeedPerUpgrade;
    }
}

