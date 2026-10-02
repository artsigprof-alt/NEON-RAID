using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int fallbackMaxHealth = 100;

    [Header("Damage Protection")]
    [SerializeField] private float invulnerabilityTime = 0.35f;

    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }

    public event Action<int, int> OnHealthChanged;

    private float nextDamageTime;
    private bool isDead;

    private void Awake()
    {
        RefreshMaxHealth();

        CurrentHealth = MaxHealth;
    }

    private void Start()
{
    RefreshMaxHealth();

    CurrentHealth = MaxHealth;

    OnHealthChanged?.Invoke(
        CurrentHealth,
        MaxHealth
    );
}

    public void RefreshMaxHealth()
    {
        MaxHealth =
            PlayerStats.Instance != null
                ? PlayerStats.Instance.MaxHealth
                : fallbackMaxHealth;

        if (CurrentHealth > MaxHealth)
            CurrentHealth = MaxHealth;
    }

    public void RestoreFullHealth()
    {
        RefreshMaxHealth();

        CurrentHealth = MaxHealth;
        isDead = false;

        OnHealthChanged?.Invoke(
            CurrentHealth,
            MaxHealth
        );
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
            return;

        if (Time.time < nextDamageTime)
            return;

        amount = Mathf.Max(0, amount);

        CurrentHealth -= amount;
        CurrentHealth =
            Mathf.Max(
                CurrentHealth,
                0
            );

        nextDamageTime =
            Time.time +
            invulnerabilityTime;

        OnHealthChanged?.Invoke(
            CurrentHealth,
            MaxHealth
        );

        CombatVFX.SpawnDamageFlash(
            transform.position
        );
        if (ProceduralAudioManager.Instance != null)
{
    ProceduralAudioManager.Instance.PlaySfx(
        AudioSfxType.PlayerDamage
    );
}

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        CombatVFX.SpawnExplosion(
            transform.position,
            1.3f
        );

        GameManager.Instance?.PlayerDied();
    }
}