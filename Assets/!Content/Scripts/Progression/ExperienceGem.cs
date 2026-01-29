using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ExperienceGem : MonoBehaviour
{
    private int xpValue;
    private ObjectPool originPool;

    public void Init(int value, ObjectPool pool)
    {
        xpValue = value;
        originPool = pool;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out _))
        {
            return;
        }

        ExperienceManager.Instance?.AddExperience(xpValue);
        originPool?.Release(gameObject);
    }
}
