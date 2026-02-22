using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// Позволяет управляться игроком.
/// </summary>
public interface IControllable
{
    public Dictionary<string, InputActionReference> Actions { get; }
}