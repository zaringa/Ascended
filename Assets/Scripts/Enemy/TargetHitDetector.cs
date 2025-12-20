using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Улучшенный детектор попаданий для мишеней
    /// Обеспечивает регистрацию попаданий с любого угла
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TargetHitDetector : MonoBehaviour
    {
        [Header("Hit Detection")]
        [SerializeField] private TargetEnemy parentTarget;
        [SerializeField] private bool autoFindParent = true;
        [SerializeField] private bool showDebugInfo = false;

        private Collider hitCollider;

        private void Awake()
        {
            hitCollider = GetComponent<Collider>();

            // Убедимся, что коллайдер не триггер
            if (hitCollider.isTrigger)
            {
                Debug.LogWarning($"TargetHitDetector на {gameObject.name}: Коллайдер не должен быть триггером для правильной регистрации попаданий!");
            }

            // Автоматически найти родительскую мишень
            if (autoFindParent && parentTarget == null)
            {
                parentTarget = GetComponentInParent<TargetEnemy>();
                if (parentTarget == null)
                {
                    Debug.LogError($"TargetHitDetector на {gameObject.name}: Не найден родительский TargetEnemy!");
                }
            }
        }

        private void Start()
        {
            // Проверка настроек коллайдера
            ValidateCollider();
        }

        private void ValidateCollider()
        {
            if (hitCollider == null) return;

            BoxCollider box = hitCollider as BoxCollider;
            if (box != null)
            {
                // Рекомендуем минимальную толщину для регистрации попаданий с обеих сторон
                if (box.size.z < 0.5f)
                {
                    if (showDebugInfo)
                    {
                        Debug.LogWarning($"TargetHitDetector: BoxCollider слишком тонкий (z={box.size.z}). Рекомендуется >= 0.5 для лучшей регистрации попаданий.");
                    }
                }
            }

            SphereCollider sphere = hitCollider as SphereCollider;
            if (sphere != null)
            {
                if (sphere.radius < 0.5f && showDebugInfo)
                {
                    Debug.LogWarning($"TargetHitDetector: SphereCollider слишком маленький (radius={sphere.radius}). Рекомендуется >= 0.5");
                }
            }
        }

        // Этот метод будет вызываться при попадании пули/raycast
        public void OnHit(float damage, Vector3 hitPoint, Vector3 hitNormal)
        {
            if (parentTarget != null)
            {
                if (showDebugInfo)
                {
                    Debug.Log($"TargetHitDetector: Попадание зарегистрировано! Damage: {damage}, Point: {hitPoint}");
                }

                parentTarget.TakeDamage(damage, hitPoint, hitNormal);
            }
        }

        private void OnDrawGizmos()
        {
            if (!showDebugInfo) return;

            // Визуализация коллайдера
            Gizmos.color = Color.green;

            if (hitCollider != null)
            {
                BoxCollider box = hitCollider as BoxCollider;
                if (box != null)
                {
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawWireCube(box.center, box.size);
                }

                SphereCollider sphere = hitCollider as SphereCollider;
                if (sphere != null)
                {
                    Gizmos.DrawWireSphere(transform.position + sphere.center, sphere.radius);
                }
            }
        }

        // Public методы для внешнего использования
        public void SetParentTarget(TargetEnemy target)
        {
            parentTarget = target;
        }

        public TargetEnemy GetParentTarget()
        {
            return parentTarget;
        }
    }
}