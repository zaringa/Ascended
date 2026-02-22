using UnityEngine;
using Player.Inventory;
using Player.Items.Implants.Movement;

namespace Player.Items
{
    /// <summary>
    /// Тестовый скрипт для экипировки имплантов в начале игры.
    /// </summary>
    public class ImplantEquipTester : MonoBehaviour
    {
        [SerializeField] private Player.Inventory.Inventory inventory;
        [SerializeField] private DashImplant dashImplant;
        
        void Start()
        {
            if (inventory == null)
            {
                inventory = GetComponent<Player.Inventory.Inventory>();
            }
            
            if (inventory != null && dashImplant != null)
            {
                bool result = inventory.EquipImplant(dashImplant);
                Debug.Log($"Equip result: {result}");
            }
        }
    }
}
