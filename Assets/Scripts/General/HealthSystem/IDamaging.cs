/// <summary>
/// Позволяет наносить урон.
/// </summary>
public interface IDamaging
{
    /// <summary>
    /// Наносит абсолютный урон цели.
    /// </summary>
    /// <param name="damageAmount">Абсолютное количество наносимого урона в диапазоне (0; +INF).</param>
    /// <param name="target">Цель, которой наносится урон.</param>
    public void InflictAbsoluteDamage(float damageAmount, IDamageable target);

    /// <summary>
    /// Наносит относительный урон цели.
    /// </summary>
    /// <param name="damageFraction">Относительное количество наносимого урона в диапазоне (0; +INF).</param>
    /// <param name="target">Цель, которой наносится урон.</param>
    public void InflictRelativeDamage(float damageFraction, IDamageable target);
}