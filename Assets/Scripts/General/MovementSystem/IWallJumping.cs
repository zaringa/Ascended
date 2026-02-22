/// <summary>
/// Позволяет прыгать от стен.
/// </summary>
public interface IWallJumping : IJumping, IAbility
{
    /// <summary>
    /// Тэг, использующийся для определения стен.
    /// </summary>
    [UnityEngine.SerializeField] protected const string WallTag = "Wall";

    /// <summary>
    /// Дистанция проверки до стены.
    /// </summary>
    public float WallCheckDistance { get; }

    /// <summary>
    /// Максимальная скорость скольжения по стене до применения бонусов.
    /// </summary>
    public float WallSlideSpeedBase { get; }

    /// <summary>
    /// Максимальная скорость скольжения по стене до применения бонусов.
    /// </summary>
    public float WallSlideSpeedModified { get; }

    /// <summary>
    /// Сила прыжка от стены до применения бонусов.
    /// </summary>
    public float WallJumpForceBase { get; }

    /// <summary>
    /// Сила прыжка от стены до применения бонусов.
    /// </summary>
    public float WallJumpForceModified { get; }

    /// <summary>
    /// Высота прыжка от стены до применения бонусов.
    /// </summary>
    public float WallJumpHeightBase { get; }

    /// <summary>
    /// Высота прыжка от стены до применения бонусов.
    /// </summary>
    public float WallJumpHeightModified { get; }

    /// <summary>
    /// Оставшееся время импульса (в секундах) до применения бонусов.
    /// </summary>
    public float MomentumDecayBase { get; }

    /// <summary>
    /// Оставшееся время импульса (в секундах) до применения бонусов.
    /// </summary>
    public float MomentumDecayModified { get; }

    /// <summary>
    /// Время перезарядки прыжка от стены (в секундах) до применения бонусов.
    /// </summary>
    public new float CooldownTimeBase { get; }

    /// <summary>
    /// Время перезарядки прыжка от стены (в секундах) после применения бонусов.
    /// </summary>
    public new float CooldownTimeModified { get; }

    /// <summary>
    /// Кулдаун-система прыжка от стены.
    /// </summary>
    public new CooldownSystem CooldownSystem { get; protected set; }

    /// <summary>
    /// Индикатор кулдаун-системы прыжка от стены.
    /// </summary>
    public new CooldownBarUI CooldownBar { get; }

    /// <summary>
    /// Возможность совершить прыжок от стены.
    /// </summary>
    public new bool CanPerform { get; }

    /// <summary>
    /// Астивация прыжка от стены.
    /// </summary>
    public new void Activate();
}