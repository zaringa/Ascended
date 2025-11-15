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

        // Инициализация состояния
        currentMagazineAmmo = gunInfo.maxMagazineCapacity;
        lastFireTime = -gunInfo.fireRate;
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
        isReloading = true;
        Debug.Log($"Начало перезарядки {gunInfo.gunName}...");
        
        // Анимация или звук начала
        
        yield return new WaitForSeconds(gunInfo.reloadTime);

        // В реальной игре тут будет расчет, сколько патронов взять из инвентаря.
        currentMagazineAmmo = gunInfo.maxMagazineCapacity;

        isReloading = false;
        Debug.Log("Перезарядка завершена.");
    }
}