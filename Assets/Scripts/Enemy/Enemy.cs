using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Idle Settings")]
    public float idleTime; // Time to stay idle before moving

    [Header("Move Settings")]
    public float moveSpeed; // Speed at which the enemy moves
    public float turnSpeed; // Speed at which the enemy turns towards the target

    [Header("Attack Settings")]
    public float attackRange; // Range within which the enemy can attack

    private Transform destination; // Destination transform for the enemy to move towards

    public Animator anim { get; private set; } // Reference to the enemy's animator
    public NavMeshAgent agent { get; private set; }
    public CharacterStats stats { get; private set; } // Reference to the enemy's stats
    public EnemyStateMachine stateMachine { get; private set; }

    protected virtual void OnEnable()
    {
        GameManager.OnPlayerDeath += HandlePlayerDeath;
    }

    protected virtual void OnDisable()
    {
        GameManager.OnPlayerDeath -= HandlePlayerDeath;
    }

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        stats = GetComponent<CharacterStats>();
        GameManager.Instance?.RegisterEnemy(this);
        destination = GameManager.Instance?.player?.transform; // Set the destination to the player's position

        agent.speed = moveSpeed; // Set the NavMeshAgent speed to the enemy's move speed
        agent.avoidancePriority = Random.Range(50, 120); // Set a random avoidance priority for the NavMeshAgent
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    private void HandlePlayerDeath()
    {
        OnPlayerDeath();
    }

    protected virtual void OnPlayerDeath()
    {
        // Overridden in derived classes to react to player death
    }

    public Vector3 GetPatrolDestination()
    {
        if (destination != null)
        {
            return destination.position; // Return the player's position as the patrol destination
        }
        return Vector3.zero; // Return zero vector if destination is not set
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();
    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, GameManager.Instance?.player?.transform.position ?? Vector3.zero) <= attackRange;
    
    public void Attack()
    {
        if (GameManager.Instance?.player != null)
        {
            GameManager.Instance.player.GetComponent<CharacterStats>().TakeDamage(stats.attackDamage);
        }
    }

    public virtual void Dead()
    {
    }

    public Quaternion FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        Vector3 currentEulerAngels = transform.rotation.eulerAngles;

        float yRotation = Mathf.LerpAngle(currentEulerAngels.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);

        return Quaternion.Euler(currentEulerAngels.x, yRotation, currentEulerAngels.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange); // Draw a wire sphere to visualize the attack range
    }

}
