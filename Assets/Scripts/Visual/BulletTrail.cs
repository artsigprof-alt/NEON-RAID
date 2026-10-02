using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    [SerializeField] private float trailLength = 0.35f;

    private LineRenderer line;
    private Rigidbody2D rb;

    private void Awake()
    {
        line =
            GetComponent<LineRenderer>();

        rb =
            GetComponent<Rigidbody2D>();

        line.positionCount = 2;

        line.startWidth = 0.05f;
        line.endWidth = 0f;
    }

    private void Update()
    {
        if (rb == null)
            return;

        Vector2 direction =
            rb.linearVelocity.normalized;

        line.SetPosition(
            0,
            transform.position
        );

        line.SetPosition(
            1,
            transform.position -
            (Vector3)(
                direction *
                trailLength
            )
        );
    }
}