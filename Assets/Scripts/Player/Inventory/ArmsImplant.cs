using UnityEngine;

/// <summary> Имплант первого слоя под руки </summary>
//[CreateAssetMenu(fileName = "New arms implant", menuName = "Items/Implants/Arms")]
public abstract class ArmsImplant : Implant
{
    public abstract void ImplantAction();
}