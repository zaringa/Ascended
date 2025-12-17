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
        sizePanel.localScale = new Vector3(size, size, 1);
    }
    void UpdateMargin(float marginWidth, float marginHeight)
    {
        float scale = Screen.height / 1080f;
        float sizePanelScreenWidth = 1920f * scale;
        float excessWidth = Screen.width - sizePanelScreenWidth;
        float localExcessHalf = excessWidth / 2f / scale;

        marginPanel.sizeDelta = new Vector2(localExcessHalf * 2f, 0f);

        marginPanel.anchorMin = new Vector2(0f + marginWidth / 1920f, marginHeight / 1080f);
        marginPanel.anchorMax = new Vector2(1f - marginWidth / 1920f, 1f - marginHeight / 1080f);
    }
    void Update()
    {
        UpdateSize(PanelSize);
        UpdateMargin(marginWidth, marginHeight);
    }
}
