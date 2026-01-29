using UnityEngine;

public class AuraAbility : BaseAbility
{
    [SerializeField] private float radiusOverride = -1f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform auraVisual;
    [SerializeField] private bool debugLogHits;

    private void Awake()
    {
        SetVisualActive(false);
    }

    public override void Configure(AbilityStats abilityStats, int newLevel)
    {
        base.Configure(abilityStats, newLevel);
        UpdateVisual();
        enabled = true;
    }

    private void OnDisable()
    {
        SetVisualActive(false);
    }

    private float ResolveRadius()
    {
        return radiusOverride > 0f ? radiusOverride : GetArea();
    }

    protected override bool Activate()
    {
        if (stats == null)
        {
            return false;
        }

        float radius = ResolveRadius();
        if (radius <= 0f)
        {
            return false;
        }

        int mask = enemyLayer.value == 0 ? ~0 : enemyLayer.value;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, mask);
        bool didHit = false;
        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(GetDamage());
                didHit = true;

                if (debugLogHits && hit.TryGetComponent<EnemyAI>(out var enemy))
                {
                    Debug.Log($"Aura dealt {GetDamage()} to {enemy.name}, HP left: {enemy.GetCurrentHealth():0.##}");
                }
            }
        }

        if (debugLogHits && didHit)
        {
            Debug.Log($"Aura hit {hits.Length} colliders, damage {GetDamage()} level {level}");
        }

        return didHit;
    }

    private void UpdateVisual()
    {
        if (auraVisual == null)
        {
            return;
        }

        float radius = ResolveRadius();
        if (radius <= 0f)
        {
            SetVisualActive(false);
            return;
        }

        SetVisualActive(true);
        float desiredDiameter = radius * 2f;
        float baseDiameter = GetBaseVisualDiameter();
        float scale = baseDiameter > Mathf.Epsilon ? desiredDiameter / baseDiameter : desiredDiameter;
        auraVisual.localScale = new Vector3(scale, scale, 1f);
        auraVisual.localPosition = Vector3.zero;

        var renderer = auraVisual.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            var color = renderer.color;
            color.a = 0.25f;
            renderer.color = color;
        }
    }

    private float GetBaseVisualDiameter()
    {
        var renderer = auraVisual.GetComponent<SpriteRenderer>();
        if (renderer != null && renderer.sprite != null)
        {
            var size = renderer.sprite.bounds.size;
            return Mathf.Max(size.x, size.y);
        }

        return Mathf.Max(auraVisual.localScale.x, auraVisual.localScale.y);
    }

    private void SetVisualActive(bool active)
    {
        if (auraVisual != null)
        {
            auraVisual.gameObject.SetActive(active);
        }
    }

    private void OnDrawGizmosSelected()
    {
        float radius = ResolveRadius();
        if (radius <= 0f)
        {
            return;
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
