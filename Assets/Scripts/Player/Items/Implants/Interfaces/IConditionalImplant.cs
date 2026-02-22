using Enemy;

namespace Player.Items.Implants.Interfaces
{
    // Интерфейс для имплантов с условными эффектами
    public interface IConditionalImplant
    {
        void SubscribeToEvents();
        void UnsubscribeFromEvents();
    }

    // Пример интерфейса для имплантов, которые срабатывают при атаке
    public interface IOnAttackImplant : IConditionalImplant
    {
        float OnAttack(float baseDamage);
    }
    
    // Пример интерфейса для имплантов, которые срабатывают при попадании
    public interface IOnHitImplant : IConditionalImplant
    {
        void OnHit(IEnemy target, ref float damage);
    }
}