using Player.Items.Implants.Base;
using UnityEngine;
using Player.Movement;

namespace Player.Items.Implants.Movement
{
    /// <summary>
    /// Имплант прыжка. Устанавливается в слот ног (Legs).
    /// Наследуется от LegsImplant для ограничения в инспекторе.
    /// </summary>
    [CreateAssetMenu(fileName = "JumpImplant", menuName = "Inventory/Implants/Movement/Jump")]
    public class JumpImplant : LegsImplant
    {
        [Header("Характеристики прыжка")]
        public float jumpForce = 10f;
        public float extraJumpHeight = 2f;
        public bool allowDoubleJump = false;

        public override void Action()
        {
            Debug.Log("Jump implant activated");
        }
        
        public override IMovementHandler GetMovementHandler()
        {
            return new JumpMovementHandler(this);
        }
    }
    
    /// <summary>
    /// Обработчик движения для импланта прыжка.
    /// </summary>
    public class JumpMovementHandler : IMovementHandler
    {
        public readonly JumpImplant implant;
        
        public JumpMovementHandler(JumpImplant implant)
        {
            this.implant = implant;
        }
        
        public bool CanDash() => true;
        public void PerformDash()
        {
            Debug.Log("Performing standard dash (from JumpMovementHandler)");
        }
        
        public bool CanJump()
        {
            Debug.Log($"JumpMovementHandler: Jump available (force: {implant.jumpForce}, doubleJump: {implant.allowDoubleJump})");
            return true;
        }
        
        public void PerformJump()
        {
            Debug.Log($"Performing JumpImplant jump: force={implant.jumpForce}, extraHeight={implant.extraJumpHeight}, doubleJump={implant.allowDoubleJump}");
            // Кастомная логика прыжка от импланта
        }
        
        public bool CanSlide() => true;
        public void PerformSlide()
        {
            Debug.Log("Performing standard slide (from JumpMovementHandler)");
        }
    }
}
