using System.Collections.Generic;
using Player.Items;
using Player.Items.Implants.Base;
using UnityEngine;

namespace Player.Inventory
{
    /// <summary>
    /// Основная система инвентаря игрока.
    /// Управляет имплантами, оружием и расходниками.
    /// </summary>
    public class Inventory : MonoBehaviour
    {
        [Header("Основные импланты")]
        [SerializeField] private ArmsImplant equippedArmsImplant;
        [SerializeField] private BodyImplant equippedBodyImplant;
        [SerializeField] private LegsImplant equippedLegsImplant;

        [Header("Малые и гига-импланты")]
        [SerializeField] private List<(SmallImplant, int)> smallImplants;
        [SerializeField] private List<(GigaImplant, int)> gigaImplants;

        [Header("Основное и второстепенное оружие")]
        [SerializeField] private GunInfo primaryWeapon;
        [SerializeField] private GunInfo secondaryWeapon;

        [Header("Расходники")]
        [SerializeField] private IConsumable[] craftedConsumables = new IConsumable[3];
        [SerializeField] private List<(IConsumable, int)> consumables;

        [Header("Общий инвентарь")]
        [SerializeField] private List<(Item, int)> generalInventory = new List<(Item, int)>();
        [SerializeField] private int maxInventorySize = 30;

        // Менеджеры для специфических систем
        private Systems.Inventory.Managers.ImplantManager implantManager;
        private Systems.Inventory.Managers.WeaponManager weaponManager;
        private Systems.Inventory.Managers.ConsumableManager consumableManager;

        void Start()
        {
            InitializeManagers();
            InitializeLists();
        }

        private void InitializeLists()
        {
            if (smallImplants == null) smallImplants = new List<(SmallImplant, int)>();
            if (gigaImplants == null) gigaImplants = new List<(GigaImplant, int)>();
            if (consumables == null) consumables = new List<(IConsumable, int)>();
            if (generalInventory == null) generalInventory = new List<(Item, int)>();
        }

        private void InitializeManagers()
        {
            implantManager = GetComponent<Systems.Inventory.Managers.ImplantManager>() ??
                           gameObject.AddComponent<Systems.Inventory.Managers.ImplantManager>();
            weaponManager = GetComponent<Systems.Inventory.Managers.WeaponManager>() ??
                          gameObject.AddComponent<Systems.Inventory.Managers.WeaponManager>();
            consumableManager = GetComponent<Systems.Inventory.Managers.ConsumableManager>() ??
                              gameObject.AddComponent<Systems.Inventory.Managers.ConsumableManager>();
        }

        #region Equipment Methods

        // Методы для экипировки имплантов
        public bool EquipImplant(Implant implant)
        {
            if (implant == null) return false;

            switch (implant)
            {
                case ArmsImplant arms:
                    if (equippedArmsImplant != null) return false;
                    equippedArmsImplant = arms;
                    implantManager?.EquipImplant(arms);
                    return true;

                case BodyImplant body:
                    if (equippedBodyImplant != null) return false;
                    equippedBodyImplant = body;
                    implantManager?.EquipImplant(body);
                    return true;

                case LegsImplant legs:
                    if (equippedLegsImplant != null) return false;
                    equippedLegsImplant = legs;
                    implantManager?.EquipImplant(legs);
                    Debug.Log($"Equipped legs implant: {legs.name}");
                    return true;

                case SmallImplant small:
                    return AddSmallImplant(small);

                case GigaImplant giga:
                    return AddGigaImplant(giga);
            }

            return false;
        }

        public bool UnequipImplant(Implant implant)
        {
            if (implant == null) return false;

            switch (implant)
            {
                case ArmsImplant arms when equippedArmsImplant == arms:
                    implantManager?.UnequipImplant(arms);
                    equippedArmsImplant = null;
                    return true;

                case BodyImplant body when equippedBodyImplant == body:
                    implantManager?.UnequipImplant(body);
                    equippedBodyImplant = null;
                    return true;

                case LegsImplant legs when equippedLegsImplant == legs:
                    implantManager?.UnequipImplant(legs);
                    equippedLegsImplant = null;
                    return true;

                case SmallImplant small:
                    return RemoveSmallImplant(small);

                case GigaImplant giga:
                    return RemoveGigaImplant(giga);
            }

            return false;
        }

        private bool AddSmallImplant(SmallImplant implant)
        {
            for (int i = 0; i < smallImplants.Count; i++)
            {
                if (smallImplants[i].Item1 == implant)
                {
                    smallImplants[i] = (implant, smallImplants[i].Item2 + 1);
                    return true;
                }
            }

            if (smallImplants.Count < 10)
            {
                smallImplants.Add((implant, 1));
                return true;
            }

            return false;
        }

        private bool RemoveSmallImplant(SmallImplant implant)
        {
            for (int i = 0; i < smallImplants.Count; i++)
            {
                if (smallImplants[i].Item1 == implant)
                {
                    if (smallImplants[i].Item2 > 1)
                    {
                        smallImplants[i] = (implant, smallImplants[i].Item2 - 1);
                    }
                    else
                    {
                        smallImplants.RemoveAt(i);
                    }
                    return true;
                }
            }
            return false;
        }

        private bool AddGigaImplant(GigaImplant implant)
        {
            for (int i = 0; i < gigaImplants.Count; i++)
            {
                if (gigaImplants[i].Item1 == implant)
                {
                    gigaImplants[i] = (implant, gigaImplants[i].Item2 + 1);
                    return true;
                }
            }

            if (gigaImplants.Count < 5)
            {
                gigaImplants.Add((implant, 1));
                return true;
            }

            return false;
        }

