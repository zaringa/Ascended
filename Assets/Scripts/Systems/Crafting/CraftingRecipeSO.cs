using UnityEngine;

namespace Systems.Crafting
{
    /// <summary>
    /// Рецепт крафта.
    /// </summary>
    [CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/Recipe")]
    public class CraftingRecipeSO : ScriptableObject
    {
        /// <summary>
        /// Результат крафта (аптечка, граната, etc.)
        /// </summary>
        [Header("Result")]
        public Player.Items.Item OutputItem => outputItem;
        [SerializeField] private Player.Items.Item outputItem;
        /// <summary>
        /// Количество создаваемых предметов.
        /// </summary>
        public uint OutputAmount => outputAmount;
        [SerializeField] private uint outputAmount = 1;

        /// <summary>
        /// Цена в "особом ресурсе".
        /// </summary>
        [Header("Cost")]
        public uint ResourceCost => resourceCost;
        [SerializeField] private uint resourceCost = 1;
        /// <summary>
        /// Время крафта в секундах.
        /// </summary>
        public float CraftTime => craftTime;
        [SerializeField] private float craftTime = 1f;
    }
}