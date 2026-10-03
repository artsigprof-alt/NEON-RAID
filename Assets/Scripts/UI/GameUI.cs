using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;

    [Header("Game Over")]
    [SerializeField] private TMP_Text gameOverKillsText;

    [Header("Victory")]
    [SerializeField] private TMP_Text victoryKillsText;
    [SerializeField] private TMP_Text victoryScrapText;

    public void HardMenu()
{
    Time.timeScale = 1f;

    SceneManager.LoadScene("HardMenu");
}
public void MainMenu()
{
    Time.timeScale = 1f;

    SceneManager.LoadScene("MainMenu");
}
    private void Awake()
    {
        Instance = this;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (victoryPanel != null)
            victoryPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (gameOverPanel == null)
            return;

        gameOverPanel.SetActive(true);

        if (gameOverKillsText != null)
        {
            gameOverKillsText.text =
                $"ENEMIES DESTROYED\n" +
                $"{GameManager.Instance.EnemiesKilled}";
        }
    }

    public void ShowVictory()
    {
        if (victoryPanel == null)
            return;

        victoryPanel.SetActive(true);

        if (victoryKillsText != null)
        {
            victoryKillsText.text =
                $"ENEMIES DESTROYED\n" +
                $"{GameManager.Instance.EnemiesKilled}";
        }

        if (victoryScrapText != null &&
            CurrencyManager.Instance != null)
        {
            victoryScrapText.text =
                $"SCRAP\n" +
                $"{CurrencyManager.Instance.Scrap}";
        }
    }

 public void Restart()
{
    Time.timeScale = 1f;

    SceneManager.LoadScene(
        SceneManager.GetActiveScene().buildIndex
    );
}
}