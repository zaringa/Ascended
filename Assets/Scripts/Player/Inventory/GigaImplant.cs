using UnityEngine;

/// <summary> Гига-имплант третьего слоя </summary>
public abstract class GigaImplant : Implant, IRare
{
    public abstract IRare.Rareness RarenessType { get; }
}