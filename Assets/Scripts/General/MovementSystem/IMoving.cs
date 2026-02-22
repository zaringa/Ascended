using UnityEngine;

/// <summary>
/// Позволяет перемещаться.
/// </summary>
public interface IMoving
{
    /// <summary>
    /// Скорость передвижения до применения бонусов.
    /// </summary>
    public float MovementSpeedBase { get; }

    /// <summary>
    /// Скорость передвижения после применения бонусов.
    /// </summary>
    public float MovementSpeedModified { get; }

    /// <summary>
    /// Оставшееся время импульса (в секундах) до применения бонусов.
    /// </summary>
    /// <remarks>В текущий момент не используется.</remarks>
    public float MomentumDecayBase { get; }

    /// <summary>
    /// Оставшееся время импульса (в секундах) после применения бонусов.
    /// </summary>
    /// <remarks>В текущий момент не используется.</remarks>
    public float MomentumDecayModified { get; }

    /// <summary>
    /// Направление движения.
    /// </summary>
    public Vector3 MoveDirection { get; protected set; }

    /// <summary>
    /// Позиция в предыдущем кадре.
    /// </summary>
    public Vector3 PreviousPosition { get; protected set; }

    /// <summary>
    /// Скорость в предыдущем кадре.
    /// </summary>
    public float PreviousSpeed { get; protected set; }

    /// <summary>
    /// 
    /// </summary>
    public void HandleMovement();
}