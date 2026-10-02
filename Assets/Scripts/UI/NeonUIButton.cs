using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NeonUIButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private Image targetImage;

    [Header("Scale")]
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float hoverScale = 1.035f;
    [SerializeField] private float pressedScale = 0.96f;
    [SerializeField] private float scaleSpeed = 12f;

    [Header("Color")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor =
        new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color pressedColor =
        new Color(0.75f, 0.95f, 1f, 1f);

    [Header("Animation")]
    [SerializeField] private float colorSpeed = 10f;

    private RectTransform rectTransform;

    private Vector3 targetScale;
    private Color targetColor;

    private bool pointerInside;
    private bool pressed;

    private void Awake()
    {
        rectTransform =
            GetComponent<RectTransform>();

        if (targetImage == null)
        {
            targetImage =
                GetComponent<Image>();
        }

        targetScale =
            Vector3.one * normalScale;

        targetColor =
            normalColor;

        if (targetImage != null)
        {
            targetImage.color =
                normalColor;
        }
    }

    private void Update()
    {
        if (rectTransform != null)
        {
            rectTransform.localScale =
                Vector3.Lerp(
                    rectTransform.localScale,
                    targetScale,
                    scaleSpeed *
                    Time.unscaledDeltaTime
                );
        }

        if (targetImage != null)
        {
            targetImage.color =
                Color.Lerp(
                    targetImage.color,
                    targetColor,
                    colorSpeed *
                    Time.unscaledDeltaTime
                );
        }
    }

    public void OnPointerEnter(
        PointerEventData eventData)
    {
        pointerInside = true;

        if (!pressed)
        {
            targetScale =
                Vector3.one *
                hoverScale;

            targetColor =
                hoverColor;
        }
    }

    public void OnPointerExit(
        PointerEventData eventData)
    {
        if (ProceduralAudioManager.Instance != null)
{
    ProceduralAudioManager.Instance.PlaySfx(
        AudioSfxType.UIHover,
        0.4f
    );
}
        pointerInside = false;

        if (!pressed)
        {
            ResetVisual();
        }
    }

    public void OnPointerDown(
        PointerEventData eventData)
    {
        if (ProceduralAudioManager.Instance != null)
{
    ProceduralAudioManager.Instance.PlaySfx(
        AudioSfxType.UIClick
    );
}
        pressed = true;

        targetScale =
            Vector3.one *
            pressedScale;

        targetColor =
            pressedColor;
    }

    public void OnPointerUp(
        PointerEventData eventData)
    {
        pressed = false;

        if (pointerInside)
        {
            targetScale =
                Vector3.one *
                hoverScale;

            targetColor =
                hoverColor;
        }
        else
        {
            ResetVisual();
        }
    }

    private void ResetVisual()
    {
        targetScale =
            Vector3.one *
            normalScale;

        targetColor =
            normalColor;
    }

    public void ForceNormal()
    {
        pressed = false;
        pointerInside = false;

        targetScale =
            Vector3.one *
            normalScale;

        targetColor =
            normalColor;
    }
}