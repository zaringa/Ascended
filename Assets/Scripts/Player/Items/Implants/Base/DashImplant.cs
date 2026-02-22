using UnityEngine;

namespace Player.Items.Implants.Base
{
    /// <summary>
    /// Имплант рывка. Устанавливается в слот ног (Legs).
    /// </summary>
    [CreateAssetMenu(fileName = "DashImplant", menuName = "Inventory/Implants/Movement/Dash")]
    public class DashImplant : LegsImplant
    {
        [Header("Dash Properties")]
        public float dashDistance = 5f;
        public float dashSpeed = 20f;
        public float cooldown = 1f;

        public override void Action()
        {
            // Вызов специфической механики рывка
            // PlayerMovement.Instance.PerformDash(dashDistance, dashSpeed, cooldown);
            Debug.Log("Dash implant activated");
        }
    }
}
