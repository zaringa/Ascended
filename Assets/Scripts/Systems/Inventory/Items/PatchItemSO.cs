using System.Collections.Generic;
using Player.Items; // Используем существующий namespace проекта
using Systems.Stats;
using UnityEngine;

namespace Systems.Inventory.Items
{
    // Обычный патч (просто статы)
    [CreateAssetMenu(fileName = "New Patch", menuName = "Inventory/Items/Patch")]
    public class PatchItemSO : Item, IStatModifierSource
    {
        [Header("Patch Specifics")]
        public bool isRare; // Можно заменить на Enum Rarity позже
        
        [Tooltip("Список статов, которые дает этот патч")]
        [SerializeField] private List<ModifierData> _modifiers;

        // Реализация интерфейса: отдаем "сырые" данные о модификаторах
        public IEnumerable<ModifierData> GetModifiers()
        {
            return _modifiers;
        }
    }
}