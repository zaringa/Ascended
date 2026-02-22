using Player.Items.Implants.Base;
using UnityEngine;
using Player.Movement;

namespace Player.Items.Implants.Movement
{
    /// <summary>
    /// Имплант слайда. Устанавливается в слот ног (Legs).
    /// Наследуется от LegsImplant для ограничения в инспекторе.
    /// </summary>
    [CreateAssetMenu(fileName = "SlideImplant", menuName = "Inventory/Implants/Movement/Slide")]
    public class SlideImplant : LegsImplant
    {
        [Header("Характеристики слайда")]
        public float slideDuration = 1f;
        public float slideSpeedMultiplier = 2f;
        public bool enableWallBounce = false;

        public override void Action()
        {
            Debug.Log("Slide implant activated");
        }
        
        public override IMovementHandler GetMovementHandler()
        {
            return new SlideMovementHandler(this);
        }
    }
    
    /// <summary>
    /// Обработчик движения для импланта слайда.
    /// </summary>
    public class SlideMovementHandler : IMovementHandler
    {
        public readonly SlideImplant implant;
        
        public SlideMovementHandler(SlideImplant implant)
        {
            this.implant = implant;
        }
        
        public bool CanDash() => true;
        public void PerformDash()
        {
            Debug.Log("Performing standard dash (from SlideMovementHandler)");
        }
        
        public bool CanJump() => true;
        public void PerformJump()
        {
            Debug.Log("Performing standard jump (from SlideMovementHandler)");
        }
        
        public bool CanSlide()
        {
            Debug.Log($"SlideMovementHandler: Slide available (duration: {implant.slideDuration})");
            return true;
        }
        
        public void PerformSlide()
        {
            Debug.Log($"Performing SlideImplant slide: duration={implant.slideDuration}, speed={implant.slideSpeedMultiplier}, wallBounce={implant.enableWallBounce}");
            // Кастомная логика слайда от импланта
        }
    }
}
