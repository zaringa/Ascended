using System.Collections.Generic;
using UnityEngine;

public abstract class CraftableItem : MonoBehaviour
{
    public IItem ResultItem;
    public bool isReadyToUse {get; set;}
    public string lename { get; set; }
    public float craftTime { get; set; }
    public virtual void OnUse()
    {
        Debug.Log(lename + " was used");
    }
    
}
