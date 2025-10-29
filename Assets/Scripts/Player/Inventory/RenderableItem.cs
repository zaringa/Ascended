using UnityEngine;

/// <summary> Предмет с 3D моделью </summary>
public abstract class RenderableItem : Item
{
    /// <summary> 3D модель предмета </summary>
    [Header("Визуализация")]
    public GameObject prefab;
}