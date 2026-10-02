using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;

    private Vector2 direction;
    private float speed;
    private int damage;

    public void Initialize(
        Vector2 newDirection,
        float newSpeed,
        int newDamage)
    {
        direction =
            newDirection.normalized;

        speed =
            newSpeed;

        damage =
            newDamage;

        float angle =
            Mathf.Atan2(
                direction.y,
                direction.x
            ) *
            Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(
                0f,
                0f,
                angle
            );
    }

    private void Start()
    {
        Destroy(
            gameObject,
            lifetime
        );
    }

    private void Update()
    {
        transform.position +=
            (Vector3)(
                direction *
                speed *
                Time.deltaTime
            );
    }

    private void OnTriggerEnter2D(
        Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerHealth health =
            other.GetComponent<PlayerHealth>();

        if (health != null)
        {
            health.TakeDamage(
                damage
            );
        }

        Destroy(gameObject);
    }
}