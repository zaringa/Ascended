using UnityEngine;

public class PistolWeapon : BaseWeapon
{
    [Header("Точка вылета снаряда")]
    [SerializeField] private Transform firePoint; // Точка, откуда вылетает снаряд

    [Header("Визуальные эффекты")]
    [SerializeField] private ParticleSystem muzzleFlash; // Вспышка выстрела
    [SerializeField] private Light muzzleLight; // Свет от выстрела

    protected override void Start()
    {
        base.Start();

        // Если точка выстрела не задана, используем позицию оружия
        if (firePoint == null)
        {
            Debug.LogWarning($"[{gunInfo.gunName}] Fire Point не назначен, используется позиция оружия");
            firePoint = transform;
        }

        if (muzzleLight != null)
        {muzzleLight.enabled = false;}
    }

    public override bool TryToFire()
    {
        // Проверка готовности (перезарядка, скорострельность)
        if (isReloading || Time.time < lastFireTime + gunInfo.fireRate)
        {return false;}

        if (currentMagazineAmmo > 0)
        {
            currentMagazineAmmo--;
            lastFireTime = Time.time;
            Fire();
            InvokeAmmoChanged(currentMagazineAmmo, gunInfo.maxMagazineCapacity);
            return true;
        }
        else
        {
            Debug.Log($"[{gunInfo.gunName}] Патроны закончились! Нужна перезарядка.");
            return false;
        }
    }

    private void Fire()
    {
        Debug.Log($"[{gunInfo.gunName}] Выстрел! Урон: {gunInfo.damage}, Патронов осталось: {currentMagazineAmmo}");
        InvokeWeaponFired();
        // PlaySound(gunInfo.fireSound); - временный мут чтобы воспроизводить очередь
        PlayMuzzleFlash();
        if (animator != null)
        {animator.SetTrigger("Fire");}
        SpawnProjectile();
    }

    private void SpawnProjectile()
    {
        if (gunInfo.projectilePrefab == null)
        {
            Debug.LogError($"[{gunInfo.gunName}] Префаб снаряда не назначен!");
            return;
        }

        Vector3 spawnPosition = GetFirePoint();
        Vector3 fireDirection = GetFireDirection();
        GameObject projectileObj = Instantiate(gunInfo.projectilePrefab, spawnPosition, Quaternion.identity);

        // Получаем компонент снаряда и инициализируем его
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(
                direction: fireDirection,
                damage: gunInfo.damage,
                speed: gunInfo.projectileSpeed,
                lifetime: gunInfo.projectileLifetime,
                hitDecal: gunInfo.hitDecalPrefab,
                enemyHitDecal: gunInfo.enemyHitDecalPrefab,
                decalLifetime: gunInfo.decalLifetime,
                ownerObject: gameObject // Передаем оружие как владельца
            );
        }
        else
        {
            Debug.LogError($"[{gunInfo.gunName}] У префаба снаряда нет компонента Projectile!");
            Destroy(projectileObj);
        }
    }

    private void PlayMuzzleFlash()
    {
        if (muzzleFlash != null)
        {muzzleFlash.Play();}
        if (muzzleLight != null)
        {
            muzzleLight.enabled = true;
            Invoke(nameof(DisableMuzzleLight), 0.05f); // Выключаем через 50мс
        }
    }

    private void DisableMuzzleLight()
    {
        if (muzzleLight != null)
        {muzzleLight.enabled = false;}
    }

    
    // Переопределение точки выстрела
    public override Vector3 GetFirePoint()
    { return firePoint != null ? firePoint.position : transform.position; }

    // Переопределение направления выстрела
    public override Vector3 GetFireDirection()
    {return firePoint != null ? firePoint.forward : transform.forward;}
}