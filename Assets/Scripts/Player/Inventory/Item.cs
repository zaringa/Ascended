using UnityEngine;

/// <summary> Любой отображаемый в UI предмет </summary>
public abstract class Item : ScriptableObject
{
    /// <summary> Наименование предмета </summary>
    [Header("Основные характеристики")]
    public new string name;

    /// <summary> Спрайт предмета </summary>
    public Sprite sprite;

    /// <summary> Описание предмета </summary>
    [TextArea(3, 5)]
    public string description;
}