/// <summary>
/// Позволяет совершать слайд.
/// </summary>
public interface ISliding : IAbility
{
    /// <summary>
    /// Продолжительность слайда до применения бонусов.
    /// </summary>
    public float SlideDurationBase { get; }

    /// <summary>
    /// Продолжительность слайда после применения бонусов.
    /// </summary>
    public float SlideDurationModified { get; }

    /// <summary>
    /// Множитель скорости на время слайда до применения бонусов.
    /// </summary>
    public float SlideSpeedMultiplierBase { get; }

    /// <summary>
    /// Множитель скорости на время слайда после применения бонусов.
    /// </summary>
    public float SlideSpeedMultiplierModified { get; }

    /// <summary>
    /// Высота персонажа во время слайда до применения бонусов.
    /// </summary>
    public float SlideHeightBase { get; }

    /// <summary>
    /// Высота персонажа во время слайда после применения бонусов.
    /// </summary>
    public float SlideHeightModified { get; }

    /// <summary>
    /// Время перезарядки слайда (в секундах) до применения бонусов.
    /// </summary>
    public new float CooldownTimeBase { get; }

    /// <summary>
    /// Время перезарядки слайда (в секундах) после применения бонусов.
    /// </summary>
    public new float CooldownTimeModified { get; }

    /// <summary>
    /// Совершается ли слайд в текущем кадре.
    /// </summary>
    public bool IsSliding { get; protected set; }

    /// <summary>
    /// Кулдаун-система слайда.
    /// </summary>
    public new CooldownSystem CooldownSystem { get; protected set; }

    /// <summary>
    /// Индикатор кулдаун-системы слайда.
    /// </summary>
    public new CooldownBarUI CooldownBar { get; }

    /// <summary>
    /// Возможность использовать слайд.
    /// </summary>
    public new bool CanPerform { get; }

    /// <summary>
    /// Астивация слайда.
    /// </summary>
    public new void Activate();
}