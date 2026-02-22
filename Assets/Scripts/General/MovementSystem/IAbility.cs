/// <summary>
/// Способность.
/// </summary>
public interface IAbility
{
    /// <summary>
    /// Время перезарядки способности (в секундах) до применения бонусов.
    /// </summary>
    public float CooldownTimeBase { get; }

    /// <summary>
    /// Время перезарядки способности (в секундах) после применения бонусов.
    /// </summary>
    public float CooldownTimeModified { get; }

    /// <summary>
    /// Кулдаун-система.
    /// </summary>
    public CooldownSystem CooldownSystem { get; protected set; }

    /// <summary>
    /// Индикатор кулдаун-системы.
    /// </summary>
    public CooldownBarUI CooldownBar { get; }

    /// <summary>
    /// Возможность использовать действие.
    /// </summary>
    public bool CanPerform { get; }

    /// <summary>
    /// Астивация способности.
    /// </summary>
    public void Activate();
}