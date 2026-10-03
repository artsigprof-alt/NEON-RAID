using TMPro;
using UnityEngine;

public class ArmoryUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Damage")]
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text damageCostText;

    [Header("Fire Rate")]
    [SerializeField] private TMP_Text fireRateText;
    [SerializeField] private TMP_Text fireRateCostText;

    [Header("Health")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text healthCostText;

    [Header("Speed")]
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text speedCostText;

    private void Start()
    {
        if (panel != null)
            panel.SetActive(false);

        Refresh();
    }

    public void Open()
    {
        if (panel != null)
            panel.SetActive(true);

        Refresh();
    }

    public void Close()
    {
        if (panel != null)
            panel.SetActive(false);
    }

   public void UpgradeDamage()
{
    if (PlayerStats.Instance == null)
        return;

    bool success =
        PlayerStats.Instance.UpgradeDamage();

    if (success)
    {
        ProceduralAudioManager.Instance?.PlaySfx(
            AudioSfxType.Upgrade
        );
    }

    Refresh();
}

    public void UpgradeFireRate()
    {
        if (PlayerStats.Instance == null)
        return;

    bool success =
        PlayerStats.Instance.UpgradeFireRate();

    if (success)
    {
        ProceduralAudioManager.Instance?.PlaySfx(
            AudioSfxType.Upgrade
        );
    }

    Refresh();
    }

    public void UpgradeHealth()
    {
        if (PlayerStats.Instance == null)
        return;

    bool success =
        PlayerStats.Instance.UpgradeHealth();

    if (success)
    {
        ProceduralAudioManager.Instance?.PlaySfx(
            AudioSfxType.Upgrade
        );
    }

    Refresh();
    }

    public void UpgradeSpeed()
    {
       if (PlayerStats.Instance == null)
        return;

    bool success =
        PlayerStats.Instance.UpgradeSpeed();

    if (success)
    {
        ProceduralAudioManager.Instance?.PlaySfx(
            AudioSfxType.Upgrade
        );
    }

    Refresh();
    }

    public void Refresh()
    {
        if (PlayerStats.Instance == null)
            return;

        PlayerStats stats =
            PlayerStats.Instance;

        if (damageText != null)
        {
            damageText.text =
                $"DAMAGE\n" +
                $"LVL {stats.DamageLevel}\n" +
                $"{stats.Damage}";
        }

        if (damageCostText != null)
        {
            damageCostText.text =
                $"UPGRADE {stats.GetDamageCost()}";
        }

        if (fireRateText != null)
        {
            fireRateText.text =
                $"FIRE RATE\n" +
                $"LVL {stats.FireRateLevel}\n" +
                $"{stats.FireRate:0.00}s";
        }

        if (fireRateCostText != null)
        {
            fireRateCostText.text =
                $"UPGRADE {stats.GetFireRateCost()}";
        }

        if (healthText != null)
        {
            healthText.text =
                $"HP\n" +
                $"LVL {stats.HealthLevel}\n" +
                $"{stats.MaxHealth}";
        }

        if (healthCostText != null)
        {
            healthCostText.text =
                $"UPGRADE {stats.GetHealthCost()}";
        }

        if (speedText != null)
        {
            speedText.text =
                $"SPEED\n" +
                $"LVL {stats.SpeedLevel}\n" +
                $"{stats.MoveSpeed:0.0}";
        }

        if (speedCostText != null)
        {
            speedCostText.text =
                $"UPGRADE {stats.GetSpeedCost()}";
        }
    }
}