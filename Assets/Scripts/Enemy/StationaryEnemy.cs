using UnityEngine;

namespace Enemy
{
    public class StationaryEnemy : BaseEnemy
    {
        protected override void Start()
        {
            base.Start();
            currentState = EnemyState.Idle;
            agent.speed = 0;
        }

        protected override void UpdateBehavior(float distanceToPlayer)
        {
            switch (currentState)
            {
                case EnemyState.Idle:
                    HandleIdleState(distanceToPlayer);
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
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                directionToPlayer.y = 0;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToPlayer), Time.deltaTime * 3f);

                if (distanceToPlayer <= attackRange)
                {
                    ChangeState(EnemyState.Attack);
                }
            }
        }

        private void HandleAttackState(float distanceToPlayer)
        {
            if (distanceToPlayer > attackRange)
            {
                ChangeState(EnemyState.Idle);
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

        protected override void OnStateChanged(EnemyState newState)
        {
            if (newState == EnemyState.Idle)
            {
                agent.ResetPath();
                agent.speed = 0;
            }
        }

        public override void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
        {
            base.TakeDamage(damage, hitPoint, hitNormal);

            if (IsAlive() && player != null)
            {
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(directionToPlayer);
            }
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Gizmos.color = Color.magenta;
            Vector3 pos = transform.position + Vector3.up * 0.1f;
            Gizmos.DrawLine(pos - Vector3.right, pos + Vector3.right);
            Gizmos.DrawLine(pos - Vector3.forward, pos + Vector3.forward);
        }
    }
}
