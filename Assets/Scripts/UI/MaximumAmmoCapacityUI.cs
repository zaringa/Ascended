using UnityEngine;
using TMPro;

public class MaximumAmmoCapacityUI : MonoBehaviour
{
    [SerializeField] PistolWeapon weapon;
    [SerializeField] TextMeshProUGUI text;
    void Update()
    {
        if (weapon != null && text != null)
        {
            text.text = weapon.MaxAmmo.ToString();
        }
    }
}
