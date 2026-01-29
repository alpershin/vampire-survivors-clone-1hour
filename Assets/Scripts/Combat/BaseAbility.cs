using UnityEngine;

public abstract class BaseAbility : MonoBehaviour
{
    [SerializeField] protected string abilityName;
    protected AbilityStats stats;
    protected int level;
    private float cooldownTimer;
    private bool configured;

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        if (!configured)
        {
            return;
        }

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f && CanActivate())
        {
            if (Activate())
            {
                cooldownTimer = GetCooldown();
            }
        }
    }

    public virtual void Configure(AbilityStats abilityStats, int newLevel)
    {
        stats = abilityStats;
        level = Mathf.Max(1, newLevel);
        configured = stats != null;
        cooldownTimer = 0f;
    }

    protected virtual bool CanActivate()
    {
        return true;
    }

    protected virtual float GetCooldown()
    {
        return stats?.Cooldown ?? 1f;
    }

    protected virtual float GetDamage()
    {
        return stats != null ? stats.Damage * level : 0f;
    }

    protected virtual float GetArea()
    {
        return stats?.Area ?? 0f;
    }

    protected virtual float GetSpeed()
    {
        return stats?.Speed ?? 0f;
    }

    protected virtual int GetProjectileCount()
    {
        return stats != null ? Mathf.Max(1, stats.ProjectileCount + (level - 1)) : 0;
    }

    protected abstract bool Activate();
}
