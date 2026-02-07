using UnityEngine;
using TMPro;

public class SpeedDisplay : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    private TextMeshProUGUI textMeshPro;

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        if (playerController != null && textMeshPro != null)
        {
            float speed = playerController.CurrentSpeed;
            textMeshPro.text = speed.ToString("F2");
        }
    }
}