using UnityEngine;
using System;

public class EquippedGun : ScriptableObject
{
    public Gun gunItem;
    public int currentClipAmmo;
    public int currentAmmo;
    // Update is called once per frame
    public void Reload()
    {
        currentClipAmmo = Math.Clamp(currentAmmo, 0, gunItem.maxClipAmmo);
        currentAmmo -= currentClipAmmo;
    }
    public void Shoot()
    {
        currentClipAmmo -= 1;
        if (currentAmmo <=0)
        {
            Reload();
        }
    }
}
