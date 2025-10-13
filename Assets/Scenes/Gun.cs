using UnityEngine;
[CreateAssetMenu(fileName = "NewGun", menuName = "Gun")]
public class Gun : ScriptableObject
{
    public int ID;
    public string gunName;
    public int maxAmmo;
    public int maxClipAmmo;
    public float damage;

}