using System.Collections.Generic;

/// <summary>
/// Позволяет хранить статусы.
/// </summary>
public interface IStatusable
{
    /// <summary>
    /// Список применённых статусов.
    /// </summary>
    public List<Status> Statuses { get; protected set; }

    /// <summary>
    /// Добавляет статус.
    /// </summary>
    /// <param name="status">Добавляемый статус.</param>
    public void AddStatus(Status status)
        => Statuses.Add(status);

    /// <summary>
    /// Убирает статус.
    /// </summary>
    /// <param name="status">Убираемый статус.</param>
    public void RemoveStatus(Status status)
        => Statuses.Remove(status);

    public float RecalculateEffect(ValueType valueType)
    {
        float resultAbsolute = 0,
              resultRelativeAdd = 0,
              resultRelativeMultiply = 0;
        foreach (var status in Statuses)
            foreach(var stat in status.Stats)
                switch(stat.ModificationType)
                {
                    case ModificationType.Absolute:
                        resultAbsolute += stat.Value;
                        break;
                    case ModificationType.RelativeAdd:
                        resultRelativeAdd += stat.Value;
                        break;
                    case ModificationType.RelativeMultiply:
                        resultRelativeMultiply += stat.Value;
                        break;
                }
        return resultAbsolute * (resultRelativeAdd + 1) * (resultRelativeMultiply + 1);
    }
}

public class Status
{
    public List<Stat> Stats { get; protected set; }
}

public struct Stat
{
    public ValueType ValueType { get; private set; }
    public float Value { get; private set; }
    public ModificationType ModificationType { get; private set; }
}

public enum ValueType
{
    MaxHP,
    JumpHeight,
    DashSpeed,
    DashDuration,
    DashCooldown
}

public enum ModificationType
{
    Absolute,
    RelativeAdd,
    RelativeMultiply
}