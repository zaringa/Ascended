using Player.Items.Implants.Base;
using Player.Items.Implants.Interfaces;
using UnityEngine;
using Systems.Stats;
using System.Collections.Generic;

namespace Player.Items.Implants.Impl
{
    /// <summary>
    /// Редкий имплант - каждый 3-й выстрел с бонусом урона.
    /// Устанавливается в слот для малых имплантов (Small).
    /// </summary>
    [CreateAssetMenu(fileName = "ThirdHitBoostImplant", menuName = "Inventory/Implants/Rare/ThirdHitBoost")]
    public class ThirdHitBoostImplant : SmallImplant, IConditionalImplant, IOnAttackImplant
    {
        [Header("Third Hit Boost")]
        public float bonusDamagePercent = 0.2f;
        
        private int hitCount = 0;
        
        public float OnAttack(float baseDamage)
        {
            hitCount++;
            
            if (hitCount >= 3)
            {
                hitCount = 0;
                return baseDamage * (1f + bonusDamagePercent);
            }
            
            return baseDamage;
        }
        
        public void SubscribeToEvents()
        {
            // Подписываемся на событие атаки
            // PlayerCombatManager.OnAttack += OnAttack;
        }
        
        public void UnsubscribeFromEvents()
        {
            // Отписываемся от события атаки
            // PlayerCombatManager.OnAttack -= OnAttack;
        }
        
        public override void Action()
        {
            // Нет активного действия для этого импланта
        }
    }
}
