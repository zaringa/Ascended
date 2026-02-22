using System;
using UnityEngine;

namespace Systems.Inventory.Affixes
{
    [Serializable]
    public class AffixInstance
    {
        public AffixSO affix;
        public bool isEquipped = false;
    }
}