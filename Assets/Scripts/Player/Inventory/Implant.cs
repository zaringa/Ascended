/// <summary> Родительский класс для всех имплантов </summary>
public abstract class Implant : Item
{
    /// <summary> Редкость импланта (для малых и гига-имплантов) </summary>
    public enum Rareness { Common, Rare }

    /// <summary> Изменяемый имплантом параметр </summary>
    public object stat;
}