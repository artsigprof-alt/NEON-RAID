using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private BossWarningUI bossWarning;

    [Header("Enemy")]
    [SerializeField]
    private GameObject enemyPrefab;

    [Header("Boss")]
    [SerializeField]
    private GameObject bossPrefab;

    [Header("Spawn")]
    [SerializeField]
    private float spawnDistance = 8f;

    [Header("Waves")]
    [SerializeField]
    private int firstWaveEnemies = 5;

    [SerializeField]
    private int enemiesIncreasePerWave = 3;

    [SerializeField]
    private float spawnInterval = 0.8f;

    [SerializeField]
    private float nextWaveDelay = 3f;

    [Header("Boss Wave")]
    [SerializeField]
    private int bossWave = 5;

    private int currentWave;
    private int enemiesToSpawn;
    private int enemiesSpawned;

    private float spawnTimer;
    private float nextWaveTimer;

    private bool spawning;
    private bool waitingForNextWave;
    private bool bossWaveActive;
    private bool gameCompleted;

    private BossController currentBoss;

    private void Start()
    {
        // =====================================================
        // HARD MODE SPAWN SETTINGS
        // =====================================================

        if (HardModeController.IsActive)
        {
            spawnInterval =
                Mathf.Max(
                    0.05f,
                    spawnInterval *
                    HardModeController.SpawnIntervalMultiplier
                );

            nextWaveDelay =
                Mathf.Max(
                    0.1f,
                    nextWaveDelay *
                    HardModeController.NextWaveDelayMultiplier
                );
        }

        StartWave(1);
    }

    private void Update()
    {
        if (gameCompleted)
            return;

        if (GameManager.Instance == null)
            return;

        if (!GameManager.Instance.GameRunning)
            return;

        // =====================================================
        // BOSS
        // =====================================================

        if (bossWaveActive)
        {
            if (currentBoss != null)
                return;

            BossDefeated();

            return;
        }

        // =====================================================
        // WAITING FOR NEXT WAVE
        // =====================================================

        if (waitingForNextWave)
        {
            nextWaveTimer -=
                Time.deltaTime;

            if (nextWaveTimer <= 0f)
            {
                StartWave(
                    currentWave + 1
                );
            }

            return;
        }

        // =====================================================
        // SPAWNING
        // =====================================================

        if (spawning)
        {
            spawnTimer -=
                Time.deltaTime;

            if (spawnTimer <= 0f)
            {
                SpawnEnemy();

                spawnTimer =
                    spawnInterval;

                if (enemiesSpawned >=
                    enemiesToSpawn)
                {
                    spawning =
                        false;
                }
            }

            return;
        }

        // =====================================================
        // WAVE COMPLETE
        // =====================================================

        if (CurrentEnemyCount() == 0)
        {
            WaveCompleted();
        }
    }

    private void StartWave(int wave)
    {
        if (gameCompleted)
            return;

        currentWave =
            wave;

        DynamicMusicController.Instance?.SetWave(
            currentWave
        );

        GameManager.Instance.SetWave(
            currentWave
        );

        enemiesSpawned =
            0;

        waitingForNextWave =
            false;

        // =====================================================
        // BOSS WAVE
        // =====================================================

        if (currentWave == bossWave)
        {
            if (bossWarning != null)
            {
                bossWarning.ShowWarning();
            }

            SpawnBoss();

            bossWaveActive =
                true;

            spawning =
                false;

            Debug.Log(
                "[Spawner] BOSS WAVE"
            );

            return;
        }

        // =====================================================
        // NORMAL WAVE
        // =====================================================

        bossWaveActive =
            false;

        int normalEnemyCount =
            firstWaveEnemies +
            (currentWave - 1) *
            enemiesIncreasePerWave;

        enemiesToSpawn =
            normalEnemyCount;

        // =====================================================
        // HARD MODE EXTRA ENEMIES
        // =====================================================

        if (HardModeController.IsActive)
        {
            enemiesToSpawn =
                Mathf.CeilToInt(
                    normalEnemyCount *
                    HardModeController.EnemyCountMultiplier
                ) +
                HardModeController.ExtraEnemiesPerWave;
        }

        spawning =
            true;

        spawnTimer =
            0f;

        Debug.Log(
            "[Spawner] Wave " +
            currentWave +
            " | Enemies = " +
            enemiesToSpawn +
            (
                HardModeController.IsActive
                    ? " | HARD MODE"
                    : ""
            )
        );
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError(
                "[Spawner] Enemy Prefab missing!"
            );

            return;
        }

        Instantiate(
            enemyPrefab,
            GetSpawnPosition(),
            Quaternion.identity
        );

        enemiesSpawned++;
    }

    private void SpawnBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogError(
                "[Spawner] Boss Prefab missing!"
            );

            return;
        }

        GameObject boss =
            Instantiate(
                bossPrefab,
                GetSpawnPosition(),
                Quaternion.identity
            );

        currentBoss =
            boss.GetComponent<BossController>();

        if (currentBoss == null)
        {
            Debug.LogError(
                "[Spawner] Boss prefab does not contain BossController!"
            );
        }
    }

    private void WaveCompleted()
    {
        if (waitingForNextWave)
            return;

        waitingForNextWave =
            true;

        nextWaveTimer =
            nextWaveDelay;
    }

    private void BossDefeated()
    {
        bossWaveActive =
            false;

        gameCompleted =
            true;

        // =====================================================
        // HARD MODE REWARD
        // =====================================================

        if (HardModeController.IsActive)
        {
            HardModeController.AwardCompletionReward();
        }

        GameManager.Instance
            ?.GameCompleted();
    }

    private int CurrentEnemyCount()
    {
        return FindObjectsOfType<Enemy>()
            .Length;
    }

    private Vector2 GetSpawnPosition()
    {
        Vector2 direction =
            Random.insideUnitCircle.normalized;

        if (direction == Vector2.zero)
        {
            direction =
                Vector2.right;
        }

        return
            (Vector2)transform.position +
            direction *
            spawnDistance;
    }
}