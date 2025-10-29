using UnityEngine;

/// <summary> Малый мплант второго слоя </summary>
[CreateAssetMenu(fileName = "New small implant", menuName = "Items/Implants/Small")]
public class SmallImplant : Implant
{
    /// <summary> Редкость импланта </summary>
    public Rareness type = Rareness.Common;
}