using UnityEngine;

namespace Player.Effects
{
    class Status3 : StatusEffect
    {
        [Header("Коэффициенты применяемых эффектов")]
        [SerializeField] private float fireRateMultiplier = 1;
        [SerializeField] private float runSpeedMultiplier = 1;
        [SerializeField] private float reloadingSpeedMultiplier = 1;

        [Header("Кривая, по которой параметры уменьшаются со временем")]
        [SerializeField] private AnimationCurve decreasingPerTick = new AnimationCurve(new(0, 0), new(1, 1));
        private float tickRate = 1;
        private float timer;

        public Status3(float duration) : base(duration, true, true) =>
            timer = tickRate;

        public override void ApplyEffect(Effect effects) { }

        public override void RemoveEffect(Effect effects) =>
            effects.RecalculateStats(); 

        public override void UpdateEffect(float deltaTime)
        {
            base.UpdateEffect(deltaTime);
            timer -= deltaTime;
            if (timer < 0)
                timer = tickRate;

            // Нанесение урона с течением времени
        }
    }
}