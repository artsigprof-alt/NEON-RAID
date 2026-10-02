using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    public int Scrap { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        Scrap = PlayerPrefs.GetInt("Scrap", 0);
    }

    public void AddScrap(int amount)
    {
        if (amount <= 0)
            return;

        Scrap += amount;

        Save();
    }

    public bool SpendScrap(int amount)
    {
        if (amount <= 0)
            return true;

        if (Scrap < amount)
            return false;

        Scrap -= amount;

        Save();

        return true;
    }

    private void Save()
    {
        PlayerPrefs.SetInt("Scrap", Scrap);
        PlayerPrefs.Save();
    }
}