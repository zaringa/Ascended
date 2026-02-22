using System;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Stats
{
    [Serializable]
    public class Stat : IStat
    {
        [SerializeField] private StatType _type;
        [SerializeField] private float _baseValue;

        // "Грязный" флаг оптимизации. Пересчитываем Value только если что-то изменилось.
        private bool _isDirty = true;
        private float _cachedValue;

        // Список всех модификаторов на этой стате
        private readonly List<IStatModifier> _modifiers = new List<IStatModifier>();

        public StatType Type => _type;
        public float BaseValue => _baseValue;

        public float Value
        {
            get
            {
                if (_isDirty)
                {
                    _cachedValue = CalculateFinalValue();
                    _isDirty = false;
                }
                return _cachedValue;
            }
        }

        public Stat(StatType type, float baseValue)
        {
            _type = type;
            _baseValue = baseValue;
        }

        public void AddModifier(IStatModifier modifier)
        {
            _isDirty = true;
            _modifiers.Add(modifier);
        }

        public void RemoveModifier(IStatModifier modifier)
        {
            _isDirty = true;
            _modifiers.Remove(modifier);
        }

        // Метод для удаления всех модификаторов от конкретного источника (например, сняли штаны)
        public void RemoveAllModifiersFromSource(object source)
        {
            bool removed = false;
            for (int i = _modifiers.Count - 1; i >= 0; i--)
            {
                if (_modifiers[i].Source == source)
                {
                    _modifiers.RemoveAt(i);
                    removed = true;
                }
            }
            if (removed) _isDirty = true;
        }

        private float CalculateFinalValue()
        {
            float finalValue = BaseValue;
            float sumPercentAdd = 0;

            // Сортировка не обязательна, если мы просто пробежимся по типам
            // 1. Сначала Flat
            for (int i = 0; i < _modifiers.Count; i++)
                if (_modifiers[i].Type == ModifierType.Flat)
                    finalValue += _modifiers[i].Value;

            // 2. Потом Percent Add (складываем проценты: +10% и +10% = +20%)
            for (int i = 0; i < _modifiers.Count; i++)
                if (_modifiers[i].Type == ModifierType.PercentAdd)
                    sumPercentAdd += _modifiers[i].Value;

            // Применяем Percent Add
            finalValue *= 1 + sumPercentAdd;

            // 3. Percent Mult (редкие мультипликативные бонусы)
            for (int i = 0; i < _modifiers.Count; i++)
                if (_modifiers[i].Type == ModifierType.PercentMult)
                    finalValue *= (1 + _modifiers[i].Value);

            return (float)Math.Round(finalValue, 4); // Округляем, чтобы убрать шум float
        }
    }

    // Реализация модификатора
    public class StatModifier : IStatModifier
    {
        public float Value { get; }
        public ModifierType Type { get; }
        public object Source { get; }

        public StatModifier(float value, ModifierType type, object source)
        {
            Value = value;
            Type = type;
            Source = source;
        }
    }
}