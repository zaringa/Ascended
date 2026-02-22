using UnityEngine;
using Player.Items.Implants.Interfaces;
using Player.Movement;

namespace Player.Items.Implants.Base
{
    public enum ImplantRarity { Common, Rare }
    
    /// <summary>
    /// Базовый класс для всех имплантов.
    /// </summary>
    public abstract class Implant : Item, IImplantable
    {
        [Header("Implant Properties")]
        public ImplantRarity rarity = ImplantRarity.Common;
        
        /// <summary>
        /// Активное действие импланта (используется игроком)
        /// </summary>
        public abstract void Action();
        
        /// <summary>
        /// Опционально: переопределение обработчика движения.
        /// Возвращает null по умолчанию (используется стандартный).
        /// </summary>
        public virtual IMovementHandler GetMovementHandler() => null;
    }
    
    /// <summary>
    /// Имплант первого слоя под руки
    /// </summary>
    public abstract class ArmsImplant : Implant { }
    
    /// <summary>
    /// Имплант первого слоя под тело
    /// </summary>
    public abstract class BodyImplant : Implant { }
    
    /// <summary>
    /// Имплант первого слоя под ноги
    /// </summary>
    public abstract class LegsImplant : Implant { }
    
    /// <summary>
    /// Малый имплант второго слоя
    /// </summary>
    public abstract class SmallImplant : Implant { }
    
    /// <summary>
    /// Гига-имплант третьего слоя
    /// </summary>
    public abstract class GigaImplant : Implant { }
}
