using UnityEngine;

namespace Systems.Inventory.Demo
{
    public class InventoryDemo : MonoBehaviour
    {
        [Header("Demo Components")]
        [SerializeField] private Player.Inventory.Inventory playerInventory;
        
        void Start()
        {
            if (playerInventory == null)
            {
                playerInventory = FindFirstObjectByType<Player.Inventory.Inventory>();
            }
            
            if (playerInventory != null)
            {
                SetupDemoInventory();
            }
            else
            {
                Debug.LogError("Player Inventory not found! Attach this script to a GameObject with Player.Inventory.Inventory component.");
            }
        }
        
        private void SetupDemoInventory()
        {
            Debug.Log("Setting up demo inventory...");
            
            // В реальной игре предметы будут создаваться из префабов или ресурсов
            // Для демонстрации просто логгируем, что система работает
            Debug.Log("Demo inventory setup complete!");
            Debug.Log($"General inventory size: {playerInventory.GetGeneralInventory().Count}");
            Debug.Log("Note: In a real game, items would be loaded from resources/prefabs, not created at runtime.");
        }
    }
}