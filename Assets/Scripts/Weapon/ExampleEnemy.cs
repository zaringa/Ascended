using UnityEngine;

public class ExampleEnemy : MonoBehaviour, IDamageable
{
    [Header("Параметры здоровья")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Визуальные эффекты")]
    [SerializeField] private GameObject deathEffectPrefab; // Эффект смерти
    [SerializeField] private Renderer meshRenderer; // Рендерер для визуализации урона

    private Material materialInstance;
    private Color originalColor;

    private void Start()
    {
        currentHealth = maxHealth;

        // Создаем копию материала для индивидуальной визуализации
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<Renderer>();
        }

        if (meshRenderer != null)
        {
            materialInstance = meshRenderer.material;
            originalColor = materialInstance.color;
        }
    }


    // Получение урона
    public void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!IsAlive()) return;

        currentHealth -= damage;
        Debug.Log($"[{gameObject.name}] Получил урон: {damage}. Осталось здоровья: {currentHealth}");

        // Визуализация получения урона (покраснение)
        if (materialInstance != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashDamage());
        }

        // Проверка на смерть
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }
    
    private void Die()
    {
        Debug.Log($"[{gameObject.name}] Уничтожен!");

        // Создание эффекта смерти
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        /* Здесь можно добавить:
        выпадение предметов
        начисление очков игроку
        запуск анимации смерти
        И т.д. */

        // Уничтожаем объект
        Destroy(gameObject);
    }

    private System.Collections.IEnumerator FlashDamage()
    {
        // Окрашиваем в красный
        if (materialInstance != null)
        {
            materialInstance.color = Color.red;
        }

        yield return new WaitForSeconds(0.1f);

        // Возвращаем оригинальный цвет
        if (materialInstance != null)
        {
            materialInstance.color = originalColor;
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Показываем полоску здоровья над врагом
        Gizmos.color = Color.green;
        Vector3 healthBarPos = transform.position + Vector3.up * 2f;
        float healthPercent = currentHealth / maxHealth;
        Gizmos.DrawLine(healthBarPos - Vector3.right * 0.5f,
                       healthBarPos + Vector3.right * (healthPercent - 0.5f));
    }
}