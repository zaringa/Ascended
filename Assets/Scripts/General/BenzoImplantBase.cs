using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public enum AffectedAction
    {
        dash,
        slide,
        jump
    }
    public abstract class BenzoImplantBase : MonoBehaviour
    {
        // Событие при использовании импланта
        //protected event Action OnUse;

        // Название импланта
        public string Name { get; set; } = "Default implant name";

        // Описание импланта
        public string Description { get; set; } = "Default implant description";

        // Экипирован ли имплант
        public bool IsEquiped { get; set; } = false;

        public AffectedAction a_action { get; set; }

        protected virtual void Start()
        {
            //OnUse += AttachStack;
        }

        // Метод для накладывания стака статуса "Адреналин". Обработчик события OnUse.
        protected virtual void AttachStack(bool bEnable)
        {
            // -- Логика накладывания стака -- 
        }

        // Абстрактный метод для применения эффекта импланта
        public abstract void Use(bool bEnable);
    }
}