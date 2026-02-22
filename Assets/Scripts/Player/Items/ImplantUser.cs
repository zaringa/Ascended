using UnityEngine;
using Player.Items.Implants.Base;

namespace Player.Items
{
    /// <summary>
    /// Компонент для использования экипированных имплантов.
    /// Вешается на игрока.
    /// </summary>
    public class ImplantUser : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Player.Inventory.Inventory inventory;
        
        [Header("Key Bindings")]
        [SerializeField] private KeyCode armsImplantKey = KeyCode.Q;
        [SerializeField] private KeyCode bodyImplantKey = KeyCode.E;
        [SerializeField] private KeyCode legsImplantKey = KeyCode.Space;
        
        void Start()
        {
            if (inventory == null)
            {
                inventory = GetComponent<Player.Inventory.Inventory>();
            }
        }
        
        void Update()
        {
            // Проверяем нажатия клавиш для имплантами
            if (Input.GetKeyDown(armsImplantKey))
            {
                UseArmsImplant();
            }
            
            if (Input.GetKeyDown(bodyImplantKey))
            {
                UseBodyImplant();
            }
            
            if (Input.GetKeyDown(legsImplantKey))
            {
                UseLegsImplant();
            }
        }
        
        private void UseArmsImplant()
        {
            var implant = inventory.GetArmsImplant();
            if (implant != null)
            {
                Debug.Log($"Using arms implant: {implant.name}");
                implant.Action();
            }
            else
            {
                Debug.Log("No arms implant equipped");
            }
        }
        
        private void UseBodyImplant()
        {
            var implant = inventory.GetBodyImplant();
            if (implant != null)
            {
                Debug.Log($"Using body implant: {implant.name}");
                implant.Action();
            }
            else
            {
                Debug.Log("No body implant equipped");
            }
        }
        
        private void UseLegsImplant()
        {
            var implant = inventory.GetLegsImplant();
            if (implant != null)
            {
                Debug.Log($"Using legs implant: {implant.name}");
                implant.Action();
            }
            else
            {
                Debug.Log("No legs implant equipped");
            }
        }
    }
}
