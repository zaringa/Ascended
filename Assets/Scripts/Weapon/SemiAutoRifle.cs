using UnityEngine;

public class SemiAutoRifle : BaseWeapon
{
    private float fireCooldown;

    protected override void Start()
    {
        base.Start();
        // Установить параметры для полуавтоматической винтовки
        gunInfo.fireRate = 120f / 60f; // 120 выстрелов в минуту
        gunInfo.maxMagazineCapacity = 10;
        gunInfo.reloadTime = 5f;
        gunInfo.damage = 50f;
        fireCooldown = 1f / gunInfo.fireRate;
    }

    public override bool TryToFire()
    {
        if (isReloading || Time.time - lastFireTime < fireCooldown)
            return false;

        lastFireTime = Time.time;
        currentMagazineAmmo--;
        InvokeAmmoChanged(currentMagazineAmmo, gunInfo.maxMagazineCapacity);
        InvokeWeaponFired();
        PlaySound(gunInfo.fireSound);

        // Логика стрельбы (например, создание снаряда)
        Debug.Log("SemiAutoRifle fired!");

        if (currentMagazineAmmo <= 0)
            StartReload();

        return true;
    }
}