using UnityEngine;
using System.Collections.Generic;
using Player.Items.Implants.Base;
using Player.Items.Implants.Interfaces;
using Systems.Stats;

namespace Systems.Inventory.Managers
{
    /// <summary>
    /// Компонент для управления экипированными имплантами игрока.
    /// </summary>
    public class ImplantManager : MonoBehaviour, IStatsProvider
    {
        [Header("Implant Slots")]
        [SerializeField] private ArmsImplant equippedArmsImplant;
        [SerializeField] private BodyImplant equippedBodyImplant;
        [SerializeField] private LegsImplant equippedLegsImplant;
        [SerializeField] private List<SmallImplant> equippedSmallImplants = new List<SmallImplant>();
        [SerializeField] private List<GigaImplant> equippedGigaImplants = new List<GigaImplant>();

        private Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat>();
        private List<IConditionalImplant> activeConditionalImplants = new List<IConditionalImplant>();

        void Start()
        {
            InitializeStats();
        }

        private void InitializeStats()
        {
            StatType[] statTypes = System.Enum.GetValues(typeof(StatType)) as StatType[];
            foreach (StatType statType in statTypes)
            {
                stats[statType] = new Stat(statType, GetBaseStatValue(statType));
            }
        }

        private float GetBaseStatValue(StatType statType)
        {
            switch (statType)
            {
                case StatType.Health:
                case StatType.MaxHealth:
                    return 100f;
                case StatType.Damage:
                    return 10f;
                case StatType.Speed:
                    return 5f;
                case StatType.FireRate:
                    return 0.1f;
                case StatType.CritChance:
                    return 0.05f;
                case StatType.CritDamage:
                    return 0.5f;
                default:
                    return 1f;
            }
        }

        public void EquipImplant(Implant implant)
        {
            if (implant == null) return;

            switch (implant)
            {
                case ArmsImplant arms:
                    if (equippedArmsImplant != null) UnequipImplant(equippedArmsImplant);
                    equippedArmsImplant = arms;
                    break;
                case BodyImplant body:
                    if (equippedBodyImplant != null) UnequipImplant(equippedBodyImplant);
                    equippedBodyImplant = body;
                    break;
                case LegsImplant legs:
                    if (equippedLegsImplant != null) UnequipImplant(equippedLegsImplant);
                    equippedLegsImplant = legs;
                    break;
                case SmallImplant small:
                    equippedSmallImplants.Add(small);
                    break;
                case GigaImplant giga:
                    equippedGigaImplants.Add(giga);
                    break;
            }

            ApplyImplantModifiers(implant);

            var conditionalImplant = implant as IConditionalImplant;
            if (conditionalImplant != null)
            {
                conditionalImplant.SubscribeToEvents();
                activeConditionalImplants.Add(conditionalImplant);
            }
        }

        public void UnequipImplant(Implant implant)
        {
            if (implant == null) return;

            RemoveImplantModifiers(implant);

            var conditionalImplant = implant as IConditionalImplant;
            if (conditionalImplant != null)
            {
                conditionalImplant.UnsubscribeFromEvents();
                activeConditionalImplants.Remove(conditionalImplant);
            }

            switch (implant)
            {
                case ArmsImplant arms when equippedArmsImplant == arms:
                    equippedArmsImplant = null;
                    break;
                case BodyImplant body when equippedBodyImplant == body:
                    equippedBodyImplant = null;
                    break;
                case LegsImplant legs when equippedLegsImplant == legs:
                    equippedLegsImplant = null;
                    break;
                case SmallImplant small:
                    equippedSmallImplants.Remove(small);
                    break;
                case GigaImplant giga:
                    equippedGigaImplants.Remove(giga);
                    break;
            }
        }

        private void ApplyImplantModifiers(Implant implant)
        {
            if (implant is IStatModifierSource statModifierSource)
            {
                var modifiers = statModifierSource.GetModifiers();
                foreach (var modifierData in modifiers)
                {
                    var stat = stats[modifierData.statType];
                    var modifier = new StatModifier(modifierData.value, modifierData.modifierType, implant);
                    stat.AddModifier(modifier);
                }
            }
        }

        private void RemoveImplantModifiers(Implant implant)
        {
            foreach (var kvp in stats)
            {
                kvp.Value.RemoveAllModifiersFromSource(implant);
            }
        }

        public float GetStatValue(StatType type)
        {
            if (stats.TryGetValue(type, out Stat stat))
                return stat.Value;
            return 0f;
        }

        public IStat GetStat(StatType type)
        {
            stats.TryGetValue(type, out Stat stat);
            return stat;
        }

        void OnDestroy()
        {
            foreach (var conditionalImplant in activeConditionalImplants)
            {
                conditionalImplant.UnsubscribeFromEvents();
            }
        }
    }
}
