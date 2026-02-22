using UnityEngine;

namespace Systems.Inventory.Managers
{
    public class WeaponManager : MonoBehaviour
    {
        private Player.Inventory.Inventory inventory;
        private global::BaseWeapon currentWeapon;
        
        void Start()
        {
            inventory = GetComponent<Player.Inventory.Inventory>();
        }
        
        public void EquipWeapon(GunInfo weapon, bool isPrimary)
        {
            // Реализация экипировки оружия
            // Подключение к системе стрельбы и т.д.
            
            // Обновляем аффиксы оружия
            UpdateWeaponAffixes(weapon);
            
            Debug.Log($"Equipped {(isPrimary ? "primary" : "secondary")} weapon: {weapon.name}");
        }
        
        private void UpdateWeaponAffixes(GunInfo weapon)
        {
            // В реальной реализации здесь будет обработка аффиксов оружия
            // и подписка условных аффиксов на события
            Debug.Log($"Updated affixes for weapon: {weapon.name}");
        }
        
        public void UnequipWeapon(bool isPrimary)
        {
            Debug.Log($"Unequipped {(isPrimary ? "primary" : "secondary")} weapon");
        }
        
        public void SwitchWeapon(bool isPrimary)
        {
            // Переключение между основным и второстепенным оружием
            Debug.Log($"Switched to {(isPrimary ? "primary" : "secondary")} weapon");
        }
    }
}