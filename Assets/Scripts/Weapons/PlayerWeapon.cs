using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private float targetRange = 12f;

    [Header("Weapon")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Base Weapon")]
    [SerializeField] private float baseFireRate = 0.25f;
    [SerializeField] private float bulletSpeed = 14f;
    [SerializeField] private int baseDamage = 10;

    [Header("Recoil")]
    [SerializeField] private float recoilForce = 1.2f;

    private Rigidbody2D playerRb;
    private float nextFireTime;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Transform target = FindNearestTarget();

        if (target == null)
            return;

        Vector2 direction =
            ((Vector2)target.position -
             (Vector2)firePoint.position).normalized;

        AimAt(direction);

        float fireRate =
            PlayerStats.Instance != null
                ? PlayerStats.Instance.FireRate
                : baseFireRate;

        if (Time.time >= nextFireTime)
        {
            Shoot(direction);
            nextFireTime = Time.time + fireRate;
        }
    }

    private void AimAt(Vector2 direction)
    {
        float angle =
            Mathf.Atan2(direction.y, direction.x) *
            Mathf.Rad2Deg;

        Quaternion targetRotation =
            Quaternion.Euler(0f, 0f, angle);

        firePoint.rotation =
            Quaternion.RotateTowards(
                firePoint.rotation,
                targetRotation,
                720f * Time.deltaTime
            );
    }

    private void Shoot(Vector2 direction)
    {
        if (bulletPrefab == null || firePoint == null)
            return;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        int damage =
            PlayerStats.Instance != null
                ? PlayerStats.Instance.Damage
                : baseDamage;

        Bullet bulletComponent =
            bullet.GetComponent<Bullet>();

        if (bulletComponent != null)
        {
            bulletComponent.Initialize(
                damage,
                direction,
                bulletSpeed
            );
        }
        else
        {
            Rigidbody2D bulletRb =
                bullet.GetComponent<Rigidbody2D>();

            if (bulletRb != null)
                bulletRb.linearVelocity =
                    direction * bulletSpeed;
        }

        ApplyRecoil(direction);

        CombatVFX.SpawnMuzzle(
            firePoint.position,
            direction
        );
        if (ProceduralAudioManager.Instance != null)
{
    ProceduralAudioManager.Instance.PlaySfx(
        AudioSfxType.Shot
    );
}
if (CameraShake.Instance != null)
{
    CameraShake.Instance.PlayerShot();
}
    }

    private void ApplyRecoil(Vector2 direction)
    {
        if (playerRb == null)
            return;

        playerRb.AddForce(
            -direction * recoilForce,
            ForceMode2D.Impulse
        );
    }

    private Transform FindNearestTarget()
    {
        Transform nearestTarget = null;
        float nearestDistance = targetRange;

        // Обычные враги
        Enemy[] enemies =
            FindObjectsOfType<Enemy>();

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null)
                continue;

            float distance =
                Vector2.Distance(
                    transform.position,
                    enemy.transform.position
                );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = enemy.transform;
            }
        }

        // Boss
        BossController[] bosses =
            FindObjectsOfType<BossController>();

        foreach (BossController boss in bosses)
        {
            if (boss == null)
                continue;

            float distance =
                Vector2.Distance(
                    transform.position,
                    boss.transform.position
                );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = boss.transform;
            }
        }

        return nearestTarget;
    }
}