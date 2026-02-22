using Player.Items;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthPotion", menuName = "Inventory/Items/Consumables/HealthPotion")]
public class HealthPotion : Item, IConsumable
{
    [Header("Health Potion Properties")]
    public float healAmount = 25f;
    
    public void Use()
    {
        // В реальной реализации здесь будет восстановление здоровья игрока
        Debug.Log($"Used Health Potion, healing {healAmount} HP");
        
        // Пример вызова восстановления здоровья
        // PlayerHealth.Instance.Heal(healAmount);
    }
}