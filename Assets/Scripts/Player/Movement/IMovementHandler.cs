using UnityEngine;

namespace Player.Movement
{
    /// <summary>
    /// Интерфейс для обработчика движения игрока.
    /// Реализует стандартное поведение.
    /// </summary>
    public interface IMovementHandler
    {
        bool CanDash();
        void PerformDash();
        
        bool CanJump();
        void PerformJump();
        
        bool CanSlide();
        void PerformSlide();
    }

    /// <summary>
    /// Стандартный обработчик движения (без имплантов).
    /// </summary>
    public class DefaultMovementHandler : IMovementHandler
    {
        public bool CanDash() => true;
        public void PerformDash()
        {
            Debug.Log("Performing standard dash");
            // Стандартная логика рывка
        }

        public bool CanJump() => true;
        public void PerformJump()
        {
            Debug.Log("Performing standard jump");
            // Стандартная логика прыжка
        }

        public bool CanSlide() => true;
        public void PerformSlide()
        {
            Debug.Log("Performing standard slide");
            // Стандартная логика слайда
        }
    }
}
