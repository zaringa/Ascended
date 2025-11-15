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
    protected bool isAttacking = false; // Для холодного оружия
    protected float lastFireTime;

    [Header("Компоненты")]
    protected AudioSource audioSource;
    protected Animator animator;

    // События для интеграции с UI и другими системами
    public event Action<int, int> OnAmmoChanged; // (текущие патроны, макс. в обойме)
    public event Action OnReloadStarted;
    public event Action OnReloadCompleted;
    public event Action OnWeaponFired;
    public event Action OnWeaponAttack; // Для холодного оружия

    public int CurrentAmmo => currentMagazineAmmo;
    public int MaxAmmo => gunInfo != null ? gunInfo.maxMagazineCapacity : 0;
    public bool IsReloading => isReloading;
    public bool IsAttacking => isAttacking;
    public GunInfo.WeaponType WeaponType => gunInfo != null ? gunInfo.type : GunInfo.WeaponType.Firearm;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D звук
        }

        animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        ValidateWeaponData();

        if (gunInfo == null) return;

        // Инициализация состояния для огнестрельного оружия
        if (gunInfo.type == GunInfo.WeaponType.Firearm)
        {
            currentMagazineAmmo = gunInfo.maxMagazineCapacity;
            lastFireTime = -gunInfo.fireRate;
            OnAmmoChanged?.Invoke(currentMagazineAmmo, gunInfo.maxMagazineCapacity);
        }
    }

    // Валидация данных оружия
    protected virtual void ValidateWeaponData()
    {
        if (gunInfo == null)
        {
            Debug.LogError($"[{gameObject.name}] GunInfo не задан!");
            enabled = false;
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

    // АБСТРАКТНЫЙ МЕТОД: Должен быть реализован каждым дочерним классом
    public abstract bool TryToFire();
    
    public virtual void OnFireButtonPressed()
    {
        PlaySound(gunInfo.fireSound);
    }
    
    public virtual void OnFireButtonReleased()
    {
        // Можно добавить логику остановки звука
    }

    // Общая логика перезарядки
    public virtual void StartReload()
    {
        // Перезаряжать можно только огнестрельное оружие
        if (gunInfo.type != GunInfo.WeaponType.Firearm) return;

        // Нельзя перезаряжаться во время перезарядки или атаки
        if (isReloading || isAttacking) return;

        // Если обойма уже полная
        if (currentMagazineAmmo == gunInfo.maxMagazineCapacity)
        {
            Debug.Log($"[{gunInfo.name}] Обойма уже полная!");
            return;
        }

        StartCoroutine(ReloadCoroutine());
    }

    protected virtual IEnumerator ReloadCoroutine()
    {
        isReloading = true;
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

        // Тут будет расчет, сколько патронов взять из инвентаря
        // Пока что просто заполняем обойму полностью
        currentMagazineAmmo = gunInfo.maxMagazineCapacity;

        isReloading = false;
        OnReloadCompleted?.Invoke();
        OnAmmoChanged?.Invoke(currentMagazineAmmo, gunInfo.maxMagazineCapacity);
        Debug.Log("Перезарядка завершена.");
    }

    // Вспомогательный метод для воспроизведения звуков
    protected void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Обновление счетчика патронов
    protected void UpdateAmmo(int newAmmo)
    {
        currentMagazineAmmo = newAmmo;
        OnAmmoChanged?.Invoke(currentMagazineAmmo, gunInfo.maxMagazineCapacity);
    }
    
    protected void InvokeWeaponFired()
    {OnWeaponFired?.Invoke();}

    protected void InvokeWeaponAttack()
    {OnWeaponAttack?.Invoke();}

    protected void InvokeAmmoChanged(int current, int max)
    {OnAmmoChanged?.Invoke(current, max);}

    // Метод для получения точки выстрела
    public virtual Vector3 GetFirePoint()
    {return transform.position + transform.forward;}

    // Метод для получения направления выстрела
    public virtual Vector3 GetFireDirection()
    {return transform.forward;}
}