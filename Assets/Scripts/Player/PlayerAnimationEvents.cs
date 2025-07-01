using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private Player player;
    private Player_Combat combat;

    private void Start()
    {
        player = GetComponentInParent<Player>();
        combat = GetComponentInParent<Player_Combat>();
    }

    public void AttackHitOnEnemy()
    {
        combat.OnAttackAnimationHit();
    }

    public void AttackAnimationEnd()
    {
        combat.IsAttackingEnd();
    }
}
