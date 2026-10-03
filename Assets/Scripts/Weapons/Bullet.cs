using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;

    private int damage;
    private Vector2 direction;

    private void Start()
    {
        Destroy(
            gameObject,
            lifetime
        );
    }

    public void Initialize(
        int newDamage,
        Vector2 newDirection,
        float speed)
    {
        damage =
            newDamage;

        direction =
            newDirection.normalized;

        Rigidbody2D rb =
            GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity =
                direction * speed;
        }

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

    private void OnTriggerEnter2D(
    Collider2D other)
{
    BossController boss =
        other.GetComponent<BossController>();

    if (boss != null)
    {
        CombatVFX.SpawnHit(
            transform.position
        );

        boss.TakeDamage(
            damage
        );

        Destroy(gameObject);
        return;
    }

    Enemy enemy =
        other.GetComponent<Enemy>();

    if (enemy == null)
        return;

    CombatVFX.SpawnHit(
        transform.position
    );

    enemy.TakeDamage(
        damage
    );

    Destroy(gameObject);
}
}