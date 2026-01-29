using UnityEngine;

public class ProjectileAbility : BaseAbility
{
    [SerializeField] private ObjectPool projectilePool;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float spreadAngle = 25f; // total cone angle in degrees

    protected override void Start()
    {
        // Configuration will be provided by AbilityManager; keep disabled until then.
        if (projectilePool == null && projectilePrefab != null)
        {
            var poolObj = new GameObject("ProjectilePool");
            poolObj.transform.SetParent(transform);
            projectilePool = poolObj.AddComponent<ObjectPool>();
            projectilePool.Configure(projectilePrefab, 10);
        }
        // Do not auto-disable here; AbilityManager controls enable state after Configure.
    }

    protected override bool Activate()
    {
        if (stats == null)
        {
            return false;
        }

        Transform target = FindNearestEnemy();
        if (target == null)
        {
            return false;
        }

        Vector3 baseDir = (target.position - transform.position).normalized;
        if (baseDir.sqrMagnitude < 1e-4f)
        {
            baseDir = Vector3.right;
        }

        int count = Mathf.Max(1, GetProjectileCount());
        float totalSpread = Mathf.Max(0f, spreadAngle);

        for (int i = 0; i < count; i++)
        {
            float angle = 0f;
            if (count == 1)
            {
                angle = 0f; // single shot straight to target
            }
            else if (count % 2 == 1)
            {
                // odd: center shot plus symmetric sides
                float step = totalSpread / (count - 1);
                float centerIndex = (count - 1) * 0.5f;
                angle = (i - centerIndex) * step;
            }
            else
            {
                // even: no center shot, symmetric around target direction
                float step = totalSpread / (count - 1);
                float centerShift = (count * 0.5f) - 0.5f;
                angle = (i - centerShift) * step;
            }

            Vector3 dir = Quaternion.Euler(0f, 0f, angle) * baseDir;
            SpawnProjectile(dir);
        }

        return true;
    }

    private Transform FindNearestEnemy()
    {
        int mask = enemyLayer.value == 0 ? ~0 : enemyLayer.value;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, mask);
        float bestDist = float.MaxValue;
        Transform best = null;
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyAI>();
            if (enemy == null)
            {
                continue;
            }

            float dist = Vector2.SqrMagnitude(hit.transform.position - transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = hit.transform;
            }
        }

        return best;
    }

    private void SpawnProjectile(Vector3 direction)
    {
        GameObject projObj = projectilePool != null ? projectilePool.Get() : null;
        if (projObj == null)
        {
            return;
        }

        projObj.transform.position = transform.position;
        var proj = projObj.GetComponent<Projectile>();
        proj?.Init(direction, GetSpeed(), GetDamage(), projectilePool, transform);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
