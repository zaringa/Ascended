using UnityEngine;

// Интерфейс для всех объектов, которые могут получать урон
public interface IDamageable
{
    void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal);
    bool IsAlive();
}