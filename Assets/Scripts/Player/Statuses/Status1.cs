using UnityEngine;

namespace Player.Effects
{
    class Status1 : StatusEffect
    {
        [Header("Коэффициенты применяемых эффектов")]
        [SerializeField] private float craftSpeedMultiplier = 1;
        [SerializeField] private float damageResistMultiplier = 1;

        public Status1(float duration) : base(duration, false, false) { }

        public override void ApplyEffect(Effect effects)
        {
            // Увеличение скорости крафта предметов
            // Увеличение сопротивляемости входящему урону
        }

        public override void RemoveEffect(Effect effects)
        {
            // Уменьшение скорости крафта предметов
            // Уменьшение сопротивляемости входящему урону
            effects.RecalculateStats();
        }
    }
}