        private bool RemoveGigaImplant(GigaImplant implant)
        {
            for (int i = 0; i < gigaImplants.Count; i++)
            {
                if (gigaImplants[i].Item1 == implant)
                {
                    if (gigaImplants[i].Item2 > 1)
                    {
                        gigaImplants[i] = (implant, gigaImplants[i].Item2 - 1);
                    }
                    else
                    {
                        gigaImplants.RemoveAt(i);
                    }
                    return true;
                }
            }
            return false;
        }

        // Методы для экипировки оружия
        public bool EquipWeapon(GunInfo weapon, bool isPrimary = true)
        {
            if (weapon == null) return false;

            if (isPrimary)
            {
                if (primaryWeapon != null) return false; // Слот занят
                primaryWeapon = weapon;
                weaponManager?.EquipWeapon(weapon, true);
            }
            else
            {
                if (secondaryWeapon != null) return false; // Слот занят
                secondaryWeapon = weapon;
                weaponManager?.EquipWeapon(weapon, false);
            }

            return true;
        }

        public bool UnequipWeapon(bool isPrimary = true)
        {
            if (isPrimary)
            {
                if (primaryWeapon != null)
                {
                    weaponManager?.UnequipWeapon(true);
                    primaryWeapon = null;
                    return true;
                }
            }
            else
            {
                if (secondaryWeapon != null)
                {
                    weaponManager?.UnequipWeapon(false);
                    secondaryWeapon = null;
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region General Inventory Methods

        public bool AddItem(Item item, int quantity = 1)
        {
            if (item == null) return false;

            // Проверяем, можем ли стакать этот предмет
            if (CanStackItem(item))
            {
                for (int i = 0; i < generalInventory.Count; i++)
                {
                    if (generalInventory[i].Item1 == item)
                    {
                        generalInventory[i] = (item, generalInventory[i].Item2 + quantity);
                        return true;
                    }
                }
            }

            // Добавляем новый предмет
            if (generalInventory.Count < maxInventorySize)
            {
                generalInventory.Add((item, quantity));
                return true;
            }

            return false; // Инвентарь полон
        }

        public bool RemoveItem(Item item, int quantity = 1)
        {
            if (item == null) return false;

            for (int i = 0; i < generalInventory.Count; i++)
            {
                if (generalInventory[i].Item1 == item)
                {
                    if (generalInventory[i].Item2 <= quantity)
                    {
                        generalInventory.RemoveAt(i);
                    }
                    else
                    {
                        generalInventory[i] = (item, generalInventory[i].Item2 - quantity);
                    }
                    return true;
                }
            }

            return false;
        }

        private bool CanStackItem(Item item)
        {
            // Определяем, можно ли стакать этот тип предмета
            // Например, патчи могут стакаться, а уникальные предметы - нет
            return item is Systems.Inventory.Items.PatchItemSO; // Упрощенная проверка
        }

        #endregion

        #region Consumable Methods

        public bool AddConsumable(IConsumable consumable, int quantity = 1)
        {
            if (consumable == null) return false;

            // Проверяем, есть ли уже такой расходник
            for (int i = 0; i < consumables.Count; i++)
            {
                if (ReferenceEquals(consumables[i].Item1, consumable))
                {
                    consumables[i] = (consumables[i].Item1, consumables[i].Item2 + quantity);
                    return true;
                }
            }

            // Добавляем новый расходник
            consumables.Add((consumable, quantity));
            return true;
        }

        public bool UseConsumable(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= craftedConsumables.Length)
                return false;

            var consumable = craftedConsumables[slotIndex];
            if (consumable != null)
            {
                consumable.Use();
                craftedConsumables[slotIndex] = null; // Удаляем использованный расходник
                return true;
            }

            return false;
        }

        public bool PrepareCrafting(Systems.Crafting.CraftingRecipeSO recipe, int slotIndex)
        {
            if (recipe == null || slotIndex < 0 || slotIndex >= craftedConsumables.Length)
                return false;

            // Проверяем, свободен ли слот
            if (craftedConsumables[slotIndex] != null)
                return false;

            // Здесь должна быть реализация процесса крафта
            // с ограничениями на одновременное крафтование и т.д.

            // Для демонстрации просто добавляем заглушку
            // В реальности здесь будет запуск корутины крафта
            if (recipe.outputItem is IConsumable consumableOutput)
            {
                craftedConsumables[slotIndex] = consumableOutput;
                return true;
            }
            else
            {
                Debug.LogError($"Recipe output item is not compatible with IConsumable: {recipe.outputItem.name}");
                return false;
            }
        }

        #endregion

        #region Public Accessors

        public GunInfo GetGunInfo(bool isPrimary) =>
            isPrimary ? primaryWeapon : secondaryWeapon;

        public ArmsImplant GetArmsImplant() => equippedArmsImplant;
        public BodyImplant GetBodyImplant() => equippedBodyImplant;
        public LegsImplant GetLegsImplant() => equippedLegsImplant;

        public List<(SmallImplant, int)> GetSmallImplants() => smallImplants;
        public List<(GigaImplant, int)> GetGigaImplants() => gigaImplants;

        public List<(Item, int)> GetGeneralInventory() => generalInventory;
        public int GetMaxInventorySize() => maxInventorySize;

        public IConsumable[] GetCraftedConsumables() => craftedConsumables;
        public List<(IConsumable, int)> GetConsumables() => consumables;

        #endregion
    }
}
