using System.Collections.Generic;

/// <summary>
/// Позволяет устанавливать импланты.
/// </summary>
public interface IImplantable
{
    /// <summary>
    /// Список установленных имплантов.
    /// </summary>
    public List<Implant> Implants { get; protected set; }

    /// <summary>
    /// Добавляет имплант.
    /// </summary>
    /// <param name="implant"></param>
    public void AddImplant(Implant implant);

    /// <summary>
    /// Убирает имплант.
    /// </summary>
    /// <param name="implant"></param>
    public void RemoveImplant(Implant implant);
}

public class Implant { }