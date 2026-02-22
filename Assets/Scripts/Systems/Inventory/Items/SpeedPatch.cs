using Systems.Inventory.Items;
using Systems.Stats;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeedPatch", menuName = "Inventory/Items/Patches/Speed")]
public class SpeedPatch : PatchItemSO
{
    [Header("Speed Bonus")]
    public float speedBonusPercent = 0.15f; // +15% к скорости
    
    private void OnEnable()
    {
        name = "Speed Enhancement Patch";
        description = "Increases movement speed by " + (speedBonusPercent * 100) + "%";
        
        _modifiers = new List<ModifierData>
        {
            new ModifierData 
            { 
                statType = StatType.Speed,
                modifierType = ModifierType.PercentAdd,
                value = speedBonusPercent
            }
        };
    }
}