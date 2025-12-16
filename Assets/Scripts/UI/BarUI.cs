using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BarUI : MonoBehaviour
{
    [SerializeField] private RectTransform fillTransform;
    private RectTransform backgroundRect;
    private float maxSize;
    public bool isWidth = true;

    private void Awake()
    {
        if (isWidth)
        {
            backgroundRect = GetComponent<RectTransform>();
            maxSize = backgroundRect.rect.width;
        }
        else
        {
            backgroundRect = GetComponent<RectTransform>();
            maxSize = backgroundRect.rect.height;
        }
    }

    public void SetFill(float percentage)
    {
        if (fillTransform != null)
        {
            float newSize = maxSize * percentage;
            if (isWidth)
            {
                fillTransform.sizeDelta = new Vector2(newSize, fillTransform.sizeDelta.y);
            }
            else
            {
                fillTransform.sizeDelta = new Vector2(fillTransform.sizeDelta.x, newSize);
            }
        }
    }
}