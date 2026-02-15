namespace Systems.Stats
{
    // Типы характеристик. Можно расширять.
    public enum StatType
    {
        Health,
        MaxHealth,
        Damage,
        Speed,
        FireRate,
        CritChance,
        CritDamage,
        // Добавь другие по мере необходимости
    }

    // Как модификатор влияет на формулу:
    // Flat: 10 + 5 = 15
    // PercentAdd: 10 * (1 + 0.1) = 11
    // PercentMult: (итог) * 1.1
    public enum ModifierType
    {
        Flat = 100,
        PercentAdd = 200,
        PercentMult = 300
    }
}