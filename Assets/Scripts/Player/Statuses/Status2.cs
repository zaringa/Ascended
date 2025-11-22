using UnityEngine;

namespace Player.Effects
{
    class Status2 : StatusEffect
    {
        [Header("Коэффициенты применяемых эффектов")]
        [SerializeField] private float fireRateMultiplier = 1;
        [SerializeField] private float runSpeedMultiplier = 1;
        [SerializeField] private float reloadingSpeedMultiplier = 1;
        
        private Effects[] applyingEffects = {
            Effects.RunSpeedMultiplier,
            Effects.PrimaryWeaponFireRateMultiplier,
            Effects.SecondaryWeaponFireRateMultiplier,
            Effects.PrimaryWeaponReloadingSpeedMultiplier,
            Effects.SecondaryWeaponReloadingSpeedMultiplier };

        public Status2(float duration) : base(duration, true, true) { }

        public override void ApplyEffect(Effect effects)
        {
            // Увеличение скорости стрельбы основного и вспомагательного орудий
            // Увеличение скорости передвижения
            // Уменьшение времени перезарядки

            //DictionaryEffect t;
            //foreach (var effect in applyingEffects)
            //{
            //    t = effects.effects.Find(e => e.Effect == effect);
            //    switch (t.Effect)
            //    {
            //        case Effects.PrimaryWeaponFireRateMultiplier:
            //            t = new(Effects.PrimaryWeaponFireRateMultiplier, ref effects.effects.Find(e => e.Effect == effect).Value);
            //            break;
            //    }
            //}
        }

        public override void RemoveEffect(Effect effects)
        {
            // Уменьшение скорости стрельбы основного и вспомагательного орудий
            // Уменьшение скорости передвижения
            // Увеличение времени перезарядки
            effects.RecalculateStats();
        }

        public override void UpdateEffect(float deltaTime)
        {
            float t = fireRateMultiplier * duration / timeReamining;
            base.UpdateEffect(deltaTime);
            fireRateMultiplier = t / duration * timeReamining;
        }
    }
}