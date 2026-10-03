using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    public static SettingsController Instance { get; private set; }

    private const string MUSIC_KEY = "NEON_RAID_MUSIC";
    private const string SFX_KEY = "NEON_RAID_SFX";

    public bool MusicEnabled { get; private set; }
    public bool SfxEnabled { get; private set; }

    [Header("Panel")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Buttons")]
    [SerializeField] private Button musicButton;
    [SerializeField] private Button sfxButton;

    [Header("Texts")]
    [SerializeField] private TMP_Text musicText;
    [SerializeField] private TMP_Text sfxText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        MusicEnabled = PlayerPrefs.GetInt(MUSIC_KEY, 1) == 1;
        SfxEnabled = PlayerPrefs.GetInt(SFX_KEY, 1) == 1;
    }

    private void Start()
    {
        if (musicButton != null)
            musicButton.onClick.AddListener(ToggleMusic);

        if (sfxButton != null)
            sfxButton.onClick.AddListener(ToggleSfx);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        RefreshUI();

        ApplyMusic();
    }

    private void OnDestroy()
    {
        if (musicButton != null)
            musicButton.onClick.RemoveListener(ToggleMusic);

        if (sfxButton != null)
            sfxButton.onClick.RemoveListener(ToggleSfx);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        RefreshUI();
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void ToggleMusic()
    {
        MusicEnabled = !MusicEnabled;

        PlayerPrefs.SetInt(
            MUSIC_KEY,
            MusicEnabled ? 1 : 0
        );

        PlayerPrefs.Save();

        ApplyMusic();
        RefreshUI();
    }

    public void ToggleSfx()
    {
        SfxEnabled = !SfxEnabled;

        PlayerPrefs.SetInt(
            SFX_KEY,
            SfxEnabled ? 1 : 0
        );

        PlayerPrefs.Save();

        RefreshUI();
    }

    private void ApplyMusic()
    {
        if (DynamicMusicController.Instance == null)
            return;

        DynamicMusicController.Instance.SetMusicEnabled(
            MusicEnabled
        );
    }

    private void RefreshUI()
    {
        if (musicText != null)
        {
            musicText.text =
                MusicEnabled
                    ? "MUSIC: ON"
                    : "MUSIC: OFF";
        }

        if (sfxText != null)
        {
            sfxText.text =
                SfxEnabled
                    ? "SFX: ON"
                    : "SFX: OFF";
        }
    }

    public static bool IsSfxEnabled()
    {
        if (Instance == null)
            return true;

        return Instance.SfxEnabled;
    }
}