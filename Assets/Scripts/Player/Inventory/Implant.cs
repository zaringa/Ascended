/// <summary> Родительский класс для всех имплантов </summary>
public abstract class Implant : Item
{
    /// <summary> Редкость импланта (для малых и гига-имплантов) </summary>
    public enum Rareness { Common, Rare }

    /// <summary> Наличие у метода изменяемого параметра </summary>
    public bool hasStat;

    /// <summary> Изменяемый имплантом параметр (при наличии) </summary>
    public float stat;

    /// <summary> Наличие у метода выполняемого им действия </summary>
    public bool hasAction;

    /// <summary> Производимое имплантом дейтсвие (при наличии) </summary>
    public abstract void Action();
}