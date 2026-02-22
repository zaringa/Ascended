/// <summary>
/// Позволяет получать урон.
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Минимальное здоровье.
    /// </summary>
    public const float MinHealth = 0;

    /// <summary>
    /// Текущее здоровье.
    /// </summary>
    public float CurrentHealth { get; protected set; }

    /// <summary>
    /// Максимальное здоровье.
    /// </summary>
    public float MaxHealth { get; protected set; }

    /// <summary>
    /// Повреждает на абсолютное значение.
    /// </summary>
    /// <param name="damageAmount">Абсолютное количество наносимого урона в диапазоне (0; +INF).</param>
    /// <param name="source">Источник урона.</param>
    public void ApplyDamageAbsolute(float damageAmount, IDamaging source);

    /// <summary>
    /// Повреждает на относительное значение.
    /// </summary>
    /// <param name="damageFraction">Относительное количество наносимого урона в диапазоне (0; 1].</param>
    /// <param name="source">Источник урона.</param>
    public void ApplyDamageRelative(float damageFraction, IDamaging source);

    /// <summary>
    /// Лечит на абсолютное значение.
    /// </summary>
    /// <param name="healAmount">Абсолютное значение лечения в диапазоне (0; +INF).</param>
    /// <param name="source">Источник лечения.</param>
    public void ApplyHealAbsolute(float healAmount, IDamaging source);

    /// <summary>
    /// Лечит на относительное значение.
    /// </summary>
    /// <param name="healFraction">Относительное количество получаемого лечения в диапазоне (0; 1].</param>
    /// <param name="source">Источник лечения.</param>
    /// <example></example>
    public void ApplyHealRelative(float healFraction, IDamaging source);

    /// <summary>
    /// Устанавливает текущее значение здоровья в конкретное значение.
    /// </summary>
    /// <param name="healthAmount">Новое абсолютное значение здоровья в диапазоне (0; +INF).</param>
    /// <param name="source">Источник урона</param>
    public void SetHealthAbsolute(float healthAmount, IDamaging source);

    /// <summary>
    /// Устанавливает текущее значение здоровья в указанное
    /// </summary>
    /// <param name="healthFraction">Новое относительное значение здоровья в диапазоне (0; 1].</param>
    /// <param name="source">Источник урона</param>
    public void SetHealthRelative(float healthFraction, IDamaging source);
}