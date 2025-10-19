using UnityEngine;

// 

[CreateAssetMenu(fileName = "NewGunInfo", menuName = "Game/Gun Info")]
public class GunInfo : ScriptableObject
{
    [Header("Идентификация")]
    public string gunName = "Default Weapon";
    public Sprite weaponSprite;
    [TextArea(3, 5)]
    public string description = "Standard issue weapon.";
    
    // Перечисление для типизации оружия
    public enum WeaponType { Melee, Firearm };
    public WeaponType type = WeaponType.Firearm;

    [Header("Визуализация")]
    public GameObject weaponModelPrefab; // 3D-модель (Префаб)

    [Header("Характеристики")]
    public float damage = 10f;
    public int maxMagazineCapacity = 10;
    public float fireRate = 0.1f;
    public float reloadTime = 2.0f;
}