using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int CurrentWave { get; private set; }
    public int EnemiesKilled { get; private set; }

    public bool GameRunning { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;

        StartGame();
    }

    public void StartGame()
    {
        CurrentWave = 1;
        EnemiesKilled = 0;

        GameRunning = true;

        Debug.Log("[Game] Started");
    }

    public void EnemyKilled()
    {
        EnemiesKilled++;
    }

    public void SetWave(int wave)
    {
        CurrentWave = wave;

        Debug.Log(
            $"[Game] Wave {CurrentWave}"
        );
    }

  public void PlayerDied()
{
    if (!GameRunning)
        return;

    GameRunning = false;

    Time.timeScale = 0f;

    DynamicMusicController.Instance?.PlayDefeat();

    GameUI.Instance?.ShowGameOver();
}

    // Вызывается BossController
    public void BossDefeated()
{
    if (!GameRunning)
        return;

    GameRunning = false;

    Time.timeScale = 0f;

    DynamicMusicController.Instance?.PlayVictory();

    GameUI.Instance?.ShowVictory();
}

    // Вызывается EnemySpawner
    public void GameCompleted()
    {
        if (!GameRunning)
            return;

        GameRunning = false;

        Debug.Log(
            $"[Game] GAME COMPLETE | Kills: {EnemiesKilled}"
        );

        Time.timeScale = 0f;

        DynamicMusicController.Instance?.PlayVictory();

        GameUI.Instance?.ShowVictory();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}