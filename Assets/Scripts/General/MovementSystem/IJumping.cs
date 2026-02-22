/// <summary>
/// Позволяет прыгать.
/// </summary>
public interface IJumping
{
    /// <summary>
    /// Возможность прыжка.
    /// </summary>
    public bool CanJump { get; }

    /// <summary>
    /// Высота прыжка до применения бонусов.
    /// </summary>
    public float JumpHeightBase { get; }

    /// <summary>
    /// Высота прыжка после применения бонусов.
    /// </summary>
    public float JumpHeightModified { get; }

    /// <summary>
    /// 
    /// </summary>
    public float JumpBufferTime { get; }

    /// <summary>
    /// 
    /// </summary>
    public float CoyoteTime { get; }

    /// <summary>
    /// 
    /// </summary>
    void HandleJump();
}