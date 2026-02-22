using Systems.Inventory.Items;
using Systems.Stats;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CritChancePatch", menuName = "Inventory/Items/Patches/CritChance")]
public class CritChancePatch : PatchItemSO
{
    [Header("Critical Chance Bonus")]
    public float critChanceBonus = 0.1f; // +10% к крит. шансу
    
    private void OnEnable()
    {
        name = "Critical Chance Patch";
        description = "Increases critical hit chance by " + (critChanceBonus * 100) + "%";
        
        _modifiers = new List<ModifierData>
        {
            new ModifierData 
            { 
                statType = StatType.CritChance,
                modifierType = ModifierType.Flat,
                value = critChanceBonus
            }
        };
    }
}