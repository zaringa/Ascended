using UnityEngine;

[CreateAssetMenu(fileName = "NewGunInfo", menuName = "Game/Gun Info")]
public class GunInfo : RenderableItem
{
    // Перечисление для типизации оружия
    public enum WeaponType { Melee, Firearm }
    public WeaponType type = WeaponType.Firearm;

    [Header("Базовые характеристики")]
    public float damage = 10f;

    [Header("Огнестрельное оружие")]
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
}