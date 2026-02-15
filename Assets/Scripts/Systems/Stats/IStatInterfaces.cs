using System.Collections.Generic;

namespace Systems.Stats
{
    // Интерфейс для одной конкретной характеристики (например, Health)
    public interface IStat
    {
        StatType Type { get; }
        float BaseValue { get; }
        float Value { get; } // Итоговое значение после всех модификаторов
        
        void AddModifier(IStatModifier modifier);
        void RemoveModifier(IStatModifier modifier);
    }

    // Интерфейс модификатора (то, что лежит внутри Патча или Аффикса)
    public interface IStatModifier
    {
        float Value { get; }
        ModifierType Type { get; }
        object Source { get; } // Кто наложил (Патч, Оружие, Бафф)
    }

    // Интерфейс для любой сущности, имеющей статы (Игрок, Враг)
    public interface IStatsProvider
    {
        float GetStatValue(StatType type);
        IStat GetStat(StatType type);
    }

    // Интерфейс для предмета, который переносит модификаторы (Патч, Оружие)
    public interface IStatModifierSource
    {
        // Возвращает список модификаторов, которые этот предмет дает владельцу
        IEnumerable<ModifierData> GetModifiers();
    }
    
    // Простая структура данных для настройки в Инспекторе
    [System.Serializable]
    public struct ModifierData
    {
        public StatType statType;
        public ModifierType modifierType;
        public float value;
    }
}