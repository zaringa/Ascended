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

        // Проверка наличия коллайдера
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = 0.05f;
        }
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
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous; 
            rb.linearVelocity = direction.normalized * speed;
        }

        // Автоуничтожение через заданное время
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;

        // Игнорируем столкновения с декалями (они могут содержать "Decal" или "Gun" в имени)
        string objName = collision.gameObject.name.ToLower();
        if (objName.Contains("decal") || objName.Contains("gun1") || objName.Contains("hitdecal"))
        {
            return;
        }
        if (IsOwnerOrParent(collision.gameObject))
        {
            return;
        }

        hasHit = true;

        Vector3 hitPoint;
        Vector3 hitNormal;

        RaycastHit rayHit;
        Vector3 rayOrigin = transform.position - rb.linearVelocity.normalized * 0.5f;
        Vector3 rayDirection = rb.linearVelocity.normalized;

        if (Physics.Raycast(rayOrigin, rayDirection, out rayHit, 1f))
        {
            hitPoint = rayHit.point;
            hitNormal = rayHit.normal;
        }
        else
        {
            ContactPoint contact = collision.contacts[0];
            hitPoint = contact.point;
            hitNormal = contact.normal;
        }

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
        Quaternion decalRotation = Quaternion.LookRotation(normal);

        Vector3 decalPosition = position + normal * 0.01f;

        GameObject decal = Instantiate(decalPrefab, decalPosition, decalRotation);
        
        // Удаляем все коллайдеры с декали - это только визуальный эффект
        Collider[] colliders = decal.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            Destroy(col);
        }

        // Компенсируем смещение дочернего объекта в префабе
        Transform childTransform = decal.transform.GetChild(0);
        if (childTransform != null)
        {
            Vector3 childLocalPos = childTransform.localPosition;
            decal.transform.position -= decal.transform.up * childLocalPos.y;
        }

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