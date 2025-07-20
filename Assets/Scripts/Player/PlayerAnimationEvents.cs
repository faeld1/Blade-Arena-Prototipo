using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private Player player;
    private Player_Combat combat;
    private Player_Skills skills;

    private void Start()
    {
        player = GetComponentInParent<Player>();
        combat = GetComponentInParent<Player_Combat>();
        skills = GetComponentInParent<Player_Skills>();
    }

    public void AttackHitOnEnemy()
    {
        combat.OnAttackAnimationHit();
    }

    public void AttackAnimationEnd()
    {
        combat.IsAttackingEnd();
    }

    public void SkillEffect()
    {
        skills?.ActivatePendingSkill();
    }

    public void SkillAnimationEffectEnd()
    {
        combat.IsAttackingEnd();
    }
}
