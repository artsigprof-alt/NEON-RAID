using UnityEngine;

public class ApplicationSettings : MonoBehaviour
{
    [SerializeField] private int targetFPS = 60;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFPS;
    }
}