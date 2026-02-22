using Enemy;
using Player.Items.Implants.Base;
using Player.Items.Implants.Interfaces;
using UnityEngine;

namespace Player.Items.Implants.Impl
{
    /// <summary>
    /// Редкий имплант - шанс вызвать взрыв при попадании.
    /// Устанавливается в слот для малых имплантов (Small).
    /// </summary>
    [CreateAssetMenu(fileName = "ExplosionImplant", menuName = "Inventory/Implants/Rare/Explosion")]
    public class ExplosionImplant : SmallImplant, IConditionalImplant, IOnHit
    {
        [Header("Explosion")]
        public float baseChance = 0.10f;
        public float baseExplosionDamage = 0.20f;
        public float perStackDamage = 0.10f;
        
        private int stack = 1;

        public void OnHit(IEnemy target, ref float damage)
        {
            if (Random.value <= baseChance)
            {
                float explosionDmg = baseExplosionDamage + perStackDamage * (stack - 1);
                float finalDmg = damage * explosionDmg;

                // Вызвать взрыв
                target.ApplyExplosionDamage(finalDmg);
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

        public override void Action() {}
    }
}
