using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class BaseEnemy : MonoBehaviour, IDamageable, IEnemy
    {
        [Header("Health")]
        [SerializeField] protected float maxHealth = 100f;
        [SerializeField] protected float currentHealth;

        [Header("Visual Feedback")]
        [SerializeField] protected Renderer meshRenderer;
        [SerializeField] protected float damageFlashDuration = 0.1f;
        [SerializeField] protected Color damageFlashColor = Color.red;

        protected Material materialInstance;
        protected Color originalColor;

        [Header("AI Parameters")]
        [SerializeField] protected float detectionRange = 15f;
        [SerializeField] protected float attackRange = 2f;
        [SerializeField] protected float attackCooldown = 1.5f;
        [SerializeField] protected float damage = 10f;

        [Header("Effects")]
        [SerializeField] protected GameObject deathEffectPrefab;
        [SerializeField] protected AudioClip attackSound;
        [SerializeField] protected AudioClip deathSound;
        [SerializeField] protected AudioClip hurtSound;

        protected NavMeshAgent agent;
        protected Transform player;
        protected AudioSource audioSource;
        protected float lastAttackTime;
        protected Vector3 startPosition;
        protected bool isDead;

        protected enum EnemyState { Patrol, Chase, Flee, Idle, Strafe, Attack, Dead }
        protected EnemyState currentState = EnemyState.Idle;

        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1f;
            }
        }

        protected virtual void Start()
        {
            currentHealth = maxHealth;
            startPosition = transform.position;
            InitializeMaterial();
            FindPlayer();
        }

        protected virtual void Update()
        {
            if (isDead) return;
            if (player == null) return;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            UpdateBehavior(distanceToPlayer);
        }

        protected abstract void UpdateBehavior(float distanceToPlayer);

        protected void InitializeMaterial()
        {
            if (meshRenderer == null)
            {
                meshRenderer = GetComponentInChildren<Renderer>();
            }

            if (meshRenderer != null)
            {
                materialInstance = meshRenderer.material;
                originalColor = materialInstance.color;
            }
        }

        protected void FindPlayer()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj == null)
            {
                playerObj = GameObject.FindObjectOfType<PlayerController>()?.gameObject;
            }

            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        protected void AttackPlayer()
        {
            PlaySound(attackSound);

            IDamageable playerDamageable = player.GetComponent<IDamageable>();
            if (playerDamageable != null)
            {
                Vector3 hitPoint = player.position;
                Vector3 hitNormal = (player.position - transform.position).normalized;
                playerDamageable.TakeDamage(damage, hitPoint, hitNormal);
            }
        }

        public virtual void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
        {
            if (!IsAlive()) return;

            currentHealth -= damage;

            if (materialInstance != null)
            {
                StopAllCoroutines();
                StartCoroutine(FlashDamage());
            }

            PlaySound(hurtSound);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public bool IsAlive()
        {
            return currentHealth > 0 && !isDead;
        }

        public virtual void ApplyExplosionDamage(float dmg)
        {
            TakeDamage(dmg, transform.position, Vector3.up);
        }

        protected virtual void Die()
        {
            isDead = true;
            currentState = EnemyState.Dead;

            agent.enabled = false;
            PlaySound(deathSound);

            if (deathEffectPrefab != null)
            {
                Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject, 0.1f);
        }

        protected System.Collections.IEnumerator FlashDamage()
        {
            if (materialInstance == null) yield break;

            materialInstance.color = damageFlashColor;
            yield return new WaitForSeconds(damageFlashDuration);
            materialInstance.color = originalColor;
        }

        protected void PlaySound(AudioClip clip)
        {
            if (clip != null && audioSource != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        protected void ChangeState(EnemyState newState)
        {
            if (currentState == newState) return;
            currentState = newState;
            OnStateChanged(newState);
        }

        protected virtual void OnStateChanged(EnemyState newState) { }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            if (Application.isPlaying)
            {
                Gizmos.color = Color.green;
                Vector3 healthBarPos = transform.position + Vector3.up * 2.5f;
                float healthPercent = currentHealth / maxHealth;
                Gizmos.DrawLine(healthBarPos - Vector3.right * 0.5f, healthBarPos + Vector3.right * (healthPercent - 0.5f));
            }
        }
    }
}
