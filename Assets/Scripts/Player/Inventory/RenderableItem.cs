using UnityEngine;

/// <summary> Предмет с 3D моделью </summary>
public abstract class RenderableItem : Item
{
    [Header("Визуализация")]
    /// <summary> 3D модель предмета </summary>
    public GameObject prefab;
}