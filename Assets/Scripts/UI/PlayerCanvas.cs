using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;
using UnityEngine.Localization.Settings;
[System.Serializable]
public struct localizationVictims
{
  public TMP_Text _tmp;
  public string _code;  
};

[ExecuteAlways]
public class PlayerCanvas : MonoBehaviour
{
    public float marginWidth = 0;
    public float marginHeight = 0;
    public float PanelSize = 1;
    public string LocalizationSource;
    
    public List<localizationVictims> availibleTexts = new List<localizationVictims>();
    [SerializeField] RectTransform sizePanel;
    [SerializeField] RectTransform marginPanel;
    void Start()
    {
        LocalizationSettings.SelectedLocaleChanged += UpdateLocale;
    }
    void UpdateSize(float size)
    {
        if (size <= 0) return;
        sizePanel.sizeDelta = new Vector2(1920 / size, 1080 / size);
        sizePanel.localScale = new Vector3(size, size, size);
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
    void UpdateLocale(Locale newLocale)
    {
        for(int i = 0; i< availibleTexts.Count; i++)
        {
            availibleTexts[i]._tmp.text = LocalizationSettings.StringDatabase.GetLocalizedString(LocalizationSource, availibleTexts[i]._code);
        }
    }
}
