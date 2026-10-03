using UnityEngine;

public class EngineTrail : MonoBehaviour
{
    [SerializeField] private Transform enginePoint;
    [SerializeField] private PlayerController player;

    [SerializeField] private float minLength = 0.15f;
    [SerializeField] private float maxLength = 0.8f;

    private LineRenderer line;

    private void Awake()
    {
        line =
            GetComponent<LineRenderer>();

        line.positionCount = 2;

        line.startWidth = 0.2f;
        line.endWidth = 0f;
    }

    private void Update()
    {
        if (enginePoint == null ||
            player == null)
            return;

        Vector2 velocity =
            player.Velocity;

        float speed =
            velocity.magnitude;

        float normalizedSpeed =
            Mathf.Clamp01(
                speed / 7f
            );

        float length =
            Mathf.Lerp(
                minLength,
                maxLength,
                normalizedSpeed
            );

        if (player.IsBoosting)
        {
            length *= 1.8f;
        }

        line.SetPosition(
            0,
            enginePoint.position
        );

        line.SetPosition(
            1,
            enginePoint.position -
            (Vector3)(
                player.Forward *
                length
            )
        );
    }
}