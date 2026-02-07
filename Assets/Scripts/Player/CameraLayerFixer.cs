using UnityEngine;

/// <summary>
/// Автоматически включает Layer "Projectile" в Culling Mask камеры.
/// Прикрепите этот скрипт к камере или к объекту Player.
/// </summary>
public class CameraLayerFixer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool autoFindCamera = true;
    [SerializeField] private Camera targetCamera;

    [Header("Layers to Enable")]
    [SerializeField] private string[] layersToEnable = new string[] { "Projectile" };

    void Start()
    {
        // Автопоиск камеры
        if (autoFindCamera && targetCamera == null)
        {
            targetCamera = Camera.main;

            if (targetCamera == null)
            {
                targetCamera = GetComponent<Camera>();
            }

            if (targetCamera == null)
            {
                targetCamera = GetComponentInChildren<Camera>();
            }
        }

        if (targetCamera == null)
        {
            //Debug.LogError("[CameraLayerFixer] Camera not found! Please assign it manually.");
            return;
        }

        EnableLayers();
    }

    void EnableLayers()
    {
        //Debug.Log($"[CameraLayerFixer] Starting EnableLayers for camera: {targetCamera.name}");
       // Debug.Log($"[CameraLayerFixer] Current Culling Mask: {targetCamera.cullingMask} (binary: {System.Convert.ToString(targetCamera.cullingMask, 2)})");

        foreach (string layerName in layersToEnable)
        {
            int layerIndex = LayerMask.NameToLayer(layerName);

            if (layerIndex == -1)
            {
                //Debug.LogWarning($"[CameraLayerFixer] Layer '{layerName}' not found in project settings.");
                //Debug.LogWarning($"[CameraLayerFixer] Available layers:");
                for (int i = 0; i < 32; i++)
                {
                    string name = LayerMask.LayerToName(i);
                    if (!string.IsNullOrEmpty(name))
                    {
                        Debug.Log($"  Layer {i}: {name}");
                    }
                }
                continue;
            }

            // Проверяем, включен ли уже этот слой
            bool isLayerEnabled = (targetCamera.cullingMask & (1 << layerIndex)) != 0;

           // Debug.Log($"[CameraLayerFixer] Layer '{layerName}' (index {layerIndex}): currently {(isLayerEnabled ? "ENABLED" : "DISABLED")}");

            if (!isLayerEnabled)
            {
                // Включаем слой в Culling Mask
                int oldMask = targetCamera.cullingMask;
                targetCamera.cullingMask |= (1 << layerIndex);
                int newMask = targetCamera.cullingMask;

               // Debug.LogWarning($"[CameraLayerFixer] ENABLED layer '{layerName}' (index {layerIndex}) for camera '{targetCamera.name}'");
                //Debug.Log($"[CameraLayerFixer] Culling mask changed: {oldMask} -> {newMask}");
            }
            else
            {
                //Debug.Log($"[CameraLayerFixer] Layer '{layerName}' is already enabled for camera '{targetCamera.name}'");
            }
        }

        //Debug.Log($"[CameraLayerFixer] Final Culling Mask: {targetCamera.cullingMask}");
    }
    
    void OnValidate()
    {
        if (Application.isPlaying && targetCamera != null)
        {
            EnableLayers();
        }
    }
}
