using UnityEngine;

public class DoubleBarrelShotgun : BaseWeapon
{
    protected override void Start()
    {
        base.Start();
        // Установить параметры для двустволки
        gunInfo.fireRate = 0f; // Нет скорострельности
        gunInfo.maxMagazineCapacity = 2; // Два патрона
        gunInfo.reloadTime = 2f;
        gunInfo.damage = 100f;
    }

    public override bool TryToFire()
    {
        if (isReloading || currentMagazineAmmo < 2)
        {
            Debug.Log("Not enough ammo to fire double shot!");
            return false;
        }

        currentMagazineAmmo -= 2;
        InvokeAmmoChanged(currentMagazineAmmo, gunInfo.maxMagazineCapacity);
        InvokeWeaponFired();
        PlaySound(gunInfo.fireSound);

        // Логика конуса урона
        ApplyConeDamage();

        if (currentMagazineAmmo <= 0)
            StartReload();

        return true;
    }

    private void ApplyConeDamage()
    {
        // Реализация механики конуса
        Debug.Log("Applying cone damage!");
        // Здесь можно использовать Physics.OverlapSphere или OverlapBox для определения целей в конусе
    }
}