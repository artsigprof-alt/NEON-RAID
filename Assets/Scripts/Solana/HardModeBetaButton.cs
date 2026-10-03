using UnityEngine;
using UnityEngine.SceneManagement;

public class HardModeBetaButton : MonoBehaviour
{
    private const string HARD_MODE_UNLOCKED =
        "NEON_RAID_HARD_MODE_UNLOCKED";

    private const string HARD_MODE_TX =
        "NEON_RAID_HARD_MODE_TX";

    [Header("Hard Menu")]
    [SerializeField]
    private string hardMenuSceneName = "HardMenu";

    public void UnlockHardModeFree()
    {
        Debug.Log("[HardMode Beta] Free unlock activated.");

        PlayerPrefs.SetInt(
            HARD_MODE_UNLOCKED,
            1
        );

        PlayerPrefs.SetString(
            HARD_MODE_TX,
            "BETA_FREE"
        );

        PlayerPrefs.Save();

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            hardMenuSceneName
        );
    }
}