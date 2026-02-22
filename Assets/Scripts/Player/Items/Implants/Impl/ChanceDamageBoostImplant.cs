using Player.Items.Implants.Base;
using Player.Items.Implants.Interfaces;
using UnityEngine;
using Enemy;

namespace Player.Items.Implants.Impl
{
    /// <summary>
    /// Редкий имплант - шанс на дополнительный урон при попадании.
    /// Устанавливается в слот для малых имплантов (Small).
    /// </summary>
    [CreateAssetMenu(fileName = "ChanceDamageBoostImplant", menuName = "Inventory/Implants/Rare/ChanceDamageBoost")]
    public class ChanceDamageBoostImplant : SmallImplant, IConditionalImplant, IOnHitImplant
    {
        [Header("Chance Damage Boost")]
        public float chance = 0.15f;
        public float bonusDamagePercent = 0.5f;

        public void OnHit(IEnemy target, ref float damage)
        {
            if(UnityEngine.Random.value <= chance)
            {
                damage *= (1f + bonusDamagePercent);
                Debug.Log($"Chance damage boost triggered! Additional {bonusDamagePercent * 100}% damage dealt.");
            }
        }

        public void SubscribeToEvents()
        {
            // Подписываемся на событие попадания
            // PlayerCombatManager.OnHit += OnHit;
        }

        public void UnsubscribeFromEvents()
        {
            // Отписываемся от события попадания
            // PlayerCombatManager.OnHit -= OnHit;
        }

        public override void Action()
        {
            // Нет активного действия для этого импланта
        }
    }
}
