namespace Systems.Inventory.Affixes
{
    // Интерфейс для условных аффиксов
    public interface IConditionalAffix
    {
        void SubscribeToEvents();
        void UnsubscribeFromEvents();
    }

    // Аффикс, который срабатывает при попадании
    public interface IOnHitAffix : IConditionalAffix
    {
        void OnHit(object target, ref float damage);
    }

    // Аффикс, который срабатывает при выстреле
    public interface IOnFireAffix : IConditionalAffix
    {
        void OnFire(ref float damage);
    }

    // Аффикс, который срабатывает при начале стрельбы
    public interface IOnStartShootingAffix : IConditionalAffix
    {
        void OnStartShooting();
    }

    // Аффикс, который срабатывает через определенное время
    public interface ITimedAffix : IConditionalAffix
    {
        void OnTimerTick();
    }
}