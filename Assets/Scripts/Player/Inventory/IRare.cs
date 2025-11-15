using UnityEngine;

/// <summary> Интерфейс для представления редкости предметов </summary>
public interface IRare
{
    [HideInInspector] public enum Rareness { Common, Rare }

    [Header("Редкость")]
    /// <summary> Редкость </summary>
    public abstract Rareness RarenessType { get; }
}