using System;

namespace Systems.Inventory.Affixes
{
    [Flags]
    public enum WeaponTag
    {
        None = 0,
        Heavy = 1 << 0,      // Тяжёлое
        Melee = 1 << 1,      // Ближнее
        Paired = 1 << 2,     // Парное
        Explosive = 1 << 3,  // Взрывное
        // Добавить другие теги по мере необходимости
    }
}