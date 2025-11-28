using System.Collections.Generic;
using Player.Items;
using Player.Items.Implants;
using Player.Items.Implants.Base;
using UnityEngine;

namespace Player.Inventory
{
    public class Inventory : MonoBehaviour
    {
        [Header("Основные импланты")]
        [SerializeField] private ArmsImplant armsImplant;
        [SerializeField] private BodyImplant bodyImplant;
        [SerializeField] private LegsImplant legsImplant;

        [Header("Малые и гига-импланты")]
        [SerializeField] private List<(SmallImplant, int)> smallImplants;
        [SerializeField] private List<(GigaImplant, int)> gigaImplants;

        [Header("Основное и второстепенное оружие")]
        [SerializeField] private GunInfo primaryWeapon;
        [SerializeField] private GunInfo secondaryWeapon;

        [Header("Расходники")]
        [SerializeField] private IConsumable[] craftedConsumables = new IConsumable[3];
        [SerializeField] private List<(IConsumable, int)> consumables;
    }
}
