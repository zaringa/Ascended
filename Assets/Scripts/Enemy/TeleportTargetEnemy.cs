using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class TeleportTargetEnemy : BaseEnemy
    {
        [Header("Teleport Settings")]
        [SerializeField] private float teleportRadius = 10f;
        [SerializeField] private float teleportInterval = 3f;
        [SerializeField] private bool teleportOnDamage = false;
        [SerializeField] private GameObject teleportEffectPrefab;

        private float nextTeleportTime;

        protected override void Start()
        {
            base.Start();
            currentState = EnemyState.Idle;
            agent.speed = 0;
            nextTeleportTime = Time.time + teleportInterval;
        }

        protected override void UpdateBehavior(float distanceToPlayer)
        {
            if (Time.time >= nextTeleportTime)
            {
                Teleport();
                nextTeleportTime = Time.time + teleportInterval;
            }
        }

        private void Teleport()
        {
            Vector2 randomCircle = Random.insideUnitCircle * teleportRadius;
            Vector3 randomPoint = startPosition + new Vector3(randomCircle.x, 0, randomCircle.y);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, teleportRadius, NavMesh.AllAreas))
            {
                if (teleportEffectPrefab != null)
                {
                    GameObject effect = Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
                    Destroy(effect, 2f);
                }

                transform.position = hit.position;

                if (teleportEffectPrefab != null)
                {
                    GameObject effect = Instantiate(teleportEffectPrefab, hit.position, Quaternion.identity);
                    Destroy(effect, 2f);
                }
            }
        }

        public override void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
        {
            base.TakeDamage(damage, hitPoint, hitNormal);

            if (IsAlive() && teleportOnDamage)
            {
                Teleport();
                nextTeleportTime = Time.time + teleportInterval;
            }
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Gizmos.color = Color.magenta;
            Vector3 teleportCenter = Application.isPlaying ? startPosition : transform.position;
            Gizmos.DrawWireSphere(teleportCenter, teleportRadius);

            Gizmos.color = Color.cyan;
            Vector3 teleportPos = transform.position + Vector3.up * 2.5f;
            for (int i = 0; i < 8; i++)
            {
                float angle = i * 45f * Mathf.Deg2Rad;
                float radius = 0.15f + (i * 0.03f);
                Vector3 point = teleportPos + new Vector3(Mathf.Cos(angle) * radius, i * 0.05f, Mathf.Sin(angle) * radius);
                Gizmos.DrawSphere(point, 0.03f);
            }
        }
    }
}
