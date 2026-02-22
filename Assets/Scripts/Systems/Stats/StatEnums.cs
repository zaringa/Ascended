namespace Systems.Stats
{
    /// <summary>
    /// Типы характеристик.
    /// </summary>
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

    /// <summary>
    /// Тип модификатора характеристики.
    /// </summary>
    /// <remarks> Определяет характер влияния модификатора на характеристику. </remarks>
    /// <example>
    /// <code>
    /// result += 0.2f; // Flat
    /// result *= (1 + 0.2)f; // PercentAdd
    /// result *= 0.2f; // PercentMult
    /// </code>
    /// </example>
    public enum ModifierType
    {
        Flat = 100,
        PercentAdd = 200,
        PercentMult = 300
    }
}