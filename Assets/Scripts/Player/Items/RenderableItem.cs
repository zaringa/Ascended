using UnityEngine;

namespace Player.Items
{
    /// <summary> Предмет с 3D моделью </summary>
    public abstract class RenderableItem : Item
    {
        [Header("Визуализация")]
        public GameObject prefab;
    }
}
