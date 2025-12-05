using Enemy;
using Player.Items.Implants.Base;
using Player.Items.Implants.Interfaces;
using UnityEngine;

namespace Player.Items.Implants.Impl
{
    public class ExplosionImplant : SmallImplant, IOnHit
    {
        [SerializeField] private float baseChance = 0.10f;
        [SerializeField] private float baseExplosionDamage = 0.20f;
        [SerializeField] private float perStackDamage = 0.10f;
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

        public override void Action() {}
    }
}
