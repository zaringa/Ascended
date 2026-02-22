using Player.Items.Implants.Base;
using UnityEngine;
using Player.Movement;

namespace Player.Items.Implants.Movement
{
    /// <summary>
    /// Имплант рывка. Устанавливается в слот ног (Legs).
    /// Наследуется от LegsImplant для ограничения в инспекторе.
    /// </summary>
    [CreateAssetMenu(fileName = "DashImplant", menuName = "Inventory/Implants/Movement/Dash")]
    public class DashImplant : LegsImplant
    {
        [Header("Характеристики рывка")]
        public float dashDistance = 5f;
        public float dashSpeed = 20f;
        public float cooldown = 1f;

        public override void Action()
        {
            // Вызов специфической механики рывка
            Debug.Log("Dash implant activated");
        }
        
        public override IMovementHandler GetMovementHandler()
        {
            return new DashMovementHandler(this);
        }
    }
    
    /// <summary>
    /// Обработчик движения для импланта рывка.
    /// Отключает стандартный рывок и использует свой.
    /// </summary>
    public class DashMovementHandler : IMovementHandler
    {
        public readonly DashImplant implant;
        
        public DashMovementHandler(DashImplant implant)
        {
            this.implant = implant;
        }
        
        public bool CanDash()
        {
            Debug.Log($"DashMovementHandler: Dash available (distance: {implant.dashDistance})");
            return true;
        }
        
        public void PerformDash()
        {
            Debug.Log($"Performing DashImplant dash: distance={implant.dashDistance}, speed={implant.dashSpeed}");
            // Кастомная логика рывка от импланта
        }
        
        public bool CanJump() => true;
        public void PerformJump()
        {
            Debug.Log("Performing standard jump (from DashMovementHandler)");
        }
        
        public bool CanSlide() => true;
        public void PerformSlide()
        {
            Debug.Log("Performing standard slide (from DashMovementHandler)");
        }
    }
}
