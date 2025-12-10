using UnityEngine;

public class CooldownBarUI : MonoBehaviour
{
    [SerializeField] private BarUI barUI;
    [SerializeField] public CooldownSystem cooldownSystem;

    private void Update()
    {
        if (cooldownSystem != null && barUI != null)
        {
            barUI.SetFill(cooldownSystem.Percentage);
            Debug.Log("" + cooldownSystem.Percentage);
        }
    }
}