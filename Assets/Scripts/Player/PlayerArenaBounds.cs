using UnityEngine;

public class PlayerArenaBounds : MonoBehaviour
{
    [Header("Arena")]
    [SerializeField] private float minX = -18f;
    [SerializeField] private float maxX = 18f;
    [SerializeField] private float minY = -10f;
    [SerializeField] private float maxY = 10f;

    [Header("Soft Boundary")]
    [SerializeField] private float boundaryForce = 8f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 position = rb.position;
        Vector2 velocity = rb.linearVelocity;

        Vector2 force = Vector2.zero;

        if (position.x < minX)
        {
            force.x += (minX - position.x) * boundaryForce;
        }
        else if (position.x > maxX)
        {
            force.x -= (position.x - maxX) * boundaryForce;
        }

        if (position.y < minY)
        {
            force.y += (minY - position.y) * boundaryForce;
        }
        else if (position.y > maxY)
        {
            force.y -= (position.y - maxY) * boundaryForce;
        }

        if (force.sqrMagnitude > 0.01f)
        {
            rb.AddForce(force);
        }

        // Экстренно не даём игроку пересечь границу.
        if (position.x < minX - 2f)
        {
            position.x = minX;
            velocity.x = Mathf.Max(velocity.x, 0f);
        }

        if (position.x > maxX + 2f)
        {
            position.x = maxX;
            velocity.x = Mathf.Min(velocity.x, 0f);
        }

        if (position.y < minY - 2f)
        {
            position.y = minY;
            velocity.y = Mathf.Max(velocity.y, 0f);
        }

        if (position.y > maxY + 2f)
        {
            position.y = maxY;
            velocity.y = Mathf.Min(velocity.y, 0f);
        }

        rb.position = position;
        rb.linearVelocity = velocity;
    }
}