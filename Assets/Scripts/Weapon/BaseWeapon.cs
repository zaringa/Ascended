using UnityEngine;
using System.Collections;
using System;

public abstract class BaseWeapon : MonoBehaviour
{
    // Ссылка на данные
    public GunInfo gunInfo;

    [Header("Текущее состояние")]
    protected int currentMagazineAmmo;
    protected bool isReloading = false;
    protected bool isAttacking = false;
    protected float lastFireTime;

    [Header("Компоненты")]
    protected AudioSource audioSource;
    protected Animator animator;

    // События
    public event Action<int, int> OnAmmoChanged; // (текущие патроны, максимум)
    public event Action OnReloadStarted;
    public event Action OnReloadCompleted;
    public event Action OnWeaponFired;
    public event Action OnWeaponAttack;

    // Публичные свойства
    public int CurrentAmmo => currentMagazineAmmo;
    public int MaxAmmo => gunInfo != null ? gunInfo.maxMagazineCapacity : 0;
    public bool IsReloading => isReloading;
    public bool IsAttacking => isAttacking;
    public GunInfo.WeaponType WeaponType => gunInfo != null ? gunInfo.type : GunInfo.WeaponType.Firearm;

    protected virtual void Start()
    {
        if (gunInfo == null)
        {
            Debug.LogError("GunInfo не задан для " + gameObject.name);
            return;
        }

        // Получаем компоненты
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        animator = GetComponent<Animator>();

        // Инициализация патронов
        if (gunInfo.type == GunInfo.WeaponType.Firearm)
        {
            currentMagazineAmmo = gunInfo.maxMagazineCapacity;
            InvokeAmmoChanged(currentMagazineAmmo, gunInfo.maxMagazineCapacity);

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
        isReloading = true;

        OnReloadStarted?.Invoke();
        // Debug.Log($"Начало перезарядки {gunInfo.name}...");

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
        OnReloadCompleted?.Invoke();
        InvokeAmmoChanged(currentMagazineAmmo, gunInfo.maxMagazineCapacity);
        // Debug.Log("Перезарядка завершена.");
    }

    // --- Вспомогательные методы ---

    /// <summary>
    /// Воспроизведение звука
    /// </summary>
    protected void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// Вызов события выстрела
    /// </summary>
    protected void InvokeWeaponFired()
    {
        OnWeaponFired?.Invoke();
    }

    /// <summary>
    /// Вызов события атаки холодным оружием
    /// </summary>
    protected void InvokeWeaponAttack()
    {
        OnWeaponAttack?.Invoke();
    }

    /// <summary>
    /// Вызов события изменения патронов
    /// </summary>
    protected void InvokeAmmoChanged(int current, int max)
    {
        OnAmmoChanged?.Invoke(current, max);
    }

    /// <summary>
    /// Публичный метод для обработки нажатия кнопки огня (для автоматического оружия)
    /// </summary>
    public virtual void OnFireButtonPressed()
    {
        // Переопределяется в дочерних классах при необходимости
    }

    /// <summary>
    /// Публичный метод для обработки отпускания кнопки огня (для автоматического оружия)
    /// </summary>
    public virtual void OnFireButtonReleased()
    {
        // Переопределяется в дочерних классах при необходимости
    }

    /// <summary>
    /// Получить точку выстрела (переопределяется в дочерних классах)
    /// </summary>
    public virtual Vector3 GetFirePoint()
    {
        return transform.position;
    }

    /// <summary>
    /// Получить направление выстрела (переопределяется в дочерних классах)
    /// </summary>
    public virtual Vector3 GetFireDirection()
    {
        return transform.forward;
    }
}