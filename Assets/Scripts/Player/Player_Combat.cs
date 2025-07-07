using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 1.5f;
    private float attackTimer;

    private CharacterStats stats;
    private Player_Movement movement;
    private Player player;

    private bool isAttacking = false;

    private Enemy currentTarget;
    private void Start()
    {
        stats = GetComponent<CharacterStats>();
        movement = GetComponent<Player_Movement>();
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (stats.isDead || GameManager.Instance == null) return;

        attackTimer += Time.deltaTime;

        TryAttack();
    }

    public void ResetCurrentTarget() => currentTarget = null;

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

        if (distance <= attackRange)
        {
            movement.StopMovement();

            if (attackTimer >= stats.attackCooldown && isAttacking == false)
            {
                attackTimer = 0;
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
