using TMPro;
using UnityEngine;

public class CurrentRoundsCount : MonoBehaviour
{
    [SerializeField] PistolWeapon weapon;
    [SerializeField] TextMeshProUGUI text;
    void Update()
    {
        if (weapon != null && text != null)
        {
            text.text = weapon.CurrentAmmo.ToString();
        }
    }
}
