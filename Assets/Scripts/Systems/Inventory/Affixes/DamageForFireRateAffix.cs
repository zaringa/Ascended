using Systems.Inventory.Affixes;
using Systems.Stats;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageForFireRateAffix", menuName = "Inventory/Affix/DamageForFireRate")]
public class DamageForFireRateAffix : AffixSO
{
    [Header("Damage for Fire Rate")]
    public float fireRateReductionPercent = 0.2f;
    public float damageIncreasePercent = 0.3f;
    
    private void OnEnable()
    {
        affixType = AffixType.Static;
        requiredRarity = WeaponRarity.Rare;
        
        _modifiers = new List<ModifierData>
        {
            new ModifierData 
            { 
                statType = StatType.FireRate,
                modifierType = ModifierType.PercentAdd,
                value = -fireRateReductionPercent
            },
            new ModifierData 
            { 
                statType = StatType.Damage,
                modifierType = ModifierType.PercentAdd,
                value = damageIncreasePercent
            }
        };
    }
}