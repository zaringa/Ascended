using System.Collections.Generic;
using Player.Items;
using Systems.Stats;
using UnityEngine;

namespace Systems.Inventory.Affixes
{
    [CreateAssetMenu(fileName = "NewAffix", menuName = "Inventory/Affix")]
    public class AffixSO : ScriptableObject, IStatModifierSource
    {
        [Header("Basic Info")]
        public new string name;
        public string description;
        public Sprite sprite;
        
        [Header("Affix Properties")]
        public AffixType affixType;
        public WeaponRarity requiredRarity;
        
        [Header("Stat Modifiers")]
        [SerializeField] protected List<ModifierData> _modifiers;

        public IEnumerable<ModifierData> GetModifiers()
        {
            return _modifiers ?? new List<ModifierData>();
        }
    }
}