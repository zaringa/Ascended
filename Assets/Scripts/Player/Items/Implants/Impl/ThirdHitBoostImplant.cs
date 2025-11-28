using Player.Items.Implants.Base;
using Player.Items.Implants.Interfaces;
using UnityEngine;

namespace Player.Items.Implants.Impl
{
    public class ThirdHitBoostImplant : SmallImplant, IAttackCounter
    {
        private int hitCount = 0;

        [SerializeField] private float baseBonus = 0.10f;
        [SerializeField] private float perStack = 0.05f;
        private int stack = 1;

        public float OnAttack(float damage)
        {
            hitCount++;

            if (hitCount >= 3)
            {
                hitCount = 0;

                float bonus = baseBonus + perStack * (stack - 1);
                return damage * (1f + bonus);
            }

            return damage;
        }

        public override void Action() {}
    }
}
