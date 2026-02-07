using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class AggressiveEnemy : BaseEnemy
    {
        [Header("Patrol Settings")]
        [SerializeField] private bool shouldPatrol = true;
        [SerializeField] private float patrolRadius = 10f;
        [SerializeField] private float patrolWaitTime = 2f;

        private float patrolWaitTimer;
        private Vector3 patrolTarget;

        protected override void Start()
        {
            base.Start();
            currentState = shouldPatrol ? EnemyState.Patrol : EnemyState.Idle;
            if (shouldPatrol)
            {
                SetNewPatrolTarget();
            }
        }

        protected override void UpdateBehavior(float distanceToPlayer)
        {
            switch (currentState)
            {
                case EnemyState.Idle:
                    HandleIdleState(distanceToPlayer);
                    break;
                case EnemyState.Patrol:
                    HandlePatrolState(distanceToPlayer);
                    break;
                case EnemyState.Chase:
                    HandleChaseState(distanceToPlayer);
                    break;
                case EnemyState.Attack:
                    HandleAttackState(distanceToPlayer);
                    break;
            }
        }

        private void HandleIdleState(float distanceToPlayer)
        {
            if (distanceToPlayer < detectionRange)
            {
                ChangeState(EnemyState.Chase);
            }
        }

        private void HandlePatrolState(float distanceToPlayer)
        {
            if (distanceToPlayer < detectionRange)
            {
                ChangeState(EnemyState.Chase);
                return;
            }

            if (!shouldPatrol) return;

            if (patrolWaitTimer > 0)
            {
                patrolWaitTimer -= Time.deltaTime;
                return;
            }

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                patrolWaitTimer = patrolWaitTime;
                SetNewPatrolTarget();
            }
        }

        private void HandleChaseState(float distanceToPlayer)
        {
            if (distanceToPlayer <= attackRange)
            {
                ChangeState(EnemyState.Attack);
                return;
            }

            if (distanceToPlayer > detectionRange * 1.5f)
            {
                ChangeState(shouldPatrol ? EnemyState.Patrol : EnemyState.Idle);
                return;
            }

            agent.SetDestination(player.position);
        }

        private void HandleAttackState(float distanceToPlayer)
        {
            if (distanceToPlayer > attackRange)
            {
                ChangeState(EnemyState.Chase);
                return;
            }

            agent.ResetPath();
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            directionToPlayer.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToPlayer), Time.deltaTime * 5f);

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
            }
        }

        private void SetNewPatrolTarget()
        {
            Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
            Vector3 randomPoint = startPosition + new Vector3(randomCircle.x, 0, randomCircle.y);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, NavMesh.AllAreas))
            {
                patrolTarget = hit.position;
                agent.SetDestination(patrolTarget);
            }
        }

        protected override void OnStateChanged(EnemyState newState)
        {
            switch (newState)
            {
                case EnemyState.Idle:
                    agent.ResetPath();
                    break;
                case EnemyState.Patrol:
                    agent.speed = 2f;
                    agent.stoppingDistance = 0.5f;
                    if (shouldPatrol) SetNewPatrolTarget();
                    break;
                case EnemyState.Chase:
                    agent.speed = 4f;
                    agent.stoppingDistance = attackRange;
                    break;
                case EnemyState.Attack:
                    agent.stoppingDistance = attackRange * 0.8f;
                    break;
            }
        }

        public override void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
        {
            base.TakeDamage(damage, hitPoint, hitNormal);

            if (IsAlive() && (currentState == EnemyState.Patrol || currentState == EnemyState.Idle))
            {
                ChangeState(EnemyState.Chase);
            }
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            if (shouldPatrol)
            {
                Gizmos.color = Color.blue;
                Vector3 patrolCenter = Application.isPlaying ? startPosition : transform.position;
                Gizmos.DrawWireSphere(patrolCenter, patrolRadius);
            }
        }
    }
}
