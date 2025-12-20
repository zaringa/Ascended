using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class WorldMarkerUI : NetworkBehaviour
{
    [Header("UI Settings")]
    public RawImage markerUIPrefab;
    public Vector2 uiOffset = new Vector2(0, 50);
    
    [Header("Target Settings")]
    public Transform targetMarker;
    
    private RawImage currentMarkerUI;
    private RectTransform canvasRect;
    private Camera mainCamera;
    
    private void Start()
    {
        if (targetMarker == null)
            targetMarker = transform;
            
        mainCamera = Camera.main;
        
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
            
            if (IsLocalPlayer || (IsServer && IsOwner))
            {
                CreateMarkerUI();
            }
        }
    }
    
    private void CreateMarkerUI()
    {
        if (markerUIPrefab != null && canvasRect != null)
        {
            currentMarkerUI = Instantiate(markerUIPrefab, canvasRect.transform);
            currentMarkerUI.rectTransform.SetAsFirstSibling();
            
            UpdateUIPosition();
        }
    }
    
    private void Update()
    {
        if (currentMarkerUI != null && mainCamera != null)
        {
            UpdateUIPosition();
        }
    }
    
    private void UpdateUIPosition()
    {

        Vector3 screenPos = mainCamera.WorldToScreenPoint(targetMarker.position);
        
        if (screenPos.z > 0)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, 
                screenPos, 
                null, 
                out localPoint
            );
            
            currentMarkerUI.rectTransform.localPosition = localPoint + uiOffset;
            currentMarkerUI.gameObject.SetActive(true);
        }
        else
        {
            currentMarkerUI.gameObject.SetActive(false);
        }
    }
    
    public override void OnNetworkDespawn()
    {
        if (currentMarkerUI != null)
        {
            Destroy(currentMarkerUI.gameObject);
        }
        
        base.OnNetworkDespawn();
    }
    
    public void SetTargetMarker(Transform newTarget)
    {
        targetMarker = newTarget;
    }
}

public class MarkerUIManager : NetworkBehaviour
{
    [Header("UI Settings")]
    public GameObject markerPrefab;
    public int poolSize = 20;
    
    [Header("Visual Settings")]
    public Texture[] markerTextures;
    public Color[] markerColors;
    
    private RawImage[] markerPool;
    private bool[] markerInUse;
    private Transform[] markerTargets;
    private Canvas canvas;
    private Camera mainCamera;
    
    private void Start()
    {
        mainCamera = Camera.main;
        canvas = FindObjectOfType<Canvas>();
        
        if (canvas != null && markerPrefab != null)
        {
            InitializePool();
        }
    }
    
    private void InitializePool()
    {
        markerPool = new RawImage[poolSize];
        markerInUse = new bool[poolSize];
        markerTargets = new Transform[poolSize];
        
        for (int i = 0; i < poolSize; i++)
        {
            GameObject markerObj = Instantiate(markerPrefab, canvas.transform);
            markerPool[i] = markerObj.GetComponent<RawImage>();
            markerPool[i].gameObject.SetActive(false);
        }
    }
    
    public int RequestMarker(Transform target, int textureIndex = 0, int colorIndex = 0)
    {
        for (int i = 0; i < poolSize; i++)
        {
            if (!markerInUse[i])
            {
                markerInUse[i] = true;
                markerTargets[i] = target;
                
                if (textureIndex < markerTextures.Length && markerTextures[textureIndex] != null)
                {
                    markerPool[i].texture = markerTextures[textureIndex];
                }
                
                if (colorIndex < markerColors.Length)
                {
                    markerPool[i].color = markerColors[colorIndex];
                }
                
                markerPool[i].gameObject.SetActive(true);
                return i; 
            }
        }
        
        return -1; 
    }
    
    public void ReleaseMarker(int markerId)
    {
        if (markerId >= 0 && markerId < poolSize && markerInUse[markerId])
        {
            markerInUse[markerId] = false;
            markerTargets[markerId] = null;
            markerPool[markerId].gameObject.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (!IsClient) return;
        
        for (int i = 0; i < poolSize; i++)
        {
            if (markerInUse[i] && markerTargets[i] != null)
            {
                UpdateMarkerPosition(i);
            }
        }
    }
    
    private void UpdateMarkerPosition(int index)
    {
        if (mainCamera == null || markerTargets[index] == null) return;
        
        Vector3 screenPos = mainCamera.WorldToScreenPoint(markerTargets[index].position);
        
        if (screenPos.z > 0)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPos,
                null,
                out localPoint
            );
            
            markerPool[index].rectTransform.localPosition = localPoint;
        }
        else
        {
            markerPool[index].rectTransform.localPosition = new Vector2(10000, 10000);
        }
    }
}