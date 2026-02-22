using System.Collections.Generic;
using Systems.Stats;
using UnityEngine;

namespace Systems.Inventory.Items
{
    /// <summary>
    /// Содержащий статы патч.
    /// </summary>
    [CreateAssetMenu(fileName = "New Patch", menuName = "Inventory/Items/Patch")]
    public class PatchItemSO : Player.Items.Item, IStatModifierSource
    {
        /// <summary>
        /// Редкость патча.
        /// </summary>
        [Header("Patch Specifics")]
        public PatchRarity IsRare => rarity;
        [SerializeField] private PatchRarity rarity = PatchRarity.Ubiquitous;

        /// <summary>
        /// Список модификаторов патча.
        /// </summary>
        [Tooltip("Список статов, которые дает этот патч")]
        public IEnumerable<ModifierData> Modifiers => modifiers;
        [SerializeField] private List<ModifierData> modifiers;

        public IEnumerable<ModifierData> GetModifiers() => Modifiers;
    }

    /// <summary>
    /// Редкость патчей.
    /// </summary>
    /// <remarks> <see langword="TODO"/>: Изменить при необходимости. </remarks>
    public enum PatchRarity
    {
        /// <summary> Вездесущий. </summary>
        Ubiquitous,
        /// <summary> Обычный. </summary>
        Common,
        /// <summary> Необычный. </summary>
        Uncommon,
        /// <summary> Редкий. </summary>
        Rare,
        /// <summary> Исключительный. </summary>
        Exceptional,
        /// <summary> Легендарный. </summary>
        Legendary
    }
}