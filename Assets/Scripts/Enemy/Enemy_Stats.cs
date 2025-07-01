using UnityEngine;

public class Enemy_Stats : CharacterStats
{
    private Enemy enemy; // Reference to the Enemy component

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<Enemy>(); // Get the Enemy component attached to this GameObject
    }
    protected override void Die()
    {
        base.Die();
        enemy.Dead(); // Call the Dead method on the Enemy component when this character dies
    }
}
