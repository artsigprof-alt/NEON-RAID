using UnityEngine;

public class BoostVisual : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Transform enginePoint;

    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float boostScale = 2.3f;

    private Vector3 originalScale;

    private void Start()
    {
        originalScale =
            transform.localScale;
    }

    private void Update()
    {
        if (player == null)
            return;

        float target =
            player.IsBoosting
                ? boostScale
                : normalScale;

        transform.localScale =
            Vector3.Lerp(
                transform.localScale,
                originalScale * target,
                12f * Time.deltaTime
            );
    }
}