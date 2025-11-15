using Player.Items.Implants.Base;
using Player.Items.Implants.Interfaces;
using UnityEngine;

namespace Player.Items.Implants.Impl
{
    public class DamageBoostImplant : SmallImplant, IDamageModifier
    {
        [SerializeField] private float baseBonus = 0.10f;
        [SerializeField] private float perStack = 0.05f;
        private int stack = 1;

        public float ModifyDamage(float damage)
        {
            float bonus = baseBonus + perStack * (stack - 1);

            return damage * (1f + bonus);
        }

        public override void Action() {}
    }
}
