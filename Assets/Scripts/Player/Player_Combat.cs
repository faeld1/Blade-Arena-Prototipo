using UnityEngine;
using Pathfinding;

public class Player_Combat : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 1.5f;
    private float attackTimer;

    private CharacterStats stats;
    private Player_Movement movement;
    private Player player;
    private Player_Skills skills;
    private RichAI agent;
    private bool isAttacking = false;

    private Enemy currentTarget;

    private void Awake()
    {
        agent = GetComponent<RichAI>();
        stats = GetComponent<CharacterStats>();
        movement = GetComponent<Player_Movement>();
        player = GetComponent<Player>();
        skills = GetComponent<Player_Skills>();
    }

    private void Update()
    {
        if (stats.isDead || GameManager.Instance == null)
            return;

        if (!GameManager.Instance.battleOngoing)
            return;

        attackTimer += Time.deltaTime;

        if (isAttacking)
        {
            var stateInfo = player.animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Player_SwordIdle01"))
                ResetAttack();
        }

        TryAttack();
    }

    public void ResetAttack()
    {
        isAttacking = false;
        attackTimer = 0f;
        if (player != null)
            player.animator.SetBool("IsAttacking", false);
    }




    public void ResetCurrentTarget() => currentTarget = null;

    public void SetCurrentTarget(Enemy target)
    {
        if (target == null || target.stats.isDead)
        {
            currentTarget = player.ClosestEnemy();
        }
        else
        {
            currentTarget = target;
        }

        movement.StopMovement();
        agent.isStopped = true;
        agent.SearchPath();
    }

    private void TryAttack()
    {
        if (currentTarget == null || currentTarget.stats.isDead)
        {
            currentTarget = player.ClosestEnemy();
        }

        if (currentTarget == null) return;

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        movement.SetTarget(currentTarget.transform);
        player.transform.rotation = player.FaceTarget(currentTarget.transform.position);

        // Always attempt to use a skill first when possible.
        if (!isAttacking)
        {
            bool usedSkill = skills != null &&
                skills.TryUseNextActiveSkill(currentTarget, attackRange);
            if (usedSkill)
            {
                movement.StopMovement();
                attackTimer = 0f;
                isAttacking = true;
                player.animator.SetBool("IsAttacking", true);
                return;
            }
        }

        if (distance <= attackRange)
        {
            movement.StopMovement();

            if (attackTimer >= stats.attackCooldown && !isAttacking)
            {
                attackTimer = 0f;
                StartAttackAnimation();
                isAttacking = true;
                player.animator.SetBool("IsAttacking", true);
            }
        }
        else
        {
            movement.ResumeMovement();
        }
    }

    public bool IsAttackingEnd()
    {
        if (currentTarget == null)
            player.SetIdle();

        player.animator.SetBool("IsAttacking", false);

        return isAttacking = false;
    }

    private void StartAttackAnimation()
    {
        player.PlayAttack();
    }

    // Chamado via Animation Event no momento do impacto
    public void OnAttackAnimationHit()
    {
        if (currentTarget != null && !currentTarget.stats.isDead)
        {
            bool isCrit;
            float damage = stats.GetDamage(out isCrit);
            currentTarget.stats.TakeDamage(damage);
        }
    }

    // Chamado no final da animação se quiser resetar algo
    public void OnAttackAnimationEnd()
    {
        // Pode ser usado para controlar combo, cooldowns, etc
    }
}
