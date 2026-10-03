using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;

    [Header("Resources")]
    [SerializeField] private TMP_Text scrapText;
    [SerializeField] private TMP_Text waveText;

    [Header("Boss")]
    [SerializeField] private GameObject bossBarRoot;
    [SerializeField] private Slider bossHealthSlider;
    [SerializeField] private TMP_Text bossText;

    private PlayerHealth playerHealth;
    private BossController boss;

    private void Start()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag(
                "Player"
            );

        if (player != null)
        {
            playerHealth =
                player.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged +=
                    OnPlayerHealthChanged;

                OnPlayerHealthChanged(
                    playerHealth.CurrentHealth,
                    playerHealth.MaxHealth
                );
            }
        }

        if (bossBarRoot != null)
            bossBarRoot.SetActive(false);
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -=
                OnPlayerHealthChanged;
        }
    }

    private void Update()
    {
        if (CurrencyManager.Instance != null)
        {
            scrapText.text =
                $"SCRAP {CurrencyManager.Instance.Scrap}";
        }

        if (GameManager.Instance != null)
        {
            waveText.text =
                $"WAVE {GameManager.Instance.CurrentWave}";
        }

        UpdateBoss();
    }

    private void OnPlayerHealthChanged(
        int current,
        int max)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue =
                max;

            healthSlider.value =
                current;
        }

        if (healthText != null)
        {
            healthText.text =
                $"{current}/{max}";
        }
    }

    private void UpdateBoss()
    {
        if (boss == null)
        {
            BossController found =
                FindObjectOfType<BossController>();

            if (found != null)
            {
                boss = found;

                if (bossBarRoot != null)
                    bossBarRoot.SetActive(true);
            }
        }

        if (boss == null)
            return;

        if (bossHealthSlider != null)
        {
            bossHealthSlider.maxValue =
                boss.MaxHealth;

            bossHealthSlider.value =
                boss.CurrentHealth;
        }

        if (bossText != null)
        {
            bossText.text =
                $"BOSS   {boss.CurrentHealth}/{boss.MaxHealth}";
        }
    }
}