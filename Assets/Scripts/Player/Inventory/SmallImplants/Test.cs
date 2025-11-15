using UnityEngine;

[CreateAssetMenu(fileName = "Test", menuName = "Scriptable Objects/Test")]
public class Test : SmallImplant, IRare
{
    [SerializeField] private IRare.Rareness rarenessType;
    public override IRare.Rareness RarenessType { get => rarenessType; }

    public override void ImplantAction()
    {

    }
}