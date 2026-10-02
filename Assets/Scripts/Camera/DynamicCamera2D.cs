using UnityEngine;

public class DynamicCamera2D : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow")]
    [SerializeField] private float followSmooth = 5f;

    [Header("Look Ahead")]
    [SerializeField] private float lookAheadDistance = 2.5f;
    [SerializeField] private float lookAheadSmooth = 4f;

    [Header("Speed Zoom")]
    [SerializeField] private float normalZoom = 6f;
    [SerializeField] private float maxZoomOut = 7.5f;
    [SerializeField] private float maxSpeedForZoom = 7f;
    [SerializeField] private float zoomSmooth = 3f;

    private Camera cam;
    private PlayerController player;

    private Vector3 currentLookAhead;
    private Vector3 basePosition;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        if (target != null)
        {
            player =
                target.GetComponent<PlayerController>();
        }

        basePosition = transform.position;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        if (player == null)
        {
            player =
                target.GetComponent<PlayerController>();
        }

        UpdateBasePosition();
        UpdateLookAhead();
        UpdateZoom();

        ApplyCameraPosition();
    }

    private void UpdateBasePosition()
    {
        Vector3 targetPosition =
            target.position + currentLookAhead;

        targetPosition.z =
            transform.position.z;

        basePosition =
            Vector3.Lerp(
                basePosition,
                targetPosition,
                followSmooth *
                Time.deltaTime
            );
    }

    private void UpdateLookAhead()
    {
        if (player == null)
            return;

        Vector2 velocity =
            player.Velocity;

        Vector2 desiredLookAhead =
            velocity.normalized *
            Mathf.Clamp(
                velocity.magnitude /
                maxSpeedForZoom,
                0f,
                1f
            ) *
            lookAheadDistance;

        currentLookAhead =
            Vector3.Lerp(
                currentLookAhead,
                desiredLookAhead,
                lookAheadSmooth *
                Time.deltaTime
            );
    }

    private void UpdateZoom()
    {
        if (player == null)
            return;

        float speed =
            player.Velocity.magnitude;

        float speedPercent =
            Mathf.Clamp01(
                speed /
                maxSpeedForZoom
            );

        float targetZoom =
            Mathf.Lerp(
                normalZoom,
                maxZoomOut,
                speedPercent
            );

        cam.orthographicSize =
            Mathf.Lerp(
                cam.orthographicSize,
                targetZoom,
                zoomSmooth *
                Time.deltaTime
            );
    }

    private void ApplyCameraPosition()
    {
        Vector3 shakeOffset =
            Vector3.zero;

        if (CameraShake.Instance != null)
        {
            shakeOffset =
                CameraShake.Instance.ShakeOffset;
        }

        transform.position =
            basePosition +
            shakeOffset;
    }
}