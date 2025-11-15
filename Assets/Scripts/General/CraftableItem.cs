using System.Collections.Generic;
using UnityEngine;

public abstract class CraftableItem : MonoBehaviour
{
    public IItem ResultItem;
    public bool isReadyToUse {get; set;}
    public string _name { get; set; }
    public float craftTime { get; set; }
    public virtual void OnUse()
    {

    }
    
}
