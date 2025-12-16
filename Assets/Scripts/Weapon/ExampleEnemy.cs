using UnityEngine;

/// <summary>
/// Пример врага для тестирования системы оружия
/// </summary>
public class ExampleEnemy : MonoBehaviour, IDamageable
{
    [Header("Параметры здоровья")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Визуальные эффекты")]
    [SerializeField] private bool showDamageEffect = true;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float damageEffectDuration = 0.2f;

    private Renderer objectRenderer;
    private Color originalColor;

    private void Start()
    {
        currentHealth = maxHealth;
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
    }

    public void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        currentHealth -= damage;
        // Debug.Log($"[{gameObject.name}] Получен урон: {damage}. Здоровье: {currentHealth}/{maxHealth}");

        if (showDamageEffect && objectRenderer != null)
        {
            ShowDamageEffect();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    private void ShowDamageEffect()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = damageColor;
            Invoke(nameof(ResetColor), damageEffectDuration);
        }
    }

    private void ResetColor()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = originalColor;
        }
    }

    private void Die()
    {
        // Debug.Log($"[{gameObject.name}] Уничтожен!");
        Destroy(gameObject);
    }
}