using UnityEngine;

public class HardModeController : MonoBehaviour
{
    public static HardModeController Instance { get; private set; }

    public static bool IsActive
    {
        get
        {
            return Instance != null;
        }
    }

    [Header("ENEMY DIFFICULTY")]
    [SerializeField, Min(1f)]
    private float enemyHealthMultiplier = 2f;

    [SerializeField, Min(1f)]
    private float enemyDamageMultiplier = 2f;

    [SerializeField, Min(1f)]
    private float enemySpeedMultiplier = 1.25f;

    [Header("ENEMY SPAWNING")]
    [SerializeField, Min(1f)]
    private float enemyCountMultiplier = 1.5f;

    [SerializeField, Min(0)]
    private int extraEnemiesPerWave = 2;

    [SerializeField, Min(0.05f)]
    private float spawnIntervalMultiplier = 0.85f;

    [SerializeField, Min(0.1f)]
    private float nextWaveDelayMultiplier = 0.75f;

    [Header("BOSS DIFFICULTY")]
    [SerializeField, Min(1f)]
    private float bossHealthMultiplier = 5f;

    [SerializeField, Min(1f)]
    private float bossDamageMultiplier = 2f;

    [SerializeField, Min(1f)]
    private float bossSpeedMultiplier = 1.35f;

    [SerializeField, Min(1f)]
    private float bossProjectileSpeedMultiplier = 1.15f;

    [SerializeField, Min(0.1f)]
    private float bossFireIntervalMultiplier = 0.75f;

    [Header("HARD CURRENCY")]
    [SerializeField, Min(0)]
    private int completionReward = 10;

    private const string HARD_CURRENCY_KEY =
        "NEON_RAID_HARD_CURRENCY";

    private bool rewardGranted;

    public static float EnemyHealthMultiplier =>
        Instance != null
            ? Instance.enemyHealthMultiplier
            : 1f;

    public static float EnemyDamageMultiplier =>
        Instance != null
            ? Instance.enemyDamageMultiplier
            : 1f;

    public static float EnemySpeedMultiplier =>
        Instance != null
            ? Instance.enemySpeedMultiplier
            : 1f;

    public static float EnemyCountMultiplier =>
        Instance != null
            ? Instance.enemyCountMultiplier
            : 1f;

    public static int ExtraEnemiesPerWave =>
        Instance != null
            ? Instance.extraEnemiesPerWave
            : 0;

    public static float SpawnIntervalMultiplier =>
        Instance != null
            ? Instance.spawnIntervalMultiplier
            : 1f;

    public static float NextWaveDelayMultiplier =>
        Instance != null
            ? Instance.nextWaveDelayMultiplier
            : 1f;

    public static float BossHealthMultiplier =>
        Instance != null
            ? Instance.bossHealthMultiplier
            : 1f;

    public static float BossDamageMultiplier =>
        Instance != null
            ? Instance.bossDamageMultiplier
            : 1f;

    public static float BossSpeedMultiplier =>
        Instance != null
            ? Instance.bossSpeedMultiplier
            : 1f;

    public static float BossProjectileSpeedMultiplier =>
        Instance != null
            ? Instance.bossProjectileSpeedMultiplier
            : 1f;

    public static float BossFireIntervalMultiplier =>
        Instance != null
            ? Instance.bossFireIntervalMultiplier
            : 1f;

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Time.timeScale = 1f;

        Debug.Log(
            "[HardMode] Hard Mode activated."
        );
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;

            Debug.Log(
                "[HardMode] Hard Mode deactivated."
            );
        }
    }

    // =========================================================
    // HARD CURRENCY
    // =========================================================

    public static void AwardCompletionReward()
    {
        if (Instance == null)
            return;

        if (Instance.rewardGranted)
            return;

        Instance.rewardGranted = true;

        int reward =
            Mathf.Max(
                0,
                Instance.completionReward
            );

        int current =
            PlayerPrefs.GetInt(
                HARD_CURRENCY_KEY,
                0
            );

        current += reward;

        PlayerPrefs.SetInt(
            HARD_CURRENCY_KEY,
            current
        );

        PlayerPrefs.Save();

        Debug.Log(
            "[HardMode] Hard Currency +" +
            reward +
            " | Total = " +
            current
        );
    }

    public static int GetHardCurrency()
    {
        return PlayerPrefs.GetInt(
            HARD_CURRENCY_KEY,
            0
        );
    }
}