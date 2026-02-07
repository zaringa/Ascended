using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    /// <summary>
    /// Тренировочная мишень, которая телепортируется при попадании
    /// </summary>
    public class TargetEnemy : BaseEnemy
    {
        [Header("Teleport Settings")]
        [SerializeField] private bool teleportOnHit = true;
        [SerializeField] private float teleportRadius = 10f;
        [SerializeField] private bool canTeleportToWalls = true;
        [SerializeField] private bool restoreHealthOnTeleport = true;
        [SerializeField] private GameObject teleportEffectPrefab;
        [SerializeField] private LayerMask wallLayers = -1;

        [Header("Training Statistics")]
        [SerializeField] private int hitCount = 0;
        [SerializeField] private float totalDamageTaken = 0f;

        private bool isTeleporting = false;
        private WallSpawner wallSpawner; // Ссылка на WallSpawner если мишень на стене

        protected override void Start()
        {
            base.Start();
            currentState = EnemyState.Idle;

            if (agent != null)
            {
                agent.speed = 0;
                agent.enabled = !canTeleportToWalls;
            }
        }

        protected override void UpdateBehavior(float distanceToPlayer)
        {
            // Мишень не двигается сама
        }

        public override void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
        {
            if (!IsAlive() || isTeleporting) return;

            hitCount++;
            totalDamageTaken += damage;

            base.TakeDamage(damage, hitPoint, hitNormal);

            if (IsAlive() && teleportOnHit)
            {
                TeleportToNewPosition();
            }
        }

        private void TeleportToNewPosition()
        {
            if (isTeleporting) return;
            isTeleporting = true;

            // Эффект на старой позиции
            if (teleportEffectPrefab != null)
            {
                GameObject effect = Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }

            // Если есть WallSpawner - используем его для телепортации в пределах стены
            if (wallSpawner != null)
            {
                wallSpawner.TeleportTarget(this);
            }
            else
            {
                // Стандартная телепортация
                if (canTeleportToWalls)
                {
                    Vector3 newPosition;
                    Quaternion newRotation;

                    if (TryFindWallPosition(out newPosition, out newRotation))
                    {
                        transform.position = newPosition;
                        transform.rotation = newRotation;
                    }
                    else
                    {
                        TeleportToGround();
                    }
                }
                else
                {
                    TeleportToGround();
                }
            }

            // Восстановление здоровья
            if (restoreHealthOnTeleport)
            {
                currentHealth = maxHealth;
            }

            // Эффект на новой позиции
            if (teleportEffectPrefab != null)
            {
                GameObject effect = Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }

            isTeleporting = false;
        }

        private bool TryFindWallPosition(out Vector3 position, out Quaternion rotation)
        {
            position = transform.position;
            rotation = transform.rotation;

            int maxAttempts = 20;
            for (int i = 0; i < maxAttempts; i++)
            {
                Vector3 randomDirection = Random.onUnitSphere;
                Vector3 origin = startPosition + randomDirection * teleportRadius * 0.5f;

                RaycastHit hit;
                if (Physics.Raycast(origin, randomDirection, out hit, teleportRadius, wallLayers))
                {
                    float angle = Vector3.Angle(hit.normal, Vector3.up);

                    // Стены и потолки (угол > 30 градусов от земли)
                    if (angle > 30f)
                    {
                        position = hit.point + hit.normal * 0.5f;
                        rotation = Quaternion.LookRotation(-hit.normal);
                        return true;
                    }
                }
            }

            return false;
        }

        private void TeleportToGround()
        {
            Vector2 randomCircle = Random.insideUnitCircle * teleportRadius;
            Vector3 randomPoint = startPosition + new Vector3(randomCircle.x, 2f, randomCircle.y);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, teleportRadius, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            }
            else
            {
                RaycastHit groundHit;
                if (Physics.Raycast(randomPoint, Vector3.down, out groundHit, 10f))
                {
                    transform.position = groundHit.point + Vector3.up * 0.1f;
                    transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                }
            }
        }

        protected override void Die()
        {
            Debug.Log($"Training Target Stats - Hits: {hitCount}, Total Damage: {totalDamageTaken:F0}");
            base.Die();
        }

        public void ResetStats()
        {
            hitCount = 0;
            totalDamageTaken = 0f;
            currentHealth = maxHealth;
        }

        public int GetHitCount() => hitCount;
        public float GetTotalDamage() => totalDamageTaken;

        /// <summary>
        /// Устанавливает WallSpawner для управления телепортацией
        /// </summary>
        public void SetWallSpawner(WallSpawner spawner)
        {
            wallSpawner = spawner;
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            // Индикатор мишени
            Gizmos.color = Color.white;
            Vector3 targetPos = transform.position + Vector3.up * 2f;
            Gizmos.DrawWireSphere(targetPos, 0.3f);
            Gizmos.DrawSphere(targetPos, 0.05f);

            // Радиус телепортации
            if (teleportOnHit)
            {
                Gizmos.color = Color.cyan;
                Vector3 teleportCenter = Application.isPlaying ? startPosition : transform.position;
                Gizmos.DrawWireSphere(teleportCenter, teleportRadius);
            }
        }
    }
}
