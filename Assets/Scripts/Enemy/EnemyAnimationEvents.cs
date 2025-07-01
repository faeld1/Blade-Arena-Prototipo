using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("EnemyAnimationEvents: No Enemy component found in parent.");
        }
    }

    public void AnimationTrigger() => enemy.AnimationTrigger();

    public void AttackHitOnPlayer() => enemy.Attack();
}
