using UnityEngine;

/// <summary> Малый мплант второго слоя </summary>
public abstract class SmallImplant : Implant, IRare
{
    public abstract IRare.Rareness RarenessType { get; }
}