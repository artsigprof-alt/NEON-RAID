using UnityEngine;

public class SafeArea : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect lastSafeArea;

    private void Awake()
    {
        rectTransform =
            GetComponent<RectTransform>();

        ApplySafeArea();
    }

    private void Update()
    {
        ApplySafeArea();
    }

    private void ApplySafeArea()
    {
        Rect safeArea =
            Screen.safeArea;

        if (safeArea == lastSafeArea)
            return;

        lastSafeArea = safeArea;

        Vector2 anchorMin =
            safeArea.position;

        Vector2 anchorMax =
            safeArea.position +
            safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;

        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin =
            anchorMin;

        rectTransform.anchorMax =
            anchorMax;
    }
}