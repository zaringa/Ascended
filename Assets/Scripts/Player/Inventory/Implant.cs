using UnityEngine;

/// <summary> Родительский класс для всех имплантов </summary>
public abstract class Implant : Item
{
    [Header("Характеристики импланта")]
    /// <summary> Наличие у импланта изменяемого параметра </summary>
    public bool hasStat;

    /// <summary> Тип измнняемого параметра </summary>
    public enum Stat
    {
        Health,
        AttackDamage,
        MeleeDamage,
        BulletsDamage,
        RunSpeed,
        JumpHeight,
        DashSpeed
    }

    [SerializeField] private Stat statType;

    /// <summary> Тип изменяемого параметра </summary>
    public Stat StatType { get => statType; }

    /// <summary> Изменяемый имплантом параметр </summary>
    public float stat;

    /// <summary> Наличие у метода выполняемого им действия </summary>
    public bool hasAction;

    /// <summary> Тип выполняемого действия </summary>
    public enum Action
    {

    }

    [SerializeField] private Action actionType;

    /// <summary> Тип выполняемого действия </summary>
    public Action ActionType { get => actionType; }

    /// <summary> Выполняемое имплантом дейтсвие </summary>
    public abstract void ImplantAction();
}