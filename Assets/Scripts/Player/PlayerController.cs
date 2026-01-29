using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] private Rigidbody2D rb;

    private float moveSpeed;
    private float maxHealth;
    private float currentHealth;
    private bool isDead;

    public event Action<float, float> OnHealthChanged;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Start()
    {
        var stats = ConfigManager.Instance?.Balance?.PlayerStats;
        moveSpeed = stats?.MoveSpeed ?? 5f;
        maxHealth = stats?.MaxHealth ?? 100f;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        Vector2 input = new Vector2(inputX, inputY);
        Vector2 direction = input.sqrMagnitude > 1e-3f ? input.normalized : Vector2.zero;
        Vector2 delta = direction * (moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + delta);
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        if (currentHealth <= 0f)
        {
            isDead = true;
            StartCoroutine(ReloadScene());
        }
    }

    private System.Collections.IEnumerator ReloadScene()
    {
        yield return null; // wait one frame to allow UI update
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
