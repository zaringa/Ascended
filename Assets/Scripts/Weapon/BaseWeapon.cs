using UnityEngine;
using System.Collections;

public abstract class BaseWeapon : MonoBehaviour
{
    // Ссылка на данные
    public GunInfo gunInfo;

    [Header("Текущее состояние")]
    protected int currentMagazineAmmo;
    protected bool isReloading = false;
    protected float lastFireTime;

    protected virtual void Start()
    {
        if (gunInfo == null)
        {
            Debug.LogError("GunInfo не задан для " + gameObject.name);
            return;
        }

        // Проверка специфичных данных для типа оружия
        if (gunInfo.type == GunInfo.WeaponType.Firearm)
        {
            if (gunInfo.projectilePrefab == null)
            {
                Debug.LogWarning($"[{gunInfo.name}] Не задан префаб снаряда для огнестрельного оружия!");
            }
        }
    }
    
    // *** АБСТРАКТНЫЙ МЕТОД: Должен быть реализован каждым дочерним классом ***
    public abstract bool TryToFire();

    // --- Общая логика перезарядки (одинаковая для всего огнестрела) ---
    public void StartReload()
    {
        // Для примера, перезаряжать можно только огнестрельное оружие
        if (gunInfo.type != GunInfo.WeaponType.Firearm) return;
        
        // Тут нужна дополнительная логика для проверки общего запаса патронов
        // В рамках этого примера, упростим:
        if (isReloading || currentMagazineAmmo == gunInfo.maxMagazineCapacity)
        {
            return;
        }

        StartCoroutine(ReloadCoroutine());
    }

    protected IEnumerator ReloadCoroutine()
    {
        /*isReloading = true;

        OnReloadStarted?.Invoke();
        Debug.Log($"Начало перезарядки {gunInfo.name}...");

        // Воспроизведение звука перезарядки
        PlaySound(gunInfo.reloadSound);

        // Анимация перезарядки
        if (animator != null)
        {
            animator.SetTrigger("Reload");
        }

        yield return new WaitForSeconds(gunInfo.reloadTime);

        // В реальной игре тут будет расчет, сколько патронов взять из инвентаря.
        currentMagazineAmmo = gunInfo.maxMagazineCapacity;

        isReloading = false;
        Debug.Log("Перезарядка завершена.");*/
        yield return new WaitForSeconds(1); //! Удалить после фикса !
    }
}