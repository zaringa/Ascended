using System.Collections.Generic;
using Player.Items;
using Systems.Stats;
using UnityEngine;

namespace Systems.Inventory.Items
{
    // Определение оружия (База)
    [CreateAssetMenu(fileName = "New Weapon Def", menuName = "Inventory/Items/Weapon Definition")]
    public class WeaponItemDefSO : RenderableItem // Наследуемся от Renderable, т.к. у оружия есть префаб
    {
        [Header("Base Weapon Stats")]
        [SerializeField] private float baseDamage;
        [SerializeField] private float fireRate;
        [SerializeField] private int ammoCapacity;
        
        [Header("Tags")]
        public List<string> weaponTags; // Heavy, Melee, etc.

        // Метод для получения базовых статов оружия
        public List<ModifierData> GetBaseStats()
        {
            return new List<ModifierData>
            {
                new ModifierData { statType = StatType.Damage, value = baseDamage, modifierType = ModifierType.Flat },
                new ModifierData { statType = StatType.FireRate, value = fireRate, modifierType = ModifierType.Flat },
                new ModifierData { statType = StatType.MaxHealth, value = 0, modifierType = ModifierType.Flat } // Пример заглушки
            };
        }
    }
}