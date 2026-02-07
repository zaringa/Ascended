using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class StrafeEnemy : BaseEnemy
    {
        public enum StrafeAxis { Forward_Backward, Left_Right }

        [Header("Strafe Settings")]
        [SerializeField] private StrafeAxis strafeAxis = StrafeAxis.Forward_Backward;
        [SerializeField] private float strafeDistance = 5f;
        [SerializeField] private float strafeSpeed = 3f;

        private Vector3 strafeTarget;
        private int strafeDirection = 1;

        protected override void Start()
        {
            base.Start();
            currentState = EnemyState.Strafe;
            agent.speed = strafeSpeed;
            SetNewStrafeTarget();
        }

        protected override void UpdateBehavior(float distanceToPlayer)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                strafeDirection = Random.value > 0.5f ? 1 : -1;

                if (Random.value > 0.7f)
                {
                    strafeAxis = strafeAxis == StrafeAxis.Forward_Backward ? StrafeAxis.Left_Right : StrafeAxis.Forward_Backward;
                }

                SetNewStrafeTarget();
            }
        }

        private void SetNewStrafeTarget()
        {
            Vector3 direction = Vector3.zero;

            switch (strafeAxis)
            {
                case StrafeAxis.Forward_Backward:
                    direction = transform.forward * strafeDirection;
                    break;
                case StrafeAxis.Left_Right:
                    direction = transform.right * strafeDirection;
                    break;
            }

            Vector3 targetPoint = transform.position + direction * strafeDistance;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPoint, out hit, strafeDistance, NavMesh.AllAreas))
            {
                strafeTarget = hit.position;
                agent.SetDestination(strafeTarget);
            }
            else
            {
                strafeDirection *= -1;
                SetNewStrafeTarget();
            }
        }

        protected override void OnStateChanged(EnemyState newState)
        {
            if (newState == EnemyState.Strafe)
            {
                agent.speed = strafeSpeed;
                agent.stoppingDistance = 0.5f;
                SetNewStrafeTarget();
            }
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Gizmos.color = Color.cyan;
            Vector3 strafeDir = Vector3.zero;

            switch (strafeAxis)
            {
                case StrafeAxis.Forward_Backward:
                    strafeDir = transform.forward;
                    break;
                case StrafeAxis.Left_Right:
                    strafeDir = transform.right;
                    break;
            }

            Vector3 lineStart = transform.position - strafeDir * strafeDistance;
            Vector3 lineEnd = transform.position + strafeDir * strafeDistance;
            Gizmos.DrawLine(lineStart, lineEnd);

            Gizmos.DrawWireSphere(lineStart, 0.2f);
            Gizmos.DrawWireSphere(lineEnd, 0.2f);
        }
    }
}
