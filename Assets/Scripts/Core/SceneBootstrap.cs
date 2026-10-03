using UnityEngine;

public class SceneBootstrap : MonoBehaviour
{
    private void Awake()
    {
        if (SceneLoader.Instance == null)
        {
            GameObject loader =
                new GameObject("SceneLoader");

            loader.AddComponent<SceneLoader>();
        }
    }
}