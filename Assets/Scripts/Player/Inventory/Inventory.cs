using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Основные импланты")]
    [SerializeField] public ArmsImplant armsImplant;
    [SerializeField] public BodyImplant bodyImplant;
    [SerializeField] public LegsImplant legsImplant;

    [Header("Малые и гига-импланты")]
    [SerializeField] public List<(SmallImplant, int)> smallImplants;
    [SerializeField] public List<(GigaImplant, int)> gigaImplants;

    [Header("Основное и второстепенное оружие")]
    [SerializeField] public GunInfo primaryWeapon;
    [SerializeField] public GunInfo secondaryWeapon;

    [Header("Расходники")]
    [SerializeField] public IConsumable[] craftedConsumables = new IConsumable[3];
    [SerializeField] public List<(IConsumable, int)> consumables;
}