/// <summary>
/// Позволяет притягиваться гравитацией.
/// </summary>
public interface IGravityAffectable
{
    /// <summary>
    /// Сила воздействия гравитации.
    /// </summary>
    public float GravityForce => GravityDirection.magnitude;

    /// <summary>
    /// Влияние гравитации.
    /// </summary>
    public bool UseGravity { get; protected set; }

    /// <summary>
    /// Направление и сила гравитации.
    /// </summary>
    public UnityEngine.Vector3 GravityDirection { get; protected set; }

    /// <summary>
    /// Изменяет силу гравитации на абсолютную величину.
    /// </summary>
    /// <param name="forceAmount">Абсолютная величина изменения гравитации.</param>
    public void ChangeForceAbsolute(float forceAmount)
        => GravityDirection = GravityDirection + GravityDirection.normalized * forceAmount;

    /// <summary>
    /// Изменяет силу гравитации на относительную величину.
    /// </summary>
    /// <param name="forceFraction">Относительная величина изменения гравитации.</param>
    public void ChangeForceRelative(float forceFraction)
        => GravityDirection = GravityDirection * forceFraction;

    /// <summary>
    /// Включает гравитацию.
    /// </summary>
    public void Enable()
        => UseGravity = true;

    /// <summary>
    /// Выключает гравитацию.
    /// </summary>
    public void Disable()
        => UseGravity = false;

    /// <summary>
    /// Переключает влияние гравитации.
    /// </summary>
    public void SwitchAffection()
        => UseGravity = !UseGravity;

    /// <summary>
    /// Изменяет гравитацию по оси <see langword="X"/>.
    /// </summary>
    public void SwitchDirectionX()
        => GravityDirection.Set(
            -GravityDirection.x,
             GravityDirection.y,
             GravityDirection.z);

    /// <summary>
    /// Изменяет гравитацию по оси <see langword="Y"/>.
    /// </summary>
    public void SwitchDirectionY()
        => GravityDirection.Set(
             GravityDirection.x,
            -GravityDirection.y,
             GravityDirection.z);

    /// <summary>
    /// Изменяет гравитацию по оси <see langword="Z"/>.
    /// </summary>
    public void SwitchDirectionZ()
        => GravityDirection.Set(
             GravityDirection.x,
             GravityDirection.y,
            -GravityDirection.z);

    /// <summary>
    /// Изменяет гравитацию по всем осям (по <see langword="X"/>, <see langword="Y"/> и <see langword="Z"/>).
    /// </summary>
    public void SwitchDirectionAll()
        => GravityDirection.Set(
            -GravityDirection.x,
            -GravityDirection.y,
            -GravityDirection.z);

    /// <summary>
    /// Устанавливает силу гравитации.
    /// </summary>
    /// <param name="forceAmount">Устанавливаемая сила гравитации.</param>
    public void SetForce(float forceAmount)
        => GravityDirection = GravityDirection.normalized * forceAmount;

    /// <summary>
    /// Устанавливает направление гравитации.
    /// </summary>
    /// <param name="direction">Устанавливаемое направление и сила гравитации.</param>
    public void SetDirection(UnityEngine.Vector3 direction)
        => GravityDirection = direction;

    // public void RotateDirection();
}