using UnityEngine;

[ExecuteAlways]
public class PlayerCanvas : MonoBehaviour
{
    public float marginWidth = 0;
    public float marginHeight = 0;
    public float PanelSize = 1;
    [SerializeField] RectTransform sizePanel;
    [SerializeField] RectTransform marginPanel;
    void UpdateSize(float size)
    {
        if (size <= 0) return;
        sizePanel.sizeDelta = new Vector2(1920 / size, 1080 / size);
        sizePanel.localScale = new Vector3(size, size, 1);
    }
    void UpdateMargin(float marginWidth, float marginHeight)
    {
        float scale = Screen.height / sizePanel.sizeDelta.y;
        float sizePanelScreenWidth = sizePanel.sizeDelta.x * scale;
        float excessWidth = Screen.width - sizePanelScreenWidth;
        float localExcessHalf = excessWidth / 2f / scale;

        marginPanel.sizeDelta = new Vector2(localExcessHalf * 2f, 0f);

        marginPanel.anchorMin = new Vector2(0f + marginWidth / sizePanel.sizeDelta.x, marginHeight / sizePanel.sizeDelta.y);
        marginPanel.anchorMax = new Vector2(1f - marginWidth / sizePanel.sizeDelta.x, 1f - marginHeight / sizePanel.sizeDelta.y);
    }
    void Update()
    {
        UpdateSize(PanelSize);
        UpdateMargin(marginWidth, marginHeight);
    }
}
