
using System;
using System.Globalization;
using UnityEngine;

public class SkrReactorManager : MonoBehaviour
{
    public static SkrReactorManager Instance { get; private set; }

    [Header("SKR Requirements")]
    [SerializeField] private float tier1Required = 1f;
    [SerializeField] private float tier2Required = 100f;
    [SerializeField] private float tier3Required = 1000f;

    [Header("Tier 1 Effects")]
    [SerializeField] private float tier1FireRateMultiplier = 0.95f;
    [SerializeField] private float tier1SpeedMultiplier = 1.05f;
    [SerializeField] private float tier1ScrapMultiplier = 1.05f;
    [SerializeField] private float tier1EnemyHealthMultiplier = 1.03f;

    [Header("Tier 2 Effects")]
    [SerializeField] private float tier2FireRateMultiplier = 0.92f;
    [SerializeField] private float tier2SpeedMultiplier = 1.08f;
    [SerializeField] private float tier2ScrapMultiplier = 1.10f;
    [SerializeField] private float tier2EnemyHealthMultiplier = 1.06f;

    [Header("Tier 3 Effects")]
    [SerializeField] private float tier3FireRateMultiplier = 0.88f;
    [SerializeField] private float tier3SpeedMultiplier = 1.10f;
    [SerializeField] private float tier3ScrapMultiplier = 1.15f;
    [SerializeField] private float tier3EnemyHealthMultiplier = 1.10f;

    public int CurrentTier { get; private set; }

    public bool IsActive { get; private set; }

    public float CurrentSkrBalance { get; private set; }

    public float FireRateMultiplier
    {
        get
        {
            if (!IsActive)
                return 1f;

            switch (CurrentTier)
            {
                case 1:
                    return tier1FireRateMultiplier;

                case 2:
                    return tier2FireRateMultiplier;

                case 3:
                    return tier3FireRateMultiplier;

                default:
                    return 1f;
            }
        }
    }

    public float SpeedMultiplier
    {
        get
        {
            if (!IsActive)
                return 1f;

            switch (CurrentTier)
            {
                case 1:
                    return tier1SpeedMultiplier;

                case 2:
                    return tier2SpeedMultiplier;

                case 3:
                    return tier3SpeedMultiplier;

                default:
                    return 1f;
            }
        }
    }

    public float ScrapMultiplier
    {
        get
        {
            if (!IsActive)
                return 1f;

            switch (CurrentTier)
            {
                case 1:
                    return tier1ScrapMultiplier;

                case 2:
                    return tier2ScrapMultiplier;

                case 3:
                    return tier3ScrapMultiplier;

                default:
                    return 1f;
            }
        }
    }

    public float EnemyHealthMultiplier
    {
        get
        {
            if (!IsActive)
                return 1f;

            switch (CurrentTier)
            {
                case 1:
                    return tier1EnemyHealthMultiplier;

                case 2:
                    return tier2EnemyHealthMultiplier;

                case 3:
                    return tier3EnemyHealthMultiplier;

                default:
                    return 1f;
            }
        }
    }

    public int GetScrapReward(int baseReward)
{
    return Mathf.RoundToInt(
        baseReward *
        ScrapMultiplier
    );
}

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (SolanaWalletService.Instance == null)
        {
            CurrentSkrBalance = 0f;
            CurrentTier = 0;
            IsActive = false;
            return;
        }

        if (!SolanaWalletService.Instance.IsConnected)
        {
            CurrentSkrBalance = 0f;
            CurrentTier = 0;
            IsActive = false;
            return;
        }

        string balanceString =
            SolanaWalletService.Instance.SkrBalance;

        if (string.IsNullOrEmpty(balanceString))
        {
            CurrentSkrBalance = 0f;
            CurrentTier = 0;
            IsActive = false;
            return;
        }

        if (!float.TryParse(
            balanceString,
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out float balance))
        {
            CurrentSkrBalance = 0f;
            CurrentTier = 0;
            IsActive = false;
            return;
        }

        CurrentSkrBalance =
            Mathf.Max(0f, balance);

        int newTier =
            CalculateTier(
                CurrentSkrBalance
            );

        // Если уровень уменьшился или SKR пропал,
        // Reactor автоматически отключается.
        if (newTier <= 0)
        {
            IsActive = false;
        }
        else if (newTier != CurrentTier)
        {
            IsActive = false;
        }

        CurrentTier = newTier;
    }

    private int CalculateTier(float balance)
    {
        if (balance >= tier3Required)
            return 3;

        if (balance >= tier2Required)
            return 2;

        if (balance >= tier1Required)
            return 1;

        return 0;
    }

    public bool CanActivate()
    {
        return CurrentTier > 0;
    }

    public bool Activate()
    {
        Refresh();

        if (!CanActivate())
        {
            IsActive = false;

            return false;
        }

        IsActive = true;

        Debug.Log(
            "[SKR] Reactor activated. Tier: " +
            CurrentTier +
            ", Balance: " +
            CurrentSkrBalance
        );

        return true;
    }

    public void Deactivate()
    {
        IsActive = false;

        Debug.Log(
            "[SKR] Reactor deactivated."
        );
    }

    public string GetTierName()
    {
        switch (CurrentTier)
        {
            case 1:
                return "REACTOR I";

            case 2:
                return "REACTOR II";

            case 3:
                return "REACTOR III";

            default:
                return "LOCKED";
        }
    }

    public string GetEffectDescription()
    {
        switch (CurrentTier)
        {
            case 1:
                return
                    "+5% SCRAP\n" +
                    "+5% SPEED\n" +
                    "+5% FIRE RATE\n" +
                    "ENEMIES +3% HP";

            case 2:
                return
                    "+10% SCRAP\n" +
                    "+8% SPEED\n" +
                    "+8% FIRE RATE\n" +
                    "ENEMIES +6% HP";

            case 3:
                return
                    "+15% SCRAP\n" +
                    "+10% SPEED\n" +
                    "+12% FIRE RATE\n" +
                    "ENEMIES +10% HP";

            default:
                return
                    "CONNECT WALLET\n" +
                    "AND HOLD SKR";
        }
    }
}

