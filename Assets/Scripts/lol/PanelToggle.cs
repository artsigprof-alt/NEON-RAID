
using UnityEngine;

public class PanelToggle : MonoBehaviour
{
    [SerializeField] private GameObject panel;
[SerializeField] private ProceduralSolanaPanel solanaPanel;
    public void Open()
    {
        panel.SetActive(true);
          solanaPanel?.Generate();
        DynamicMusicController.Instance?.StopMusic();
    }

    public void Close()
    {
        panel.SetActive(false);
        DynamicMusicController.Instance?.SetMenu();
    }

    public void Toggle()
    {
        if (panel.activeSelf) Close();
        else Open();
    }
}

