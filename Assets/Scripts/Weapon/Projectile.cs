using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Настройки снаряда")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float speed = 100f;
    [SerializeField] private float lifetime = 5f;

    [Header("Эффекты попадания")]
    [SerializeField] private GameObject hitDecalPrefab;
    [SerializeField] private GameObject enemyHitDecalPrefab;
    [SerializeField] private float decalLifetime = 10f;

    [Header("Визуальные эффекты (опционально)")]
    [SerializeField] private GameObject hitEffectPrefab; // Эффект частиц при попадании
    [SerializeField] private TrailRenderer trailRenderer; // След снаряда

    private Rigidbody rb;
    private Collider projectileCollider;
    private bool hasHit = false;
    private Vector3 direction;
    private GameObject owner; // Кто выстрелил (чтобы игнорировать коллизии)

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        projectileCollider = GetComponent<Collider>();

        // Настройка Rigidbody для снаряда
        rb.useGravity = false; // Снаряд летит по прямой без гравитации
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // Устанавливаем слой "Projectile" если он существует
        int projectileLayer = LayerMask.NameToLayer("Projectile");
        if (projectileLayer != -1)
        {
            gameObject.layer = projectileLayer;
        }
    }


    /*
    direction Направление полета (уже нормализованное)
    damage Урон
    speed Скорость полета
    lifetime Время жизни
    hitDecal Префаб декали для поверхностей
    enemyHitDecal Префаб декали для врагов
    decalLifetime Время жизни декали
    ownerObject Объект который выстрелил - костыль для того чтобы не стрелять в себя дефолт оружием
    */
    public void Initialize(Vector3 direction, float damage, float speed, float lifetime,
                          GameObject hitDecal = null, GameObject enemyHitDecal = null, float decalLifetime = 10f,
                          GameObject ownerObject = null)
    {
        // Убеждаемся что Rigidbody инициализирован
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }
        }

        this.direction = direction.normalized;
        this.damage = damage;
        this.speed = speed;
        this.lifetime = lifetime;
        this.hitDecalPrefab = hitDecal;
        this.enemyHitDecalPrefab = enemyHitDecal;
        this.decalLifetime = decalLifetime;
        this.owner = ownerObject;

        // Игнор коллизии с владельцем 
        if (owner != null && projectileCollider != null)
        {
            Collider[] ownerColliders = owner.GetComponentsInChildren<Collider>();
            foreach (Collider ownerCol in ownerColliders)
            {
                Physics.IgnoreCollision(projectileCollider, ownerCol, true);
            }
        }

        Debug.Log($"[Projectile] Создан снаряд в позиции {transform.position}, направление: {this.direction}, скорость: {this.speed}");

        // Скорость снаряда 
        if (rb != null)
        {
            #if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = this.direction * this.speed;
            Debug.Log($"[Projectile] Установлена linearVelocity: {rb.linearVelocity}");
            #else
            rb.velocity = this.direction * this.speed;
            Debug.Log($"[Projectile] Установлена velocity: {rb.velocity}");
            #endif
        }
        else
        {
            Debug.LogError("[Projectile] Rigidbody не найден! Снаряд не будет двигаться!");
        }

        // Направление полета
        transform.forward = this.direction;

        // Уничтожаем снаряд через время жизни
        Destroy(gameObject, this.lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;
        hasHit = true;

        // Останавливаем снаряд
        #if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = Vector3.zero;
        #else
        rb.velocity = Vector3.zero;
        #endif
        rb.isKinematic = true;

        // Точка попадания
        ContactPoint contact = collision.contacts[0];
        Vector3 hitPoint = contact.point;
        Vector3 hitNormal = contact.normal;

        //IDamageable для регистра выстрела обхект должен реализовывать этот интрфейс
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        bool hitEnemy = damageable != null;

        if (hitEnemy)
        {
            damageable.TakeDamage(damage, hitPoint, hitNormal);
            Debug.Log($"Снаряд попал в {collision.gameObject.name}, урон: {damage}");

            // Создаем декаль
            if (enemyHitDecalPrefab != null)
            {
                CreateDecal(enemyHitDecalPrefab, hitPoint, hitNormal, collision.transform);
            }
            else if (hitDecalPrefab != null)
            {
                // Если нет специальной декали для врага, используем обычную
                CreateDecal(hitDecalPrefab, hitPoint, hitNormal, collision.transform);
            }
        }
        else
        {
            // Попали в обычную поверхность
            Debug.Log($"Снаряд попал в поверхность: {collision.gameObject.name}");

            // Создаем декаль попадания
            if (hitDecalPrefab != null)
            {
                CreateDecal(hitDecalPrefab, hitPoint, hitNormal, collision.transform);
            }
        }

        // Создаем эффект попадания (частицы)
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, hitPoint, Quaternion.LookRotation(hitNormal));
            Destroy(effect, 2f); // Уничтожаем эффект через 2 секунды
        }
        DisableVisuals();
        Destroy(gameObject, 0.05f);
    }
    
    // Создание декали
    private void CreateDecal(GameObject decalPrefab, Vector3 position, Vector3 normal, Transform parent)
    {
        if (decalPrefab == null)
        {
            Debug.LogWarning("[Projectile] Префаб декали не назначен!");
            return;
        }

        /*
         Создаем декаль - перпендикулярно поверхности
         Quad рендерится только с одной стороны
         Quad смотрит назад по нормали
        */
        
        Quaternion rotation = Quaternion.LookRotation(-normal);
        Vector3 decalPosition = position + normal * 0.05f;
        GameObject decal = Instantiate(decalPrefab, decalPosition, rotation);

        if (decal == null)
        {
            Debug.LogError("[Projectile] ОШИБКА! Декаль не создалась (Instantiate вернул null)!");
            return;
        }

        Debug.Log($"[Projectile] ✓ Декаль создана! Имя: {decal.name}, Позиция: {decalPosition}, Active: {decal.activeSelf}");
        decal.SetActive(true);
        
        if (decal.transform.localScale.magnitude < 0.5f)
        {
            decal.transform.localScale = Vector3.one * 0.5f; // Увеличиваем до 0.5 метров
            Debug.Log($"[Projectile] Декаль увеличена до размера {decal.transform.localScale}");
        }
        
        if (parent != null)
        {
            decal.transform.SetParent(parent, true);
            Debug.Log($"[Projectile] Декаль привязана к родителю: {parent.name}");
        }
        
        MeshRenderer renderer = decal.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            Debug.LogWarning("[Projectile] У декали нет MeshRenderer!");
        }
        else
        {
            Debug.Log($"[Projectile] MeshRenderer найден, enabled: {renderer.enabled}, material: {renderer.material?.name}");
        }
        
        Debug.Log($"[Projectile] Декаль будет уничтожена через {decalLifetime} секунд");
        Destroy(decal, decalLifetime);
    }
    
    private void DisableVisuals()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
        
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }
        
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
    }
}