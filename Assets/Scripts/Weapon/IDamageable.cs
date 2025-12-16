using UnityEngine;

/// <summary>
/// Интерфейс для объектов, которые могут получать урон
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Нанести урон объекту
    /// </summary>
    /// <param name="damage">Количество урона</param>
    /// <param name="hitPoint">Точка попадания</param>
    /// <param name="hitNormal">Нормаль поверхности в точке попадания</param>
    void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal);

    /// <summary>
    /// Проверка, жив ли объект
    /// </summary>
    bool IsAlive();
}