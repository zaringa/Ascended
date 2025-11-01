using UnityEngine;

/// <summary> Имплант первого слоя под тело </summary>
//[CreateAssetMenu(fileName = "New body implant", menuName = "Items/Implants/Body")]
public abstract class BodyImplant : Implant
{
    public abstract void ImplantAction();
}