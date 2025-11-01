using UnityEngine;

/// <summary> Гига-имплант третьего слоя </summary>
[CreateAssetMenu(fileName = "New giga implant", menuName = "Items/Implants/Giga")]
public abstract class GigaImplant : Implant
{
    /// <summary> Редкость импланта </summary>
    public Rareness type = Rareness.Common;
}