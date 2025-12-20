using UnityEngine;
using Enemy;
using System.Collections.Generic;

/// <summary>
/// Спавнер мишеней на конкретной стене
/// Размещается на GameObject стены и спавнит мишени на её поверхности
/// </summary>
public class WallSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private int spawnCount = 5;
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(10f, 5f); // Ширина и высота области спавна
    [SerializeField] private float offsetFromWall = 0.5f; // Отступ от стены

    [Header("Spacing")]
    [SerializeField] private float minDistanceBetweenTargets = 2f;
    [SerializeField] private int maxSpawnAttempts = 30;

    [Header("Auto Spawn")]
    [SerializeField] private bool autoRespawn = true;
    [SerializeField] private float respawnDelay = 1f;

    private List<TargetEnemy> spawnedTargets = new List<TargetEnemy>();
    private List<Vector3> spawnedPositions = new List<Vector3>();

    private void Start()
    {
        SpawnInitialTargets();
    }

    private void Update()
    {
        if (autoRespawn)
        {
            CleanupDestroyedTargets();

            // Проверяем, нужно ли респавнить
            if (spawnedTargets.Count < spawnCount)
            {
                SpawnTarget();
            }
        }
    }

    private void SpawnInitialTargets()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnTarget();
        }
    }

    public void SpawnTarget()
    {
        if (targetPrefab == null)
        {
            Debug.LogWarning("WallSpawner: Target prefab not assigned!");
            return;
        }

        Vector3 spawnPosition;
        Quaternion spawnRotation;

        if (TryFindSpawnPosition(out spawnPosition, out spawnRotation))
        {
            GameObject targetObj = Instantiate(targetPrefab, spawnPosition, spawnRotation);
            TargetEnemy target = targetObj.GetComponent<TargetEnemy>();

            if (target != null)
            {
                spawnedTargets.Add(target);
                spawnedPositions.Add(spawnPosition);

                // Отключаем NavMeshAgent для мишеней на стене
                var agent = target.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent != null)
                {
                    agent.enabled = false;
                }

                // Устанавливаем ссылку на спавнер для телепортации
                target.SetWallSpawner(this);
            }
        }
    }

    private bool TryFindSpawnPosition(out Vector3 position, out Quaternion rotation)
    {
        position = transform.position;
        rotation = transform.rotation;

        Vector3 wallNormal = transform.forward; // Нормаль стены
        Vector3 wallRight = transform.right;    // Вправо по стене
        Vector3 wallUp = transform.up;          // Вверх по стене

        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            // Случайная позиция в пределах области спавна
            float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
            float randomY = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);

            Vector3 candidatePosition = transform.position
                + wallRight * randomX
                + wallUp * randomY
                + wallNormal * offsetFromWall;

            // Проверяем минимальное расстояние от других мишеней
            bool tooClose = false;
            foreach (Vector3 existingPos in spawnedPositions)
            {
                if (Vector3.Distance(candidatePosition, existingPos) < minDistanceBetweenTargets)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                position = candidatePosition;
                rotation = Quaternion.LookRotation(-wallNormal); // Смотрим от стены
                return true;
            }
        }

        Debug.LogWarning("WallSpawner: Could not find valid spawn position after max attempts");
        return false;
    }

    private void CleanupDestroyedTargets()
    {
        // Удаляем уничтоженные мишени из списка
        for (int i = spawnedTargets.Count - 1; i >= 0; i--)
        {
            if (spawnedTargets[i] == null)
            {
                spawnedTargets.RemoveAt(i);
                if (i < spawnedPositions.Count)
                {
                    spawnedPositions.RemoveAt(i);
                }
            }
        }
    }

    public void ClearAllTargets()
    {
        foreach (var target in spawnedTargets)
        {
            if (target != null)
            {
                Destroy(target.gameObject);
            }
        }

        spawnedTargets.Clear();
        spawnedPositions.Clear();
    }

    public void ResetAllTargets()
    {
        foreach (var target in spawnedTargets)
        {
            if (target != null)
            {
                target.ResetStats();
            }
        }
    }

    /// <summary>
    /// Телепортирует конкретную мишень в новую позицию на стене
    /// </summary>
    public bool TeleportTarget(TargetEnemy target)
    {
        if (target == null) return false;

        Vector3 newPosition;
        Quaternion newRotation;

        // Находим индекс мишени
        int targetIndex = spawnedTargets.IndexOf(target);
        if (targetIndex < 0) return false;

        // Временно убираем старую позицию из списка
        Vector3 oldPosition = Vector3.zero;
        if (targetIndex < spawnedPositions.Count)
        {
            oldPosition = spawnedPositions[targetIndex];
            spawnedPositions.RemoveAt(targetIndex);
        }

        // Ищем новую позицию
        if (TryFindSpawnPosition(out newPosition, out newRotation))
        {
            target.transform.position = newPosition;
            target.transform.rotation = newRotation;

            // Обновляем позицию в списке
            if (targetIndex < spawnedPositions.Count)
            {
                spawnedPositions[targetIndex] = newPosition;
            }
            else
            {
                spawnedPositions.Add(newPosition);
            }

            return true;
        }
        else
        {
            // Если не нашли новую позицию, возвращаем старую
            if (oldPosition != Vector3.zero)
            {
                spawnedPositions.Insert(targetIndex, oldPosition);
            }
            return false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Рисуем область спавна
        Gizmos.color = Color.green;

        Vector3 wallNormal = transform.forward;
        Vector3 wallRight = transform.right;
        Vector3 wallUp = transform.up;

        Vector3 center = transform.position + wallNormal * offsetFromWall;

        // Четыре угла области спавна
        Vector3 topLeft = center - wallRight * (spawnAreaSize.x / 2) + wallUp * (spawnAreaSize.y / 2);
        Vector3 topRight = center + wallRight * (spawnAreaSize.x / 2) + wallUp * (spawnAreaSize.y / 2);
        Vector3 bottomLeft = center - wallRight * (spawnAreaSize.x / 2) - wallUp * (spawnAreaSize.y / 2);
        Vector3 bottomRight = center + wallRight * (spawnAreaSize.x / 2) - wallUp * (spawnAreaSize.y / 2);

        // Рисуем прямоугольник
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        // Рисуем нормаль стены
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(center, wallNormal * 2f);

        // Показываем позиции спавна
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            foreach (Vector3 pos in spawnedPositions)
            {
                Gizmos.DrawWireSphere(pos, 0.3f);
            }
        }
    }

    // Public getters
    public int GetActiveTargetCount() => spawnedTargets.Count;
    public int GetTotalHits()
    {
        int total = 0;
        foreach (var target in spawnedTargets)
        {
            if (target != null)
            {
                total += target.GetHitCount();
            }
        }
        return total;
    }

    public float GetTotalDamage()
    {
        float total = 0;
        foreach (var target in spawnedTargets)
        {
            if (target != null)
            {
                total += target.GetTotalDamage();
            }
        }
        return total;
    }
}
