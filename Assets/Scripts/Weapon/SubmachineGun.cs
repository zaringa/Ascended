using UnityEngine;

public class SubmachineGun : BaseWeapon
{
    protected override void Start()
    {
        base.Start();
        // Установить параметры для пистолета-пулемёта
        gunInfo.fireRate = 900f / 60f; // 900 выстрелов в минуту
        gunInfo.maxMagazineCapacity = 30;
        gunInfo.reloadTime = 2f;
        gunInfo.damage = 5f;
    }

    public override bool TryToFire()
    {
        if (isReloading || Time.time - lastFireTime < 1f / gunInfo.fireRate)
            return false;

        lastFireTime = Time.time;
        currentMagazineAmmo--;
        InvokeAmmoChanged(currentMagazineAmmo, gunInfo.maxMagazineCapacity);
        InvokeWeaponFired();
        PlaySound(gunInfo.fireSound);

        // Логика стрельбы (например, создание снаряда)
        Debug.Log("SubmachineGun fired!");

        if (currentMagazineAmmo <= 0)
            StartReload();

        return true;
    }
}