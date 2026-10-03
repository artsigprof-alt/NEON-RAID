using UnityEngine;
using UnityEngine.EventSystems;

public class BoostButton : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler
{
    [SerializeField] private PlayerController player;

    public void OnPointerDown(
        PointerEventData eventData)
    {
        if (player != null)
            player.SetBoost(true);
    }

    public void OnPointerUp(
        PointerEventData eventData)
    {
        if (player != null)
            player.SetBoost(false);
    }

    public void OnPointerExit(
        PointerEventData eventData)
    {
        if (player != null)
            player.SetBoost(false);
    }
}