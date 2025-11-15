using UnityEngine;

public class Syringe : CraftableItem 
{
    public string _name = "Syringe";
    public float craftTime = 4.5f;
    public float deltaAdd = 15F;
    public override void OnUse()
    {
        base.OnUse();
        Debug.Log("HEALED "+ deltaAdd+ " HEALTH");
    }
}