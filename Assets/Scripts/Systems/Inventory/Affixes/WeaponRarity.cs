namespace Systems.Inventory.Affixes
{
    public enum WeaponRarity
    {
        Standard,    // 0 слотов под аффиксы
        Uncommon,    // 1 слот под статический аффикс
        Rare,        // 1 слот под статический + 1 под условный
        Epic,        // 2 слота под статические + 1 под условный
        Legendary    // 2 слота под статические + 2 под условные
    }
}