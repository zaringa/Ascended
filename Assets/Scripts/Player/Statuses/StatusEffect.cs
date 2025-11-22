namespace Player.Effects
{
    public abstract class StatusEffect
    {
        /// <summary> Общая продолжительность статуса </summary>
        public float duration;
        /// <summary> Оставшееся время статуса </summary>
        protected float timeReamining;
        /// <summary> Закончилось ли действие статуса </summary>
        public bool IsFinished { get; private set; }
        /// <summary> Накладываются ли несколько одинаковых статусов один поверх другого </summary>
        public readonly bool isStackable;
        /// <summary> Снимается ли статус по истечению времени </summary>
        public readonly bool isPermanent;

        public StatusEffect(float duration, bool isStackable = false, bool isPermanent = false)
        {
            this.duration = duration;
            timeReamining = duration;
            this.isStackable = isStackable;
            this.isPermanent = isPermanent;
        }

        /// <summary> Применяет эффект статуса </summary>
        /// <param name="effects"> Класс эффектов </param>
        public abstract void ApplyEffect(Effect effects);

        /// <summary> Убирает эффект статуса </summary>
        /// <param name="effects"> Класс эффектов </param>
        public abstract void RemoveEffect(Effect effects);

        /// <summary> Обновляет значение эффекта </summary>
        /// <param name="effects"> Класс эффектов </param>
        public virtual void UpdateEffect(float deltaTime)
        {
            if (!isPermanent)
                return;
            timeReamining -= deltaTime;
            if (timeReamining <= 0)
                IsFinished = true;
        }

        /// <summary> Обновляет таймер эффекта </summary>
        public void ResetTimer() => timeReamining = duration;
    }
}