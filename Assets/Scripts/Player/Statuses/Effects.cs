using System.Collections.Generic;
using UnityEngine;

namespace Player.Effects
{
    /// <summary> Все возможные накладываемые эффекты </summary>
    public enum Effects
    {
        CraftSpeedMultiplier,
        DamageResistMultiplier,
        RunSpeedMultiplier,
        PrimaryWeaponFireRateMultiplier,
        SecondaryWeaponFireRateMultiplier,
        PrimaryWeaponReloadingSpeedMultiplier,
        SecondaryWeaponReloadingSpeedMultiplier,
        DamageOverTimeAmount
    }

    /// <summary> Хранит изменяемое эффектом значение: тип и числовое значение </summary>
    public struct DictionaryEffect
    {
        float value;
        public float Value => value;

        Effects effect;
        public Effects Effect => effect;

        public DictionaryEffect(Effects effect, ref float value)
        {
            this.effect = effect;
            this.value = value;
        }
    }

    [RequireComponent(typeof(Inventory.Inventory))]
    public class Effect : MonoBehaviour
    {
        /// <summary> Список активных эффектов </summary>
        private List<StatusEffect> activeEffects;
        /// <summary> Список изменяемых эффектами параметров </summary>
        public List<DictionaryEffect> effects;

        /// <summary> Устанавливает список параметров необходимыми ссылками и значениями параметров </summary>
        void Start()
        {
            var inventory = GetComponent<Inventory.Inventory>();
            //effects.Add(Effects.RunSpeedMultiplier,
            //    new(ref PlayerController.movementSpeed));
            effects.Add(new(Effects.PrimaryWeaponFireRateMultiplier,
                ref inventory.GetGunInfo(true).fireRate));
            effects.Add(new(Effects.SecondaryWeaponFireRateMultiplier,
                ref inventory.GetGunInfo(false).fireRate));
            effects.Add(new(Effects.PrimaryWeaponReloadingSpeedMultiplier,
                ref inventory.GetGunInfo(true).reloadTime));
            effects.Add(new(Effects.SecondaryWeaponReloadingSpeedMultiplier,
                ref inventory.GetGunInfo(false).reloadTime));
        }

        /// <summary> Обновляет таймеры и значения всех наложенных эффектов (при необходимости - снимает эффекты с истёкшим таймером) </summary>
        void Update()
        {
            foreach (var effect in activeEffects)
            {
                effect.UpdateEffect(Time.deltaTime);
                if (effect.IsFinished)
                    RemoveEffect(effect);
            }
        }

        /// <summary> Добавляет эффект </summary>
        /// <param name="effect"> Добавляемый эффект </param>
        public void AddEffect(StatusEffect effect)
        {
            if (effect.isStackable || !activeEffects.Exists(e => e.GetType() == effect.GetType()))
            {
                activeEffects.Add(effect);
                effect.ApplyEffect(this);
            }
            else if (!effect.isStackable)
            {
                var existingEffect = activeEffects.Find(e => e.GetType() == effect.GetType());
                existingEffect?.ResetTimer();
            }
        }

        /// <summary> Убирает эффект </summary>
        /// <param name="effect"> Убираемый эффект </param>
        public void RemoveEffect(StatusEffect effect)
        {
            effect.RemoveEffect(this);
            activeEffects.Remove(effect);
        }

        /// <summary> Обновляет все значения параметров в соответствии с наложенными эффектами </summary>
        public void RecalculateStats()
        {
            // Установка всех параметров в значения по умолчанию
            foreach (var effect in activeEffects)
                effect.ApplyEffect(this);
        }
    }
}