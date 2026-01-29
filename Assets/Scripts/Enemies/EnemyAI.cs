using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IDamageable
{
    private Transform target;
    private float speed;
    private float currentHealth;
    private float damage;
    private int xpValue;

    private Action<EnemyAI> onDeath;

    private float attackRange;
    [SerializeField] private float attackCooldown = 1f;
    private float attackTimer;
    private IDamageable targetDamageable;

    public void Init(EnemyStats stats, Transform targetTransform, Action<EnemyAI> onDeathCallback)
    {
        target = targetTransform;
        speed = stats?.Speed ?? 1f;
        currentHealth = stats?.HP ?? 1f;
        damage = stats?.Damage ?? 1f;
        xpValue = stats?.XP_Value ?? 1;
        onDeath = onDeathCallback;
        attackRange = stats?.AttackRange ?? 0.5f;
        targetDamageable = targetTransform != null ? targetTransform.GetComponent<IDamageable>() : null;
        attackTimer = 0f;
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        Vector2 current = transform.position;
        Vector2 destination = target.position;
        Vector2 toTarget = destination - current;
        float sqrDist = toTarget.sqrMagnitude;
        float stopSqr = attackRange * attackRange;

        if (sqrDist > stopSqr && toTarget != Vector2.zero)
        {
            Vector2 stopPosition = destination - toTarget.normalized * attackRange;
            Vector2 next = Vector2.MoveTowards(current, stopPosition, speed * Time.deltaTime);
            transform.position = next;
        }
        else
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (targetDamageable == null)
        {
            return;
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            targetDamageable.TakeDamage(damage);
            attackTimer = attackCooldown;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        onDeath?.Invoke(this);
    }

    public int GetXpValue()
    {
        return xpValue;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
