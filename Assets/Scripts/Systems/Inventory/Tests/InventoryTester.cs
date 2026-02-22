using UnityEngine;

namespace Systems.Inventory.Tests
{
    public class InventoryTester : MonoBehaviour
    {
        [Header("Test Components")]
        [SerializeField] private Player.Inventory.Inventory inventory;
        
        void Start()
        {
            if (inventory == null)
            {
                inventory = FindObjectOfType<Player.Inventory.Inventory>();
            }
            
            if (inventory != null)
            {
                RunTests();
            }
            else
            {
                Debug.LogError("Inventory not found for testing!");
            }
        }
        
        private void RunTests()
        {
            Debug.Log("=== Starting Inventory System Tests ===");
            
            // В реальной игре тесты будут использовать настоящие ассеты
            // Для демонстрации просто проверим, что инвентарь существует
            bool inventoryExists = inventory != null;
            Debug.Log($"Test 1 - Inventory exists: {inventoryExists} (Expected: True)");
            
            // Проверим количество предметов в инвентаре
            int inventorySize = inventory.GetGeneralInventory().Count;
            Debug.Log($"Test 2 - Inventory size: {inventorySize} (Expected: Non-zero if items were added elsewhere)");
            
            Debug.Log("=== Inventory System Tests Completed ===");
            Debug.Log("Note: In a real game, tests would use actual asset references, not runtime creation.");
        }
    }
}