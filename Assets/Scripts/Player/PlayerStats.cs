
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Base Stats")]
    [SerializeField] private int baseDamage = 10;
    [SerializeField] private float baseFireRate = 0.25f;
    [SerializeField] private int baseHealth = 100;
    [SerializeField] private float baseSpeed = 7f;

    [Header("Upgrade Amounts")]
    [SerializeField] private int damagePerLevel = 3;
    [SerializeField] private float fireRateReductionPerLevel = 0.02f;
    [SerializeField] private int healthPerLevel = 20;
    [SerializeField] private float speedPerLevel = 0.4f;

    [Header("Costs")]
    [SerializeField] private int baseDamageCost = 100;
    [SerializeField] private int baseFireRateCost = 150;
    [SerializeField] private int baseHealthCost = 120;
    [SerializeField] private int baseSpeedCost = 130;

    [Header("Max Levels")]
    [SerializeField] private int maxDamageLevel = 10;
    [SerializeField] private int maxFireRateLevel = 10;
    [SerializeField] private int maxHealthLevel = 10;
    [SerializeField] private int maxSpeedLevel = 10;

    public int DamageLevel { get; private set; }
    public int FireRateLevel { get; private set; }
    public int HealthLevel { get; private set; }
    public int SpeedLevel { get; private set; }

    // =========================================================
    // FINAL STATS
    // =========================================================

    public int Damage =>
        baseDamage +
        DamageLevel *
        damagePerLevel;

    public float FireRate
    {
        get
        {
            float baseValue =
                Mathf.Max(
                    0.08f,
                    baseFireRate -
                    FireRateLevel *
                    fireRateReductionPerLevel
                );

            // FireRate = time between shots.
            // Smaller value = faster shooting.
            if (SkrReactorManager.Instance != null &&
                SkrReactorManager.Instance.IsActive)
            {
                baseValue *=
                    SkrReactorManager.Instance.FireRateMultiplier;
            }

            return Mathf.Max(
                0.05f,
                baseValue
            );
        }
    }

    public int MaxHealth =>
        baseHealth +
        HealthLevel *
        healthPerLevel;

    public float MoveSpeed
    {
        get
        {
            float baseValue =
                baseSpeed +
                SpeedLevel *
                speedPerLevel;

            if (SkrReactorManager.Instance != null &&
                SkrReactorManager.Instance.IsActive)
            {
                baseValue *=
                    SkrReactorManager.Instance.SpeedMultiplier;
            }

            return baseValue;
        }
    }

    // =========================================================
    // TOTAL UPGRADES
    // =========================================================

    public int TotalUpgradeLevels
    {
        get
        {
            return
                DamageLevel +
                FireRateLevel +
                HealthLevel +
                SpeedLevel;
        }
    }

    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        Load();
    }

    // =========================================================
    // DAMAGE
    // =========================================================

    public bool UpgradeDamage()
    {
        if (DamageLevel >= maxDamageLevel)
            return false;

        int cost =
            GetDamageCost();

        if (!SpendScrap(cost))
            return false;

        DamageLevel++;

        Save();

        return true;
    }

    // =========================================================
    // FIRE RATE
    // =========================================================

    public bool UpgradeFireRate()
    {
        if (FireRateLevel >= maxFireRateLevel)
            return false;

        int cost =
            GetFireRateCost();

        if (!SpendScrap(cost))
            return false;

        FireRateLevel++;

        Save();

        return true;
    }

    // =========================================================
    // HEALTH
    // =========================================================

    public bool UpgradeHealth()
    {
        if (HealthLevel >= maxHealthLevel)
            return false;

        int cost =
            GetHealthCost();

        if (!SpendScrap(cost))
            return false;

        HealthLevel++;

        Save();

        return true;
    }

    // =========================================================
    // SPEED
    // =========================================================

    public bool UpgradeSpeed()
    {
        if (SpeedLevel >= maxSpeedLevel)
            return false;

        int cost =
            GetSpeedCost();

        if (!SpendScrap(cost))
            return false;

        SpeedLevel++;

        Save();

        return true;
    }

    // =========================================================
    // SCRAP
    // =========================================================

    private bool SpendScrap(int amount)
    {
        if (CurrencyManager.Instance == null)
            return false;

        return CurrencyManager.Instance.SpendScrap(
            amount
        );
    }

    // =========================================================
    // COSTS
    // =========================================================

    public int GetDamageCost()
    {
        return Mathf.RoundToInt(
            baseDamageCost *
            Mathf.Pow(
                1.35f,
                DamageLevel
            )
        );
    }

    public int GetFireRateCost()
    {
        return Mathf.RoundToInt(
            baseFireRateCost *
            Mathf.Pow(
                1.35f,
                FireRateLevel
            )
        );
    }

    public int GetHealthCost()
    {
        return Mathf.RoundToInt(
            baseHealthCost *
            Mathf.Pow(
                1.35f,
                HealthLevel
            )
        );
    }

    public int GetSpeedCost()
    {
        return Mathf.RoundToInt(
            baseSpeedCost *
            Mathf.Pow(
                1.35f,
                SpeedLevel
            )
        );
    }

    // =========================================================
    // SAVE
    // =========================================================

    private void Save()
    {
        PlayerPrefs.SetInt(
            "DamageLevel",
            DamageLevel
        );

        PlayerPrefs.SetInt(
            "FireRateLevel",
            FireRateLevel
        );

        PlayerPrefs.SetInt(
            "HealthLevel",
            HealthLevel
        );

        PlayerPrefs.SetInt(
            "SpeedLevel",
            SpeedLevel
        );

        PlayerPrefs.Save();
    }

    // =========================================================
    // LOAD
    // =========================================================

    private void Load()
    {
        DamageLevel =
            PlayerPrefs.GetInt(
                "DamageLevel",
                0
            );

        FireRateLevel =
            PlayerPrefs.GetInt(
                "FireRateLevel",
                0
            );

        HealthLevel =
            PlayerPrefs.GetInt(
                "HealthLevel",
                0
            );

        SpeedLevel =
            PlayerPrefs.GetInt(
                "SpeedLevel",
                0
            );
    }
}
