using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    private float speed;
    private float damage;
    private ObjectPool originPool;

    [SerializeField] private float maxLifetime = 5f;
    private float lifeTimer;
    private Vector3 direction;
    private Transform owner;

    public void Init(Vector3 directionNormalized, float projectileSpeed, float damageAmount, ObjectPool pool, Transform ownerTransform)
    {
        direction = directionNormalized.sqrMagnitude > 0f ? directionNormalized.normalized : Vector3.right;
        speed = projectileSpeed;
        damage = damageAmount;
        originPool = pool;
        lifeTimer = maxLifetime;
        owner = ownerTransform;
    }

    private void Update()
    {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            originPool?.Release(gameObject);
            return;
        }

        Vector3 next = transform.position + direction * (speed * Time.deltaTime);
        transform.position = next;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (owner != null && other.transform == owner)
        {
            return; // don't hit the owner
        }

        var damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            originPool?.Release(gameObject);
        }
    }
}
