using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BarUI : MonoBehaviour
{
    [SerializeField] private RectTransform fillTransform;
    private RectTransform backgroundRect;
    private float maxWidth;

    private void Awake()
    {
        backgroundRect = GetComponent<RectTransform>();
        maxWidth = backgroundRect.rect.width;
    }

    public void SetFill(float percentage)
    {
        if (fillTransform != null)
        {
            float newWidth = maxWidth * percentage;
            fillTransform.sizeDelta = new Vector2(newWidth, fillTransform.sizeDelta.y);
        }
    }
}