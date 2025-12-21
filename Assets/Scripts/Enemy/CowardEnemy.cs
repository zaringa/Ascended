using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class CowardEnemy : BaseEnemy
    {
        [Header("Flee Settings")]
        [SerializeField] private float fleeDistance = 20f;
        [SerializeField] private float safeDistance = 15f;

        protected override void Start()
        {
            base.Start();
            currentState = EnemyState.Idle;
        }

        protected override void UpdateBehavior(float distanceToPlayer)
        {
            switch (currentState)
            {
                case EnemyState.Idle:
                    HandleIdleState(distanceToPlayer);
                    break;
                case EnemyState.Flee:
                    HandleFleeState(distanceToPlayer);
                    break;
            }
        }

        private void HandleIdleState(float distanceToPlayer)
        {
            if (distanceToPlayer < detectionRange)
            {
                ChangeState(EnemyState.Flee);
            }
        }

        private void HandleFleeState(float distanceToPlayer)
        {
            if (distanceToPlayer > safeDistance)
            {
                ChangeState(EnemyState.Idle);
                return;
            }

            Vector3 fleeDirection = (transform.position - player.position).normalized;
            Vector3 fleeTarget = transform.position + fleeDirection * fleeDistance;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(fleeTarget, out hit, fleeDistance, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            else
            {
                Vector3 rightFlee = transform.position + (fleeDirection + transform.right).normalized * fleeDistance;
                Vector3 leftFlee = transform.position + (fleeDirection - transform.right).normalized * fleeDistance;

                if (NavMesh.SamplePosition(rightFlee, out hit, fleeDistance, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
                else if (NavMesh.SamplePosition(leftFlee, out hit, fleeDistance, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
            }
        }

        protected override void OnStateChanged(EnemyState newState)
        {
            switch (newState)
            {
                case EnemyState.Idle:
                    agent.ResetPath();
                    break;
                case EnemyState.Flee:
                    agent.speed = 5f;
                    agent.stoppingDistance = 0f;
                    break;
            }
        }

        public override void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
        {
            base.TakeDamage(damage, hitPoint, hitNormal);

            if (IsAlive() && currentState == EnemyState.Idle)
            {
                ChangeState(EnemyState.Flee);
            }
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, safeDistance);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, fleeDistance);
        }
    }
}
