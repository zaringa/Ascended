using UnityEngine;

public class PistolWeapon : BaseWeapon
{
    // Реализация абстрактного метода
    public override bool TryToFire()
    {
        // Проверка готовности (перезарядка, скорострельность)
        if (isReloading || Time.time < lastFireTime + gunInfo.fireRate)
        {
            return false;
        }

        if (currentMagazineAmmo > 0)
        {
            currentMagazineAmmo--;
            lastFireTime = Time.time;
            
            // --- Логика стрельбы Пистолета ---
            Debug.Log($"[ПИСТОЛЕТ] Выстрел. Урон: {gunInfo.damage}");
            
            // TODO: Raycast или логика нанесения урона
            
            return true;
        }
        else
        {
            // Патроны закончились
            return false;
        }
    }
}