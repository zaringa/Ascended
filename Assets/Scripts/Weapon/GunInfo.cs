using Player.Items;
using UnityEngine;
using Systems.Stats;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewGunInfo", menuName = "Game/Gun Info")]
public class GunInfo : RenderableItem, IStatModifierSource 
{
    [Header("Идентификация")]
    public string gunName = "Default Weapon";
    public Sprite weaponSprite;
    
    // Перечисление для типизации оружия
    public enum WeaponType { Melee, Firearm };
    public WeaponType type = WeaponType.Firearm;

    [Header("Визуализация")]
    public GameObject weaponModelPrefab; // 3D-модель (Префаб)

    [Header("Характеристики")]
    public float damage = 10f;
    public int maxMagazineCapacity = 10;
    public float fireRate = 0.1f; // Время между выстрелами
    public float reloadTime = 2.0f; // Время перезарядки

    [Header("Снаряды (Projectiles)")]
    public GameObject projectilePrefab; // Префаб снаряда
    public float projectileSpeed = 100f; // Скорость полета снаряда
    public float projectileLifetime = 5f; // Время жизни снаряда

    [Header("Декали")]
    public GameObject hitDecalPrefab; // Префаб декали попадания по поверхности
    public GameObject enemyHitDecalPrefab; // Префаб декали попадания по врагу (опционально)
    public float decalLifetime = 10f; // Время жизни декали

    // Параметры холодного оружия
    [Header("Холодное оружие")]
    public float meleeRange = 2f; // Дальность атаки
    public float meleeAttackDuration = 0.5f; // Длительность анимации удара
    public LayerMask meleeHitLayers; // Какие слои может поражать холодное оружие
    public AnimationClip meleeAttackAnimation; // Анимация удара

    [Header("Звуки")]
    public AudioClip fireSound;
    public AudioClip reloadSound;
    public AudioClip meleeSwingSound;
    public AudioClip meleeHitSound;

    private void OnValidate()
    {
        // Проверка корректности параметров для огнестрельного оружия
        if (type == WeaponType.Firearm)
        {
            if (projectilePrefab == null)
                Debug.LogWarning($"[{name}] Огнестрельное оружие должно иметь префаб снаряда!");

            if (maxMagazineCapacity <= 0)
                maxMagazineCapacity = 1;

            if (fireRate < 0)
                fireRate = 0.1f;
        }

        // Проверка корректности параметров для холодного оружия
        if (type == WeaponType.Melee)
        {
            if (meleeRange <= 0)
                meleeRange = 1f;

            if (meleeAttackDuration <= 0)
                meleeAttackDuration = 0.5f;
        }

        // Общие проверки
        if (damage <= 0)
            damage = 1f;
    }

    // --- Реализация интерфейса для новой системы статов ---
    public IEnumerable<ModifierData> GetModifiers()
    {
        var modifiers = new List<ModifierData>();

        // Превращаем старые поля GunInfo в новые Модификаторы
        
        // 1. Урон
        modifiers.Add(new ModifierData {
            statType = StatType.Damage,
            modifierType = ModifierType.Flat,
            value = damage
        });

        // 2. Скорострельность
        modifiers.Add(new ModifierData {
            statType = StatType.FireRate,
            modifierType = ModifierType.Flat,
            value = fireRate
        });

        // 3. Магазин (если есть такой тип статы)
        // modifiers.Add(new ModifierData { ... value = maxMagazineCapacity ... });

        return modifiers;
    }
}