using UnityEngine;
using Player.Items;

namespace Systems.Inventory.Managers
{
    public class ConsumableManager : MonoBehaviour
    {
        private Player.Inventory.Inventory inventory;
        
        void Start()
        {
            inventory = GetComponent<Player.Inventory.Inventory>();
        }
        
        // Метод для активации расходника из слота
        public bool UseConsumable(int slotIndex)
        {
            if (inventory == null || slotIndex < 0 || slotIndex >= inventory.GetCraftedConsumables().Length)
                return false;
                
            var craftedConsumables = inventory.GetCraftedConsumables();
            if (slotIndex >= craftedConsumables.Length)
                return false;
                
            var consumable = craftedConsumables[slotIndex];
            if (consumable == null)
                return false;
                
            consumable.Use();
            craftedConsumables[slotIndex] = null; // Удаляем использованный расходник
            
            return true;
        }
        
        // Метод для подготовки расходника к крафту
        public bool PrepareCrafting(Systems.Crafting.CraftingRecipeSO recipe, int slotIndex)
        {
            if (recipe == null || slotIndex < 0 || slotIndex >= 3)
                return false;
                
            // Проверяем, свободен ли слот
            var craftedConsumables = inventory.GetCraftedConsumables();
            if (craftedConsumables[slotIndex] != null)
                return false; // Слот занят
            
            // Здесь должна быть реализация системы крафта
            // с ограничениями на одновременное крафтование и т.д.
            
            // Для демонстрации просто добавляем результат крафта в слот
            if (recipe.outputItem is IConsumable consumableOutput)
            {
                craftedConsumables[slotIndex] = consumableOutput;
                return true;
            }
            else
            {
                Debug.LogError($"Recipe output item is not compatible with IConsumable: {recipe.outputItem.name}");
                return false;
            }
        }
    }
}