using UnityEngine;

namespace Player.Items
{
    /// <summary> Любой отображаемый в UI предмет </summary>
    public abstract class Item : ScriptableObject
    {
        [Header("Основные характеристики")]
        public new string name;

        public Sprite sprite;

        [TextArea(3, 5)]
        public string description;
    }
}
