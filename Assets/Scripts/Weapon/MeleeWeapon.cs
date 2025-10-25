using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MeleeWeapon : BaseWeapon
{
    [Header("Коллайдер лезвия")]
    [SerializeField] private Collider bladeCollider; // Коллайдер лезвия для определения попаданий

    [Header("Дополнительные настройки")]
    [SerializeField] private bool useSphereCast = false; // Альтернативный метод детекции через SphereCast
    [SerializeField] private float sphereCastRadius = 0.5f;

    private HashSet<IDamageable> hitTargets = new HashSet<IDamageable>(); // Чтобы не наносить урон дважды за один удар
    private bool canDealDamage = false; // Флаг, можно ли наносить урон в данный момент

    protected override void Start()
    {
        base.Start();

        // Проверка наличия коллайдера лезвия
        if (bladeCollider == null)
        {
            Debug.LogWarning($"[{gunInfo.gunName}] Коллайдер лезвия не назначен! Ищем автоматически...");
            bladeCollider = GetComponentInChildren<Collider>();
        }

        // Изначально коллайдер лезвия должен быть триггером и выключен
        if (bladeCollider != null)
        {
            bladeCollider.isTrigger = true;
            bladeCollider.enabled = false;
        }
        else
        {
            Debug.LogError($"[{gunInfo.gunName}] Коллайдер лезвия не найден! Холодное оружие не будет работать!");
        }
    }
    public override bool TryToFire()
    {
        // Проверка, что оружие не атакует и не перезаряжается
        if (isAttacking || isReloading)
        {
            return false;
        }

        // Проверка скорострельности (для ближнего это задержка между ударами)
        if (Time.time < lastFireTime + gunInfo.fireRate)
        {
            return false;
        }
        StartCoroutine(AttackCoroutine());
        lastFireTime = Time.time;
        return true;
    }
    
    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        hitTargets.Clear();
        InvokeWeaponAttack();

        Debug.Log($"[{gunInfo.gunName}] Начало атаки холодным оружием");

        // Воспроизведение звука взмаха
        PlaySound(gunInfo.meleeSwingSound);

        // Запуск анимации удара
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // Ждем начала фазы удара (можно настроить через аниматор события)
        // Для простоты используем долю от длительности анимации
        float attackStartDelay = gunInfo.meleeAttackDuration * 0.3f; // Урон начинает наноситься через 30% анимации
        float attackEndDelay = gunInfo.meleeAttackDuration * 0.7f;   // Урон перестает наноситься через 70% анимации

        yield return new WaitForSeconds(attackStartDelay);

        canDealDamage = true;
        if (bladeCollider != null)
        {
            bladeCollider.enabled = true;
        }

        Debug.Log($"[{gunInfo.gunName}] Фаза нанесения урона началась");

        // Ждем окончания фазы удара
        yield return new WaitForSeconds(attackEndDelay - attackStartDelay);

        // Выключаем возможность нанесения урона
        canDealDamage = false;
        if (bladeCollider != null)
        {
            bladeCollider.enabled = false;
        }

        Debug.Log($"[{gunInfo.gunName}] Фаза нанесения урона закончилась");

        // Ждем окончания анимации
        yield return new WaitForSeconds(gunInfo.meleeAttackDuration - attackEndDelay);

        isAttacking = false;
        Debug.Log($"[{gunInfo.gunName}] Атака завершена. Поражено целей: {hitTargets.Count}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canDealDamage) return;

        // Проверяем, находится ли объект на нужном слое
        if (gunInfo.meleeHitLayers != (gunInfo.meleeHitLayers | (1 << other.gameObject.layer)))
        {
            return;
        }

        // Проверяем, есть ли у объекта интерфейс IDamageable
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && !hitTargets.Contains(damageable))
        {
            // Урон
            Vector3 hitPoint = other.ClosestPoint(bladeCollider.bounds.center);
            Vector3 hitNormal = (bladeCollider.bounds.center - hitPoint).normalized;

            damageable.TakeDamage(gunInfo.damage, hitPoint, hitNormal);
            hitTargets.Add(damageable);

            // Звук попадания
            PlaySound(gunInfo.meleeHitSound);

            Debug.Log($"[{gunInfo.gunName}] Попадание по {other.gameObject.name}, урон: {gunInfo.damage}");

            // Можно добавить эффекты попадания
            CreateMeleeHitEffect(hitPoint, hitNormal);
        }
    }
    
    private void PerformSphereCastAttack()
    {
        if (!canDealDamage) return;

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        RaycastHit[] hits = Physics.SphereCastAll(origin, sphereCastRadius, direction, gunInfo.meleeRange, gunInfo.meleeHitLayers);

        foreach (RaycastHit hit in hits)
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null && !hitTargets.Contains(damageable))
            {
                damageable.TakeDamage(gunInfo.damage, hit.point, hit.normal);
                hitTargets.Add(damageable);

                PlaySound(gunInfo.meleeHitSound);
                CreateMeleeHitEffect(hit.point, hit.normal);

                Debug.Log($"[{gunInfo.gunName}] SphereCast попадание по {hit.collider.gameObject.name}");
            }
        }
    }

    // Создание эффектов попадания холодного оружия
    private void CreateMeleeHitEffect(Vector3 position, Vector3 normal)
    {
        // Здесь можно добавить партиклы 
        Debug.Log($"Эффект попадания в точке {position}");

        // Можно создать декаль если нужно
        if (gunInfo.hitDecalPrefab != null)
        {
            GameObject decal = Instantiate(gunInfo.hitDecalPrefab, position, Quaternion.LookRotation(normal));
            decal.transform.position += normal * 0.01f;
            Destroy(decal, gunInfo.decalLifetime);
        }
    }
    
    // Визуализация зоны атаки
    private void OnDrawGizmosSelected()
    {
        if (gunInfo == null) return;
        Gizmos.color = Color.red;
        if (useSphereCast)
        {
            // Показываем зону SphereCast
            Gizmos.DrawWireSphere(transform.position + transform.forward * gunInfo.meleeRange / 2, sphereCastRadius);
        }
        else if (bladeCollider != null)
        {
            // Показываем коллайдер лезвия
            Gizmos.DrawWireCube(bladeCollider.bounds.center, bladeCollider.bounds.size);
        }
    }
}