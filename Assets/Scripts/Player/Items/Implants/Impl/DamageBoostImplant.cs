using Player.Items.Implants.Base;
using Player.Items.Implants.Interfaces;
using UnityEngine;
using Systems.Stats;
using System.Collections.Generic;

namespace Player.Items.Implants.Impl
{
    /// <summary>
    /// Редкий имплант, увеличивающий урон.
    /// Устанавливается в слот для малых имплантов (Small).
    /// </summary>
    [CreateAssetMenu(fileName = "DamageBoostImplant", menuName = "Inventory/Implants/Rare/DamageBoost")]
    public class DamageBoostImplant : SmallImplant, IStatModifierSource
    {
        [Header("Damage Boost")]
        public float baseBonus = 0.10f;
        public float perStack = 0.05f;
        
        [SerializeField] private List<ModifierData> _modifiers;
        
        private void OnEnable()
        {
            rarity = ImplantRarity.Rare;
            
            _modifiers = new List<ModifierData>
            {
                new ModifierData
                {
                    statType = StatType.Damage,
                    modifierType = ModifierType.PercentAdd,
                    value = baseBonus + perStack * 0 // Для стека 1
                }
            };
        }

        public IEnumerable<ModifierData> GetModifiers()
        {
            return _modifiers;
        }

        public override void Action() {}
    }
}
