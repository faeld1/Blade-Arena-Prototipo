using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator animator { get; set; } // Reference to the player's animator
    public CharacterStats Stats { get; private set; } // Reference to the player's stats
    private Player_Movement movement;

    public Enemy CurrentEnemyTarget { get; set; } // The current target enemy for the player

    [SerializeField] private float turnSpeed; // Speed at which the player turns towards the target

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        Stats = GetComponent<CharacterStats>();
        movement = GetComponent<Player_Movement>();
    }

    private void Update()
    {
        if (Stats.isDead)
        {
            SetIdle();
            return;
        }

        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        bool isMoving = movement.IsMoving();

        // Só muda o bool se ele for diferente do valor atual
        if (animator.GetBool("IsWalking") != isMoving)
        {
            animator.SetBool("IsWalking", isMoving);
        }
    }

    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void SetIdle()
    {
        animator.SetBool("IsWalking", false);
    }

    private void GetClosestEnemy()
    {
        var enemies = GameManager.Instance.activeEnemies;
        float closestDist = Mathf.Infinity;
        Enemy closest = null;

        foreach (var enemy in enemies)
        {
            if (enemy == null || enemy.stats.isDead) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = enemy;
            }
        }

        CurrentEnemyTarget = closest;
    }

    public Enemy ClosestEnemy()
    {
        if (CurrentEnemyTarget == null || CurrentEnemyTarget.stats.isDead)
        {
            GetClosestEnemy();
        }
        return CurrentEnemyTarget;
    }

    public void SetManualEnemyTarget() => GetClosestEnemy();

    public void ClosestEnemyNull() => CurrentEnemyTarget = null;

    public Quaternion FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        Vector3 currentEulerAngels = transform.rotation.eulerAngles;

        float yRotation = Mathf.LerpAngle(currentEulerAngels.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);

        return Quaternion.Euler(currentEulerAngels.x, yRotation, currentEulerAngels.z);
    }
}
