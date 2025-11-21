using UnityEngine;

public class Syringe : CraftableItem 
{
    void Awake()
    {
        lename = "Syringe";
    }
    public new string lename = "Syringe";
    public new float craftTime = 4.5f;
    public float deltaAdd = 15F;
    public override void OnUse()
    {
        base.OnUse();
        Debug.Log("HEALED "+ deltaAdd+ " HEALTH");
    }
}