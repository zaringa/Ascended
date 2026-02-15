using UnityEngine;
using Player.Items;

namespace Systems.Crafting
{
    [CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/Recipe")]
    public class CraftingRecipeSO : ScriptableObject
    {
        [Header("Result")]
        public Item outputItem;     // Что получится (Граната, Аптечка)
        public int outputAmount = 1;

        [Header("Cost")]
        public int resourceCost;    // Цена в "особом ресурсе"
        public float craftTime;     // Время крафта в секундах
    }
}