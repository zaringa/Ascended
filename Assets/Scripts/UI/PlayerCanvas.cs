using UnityEngine;

public class PlayerCanvas : MonoBehaviour
{
    public float marginWidth = 0;
    public float marginHeight = 0;
    public float PanelSize = 1;
    [SerializeField] RectTransform sizePanel;
    [SerializeField] RectTransform marginPanel;
    void UpdateSize(float size)
    {
        sizePanel.sizeDelta = new Vector2(1920 / size, 1080 / size);
        sizePanel.localScale = new Vector2(size, size);
    }
    void UpdateMargin(float marginWidth, float marginHeight)
    {
        marginPanel.anchorMin = new Vector2(marginWidth / 1920 / 2, marginHeight / 1080 / 2);
        marginPanel.anchorMax = new Vector2(1 - marginWidth / 1920 / 2, 1 - marginHeight / 1080 / 2);
    }
    void Update()
    {
        UpdateSize(PanelSize);
        UpdateMargin(marginWidth, marginHeight);
    }
}
