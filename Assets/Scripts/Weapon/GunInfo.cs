using Player.Items;
using UnityEngine;
using Systems.Stats;
using System.Collections.Generic;
using Systems.Inventory.Affixes;

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
    
    [Header("Накладываемые характеристики")]
    public WeaponRarity rarity = WeaponRarity.Standard;
    public WeaponTag tags;
    public List<AffixInstance> affixes = new List<AffixInstance>();

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
        modifiers.Add(new ModifierData {
            statType = StatType.Damage,
            modifierType = ModifierType.Flat,
            value = damage
        });

        modifiers.Add(new ModifierData {
            statType = StatType.FireRate,
            modifierType = ModifierType.Flat,
            value = fireRate
        });

        // Модификаторы от аффиксов
        foreach(var affixInstance in affixes)
        {
            if(affixInstance.isEquipped && affixInstance.affix != null)
            {
                var affixModifiers = affixInstance.affix.GetModifiers();
                foreach(var modifier in affixModifiers)
                {
                    modifiers.Add(modifier);
                }
            }
        }

        return modifiers;
    }
    
    // Методы для работы с аффиксами
    public bool CanAddAffix(AffixSO affix)
    {
        // Проверяем, соответствует ли редкость аффикса редкости оружия
        if(affix.requiredRarity > this.rarity)
            return false;
            
        // Проверяем количество доступных слотов
        int staticSlots = GetAvailableStaticSlots();
        int conditionalSlots = GetAvailableConditionalSlots();
        
        if(affix.affixType == AffixType.Static)
            return GetEquippedStaticAffixesCount() < staticSlots;
        else
            return GetEquippedConditionalAffixesCount() < conditionalSlots;
    }
    
    public bool AddAffix(AffixSO affix)
    {
        if(!CanAddAffix(affix))
            return false;
            
        affixes.Add(new AffixInstance { affix = affix, isEquipped = false });
        return true;
    }
    
    public bool EquipAffix(AffixSO affix)
    {
        for(int i = 0; i < affixes.Count; i++)
        {
            if(affixes[i].affix == affix && !affixes[i].isEquipped)
            {
                // Проверяем, есть ли место для экипировки
                if(affix.affixType == AffixType.Static && GetEquippedStaticAffixesCount() < GetAvailableStaticSlots())
                {
                    affixes[i].isEquipped = true;
                    return true;
                }
                else if(affix.affixType == AffixType.Conditional && GetEquippedConditionalAffixesCount() < GetAvailableConditionalSlots())
                {
                    affixes[i].isEquipped = true;
                    return true;
                }
            }
        }
        return false;
    }
    
    public bool UnequipAffix(AffixSO affix)
    {
        for(int i = 0; i < affixes.Count; i++)
        {
            if(affixes[i].affix == affix && affixes[i].isEquipped)
            {
                affixes[i].isEquipped = false;
                return true;
            }
        }
        return false;
    }
    
    private int GetAvailableStaticSlots()
    {
        switch(rarity)
        {
            case WeaponRarity.Uncommon:
            case WeaponRarity.Rare:
                return 1;
            case WeaponRarity.Epic:
                return 2;
            case WeaponRarity.Legendary:
                return 2;
            default:
                return 0;
        }
    }
    
    private int GetAvailableConditionalSlots()
    {
        switch(rarity)
        {
            case WeaponRarity.Rare:
                return 1;
            case WeaponRarity.Epic:
                return 1;
            case WeaponRarity.Legendary:
                return 2;
            default:
                return 0;
        }
    }
    
    private int GetEquippedStaticAffixesCount()
    {
        int count = 0;
        foreach(var affixInstance in affixes)
        {
            if(affixInstance.isEquipped && affixInstance.affix != null && affixInstance.affix.affixType == AffixType.Static)
                count++;
        }
        return count;
    }
    
    private int GetEquippedConditionalAffixesCount()
    {
        int count = 0;
        foreach(var affixInstance in affixes)
        {
            if(affixInstance.isEquipped && affixInstance.affix != null && affixInstance.affix.affixType == AffixType.Conditional)
                count++;
        }
        return count;
    }
}