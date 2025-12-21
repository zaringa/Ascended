using UnityEngine;

/// <summary>
/// Компонент снаряда для огнестрельного оружия
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    private float damage;
    private float lifetime;
    private GameObject hitDecalPrefab;
    private GameObject enemyHitDecalPrefab;
    private float decalLifetime;
    private GameObject ownerObject;
    private Rigidbody rb;
    private bool hasHit = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Инициализация снаряда
    /// </summary>
    public void Initialize(Vector3 direction, float damage, float speed, float lifetime,
        GameObject hitDecal, GameObject enemyHitDecal, float decalLifetime, GameObject ownerObject = null)
    {
        this.damage = damage;
        this.lifetime = lifetime;
        this.hitDecalPrefab = hitDecal;
        this.enemyHitDecalPrefab = enemyHitDecal;
        this.decalLifetime = decalLifetime;
        this.ownerObject = ownerObject;

        // Настройка Rigidbody
        if (rb != null)
        {
            rb.useGravity = false; // Снаряд летит прямо без гравитации
            rb.linearVelocity = direction.normalized * speed;
        }

        // Автоуничтожение через заданное время
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;

        // Проверяем, не попали ли мы в владельца или его родителей
        if (IsOwnerOrParent(collision.gameObject))
        {
            return;
        }

        hasHit = true;

        // Получаем точку и нормаль попадания
        ContactPoint contact = collision.contacts[0];
        Vector3 hitPoint = contact.point;
        Vector3 hitNormal = contact.normal;

        // Проверяем, может ли объект получать урон
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage, hitPoint, hitNormal);

            // Создаем декаль для врага
            if (enemyHitDecalPrefab != null)
            {
                CreateDecal(enemyHitDecalPrefab, hitPoint, hitNormal);
            }
        }
        else
        {
            // Создаем декаль для обычной поверхности
            if (hitDecalPrefab != null)
            {
                CreateDecal(hitDecalPrefab, hitPoint, hitNormal);
            }
        }

        // Уничтожаем снаряд
        Destroy(gameObject);
    }

    private void CreateDecal(GameObject decalPrefab, Vector3 position, Vector3 normal)
    {
        GameObject decal = Instantiate(decalPrefab, position, Quaternion.LookRotation(normal));
        decal.transform.position += normal * 0.01f; // Небольшой отступ от поверхности
        Destroy(decal, decalLifetime);
    }

    /// <summary>
    /// Проверка, является ли объект владельцем или его родителем
    /// </summary>
    private bool IsOwnerOrParent(GameObject obj)
    {
        if (ownerObject == null) return false;
        if (obj == ownerObject) return true;

        // Проверяем всю иерархию владельца (игрок, камера, оружие)
        Transform current = ownerObject.transform;
        while (current != null)
        {
            if (current.gameObject == obj)
            {
                return true;
            }
            current = current.parent;
        }

        // Проверяем иерархию объекта столкновения
        current = obj.transform;
        while (current != null)
        {
            if (current.gameObject == ownerObject)
            {
                return true;
            }
            current = current.parent;
        }

        return false;
    }
}