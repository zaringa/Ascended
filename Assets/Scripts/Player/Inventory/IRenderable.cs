using UnityEngine;

/// <summary> Интерфейс для представления 3D модели </summary>
public interface IRenderable
{
    [Header("Визуализация")]
    /// <summary> 3D модель объекта </summary>
    public GameObject Prefab { get; }
}