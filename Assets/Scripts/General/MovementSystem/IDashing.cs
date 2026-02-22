/// <summary>
/// Позволяет совершать рывок.
/// </summary>
public interface IDashing : IAbility
{
    /// <summary>
    /// Длительность рывка до применения бонусов.
    /// </summary>
    public float DashDurationBase { get; }

    /// <summary>
    /// Длительность рывка после применения бонусов.
    /// </summary>
    public float DashDurationModified { get; }

    /// <summary>
    /// Скорость рывка до применения бонусов.
    /// </summary>
    public float DashSpeedBase { get; }

    /// <summary>
    /// Скорость рывка после применения бонусов.
    /// </summary>
    public float DashSpeedModified { get; }

    /// <summary>
    /// Время перезарядки рывка (в секундах) до применения бонусов.
    /// </summary>
    public new float CooldownTimeBase { get; }

    /// <summary>
    /// Время перезарядки рывка (в секундах) после применения бонусов.
    /// </summary>
    public new float CooldownTimeModified { get; }

    /// <summary>
    /// Совершается ли рывок в текущем кадре.
    /// </summary>
    public bool IsDashing { get; protected set; }

    /// <summary>
    /// Кулдаун-система рывка.
    /// </summary>
    public new CooldownSystem CooldownSystem { get; protected set; }

    /// <summary>
    /// Индикатор кулдаун-системы рывка.
    /// </summary>
    public new CooldownBarUI CooldownBar { get; }

    /// <summary>
    /// Возможность использовать рывок.
    /// </summary>
    public new bool CanPerform { get; }

    /// <summary>
    /// Астивация рывка.
    /// </summary>
    public new void Activate();
}