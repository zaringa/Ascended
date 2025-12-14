using UnityEngine;
using System.Collections.Generic;

public class GrapplePoint : MonoBehaviour
{
    // Глобальный список всех активных точек на сцене
    public static List<GrapplePoint> AllPoints = new List<GrapplePoint>();

    [Header("Settings")]
    public float minDistance = 3f;
    public float maxDistance = 25f;
    public float minHeightDifference = 1f; // Игрок должен быть ниже точки на это значение

    [Header("UI")]
    public GameObject hintUI;

    void OnEnable()
    {
        if (!AllPoints.Contains(this))
            AllPoints.Add(this);
    }

    void OnDisable()
    {
        if (AllPoints.Contains(this))
            AllPoints.Remove(this);
    }
    
    // Скрываем UI при выключении объекта
    void OnDestroy()
    {
         if(hintUI != null) hintUI.SetActive(false);
    }
}