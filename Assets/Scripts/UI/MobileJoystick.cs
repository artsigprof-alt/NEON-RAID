using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJoystick : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IDragHandler
{
    [Header("References")]
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;

    [Header("Settings")]
    [SerializeField] private float handleRange = 60f;

    public Vector2 Input { get; private set; }

    private Vector2 startPosition;

    private void Start()
    {
        if (background == null)
            background = GetComponent<RectTransform>();

        startPosition = background.position;

        ResetJoystick();
    }

    public void OnPointerDown(
        PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(
        PointerEventData eventData)
    {
        if (background == null ||
            handle == null)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        Vector2 radius =
            background.rect.size * 0.5f;

        if (radius.x <= 0f ||
            radius.y <= 0f)
            return;

        Vector2 normalized = new Vector2(
            localPoint.x / radius.x,
            localPoint.y / radius.y
        );

        Input =
            Vector2.ClampMagnitude(
                normalized,
                1f
            );

        handle.anchoredPosition =
            Input *
            handleRange;
    }

    public void OnPointerUp(
        PointerEventData eventData)
    {
        ResetJoystick();
    }

    private void ResetJoystick()
    {
        Input = Vector2.zero;

        if (handle != null)
            handle.anchoredPosition =
                Vector2.zero;
    }
}