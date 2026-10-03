using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HardMenuController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField]
    private string hardGameSceneName = "HardMode";

    [SerializeField]
    private string mainMenuSceneName = "MainMenu";

    [Header("UI")]
    [SerializeField]
    private TMP_Text statusText;

    private void Start()
    {
        Time.timeScale = 1f;

        if (statusText != null)
        {
            statusText.text =
                "HARD MODE\n" +
                "EXTREME DIFFICULTY";
        }
    }

    public void StartHardMode()
    {
        Debug.Log("[HardMenu] Starting Hard Mode.");

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            hardGameSceneName
        );
    }

    public void BackToMainMenu()
    {
        Debug.Log("[HardMenu] Returning to Main Menu.");

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            mainMenuSceneName
        );
    }
}