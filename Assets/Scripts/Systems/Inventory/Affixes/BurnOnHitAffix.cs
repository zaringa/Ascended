using Systems.Inventory.Affixes;
using UnityEngine;

[CreateAssetMenu(fileName = "BurnOnHitAffix", menuName = "Inventory/Affix/BurnOnHit")]
public class BurnOnHitAffix : ScriptableObject, IOnHitAffix
{
    [Header("Burn on Hit")]
    public int burnStacks = 3;
    public float burnDamagePerSecond = 5f;
    public float chanceToApply = 0.3f;
    
    public void OnHit(object target, ref float damage)
    {
        if(Random.value <= chanceToApply)
        {
            // Применить эффект горения к цели
            // В реальной реализации здесь будет вызов метода для применения статуса
            Debug.Log($"Applied burn effect to target with {burnStacks} stacks");
        }
    }
    
    public void SubscribeToEvents()
    {
        // Подписаться на событие попадания
        // WeaponEventManager.OnHit += OnHit;
    }
    
    public void UnsubscribeFromEvents()
    {
        // Отписаться от события попадания
        // WeaponEventManager.OnHit -= OnHit;
    }
}