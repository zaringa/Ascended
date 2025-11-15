using System.Collections.Generic;
using UnityEngine;

public abstract class CraftableItem : ScriptableObject
{
    public IItem ResultItem;
    public bool isReadyToUse {get; set;}
    public string _name { get; set; }
    public float craftTime { get; set; }
    public virtual void OnUse()
    {

    }
    
}
[CreateAssetMenu(fileName = "NewSyringe", menuName = "Items/Syringe")]
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