using UnityEngine;
using Enemy;
using System.Collections.Generic;

/// <summary>
/// Менеджер тренировочной комнаты для управления мишенями
/// </summary>
public class TrainingRoomManager : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private int initialTargetCount = 5;
    [SerializeField] private float spawnRadius = 10f;

    [Header("Auto Respawn")]
    [SerializeField] private bool autoRespawn = true;
    [SerializeField] private int minTargetCount = 3;
    [SerializeField] private float respawnDelay = 1f;

    [Header("Statistics")]
    [SerializeField] private bool showStats = true;

    private List<TargetEnemy> activeTargets = new List<TargetEnemy>();
    private int totalHits = 0;
    private float totalDamage = 0f;
    private float sessionStartTime;
    private float lastRespawnTime;

    private void Start()
    {
        sessionStartTime = Time.time;
        SpawnInitialTargets();
    }

    private void SpawnInitialTargets()
    {
        if (targetPrefab == null)
        {
            Debug.LogWarning("TrainingRoomManager: Target prefab not assigned!");
            return;
        }

        for (int i = 0; i < initialTargetCount; i++)
        {
            SpawnTarget();
        }
    }

    public void SpawnTarget()
    {
        if (targetPrefab == null) return;

        Vector3 spawnPos = transform.position + Random.insideUnitSphere * spawnRadius;
        spawnPos.y = transform.position.y + 1.5f;

        GameObject targetObj = Instantiate(targetPrefab, spawnPos, Quaternion.identity);
        TargetEnemy target = targetObj.GetComponent<TargetEnemy>();

        if (target != null)
        {
            activeTargets.Add(target);
        }
    }

    public void RemoveDestroyedTargets()
    {
        activeTargets.RemoveAll(t => t == null);
    }

    public void ResetAllTargets()
    {
        foreach (var target in activeTargets)
        {
            if (target != null)
            {
                target.ResetStats();
            }
        }

        totalHits = 0;
        totalDamage = 0f;
        sessionStartTime = Time.time;
    }

    public void ClearAllTargets()
    {
        foreach (var target in activeTargets)
        {
            if (target != null)
            {
                Destroy(target.gameObject);
            }
        }

        activeTargets.Clear();
        totalHits = 0;
        totalDamage = 0f;
    }

    private void Update()
    {
        RemoveDestroyedTargets();
        UpdateStats();

        // Автоматический респавн
        if (autoRespawn && activeTargets.Count < minTargetCount)
        {
            if (Time.time >= lastRespawnTime + respawnDelay)
            {
                SpawnTarget();
                lastRespawnTime = Time.time;
            }
        }
    }

    private void UpdateStats()
    {
        totalHits = 0;
        totalDamage = 0f;

        foreach (var target in activeTargets)
        {
            if (target != null)
            {
                totalHits += target.GetHitCount();
                totalDamage += target.GetTotalDamage();
            }
        }
    }

    private void OnGUI()
    {
        if (!showStats) return;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 20;
        style.normal.textColor = Color.white;

        float sessionTime = Time.time - sessionStartTime;
        int minutes = Mathf.FloorToInt(sessionTime / 60f);
        int seconds = Mathf.FloorToInt(sessionTime % 60f);

        string stats = $"Training Room Statistics\n" +
                      $"Active Targets: {activeTargets.Count}\n" +
                      $"Total Hits: {totalHits}\n" +
                      $"Total Damage: {totalDamage:F0}\n" +
                      $"Session Time: {minutes:00}:{seconds:00}";

        if (totalHits > 0)
        {
            stats += $"\nAvg Damage/Hit: {(totalDamage / totalHits):F1}";
        }

        GUI.Label(new Rect(10, 10, 400, 200), stats, style);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

    // Public methods for external control (можно вызывать через кнопки или триггеры)
    public int GetActiveTargetCount() => activeTargets.Count;
    public int GetTotalHits() => totalHits;
    public float GetTotalDamage() => totalDamage;
}
